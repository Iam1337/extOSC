/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

namespace extOSC.Components.ReceiverReflections
{
	[AddComponentMenu("extOSC/Components/Receiver/Char Reflection")]
	public class OSCReceiverReflectionChar : OSCReceiverReflection<char>
	{
		#region Protected Methods

		protected override bool ProcessMessage(OSCMessage message, out char value) => message.ToChar(out value);

		#endregion
	}
}