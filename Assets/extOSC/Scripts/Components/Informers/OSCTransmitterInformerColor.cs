/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Informers
{
	[AddComponentMenu("extOSC/Components/Transmitter/Color Informer")]
	public class OSCTransmitterInformerColor : OSCTransmitterInformer<Color>
	{
		#region Protected Methods

		protected override void FillMessage(OSCMessage message, Color value) => message.AddValue(OSCValue.Color(value));

		#endregion
	}
}