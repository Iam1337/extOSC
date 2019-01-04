/* Copyright (c) 2019 ExT (V.Sigalkin) */

#if !NETFX_CORE
using System;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerArray : OSCPacker<Array>
    {
        #region Protected Methods

        protected override Array OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            var size = values[start++].IntValue;
            var elementType = type.GetElementType();
            var value = Array.CreateInstance(elementType, size);

            for (var i = 0; i < size; i++)
            {
                value.SetValue(OSCSerializer.Unpack(values, ref start, elementType), i);
            }

            return value;
        }

        protected override void ValueToOSCValues(List<OSCValue> values, Array value)
        {
            values.Add(OSCValue.Int(value.Length));

            foreach (var element in value)
            {
                OSCSerializer.Pack(values, element);
            }
        }

        #endregion
    }
}
#endif