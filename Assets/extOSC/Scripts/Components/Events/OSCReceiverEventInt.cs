/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
    [AddComponentMenu("extOSC/Components/Receiver/Integer Event")]
    public class OSCReceiverEventInt : OSCReceiverEvent<OSCEventInt>
    {
        #region Protected Methods

        protected override void Invoke(OSCMessage message)
        {
            int value;

            if (message.ToInt(out value))
            {
                if (onReceive != null)
                    onReceive.Invoke(value);
            }
        }

        #endregion
    }
}