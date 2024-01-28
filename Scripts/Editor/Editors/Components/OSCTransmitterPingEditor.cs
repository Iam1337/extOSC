/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEditor;

using extOSC.Components.Misc;

namespace extOSC.Editor.Components
{
	[CustomEditor(typeof(OSCTransmitterPing), true)]
	public class OSCTransmitterPingEditor : OSCTransmitterComponentEditor
	{
		#region Static Private Vars

		private static readonly GUIContent _settingsContent = new GUIContent("Settings:");

		private static readonly GUIContent _intervalContent = new GUIContent("Interval:");

		private static readonly GUIContent _autoStartContent = new GUIContent("Auto Start");

		private static readonly GUIContent _inGameContent = new GUIContent("In Game Controls:");

		private static readonly GUIContent _startContent = new GUIContent("Start");

		private static readonly GUIContent _stopContent = new GUIContent("Stop");

		private static readonly GUIContent _pauseContent = new GUIContent("Pause");

		#endregion

		#region Private Vars

		private OSCTransmitterPing _ping;

		private SerializedProperty _intervalProperty;

		private SerializedProperty _autoStartProperty;

		private Color _defaultColor;

		#endregion

		#region Unity Methods

		protected override void OnEnable()
		{
			base.OnEnable();

			_ping = target as OSCTransmitterPing;

			_intervalProperty = serializedObject.FindProperty("_interval");
			_autoStartProperty = serializedObject.FindProperty("_autoStart");
		}

		protected override void DrawSettings()
		{
			_defaultColor = GUI.color;

			EditorGUILayout.LabelField(_settingsContent, EditorStyles.boldLabel);
			using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
			{
				EditorGUILayout.PropertyField(_intervalProperty, _intervalContent);

				if (_intervalProperty.floatValue < 0)
					_intervalProperty.floatValue = 0;

				EditorGUILayout.HelpBox("Set to 0 for send message with each frame.", MessageType.Info);
			}

			using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
			{
				GUI.color = _autoStartProperty.boolValue ? Color.green : Color.red;
				if (GUILayout.Button(_autoStartContent))
				{
					_autoStartProperty.boolValue = !_autoStartProperty.boolValue;
				}

				GUI.color = Color.white;
			}

			GUI.enabled = Application.isPlaying;

			EditorGUILayout.LabelField(_inGameContent, EditorStyles.boldLabel);
			using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
			{
				if ((!_ping.IsRunning && !Application.isPlaying && !_ping.AutoStart) ||
					(Application.isPlaying && !_ping.IsRunning))
				{
					GUI.color = Color.green;
					if (GUILayout.Button(_startContent))
					{
						_ping.StartPing();
					}

					GUI.color = _defaultColor;
				}
				else
				{
					using (new GUILayout.HorizontalScope())
					{
						GUI.color = Color.yellow;
						if (GUILayout.Button(_pauseContent))
						{
							_ping.PausePing();
						}

						GUI.color = Color.red;
						if (GUILayout.Button(_stopContent))
						{
							_ping.StopPing();
						}

						GUI.color = _defaultColor;
					}
				}
			}

			GUI.enabled = true;
		}

		#endregion
	}
}