/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
    [AddComponentMenu("extOSC/Components/Receiver/Blob Event")]
    public class OSCReceiverEventBlob : OSCReceiverEvent<OSCEventBlob>
    {
        #region Protected Methods

        protected override void Invoke(OSCMessage message)
        {
            byte[] value;

            if (message.ToBlob(out value))
            {
                if (onReceive != null)
                    onReceive.Invoke(value);
            }
        }

        #endregion
    }
}