/* Copyright (c) 2019 ExT (V.Sigalkin) */

namespace extOSC.Core
{
	public enum OSCConsolePacketType
	{
		Received,

		Transmitted
	}

	public class OSCConsolePacket
	{
		#region Public Vars

		public IOSCPacket IoscPacket
		{
			get { return _ioscPacket; }
			set
			{
				_ioscPacket = value;
				_description = null;
			}
		}

		public OSCConsolePacketType PacketType
		{
			get { return _packetType; }
			set
			{
				_packetType = value;
				_description = null;
			}
		}

		public string Info
		{
			get { return _info; }
			set
			{
				_info = value;
				_description = null;
			}
		}

		#endregion

		#region Private Vars

		private IOSCPacket _ioscPacket;

		private OSCConsolePacketType _packetType;

		private string _info;

		private string _description;

		#endregion

		#region Public Methods

#if UNITY_EDITOR
		public override string ToString()
		{
			if (_description == null && _ioscPacket != null)
			{
				var packetDescription = string.Empty;
				if (!_ioscPacket.IsBundle())
				{
					packetDescription = string.Format("<color=orange>Message:</color> {0}", _ioscPacket.Address);
				}

				var bundle = _ioscPacket as OSCBundle;
				if (bundle != null)
				{
					packetDescription =
						string.Format("<color=yellow>Bundle:</color> (Packets: {0})", bundle.Packets.Count);
				}

				_description = packetDescription + "\n" + _info;
			}

			return _description;
		}
#endif

		#endregion
	}
}