/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using System;

namespace extOSC
{
	[Serializable]
	public struct OSCMidi
	{
		#region Public Vars

		public byte Channel;

		public byte Status;

		public byte Data1;

		public byte Data2;

		#endregion

		#region Public Methods

		public OSCMidi(byte channel, byte status, byte data1, byte data2)
		{
			Channel = channel;
			Status = status;
			Data1 = data1;
			Data2 = data2;
		}

		public override string ToString()
		{
			return $"({Channel}, {Status}, {Data1}, {Data2})";
		}

		#endregion
	}
}