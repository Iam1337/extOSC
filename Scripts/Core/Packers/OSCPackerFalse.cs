/* Copyright (c) 2018 ExT (V.Sigalkin) */

namespace extOSC.Core.Packers
{
    class OSCPackerFalse : OSCPacker<bool>
    {
        #region Public Methods

        public override OSCValueType GetPackerType()
        {
            return OSCValueType.False;
        }

        #endregion

        #region Protected Methods

        protected override bool BytesToValue(byte[] bytes, ref int start)
        {
            return false;
        }

        protected override byte[] ValueToBytes(bool value)
        {
            return default(byte[]);
        }

        #endregion
    }
}