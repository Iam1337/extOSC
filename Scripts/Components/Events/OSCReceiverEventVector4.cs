/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
    [AddComponentMenu("extOSC/Components/Receiver/Vector4 Event")]
    public class OSCReceiverEventVector4 : OSCReceiverEvent<OSCEventVector4>
    {
        #region Protected Methods

        protected override void Invoke(OSCMessage message)
        {
            Vector4 value;

            if (message.ToVector4(out value))
            {
                if (onReceive != null)
                    onReceive.Invoke(value);
            }
        }

        #endregion
    }
}