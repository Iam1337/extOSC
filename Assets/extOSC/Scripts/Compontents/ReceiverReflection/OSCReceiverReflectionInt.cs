﻿/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
    [AddComponentMenu("extOSC/Components/Receiver/Int Reflection")]
    public class OSCReceiverReflectionInt : OSCReceiverReflection<int>
    {
        #region Protected Methods

        protected override bool ProcessMessage(OSCMessage message, out int value)
        {
            value = 0;

            if (message.ToInt(out value))
                return true;

            return false;
        }

        #endregion
    }
}