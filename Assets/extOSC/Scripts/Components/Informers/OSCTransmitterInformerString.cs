/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Informers
{
	[AddComponentMenu("extOSC/Components/Transmitter/String Informer")]
	public class OSCTransmitterInformerString : OSCTransmitterInformer<string>
	{
		#region Protected Methods

		protected override void FillMessage(OSCMessage message, string value) => message.AddValue(OSCValue.String(value));

		#endregion
	}
}