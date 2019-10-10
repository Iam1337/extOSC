/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
	[AddComponentMenu("extOSC/Components/Receiver/Vector3 Event")]
	public class OSCReceiverEventVector3 : OSCReceiverEvent<OSCEventVector3>
	{
		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (onReceive != null && message.ToVector3(out var value))
			{
				onReceive.Invoke(value);
			}
		}

		#endregion
	}
}