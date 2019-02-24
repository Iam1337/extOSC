/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerLong : OSCPacker<long>
    {
        #region Protected Methods

        protected override long OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            return values[start++].LongValue;
        }

        protected override void ValueToOSCValues(List<OSCValue> values, long value)
        {
            values.Add(OSCValue.Long(value));
        }

        #endregion
    }
}