﻿/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
	[AddComponentMenu("extOSC/Components/Receiver/Color Event")]
	public class OSCReceiverEventColor : OSCReceiverEvent<OSCEventColor>
	{
		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (onReceive != null && message.ToColor(out var value))
			{
				onReceive.Invoke(value);
			}
		}

		#endregion
	}
}