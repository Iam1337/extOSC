/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Informers
{
	[AddComponentMenu("extOSC/Components/Transmitter/Int Informer")]
	public class OSCTransmitterInformerInt : OSCTransmitterInformer<int>
	{
		#region Protected Methods

		protected override void FillMessage(OSCMessage message, int value) => message.AddValue(OSCValue.Int(value));

		#endregion
	}
}