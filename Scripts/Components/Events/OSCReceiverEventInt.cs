/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
	[AddComponentMenu("extOSC/Components/Receiver/Integer Event")]
	public class OSCReceiverEventInt : OSCReceiverEvent<OSCEventInt>
	{
		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (onReceive != null && message.ToInt(out var value))
			{
				onReceive.Invoke(value);
			}
		}

		#endregion
	}
}