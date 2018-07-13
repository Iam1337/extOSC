/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

using extOSC.Core.Console;
using extOSC.Editor.Panels;

namespace extOSC.Editor.Windows
{
    public class OSCWindowConsole : OSCWindow<OSCWindowConsole, OSCSplitPanel>
    {
        #region Static Public Vars

        public static List<OSCConsolePacket> ConsoleBuffer;

        #endregion

        #region Static Private Vars

        private static OSCConsolePacket[] _tempBuffer = new OSCConsolePacket[0];

        private static OSCConsolePacket _lastMessage;

        private static bool _previousTransmitted;

        private static bool _previousReceived;

        private static int _maxBufferCapacity = 256;

        private static OSCConsolePacket[] _emptyBuffer = new OSCConsolePacket[0];

        #endregion

        #region Public Static Methods

        [MenuItem("Window/extOSC/Console Window", false, 0)]
        public static void ShowWindow()
        {
            Instance.titleContent = new GUIContent("OSC Console", OSCEditorTextures.IronWall);
            Instance.minSize = new Vector2(610, 200);
            Instance.Show();
        }

        public static OSCConsolePacket[] GetConsoleBuffer(bool transmitted, bool received)
        {
            if (ConsoleBuffer == null || (ConsoleBuffer != null && ConsoleBuffer.Count == 0))
                return _emptyBuffer;

            var requireRebuild = false;

            if (_previousTransmitted != transmitted ||
                _previousReceived != received)
            {
                _previousTransmitted = transmitted;
                _previousReceived = received;

                requireRebuild = true;
            }
            else if (ConsoleBuffer.Count > 0)
            {
                requireRebuild = (ConsoleBuffer[0] != _lastMessage);
            }

            if (!requireRebuild)
                return _tempBuffer;

            _lastMessage = ConsoleBuffer.Count > 0 ? ConsoleBuffer[0] : null;

            var consoleList = new List<OSCConsolePacket>();

            foreach (var consoleMessage in ConsoleBuffer)
            {
                if ((transmitted && consoleMessage.PacketType == OSCConsolePacketType.Transmitted) ||
                    (received && consoleMessage.PacketType == OSCConsolePacketType.Received))
                {
                    consoleList.Add(consoleMessage);
                }
            }

            _tempBuffer = consoleList.ToArray();

            return _tempBuffer;
        }

        public static void Clear()
        {
            if (ConsoleBuffer != null)
            {
                ConsoleBuffer.Clear();

                OSCEditorUtils.SaveConsoleMessages(OSCEditorUtils.LogsFilePath, ConsoleBuffer);
            }
        }

        #endregion

        #region Protected Vars

        private OSCPanelConsole logPanel;

        private OSCPanelPacket packetPanel;

        #endregion

        #region Private Vars

        private readonly string _showReceivedSettings = OSCEditorSettings.Console + "showreceived";

        private readonly string _showTransmittedSettings = OSCEditorSettings.Console + "showtransmitted";

        private readonly string _trackLastSettings = OSCEditorSettings.Console + "tracklast";

        #endregion

        #region Unity Methods

        protected override void OnEnable()
        {
            logPanel = new OSCPanelConsole(this, "oscLogPanel1");
            packetPanel = new OSCPanelPacket(this, "oscPacketPanel1");

            rootPanel.AddPanel(logPanel, 310, 0.6f);
            rootPanel.AddPanel(packetPanel, 300, 0.4f);

            base.OnEnable();
        }

        protected override void Update()
        {
            if (ConsoleBuffer == null)
            {
                ConsoleBuffer = OSCEditorUtils.LoadConsoleMessages(OSCEditorUtils.LogsFilePath);

                Repaint();
            }

            if (OSCConsole.ConsoleBuffer.Count > 0)
            {
                foreach (var message in OSCConsole.ConsoleBuffer)
                {
                    if (ConsoleBuffer.Count >= _maxBufferCapacity)
                        ConsoleBuffer.RemoveAt(ConsoleBuffer.Count - 1);

                    ConsoleBuffer.Insert(0, message);
                }

                OSCEditorUtils.SaveConsoleMessages(OSCEditorUtils.LogsFilePath, ConsoleBuffer);
                OSCConsole.ConsoleBuffer.Clear();

                Repaint();
            }

            if (packetPanel.SelecedMessage != logPanel.SelectedMessage)
            {
                packetPanel.SelecedMessage = logPanel.SelectedMessage;

                Repaint();
            }
        }

        #endregion

        #region Protected Methods

        protected override void LoadWindowSettings()
        {
            base.LoadWindowSettings();

            if (logPanel == null) return;

            logPanel.ShowReceived = OSCEditorSettings.GetBool(_showReceivedSettings, true);
            logPanel.ShowTransmitted = OSCEditorSettings.GetBool(_showTransmittedSettings, true);
            logPanel.TrackLast = OSCEditorSettings.GetBool(_trackLastSettings, false);
        }

        protected override void SaveWindowSettings()
        {
            base.SaveWindowSettings();

            if (logPanel == null) return;

            OSCEditorSettings.SetBool(_showReceivedSettings, logPanel.ShowReceived);
            OSCEditorSettings.SetBool(_showTransmittedSettings, logPanel.ShowTransmitted);
            OSCEditorSettings.SetBool(_trackLastSettings, logPanel.TrackLast);
        }

        #endregion
    }
}