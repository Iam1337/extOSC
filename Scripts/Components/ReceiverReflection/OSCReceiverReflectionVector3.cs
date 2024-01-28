/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
	[AddComponentMenu("extOSC/Components/Receiver/Vector3 Reflection")]
	public class OSCReceiverReflectionVector3 : OSCReceiverReflection<Vector3>
	{
		#region Protected Methods

		protected override bool ProcessMessage(OSCMessage message, out Vector3 value) => message.ToVector3(out value);

		#endregion
	}
}