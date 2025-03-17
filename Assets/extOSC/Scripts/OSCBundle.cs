/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using System;
using System.Net;
using System.Collections.Generic;

using extOSC.Core;

namespace extOSC
{
	public class OSCBundle : IOSCPacket
	{
		#region Constants

		public const string BundleAddress = "#bundle";

		#endregion

		#region Public Vars

		public string Address => "#bundle";

		public IPAddress Ip
		{
			get => _ip;
			set
			{
				_ip = value;

				for (var i = 0; i < Packets.Count; i++) 
					Packets[i].Ip = value;
			}
		}

		public int Port
		{
			get => _port;
			set
			{
				_port = value;

				for (var i = 0; i < Packets.Count; i++) 
					Packets[i].Port = value;
			}
		}

		public List<IOSCPacket> Packets { get; } = new List<IOSCPacket>();

		public long TimeStamp { get; set; }

		#endregion

		#region Private Vars

		private IPAddress _ip;
		private int _port;

		#endregion

		#region Public Methods

		public OSCBundle()
		{ }

		public OSCBundle(params IOSCPacket[] packets)
		{
			AddRange(packets);
		}

		public void AddPacket(IOSCPacket packet)
		{
			if (packet == null)
				throw new NullReferenceException(nameof(packet));

			Packets.Add(packet);
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