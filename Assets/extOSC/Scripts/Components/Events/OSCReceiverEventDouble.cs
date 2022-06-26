/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
	[AddComponentMenu("extOSC/Components/Receiver/Double Event")]
	public class OSCReceiverEventDouble : OSCReceiverEvent<OSCEventDouble>
	{
		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (onReceive != null && message.ToDouble(out var value))
			{
				onReceive.Invoke(value);
			}
		}

		#endregion
	}
}