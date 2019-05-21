/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
    [AddComponentMenu("extOSC/Components/Receiver/Float Reflection")]
    public class OSCReceiverReflectionFloat : OSCReceiverReflection<float>
    {
        #region Protected Methods

        protected override bool ProcessMessage(OSCMessage message, out float value)
        {
            value = 0;

            if (message.ToFloat(out value))
                return true;

            return false;
        }

        #endregion
    }
}