/* Copyright (c) 2020 ExT (V.Sigalkin) */

#if UNITY_WSA && !UNITY_EDITOR

using UnityEngine;

using System;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace extOSC.Core.Network
{
    internal class OSCTransmitterWindowsStoreBackend : OSCTransmitterBackend
    {
        #region Public Vars

        public override bool IsAvailable => _datagramSocket != null;

        #endregion

        #region Private Vars

        private DatagramSocket _datagramSocket;

        private HostName _remoteHost;

        private string _remotePort;

        #endregion

        #region Public Methods

        public override void Connect(string localHost, int localPort)
        {
            if (_datagramSocket != null)
                Close();

            //_remoteHost = new HostName(remoteHost);
            //_remotePort = remotePort.ToString();

            _datagramSocket = new DatagramSocket();
        }

        public override void RefreshRemote(string remoteHost, int remotePort)
        {
            _remoteHost = new HostName(remoteHost);
            _remotePort = remotePort.ToString();
        }

        public override void Close()
        {
            if (_datagramSocket != null)
                _datagramSocket.Dispose();

            _datagramSocket = null;
        }

        public override void Send(byte[] data, int length)
        {
            SendAsync(data.AsBuffer(0, length));
        }

        #endregion

        #region Private Methods

        private async void SendAsync(IBuffer buffer)
        {
            using (var dataWriter =
                new DataWriter(await _datagramSocket.GetOutputStreamAsync(_remoteHost, _remotePort)))
            {
                try
                {
                    dataWriter.WriteBuffer(buffer);
                    await dataWriter.StoreAsync();
                }
                catch (Exception exception)
                {
                    Debug.LogWarningFormat("[OSCTranmitter] Error: {0}", exception);
                }
            }
        }

        #endregion
    }
}

#endif