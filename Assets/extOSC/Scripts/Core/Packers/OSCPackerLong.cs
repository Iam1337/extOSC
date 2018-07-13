/* Copyright (c) 2018 ExT (V.Sigalkin) */

using System;

namespace extOSC.Core.Packers
{
    public class OSCPackerLong : OSCPacker<long>
    {
        #region Public Methods

        public override OSCValueType GetPackerType()
        {
            return OSCValueType.Long;
        }

        #endregion

        #region Protected Methods

        protected override long BytesToValue(byte[] bytes, ref int start)
        {
            const int size = sizeof(long);
            var data = new byte[size];

            for (var i = 0; i < size; i++)
            {
                data[i] = bytes[start]; start++;
            }

            return BitConverter.ToInt64(BitConverter.IsLittleEndian ? ReverseBytes(data) : data, 0);
        }

        protected override byte[] ValueToBytes(long value)
        {
            var bytes = BitConverter.GetBytes(value);

            return BitConverter.IsLittleEndian ? ReverseBytes(bytes) : bytes;
        }

        #endregion
    }
}