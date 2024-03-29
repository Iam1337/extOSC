﻿/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Informers
{
	[AddComponentMenu("extOSC/Components/Transmitter/Double Informer")]
	public class OSCTransmitterInformerDouble : OSCTransmitterInformer<double>
	{
		#region Protected Methods

		protected override void FillMessage(OSCMessage message, double value) => message.AddValue(OSCValue.Double(value));

		#endregion
	}
}