/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
    [AddComponentMenu("extOSC/Components/Receiver/Blob Reflection")]
    public class OSCReceiverReflectionBlob : OSCReceiverReflection<byte[]>
    {
        #region Protected Methods

        protected override bool ProcessMessage(OSCMessage message, out byte[] value)
        {
            value = null;

            if (message.ToBlob(out value))
                return true;

            return false;
        }

        #endregion
    }
}