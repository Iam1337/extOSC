/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Informers
{
	[AddComponentMenu("extOSC/Components/Transmitter/Bool Informer")]
	public class OSCTransmitterInformerBool : OSCTransmitterInformer<bool>
	{
		#region Protected Methods

		protected override void FillMessage(OSCMessage message, bool value) => message.AddValue(OSCValue.Bool(value));

		#endregion
	}
}