/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

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

		private static readonly GUIContent _infoContent = new GUIContent("Create or load debug Packet!");

		private static readonly GUIContent[] _createPopupItems =
		{
			new GUIContent("Message"),
			new GUIContent("Bundle")
		};

		#endregion

		#region Public Vars

		public IOSCPacket CurrentPacket;

		public string FilePath;

		public string PacketName => string.IsNullOrEmpty(FilePath) ? "unnamed" : Path.GetFileNameWithoutExtension(FilePath);

		#endregion

		#region Private Vars

		private Vector2 _scrollPosition;

		private readonly OSCPacketEditableDrawer _packetDrawer;

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
						var customMenuRect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0);
						EditorUtility.DisplayCustomMenu(customMenuRect, _createPopupItems, -1, CreatePacket, null);
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

					if (contentRect.width > 470)
					{
						if (CurrentPacket != null)
						{
							GUILayout.Space(5);

							if (GUILayout.Button(_generateCodeContent, EditorStyles.toolbarButton))
							{
								GenerateSharpCode();
							}
						}
					}

					GUILayout.FlexibleSpace();

					if (CurrentPacket != null)
						GUILayout.Label($"Name: {PacketName}");
				}

				if (CurrentPacket != null)
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

		public OSCPanelPacketEditor(OSCWindow window) : base(window)
		{
			_packetDrawer = new OSCPacketEditableDrawer();
		}

		#endregion

		#region Private Methods

		private void CreatePacket(object userData, string[] options, int selected)
		{
			CurrentPacket = selected == 0 ? (IOSCPacket) new OSCMessage("/address") : new OSCBundle();
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