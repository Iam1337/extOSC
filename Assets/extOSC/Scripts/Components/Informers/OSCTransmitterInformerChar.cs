﻿/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Informers
{
	[AddComponentMenu("extOSC/Components/Transmitter/Char Informer")]
	public class OSCTransmitterInformerChar : OSCTransmitterInformer<char>
	{
		#region Protected Methods

		protected override void FillMessage(OSCMessage message, char value) => message.AddValue(OSCValue.Char(value));

		#endregion
	}
}