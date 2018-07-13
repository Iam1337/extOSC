/* Copyright (c) 2018 ExT (V.Sigalkin) */

namespace extOSC.Core.Packers
{
    class OSCPackerImpulse : OSCPacker<object>
    {
        #region Public Methods

        public override OSCValueType GetPackerType()
        {
            return OSCValueType.Impulse;
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