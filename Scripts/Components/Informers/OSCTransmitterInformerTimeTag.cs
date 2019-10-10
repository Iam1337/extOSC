/* Copyright (c) 2019 ExT (V.Sigalkin) */

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