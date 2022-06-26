/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Informers
{
	[AddComponentMenu("extOSC/Components/Transmitter/Vector3 Informer")]
	public class OSCTransmitterInformerVector3 : OSCTransmitterInformer<Vector3>
	{
		#region Protected Methods

		protected override void FillMessage(OSCMessage message, Vector3 value)
		{
			message.AddValue(OSCValue.Float(value.x));
			message.AddValue(OSCValue.Float(value.y));
			message.AddValue(OSCValue.Float(value.z));
		}

		#endregion
	}
}