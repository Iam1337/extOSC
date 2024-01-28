/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
	[AddComponentMenu("extOSC/Components/Receiver/Vector4 Event")]
	public class OSCReceiverEventVector4 : OSCReceiverEvent<OSCEventVector4>
	{
		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (onReceive != null && message.ToVector4(out var value))
			{
				onReceive.Invoke(value);
			}
		}

		#endregion
	}
}