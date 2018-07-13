/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using extOSC.Core;
using extOSC.Editor.Panels;

namespace extOSC.Editor.Windows
{
    public class OSCWindowDebug : OSCWindow<OSCWindowDebug, OSCSplitPanel>
    {
        #region Static Public Vars

        public static OSCPacket CurrentPacket
        {
            get
            {
                if (Instance != null && Instance.packetEditorPanel != null && Instance.packetEditorPanel.CurrentPacket != null)
                    return Instance.packetEditorPanel.CurrentPacket;

                return null;
            }
        }

        #endregion

        #region Static Public Methods

        [MenuItem("Window/extOSC/Debug Window", false, 2)]
        public static void ShowWindow()
        {
            Instance.titleContent = new GUIContent("OSC Debug", OSCEditorTextures.IronWall);
            Instance.minSize = new Vector2(550, 200);
            Instance.Show();
        }

        public static void OpenPacket(OSCPacket packet)
        {
            ShowWindow();

            Instance.packetEditorPanel.CurrentPacket = OSCEditorUtils.CopyPacket(packet);
            Instance.Focus();
        }

        #endregion

        #region Protected Vars

        protected OSCPanelPacketEditor packetEditorPanel;

        protected OSCPanelContollers controllersPanel;

        #endregion

        #region Private Vars

        private readonly string _lastFileSettings = OSCEditorSettings.Debug + "lastfile";

        #endregion

        #region Unity Methods

        protected override void OnEnable()
        {
            packetEditorPanel = new OSCPanelPacketEditor(this, "debugPacketEditor");
            controllersPanel = new OSCPanelContollers(this, "debugOSCControllers");

            rootPanel.AddPanel(packetEditorPanel, 300);
            rootPanel.AddPanel(controllersPanel, 250);

            base.OnEnable();
        }

        protected void OnInspectorUpdate()
        {
            if (controllersPanel != null)
                controllersPanel.Refresh();

            Repaint();
        }

        #endregion

        #region Protected Methods

        protected override void SaveWindowSettings()
        {
            if (packetEditorPanel == null) return;

            var debugPacket = packetEditorPanel.CurrentPacket;
            if (debugPacket != null)
            {
                if (string.IsNullOrEmpty(packetEditorPanel.FilePath))
                {
                    packetEditorPanel.FilePath = OSCEditorUtils.BackupFolder + "unsaved.eod";
                }

                OSCEditorUtils.SavePacket(packetEditorPanel.FilePath, debugPacket);
                OSCEditorSettings.SetString(_lastFileSettings, packetEditorPanel.FilePath);

                return;
            }

            OSCEditorSettings.SetString(_lastFileSettings, "");
        }

        protected override void LoadWindowSettings()
        {
            if (packetEditorPanel == null) return;

            var lastOpenedFile = OSCEditorSettings.GetString(_lastFileSettings, "");

            if (!string.IsNullOrEmpty(lastOpenedFile))
            {
                var debugPacket = OSCEditorUtils.LoadPacket(lastOpenedFile);
                if (debugPacket != null)
                {
                    packetEditorPanel.CurrentPacket = debugPacket;
                    packetEditorPanel.FilePath = lastOpenedFile;
                }
            }
        }

        #endregion
    }
}