/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Ping
{
    [AddComponentMenu("extOSC/Components/Ping/Ping Client")]
    public class OSCPingClient : OSCComponent
    {
        #region Public Vars

        public float Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        public float Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        public bool AutoStart
        {
            get { return autoStart; }
            set { autoStart = value; }
        }

        public bool IsAvailable
        {
            get { return _IsAvailable; }
        }

        public float Timer
        {
            get { return _timer; }
        }

        public float LastReceiveTime
        {
            get { return _lastReceiveTime; }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        #endregion

        #region Protected Vars

        [SerializeField]
        protected float interval;

        [SerializeField]
        protected float timeout;

        [SerializeField]
        protected bool autoStart = true;

        #endregion

        #region Private Vars

        protected float _timer;

        protected float _lastReceiveTime;

        protected bool _IsAvailable;

        protected bool _isRunning;

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
                Send();

                _timer = 0;
            }

            _lastReceiveTime += Time.deltaTime;
            _IsAvailable = timeout > _lastReceiveTime;
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
            if (receiver == null) return false;

            message.AddValue(OSCValue.String(receiverAddress));
            message.AddValue(OSCValue.Int(receiver.LocalPort));

            return true;
        }

        protected override void Invoke(OSCMessage message)
        {
            if (message.HasImpulse())
                _lastReceiveTime = 0;
        }

        #endregion
    }
}