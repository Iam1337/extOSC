/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System;

using extOSC.Mapping;
using UnityEngine.Serialization;

namespace extOSC.Core
{
    [ExecuteInEditMode]
    public abstract class OSCBase : MonoBehaviour
    {
        #region Public Vars

        public bool AutoConnect
        {
            get => _autoConnect;
			set => _autoConnect = value;
		}

        public OSCMapBundle MapBundle
        {
            get => _mapBundle;
			set => _mapBundle = value;
		}

        public bool WorkInEditor
        {
            get => _workInEditor;
			set => _workInEditor = value;
		}

        public bool CloseOnPause
        {
            get => _closeOnPause;
			set => _closeOnPause = value;
		}

        public abstract bool IsStarted { get; }

        #endregion

        #region Private Vars

        [SerializeField]
		[FormerlySerializedAs("autoConnect")]
        private bool _autoConnect = true;

        [SerializeField]
		[FormerlySerializedAs("closeOnPause")]
		private bool _closeOnPause = false;

        [SerializeField]
		[FormerlySerializedAs("mapBundle")]
		private OSCMapBundle _mapBundle;

        [SerializeField]
		[FormerlySerializedAs("workInEditor")]
        private bool _workInEditor;

		private bool _restoreOnEnable;

		private bool _wasClosed;

        #endregion

		#region Unity Methods

        protected virtual void Start()
        {
            if (!Application.isPlaying) return;

            if (AutoConnect) Connect();
        }

        protected virtual void OnEnable()
        {
            if (Application.isPlaying && _restoreOnEnable)
                Connect();
        }

        protected virtual void OnDisable()
        {
            _restoreOnEnable = IsStarted;

            if (Application.isPlaying && _restoreOnEnable)
                Close();
        }

		protected void OnApplicationPause(bool pauseStatus)
        {
            if (!_closeOnPause) return;

            if (pauseStatus)
            {
                if (IsStarted)
                {
                    Close();

                    _wasClosed = true;
                }
            }
            else
            {
                if (_wasClosed)
                {
                    Connect();

                    _wasClosed = false;
                }
            }
        }

        #endregion

        #region Public Methods

        public abstract void Connect();

        public abstract void Close();

		#endregion
	}
}