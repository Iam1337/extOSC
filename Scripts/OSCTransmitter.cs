/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System;
using System.Collections.Generic;

using extOSC.Core;
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

        public OSCLocalHostMode LocalHostMode 
        {
            get { return localHostMode; }
            set {
                if (localHostMode == value)
                    return;

                localHostMode = value;

                if (IsAvailable)
                {
                    Close();
                    Connect();
                }
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

        public OSCReceiver SourceReceiver
        {
            get { return localReceiver; }
            set {
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
        
        [Obsolete("\"LocalReceiver\" is deprecated. Use \"SourceReceiver\" property.")]
		public OSCReceiver LocalReceiver
		{
		    get { return SourceReceiver; }
		    set { SourceReceiver = value; }
		}

        public string LocalHost
        {
            get { return RequestLocalHost(); }
            set
            {
                if (localHost == value)
                    return;

                if (IsAvailable &&
                    localPortMode == OSCLocalPortMode.Custom &&
                    localHostMode == OSCLocalHostMode.Custom)
                {
                    Close();
                    Connect();
                }
            }
        }

		public int LocalPort
		{
		    get { return RequestLocalPort(); }
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

                transmitterBackend.RefreshRemote(remoteHost, remotePort);

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

				transmitterBackend.RefreshRemote(remoteHost, remotePort);
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
        protected OSCLocalHostMode localHostMode = OSCLocalHostMode.Any;

        [SerializeField]
		protected OSCLocalPortMode localPortMode = OSCLocalPortMode.Random;

		[OSCSelector]
		[SerializeField]
		protected OSCReceiver localReceiver;

	    [OSCHost]
		[SerializeField]
        protected string localHost;

		[SerializeField]
		protected int localPort = 7000;

		[OSCHost]
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

		    if (string.IsNullOrEmpty(localHost))
		        localHost = OSCUtilities.GetLocalHost();

            if (localPort > 0)
				localPort = OSCUtilities.ClampPort(localPort);

			transmitterBackend.RefreshRemote(remoteHost, remotePort);

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
            transmitterBackend.Connect(RequestLocalHost(), RequestLocalPort());
            transmitterBackend.RefreshRemote(remoteHost, remotePort);
        }

        public override void Close()
        {
            if (transmitterBackend.IsAvailable)
                transmitterBackend.Close();
        }

        public override string ToString()
        {
            return string.Format("<{0} (LocalHost: {1} LocalPort: {2} | RemoteHost: {3}, RemotePort: {4})>",
                GetType().Name, localHost, localPort, remoteHost, remotePort);
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

        private string RequestLocalHost()
        {
            if (localReceiver != null)
                return localReceiver.LocalHost;

            if (localHostMode == OSCLocalHostMode.Any)
                return "0.0.0.0";

            return localHost;
        }

        private int RequestLocalPort()
        {
            if (localReceiver != null)
                return localReceiver.LocalPort;
            
            if (localPortMode == OSCLocalPortMode.Random)
                return 0;

            if (localPortMode == OSCLocalPortMode.FromReceiver)
                throw new Exception("[OSCTransmitter] Local Port Mode does not support \"FromReceiver\" option.");

            if (localPortMode == OSCLocalPortMode.Custom)
                return localPort;

            return remotePort;
        }

        #endregion
    }
}