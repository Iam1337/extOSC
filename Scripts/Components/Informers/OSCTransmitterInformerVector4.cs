/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Informers
{
	[AddComponentMenu("extOSC/Components/Transmitter/Vector4 Informer")]
	public class OSCTransmitterInformerVector4 : OSCTransmitterInformer<Vector4>
	{
		#region Protected Methods

		protected override void FillMessage(OSCMessage message, Vector4 value)
		{
			message.AddValue(OSCValue.Float(value.x));
			message.AddValue(OSCValue.Float(value.y));
			message.AddValue(OSCValue.Float(value.z));
			message.AddValue(OSCValue.Float(value.w));
		}

		#endregion
	}
}