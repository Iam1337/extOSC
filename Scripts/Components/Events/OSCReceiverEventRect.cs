/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
	[AddComponentMenu("extOSC/Components/Receiver/Rect Event")]
	public class OSCReceiverEventRect : OSCReceiverEvent<OSCEventRect>
	{
		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (onReceive != null && message.ToRect(out var value))
			{
				onReceive.Invoke(value);
			}
		}

		#endregion
	}
}