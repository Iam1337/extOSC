﻿/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using System.Collections.Generic;

namespace extOSC.Components.Informers
{
	[AddComponentMenu("extOSC/Components/Transmitter/Array Informer")]
	public class OSCTransmitterInformerArray : OSCTransmitterInformer<List<OSCValue>>
	{
		#region Protected Methods

		protected override void FillMessage(OSCMessage message, List<OSCValue> value) => message.AddValue(OSCValue.Array(value.ToArray()));

		#endregion
	}
}