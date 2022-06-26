/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
	[AddComponentMenu("extOSC/Components/Receiver/Null Event")]
	public class OSCReceiverEventNull : OSCReceiverEvent<OSCEventNull>
	{
		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (onReceive != null && message.HasNull())
			{
				onReceive.Invoke();
			}
		}

		#endregion
	}
}