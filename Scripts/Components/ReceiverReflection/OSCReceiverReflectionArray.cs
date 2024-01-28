/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using System.Collections.Generic;

namespace extOSC.Components.ReceiverReflections
{
	[AddComponentMenu("extOSC/Components/Receiver/Array Reflection")]
	public class OSCReceiverReflectionArray : OSCReceiverReflection<List<OSCValue>>
	{
		#region Protected Methods

		protected override bool ProcessMessage(OSCMessage message, out List<OSCValue> value) => message.ToArray(out value);

		#endregion
	}
}