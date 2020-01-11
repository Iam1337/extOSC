/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;

namespace extOSC.Core.Packers
{
	internal class OSCPackerInt : OSCPacker<int>
	{
		#region Public Methods

		public override OSCValueType PackerType => OSCValueType.Int;

		#endregion

		#region Private Vars

		private readonly byte[] _data = new byte[sizeof(int)];

		#endregion

		#region Protected Methods

		protected override int BytesToValue(byte[] buffer, ref int index)
		{
			_data[0] = buffer[index++];
			_data[1] = buffer[index++];
			_data[2] = buffer[index++];
			_data[3] = buffer[index++];

			if (BitConverter.IsLittleEndian)
				Array.Reverse(_data);

			return BitConverter.ToInt32(_data, 0);
		}

		protected override void ValueToBytes(byte[] buffer, ref int index, int value)
		{
			// TODO: To marshall structure
			var data = BitConverter.GetBytes(value);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(data);

			buffer[index++] = data[0];
			buffer[index++] = data[1];
			buffer[index++] = data[2];
			buffer[index++] = data[3];
		}

		#endregion
	}
}