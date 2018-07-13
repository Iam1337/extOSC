/* Copyright (c) 2018 ExT (V.Sigalkin) */

#if !NETFX_CORE
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;

namespace extOSC.Core.Network
{
    public class OSCReceiverStandaloneBackend : OSCReceiverBackend
    {
        #region Public Vars

        public override OSCReceivedCallback ReceivedCallback
        {
            get { return _receivedCallback; }
            set { _receivedCallback = value; }
        }

        public override bool IsAvaible
        {
            get { return _client != null; }
        }

        public override bool IsRunning
        {
            get { return _isRunning; }
        }

        #endregion

        #region Private Vars

        private bool _isRunning;

        private UdpClient _client;

		private IPEndPoint _localEndPoint;

        private AsyncCallback _controllerThreadAsync;

        private OSCReceivedCallback _receivedCallback;

        #endregion

        #region Public Methods

        public override void Connect(int localPort)
        {
            if (_client != null)
                Close();

            try
            {
				_localEndPoint = OSCStandaloneManager.CreateLocalEndPoint(localPort);

				_client = OSCStandaloneManager.CreateClient(_localEndPoint);

                _controllerThreadAsync = new AsyncCallback(ControllerThread);
                _client.BeginReceive(_controllerThreadAsync, _client);

                _isRunning = true;
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10048)
                {
                    Debug.LogErrorFormat(
                        "[OSCReceiver] Socket Error: Could not use port {0} because another application is listening on it.",
                        localPort);
                }
                else
                {
                    Debug.LogErrorFormat("[OSCReceiver] Socket Error: Error Code {0}.\n{1}", e.ErrorCode, e.Message);
                }

                Close();
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.LogErrorFormat("[OSCReceiver] Invalid port: {0}", localPort);

                Close();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("[OSCReceiver] Error: {0}", e);

                Close();
            }
        }

        public override void Close()
        {
            _isRunning = false;

			OSCStandaloneManager.RemoveClient(_client);

            _client = null;
        }

        #endregion

        #region Protected Methods

        protected void ControllerThread(IAsyncResult result)
        {
            if (!_isRunning) return;

            try
            {
                var receivedClient = result.AsyncState as UdpClient;
                if (receivedClient == null) return;

                var packet = Receive(receivedClient, result);
                if (packet != null)
                {
                    if (_receivedCallback != null)
                        _receivedCallback.Invoke(packet);
                }

                if (IsAvaible)
                    receivedClient.BeginReceive(_controllerThreadAsync, receivedClient);
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception e)
            {
                Debug.LogErrorFormat("[OSCReceiver] Error: " + e);
            }
        }

        protected OSCPacket Receive(UdpClient receivedClient, IAsyncResult result)
        {
            try
            {
                IPEndPoint ip = null;
                var bytes = receivedClient.EndReceive(result, ref ip);

                if (bytes != null && bytes.Length > 0)
                {
                    var packet = OSCConverter.Unpack(bytes);
                    packet.Ip = ip.Address;
                    packet.Port = ip.Port;

                    return packet;
                }
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception e)
            {
                Debug.LogErrorFormat("[OSCReceiver] Receive error: " + e);
            }

            return null;
        }

        #endregion
    }
}
#endif