/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine.Events;

using System;

namespace extOSC.Core.Events
{
	[Obsolete]
	public interface IOSCEvent
	{
		Type EventType { get; }
	}

	[Serializable]
	[Obsolete]
	public class OSCEvent : UnityEvent, IOSCEvent
	{
		#region Public Vars

		public Type EventType
		{
			get { return null; }
		}

		#endregion
	}

	[Serializable]
	[Obsolete]
	public class OSCEvent<T> : UnityEvent<T>, IOSCEvent
	{
		#region Public Vars

		public Type EventType => typeof(T);

		#endregion
	}
}