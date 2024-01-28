/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
	[AddComponentMenu("extOSC/Components/Receiver/Bool Event")]
	public class OSCReceiverEventBool : OSCReceiverEvent<OSCEventBool>
	{
		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (onReceive != null && message.ToBool(out var value))
			{
				onReceive.Invoke(value);
			}
		}

		#endregion
	}
}