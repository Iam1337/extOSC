/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
    [AddComponentMenu("extOSC/Components/Receiver/Float Event")]
    public class OSCReceiverEventFloat : OSCReceiverEvent<OSCEventFloat>
    {
        #region Protected Methods

        protected override void Invoke(OSCMessage message)
        {
            float value;

            if (message.ToFloat(out value))
            {
                if (onReceive != null)
                    onReceive.Invoke(value);
            }
        }

        #endregion
    }
}