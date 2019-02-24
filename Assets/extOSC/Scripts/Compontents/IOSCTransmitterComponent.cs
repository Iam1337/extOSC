/* Copyright (c) 2019 ExT (V.Sigalkin) */

namespace extOSC.Components
{
    public interface IOSCTransmitterComponent
    {
        #region Public Vars

        OSCTransmitter Transmitter { get; }

        string TransmitterAddress { get; }

        #endregion

        #region Public Methods

        void Send();

        #endregion
    }
}