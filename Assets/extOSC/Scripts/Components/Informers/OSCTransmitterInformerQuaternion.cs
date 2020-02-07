/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Informers
{
	[AddComponentMenu("extOSC/Components/Transmitter/Quaternion Informer")]
	public class OSCTransmitterInformerQuaternion : OSCTransmitterInformer<Quaternion>
	{
		#region Protected Methods

		protected override void FillMessage(OSCMessage message, Quaternion value)
		{
			message.AddValue(OSCValue.Float(value.x));
			message.AddValue(OSCValue.Float(value.y));
			message.AddValue(OSCValue.Float(value.z));
			message.AddValue(OSCValue.Float(value.w));
		}

		#endregion
	}
}