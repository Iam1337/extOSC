/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
	[AddComponentMenu("extOSC/Components/Receiver/Message Event")]
	public class OSCReceiverEventMessage : OSCReceiverEvent<OSCEventMessage>
	{
		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (onReceive != null)
				onReceive.Invoke(message);
		}

		#endregion
	}
}