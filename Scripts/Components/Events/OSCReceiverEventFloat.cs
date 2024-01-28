/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
	[AddComponentMenu("extOSC/Components/Receiver/Float Event")]
	public class OSCReceiverEventFloat : OSCReceiverEvent<OSCEventFloat>
	{
		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (onReceive != null && message.ToFloat(out var value))
			{
				onReceive.Invoke(value);
			}
		}

		#endregion
	}
}