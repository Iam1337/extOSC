using UnityEngine;

namespace extOSC.Core.Network
{
    public class ReceiverBackend
    {
        #region Public Vars

        public bool IsAvailable => _backend != null;
        
        #endregion

        #region Private Vars

        private IBackend _backend;

        #endregion
        
        #region Public Methods

        public void StartReceive(string localHost, int localPort)
        {
            var backend = BackendManager.Get(localHost, localPort);
            if (backend.IsReceiving)
            {
                BackendManager.Release(backend);
                // TODO: Exception, or what?
                return;
            }

            _backend = backend;
            _backend.StartReceiving();
        }

        public void StopReceive()
        {
            if (_backend == null)
            {
                // TODO: Exception, or what?
                return;
            }

            _backend.StopReceiving();
            BackendManager.Release(_backend);
            _backend = null;
        }

        public bool IsReceived(out IOSCPacket packet) => _backend.IsReceived(out packet);

        #endregion
    }
}