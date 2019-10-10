/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
	[AddComponentMenu("extOSC/Components/Receiver/Impulse Event")]
	public class OSCReceiverEventImpulse : OSCReceiverEvent<OSCEventImpulse>
	{
		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (onReceive != null && message.HasImpulse())
			{
				onReceive.Invoke();
			}
		}

		#endregion
	}
}