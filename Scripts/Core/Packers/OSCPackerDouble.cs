/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using System;

namespace extOSC.Core.Packers
{
	internal class OSCPackerDouble : OSCPacker<double>
	{
		#region Public Methods

		public override OSCValueType PackerType => OSCValueType.Double;

		#endregion

		#region Private Vars

		private readonly byte[] _data = new byte[sizeof(double)];

		#endregion

		#region Protected Methods

		protected override double BytesToValue(byte[] buffer, ref int index)
		{
			_data[0] = buffer[index++];
			_data[1] = buffer[index++];
			_data[2] = buffer[index++];
			_data[3] = buffer[index++];
			_data[4] = buffer[index++];
			_data[5] = buffer[index++];
			_data[6] = buffer[index++];
			_data[7] = buffer[index++];

			if (BitConverter.IsLittleEndian)
				Array.Reverse(_data);

			return BitConverter.ToDouble(_data, 0);
		}

		protected override void ValueToBytes(byte[] buffer, ref int index, double value)
		{
			// TODO: To marshall structure
			var data = BitConverter.GetBytes(value);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(data);

			buffer[index++] = data[0];
			buffer[index++] = data[1];
			buffer[index++] = data[2];
			buffer[index++] = data[3];
			buffer[index++] = data[4];
			buffer[index++] = data[5];
			buffer[index++] = data[6];
			buffer[index++] = data[7];
		}

		#endregion
	}
}