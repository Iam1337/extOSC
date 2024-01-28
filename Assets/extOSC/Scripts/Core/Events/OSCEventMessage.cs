/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine.Events;

namespace extOSC.Core.Events
{
	[System.Serializable]
	public class OSCEventMessage : UnityEvent<OSCMessage>
	{ }
}