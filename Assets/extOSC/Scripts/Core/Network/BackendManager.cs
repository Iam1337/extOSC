using System.Collections.Generic;

namespace extOSC.Core.Network
{
    internal static class BackendManager
    {
        #region Extensions

        private interface IBackendPool
        {
            IBackend Get(string localHost, int localPort);
            void Release(IBackend backend);
        }

        private class BackendPool<TBackend> : IBackendPool where TBackend : class, IBackend, new()
        {
            #region Extensions

            private class PoolItem
            {
                public string LocalHost;

                public int LocalPort;

                public int Links;

                public TBackend Backend;
            }

            #endregion

            #region Private Vars

            private readonly List<PoolItem> _items = new List<PoolItem>();

            #endregion

            #region Public Methods

            public IBackend Get(string localHost, int localPort)
            {
                var poolItem = _items.Find(p => p.LocalHost == localHost && p.LocalPort == localPort);
                if (poolItem == null)
                {
                    poolItem = new PoolItem();
                    poolItem.LocalHost = localHost;
                    poolItem.LocalPort = localPort;
                    poolItem.Backend = new TBackend();
                    poolItem.Backend.Connect(localHost, localPort);

                    _items.Add(poolItem);
                }

                poolItem.Links++;

                return poolItem.Backend;
            }

            public void Release(IBackend backend)
            {
                var poolItem = _items.Find(p => p.Backend == backend);
                if (poolItem == null)
                    return;

                poolItem.Links--;

                if (poolItem.Links <= 0)
                {
                    poolItem.Backend.Close();
                    _items.Remove(poolItem);
                }
            }

            #endregion
        }

        #endregion

        #region Private Vars

        private static readonly IBackendPool _pool;

        #endregion

        #region Public Methods

        static BackendManager()
        {
            _pool = new BackendPool<StandaloneBackend>();
        }

        public static IBackend Get(string localHost, int localPort) => _pool.Get(localHost, localPort);

        public static void Release(IBackend backend) => _pool.Release(backend);

        #endregion
    }
}