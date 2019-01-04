/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerBlob : OSCPacker<byte[]>
    {
        #region Protected Methods

        protected override byte[] OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            return values[start++].BlobValue;
        }

        protected override void ValueToOSCValues(List<OSCValue> values, byte[] value)
        {
            values.Add(OSCValue.Blob(value));
        }

        #endregion
    }
}