/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerVector2 : OSCPacker<Vector2>
    {
        #region Protected Methods

        protected override Vector2 OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            var vector = new Vector2();
            vector.x = values[start++].FloatValue;
            vector.y = values[start++].FloatValue;

            return vector;
        }

        protected override void ValueToOSCValues(List<OSCValue> values, Vector2 value)
        {
            values.Add(OSCValue.Float(value.x));
            values.Add(OSCValue.Float(value.y));
        }

        #endregion
    }
}