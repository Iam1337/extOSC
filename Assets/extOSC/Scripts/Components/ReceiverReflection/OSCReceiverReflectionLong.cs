﻿/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
	[AddComponentMenu("extOSC/Components/Receiver/Long Reflection")]
	public class OSCReceiverReflectionLong : OSCReceiverReflection<long>
	{
		#region Protected Methods

		protected override bool ProcessMessage(OSCMessage message, out long value) => message.ToLong(out value);

		#endregion
	}
}