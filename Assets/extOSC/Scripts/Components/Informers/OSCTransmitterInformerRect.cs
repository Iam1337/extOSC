/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Informers
{
	[AddComponentMenu("extOSC/Components/Transmitter/Rect Informer")]
	public class OSCTransmitterInformerRect : OSCTransmitterInformer<Rect>
	{
		#region Protected Methods

		protected override void FillMessage(OSCMessage message, Rect value)
		{
			message.AddValue(OSCValue.Float(value.x));
			message.AddValue(OSCValue.Float(value.y));
			message.AddValue(OSCValue.Float(value.width));
			message.AddValue(OSCValue.Float(value.height));
		}

		#endregion
	}
}