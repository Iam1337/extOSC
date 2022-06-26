/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
	[AddComponentMenu("extOSC/Components/Receiver/String Event")]
	public class OSCReceiverEventString : OSCReceiverEvent<OSCEventString>
	{
		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (onReceive != null && message.ToString(out var value))
			{
				onReceive.Invoke(value);
			}
		}

		#endregion
	}
}