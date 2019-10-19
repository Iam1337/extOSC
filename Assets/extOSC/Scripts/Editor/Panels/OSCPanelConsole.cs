/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEditor;
using UnityEngine;

using extOSC.Core;
using extOSC.Editor.Drawers;
using extOSC.Editor.Windows;

namespace extOSC.Editor.Panels
{
	public class OSCPanelConsole : OSCPanel
	{
		#region Static Private Vars

		private static readonly GUIContent _clearContent = new GUIContent("Clear");

		private static readonly GUIContent _transmittedContent = new GUIContent("Transmitted");

		private static readonly GUIContent _recevedContent = new GUIContent("Received");

		private static readonly GUIContent _trackLastContent = new GUIContent("Track Last");

		private static readonly GUIContent _openInDebugContent = new GUIContent("Open in debug");

		private static readonly GUIContent _generateCodeContent = new GUIContent("Generate Sharp Code");

		#endregion

		#region Static Private Methods

		private static void ShowMessageGenericMenu(OSCConsolePacket consoleMessage)
		{
			var genericMenu = new GenericMenu();
			genericMenu.AddItem(_openInDebugContent, false, OpenInDebugCallback, consoleMessage);
			genericMenu.AddItem(_generateCodeContent, false, GenerateCodeCallback, consoleMessage);
			genericMenu.ShowAsContext();
		}

		private static void OpenInDebugCallback(object menuData)
		{
			var consoleMessage = menuData as OSCConsolePacket;
			if (consoleMessage == null) return;

			OSCWindowDebug.OpenPacket(consoleMessage.Packet);
		}

		private static void GenerateCodeCallback(object menuData)
		{
			var consoleMessage = menuData as OSCConsolePacket;
			if (consoleMessage == null) return;

			EditorGUIUtility.systemCopyBuffer = OSCSharpCode.GeneratePacket(consoleMessage.Packet);

			Debug.LogFormat("[OSCConsole] CSharp code generated and stored in copy buffer!");
		}

		#endregion

		#region Public Vars

		public bool ShowTransmitted { get; set; }

		public bool ShowReceived { get; set; }

		public bool TrackLast { get; set; }

		public OSCConsolePacket SelectedMessage { get; set; }

		public string Filter
		{
			get { return _filterDrawer.FilterValue; }
			set { _filterDrawer.FilterValue = value; }
		}

		#endregion

		#region Private Vars

		private Rect _contentRect;

		// TOOLBAR

		// UI
		private Rect _lastContentRect;

		private Vector2 _scrollPosition;

		private Rect _filterButtonPosition;

		private float _lineHeight = 30f;

		private OSCConsolePacket _rightClickMessage;

		private OSCConsolePacket[] _consoleBuffer;

		private GenericMenu _messageGenericMenu;

		private OSCFilterDrawer _filterDrawer;

		#endregion

		#region Public Methods

		public OSCPanelConsole(OSCWindow window, string panelId) : base(window, panelId)
		{
			_filterDrawer = new OSCFilterDrawer();
		}

		public void SelectFirstItem()
		{
			if (_consoleBuffer.Length <= 0) return;

			SelectedMessage = _consoleBuffer[0];
			ScrollToItem(SelectedMessage);
		}

		public void SelectNextItem()
		{
			if (_consoleBuffer.Length <= 0) return;

			var currentIndex = _consoleBuffer.IndexOf(SelectedMessage);

			SelectedMessage = _consoleBuffer[Mathf.Clamp(currentIndex + 1, 0, _consoleBuffer.Length - 1)];
			ScrollToItem(SelectedMessage);
		}

		public void SelectPreviousItem()
		{
			if (_consoleBuffer.Length <= 0) return;

			var currentIndex = _consoleBuffer.IndexOf(SelectedMessage);

			SelectedMessage = _consoleBuffer[Mathf.Clamp(currentIndex - 1, 0, _consoleBuffer.Length - 1)];
			ScrollToItem(SelectedMessage);
		}

		public void SelectLastItem()
		{
			if (_consoleBuffer.Length <= 0) return;

			SelectedMessage = _consoleBuffer[_consoleBuffer.Length - 1];
			ScrollToItem(SelectedMessage);
		}

		public void ScrollToItem(OSCConsolePacket consolePacket)
		{
			if (consolePacket == null)
				return;

			var packetIndex = _consoleBuffer.IndexOf(SelectedMessage);
			if (packetIndex < 0)
				return;

			var itemY = _lineHeight * packetIndex;
			if (itemY < _scrollPosition.y)
			{
				_scrollPosition.y = itemY;
			}
			else if (itemY > _scrollPosition.y + _lastContentRect.height - _lineHeight)
			{
				_scrollPosition.y = itemY - _lastContentRect.height + _lineHeight;
			}
		}

		#endregion

		#region Protected Methods

