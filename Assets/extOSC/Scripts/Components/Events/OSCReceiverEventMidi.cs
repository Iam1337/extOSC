/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
    [AddComponentMenu("extOSC/Components/Receiver/Midi Event")]
    public class OSCReceiverEventMidi : OSCReceiverEvent<OSCEventMidi>
    {
        #region Protected Methods

        protected override void Invoke(OSCMessage message)
        {
            OSCMidi value;

            if (message.ToMidi(out value))
            {
                if (onReceive != null)
                    onReceive.Invoke(value);
            }
        }

        #endregion
    }
}