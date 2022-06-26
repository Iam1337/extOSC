/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
	[AddComponentMenu("extOSC/Components/Receiver/Quaternion Event")]
	public class OSCReceiverEventQuaternion : OSCReceiverEvent<OSCEventQuaternion>
	{
		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (onReceive != null && message.ToQuaternion(out var value))
			{
				onReceive.Invoke(value);
			}
		}

		#endregion
	}
}