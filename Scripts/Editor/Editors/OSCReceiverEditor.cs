/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEditor;

using System.Reflection;

namespace extOSC.Editor
{
	[CustomEditor(typeof(OSCReceiver))]
	public class OSCReceiverEditor : UnityEditor.Editor
	{
		#region Static Private Vars

		private static readonly GUIContent _localPortContent = new GUIContent("Local Port:");

		private static readonly GUIContent _localHostContent = new GUIContent("Local Host:");

		private static readonly GUIContent _localHostModeContent = new GUIContent("Local Host Mode:");

		private static readonly GUIContent _advancedContent = new GUIContent("Advanced Settings:");

		private static readonly GUIContent _mapBundleContent = new GUIContent("Map Bundle:");

		private static readonly GUIContent _inGameContent = new GUIContent("In Game Controls:");

		private static readonly GUIContent _inEditorContent = new GUIContent("In Editor Controls:");

		private static readonly GUIContent _receiverSettingsContent = new GUIContent("Receiver Settings:");

		private static readonly GUIContent _autoConnectContent = new GUIContent("Auto Connect");

		private static readonly GUIContent _closeOnPauseContent = new GUIContent("Close On Pause");

		private static readonly GUIContent _drownContent = new GUIContent("Receiver is drown!");

		private static string _advancedSettingsText = "\"Advanced settings\" are not available for UWP (WSA).";
		
		private static string _drownText = "OSCReceiver is unable to process the current number of packets. Try reducing the number of packetss, or turn off optimizations: \"Tools/extOSC/\".";

		private static MethodInfo _updateMethod;

		#endregion

		#region Private Vars

		private SerializedProperty _localHostModeProperty;

		private SerializedProperty _localHostProperty;

		private SerializedProperty _localPortProperty;

		private SerializedProperty _autoConnectProperty;

		private SerializedProperty _workInEditorProperty;

		private SerializedProperty _mapBundleProperty;

		private SerializedProperty _closeOnPauseProperty;

		private OSCReceiver _receiver;

		private string _localHostCache;

		private Color _defaultColor;

		#endregion

		#region Unity Methods

		protected void OnEnable()
		{
			_receiver = target as OSCReceiver;
			_localHostCache = OSCUtilities.GetLocalHost();

			_localHostModeProperty = serializedObject.FindProperty("_localHostMode");
			_localHostProperty = serializedObject.FindProperty("_localHost");
			_localPortProperty = serializedObject.FindProperty("_localPort");
			_autoConnectProperty = serializedObject.FindProperty("_autoConnect");
			_workInEditorProperty = serializedObject.FindProperty("_workInEditor");
			_mapBundleProperty = serializedObject.FindProperty("_mapBundle");
			_closeOnPauseProperty = serializedObject.FindProperty("_closeOnPause");

			EditorApplication.update += ReceiverEditorUpdate;

			if (!Application.isPlaying && !_receiver.IsStarted && _workInEditorProperty.boolValue)
			{
				_receiver.Connect();
			}
		}

		protected void OnDisable()
		{
			_receiver = target as OSCReceiver;

			EditorApplication.update -= ReceiverEditorUpdate;

			if (!Application.isPlaying && _receiver.IsStarted)
			{
				_receiver.Close();
			}
		}

		public override void OnInspectorGUI()
		{
			_defaultColor = GUI.color;

			serializedObject.Update();

			EditorGUI.BeginChangeCheck();

			// LOGO
			OSCEditorInterface.LogoLayout();

			// IS DROWN INDICATE
			if (_receiver.IsDrown)
			{
				GUI.color = Color.red;
				using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
				{
					GUILayout.Label(_drownContent, OSCEditorStyles.CenterBoldLabel);
					GUI.color = _defaultColor;
					EditorGUILayout.HelpBox(_drownText, MessageType.Error);
				}
			}

			// INSPECTOR
			EditorGUILayout.LabelField("Active: " + _receiver.IsStarted, EditorStyles.boldLabel);
			using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
			{
				// SETTINGS BLOCK
				EditorGUILayout.LabelField(_receiverSettingsContent, EditorStyles.boldLabel);
				using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
				{
					if (_receiver.LocalHostMode == OSCLocalHostMode.Any)
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

					EditorGUILayout.PropertyField(_localPortProperty, _localPortContent);
					EditorGUILayout.PropertyField(_mapBundleProperty, _mapBundleContent);
				}

				// PARAMETERS BLOCK
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
						EditorGUILayout.HelpBox(_advancedSettingsText, MessageType.Info);
						GUI.color = _defaultColor;
					}

					EditorGUILayout.PropertyField(_localHostModeProperty, _localHostModeContent);
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
			GUI.color = _receiver.IsStarted ? Color.green : Color.red;
			if (GUILayout.Button(_receiver.IsStarted ? "Connected" : "Disconnected"))
			{
				if (_receiver.IsStarted) _receiver.Close();
				else _receiver.Connect();
			}

			GUI.color = Color.yellow;
			GUI.enabled = _receiver.IsStarted;
			if (GUILayout.Button("Reconnect"))
			{
				if (_receiver.IsStarted)
					_receiver.Close();

				_receiver.Connect();
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
					if (_receiver.IsStarted)
						_receiver.Close();

					_receiver.Connect();
				}
				else
				{
					if (_receiver.IsStarted)
						_receiver.Close();
				}
			}

			GUI.color = Color.yellow;
			GUI.enabled = _workInEditorProperty.boolValue;
			if (GUILayout.Button("Reconnect"))
			{
				if (_workInEditorProperty.boolValue)
				{
					if (_receiver.IsStarted)
						_receiver.Close();

					_receiver.Connect();
				}
			}

			GUI.enabled = true;
		}

		protected void ReceiverEditorUpdate()
		{
			if (_updateMethod == null)
				_updateMethod = typeof(OSCReceiver).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic);

			if (_receiver != null && _updateMethod != null)
				_updateMethod.Invoke(_receiver, null);
		}

		#endregion
	}
}