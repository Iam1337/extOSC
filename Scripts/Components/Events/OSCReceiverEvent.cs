/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEngine.Events;

namespace extOSC.Components.Events
{
	public abstract class OSCReceiverEvent<T> : OSCReceiverComponent where T : UnityEventBase
	{
		#region Public Vars

		public T OnReceive
		{
			get => onReceive;
			set => onReceive = value;
		}

		#endregion

		#region Protected Vars

		[SerializeField]
		protected T onReceive;

		#endregion
	}
}