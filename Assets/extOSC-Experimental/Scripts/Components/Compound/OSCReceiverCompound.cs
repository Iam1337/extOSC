/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

using System;

namespace extOSC.Components.Compounds
{
	[AddComponentMenu("extOSC/Components/Receiver/Receiver Compound Message")]
	public class OSCReceiverCompound : OSCReceiverComponent
	{
		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}