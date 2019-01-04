/* Copyright (c) 2019 ExT (V.Sigalkin) */

namespace extOSC
{
    public class OSCMatchPattern
    {
        #region Public Methods

        public OSCValueType[] Types
        {
            get { return types; }
        }

        #endregion

        #region Protected Methods

        protected OSCValueType[] types;

        #endregion

        #region Public Methods

        public OSCMatchPattern(params OSCValueType[] paramTypes)
        {
            types = paramTypes;
        }

        #endregion
    }
}