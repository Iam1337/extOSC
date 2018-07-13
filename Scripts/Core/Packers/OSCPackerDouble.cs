/* Copyright (c) 2018 ExT (V.Sigalkin) */

using System;

namespace extOSC.Core.Packers
{
    public class OSCPackerDouble : OSCPacker<double>
    {
        #region Public Methods

        public override OSCValueType GetPackerType()
        {
            return OSCValueType.Double;
        }

        #endregion

        #region Protected Methods

        protected override double BytesToValue(byte[] bytes, ref int start)
        {
            const int size = sizeof(double);
            var data = new byte[size];

            for (var i = 0; i < size; i++)
            {
                data[i] = bytes[start];

                start++;
            }

            return BitConverter.ToDouble(BitConverter.IsLittleEndian ? ReverseBytes(data) : data, 0);
        }

        protected override byte[] ValueToBytes(double value)
        {
            var bytes = BitConverter.GetBytes(value);

            return BitConverter.IsLittleEndian ? ReverseBytes(bytes) : bytes;
        }

        #endregion
    }
}