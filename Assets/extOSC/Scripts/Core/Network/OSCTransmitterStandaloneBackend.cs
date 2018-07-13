/* Copyright (c) 2018 ExT (V.Sigalkin) */

#if !NETFX_CORE
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System;

namespace extOSC.Core.Network
{
    public class OSCTransmitterStandaloneBackend : OSCTransmitterBackend
    {
        #region Public Vars

        public override bool IsAvaible
        {
            get { return _client != null; }
        }

        #endregion

        #region Private Vars

		private IPEndPoint _remoteEndPoint;

		private IPEndPoint _localEndPoint;

        private UdpClient _client;

        #endregion

        #region Public Methods

		public override void Connect(int localPort, string remoteHost, int remotePort)
        {
            if (_client != null)
                Close();

			_localEndPoint = OSCStandaloneManager.CreateLocalEndPoint(localPort);
			_remoteEndPoint = OSCStandaloneManager.CreateRemoteEndPoint(remoteHost, remotePort);

			try
			{
				_client = OSCStandaloneManager.CreateClient(_localEndPoint);
			}
			catch (SocketException e)
			{
				if (e.ErrorCode == 10048)
				{
					Debug.LogErrorFormat(
						"[OSCTransmitter] Socket Error: Could not use local port {0} because another application is listening on it.",
						localPort);
				}
				else
				{
					Debug.LogErrorFormat("[OSCTransmitter] Socket Error: Error Code {0}.\n{1}", e.ErrorCode, e.Message);
				}

				Close();
			}
			catch (ArgumentOutOfRangeException)
			{
				Debug.LogErrorFormat("[OSCTransmitter] Invalid port: {0}", localPort);

				Close();
			}
			catch (Exception e)
			{
				Debug.LogErrorFormat("[OSCTransmitter] Error: {0}", e);

				Close();
			}
        }

		public override void RefreshConnection(string remoteHost, int remotePort)
        {
			_remoteEndPoint = OSCStandaloneManager.CreateRemoteEndPoint(remoteHost, remotePort);
        }

        public override void Close()
        {
			OSCStandaloneManager.RemoveClient(_client);

            _client = null;
        }

        public override void Send(byte[] data)
        {
            if (_client == null) return;

            try
            {
                _client.Send(data, data.Length, _remoteEndPoint);
            }
            catch (SocketException exception)
            {
                Debug.LogWarningFormat("[OSCTranmitter] Error: {0}", exception);
            }
        }

        #endregion
		/*
		// Some old code.
        #region Private Methods

        private IPEndPoint CreateEndPoint(string remoteHost, int remotePort, bool showWarning = true)
        {
            IPAddress ipAddress;

            if (!IPAddress.TryParse(remoteHost, out ipAddress))
            {
                if (showWarning) Debug.LogWarningFormat("[OSCTranmitter] Invalid IP address: {0}.", remoteHost);
                return null;
            }

            return new IPEndPoint(ipAddress, remotePort);
        }

        #endregion
		*/
    }
}
#endif