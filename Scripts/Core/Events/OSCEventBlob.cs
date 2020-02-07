/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine.Events;

namespace extOSC.Core.Events
{
	[System.Serializable]
	public class OSCEventBlob : UnityEvent<byte[]>
	{ }
}