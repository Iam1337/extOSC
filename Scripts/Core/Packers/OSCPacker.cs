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

        public abstract OSCValue Unpack(byte[] bytes, ref int start);

        public abstract object UnpackValue(byte[] bytes, ref int start);

        public abstract byte[] Pack(OSCValue oscValue);

        public abstract byte[] PackValue(object value);

        #endregion

        #region Protected Static Methods

        protected byte[] IncludeZeroBytes(byte[] bytes)
        {
            var byteList = bytes.ToList();

            var zeroCount = 4 - (byteList.Count % 4);
            for (var i = 0; i < zeroCount; i++)
            {
                byteList.Add(ZeroByte);
            }

            return byteList.ToArray();
        }

        #endregion
    }


    public abstract class OSCPacker<T> : OSCPacker
    {
        #region Public Methods

        public override OSCValue Unpack(byte[] bytes, ref int start)
        {
            return new OSCValue(GetPackerType(), BytesToValue(bytes, ref start));
        }

        public override object UnpackValue(byte[] bytes, ref int start)
        {
            return BytesToValue(bytes, ref start);
        }

        public override byte[] Pack(OSCValue oscValue)
        {
            var value = (T)oscValue.Value;
            return value != null ? ValueToBytes((T)oscValue.Value) : null;
        }

        public override byte[] PackValue(object value)
        {
            var unpackedValue = (T)value;
            return unpackedValue != null ? ValueToBytes((T)value) : null;
        }

        #endregion

        #region Protected Methods

        protected abstract T BytesToValue(byte[] bytes, ref int start);

        protected abstract byte[] ValueToBytes(T value);

        #endregion
    }
}