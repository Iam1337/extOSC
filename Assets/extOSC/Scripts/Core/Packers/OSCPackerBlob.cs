/* Copyright (c) 2018 ExT (V.Sigalkin) */

using System;
using System.Collections.Generic;

namespace extOSC.Core.Packers
{
    class OSCPackerBlob : OSCPacker<byte[]>
    {
        #region Public Methods

        public override OSCValueType GetPackerType()
        {
            return OSCValueType.Blob;
        }

        #endregion

        #region Protected Methods

        protected override byte[] BytesToValue(byte[] bytes, ref int start)
        {
            const int size = sizeof(int);
            var data = new byte[size];

            for (var i = 0; i < size; i++)
            {
                data[i] = bytes[start];

                start++;
            }

            var length = BitConverter.ToInt32(BitConverter.IsLittleEndian ? ReverseBytes(data) : data, 0);
            var blob = new byte[length];

            Array.Copy(bytes, start, blob, 0, length);

            start += length + (4 - (length % 4));

            return blob;
        }

        protected override byte[] ValueToBytes(byte[] value)
        {
            var bytes = new List<byte>();
            var lengthBytes = BitConverter.GetBytes(value.Length);

            bytes.AddRange(BitConverter.IsLittleEndian ? ReverseBytes(lengthBytes) : lengthBytes);
            bytes.AddRange(IncludeZeroBytes(value));

            return bytes.ToArray();
        }

        #endregion
    }
}