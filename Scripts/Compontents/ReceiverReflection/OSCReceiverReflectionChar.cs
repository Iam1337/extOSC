﻿/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
    [AddComponentMenu("extOSC/Components/Receiver/Char Reflection")]
    public class OSCReceiverReflectionChar : OSCReceiverReflection<char>
    {
        #region Protected Methods

        protected override bool ProcessMessage(OSCMessage message, out char value)
        {
            value = ' ';

            if (message.ToChar(out value))
                return true;

            return false;
        }

        #endregion
    }
}