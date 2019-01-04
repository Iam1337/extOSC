/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerBool : OSCPacker<bool>
    {
        #region Protected Methods

        protected override bool OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            return values[start++].BoolValue;
        }

        protected override void ValueToOSCValues(List<OSCValue> values, bool value)
        {
            values.Add(OSCValue.Bool(value));
        }

        #endregion
    }
}