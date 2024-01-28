/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
	[AddComponentMenu("extOSC/Components/Receiver/Color Reflection")]
	public class OSCReceiverReflectionColor : OSCReceiverReflection<Color>
	{
		#region Protected Methods

		protected override bool ProcessMessage(OSCMessage message, out Color value) => message.ToColor(out value);

		#endregion
	}
}