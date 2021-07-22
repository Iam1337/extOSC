﻿/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
	[AddComponentMenu("extOSC/Components/Receiver/Midi Reflection")]
	public class OSCReceiverReflectionMidi : OSCReceiverReflection<OSCMidi>
	{
		#region Protected Methods

		protected override bool ProcessMessage(OSCMessage message, out OSCMidi value) => message.ToMidi(out value);

		#endregion
	}
}