/* Copyright (c) 2018 ExT (V.Sigalkin) */

namespace extOSC.Core.Packers
{
    class OSCPackerNull : OSCPacker<object>
    {
        #region Public Methods

        public override OSCValueType GetPackerType()
        {
            return OSCValueType.Null;
        }

        #endregion

        #region Protected Methods

        protected override object BytesToValue(byte[] bytes, ref int start)
        {
            return null;
        }

        protected override byte[] ValueToBytes(object value)
        {
            return default(byte[]);
        }

        #endregion
    }
}