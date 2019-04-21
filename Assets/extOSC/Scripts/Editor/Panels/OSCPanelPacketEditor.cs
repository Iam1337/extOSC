﻿/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using System.IO;

using extOSC.Core;
using extOSC.Editor.Drawers;
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

        private static readonly GUIContent[] _createPoputItems = new GUIContent[] {
            new GUIContent("Message"),
            new GUIContent("Bundle")
        };

        #endregion

        #region Public Vars

        public OSCPacket CurrentPacket { get; set; }

	    public string FilePath { get; set; }

	    public string PacketName
        {
            get
            {
                if (string.IsNullOrEmpty(FilePath))
                    return "unnamed";

                return Path.GetFileNameWithoutExtension(FilePath);
            }
        }

        #endregion

        #region Private Vars

        private Vector2 _scrollPosition;

        private OSCPacketEditableDrawer _packetDrawer;

	    #endregion

        #region Unity Methods

        protected override void DrawContent(ref Rect contentRect)
        {
	        using (new GUILayout.VerticalScope())
	        {
				// TOOLBAR
				using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
		        {
			        if (GUILayout.Button(_createContent, EditorStyles.toolbarDropDown))
			        {
				        var customMenuRect =
					        new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0);
				        EditorUtility.DisplayCustomMenu(customMenuRect, _createPoputItems, -1, CreatePacket, null);
			        }

			        GUILayout.Space(5);

			        if (GUILayout.Button(_openContent, EditorStyles.toolbarButton))
			        {
				        OpenPacket();
			        }

			        if (GUILayout.Button(_saveContent, EditorStyles.toolbarButton))
			        {
				        SavePacket();
			        }

			        if (CurrentPacket != null)
			        {
				        GUILayout.Space(5);

				        if (GUILayout.Button(_generateCodeContent, EditorStyles.toolbarButton))
				        {
					        GenerateSharpCode();
				        }
			        }

			        GUILayout.FlexibleSpace();

			        if (CurrentPacket != null)
				        GUILayout.Label(string.Format("Name: {0}", PacketName));
		        }

		        if (CurrentPacket != null && CurrentPacket != null)
		        {
			        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
			        _packetDrawer.DrawLayout(CurrentPacket);

			        EditorGUILayout.EndScrollView();
		        }
		        else
		        {
			        EditorGUILayout.LabelField(_infoContent, OSCEditorStyles.CenterLabel, GUILayout.Height(contentRect.height));
		        }
	        }
        }

        #endregion

        #region Public Methods

        public OSCPanelPacketEditor(OSCWindow window, string panelId) : base(window, panelId)
        {
            _packetDrawer = new OSCPacketEditableDrawer();
        }

        #endregion

        #region Private Methods

	    private void CreatePacket(object userData, string[] options, int selected)
	    {
		    if (selected == 0)
		    {
			    CurrentPacket = new OSCMessage("/address");
		    }
		    else
		    {
			    CurrentPacket = new OSCBundle();
		    }

		    FilePath = string.Empty;
	    }

        private void SavePacket()
        {
            if (CurrentPacket == null) return;

            var file = EditorUtility.SaveFilePanel("Save Packet", OSCEditorUtils.DebugFolder, "New Debug Packet", "eod");
            if (!string.IsNullOrEmpty(file))
            {
                FilePath = file;
				OSCEditorUtils.SavePacket(file, CurrentPacket);
            }
        }

        private void OpenPacket()
        {
            var file = EditorUtility.OpenFilePanel("Open Packet", OSCEditorUtils.DebugFolder, "eod");
            if (!string.IsNullOrEmpty(file))
            {
	            FilePath = file;
                CurrentPacket = OSCEditorUtils.LoadPacket(file);
            }
        }

        private void GenerateSharpCode()
        {
            if (CurrentPacket == null)
                return;

            EditorGUIUtility.systemCopyBuffer = OSCSharpCode.GeneratePacket(CurrentPacket);
			Debug.LogFormat("[extOSC] CSharp code generated and stored in copy buffer!");
        }


        #endregion
    }
}