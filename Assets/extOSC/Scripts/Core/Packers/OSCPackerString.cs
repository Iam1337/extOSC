/* Copyright (c) 2020 ExT (V.Sigalkin) */

using System;
using System.Text;

namespace extOSC.Core.Packers
{
	internal class OSCPackerString : OSCPacker<string>
	{
		#region Public Methods

		public override OSCValueType PackerType => OSCValueType.String;

		#endregion

		#region Protected Methods

		protected override string BytesToValue(byte[] buffer, ref int index)
		{
			var length = 0;
			var position = index;

			while (buffer[position] != 0 && position < buffer.Length)
			{
				position++;
				length++;
			}

#if !EXTOSC_UTF8
			var value = Encoding.ASCII.GetString(buffer, index, length);
#else
            var value = Encoding.UTF8.GetString(buffer, index, length);
#endif
			index += length + (4 - length % 4);

			return value;
		}

		protected override void ValueToBytes(byte[] buffer, ref int index, string value)
		{
#if !EXTOSC_UTF8
			var data = Encoding.ASCII.GetBytes(value);
#else
            var data = Encoding.UTF8.GetBytes(value);
#endif

			Array.Copy(data, 0, buffer, index, data.Length);

			index += data.Length;

			IncludeZeroBytes(buffer, data.Length, ref index);
		}

		#endregion
	}
}