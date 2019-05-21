/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
    [AddComponentMenu("extOSC/Components/Receiver/Vector3 Reflection")]
    public class OSCReceiverReflectionVector3 : OSCReceiverReflection<Vector3>
    {
        #region Protected Methods

        protected override bool ProcessMessage(OSCMessage message, out Vector3 value)
        {
            value = Vector3.zero;

            if (message.ToVector3(out value))
                return true;

            return false;
        }

        #endregion
    }
}