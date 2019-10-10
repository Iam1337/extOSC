﻿/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System;

namespace extOSC.Components.ReceiverReflections
{
	[AddComponentMenu("extOSC/Components/Receiver/TimeTag Reflection")]
	public class OSCReceiverReflectionTimeTag : OSCReceiverReflection<DateTime>
	{
		#region Protected Methods

		protected override bool ProcessMessage(OSCMessage message, out DateTime value) => message.ToTimeTag(out value);

		#endregion
	}
}