/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Misc
{
    [AddComponentMenu("extOSC/Components/Misc/Ping")]
    public class OSCTransmitterPing : OSCTransmitterComponent
    {
        #region Public Vars

        public bool AutoStart
        {
            get { return autoStart; }
            set { autoStart = value; }
        }

        public float Interval
        {
            get { return interval; }
            set
            {
                interval = value;

                if (interval < 0)
                    interval = 0;
            }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        #endregion

        #region Protected Vars

        [Range(0, 60)]
        [SerializeField]
        protected float interval = 1;

        [SerializeField]
        protected bool autoStart = true;

        #endregion

        #region Private Vars

        private float _timer;

        private bool _isRunning;

        #endregion

        #region Unity Methods

        protected virtual void Start()
        {
            if (autoStart) StartPing();
        }

        protected virtual void Update()
        {
            if (!_isRunning) return;

            _timer += Time.deltaTime;

            if (_timer >= interval)
            {
                _timer = 0;

                Send();
            }
        }

        #endregion

        #region Public Methods

        public void StartPing()
        {
            _isRunning = true;
        }

        public void StopPing()
        {
            _isRunning = false;
            _timer = 0;
        }

        public void PausePing()
        {
            _isRunning = false;
        }

        #endregion

        #region Protected Methods

        protected override bool FillMessage(OSCMessage message)
        {
            return true;
        }

        #endregion
    }
}