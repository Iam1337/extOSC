﻿/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Informers
{
	[AddComponentMenu("extOSC/Components/Transmitter/Midi Informer")]
	public class OSCTransmitterInformerMidi : OSCTransmitterInformer<OSCMidi>
	{
		#region Protected Methods

		protected override void FillMessage(OSCMessage message, OSCMidi value) => message.AddValue(OSCValue.Midi(value));

		#endregion
	}
}