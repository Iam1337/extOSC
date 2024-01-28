/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine.Events;

namespace extOSC.Core.Events
{
	[System.Serializable]
	public class OSCEventArray : UnityEvent<System.Collections.Generic.List<OSCValue>>
	{ }
}