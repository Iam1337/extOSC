/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
	[AddComponentMenu("extOSC/Components/Receiver/String Reflection")]
	public class OSCReceiverReflectionString : OSCReceiverReflection<string>
	{
		#region Protected Methods

		protected override bool ProcessMessage(OSCMessage message, out string value) => message.ToString(out value);

		#endregion
	}
}