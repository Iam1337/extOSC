/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
    [AddComponentMenu("extOSC/Components/Receiver/Vector2 Reflection")]
    public class OSCReceiverReflectionVector2 : OSCReceiverReflection<Vector2>
    {
        #region Protected Methods

        protected override bool ProcessMessage(OSCMessage message, out Vector2 value)
        {
            value = Vector2.zero;

            if (message.ToVector2(out value))
                return true;

            return false;
        }

        #endregion
    }
}