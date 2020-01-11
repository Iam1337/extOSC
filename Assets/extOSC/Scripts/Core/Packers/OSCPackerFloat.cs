/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;

namespace extOSC.Core.Packers
{
	internal class OSCPackerFloat : OSCPacker<float>
	{
		#region Public Methods

		public override OSCValueType PackerType => OSCValueType.Float;

		#endregion

		#region Private Vars

		private readonly byte[] _data = new byte[sizeof(int)];

		#endregion

		#region Protected Methods

		protected override float BytesToValue(byte[] buffer, ref int index)
		{
			_data[0] = buffer[index++];
			_data[1] = buffer[index++];
			_data[2] = buffer[index++];
			_data[3] = buffer[index++];

			if (BitConverter.IsLittleEndian)
				Array.Reverse(_data);

			return BitConverter.ToSingle(_data, 0);
		}

		protected override void ValueToBytes(byte[] buffer, ref int index, float value)
		{
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