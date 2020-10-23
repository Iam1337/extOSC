/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using System.Reflection;

using extOSC.Core;
using extOSC.Editor.Windows;

namespace extOSC.Editor.Panels
{
	public class OSCPanelControllers : OSCPanel
	{
		#region Private Static Vars

		private static readonly GUIContent _sendActionContent = new GUIContent("Send");

		private static readonly GUIContent _receiveActionContent = new GUIContent("Receive");

		private static readonly GUIContent _selectActionContent = new GUIContent("Select");

		private static readonly GUIContent _transmittersContent = new GUIContent("Transmitters:");

		private static readonly GUIContent _receiversContent = new GUIContent("Receivers:");

		private static readonly GUIContent _actionsContent = new GUIContent("Actions:");

		#endregion

		#region Private Vars

		private OSCTransmitter[] _transmitters;

		private OSCReceiver[] _receivers;

		private Vector2 _scrollPosition;

		private MethodInfo _receiveMethod;

		private Color _defaultColor;

		#endregion

		#region Unity Methods

		protected override void DrawContent(ref Rect contentRect)
		{
			_defaultColor = GUI.color;

			using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
			{
				GUILayout.FlexibleSpace();
			}

			using (var scroll = new GUILayout.ScrollViewScope(_scrollPosition))
			{
				EditorGUILayout.HelpBox("For component activation in Edit Mode you need to choose appropriate GameObject and have \"Work In Editor\" turned on.",
										MessageType.Info);

				var expand = contentRect.width > 350;
				if (expand)
					GUILayout.BeginHorizontal();

				using (new GUILayout.VerticalScope())
				{
					GUILayout.Label(_transmittersContent, OSCEditorStyles.ConsoleBoldLabel);

					if (_transmitters.Length > 0)
					{
						for (var i = 0; i < _transmitters.Length; ++i)
						{
							DrawElement(_transmitters[i]);
						}
					}
					else
					{
						EditorGUILayout.HelpBox("Scene doesn't have OSCTransmitter.", MessageType.Info);
					}
				}

				using (new GUILayout.VerticalScope())
				{
					GUILayout.Label(_receiversContent, OSCEditorStyles.ConsoleBoldLabel);

					if (_receivers.Length > 0)
					{
						for (var i = 0; i < _receivers.Length; ++i)
						{
							DrawElement(_receivers[i]);
						}
					}
					else
					{
						EditorGUILayout.HelpBox("Scene doesn't have OSCReceiver.", MessageType.Info);
					}
				}

				if (expand)
					GUILayout.EndHorizontal();

				_scrollPosition = scroll.scrollPosition;
			}
		}

		#endregion

		#region Public Methods

		public OSCPanelControllers(OSCWindow window) : base(window)
		{
			Refresh();
		}

		public void Refresh()
		{
			_transmitters = Object.FindObjectsOfType<OSCTransmitter>();
			_receivers = Object.FindObjectsOfType<OSCReceiver>();
		}

		#endregion

		#region Private Methods

		private void DrawElement(OSCBase osc)
		{
			GUI.color = osc.IsStarted ? Color.green : Color.red;
			using (new GUILayout.VerticalScope(EditorStyles.helpBox))
			{
				DrawName(osc);
				using (new GUILayout.HorizontalScope(OSCEditorStyles.Box))
				{
					GUILayout.Label("Active: " + osc.IsStarted);
				}

				GUILayout.Label(_actionsContent);
				using (new GUILayout.HorizontalScope(OSCEditorStyles.Box))
				{
					DrawActions(osc);
				}
			}

			GUI.color = _defaultColor;
		}

		private void DrawName(OSCBase osc)
		{
			var transmitter = osc as OSCTransmitter;
			if (transmitter != null)
			{
				GUILayout.Label($"Transmitter: {transmitter.RemoteHost}:{transmitter.RemotePort}");
			}

			var receiver = osc as OSCReceiver;
			if (receiver != null)
			{
				GUILayout.Label($"Receiver: {receiver.LocalPort}");
			}
		}

		private void DrawActions(OSCBase osc)
		{
			GUI.color = Color.yellow;
			GUI.enabled = osc.IsStarted;

			var transmitter = osc as OSCTransmitter;
			if (transmitter != null)
			{
				if (GUILayout.Button(_sendActionContent))
				{
					var debugPacket = OSCWindowDebug.CurrentPacket;
					if (debugPacket != null)
					{
						transmitter.Send(debugPacket.Copy(), OSCSendOptions.IgnoreBundle);
					}
				}
			}

			var receiver = osc as OSCReceiver;
			if (receiver != null)
			{
				if (GUILayout.Button(_receiveActionContent))
				{
					var debugPacket = OSCWindowDebug.CurrentPacket;
					if (debugPacket != null)
					{
						if (_receiveMethod == null)
							_receiveMethod = typeof(OSCReceiver).GetMethod("PacketReceived", BindingFlags.Instance | BindingFlags.NonPublic);

						_receiveMethod.Invoke(receiver, new object[] {debugPacket.Copy()});
					}
				}
			}

			GUI.enabled = true;
			GUI.color = _defaultColor;

			var selectButton = GUILayout.Button(_selectActionContent, GUILayout.MaxWidth(60));
			if (selectButton)
			{
				EditorGUIUtility.PingObject(osc);
				Selection.activeObject = osc;
			}
		}

		#endregion
	}
}