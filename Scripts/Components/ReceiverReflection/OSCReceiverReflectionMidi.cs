/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
    [AddComponentMenu("extOSC/Components/Receiver/Midi Reflection")]
    public class OSCReceiverReflectionMidi : OSCReceiverReflection<OSCMidi>
    {
        #region Protected Methods

        protected override bool ProcessMessage(OSCMessage message, out OSCMidi value)
        {
            value = new OSCMidi();

            if (message.ToMidi(out value))
                return true;

            return false;
        }

        #endregion
    }
}