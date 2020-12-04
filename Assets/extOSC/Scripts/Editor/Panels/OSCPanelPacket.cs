/* Copyright (c) 2020 ExT (V.Sigalkin) */

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

		public OSCConsolePacket SelectedMessage;

		#endregion

		#region Private Vars

		private readonly OSCPacketDrawer _packetDrawer;

		private Vector2 _scrollPosition;

		#endregion

		#region Public Methods

		public OSCPanelPacket(OSCWindow window) : base(window)
		{
			_packetDrawer = new OSCPacketDrawer();
		}

		#endregion

		#region Protected Methods

		protected override void DrawContent(ref Rect contentRect)
		{
			if (SelectedMessage == null)
			{
				GUILayout.BeginHorizontal(EditorStyles.toolbar);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				EditorGUILayout.LabelField(_packetNotSelectedContent, OSCEditorStyles.CenterLabel, GUILayout.Height(contentRect.height));
			}
			else
			{
				GUILayout.BeginHorizontal(EditorStyles.toolbar);

				GUILayout.FlexibleSpace();

				var debugButton = GUILayout.Button(_openInDebugContent, EditorStyles.toolbarButton);
				if (debugButton)
				{
					OSCWindowDebug.OpenPacket(SelectedMessage.Packet);
				}

				GUILayout.EndHorizontal();

				_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
				_packetDrawer.DrawLayout(SelectedMessage.Packet);

				EditorGUILayout.EndScrollView();
			}
		}

		#endregion
	}
}