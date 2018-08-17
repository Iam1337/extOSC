/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Informers
{
    [AddComponentMenu("extOSC/Components/Transmitter/Bool (To Int) Informer")]
    public class OSCTransmitterInformerBoolToInt : OSCTransmitterInformer<bool>
    {
        #region Protected Methods

        protected override void FillMessage(OSCMessage message, bool value)
        {
            message.AddValue(OSCValue.Int(value ? 1 : 0));
        }

        #endregion
    }
}