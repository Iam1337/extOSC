﻿/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
    [AddComponentMenu("extOSC/Components/Receiver/Long Reflection")]
    public class OSCReceiverReflectionLong : OSCReceiverReflection<long>
    {
        #region Protected Methods

        protected override bool ProcessMessage(OSCMessage message, out long value)
        {
            value = 0;

            if (message.ToLong(out value))
                return true;

            return false;
        }

        #endregion
    }
}