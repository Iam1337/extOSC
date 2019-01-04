/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerChar : OSCPacker<char>
    {
        #region Protected Methods

        protected override char OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            return values[start++].CharValue;
        }

        protected override void ValueToOSCValues(List<OSCValue> values, char value)
        {
            values.Add(OSCValue.Char(value));
        }

        #endregion
    }
}