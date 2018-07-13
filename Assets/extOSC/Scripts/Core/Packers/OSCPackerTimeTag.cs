/* Copyright (c) 2018 ExT (V.Sigalkin) */

using System;
using System.Collections.Generic;

namespace extOSC.Core.Packers
{
    class OSCPackerTimeTag : OSCPacker<DateTime>
    {
        #region Private Static Vars

        private static readonly DateTime _epoch = new DateTime(1900, 1, 1, 0, 0, 0, 0);

        #endregion

        #region Public Methods

        public override OSCValueType GetPackerType()
        {
            return OSCValueType.TimeTag;
        }

        #endregion

        #region Protected Methods

        protected override DateTime BytesToValue(byte[] bytes, ref int start)
        {
            const int size = sizeof(uint);

            var dataSeconds = new byte[size];
            for (var i = 0; i < size; i++)
            {
                dataSeconds[i] = bytes[start];

                start++;
            }

            var dataFractional = new byte[size];
            for (var i = 0; i < size; i++)
            {
                dataFractional[i] = bytes[start];

                start++;
            }

            var seconds = BitConverter.ToUInt32(BitConverter.IsLittleEndian ? ReverseBytes(dataSeconds) : dataSeconds, 0);
            var fractional = BitConverter.ToUInt32(BitConverter.IsLittleEndian ? ReverseBytes(dataFractional) : dataFractional, 0);

            return _epoch.AddSeconds(seconds).AddMilliseconds(fractional);
        }

        protected override byte[] ValueToBytes(DateTime value)
        {
            var bytes = new List<byte>();

            var timeOffset = (value - _epoch);
            var seconds = (uint)timeOffset.TotalSeconds;
            var fractional = (uint)timeOffset.Milliseconds;

            var dataSeconds = BitConverter.GetBytes(seconds);
            var dataFractional = BitConverter.GetBytes(fractional);

            bytes.AddRange(BitConverter.IsLittleEndian ? ReverseBytes(dataSeconds) : dataSeconds);
            bytes.AddRange(BitConverter.IsLittleEndian ? ReverseBytes(dataFractional) : dataFractional);

            return bytes.ToArray();
        }

        #endregion
    }
}