		protected override void DrawContent(ref Rect contentRect)
		{
			// TOOLBAR
			DrawToolbar(ref contentRect);

			_lastContentRect = new Rect(contentRect);
			_consoleBuffer = OSCWindowConsole.GetConsoleBuffer(ShowTransmitted, ShowReceived, _filterDrawer.FilterValue);

			if (TrackLast)
			{
				SelectedMessage = _consoleBuffer.Length > 0 ? _consoleBuffer[0] : null;
			}

			var viewRect = new Rect(contentRect);
			viewRect.height = _consoleBuffer.Length * _lineHeight;

			if (viewRect.height > contentRect.height)
				viewRect.width -= 15f;

			var itemRect = new Rect(0, viewRect.y, viewRect.width, _lineHeight);

			using (var scroll = new GUI.ScrollViewScope(contentRect, _scrollPosition, viewRect))
			{
				var drawed = false;

				for (var index = 0; index < _consoleBuffer.Length; index++)
				{
					if (itemRect.y + itemRect.height > _scrollPosition.y &&
						itemRect.y < _scrollPosition.y + contentRect.height + itemRect.height)
					{
						drawed = true;

						var consoleMessage = _consoleBuffer[index];
						var selected = SelectedMessage == consoleMessage;
						var color = GUI.color;

						DrawItem(ref itemRect, index, consoleMessage, selected);

						GUI.color = color;

						if (Event.current.type == EventType.MouseDown && itemRect.Contains(Event.current.mousePosition))
						{
							GUIUtility.keyboardControl = 0;

							if (Event.current.button == 0)
							{
								TrackLast = false;

								if (SelectedMessage != consoleMessage)
								{
									SelectedMessage = consoleMessage;
									Window.Repaint();
								}

								Event.current.Use();
							}
							else if (Event.current.button == 1)
							{
								ShowMessageGenericMenu(consoleMessage);

								Event.current.Use();
							}
						}
					}
					else if (drawed)
					{
						break;
					}

					itemRect.y += itemRect.height;
				}

				_scrollPosition = scroll.scrollPosition;
			}
		}

		protected override void PostDrawContent()
		{
			var current = Event.current;

			if (!current.isKey || current.type != EventType.KeyDown) return;

			if (current.keyCode == KeyCode.DownArrow)
			{
				SelectNextItem();
			}
			else if (current.keyCode == KeyCode.UpArrow)
			{
				SelectPreviousItem();
			}
			else if (current.keyCode == KeyCode.End)
			{
				SelectLastItem();
			}
			else if (current.keyCode == KeyCode.Home)
			{
				SelectFirstItem();
			}

			current.Use();

			Window.Repaint();
		}

		#endregion

		#region Private Methods

		private void DrawToolbar(ref Rect contentRect)
		{
#if UNITY_2019_3_OR_NEWER
            var toolbarSize = 22;
#else
			var toolbarSize = 18;
#endif

			contentRect.y += toolbarSize;
			contentRect.height -= toolbarSize;

			GUILayout.BeginArea(new Rect(0, 0, contentRect.width, toolbarSize));

			EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

			var clearButton = GUILayout.Button(_clearContent, EditorStyles.toolbarButton, GUILayout.Height(45f));

			GUILayout.Space(5f);

			ShowReceived = GUILayout.Toggle(ShowReceived, _recevedContent, EditorStyles.toolbarButton);
			ShowTransmitted = GUILayout.Toggle(ShowTransmitted, _transmittedContent, EditorStyles.toolbarButton);

			GUILayout.FlexibleSpace();
			GUILayout.Space(5f);

			_filterDrawer.Draw();

			GUILayout.Space(5f);

			TrackLast = GUILayout.Toggle(TrackLast, _trackLastContent, EditorStyles.toolbarButton);

			GUILayout.Space(5f);

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();

			GUILayout.EndArea();

			if (clearButton)
			{
				OSCWindowConsole.Clear();

				SelectedMessage = null;
			}

		}

		private void DrawItem(ref Rect itemRect, int index, OSCConsolePacket consolePacket, bool selected)
		{
			using (new GUI.GroupScope(itemRect))
			{
				var localRect = itemRect;
				localRect.x = localRect.y = 0;

				if (Event.current.type == EventType.Repaint)
				{
					var backStyle = index % 2 != 0 ? OSCEditorStyles.ConsoleItemBackEven : OSCEditorStyles.ConsoleItemBackOdd;
					backStyle.Draw(localRect, false, false, selected, false);
				}

				DrawIcons(ref localRect, consolePacket);

				GUI.Label(localRect, consolePacket.ToString(), OSCEditorStyles.ConsoleLabel);
			}
		}

		private void DrawIcons(ref Rect iconsRect, OSCConsolePacket consolePacket)
		{
			var padding = 4;
			var consoleTexture = (Texture2D) null;

			if (consolePacket.PacketType == OSCConsolePacketType.Received)
			{
				consoleTexture = OSCEditorTextures.Receiver;
			}
			else if (consolePacket.PacketType == OSCConsolePacketType.Transmitted)
			{
				consoleTexture = OSCEditorTextures.Transmitter;
			}

			GUI.DrawTexture(new Rect(iconsRect.x + padding,
									 iconsRect.y + padding,
									 iconsRect.height - padding * 2f,
									 iconsRect.height - padding * 2f),
							consoleTexture);

			iconsRect.x += iconsRect.height;
			iconsRect.width -= iconsRect.height;

			GUI.DrawTexture(new Rect(iconsRect.x + padding,
									 iconsRect.y + padding,
									 iconsRect.height - padding * 2f,
									 iconsRect.height - padding * 2f),
							consolePacket.Packet.IsBundle()
								? OSCEditorTextures.Bundle
								: OSCEditorTextures.Message);

			iconsRect.x += iconsRect.height;
			iconsRect.width -= iconsRect.height;
		}

		#endregion
	}
}