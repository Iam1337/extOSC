/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
    [AddComponentMenu("extOSC/Components/Receiver/Bool Event")]
    public class OSCReceiverEventBool : OSCReceiverEvent<OSCEventBool>
    {
        #region Protected Methods

        protected override void Invoke(OSCMessage message)
        {
            bool value;

            if (message.ToBool(out value))
            {
                if (onReceive != null)
                    onReceive.Invoke(value);
            }
        }

        #endregion
    }
}