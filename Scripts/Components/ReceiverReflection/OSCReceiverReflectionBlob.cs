/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
	[AddComponentMenu("extOSC/Components/Receiver/Blob Reflection")]
	public class OSCReceiverReflectionBlob : OSCReceiverReflection<byte[]>
	{
		#region Protected Methods

		protected override bool ProcessMessage(OSCMessage message, out byte[] value) => message.ToBlob(out value);

		#endregion
	}
}