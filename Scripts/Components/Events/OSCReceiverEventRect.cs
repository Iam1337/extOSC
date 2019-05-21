/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
    [AddComponentMenu("extOSC/Components/Receiver/Rect Event")]
    public class OSCReceiverEventRect : OSCReceiverEvent<OSCEventRect>
    {
        #region Protected Methods

        protected override void Invoke(OSCMessage message)
        {
            Rect value;

            if (message.ToRect(out value))
            {
                if (onReceive != null)
                    onReceive.Invoke(value);
            }
        }

        #endregion
    }
}