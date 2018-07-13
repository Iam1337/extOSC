/* Copyright (c) 2018 ExT (V.Sigalkin) */

namespace extOSC.Core.Packers
{
    class OSCPackerTrue : OSCPacker<bool>
    {
        #region Public Methods

        public override OSCValueType GetPackerType()
        {
            return OSCValueType.True;
        }

        #endregion

        #region Protected Methods

        protected override bool BytesToValue(byte[] bytes, ref int start)
        {
            return true;
        }

        protected override byte[] ValueToBytes(bool value)
        {
            return default(byte[]);
        }

        #endregion
    }
}