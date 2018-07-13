﻿/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
    [AddComponentMenu("extOSC/Components/Receiver/Double Reflection")]
    public class OSCReceiverReflectionDouble : OSCReceiverReflection<double>
    {
        #region Protected Methods

        protected override bool ProcessMessage(OSCMessage message, out double value)
        {
            value = 0;

            if (message.ToDouble(out value))
                return true;

            return false;
        }

        #endregion
    }
}