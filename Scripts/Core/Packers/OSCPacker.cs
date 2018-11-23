/* Copyright (c) 2018 ExT (V.Sigalkin) */

using System.Linq;

namespace extOSC.Core.Packers
{
    public abstract class OSCPacker
    {
        #region Protected Static Methods

        protected static byte[] ReverseBytes(byte[] bytes)
        {
            var newBytes = new byte[bytes.Length];

            for (var i = 0; i < bytes.Length; i++)
            {
                newBytes[i] = bytes[(bytes.Length - 1) - i];
            }

            return newBytes;
        }

        #endregion

        #region Protected Vars

        protected const byte ZeroByte = 0;

        #endregion

        #region Public Methods

        public abstract OSCValueType GetPackerType();

        public abstract object Unpack(byte[] bytes, ref int start);

        public abstract void Pack(byte[] bytes, ref int index, object value);

        #endregion

        #region Protected Static Methods

        protected void IncludeZeroBytes(byte[] bytes, int size, ref int index)
        {
            var zeroCount = 4 - size % 4;
            for (var i = 0; i < zeroCount; i++)
            {
                bytes[index] = 0;
                index++;
            }
        }

        #endregion
    }

    public abstract class OSCPacker<T> : OSCPacker
    {
        #region Public Methods

        public override object Unpack(byte[] bytes, ref int start)
        {
            return BytesToValue(bytes, ref start);
        }

        public override void Pack(byte[] bytes, ref int index, object value)
        {
            var unpackedValue = (T)value;
            if (unpackedValue == null) return;

            ValueToBytes(bytes, ref index, (T)value);
        }

        #endregion

        #region Protected Methods

        protected abstract T BytesToValue(byte[] buffer, ref int index);

        protected abstract void ValueToBytes(byte[] buffer, ref int index, T value);

        #endregion
    }
}