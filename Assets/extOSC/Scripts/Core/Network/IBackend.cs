using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace extOSC.Core.Network
{
    internal interface IBackend
    {
        #region Vars
        
        bool IsAvailable { get; }

        bool IsReceiving { get; }

        bool IsTransmitting { get; }

        #endregion

        #region Method

        void Connect(string localHost, int localPort);

        void Close();

        // Receiving
        void StartReceiving();

        bool IsReceived(out IOSCPacket packet);

        void StopReceiving();
        
        // Transmitting
        void StartTransmitting(string remoteHost, int remotePort);

        void Transmit(IOSCPacket packet);

        void StopTransmitting();

        #endregion
    }
}