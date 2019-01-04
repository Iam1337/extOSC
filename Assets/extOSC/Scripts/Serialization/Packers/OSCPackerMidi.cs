/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerMidi : OSCPacker<OSCMidi>
    {
        #region Protected Methods

        protected override OSCMidi OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            return values[start++].MidiValue;
        }

        protected override void ValueToOSCValues(List<OSCValue> values, OSCMidi value)
        {
            values.Add(OSCValue.Midi(value));
        }

        #endregion
    }
}