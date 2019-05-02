/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
    [AddComponentMenu("extOSC/Components/Receiver/Long Event")]
    public class OSCReceiverEventLong : OSCReceiverEvent<OSCEventLong>
    {
        #region Protected Methods

        protected override void Invoke(OSCMessage message)
        {
            long value;

            if (message.ToLong(out value))
            {
                if (onReceive != null)
                    onReceive.Invoke(value);
            }
        }

        #endregion
    }
}