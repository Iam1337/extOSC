/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;

using extOSC.Editor.Windows;

namespace extOSC.Editor.Panels
{
    public class OSCPanel
    {
        #region Extensions

        protected class PanelCoroutine
        {
            #region Static Public Methods

            public static PanelCoroutine Create(IEnumerator routine)
            {
                var coroutine = new PanelCoroutine(routine);
                coroutine.Start();

                return coroutine;
            }

            #endregion

            #region Private Vars

            private readonly IEnumerator _routine;

            #endregion

            #region Public Method

            public PanelCoroutine(IEnumerator routine)
            {
                _routine = routine;
            }

            public void Start()
            {
                EditorApplication.update += Process;
            }

            public void Stop()
            {
                EditorApplication.update -= Process;
            }

            #endregion

            #region Private Methods

            private void Process()
            {
                var process = _routine.MoveNext();
                if (!process) Stop();

            }

            #endregion
        }

        #endregion

        #region Public Vars

        public Rect Rect
        {
            get { return _rect; }
            set { _rect = value; }
        }

        public OSCWindow ParentWindow
        {
            get { return _parentWindow; }
        }

        public string PanelId
        {
            get { return _panelId; }
            set { _panelId = value; }
        }

        [Obsolete("\"Parent\" is deprecated, please use \"ParentWindow\" instead.")]
        public OSCWindow Parent
        {
            get { return ParentWindow; }
            set { Debug.LogWarning("\"Parent\" is deprecated."); }
        }

        [Obsolete("\"UniqueName\" is deprecated, please use \"PanelId\" instead.")]
        public string UniqueName
        {
            get { return PanelId; }
            set { Debug.LogWarning("\"UniqueName\" is deprecated."); }
        }

        [Obsolete("\"ContentRect\" is deprecated, please use \"Rect\" instead.")]
        public Rect ContentRect
        {
            get { return Rect; }
        }

        #endregion

        #region Protected Vars

        protected OSCWindow _parentWindow;

        protected string _panelId;

        #endregion

        #region Private Vars

        private Rect _rect;

        private List<PanelCoroutine> _coroutines = new List<PanelCoroutine>();

        #endregion

        #region Public Methods

        public OSCPanel(OSCWindow parentWindow, string panelId)
        {
            _panelId = panelId;
            _parentWindow = parentWindow;
        }

        [Obsolete("\"SetContentRect(Rect)\" is deprecated, please use \"Rect\" instead.")]
        public virtual void SetContentRect(Rect rect)
        {
            _rect = rect;
        }

        public virtual void Update()
        { }

        public virtual void Draw()
        {
            GUILayout.BeginArea(_rect);
            // PRE DRAW
            PreDrawContent();

            _rect.x = _rect.y = 0;

            DrawContent(_rect);

            // POST DRAW
            PostDrawContent();

            GUILayout.EndArea();
        }

        public virtual void StopCoroutines()
        {
            StopAllCoroutines();
        }

        #endregion

        #region Protected Methods

        protected virtual void PreDrawContent()
        { }

        protected virtual void DrawContent(Rect contentRect)
        { }

        protected virtual void PostDrawContent()
        { }

        protected PanelCoroutine StartCoroutine(IEnumerator routine)
        {
            var coroutine = PanelCoroutine.Create(routine);

            _coroutines.Add(coroutine);

            return coroutine;
        }

        protected void StopCoroutine(PanelCoroutine coroutine)
        {
            coroutine.Stop();

            if (_coroutines.Contains(coroutine))
                _coroutines.Remove(coroutine);
        }

        protected void StopAllCoroutines()
        {
            foreach (var coroutine in _coroutines)
            {
                coroutine.Stop();
            }

            _coroutines.Clear();
        }

        #endregion
    }
}