/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
	[AddComponentMenu("extOSC/Components/Receiver/Vector2 Event")]
	public class OSCReceiverEventVector2 : OSCReceiverEvent<OSCEventVector2>
	{
		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (onReceive != null && message.ToVector2(out var value))
			{
				onReceive.Invoke(value);
			}
		}

		#endregion
	}
}