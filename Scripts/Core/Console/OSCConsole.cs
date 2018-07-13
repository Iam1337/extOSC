/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

using System.Collections.Generic;

namespace extOSC.Core.Console
{
    public static class OSCConsole
    {
        #region Public Vars

        public static List<OSCConsolePacket> ConsoleBuffer
        {
            get { return _consoleBuffer; }
            set { _consoleBuffer = value; }
        }

        public static bool LogConsole
        {
            get { return _logConsole; }
            set { _logConsole = value; }
        }

        #endregion

        #region Private Vars

        private static List<OSCConsolePacket> _consoleBuffer = new List<OSCConsolePacket>();

        private static bool _logConsole = false;

        #endregion

        #region Public Methods

        public static void Received(OSCReceiver receiver, OSCPacket packet)
        {
			var ip = packet.Ip != null ? string.Format("{0}:{1}", packet.Ip, packet.Port) : "Debug";

			var message = new OSCConsolePacket();
            message.Info = string.Format("Receiver: {0}. From: {1}", receiver.LocalPort, ip);
            message.PacketType = OSCConsolePacketType.Received;
            message.Packet = packet;

            Log(message);
        }

        public static void Transmitted(OSCTransmitter transmitter, OSCPacket packet)
        {
            var message = new OSCConsolePacket();
            message.Info = string.Format("Transmitter: {0}:{1}", transmitter.RemoteHost, transmitter.RemotePort);
            message.PacketType = OSCConsolePacketType.Transmitted;
            message.Packet = packet;

            Log(message);
        }

        #endregion

        #region Private Methods

        private static void Log(OSCConsolePacket message)
        {
#if UNITY_EDITOR
            // COPY PACKET
            var rawData = OSCConverter.Pack(message.Packet);
            message.Packet = OSCConverter.Unpack(rawData);
            
            _consoleBuffer.Add(message);
#else
            if (_logConsole)
            {
                Debug.LogFormat("[OSCConsole] Packed {0}: {1}", message.PacketType, message.Packet);
            }
#endif
        }

        #endregion
    }
}