/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Informers
{
	[AddComponentMenu("extOSC/Components/Transmitter/Blob Informer")]
	public class OSCTransmitterInformerBlob : OSCTransmitterInformer<byte[]>
	{
		#region Protected Methods

		protected override void FillMessage(OSCMessage message, byte[] value) => message.AddValue(OSCValue.Blob(value));

		#endregion
	}
}