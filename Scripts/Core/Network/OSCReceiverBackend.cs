/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

namespace extOSC.Core.Network
{
    public abstract class OSCReceiverBackend
    {
        #region Extensions

        public delegate void OSCReceivedCallback(IOSCPacket packet);

        #endregion

        #region Public Methods

        public static OSCReceiverBackend Create()
        {
#if UNITY_WSA && !UNITY_EDITOR
            return new OSCReceiverWindowsStoreBackend();
#else
            return new OSCReceiverStandaloneBackend();
#endif
        }

        #endregion

        #region Public Vars

        public abstract OSCReceivedCallback ReceivedCallback { get; set; }

        public abstract bool IsAvailable { get; }

        public abstract bool IsRunning { get; }

        #endregion

        #region Public Methods

        public abstract void Connect(string localHost, int localPort);

        public abstract void Close();

        #endregion
    }
}