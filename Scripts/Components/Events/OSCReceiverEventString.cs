/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
    [AddComponentMenu("extOSC/Components/Receiver/String Event")]
    public class OSCReceiverEventString : OSCReceiverEvent<OSCEventString>
    {
        #region Protected Methods

        protected override void Invoke(OSCMessage message)
        {
            string value;

            if (message.ToString(out value))
            {
                if (onReceive != null)
                    onReceive.Invoke(value);
            }
        }

        #endregion
    }
}