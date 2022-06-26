/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine.Events;

namespace extOSC.Core.Events
{
	[System.Serializable]
	public class OSCEventBlob : UnityEvent<byte[]>
	{ }
}