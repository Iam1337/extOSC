/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
    [AddComponentMenu("extOSC/Components/Receiver/Vector3 Event")]
    public class OSCReceiverEventVector3 : OSCReceiverEvent<OSCEventVector3>
    {
        #region Protected Methods

        protected override void Invoke(OSCMessage message)
        {
            Vector3 value;

            if (message.ToVector3(out value))
            {
                if (onReceive != null)
                    onReceive.Invoke(value);
            }
        }

        #endregion
    }
}