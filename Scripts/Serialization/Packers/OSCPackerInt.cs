/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerInt : OSCPacker<int>
    {
        #region Protected Methods

        protected override int OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            return values[start++].IntValue;
        }

        protected override void ValueToOSCValues(List<OSCValue> values, int value)
        {
            values.Add(OSCValue.Int(value));
        }

        #endregion
    }
}