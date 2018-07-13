/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using System.IO;

using extOSC.Core;
using extOSC.Editor.Windows;

namespace extOSC.Editor.Panels
{
    public class OSCPanelPacketEditor : OSCPanel
    {
        #region Static Private Vars

        private static readonly GUIContent _createContent = new GUIContent("Create");

        private static readonly GUIContent _openContent = new GUIContent("Open Packet");

        private static readonly GUIContent _saveContent = new GUIContent("Save Packet");

        private static readonly GUIContent _generateCodeContent = new GUIContent("Generate Sharp Code");

        private static readonly GUIContent _infoContent = new GUIContent("Create or load debug packet!");

        #endregion

        #region Public Vars

        public OSCPacket CurrentPacket
        {
            get { return _currentPacket; }
            set { _currentPacket = value; }
        }

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        public string PacketName
        {
            get
            {
                if (string.IsNullOrEmpty(_filePath))
                    return "unnamed";

                return Path.GetFileNameWithoutExtension(_filePath);
            }
        }

        #endregion

        #region Private Vars

        private GUIContent[] _createPoputItems = new GUIContent[] {
            new GUIContent("Message"),
            new GUIContent("Bundle")
        };

        private Vector2 _scrollPosition;

        private OSCPacket _currentPacket;

        private string _filePath;

        #endregion

        #region Unity Methods

        protected override void DrawContent(Rect contentRect)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button(_createContent, EditorStyles.toolbarDropDown))
            {
                var customMenuRect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0);

                EditorUtility.DisplayCustomMenu(customMenuRect, _createPoputItems, -1, CreatePacket, null);
            }

            GUILayout.Space(5);

            var openButton = GUILayout.Button(_openContent, EditorStyles.toolbarButton);
            var saveButton = GUILayout.Button(_saveContent, EditorStyles.toolbarButton);
            var generateButton = false;

            if (_currentPacket != null)
            {
                GUILayout.Space(5);

                generateButton = GUILayout.Button(_generateCodeContent, EditorStyles.toolbarButton);
            }

            GUILayout.FlexibleSpace();

            if (_currentPacket != null)
                GUILayout.Label(string.Format("Name: {0}", PacketName));

            EditorGUILayout.EndHorizontal();

            if (_currentPacket != null && _currentPacket != null)
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                OSCEditorLayout.EditablePacket(_currentPacket);

                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.LabelField(_infoContent, OSCEditorStyles.CenterLabel, GUILayout.Height(contentRect.height));
            }

            EditorGUILayout.EndVertical();

            if (openButton) OpenCurrentDebugPacket();
            if (saveButton) SaveCurrentDebugPacket();
            if (generateButton) GenerateCurrendDebugCode();
        }

        #endregion

        #region Public Methods

        public OSCPanelPacketEditor(OSCWindow parentWindow, string panelId) : base(parentWindow, panelId)
        { }

        #endregion

        #region Private Methods

        private void SaveCurrentDebugPacket()
        {
            if (_currentPacket == null) return;

            var filePath = EditorUtility.SaveFilePanel("Save Packet", OSCEditorUtils.DebugFolder, "New Debug Packet", "eod");
            if (!string.IsNullOrEmpty(filePath))
            {
                FilePath = filePath;

                OSCEditorUtils.SavePacket(filePath, _currentPacket);
            }
        }

        private void OpenCurrentDebugPacket()
        {
            var filePath = EditorUtility.OpenFilePanel("Open Packet", OSCEditorUtils.DebugFolder, "eod");
            if (!string.IsNullOrEmpty(filePath))
            {
                _currentPacket = OSCEditorUtils.LoadPacket(filePath);
                _filePath = filePath;
            }
        }

        private void GenerateCurrendDebugCode()
        {
            if (_currentPacket == null)
                return;

            EditorGUIUtility.systemCopyBuffer = OSCSharpCode.GeneratePacket(_currentPacket);

            Debug.LogFormat("[OSCDebug] CSharp code generated and stored in copy buffer!");
        }

        private void CreatePacket(object userData, string[] options, int selected)
        {
            if (selected == 0)
                _currentPacket = new OSCMessage("/address");
            else
                _currentPacket = new OSCBundle();

            _filePath = string.Empty;
        }

        #endregion
    }
}