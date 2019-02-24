/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerString : OSCPacker<string>
    {
        #region Protected Methods

        protected override string OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            return values[start++].StringValue;
        }

        protected override void ValueToOSCValues(List<OSCValue> values, string value)
        {
            values.Add(OSCValue.String(value));
        }

        #endregion
    }
}