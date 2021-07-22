﻿/* Copyright (c) 2021 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Informers
{
	[AddComponentMenu("extOSC/Components/Transmitter/Int Informer")]
	public class OSCTransmitterInformerInt : OSCTransmitterInformer<int>
	{
		#region Protected Methods

		protected override void FillMessage(OSCMessage message, int value) => message.AddValue(OSCValue.Int(value));

		#endregion
	}
}