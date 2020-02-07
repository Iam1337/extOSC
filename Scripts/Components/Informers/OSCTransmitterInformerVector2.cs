/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Informers
{
	[AddComponentMenu("extOSC/Components/Transmitter/Vector2 Informer")]
	public class OSCTransmitterInformerVector2 : OSCTransmitterInformer<Vector2>
	{
		#region Protected Methods

		protected override void FillMessage(OSCMessage message, Vector2 value)
		{
			message.AddValue(OSCValue.Float(value.x));
			message.AddValue(OSCValue.Float(value.y));
		}

		#endregion
	}
}