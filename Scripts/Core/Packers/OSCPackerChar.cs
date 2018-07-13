/* Copyright (c) 2018 ExT (V.Sigalkin) */

using System;

namespace extOSC.Core.Packers
{
    class OSCPackerChar : OSCPacker<char>
    {
        #region Public Methods

        public override OSCValueType GetPackerType()
        {
            return OSCValueType.Char;
        }

        #endregion

        #region Protected Methods

        protected override char BytesToValue(byte[] bytes, ref int start)
        {
            const int size = sizeof(char);
            var data = new byte[size];

            for (var i = 0; i < size; i++)
            {
                data[i] = bytes[start];

                start++;
            }

            return BitConverter.ToChar(BitConverter.IsLittleEndian ? ReverseBytes(data) : data, 0);
        }

        protected override byte[] ValueToBytes(char value)
        {
            var bytes = BitConverter.GetBytes(value);

            return BitConverter.IsLittleEndian ? ReverseBytes(bytes) : bytes;
        }

        #endregion
    }
}