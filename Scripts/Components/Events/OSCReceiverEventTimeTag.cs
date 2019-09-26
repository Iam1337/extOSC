/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core;
using extOSC.Core.Events;

namespace extOSC.Components.Events
{
    [AddComponentMenu("extOSC/Components/Receiver/TimeTag Event")]
    public class OSCReceiverEventTimeTag : OSCReceiverEvent<OSCEventDateTime>
    {
        #region Protected Methods

        protected override void Invoke(OSCMessage message)
        {
            var values = message.FindValues(OSCValueType.TimeTag);
            if (values.Length > 0)
            {
                var value = values[0];

                if (onReceive != null)
                    onReceive.Invoke(value.TimeTagValue);
            }
        }

        #endregion
    }
}