/* Copyright (c) 2018 ExT (V.Sigalkin) */

namespace extOSC.Core.Console
{
    public enum OSCConsolePacketType
    {
        Received,

        Transmitted
    }

    public class OSCConsolePacket
    {
        #region Public Vars

        public OSCPacket Packet;

        public OSCConsolePacketType PacketType;

        public string Info;

        #endregion
    }
}