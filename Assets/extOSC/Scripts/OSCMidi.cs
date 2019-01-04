/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;

namespace extOSC
{
    [Serializable]
    public struct OSCMidi
    {
        #region Public Vars

        public byte Channel;

        public byte Status;

        public byte Data1;

        public byte Data2;

        #endregion

        #region Public Methods

        public OSCMidi(byte channel, byte status, byte data1, byte data2)
        {
            Channel = channel;
            Status = status;
            Data1 = data1;
            Data2 = data2;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}, {3})", Channel, Status, Data1, Data2);
        }

        #endregion
    }
}