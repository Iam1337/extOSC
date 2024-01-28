/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Informers
{
	[AddComponentMenu("extOSC/Components/Transmitter/Int To Float Informer")]
	public class OSCTransmitterInformerIntToFloat : OSCTransmitterInformer<int>
	{
		#region Protected Methods

		protected override void FillMessage(OSCMessage message, int value) => message.AddValue(OSCValue.Float(value));

		#endregion
	}
}