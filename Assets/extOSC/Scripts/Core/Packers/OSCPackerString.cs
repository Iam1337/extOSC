/* Copyright (c) 2018 ExT (V.Sigalkin) */

using System.Text;

namespace extOSC.Core.Packers
{
    class OSCPackerString : OSCPacker<string>
    {

        #region Public Methods

        public override OSCValueType GetPackerType()
        {
            return OSCValueType.String;
        }

        #endregion

        #region Protected Methods

        protected override string BytesToValue(byte[] bytes, ref int start)
        {
            var length = 0;
            var index = start;

            while (bytes[index] != 0 && index < bytes.Length)
            {
                index++;
                length++;
            }

            var stringValue = Encoding.ASCII.GetString(bytes, start, length);
            start += length + (4 - (length % 4));

            return stringValue;
        }

        protected override byte[] ValueToBytes(string value)
        {
            var data = Encoding.ASCII.GetBytes(value);

            return IncludeZeroBytes(data);
        }

        #endregion
    }
}