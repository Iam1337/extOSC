﻿/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

using System.Collections.Generic;

namespace extOSC.Components.ReceiverReflections
{
    [AddComponentMenu("extOSC/Components/Receiver/Array Reflection")]
    public class OSCReceiverReflectionArray : OSCReceiverReflection<List<OSCValue>>
    {
        #region Protected Methods

        protected override bool ProcessMessage(OSCMessage message, out List<OSCValue> value)
        {
            value = null;

            if (message.ToArray(out value))
                return true;

            return false;
        }

        #endregion
    }
}