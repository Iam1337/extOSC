using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace extOSC.Core.Network
{
    internal class StandaloneBackend : IBackend
    {
        #region Public Vars

        public bool IsAvailable => _socket != null;
        public bool IsReceiving => _receiving;

        public bool IsTransmitting => _transmitting;

        #endregion

        #region Private Vars

        private string _localHost;

        private int _localPort;
        
        private Socket _socket;

        private bool _receiving;

        private Thread _receivingThread;

        private readonly Queue<IOSCPacket> _receivingPackets = new Queue<IOSCPacket>();

        private readonly object _receivingLock = new object();

        private bool _transmitting;

        #endregion

        #region Public Methods

        public void Connect(string localHost, int localPort)
        {
            try
            {
                _localHost = localHost;
                _localPort = localPort;
                
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _socket.DontFragment = true;
                _socket.ReceiveTimeout = 10;
                _socket.Bind(new IPEndPoint(IPAddress.Parse(localHost), localPort));
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10048)
                {
                    Debug.LogErrorFormat(
                        "[OSCReceiver] Socket Error: Could not use port {0} because another application is listening on it.",
                        localPort);
                }
                else if (e.ErrorCode == 10049)
                {
                    Debug.LogErrorFormat(
                        "[OSCReceiver] Socket Error: Could not use local host \"{0}\". Cannot assign requested address.",
                        localHost);
                }
                else
                {
                    Debug.LogErrorFormat("[OSCReceiver] Socket Error: Error Code {0}.\n{1}", e.ErrorCode, e.Message);
                }

                Close();
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.LogErrorFormat("[OSCReceiver] Invalid port: {0}!", localPort);

                Close();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("[OSCReceiver] Error: {0}", e);

                Close();
            }
        }

        public void Close()
        {
            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }
        }

        public void StartReceiving()
        {
            if (_socket == null)
                return;

            _receiving = true;
            _receivingThread = new Thread(ReceivingThread);
            _receivingThread.Start();
        }

        public bool IsReceived(out IOSCPacket packet)
        {
            lock (_receivingLock)
                _receivingPackets.TryDequeue(out packet);

            return packet != null;
        }

        public void StopReceiving()
        {
            _receiving = false;
            _receivingThread.Join();
        }

        public void StartTransmitting(string remoteHost, int remotePort)
        {
            if (_transmitting)
                return;

            _transmitting = true;
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, _localHost == "255.255.255.255");
            _socket.Connect(new IPEndPoint(IPAddress.Parse(remoteHost), remotePort));
        }

        public void Transmit(IOSCPacket packet)
        {
            if (!_transmitting)
                return;
            
            var bufferLength = OSCConverter.Pack(packet, out var buffer);
            _socket.Send(buffer, bufferLength, SocketFlags.None);
        }

        public void StopTransmitting()
        {
            if (!_transmitting)
                return;
            
            _transmitting = false;
        }

        #endregion

        #region Private Methods

        private void ReceivingThread()
        {
            var from = new IPEndPoint(IPAddress.Any, 0);
            var fromRemote = (EndPoint) from;
            var buffer = new byte[65507];

            while (_receiving)
            {
                try
                {
                    var bufferSize = _socket.ReceiveFrom(buffer, ref fromRemote);
                    if (bufferSize > 0 && _receiving)
                    {
                        var packet = OSCConverter.Unpack(buffer, bufferSize);
                        if (packet == null) continue;

                        packet.Ip = from.Address;
                        packet.Port = from.Port;

                        lock (_receivingLock)
                            _receivingPackets.Enqueue(packet);
                    }
                }
                catch (ThreadAbortException)
                {
                }
                catch (SocketException)
                {
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("[OSCReceiver] Error: {0}", e);
                }
            }
        }

        #endregion
    }
}