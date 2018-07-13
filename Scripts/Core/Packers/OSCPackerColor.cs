/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Core.Packers
{
    class OSCPackerColor : OSCPacker<Color>
    {
        #region Public Methods

        public override OSCValueType GetPackerType()
        {
            return OSCValueType.Color;
        }

        #endregion

        #region Protected Methods

        protected override Color BytesToValue(byte[] bytes, ref int start)
        {
            start += 4;

            return new Color32(bytes[start - 4], bytes[start - 3], bytes[start - 2], bytes[start - 1]);
        }

        protected override byte[] ValueToBytes(Color value)
        {
            const int size = 4;

            var bytes = new byte[size];
            var color = (Color32)value;

            bytes[0] = color.r;
            bytes[1] = color.g;
            bytes[2] = color.b;
            bytes[3] = color.a;

            return bytes;
        }

        #endregion
    }
}