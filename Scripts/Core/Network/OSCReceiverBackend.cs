/* Copyright (c) 2018 ExT (V.Sigalkin) */

namespace extOSC.Core.Network
{
    public abstract class OSCReceiverBackend
    {
        #region Extensions

        public delegate void OSCReceivedCallback(OSCPacket packet);

        #endregion

        #region Public Methods

        public static OSCReceiverBackend Create()
        {
#if NETFX_CORE
            return new OSCReceiverWindowsStoreBackend();
#else
            return new OSCReceiverStandaloneBackend();
#endif
        }

        #endregion

        #region Public Vars

        public abstract OSCReceivedCallback ReceivedCallback { get; set; }

        public abstract bool IsAvaible { get; }

        public abstract bool IsRunning { get; }

        #endregion

        #region Public Methods

        public abstract void Connect(int localPort);

        public abstract void Close();

        #endregion
    }
}