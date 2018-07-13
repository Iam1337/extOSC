/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using extOSC.Core;
using extOSC.Core.Console;
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

        private static void DrawItem(Rect itemRect, int index, OSCConsolePacket consoleContainer, bool selected)
        {
            var dataRect = new Rect(0, 0, itemRect.width, itemRect.height);
            var backStyle = index % 2 != 0 ? OSCEditorStyles.ConsoleItemBackEven : OSCEditorStyles.ConsoleItemBackOdd;

            if (Event.current.type == EventType.Repaint)
            {
                backStyle.Draw(itemRect, false, false, selected, false);
            }

            GUI.BeginGroup(itemRect);

            DrawIcons(consoleContainer, ref dataRect);

            GUI.Label(dataRect, ItemToString(consoleContainer), OSCEditorStyles.ConsoleLabel);

            GUI.EndGroup();
        }

        private static Rect PaddingRect(Rect sourceRect, float padding)
        {
            return new Rect(sourceRect.x + padding, sourceRect.y + padding, sourceRect.height - padding * 2f, sourceRect.height - padding * 2f);
        }

        private static void DrawIcons(OSCConsolePacket consoleMessage, ref Rect dataRect)
        {
            Texture2D consoleTexture = null;

            if (consoleMessage.PacketType == OSCConsolePacketType.Received)
            {
                consoleTexture = OSCEditorTextures.Receiver;
            }
            else if (consoleMessage.PacketType == OSCConsolePacketType.Transmitted)
            {
                consoleTexture = OSCEditorTextures.Transmitter;
            }

            GUI.DrawTexture(PaddingRect(dataRect, 4), consoleTexture);

            dataRect.x += dataRect.height;
            dataRect.width -= dataRect.height;

            if (consoleMessage.Packet != null)
            {
                var packetTypeTextire = consoleMessage.Packet.IsBundle() ? OSCEditorTextures.Bundle : OSCEditorTextures.Message;

                GUI.DrawTexture(PaddingRect(dataRect, 4), packetTypeTextire);
            }

            dataRect.x += dataRect.height;
            dataRect.width -= dataRect.height;
        }

        private static string ItemToString(OSCConsolePacket consoleMessage)
        {
            if (consoleMessage == null)
                return string.Empty;

            var stringMessage = PacketToString(consoleMessage.Packet);
            stringMessage += "\n" + consoleMessage.Info;

            return stringMessage;
        }

        private static string PacketToString(OSCPacket packet)
        {
            if (!packet.IsBundle())
            {
                return string.Format("<color=orange>Message:</color> {0}", packet.Address);
            }

            var bundle = packet as OSCBundle;
            if (bundle != null)
            {
                return string.Format("<color=yellow>Bundle:</color> (Packets: {0})", bundle.Packets.Count);
            }

            return string.Empty;
        }

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

        public bool ShowTransmitted
        {
            get { return _showTransmitted; }
            set { _showTransmitted = value; }
        }

        public bool ShowReceived
        {
            get { return _showReceived; }
            set { _showReceived = value; }
        }


        [System.Obsolete("Collapse function was removed.")]
        public bool CollapseConsole
        {
            get; set;
        }

        public bool TrackLast
        {
            get { return _trackLast; }
            set { _trackLast = value; }
        }

        public OSCConsolePacket SelectedMessage
        {
            get { return _selectedMessage; }
            set { _selectedMessage = value; }
        }

        public int SelectedMessageId
        {
            get { return GetItemIndex(_selectedMessage); }
            set { if (_consoleBuffer != null && _consoleBuffer.Length > 0) _selectedMessage = _consoleBuffer[Mathf.Clamp(value, 0, _consoleBuffer.Length - 1)]; }
        }

        #endregion

        #region Private Vars

        private Rect _contentRect;

        private bool _showReceived;

        private bool _showTransmitted;

        protected bool _trackLast;

        private Rect _lastContentRect;

        private Vector2 _scrollPosition;

        private float _lineHeight = 30f;

        private OSCConsolePacket _selectedMessage;

        private OSCConsolePacket _rightClickMessage;

        private OSCConsolePacket[] _consoleBuffer;

        private GenericMenu _messageGenericMenu;

        #endregion

        #region Public Methods

        public OSCPanelConsole(OSCWindow parentWindow, string panelId) : base(parentWindow, panelId) 
        { }

        public void SelectFirstItem()
        {
            if (_consoleBuffer.Length <= 0) return;

            _selectedMessage = _consoleBuffer[0];
            ScrollToItem(_selectedMessage);
        }

        public void SelectNextItem()
        {
            if (_consoleBuffer.Length <= 0) return;

            var currentIndex = GetItemIndex(_selectedMessage);

            _selectedMessage = _consoleBuffer[Mathf.Clamp(currentIndex + 1, 0, _consoleBuffer.Length - 1)];
            ScrollToItem(_selectedMessage);
        }

        public void SelectPreviousItem()
        {
            if (_consoleBuffer.Length <= 0) return;

            var currentIndex = GetItemIndex(_selectedMessage);

            _selectedMessage = _consoleBuffer[Mathf.Clamp(currentIndex - 1, 0, _consoleBuffer.Length - 1)];
            ScrollToItem(_selectedMessage);
        }

        public void SelectLastItem()
        {
            if (_consoleBuffer.Length <= 0) return;

            _selectedMessage = _consoleBuffer[_consoleBuffer.Length - 1];
            ScrollToItem(_selectedMessage);
        }

        public void ScrollToItem(OSCConsolePacket consoleMessage)
        {
            if (consoleMessage == null) return;

            var messageItem = GetItemIndex(consoleMessage);

            if (messageItem < 0) return;

            var itemY = _lineHeight * messageItem;

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

        protected override void DrawContent(Rect contentRect)
        {
            GUILayout.BeginArea(new Rect(0, 0, contentRect.width, 18));

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            var clearButton = GUILayout.Button(_clearContent, EditorStyles.toolbarButton, GUILayout.Height(45f));

            GUILayout.Space(5f);

            _showReceived = GUILayout.Toggle(_showReceived, _recevedContent, EditorStyles.toolbarButton);
            _showTransmitted = GUILayout.Toggle(_showTransmitted, _transmittedContent, EditorStyles.toolbarButton);

            GUILayout.Space(5f);

            _trackLast = GUILayout.Toggle(_trackLast, _trackLastContent, EditorStyles.toolbarButton);

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            GUILayout.EndArea();

            contentRect.y += 18;
            contentRect.height -= 18;

            _lastContentRect = new Rect(contentRect);
            _consoleBuffer = OSCWindowConsole.GetConsoleBuffer(_showTransmitted, _showReceived);

            if (_trackLast)
            {
                if (_consoleBuffer.Length > 0)
                {
                    _selectedMessage = _consoleBuffer[0];
                }
                else
                {
                    _selectedMessage = null;
                }
            }

            var viewRect = new Rect(contentRect);
            viewRect.height = _consoleBuffer.Length * _lineHeight;

            if (viewRect.height > contentRect.height)
                viewRect.width -= 15f;

            var itemRect = new Rect(0, viewRect.y, viewRect.width, _lineHeight);
            _scrollPosition = GUI.BeginScrollView(contentRect, _scrollPosition, viewRect);

            var drawed = false;

            for (var index = 0; index < _consoleBuffer.Length; index++)
            {
                var drawItem = !((itemRect.y + itemRect.height < _scrollPosition.y) ||
                                 (itemRect.y > _scrollPosition.y + contentRect.height + itemRect.height));

                if (drawItem)
                {
                    drawed = true;

                    var consoleMessage = _consoleBuffer[index];
                    var selected = _selectedMessage == consoleMessage;
                    var color = GUI.color;

                    DrawItem(itemRect, index, consoleMessage, selected);

                    GUI.color = color;

                    if (Event.current.type == EventType.MouseDown && itemRect.Contains(Event.current.mousePosition))
                    {
                        if (Event.current.button == 0)
                        {
                            _trackLast = false;

                            if (_selectedMessage != consoleMessage)
                            {
                                _selectedMessage = consoleMessage;
                                _parentWindow.Repaint();
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

            GUI.EndScrollView(true);

            if (clearButton)
            {
                OSCWindowConsole.Clear();

                _selectedMessage = null;
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

            _parentWindow.Repaint();
        }

        #endregion

        #region Private Methods

        private int GetItemIndex(OSCConsolePacket consoleMessage)
        {
            for (var index = 0; index < _consoleBuffer.Length; index++)
            {
                if (_consoleBuffer[index] == consoleMessage) return index;
            }

            return -1;
        }

        #endregion
    }
}