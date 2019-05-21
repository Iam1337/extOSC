/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
    [AddComponentMenu("extOSC/Components/Receiver/Vector2 Event")]
    public class OSCReceiverEventVector2 : OSCReceiverEvent<OSCEventVector2>
    {
        #region Protected Methods

        protected override void Invoke(OSCMessage message)
        {
            Vector2 value;

            if (message.ToVector2(out value))
            {
                if (onReceive != null)
                    onReceive.Invoke(value);
            }
        }

        #endregion
    }
}