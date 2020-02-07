/* Copyright (c) 2020 ExT (V.Sigalkin) */

using System;

namespace extOSC.Core.Packers
{
	internal class OSCPackerChar : OSCPacker<char>
	{
		#region Public Methods

		public override OSCValueType PackerType => OSCValueType.Char;

		#endregion

		#region Private Vars

		private readonly byte[] _data = new byte[sizeof(char)];

		#endregion

		#region Protected Methods

		protected override char BytesToValue(byte[] buffer, ref int index)
		{
			_data[0] = buffer[index++];
			_data[1] = buffer[index++];

			if (BitConverter.IsLittleEndian)
				Array.Reverse(_data);

			return BitConverter.ToChar(_data, 0);
		}

		protected override void ValueToBytes(byte[] buffer, ref int index, char value)
		{
			// TODO: To marshall structure
			var data = BitConverter.GetBytes(value);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(data);

			buffer[index++] = data[0];
			buffer[index++] = data[1];
		}

		#endregion
	}
}