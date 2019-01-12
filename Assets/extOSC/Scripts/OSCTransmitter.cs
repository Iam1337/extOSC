/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System.Collections.Generic;

using extOSC.Core;
using extOSC.Core.Console;
using extOSC.Core.Network;

namespace extOSC
{
    [AddComponentMenu("extOSC/OSC Transmitter")]
    public class OSCTransmitter : OSCBase
    {
        #region Public Vars

        public override bool IsAvailable
        {
            get
            {
                if (transmitterBackend != null)
                    return transmitterBackend.IsAvailable;

                return false;
            }
        }

		public OSCLocalPortMode LocalPortMode
		{
		    get { return localPortMode; }
			set
			{
				if (localPortMode == value)
					return;

				localPortMode = value;

				if (IsAvailable)
				{
					Close();
					Connect();
				}
			}
		}

		public OSCReceiver LocalReceiver
		{
			get { return localReceiver; }
			set 
			{
				if (localReceiver == value)
					return;

				localReceiver = value;

				if (IsAvailable && localPortMode == OSCLocalPortMode.FromReceiver)
				{
					Close();
					Connect();
				}
			}
		}

		public int LocalPort
		{
		    get
		    {
                return RequestLocalPort();
            }
			set 
			{
				if (localPort == value)
					return;

				localPort = value;

				if (IsAvailable && localPortMode == OSCLocalPortMode.Custom)
				{
					Close();
					Connect();
				}
			}
		}

        public string RemoteHost
        {
            get { return remoteHost; }
            set
            {
                if (remoteHost == value)
                    return;

				remoteHost = value;

                transmitterBackend.RefreshConnection(remoteHost, remotePort);

                if (IsAvailable && localPortMode == OSCLocalPortMode.FromRemotePort)
                {
                    Close();
                    Connect();
                }
            }
        }

        public int RemotePort
        {
            get { return remotePort; }
            set
            {
                value = OSCUtilities.ClampPort(value);

                if (remotePort == value)
                    return;

				remotePort = value;

				transmitterBackend.RefreshConnection(remoteHost, remotePort);
            }
        }

        public bool UseBundle
        {
            get { return useBundle; }
            set { useBundle = value; }
        }

		#endregion

		#region Protected Vars

		[SerializeField]
		protected OSCLocalPortMode localPortMode = OSCLocalPortMode.Random;

		[SerializeField]
		protected OSCReceiver localReceiver;

		[SerializeField]
		protected int localPort = 7000;

        [SerializeField]
        protected string remoteHost = "127.0.0.1";

        [SerializeField]
        protected int remotePort = 7000;

        [SerializeField]
        protected bool useBundle;

        protected OSCTransmitterBackend transmitterBackend
        {
            get
            {
                if (_transmitterBackend == null)
                    _transmitterBackend = OSCTransmitterBackend.Create();

                return _transmitterBackend;
            }
        }

        #endregion

        #region Private Vars

        private readonly List<OSCPacket> _packetPool = new List<OSCPacket>();

        private OSCTransmitterBackend _transmitterBackend;

        #endregion

        #region Unity Methods

        protected virtual void Update()
        {
            if (_packetPool.Count > 0)
            {
                var bundle = new OSCBundle();

                foreach (var packet in _packetPool)
                {
                    bundle.AddPacket(packet);
                }

                Send(bundle);

                _packetPool.Clear();
            }
        }

#if UNITY_EDITOR
		protected void OnValidate()
		{
			remotePort = OSCUtilities.ClampPort(remotePort);

			if (localPort > 0)
				localPort = OSCUtilities.ClampPort(localPort);

			transmitterBackend.RefreshConnection(remoteHost, remotePort);

			if (IsAvailable)
			{
				Close();
				Connect();
			}
		}
#endif

        #endregion

        #region Public Methods

        public override void Connect()
        {
            transmitterBackend.Connect(RequestLocalPort(), remoteHost, remotePort);
        }

        public override void Close()
        {
            transmitterBackend.Close();
        }

        public override string ToString()
        {
            return string.Format("<{0} (Host: {1}, Port: {2})>", GetType().Name, remoteHost, remotePort);
        }

        public void Send(OSCPacket packet)
        {
            if (useBundle && packet != null && (packet is OSCMessage))
            {
                _packetPool.Add(packet);

                return;
            }

 			if (!transmitterBackend.IsAvailable)
				return; 

			if (mapBundle != null)
				mapBundle.Map(packet);

            var length = 0;
            var buffer = OSCConverter.Pack(packet, out length);

            transmitterBackend.Send(buffer, length);

            OSCConsole.Transmitted(this, packet);
        }

        public virtual void Send(string address, OSCValue value)
        {
            var message = new OSCMessage(address);
            message.AddValue(value);

            Send(message);
        }

        #endregion

        #region Private Methods

        private int RequestLocalPort()
        {
            if (localPortMode == OSCLocalPortMode.Random)
                return 0;

            if (localPortMode == OSCLocalPortMode.FromReceiver && localReceiver != null)
                return localReceiver.LocalPort;

            if (localPortMode == OSCLocalPortMode.Custom)
                return localPort;

            return remotePort;
        }

        #endregion
    }
}