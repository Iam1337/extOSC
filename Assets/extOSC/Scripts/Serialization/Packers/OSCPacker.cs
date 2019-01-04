/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public abstract class OSCPacker
    {
        #region Private Vars

        private string _packerId;

        #endregion

        #region Public Methods

        public virtual string GetId()
        {
            if (string.IsNullOrEmpty(_packerId))
            {
                _packerId = GetPackerType().Name.ToLower();
            }

            return _packerId;
        }

        public abstract Type GetPackerType();

        public abstract object GetDefaultValue();

        public abstract void Pack(List<OSCValue> values, object value);

        public abstract object Unpack(List<OSCValue> values, ref int start, Type type);

        #endregion
    }

    public abstract class OSCPacker<T> : OSCPacker
    {
        #region Public Methods

        public override Type GetPackerType()
        {
            return typeof(T);
        }

        public override void Pack(List<OSCValue> values, object value)
        {
            ValueToOSCValues(values, (T)value);
        }

        public override object Unpack(List<OSCValue> values, ref int start, Type type)
        {
            return OSCValuesToValue(values, ref start, type);
        }

        public override object GetDefaultValue()
        {
            return default(T);
        }

        #endregion

        #region Protected Methods

        protected abstract void ValueToOSCValues(List<OSCValue> values, T value);

        protected abstract T OSCValuesToValue(List<OSCValue> values, ref int start, Type type);

        #endregion
    }
}