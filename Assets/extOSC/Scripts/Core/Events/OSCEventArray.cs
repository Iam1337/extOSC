/* Copyright (c) 2018 ExT (V.Sigalkin) */

using System;
using System.Collections.Generic;

namespace extOSC.Core.Events
{
    [Serializable]
    public class OSCEventArray : OSCEvent<List<OSCValue>>
    { }
}