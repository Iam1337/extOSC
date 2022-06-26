/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
	[AddComponentMenu("extOSC/Components/Receiver/Int Reflection")]
	public class OSCReceiverReflectionInt : OSCReceiverReflection<int>
	{
		#region Protected Methods

		protected override bool ProcessMessage(OSCMessage message, out int value) => message.ToInt(out value);

		#endregion
	}
}