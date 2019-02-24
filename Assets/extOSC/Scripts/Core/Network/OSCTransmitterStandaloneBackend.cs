/* Copyright (c) 2019 ExT (V.Sigalkin) */

#if !NETFX_CORE

using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;

namespace extOSC.Core.Network
{
    internal class OSCTransmitterStandaloneBackend : OSCTransmitterBackend
    {
        #region Public Vars

        public override bool IsAvailable
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

		public override void Connect(string localHost, int localPort)
        {
            if (_client != null)
                Close();

			try
			{
				_client = OSCStandaloneManager.Create(localHost, localPort);
            }
			catch (SocketException e)
			{
				if (e.ErrorCode == 10048)
				{
					Debug.LogErrorFormat("[OSCTransmitter] Socket Error: Could not use local port {0} because another application is listening on it.",
						localPort);
				}
                else if (e.ErrorCode == 10049)
			    {
			        Debug.LogErrorFormat("[OSCTransmitter] Socket Error: Could not use local host \"{0}\". Cannot assign requested address.",
			            localHost);
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

		public override void RefreshRemote(string remoteHost, int remotePort)
        {
			_remoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteHost), remotePort);
        }

        public override void Close()
        {
            if (_client != null)
			    OSCStandaloneManager.Close(_client);

            _client = null;
        }

        public override void Send(byte[] data, int length)
        {
            if (_client == null) return;

            try
            {
                _client.Send(data, length, _remoteEndPoint);
            }
            catch (SocketException exception)
            {
                Debug.LogWarningFormat("[OSCTransmitter] Error: {0}", exception);
            }
        }

        #endregion
    }
}

#endif