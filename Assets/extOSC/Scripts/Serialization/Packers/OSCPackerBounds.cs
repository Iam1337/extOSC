/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerBounds : OSCPacker<Bounds>
    {
        #region Protected Methods

        protected override Bounds OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            var center = new Vector3();
            center.x = values[start++].FloatValue;
            center.y = values[start++].FloatValue;
            center.z = values[start++].FloatValue;

            var size = new Vector3();
            size.x = values[start++].FloatValue;
            size.y = values[start++].FloatValue;
            size.z = values[start++].FloatValue;

            return new Bounds(center, size);
        }

        protected override void ValueToOSCValues(List<OSCValue> values, Bounds value)
        {
            values.Add(OSCValue.Float(value.center.x));
            values.Add(OSCValue.Float(value.center.y));
            values.Add(OSCValue.Float(value.center.z));
            values.Add(OSCValue.Float(value.size.x));
            values.Add(OSCValue.Float(value.size.y));
            values.Add(OSCValue.Float(value.size.z));
        }

        #endregion
    }
}