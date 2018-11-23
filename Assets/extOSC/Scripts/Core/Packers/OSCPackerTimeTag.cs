/* Copyright (c) 2018 ExT (V.Sigalkin) */

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

        public override OSCValueType GetPackerType()
        {
            return OSCValueType.TimeTag;
        }

        #endregion


        #region Private Vars

        private readonly byte[] _dataSeconds = new byte[sizeof(uint)];

        private readonly byte[] _dataMilliseconds = new byte[sizeof(uint)];

        #endregion
        
        #region Protected Methods

        protected override DateTime BytesToValue(byte[] buffer, ref int index)
        {
            const int size = sizeof(uint);

            var dataSeconds = new byte[size];
            for (var i = 0; i < size; i++)
            {
                dataSeconds[i] = buffer[index];

                index++;
            }

            var dataFractional = new byte[size];
            for (var i = 0; i < size; i++)
            {
                dataFractional[i] = buffer[index];

                index++;
            }

            var seconds =
                BitConverter.ToUInt32(BitConverter.IsLittleEndian ? ReverseBytes(dataSeconds) : dataSeconds, 0);
            var fractional =
                BitConverter.ToUInt32(BitConverter.IsLittleEndian ? ReverseBytes(dataFractional) : dataFractional, 0);

            return _zeroTime.AddSeconds(seconds).AddMilliseconds(fractional);
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