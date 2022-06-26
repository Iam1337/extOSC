/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
	[AddComponentMenu("extOSC/Components/Receiver/Bool Reflection")]
	public class OSCReceiverReflectionBool : OSCReceiverReflection<bool>
	{
		#region Protected Methods

		protected override bool ProcessMessage(OSCMessage message, out bool value) => message.ToBool(out value);

		#endregion
	}
}