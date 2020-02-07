/* Copyright (c) 2020 ExT (V.Sigalkin) */

using System.Net;

namespace extOSC.Core
{
	public interface IOSCPacket
	{
		#region Vars

		string Address { get; }

		IPAddress Ip { get; set; }

		int Port { get; set; }

		#endregion

		#region Public Methods

		bool IsBundle();

		IOSCPacket Copy();

		#endregion
	}
}