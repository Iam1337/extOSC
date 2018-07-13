/* Copyright (c) 2018 ExT (V.Sigalkin) */

using System;

namespace extOSC.Core.Packers
{
    class OSCPackerMidi : OSCPacker<OSCMidi>
    {
        #region Public Methods

        public override OSCValueType GetPackerType()
        {
            return OSCValueType.Midi;
        }

        #endregion

        #region Protected Methods

        protected override OSCMidi BytesToValue(byte[] bytes, ref int start)
        {
            start += 4;

            return new OSCMidi(bytes[start - 4], bytes[start - 3], bytes[start - 2], bytes[start - 1]);
        }

        protected override byte[] ValueToBytes(OSCMidi value)
        {
            return new byte[] { value.Channel, value.Status, value.Data1, value.Data2 };
        }

        #endregion
    }
}