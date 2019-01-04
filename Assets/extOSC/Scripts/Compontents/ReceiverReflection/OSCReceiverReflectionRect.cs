﻿/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
    [AddComponentMenu("extOSC/Components/Receiver/Rect Reflection")]
    public class OSCReceiverReflectionRect : OSCReceiverReflection<Rect>
    {
        #region Protected Methods

        protected override bool ProcessMessage(OSCMessage message, out Rect value)
        {
            value = new Rect();

            if (message.ToRect(out value))
                return true;

            return false;
        }

        #endregion
    }
}