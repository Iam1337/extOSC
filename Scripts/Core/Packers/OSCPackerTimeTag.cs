/* Copyright (c) 2020 ExT (V.Sigalkin) */

using System;
using System.Collections.Generic;

namespace extOSC.Core.Packers
{
	internal class OSCPackerTimeTag : OSCPacker<DateTime>
	{
		#region Private Static Vars

		private static readonly DateTime _zeroTime = new DateTime(1900, 1, 1, 0, 0, 0, 0);

		#endregion

		#region Public Methods

		public override OSCValueType PackerType => OSCValueType.TimeTag;

		#endregion

		#region Private Vars

		private readonly byte[] _dataSeconds = new byte[sizeof(uint)];

		private readonly byte[] _dataMilliseconds = new byte[sizeof(uint)];

		#endregion

		#region Protected Methods

		protected override DateTime BytesToValue(byte[] buffer, ref int index)
		{
			_dataSeconds[0] = buffer[index++];
			_dataSeconds[1] = buffer[index++];
			_dataSeconds[2] = buffer[index++];
			_dataSeconds[3] = buffer[index++];
			_dataMilliseconds[0] = buffer[index++];
			_dataMilliseconds[1] = buffer[index++];
			_dataMilliseconds[2] = buffer[index++];
			_dataMilliseconds[3] = buffer[index++];


			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(_dataSeconds);
				Array.Reverse(_dataMilliseconds);
			}

			var seconds = BitConverter.ToUInt32(_dataSeconds, 0);
			var milliseconds = BitConverter.ToUInt32(_dataMilliseconds, 0);

			return _zeroTime.AddSeconds(seconds).AddMilliseconds(milliseconds);
		}

		protected override void ValueToBytes(byte[] buffer, ref int index, DateTime value)
		{
			var deltaTime = value - _zeroTime;

			var seconds = (uint) deltaTime.TotalSeconds;
			var milliseconds = (uint) deltaTime.Milliseconds;

			var dataSeconds = BitConverter.GetBytes(seconds);
			var dataMilliseconds = BitConverter.GetBytes(milliseconds);

			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(dataSeconds);
				Array.Reverse(dataMilliseconds);
			}

			buffer[index++] = dataSeconds[0];
			buffer[index++] = dataSeconds[1];
			buffer[index++] = dataSeconds[2];
			buffer[index++] = dataSeconds[3];
			buffer[index++] = dataMilliseconds[0];
			buffer[index++] = dataMilliseconds[1];
			buffer[index++] = dataMilliseconds[2];
			buffer[index++] = dataMilliseconds[3];
		}

		#endregion
	}
}