/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System;

using extOSC.Mapping;

namespace extOSC.Core
{
    [ExecuteInEditMode]
    public abstract class OSCBase : MonoBehaviour, IDisposable
    {
        #region Public Vars

        public bool AutoConnect
        {
            get { return autoConnect; }
            set { autoConnect = value; }
        }

        public OSCMapBundle MapBundle
        {
            get { return mapBundle; }
            set { mapBundle = value; }
        }

        public bool WorkInEditor
        {
            get { return workInEditor; }
            set { workInEditor = value; }
        }

        public bool CloseOnPause
        {
            get { return closeOnPause; }
            set { closeOnPause = value; }
        }

        public abstract bool IsAvailable { get; }

        #endregion

        #region Protected Vars

        [SerializeField]
        protected bool autoConnect = true;

        [SerializeField]
        protected bool closeOnPause = false;

        [SerializeField]
        protected OSCMapBundle mapBundle;

        [SerializeField]
        protected bool workInEditor;

        protected bool restoreOnEnable;

        #endregion

        #region Private Vars

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
            if (Application.isPlaying && restoreOnEnable)
                Connect();
        }

        protected virtual void OnDisable()
        {
            restoreOnEnable = IsAvailable;

            if (Application.isPlaying && restoreOnEnable)
                Close();
        }

        protected virtual void OnDestroy()
        {
            Dispose();
        }

        protected void OnApplicationPause(bool pauseStatus)
        {
            if (!closeOnPause) return;

            if (pauseStatus)
            {
                if (IsAvailable)
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

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ///IDK, rly...
            }

            Close();
        }

        #endregion
    }
}