/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.Events;

using System.Threading;
using System.Collections.Generic;

using extOSC.Core;
using extOSC.Core.Console;
using extOSC.Core.Network;

namespace extOSC
{
    [AddComponentMenu("extOSC/OSC Receiver")]
    public class OSCReceiver : OSCBase
    {
        #region Extensions

        public delegate void MessageReceiveDelegate(OSCMessage message);

        #endregion

        #region Public Vars

        public int LocalPort
        {
            get { return localPort; }
            set
            {
                value = OSCUtilities.ClampPort(value);

                if (localPort == value)
                    return;

				localPort = value;

                if (receiverBackend.IsRunning && IsAvaible)
                {
                    Close();
                    Connect();
                }
            }
        }

        public override bool IsAvaible
        {
            get
            {
                return receiverBackend.IsAvaible;
            }
        }

        public bool IsRunning
        {
            get
            {
                return enabled ? false : receiverBackend.IsRunning;
            }
        }

        #endregion

        #region Protected Vars

        [SerializeField]
        protected int localPort = 7001;

        protected Thread thread;

        protected Queue<OSCPacket> packets = new Queue<OSCPacket>();

        protected List<IOSCBind> bindings = new List<IOSCBind>();

        protected OSCReceiverBackend receiverBackend
        {
            get
            {
                if (_receiverBackend == null)
                {
                    _receiverBackend = OSCReceiverBackend.Create();
                    _receiverBackend.ReceivedCallback = PacketReceived;
                }

                return _receiverBackend;
            }
        }

        #endregion

        #region Private Vars

        private object _lock = new object();

        private OSCReceiverBackend _receiverBackend;

        #endregion

        #region Unity Methods

        protected virtual void Update()
        {
            if (!IsAvaible || !receiverBackend.IsRunning) return;

            lock (_lock)
            {
                while (packets.Count > 0)
                {
                    var packet = packets.Dequeue();

                    if (mapBundle != null)
                        mapBundle.Map(packet);

                    OSCConsole.Received(this, packet);

                    InvokePacket(packet);
                }
            }
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            localPort = OSCUtilities.ClampPort(localPort);

            if (receiverBackend.IsRunning && IsAvaible)
            {
                Close();
                Connect();
            }
        }
#endif

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return string.Format("<{0} (Port: {1})>", GetType().Name, localPort);
        }

        public override void Connect()
        {
            receiverBackend.Connect(localPort);
        }

        public override void Close()
        {
            receiverBackend.Close();
        }

        public void Bind(IOSCBind bind)
        {
            if (bind == null) return;

            if (string.IsNullOrEmpty(bind.ReceiverAddress))
            {
                Debug.Log("[OSCReceiver] Address can not be empty!");
                return;
            }

            if (!bindings.Contains(bind))
                bindings.Add(bind);
        }

        public OSCBind Bind(string address, UnityAction<OSCMessage> callback)
        {
            var bind = new OSCBind(address, callback);

            Bind(bind);

            return bind;
        }

        public void Unbind(IOSCBind bind)
        {
            if (bind == null) return;

            if (bindings.Contains(bind))
                bindings.Remove(bind);
        }

        [System.Obsolete("\"FakeReceive(OSCPacket)\" is deprecated. OSC Debug now use reflection.")]
        public void FakeReceive(OSCPacket packet)
        {
            lock (_lock)
            {
                packets.Enqueue(packet);
            }
        }

        public void UnbindAll()
        {
            bindings.Clear();
        }

        #endregion

        #region Protected Methods

        protected void InvokePacket(OSCPacket packet)
        {
            if (packet.IsBundle())
            {
                InvokeBundle(packet as OSCBundle);
            }
            else
            {
                InvokeMessage(packet as OSCMessage);
            }
        }

        protected void InvokeBundle(OSCBundle bundle)
        {
            if (bundle == null) return;

            foreach (var packet in bundle.Packets)
            {
                InvokePacket(packet);
            }
        }

        protected void InvokeMessage(OSCMessage message)
        {
            if (message == null) return;

            foreach (var bind in bindings)
            {
                if (bind == null) continue;

                if (OSCUtilities.CompareAddresses(bind.ReceiverAddress, message.Address))
                {
                    if (bind.Callback != null)
                        bind.Callback.Invoke(message);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            UnbindAll();
        }

        protected virtual void PacketReceived(OSCPacket packet)
        {
            lock (_lock)
            {
                packets.Enqueue(packet);
            }
        }

        #endregion
    }
}