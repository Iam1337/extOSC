/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using extOSC.Core;
using extOSC.Editor.Panels;

namespace extOSC.Editor.Windows
{
	public class OSCWindowDebug : OSCWindow<OSCWindowDebug, OSCSplitPanel>
	{
		#region Static Public Vars

		public static IOSCPacket CurrentPacket
		{
			get
			{
				if (Instance != null &&
					Instance._packetEditorPanel != null &&
					Instance._packetEditorPanel.CurrentPacket != null)
					return Instance._packetEditorPanel.CurrentPacket;

				return null;
			}
		}

		#endregion

		#region Static Public Methods

		public static void Open()
		{
			Instance.titleContent = new GUIContent("OSC Debug", OSCEditorTextures.IronWallSmall);
			Instance.minSize = new Vector2(550, 200);
			Instance.Show();
			Instance.Focus();
		}

		public static void OpenPacket(IOSCPacket packet)
		{
			Open();

			Instance._packetEditorPanel.CurrentPacket = packet.Copy();
		}

		#endregion

		#region Private Vars

		private readonly string _lastFileSettings = OSCEditorSettings.Debug + "lastfile";

		private OSCPanelPacketEditor _packetEditorPanel;

		private OSCPanelControllers _controllersPanel;

		#endregion

		#region Unity Methods

		protected override void OnEnable()
		{
			_packetEditorPanel = new OSCPanelPacketEditor(this);
			_controllersPanel = new OSCPanelControllers(this);

			rootPanel.Setup(_packetEditorPanel, _controllersPanel);

			base.OnEnable();
		}

		protected void OnInspectorUpdate()
		{
			if (_controllersPanel != null)
				_controllersPanel.Refresh();

			Repaint();
		}

		#endregion

		#region Protected Methods

		protected override void SaveWindowSettings()
		{
			if (_packetEditorPanel == null) return;

			var debugPacket = _packetEditorPanel.CurrentPacket;
			if (debugPacket != null)
			{
				if (string.IsNullOrEmpty(_packetEditorPanel.FilePath))
				{
					_packetEditorPanel.FilePath = OSCEditorUtils.BackupFolder + "unsaved.eod";
				}

				OSCEditorUtils.SavePacket(_packetEditorPanel.FilePath, debugPacket);
				OSCEditorSettings.SetString(_lastFileSettings, _packetEditorPanel.FilePath);

				return;
			}

			OSCEditorSettings.SetString(_lastFileSettings, "");
		}

		protected override void LoadWindowSettings()
		{
			if (_packetEditorPanel == null) return;

			var lastOpenedFile = OSCEditorSettings.GetString(_lastFileSettings, "");

			if (!string.IsNullOrEmpty(lastOpenedFile))
			{
				var debugPacket = OSCEditorUtils.LoadPacket(lastOpenedFile);
				if (debugPacket != null)
				{
					_packetEditorPanel.CurrentPacket = debugPacket;
					_packetEditorPanel.FilePath = lastOpenedFile;
				}
			}
		}

		#endregion
	}
}