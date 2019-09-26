/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System;
using System.Collections.Generic;

using extOSC.Core;
using extOSC.Core.Network;
using UnityEngine.Serialization;

namespace extOSC
{
    [AddComponentMenu("extOSC/OSC Transmitter")]
    public class OSCTransmitter : OSCBase
    {
        #region Public Vars

		[Obsolete("Use IsStarted property.")]
		public bool IsAvailable => IsStarted;

		public override bool IsStarted => _transmitterBackend.IsAvailable;


		public OSCLocalHostMode LocalHostMode
		{
			get => _localHostMode;
			set
			{
				if (_localHostMode == value)
					return;

				_localHostMode = value;

				LocalRefresh();
            }
		}

		public OSCLocalPortMode LocalPortMode
		{
		    get => _localPortMode;
			set
			{
				if (_localPortMode == value)
					return;

				_localPortMode = value;

				LocalRefresh();
            }
		}

		public OSCReceiver SourceReceiver
		{
			get => _localReceiver;
			set
			{
				if (_localReceiver == value)
					return;

				_localReceiver = value;

				LocalRefresh();
			}
		}

		public string LocalHost
        {
            get => RequestLocalHost();
			set
            {
                if (_localHost == value)
                    return;

				_localHost = value;

				LocalRefresh();
            }
        }

		public int LocalPort
		{
		    get => RequestLocalPort();
			set 
			{
				if (_localPort == value)
					return;

				_localPort = value;

				LocalRefresh();
            }
		}

        public string RemoteHost
        {
            get => _remoteHost;
			set
            {
                if (_remoteHost == value)
                    return;

				_remoteHost = value;

				RemoteRefresh();
            }
        }

        public int RemotePort
        {
            get => _remotePort;
			set
            {
                value = OSCUtilities.ClampPort(value);

                if (_remotePort == value)
                    return;

				_remotePort = value;

				RemoteRefresh();
            }
        }

        public bool UseBundle
        {
            get => _useBundle;
			set => _useBundle = value;
		}

        #endregion

		#region Private Vars

		[SerializeField]
		[FormerlySerializedAs("localHostMode")]
		private OSCLocalHostMode _localHostMode = OSCLocalHostMode.Any;

		[SerializeField]
		[FormerlySerializedAs("localPortMode")]
		private OSCLocalPortMode _localPortMode = OSCLocalPortMode.Random;

		[OSCSelector]
		[SerializeField]
		[FormerlySerializedAs("localReceiver")]
		private OSCReceiver _localReceiver;

		[OSCHost]
		[SerializeField]
		[FormerlySerializedAs("localHost")]
		private string _localHost;

		[SerializeField]
		[FormerlySerializedAs("localPort")]
		private int _localPort = 7000;

		[OSCHost]
		[SerializeField]
		[FormerlySerializedAs("remoteHost")]
		private string _remoteHost = "127.0.0.1";

		[SerializeField]
		[FormerlySerializedAs("remotePort")]
		private int _remotePort = 7000;

		[SerializeField]
		[FormerlySerializedAs("useBundle")]
		private bool _useBundle;

		private readonly List<IOSCPacket> _bundleBuffer = new List<IOSCPacket>();
		
        private OSCTransmitterBackend _transmitterBackend => __transmitterBackend ?? (__transmitterBackend = OSCTransmitterBackend.Create());

		private OSCTransmitterBackend __transmitterBackend;

        #endregion

        #region Unity Methods

        protected virtual void Update()
        {
            if (_bundleBuffer.Count > 0)
            {
                var bundle = new OSCBundle();

                foreach (var packet in _bundleBuffer)
                {
                    bundle.AddPacket(packet);
                }

                Send(bundle);

                _bundleBuffer.Clear();
            }
        }

#if UNITY_EDITOR
		protected void OnValidate()
		{
			_remotePort = OSCUtilities.ClampPort(_remotePort);

		    if (string.IsNullOrEmpty(_localHost))
		        _localHost = OSCUtilities.GetLocalHost();

            if (_localPort > 0)
				_localPort = OSCUtilities.ClampPort(_localPort);

			_transmitterBackend.RefreshRemote(_remoteHost, _remotePort);

			if (IsStarted)
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
            _transmitterBackend.Connect(RequestLocalHost(), RequestLocalPort());
            _transmitterBackend.RefreshRemote(_remoteHost, _remotePort);
        }

        public override void Close()
        {
            if (_transmitterBackend.IsAvailable)
                _transmitterBackend.Close();
        }

        public override string ToString()
        {
            return string.Format("<{0} (LocalHost: {1} LocalPort: {2} | RemoteHost: {3}, RemotePort: {4})>",
                GetType().Name, _localHost, _localPort, _remoteHost, _remotePort);
        }

        public void Send(IOSCPacket ioscPacket)
        {
            if (_useBundle && ioscPacket != null && (ioscPacket is OSCMessage))
            {
                _bundleBuffer.Add(ioscPacket);

                return;
            }

 			if (!_transmitterBackend.IsAvailable)
				return; 

			if (mapBundle != null)
				mapBundle.Map(ioscPacket);

            var length = 0;
            var buffer = OSCConverter.Pack(ioscPacket, out length);

            _transmitterBackend.Send(buffer, length);

            OSCConsole.Transmitted(this, ioscPacket);
        }

        public virtual void Send(string address, OSCValue value)
        {
            var message = new OSCMessage(address);
            message.AddValue(value);

            Send(message);
        }

        #endregion

        #region Private Methods

		private void LocalRefresh()
		{
			if (IsStarted)
			{
				Close();
				Connect();
			}
        }

		private void RemoteRefresh()
		{
			_transmitterBackend.RefreshRemote(_remoteHost, _remotePort);
		}

        private string RequestLocalHost()
        {
            if (_localReceiver != null)
                return _localReceiver.LocalHost;

            if (_localHostMode == OSCLocalHostMode.Any)
                return "0.0.0.0";

            return _localHost;
        }

        private int RequestLocalPort()
        {
            if (_localReceiver != null)
                return _localReceiver.LocalPort;
            
            if (_localPortMode == OSCLocalPortMode.Random)
                return 0;

            if (_localPortMode == OSCLocalPortMode.FromReceiver)
                throw new Exception("[OSCTransmitter] Local Port Mode does not support \"FromReceiver\" option.");

            if (_localPortMode == OSCLocalPortMode.Custom)
                return _localPort;

            return _remotePort;
        }

        #endregion
    }
}