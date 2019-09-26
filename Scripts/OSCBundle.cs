/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;
using System.Net;
using System.Collections.Generic;

using extOSC.Core;
using JetBrains.Annotations;

namespace extOSC
{
    public class OSCBundle : IOSCPacket
    {
        #region Static Public Methods

        public static OSCBundle Create(params IOSCPacket[] packets)
        {
            return new OSCBundle(packets);
        }

        #endregion

        #region Constants

		[Obsolete("Use BundleAddress constant.")]
		public const string KBundle = "#bundle";

        public const string BundleAddress = "#bundle";

        #endregion

        #region Public Vars

		public string Address => "#bundle";

		public IPAddress Ip { get; set; }

		public int Port { get; set; }

		public List<IOSCPacket> Packets { get; } = new List<IOSCPacket>();

		public long TimeStamp { get; set; }

		#endregion

        #region Public Methods

        public OSCBundle() { }

		public OSCBundle(params IOSCPacket[] packets)
		{
			AddRange(packets);
		}

		public void AddPacket(IOSCPacket ioscPacket)
        {
			if (ioscPacket == null)
				throw new NullReferenceException(nameof(ioscPacket));

			Packets.Add(ioscPacket);
		}

		public void AddRange(IEnumerable<IOSCPacket> packets)
		{
			if (packets == null)
				throw new NullReferenceException(nameof(packets));

			Packets.AddRange(packets);
		}

		public bool IsBundle() => true;

		public IOSCPacket Copy()
		{
			var packetsCount = Packets.Count;
			var packets = new IOSCPacket[packetsCount];

			for (var i = 0; i < packetsCount; ++i)
			{
				packets[i] = Packets[i].Copy();
			}

			return new OSCBundle(packets);
        }

		public override string ToString()
		{
			var stringValues = string.Empty;

			if (Packets.Count > 0)
			{
				foreach (var packet in Packets)
				{
					stringValues += $"[{packet}], ";
				}

				stringValues = $"({stringValues.Remove(stringValues.Length - 2)})";
			}

			return $"<{GetType().Name}> : {(string.IsNullOrEmpty(stringValues) ? "null" : stringValues)}";
		}

        #endregion
    }
}