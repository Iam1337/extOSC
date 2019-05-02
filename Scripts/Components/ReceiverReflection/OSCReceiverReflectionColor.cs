/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
    [AddComponentMenu("extOSC/Components/Receiver/Color Reflection")]
    public class OSCReceiverReflectionColor : OSCReceiverReflection<Color>
    {
        #region Protected Methods

        protected override bool ProcessMessage(OSCMessage message, out Color value)
        {
            value = Color.white;

            if (message.ToColor(out value))
                return true;

            return false;
        }

        #endregion
    }
}