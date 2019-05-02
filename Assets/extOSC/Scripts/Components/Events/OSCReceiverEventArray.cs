/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System.Collections.Generic;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
    [AddComponentMenu("extOSC/Components/Receiver/Array Event")]
    public class OSCReceiverEventArray : OSCReceiverEvent<OSCEventArray>
    {
        #region Protected Methods

        protected override void Invoke(OSCMessage message)
        {
            List<OSCValue> value;

            if (message.ToArray(out value))
            {
                if (onReceive != null)
                    onReceive.Invoke(value);
            }
        }

        #endregion
    }
}