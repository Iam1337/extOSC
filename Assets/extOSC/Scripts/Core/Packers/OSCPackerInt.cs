/* Copyright (c) 2018 ExT (V.Sigalkin) */

using System;

namespace extOSC.Core.Packers
{
    public class OSCPackerInt : OSCPacker<int>
    {
        #region Public Methods

        public override OSCValueType GetPackerType()
        {
            return OSCValueType.Int;
        }

        #endregion

        #region Protected Methods

        protected override int BytesToValue(byte[] bytes, ref int start)
        {
            const int size = sizeof(int);
            var data = new byte[size];

            for (var i = 0; i < size; i++)
            {
                data[i] = bytes[start];

                start++;
            }

            return BitConverter.ToInt32(BitConverter.IsLittleEndian ? ReverseBytes(data) : data, 0);
        }

        protected override byte[] ValueToBytes(int value)
        {
            var bytes = BitConverter.GetBytes(value);

            return BitConverter.IsLittleEndian ? ReverseBytes(bytes) : bytes;
        }

        #endregion
    }
}