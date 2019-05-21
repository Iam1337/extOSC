/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
    [AddComponentMenu("extOSC/Components/Receiver/Quaternion Reflection")]
    public class OSCReceiverReflectionQuaternion : OSCReceiverReflection<Quaternion>
    {
        #region Protected Methods

        protected override bool ProcessMessage(OSCMessage message, out Quaternion value)
        {
            value = new Quaternion();

            if (message.ToQuaternion(out value))
                return true;

            return false;
        }

        #endregion
    }
}