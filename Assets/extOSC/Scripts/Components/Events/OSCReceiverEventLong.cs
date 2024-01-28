/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
	[AddComponentMenu("extOSC/Components/Receiver/Long Event")]
	public class OSCReceiverEventLong : OSCReceiverEvent<OSCEventLong>
	{
		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (onReceive != null && message.ToLong(out var value))
			{
				onReceive.Invoke(value);
			}
		}

		#endregion
	}
}