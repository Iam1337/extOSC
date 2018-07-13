﻿/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
    [AddComponentMenu("extOSC/Components/Receiver/Bool Reflection")]
    public class OSCReceiverReflectionBool : OSCReceiverReflection<bool>
    {
        #region Protected Methods

        protected override bool ProcessMessage(OSCMessage message, out bool value)
        {
            value = false;

            if (message.ToBool(out value))
                return true;

            return false;
        }

        #endregion
    }
}