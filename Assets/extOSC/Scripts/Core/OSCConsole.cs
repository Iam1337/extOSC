/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System.Collections.Generic;

namespace extOSC.Core
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

			var consolePacket = new OSCConsolePacket();
            consolePacket.Info = string.Format("Receiver: {0}. From: {1}", receiver.LocalPort, ip);
            consolePacket.PacketType = OSCConsolePacketType.Received;
            consolePacket.Packet = packet;

            Log(consolePacket);
        }

        public static void Transmitted(OSCTransmitter transmitter, OSCPacket packet)
        {
            var consolePacket = new OSCConsolePacket();
            consolePacket.Info = string.Format("Transmitter: {0}:{1}", transmitter.RemoteHost, transmitter.RemotePort);
            consolePacket.PacketType = OSCConsolePacketType.Transmitted;
            consolePacket.Packet = packet;

            Log(consolePacket);
        }

        #endregion

        #region Private Methods

        private static void Log(OSCConsolePacket consolePacket)
        {
#if UNITY_EDITOR
            // COPY PACKET
	        consolePacket.Packet = consolePacket.Packet.Copy();
            
            _consoleBuffer.Add(consolePacket);
#else
            if (_logConsole)
            {
                UnityEngine.Debug.LogFormat("[OSCConsole] Packed {0}: {1}", consolePacket.PacketType, consolePacket.Packet);
            }
#endif
        }

        #endregion
    }
}