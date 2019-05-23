/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEditor;
using UnityEngine;

using extOSC.Core;
using extOSC.Editor.Drawers;
using extOSC.Editor.Windows;

namespace extOSC.Editor.Panels
{
    public class OSCPanelPacket : OSCPanel
    {
        #region Static Public Vars

        private static readonly GUIContent _packetNotSelectedContent = new GUIContent("Packet is not selected!");

        private static readonly GUIContent _openInDebugContent = new GUIContent("Open in debug");

        #endregion

        #region Public Vars

        public OSCConsolePacket SelecedMessage
        {
            get { return _selectedMessage; }
            set { _selectedMessage = value; }
        }

        #endregion

        #region Private Vars

        private OSCConsolePacket _selectedMessage;

        private Vector2 _scrollPosition;

        private OSCPacketDrawer _packetDrawer;

        #endregion

        #region Public Methods

        public OSCPanelPacket(OSCWindow window, string panelId) : base(window, panelId)
        {
            _packetDrawer = new OSCPacketDrawer();
        }

        #endregion

        #region Protected Methods

        protected override void DrawContent(ref Rect contentRect)
        {
            if (_selectedMessage == null)
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                EditorGUILayout.LabelField(_packetNotSelectedContent, OSCEditorStyles.CenterLabel, GUILayout.Height(contentRect.height));

                return;
            }

            if (_selectedMessage != null)
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);

                GUILayout.FlexibleSpace();

                var debugButton = GUILayout.Button(_openInDebugContent, EditorStyles.toolbarButton);
                if (debugButton)
                {
                    OSCWindowDebug.OpenPacket(SelecedMessage.Packet);
                }

                GUILayout.EndHorizontal();

                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                _packetDrawer.DrawLayout(_selectedMessage.Packet);

                EditorGUILayout.EndScrollView();
            }
        }

        #endregion
    }
}