/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
	[AddComponentMenu("extOSC/Components/Receiver/Double Reflection")]
	public class OSCReceiverReflectionDouble : OSCReceiverReflection<double>
	{
		#region Protected Methods

		protected override bool ProcessMessage(OSCMessage message, out double value) => message.ToDouble(out value);

		#endregion
	}
}