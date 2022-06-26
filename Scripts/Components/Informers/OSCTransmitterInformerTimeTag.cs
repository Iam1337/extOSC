/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using System;

namespace extOSC.Components.Informers
{
	[AddComponentMenu("extOSC/Components/Transmitter/TimeTag Informer")]
	public class OSCTransmitterInformerTimeTag : OSCTransmitterInformer<DateTime>
	{
		#region Protected Methods

		protected override void FillMessage(OSCMessage message, DateTime value) => message.AddValue(OSCValue.TimeTag(value));

		#endregion
	}
}