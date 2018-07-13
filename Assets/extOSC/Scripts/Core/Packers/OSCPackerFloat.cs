/* Copyright (c) 2018 ExT (V.Sigalkin) */

using System;

namespace extOSC.Core.Packers
{
    public class OSCPackerFloat : OSCPacker<float>
    {
        #region Public Methods

        public override OSCValueType GetPackerType()
        {
            return OSCValueType.Float;
        }

        #endregion

        #region Protected Methods

        protected override float BytesToValue(byte[] bytes, ref int start)
        {
            const int size = sizeof(float);
            var data = new byte[size];

            for (var i = 0; i < size; i++)
            {
                data[i] = bytes[start];

                start++;
            }

            return BitConverter.ToSingle(BitConverter.IsLittleEndian ? ReverseBytes(data) : data, 0);
        }

        protected override byte[] ValueToBytes(float value)
        {
            var bytes = BitConverter.GetBytes(value);

            return BitConverter.IsLittleEndian ? ReverseBytes(bytes) : bytes;
        }

        #endregion
    }
}