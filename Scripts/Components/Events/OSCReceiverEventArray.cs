﻿/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
	[AddComponentMenu("extOSC/Components/Receiver/Array Event")]
	public class OSCReceiverEventArray : OSCReceiverEvent<OSCEventArray>
	{
		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (onReceive != null && message.ToArray(out var value))
			{
				onReceive.Invoke(value);
			}
		}

		#endregion
	}
}