using UnityEngine;

namespace extOSC.Core.Network
{
    public class TransmitterBackend
    {
        #region Public Vars

        public bool IsAvailable => _backend != null;

        #endregion
        
        #region Private Vars

        private IBackend _backend;

        #endregion
        
        #region Public Methods

        public void StartTransmitter(string localHost, int localPort, string remoteHost, int remotePort)
        {
            var backend = BackendManager.Get(localHost, localPort);
            if (backend.IsTransmitting)
            {
                BackendManager.Release(backend);
                // TODO: Exception, or what?
                return;
            }

            _backend = backend;
            _backend.StartTransmitting(remoteHost, remotePort); // TODO: Add some check?
        }

        public void StopTransmitter()
        {
            if (_backend == null)
            {
                // TODO: Exception, or what?
                return;
            }

            _backend.StopTransmitting();
            BackendManager.Release(_backend);
            _backend = null;
        }

        public void Transmit(IOSCPacket packet) => _backend.Transmit(packet);

        #endregion
    }
}