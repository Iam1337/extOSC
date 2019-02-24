/* Copyright (c) 2019 ExT (V.Sigalkin) */

#if !NETFX_CORE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerDictionary : OSCPacker<IDictionary>
    {
        #region Protected Methods

        protected override IDictionary OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            var size = values[start++].IntValue;

            var keyElement = type.GetGenericArguments()[0];
            var valueElement = type.GetGenericArguments()[1];
            var value = (IDictionary)Activator.CreateInstance(type, size);

            for (var i = 0; i < size; i++)
            {
                value.Add(OSCSerializer.Unpack(values, ref start, keyElement), OSCSerializer.Unpack(values, ref start, valueElement));
            }

            return value;
        }

        protected override void ValueToOSCValues(List<OSCValue> values, IDictionary value)
        {
            values.Add(OSCValue.Int(value.Count));

            for (var i = 0; i < value.Count; i++)
            {
                OSCSerializer.Pack(values, value.Keys.OfType<object>().ElementAt(i));
                OSCSerializer.Pack(values, value.Values.OfType<object>().ElementAt(i));
            }
        }

        #endregion
    }
}
#endif