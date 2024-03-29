﻿/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Informers
{
	[AddComponentMenu("extOSC/Components/Transmitter/Float To Int Informer")]
	public class OSCTransmitterInformerFloatToInt : OSCTransmitterInformer<float>
	{
		#region Protected Methods

		protected override void FillMessage(OSCMessage message, float value) => message.AddValue(OSCValue.Int((int) value));

		#endregion
	}
}