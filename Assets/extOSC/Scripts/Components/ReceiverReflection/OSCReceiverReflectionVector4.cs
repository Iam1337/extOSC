/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
	[AddComponentMenu("extOSC/Components/Receiver/Vector4 Reflection")]
	public class OSCReceiverReflectionVector4 : OSCReceiverReflection<Vector4>
	{
		#region Protected Methods

		protected override bool ProcessMessage(OSCMessage message, out Vector4 value) => message.ToVector4(out value);

		#endregion
	}
}