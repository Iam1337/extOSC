/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerColor : OSCPacker<Color>
    {
        #region Protected Methods

        protected override Color OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            return values[start++].ColorValue;
        }

        protected override void ValueToOSCValues(List<OSCValue> values, Color value)
        {
            values.Add(OSCValue.Color(value));
        }

        #endregion
    }
}