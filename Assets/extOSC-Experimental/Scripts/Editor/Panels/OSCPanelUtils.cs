/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using extOSC.Editor.Windows;

using System;
using System.Collections;

namespace extOSC.Editor.Panels
{
    public class OSCPanelUtils : OSCPanel
    {
        #region Extensions

        [Serializable]
        private class Extension
        {
            #region Public Vars

            public string Name;

            public string TargetVersion;

            public string Description;

            public string Url;

            #endregion
        }

        [Serializable]
        private class ExtensionsWrapper
        {
            #region Public Vars

            public Extension[] Extensions;

            #endregion
        }

        #endregion

        #region Static Private Vars

        private static readonly GUIContent _refreshContent = new GUIContent("Refresh");

        private static readonly GUIContent _loadingContent = new GUIContent("Loading...");

        private static readonly GUIContent _openContent = new GUIContent("Open In Browser");

        #endregion

        #region Public Vars

        #endregion

        #region Protected Vars

        #endregion

        #region Private Vars

        private readonly string _url = "http://iron-wall.org/extOSC/utils.json";

        private Extension[] _extensions = new Extension[0];

        private float _lineHeight = 35f;

        private Vector2 _scrollPosition;

        private bool _isLoading;

        #endregion

        #region Unity Methods

        #endregion

        #region Public Methods

        public OSCPanelUtils(OSCWindow parentWindow, string panelId) : base(parentWindow, panelId)
        {
            // Shitty hack...
            var extension = new Extension() { Name = "None", Description = "Loading...", TargetVersion = string.Empty, Url = string.Empty };
            var wrapper = new ExtensionsWrapper() { Extensions = new[] { extension } };

            _extensions = wrapper.Extensions;
        }

        public void Refresh()
        {
            StartCoroutine(LoadJsonCoroutine(_url, JsonCallback));
        }

        protected override void DrawContent(Rect contentRect)
        {
            // TOOLBAR
            GUILayout.BeginArea(new Rect(0, 0, contentRect.width, 18));

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            var refreshButton = false;

            if (!_isLoading)
            {
                refreshButton = GUILayout.Button(_refreshContent, EditorStyles.toolbarButton, GUILayout.Width(55f));
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Button(_loadingContent, EditorStyles.toolbarButton, GUILayout.Width(55f));
                GUI.enabled = true;
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            GUILayout.EndArea();

            contentRect.y += 18;
            contentRect.height -= 18;

            var viewRect = new Rect(contentRect);
            viewRect.height = _extensions.Length * _lineHeight;

            if (viewRect.height > contentRect.height)
                viewRect.width -= 15f;

            var itemRect = new Rect(0, viewRect.y, viewRect.width, _lineHeight);
            _scrollPosition = GUI.BeginScrollView(contentRect, _scrollPosition, viewRect);

            for (var index = 0; index < _extensions.Length; index++)
            {
                DrawExtensions(itemRect, index, _extensions[index]);

                itemRect.y += itemRect.height;
            }

            GUILayout.Label(_extensions.Length.ToString());

            GUI.EndScrollView();

            // ACTIONS
            if (refreshButton)
            {
                Refresh();
            }
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Private Methods

        private IEnumerator LoadJsonCoroutine(string url, Action<string> callback)
        {
            _isLoading = true;

            var www = new WWW(url);

            while (!www.isDone)
                yield return null;

            if (callback != null)
                callback(www.text);

            _isLoading = false;
        }

        private void JsonCallback(string json)
        {
            var wrapper = JsonUtility.FromJson<ExtensionsWrapper>(json);
            if (wrapper == null) return;

            if (wrapper.Extensions == null ||
                wrapper.Extensions.Length == 0)
                return;

            _extensions = wrapper.Extensions;

            _parentWindow.Repaint();
        }

        private void DrawExtensions(Rect itemRect, int index, Extension extension)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var backStyle = index % 2 != 0 ? OSCEditorStyles.ConsoleItemBackEven : OSCEditorStyles.ConsoleItemBackOdd;
                backStyle.Draw(itemRect, false, false, false, false);
            }

            GUILayout.BeginArea(itemRect);
            GUILayout.BeginHorizontal(GUILayout.Height(_lineHeight));

            GUI.DrawTexture(new Rect(5, 0, _lineHeight, _lineHeight), OSCEditorTextures.Bundle);
            GUILayout.Space(_lineHeight + 10);

            var labelWidth = itemRect.width - (_lineHeight + 130f);

            GUILayout.Label("<b>" + extension.Name + " (" + extension.TargetVersion + ")</b>\n" + extension.Description, OSCEditorStyles.ConsoleLabel, GUILayout.Width(labelWidth));

            GUILayout.BeginVertical(GUILayout.Width(110f));
            GUILayout.Space(5f);
            if (GUILayout.Button(_openContent, GUILayout.Height(_lineHeight - 10)))
            {
                Application.OpenURL(extension.Url);
            }
            GUILayout.EndVertical();

            GUILayout.Space(5);

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        #endregion
    }
}