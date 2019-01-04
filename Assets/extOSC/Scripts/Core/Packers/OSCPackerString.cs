/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;
using System.Text;

namespace extOSC.Core.Packers
{
    internal class OSCPackerString : OSCPacker<string>
    {

        #region Public Methods

        public override OSCValueType GetPackerType()
        {
            return OSCValueType.String;
        }

        #endregion

        #region Protected Methods

        protected override string BytesToValue(byte[] buffer, ref int index)
        {
            var length = 0;
            var position = index;

            while (buffer[position] != 0 && position < buffer.Length)
            {
                position++;
                length++;
            }

            var value = Encoding.ASCII.GetString(buffer, index, length);

            index += length + (4 - length % 4);

            return value;
        }

        protected override void ValueToBytes(byte[] buffer, ref int index, string value)
        {
            var data = Encoding.ASCII.GetBytes(value);
            
            Array.Copy(data, 0, buffer, index, data.Length);

            index += value.Length;

            IncludeZeroBytes(buffer, value.Length, ref index);
        }

        #endregion
    }
}