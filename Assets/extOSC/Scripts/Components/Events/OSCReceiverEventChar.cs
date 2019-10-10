/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
	[AddComponentMenu("extOSC/Components/Receiver/Char Event")]
	public class OSCReceiverEventChar : OSCReceiverEvent<OSCEventChar>
	{
		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (onReceive != null && message.ToChar(out var value))
			{
				onReceive.Invoke(value);
			}
		}

		#endregion
	}
}