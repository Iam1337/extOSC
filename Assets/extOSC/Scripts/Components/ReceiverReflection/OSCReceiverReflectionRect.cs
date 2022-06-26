/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
	[AddComponentMenu("extOSC/Components/Receiver/Rect Reflection")]
	public class OSCReceiverReflectionRect : OSCReceiverReflection<Rect>
	{
		#region Protected Methods

		protected override bool ProcessMessage(OSCMessage message, out Rect value) => message.ToRect(out value);

		#endregion
	}
}