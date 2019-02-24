/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerDouble : OSCPacker<double>
    {
        #region Protected Methods

        protected override double OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            return values[start++].DoubleValue;
        }

        protected override void ValueToOSCValues(List<OSCValue> values, double value)
        {
            values.Add(OSCValue.Double(value));
        }

        #endregion
    }
}