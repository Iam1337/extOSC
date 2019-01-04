/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerTimeTag : OSCPacker<DateTime>
    {
        #region Protected Methods

        protected override DateTime OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            return values[start++].TimeTagValue;
        }

        protected override void ValueToOSCValues(List<OSCValue> values, DateTime value)
        {
            values.Add(OSCValue.TimeTag(value));
        }

        #endregion
    }
}