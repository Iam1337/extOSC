/* Copyright (c) 2019 ExT (V.Sigalkin) */

#if !NETFX_CORE
using System;
using System.Collections;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerList : OSCPacker<IList>
    {
        #region Protected Methods

        protected override IList OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            var size = values[start++].IntValue;
            var elementType = type.GetGenericArguments()[0];
            var value = (IList)Activator.CreateInstance(type, size);

            for (var i = 0; i < size; i++)
            {
                value.Add(OSCSerializer.Unpack(values, ref start, elementType));
            }

            return value;
        }

        protected override void ValueToOSCValues(List<OSCValue> values, IList value)
        {
            values.Add(OSCValue.Int(value.Count));

            foreach (var element in value)
            {
                OSCSerializer.Pack(values, element);
            }
        }

        #endregion
    }
}
#endif