/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEditor;
using UnityEngine;

namespace extOSC.Editor
{
	[CustomEditor(typeof(OSCTransmitter))]
	public class OSCTransmitterEditor : UnityEditor.Editor
	{
		#region Static Private Vars

		private static readonly GUIContent _hostContent = new GUIContent("Remote Host:");

		private static readonly GUIContent _portContent = new GUIContent("Remote Port:");

		private static readonly GUIContent _mapBundleContent = new GUIContent("Map Bundle:");

		private static readonly GUIContent _advancedContent = new GUIContent("Advanced Settings:");

		private static readonly GUIContent _localPortModeContent = new GUIContent("Local Port Mode:");

		private static readonly GUIContent _localHostModeContent = new GUIContent("Local Host Mode:");

		private static readonly GUIContent _sourceReceiverContent = new GUIContent("Source Receiver:");

		private static readonly GUIContent _localHostContent = new GUIContent("Local Host:");

		private static readonly GUIContent _localPortContent = new GUIContent("Local Port:");

		private static readonly GUIContent _inGameContent = new GUIContent("In Game Controls:");

		private static readonly GUIContent _inEditorContent = new GUIContent("In Editor Controls:");

		private static readonly GUIContent _transmitterSettingsContent = new GUIContent("Transmitter Settings:");

		private static readonly GUIContent _useBundleContent = new GUIContent("Use Bundle");

		private static readonly GUIContent _autoConnectContent = new GUIContent("Auto Connect");

		private static readonly GUIContent _closeOnPauseContent = new GUIContent("Close On Pause");

		private static readonly GUIContent _orContent = new GUIContent("Or...");

		private static string _advancedHelp = "Currently \"Advanced settings\" are not available for UWP (WSA).";

		private static string _fromReceiverHelp = "\"FromReceiver\" option is deprecated. Use \"Source Receiver\" settings.";

		#endregion

		#region Private Vars

		private SerializedProperty _remoteHostProperty;

		private SerializedProperty _remotePortProperty;

		private SerializedProperty _autoConnectProperty;

		private SerializedProperty _workInEditorProperty;

		private SerializedProperty _mapBundleProperty;

		private SerializedProperty _useBundleProperty;

		private SerializedProperty _closeOnPauseProperty;

		private SerializedProperty _sourceReceiverProperty;

		private SerializedProperty _localHostModeProperty;

		private SerializedProperty _localHostProperty;

		private SerializedProperty _localPortModeProperty;

		private SerializedProperty _localPortProperty;

		private OSCTransmitter _transmitter;

		private string _localHostCache;

		private Color _defaultColor;

		#endregion

		#region Unity Methods

		protected void OnEnable()
		{
			_transmitter = target as OSCTransmitter;
			_localHostCache = OSCUtilities.GetLocalHost();

			_remoteHostProperty = serializedObject.FindProperty("_remoteHost");
			_remotePortProperty = serializedObject.FindProperty("_remotePort");
			_autoConnectProperty = serializedObject.FindProperty("_autoConnect");
			_workInEditorProperty = serializedObject.FindProperty("_workInEditor");
			_mapBundleProperty = serializedObject.FindProperty("_mapBundle");
			_useBundleProperty = serializedObject.FindProperty("_useBundle");
			_closeOnPauseProperty = serializedObject.FindProperty("_closeOnPause");
			_sourceReceiverProperty = serializedObject.FindProperty("_localReceiver");
			_localHostModeProperty = serializedObject.FindProperty("_localHostMode");
			_localHostProperty = serializedObject.FindProperty("_localHost");
			_localPortModeProperty = serializedObject.FindProperty("_localPortMode");
			_localPortProperty = serializedObject.FindProperty("_localPort");

			if (!Application.isPlaying && !_transmitter.IsStarted && _workInEditorProperty.boolValue)
			{
				_transmitter.Connect();
			}
		}

		protected void OnDisable()
		{
			if (_transmitter == null)
				_transmitter = target as OSCTransmitter;

			if (!Application.isPlaying && _transmitter.IsStarted)
			{
				_transmitter.Close();
			}
		}

