/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Informers
{
    [AddComponentMenu("extOSC/Components/Transmitter/Bool (To Float) Informer")]
    public class OSCTransmitterInformerBoolToFloat : OSCTransmitterInformer<bool>
    {
        #region Protected Methods

        protected override void FillMessage(OSCMessage message, bool value)
        {
            message.AddValue(OSCValue.Float(value ? 1.0f : 0.0f));
        }

        #endregion
    }
}