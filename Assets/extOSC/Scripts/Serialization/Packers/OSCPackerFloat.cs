/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerFloat : OSCPacker<float>
    {
        #region Protected Methods

        protected override float OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            return values[start++].FloatValue;
        }

        protected override void ValueToOSCValues(List<OSCValue> values, float value)
        {
            values.Add(OSCValue.Float(value));
        }

        #endregion
    }
}