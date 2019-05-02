/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
    [AddComponentMenu("extOSC/Components/Receiver/Color Event")]
    public class OSCReceiverEventColor : OSCReceiverEvent<OSCEventColor>
    {
        #region Protected Methods

        protected override void Invoke(OSCMessage message)
        {
            Color value;

            if (message.ToColor(out value))
            {
                if (onReceive != null)
                    onReceive.Invoke(value);
            }
        }

        #endregion
    }
}