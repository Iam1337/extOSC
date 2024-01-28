/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using System;

namespace extOSC.Core.Packers
{
	internal class OSCPackerLong : OSCPacker<long>
	{
		#region Public Methods

		public override OSCValueType PackerType => OSCValueType.Long;

		#endregion

		#region Private Vars

		private readonly byte[] _data = new byte[sizeof(long)];

		#endregion

		#region Protected Methods

		protected override long BytesToValue(byte[] buffer, ref int index)
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

			return BitConverter.ToInt64(_data, 0);
		}

		protected override void ValueToBytes(byte[] buffer, ref int index, long value)
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