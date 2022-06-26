/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
	[AddComponentMenu("extOSC/Components/Receiver/Quaternion Reflection")]
	public class OSCReceiverReflectionQuaternion : OSCReceiverReflection<Quaternion>
	{
		#region Protected Methods

		protected override bool ProcessMessage(OSCMessage message, out Quaternion value) => message.ToQuaternion(out value);

		#endregion
	}
}