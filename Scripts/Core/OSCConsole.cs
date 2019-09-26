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

        public static void Received(OSCReceiver receiver, IOSCPacket ioscPacket)
        {
			var ip = ioscPacket.Ip != null ? string.Format("{0}:{1}", ioscPacket.Ip, ioscPacket.Port) : "Debug";

			var consolePacket = new OSCConsolePacket();
            consolePacket.Info = string.Format("Receiver: {0}. From: {1}", receiver.LocalPort, ip);
            consolePacket.PacketType = OSCConsolePacketType.Received;
            consolePacket.IoscPacket = ioscPacket;

            Log(consolePacket);
        }

        public static void Transmitted(OSCTransmitter transmitter, IOSCPacket ioscPacket)
        {
            var consolePacket = new OSCConsolePacket();
            consolePacket.Info = string.Format("Transmitter: {0}:{1}", transmitter.RemoteHost, transmitter.RemotePort);
            consolePacket.PacketType = OSCConsolePacketType.Transmitted;
            consolePacket.IoscPacket = ioscPacket;

            Log(consolePacket);
        }

        #endregion

        #region Private Methods

        private static void Log(OSCConsolePacket consolePacket)
        {
#if UNITY_EDITOR
            // COPY PACKET
	        consolePacket.IoscPacket = consolePacket.IoscPacket.Copy();
            
            _consoleBuffer.Add(consolePacket);
#else
            if (_logConsole)
            {
                UnityEngine.Debug.LogFormat("[OSCConsole] Packed {0}: {1}", consolePacket.PacketType, consolePacket.IoscPacket);
            }
#endif
        }

        #endregion
    }
}