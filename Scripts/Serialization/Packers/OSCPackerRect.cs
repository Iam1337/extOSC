/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerRect : OSCPacker<Rect>
    {
        #region Protected Methods

        protected override Rect OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            var position = new Vector2();
            position.x = values[start++].FloatValue;
            position.y = values[start++].FloatValue;

            var size = new Vector2();
            size.x = values[start++].FloatValue;
            size.y = values[start++].FloatValue;

            return new Rect(position, size);
        }

        protected override void ValueToOSCValues(List<OSCValue> values, Rect value)
        {
            values.Add(OSCValue.Float(value.center.x));
            values.Add(OSCValue.Float(value.center.y));
            values.Add(OSCValue.Float(value.size.x));
            values.Add(OSCValue.Float(value.size.y));
        }

        #endregion
    }
}