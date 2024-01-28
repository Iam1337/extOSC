/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
	[AddComponentMenu("extOSC/Components/Receiver/Vector2 Reflection")]
	public class OSCReceiverReflectionVector2 : OSCReceiverReflection<Vector2>
	{
		#region Protected Methods

		protected override bool ProcessMessage(OSCMessage message, out Vector2 value) => message.ToVector2(out value);

		#endregion
	}
}