/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
    [AddComponentMenu("extOSC/Components/Receiver/Impulse Event")]
    public class OSCReceiverEventImpulse : OSCReceiverEvent<OSCEventImpulse>
    {
        #region Protected Methods

        protected override void Invoke(OSCMessage message)
        {
            var values = message.GetValues(OSCValueType.Impulse);
            if (values.Length > 0)
            {
                if (onReceive != null)
                    onReceive.Invoke();
            }
        }

        #endregion
    }
}