		public override void OnInspectorGUI()
		{
			_defaultColor = GUI.color;

			serializedObject.Update();

			EditorGUI.BeginChangeCheck();

			// LOGO
			OSCEditorInterface.LogoLayout();

			EditorGUILayout.LabelField("Active: " + _transmitter.IsStarted, EditorStyles.boldLabel);
			using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
			{
				// SETTINGS BLOCK
				EditorGUILayout.LabelField(_transmitterSettingsContent, EditorStyles.boldLabel);
				using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
				{
					EditorGUILayout.PropertyField(_remoteHostProperty, _hostContent);
					EditorGUILayout.PropertyField(_remotePortProperty, _portContent);
					EditorGUILayout.PropertyField(_mapBundleProperty, _mapBundleContent);

					GUI.color = _useBundleProperty.boolValue ? Color.green : Color.red;
					if (GUILayout.Button(_useBundleContent))
					{
						_useBundleProperty.boolValue = !_useBundleProperty.boolValue;
					}

					GUI.color = _defaultColor;
				}

				// PROPERTIES BLOCK
				using (new GUILayout.HorizontalScope(OSCEditorStyles.Box))
				{
					GUI.color = _autoConnectProperty.boolValue ? Color.green : Color.red;
					if (GUILayout.Button(_autoConnectContent))
					{
						_autoConnectProperty.boolValue = !_autoConnectProperty.boolValue;
					}

					GUI.color = _closeOnPauseProperty.boolValue ? Color.green : Color.red;
					if (GUILayout.Button(_closeOnPauseContent))
					{
						_closeOnPauseProperty.boolValue = !_closeOnPauseProperty.boolValue;
					}

					GUI.color = _defaultColor;
				}

				// ADVANCED BLOCK
				EditorGUILayout.LabelField(_advancedContent, EditorStyles.boldLabel);
				using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
				{
					if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.WSA)
					{
						GUI.color = Color.yellow;
						EditorGUILayout.HelpBox(_advancedHelp, MessageType.Info);

						GUI.color = _defaultColor;
					}

					EditorGUILayout.PropertyField(_sourceReceiverProperty, _sourceReceiverContent);

					var sourceReceiver = _transmitter.SourceReceiver;
					if (sourceReceiver != null)
					{
						var localHost = sourceReceiver.LocalHostMode == OSCLocalHostMode.Any
							? _localHostCache
							: sourceReceiver.LocalHost;
						var localPort = sourceReceiver.LocalPort.ToString();

						using (new GUILayout.HorizontalScope())
						{
							EditorGUILayout.LabelField(_localHostContent, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
							EditorGUILayout.SelectableLabel(localHost, GUILayout.Height(EditorGUIUtility.singleLineHeight));
						}

						using (new GUILayout.HorizontalScope())
						{
							EditorGUILayout.LabelField(_localPortContent, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
							EditorGUILayout.SelectableLabel(localPort, GUILayout.Height(EditorGUIUtility.singleLineHeight));
						}
					}
					else
					{
						EditorGUILayout.LabelField(_orContent, EditorStyles.boldLabel);

						// LOCAL HOST MODE
						EditorGUILayout.PropertyField(_localHostModeProperty, _localHostModeContent);
						if (_transmitter.LocalHostMode == OSCLocalHostMode.Any)
						{
							using (new GUILayout.HorizontalScope())
							{
								EditorGUILayout.LabelField(_localHostContent, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
								EditorGUILayout.SelectableLabel(_localHostCache, GUILayout.Height(EditorGUIUtility.singleLineHeight));
							}
						}
						else
						{
							EditorGUILayout.PropertyField(_localHostProperty, _localHostContent);
						}

						// LOCAL PORT MODE
						EditorGUILayout.PropertyField(_localPortModeProperty, _localPortModeContent);
						if (_transmitter.LocalPortMode == OSCLocalPortMode.FromRemotePort)
						{
							// LOCAL FROM REMOTE PORT
							using (new GUILayout.HorizontalScope())
							{
								EditorGUILayout.LabelField(_localPortContent, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
								EditorGUILayout.SelectableLabel(_transmitter.RemotePort.ToString(), GUILayout.Height(EditorGUIUtility.singleLineHeight));
							}
						}
						else if (_transmitter.LocalPortMode == OSCLocalPortMode.FromReceiver)
						{
							GUI.color = Color.red;
							EditorGUILayout.HelpBox(_fromReceiverHelp, MessageType.Warning);

							GUI.color = _defaultColor;
						}
						else if (_transmitter.LocalPortMode == OSCLocalPortMode.Custom)
						{
							EditorGUILayout.PropertyField(_localPortProperty, _localPortContent);
						}
					}
				}

				// CONTROLS
				EditorGUILayout.LabelField(Application.isPlaying ? _inGameContent : _inEditorContent, EditorStyles.boldLabel);
				using (new GUILayout.HorizontalScope(OSCEditorStyles.Box))
				{
					if (Application.isPlaying) InGameControls();
					else InEditorControls();
				}
			}

			if (EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}

		#endregion

		#region Private Methods

		protected void InGameControls()
		{
			GUI.color = _transmitter.IsStarted ? Color.green : Color.red;
			if (GUILayout.Button(_transmitter.IsStarted ? "Connected" : "Disconnected"))
			{
				if (_transmitter.IsStarted)
					_transmitter.Close();

				else _transmitter.Connect();
			}

			GUI.color = Color.yellow;
			GUI.enabled = _transmitter.IsStarted;
			if (GUILayout.Button("Reconnect"))
			{
				if (_transmitter.IsStarted)
					_transmitter.Close();

				_transmitter.Connect();
			}

			GUI.enabled = true;
		}

		protected void InEditorControls()
		{
			GUI.color = _workInEditorProperty.boolValue ? Color.green : Color.red;
			if (GUILayout.Button("Work In Editor"))
			{
				_workInEditorProperty.boolValue = !_workInEditorProperty.boolValue;

				if (_workInEditorProperty.boolValue)
				{
					if (_transmitter.IsStarted)
						_transmitter.Close();

					_transmitter.Connect();
				}
				else
				{
					if (_transmitter.IsStarted)
						_transmitter.Close();
				}
			}

			GUI.color = Color.yellow;
			GUI.enabled = _workInEditorProperty.boolValue;
			if (GUILayout.Button("Reconnect"))
			{
				if (_workInEditorProperty.boolValue)
				{
					if (_transmitter.IsStarted)
						_transmitter.Close();

					_transmitter.Connect();
				}
			}

			GUI.enabled = true;
		}

		#endregion
	}
}