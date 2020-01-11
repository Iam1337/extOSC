/* Copyright (c) 2019 ExT (V.Sigalkin) */

namespace extOSC.Core.Packers
{
	internal class OSCPackerMidi : OSCPacker<OSCMidi>
	{
		#region Public Methods

		public override OSCValueType PackerType => OSCValueType.Midi;

		#endregion

		#region Protected Methods

		protected override OSCMidi BytesToValue(byte[] buffer, ref int index)
		{
			index += 4;

			return new OSCMidi(buffer[index - 4],
							   buffer[index - 3],
							   buffer[index - 2],
							   buffer[index - 1]);
		}

		protected override void ValueToBytes(byte[] buffer, ref int index, OSCMidi value)
		{
			buffer[index++] = value.Channel;
			buffer[index++] = value.Status;
			buffer[index++] = value.Data1;
			buffer[index++] = value.Data2;
		}

		#endregion
	}
}