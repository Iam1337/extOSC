/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using extOSC.Components.Ping;

namespace extOSC.Editor.Components
{
	[CustomEditor(typeof(OSCPingClient), true)]
	public class OSCPingClientEditor : OSCComponentEditor
	{
		#region Static Private Vars

		private static readonly GUIContent _pingSettingsContent = new GUIContent("Ping Client Settings:");

		private static readonly GUIContent _pingStatusContent = new GUIContent("Ping Client Status:");

		private static readonly GUIContent _intervalContent = new GUIContent("Interval:");

		private static readonly GUIContent _timeoutContent = new GUIContent("Timeout:");

		private static readonly GUIContent _autoStartContent = new GUIContent("Auto Start");

		private static readonly GUIContent _inGameContent = new GUIContent("In Game Controls:");

		private static readonly GUIContent _startContent = new GUIContent("Start");

		private static readonly GUIContent _stopContent = new GUIContent("Stop");

		private static readonly GUIContent _pauseContent = new GUIContent("Pause");

		#endregion

		#region Private Vars

		private OSCPingClient _ping;

		private SerializedProperty _intervalProperty;

		private SerializedProperty _timeoutProperty;

		private SerializedProperty _autoStartProperty;

		private bool _drawedState;

		private Color _defaultColor;

		#endregion

		#region Unity Methods

		protected override void OnEnable()
		{
			base.OnEnable();

			_ping = target as OSCPingClient;

			_intervalProperty = serializedObject.FindProperty("_interval");
			_timeoutProperty = serializedObject.FindProperty("_timeout");
			_autoStartProperty = serializedObject.FindProperty("_autoStart");

			EditorApplication.update += Update;
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			EditorApplication.update -= Update;
		}

		#endregion

		#region Protected Methods

		protected override void DrawSettings()
		{
			_defaultColor = GUI.color;

			// PING SETTINGS
			EditorGUILayout.LabelField(_pingSettingsContent, EditorStyles.boldLabel);
			using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
			{
				EditorGUILayout.PropertyField(_intervalProperty, _intervalContent);
				EditorGUILayout.PropertyField(_timeoutProperty, _timeoutContent);
			}

			using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
			{
				GUI.color = _autoStartProperty.boolValue ? Color.green : Color.red;
				if (GUILayout.Button(_autoStartContent))
				{
					_autoStartProperty.boolValue = !_autoStartProperty.boolValue;
				}

				GUI.color = _defaultColor;
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

			EditorGUILayout.LabelField(_pingStatusContent, EditorStyles.boldLabel);
			using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
			{
				_drawedState = _ping.IsAvailable;

				GUI.color = _drawedState ? Color.green : Color.red;
				using (new GUILayout.VerticalScope(EditorStyles.helpBox))
				{
					EditorGUILayout.LabelField(_drawedState ? "Available" : "Not Available", OSCEditorStyles.CenterLabel);
				}

				GUI.color = _defaultColor;
			}
		}

		#endregion

		#region Private Methods

		private void Update()
		{
			if (_ping == null)
				return;

			if (_ping.IsAvailable != _drawedState)
				Repaint();
		}

		#endregion
	}
}