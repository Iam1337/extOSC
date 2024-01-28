/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using System.Collections.Generic;

using extOSC.Core;
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

		private static string _previousFilter;

		private static int _maxBufferCapacity = 256;

		private static readonly OSCConsolePacket[] _emptyBuffer = new OSCConsolePacket[0];

		#endregion

		#region Public Static Methods

		public static void Open()
		{
			Instance.titleContent = new GUIContent("OSC Console", OSCEditorTextures.IronWallSmall);
			Instance.minSize = new Vector2(610, 200);
			Instance.Show();
			Instance.Focus();
		}

		public static OSCConsolePacket[] GetConsoleBuffer(bool transmitted, bool received, string filter)
		{
			if (ConsoleBuffer == null || ConsoleBuffer.Count == 0)
				return _emptyBuffer;

			var requireRebuild = false;

			if (_previousTransmitted != transmitted ||
				_previousReceived != received ||
				_previousFilter != filter)
			{
				_previousTransmitted = transmitted;
				_previousReceived = received;
				_previousFilter = filter;

				requireRebuild = true;
			}
			else
			{
				requireRebuild = ConsoleBuffer[0] != _lastMessage;
			}

			if (!requireRebuild)
				return _tempBuffer;

			_lastMessage = ConsoleBuffer[0];

			var consoleList = new List<OSCConsolePacket>();

			var inverse = filter.StartsWith("!");
			if (inverse)
			{
				filter = filter.Remove(0, 1);
			}

			foreach (var consoleMessage in ConsoleBuffer)
			{
				if (transmitted && consoleMessage.PacketType == OSCConsolePacketType.Transmitted ||
					received && consoleMessage.PacketType == OSCConsolePacketType.Received)
				{
					if (!string.IsNullOrEmpty(filter))
					{
						var address = consoleMessage.Packet.Address;
						var compare = OSCUtilities.CompareAddresses(filter, address);

						if (inverse && compare ||
							!inverse && !compare)
							continue;
					}

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

		#region Private Vars

		private readonly string _showReceivedSettings = OSCEditorSettings.Console + "showreceived";

		private readonly string _showTransmittedSettings = OSCEditorSettings.Console + "showtransmitted";

		private readonly string _trackLastSettings = OSCEditorSettings.Console + "tracklast";

		private readonly string _filterSettings = OSCEditorSettings.Console + "filter";

		private OSCPanelConsole _logPanel;

		private OSCPanelPacket _packetPanel;

		#endregion

		#region Unity Methods

		protected override void OnEnable()
		{
			_logPanel = new OSCPanelConsole(this);
			_packetPanel = new OSCPanelPacket(this);

			rootPanel.Setup(_logPanel, _packetPanel);

			base.OnEnable();
		}

		protected void Update()
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

			if (_packetPanel.SelectedMessage != _logPanel.SelectedMessage)
			{
				_packetPanel.SelectedMessage = _logPanel.SelectedMessage;
				Repaint();
			}
		}

		#endregion

		#region Protected Methods

		protected override void LoadWindowSettings()
		{
			base.LoadWindowSettings();

			if (_logPanel == null) return;

			_logPanel.ShowReceived = OSCEditorSettings.GetBool(_showReceivedSettings, true);
			_logPanel.ShowTransmitted = OSCEditorSettings.GetBool(_showTransmittedSettings, true);
			_logPanel.TrackLast = OSCEditorSettings.GetBool(_trackLastSettings, false);
			_logPanel.Filter = OSCEditorSettings.GetString(_filterSettings, string.Empty);
		}

		protected override void SaveWindowSettings()
		{
			base.SaveWindowSettings();

			if (_logPanel == null) return;

			OSCEditorSettings.SetBool(_showReceivedSettings, _logPanel.ShowReceived);
			OSCEditorSettings.SetBool(_showTransmittedSettings, _logPanel.ShowTransmitted);
			OSCEditorSettings.SetBool(_trackLastSettings, _logPanel.TrackLast);
			OSCEditorSettings.SetString(_filterSettings, _logPanel.Filter);
		}

		#endregion
	}
}