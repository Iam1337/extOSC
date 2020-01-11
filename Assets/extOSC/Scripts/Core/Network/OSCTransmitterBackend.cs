/* Copyright (c) 2019 ExT (V.Sigalkin) */

namespace extOSC.Core.Network
{
	public abstract class OSCTransmitterBackend
	{
		#region Static Public Methods

		public static OSCTransmitterBackend Create()
		{
#if NETFX_CORE
            return new OSCTransmitterWindowsStoreBackend();
#else
			return new OSCTransmitterStandaloneBackend();
#endif
		}

		#endregion

		#region Public Vars

		public abstract bool IsAvailable { get; }

		#endregion

		#region Public Methods

		public abstract void Connect(string localHost, int localPort);

		public abstract void RefreshRemote(string remoteHost, int remotePort);

		public abstract void Close();

		public abstract void Send(byte[] data, int length);

		#endregion
	}
}