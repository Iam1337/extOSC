﻿/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEditor;
using UnityEngine;

using System;

using extOSC.Editor.Panels;

namespace extOSC.Editor.Windows
{
	public abstract class OSCWindow : EditorWindow
    {
        #region Public Vars

        public abstract OSCPanel RootPanel { get; }

        #endregion

        #region Unity Methods

        protected virtual void Awake()
        { }

        protected virtual void OnEnable()
        {
            LoadWindowSettings();
        }

        protected virtual void OnDisable()
        {
            SaveWindowSettings();
        }

        protected virtual void OnDestroy()
        { }

        protected abstract void OnGUI();

        #endregion

        #region Protected Methods

        protected virtual void LoadWindowSettings()
        { }

        protected virtual void SaveWindowSettings()
        { }

		#endregion
    }

    public class OSCWindow<TWindow, TPanel> : OSCWindow where TWindow : OSCWindow where TPanel : OSCPanel
    {
        #region Public Vars

        public static TWindow Instance
        {
            get { return GetWindow<TWindow>(false, "", false); }
        }

        public override OSCPanel RootPanel
        {
            get { return rootPanel; }
        }

        #endregion

        #region Protected Vars

        protected TPanel rootPanel
        {
            get
            {
                if (_rootPanel == null)
                    _rootPanel = CreateRoot();

                return _rootPanel;
            }
        }

        #endregion

        #region Private Vars

        private TPanel _rootPanel;

        #endregion

        #region Unity Methods

        protected override void OnGUI()
        {
            DrawRootPanel(new Rect(0, 0, position.width, position.height));
        }

        #endregion

        #region Protected Methods

        protected virtual T CreatePanel<T>(string panelId) where T : OSCPanel
        {
            var panel = (T)Activator.CreateInstance(typeof(T), panelId, this);
            if (panel == null) return null;
            
            return panel;
        }

        protected TPanel CreateRoot()
        {
            if (_rootPanel != null)
            {
                Debug.LogErrorFormat("[{0}] Already has root panel!", GetType());
                return default(TPanel);
            }

            var panel = (TPanel)Activator.CreateInstance(typeof(TPanel), this, "root" + name);

            _rootPanel = panel;

            return panel;
        }

        protected void DrawRootPanel(Rect contentRect)
        {
            if (rootPanel == null) return;

            rootPanel.Rect = contentRect;
            rootPanel.Draw();
        }

        #endregion
    }
}