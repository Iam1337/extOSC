/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEditor;
using UnityEngine;

using System.Net;

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

		private static readonly GUIContent _localReceiverContent = new GUIContent("Receiver:");

		private static readonly GUIContent _localPortContent = new GUIContent("Local Port:");

        private static readonly GUIContent _inGameContent = new GUIContent("In Game Controls:");

        private static readonly GUIContent _inEditorContent = new GUIContent("In Editor Controls:");

        #endregion

        #region Private Vars

        private SerializedProperty _remoteHostProperty;

        private SerializedProperty _remotePortProperty;

        private SerializedProperty _autoConnectProperty;

        private SerializedProperty _workInEditorProperty;

        private SerializedProperty _mapBundleProperty;

        private SerializedProperty _useBundleProperty;

        private SerializedProperty _closeOnPauseProperty;

		private SerializedProperty _localPortMode;

		private SerializedProperty _localReceiver;

		private SerializedProperty _localPort;

        private Color _defaultColor;

        private OSCTransmitter _transmitter;

        #endregion

        #region Unity Methods

        protected void OnEnable()
        {
            _transmitter = target as OSCTransmitter;

            _remoteHostProperty = serializedObject.FindProperty("remoteHost");
            _remotePortProperty = serializedObject.FindProperty("remotePort");
            _autoConnectProperty = serializedObject.FindProperty("autoConnect");
            _workInEditorProperty = serializedObject.FindProperty("workInEditor");
            _mapBundleProperty = serializedObject.FindProperty("mapBundle");
            _useBundleProperty = serializedObject.FindProperty("useBundle");
            _closeOnPauseProperty = serializedObject.FindProperty("closeOnPause");
			_localPortMode = serializedObject.FindProperty("localPortMode");
			_localReceiver = serializedObject.FindProperty("localReceiver");
			_localPort = serializedObject.FindProperty("localPort");


            if (!Application.isPlaying && !_transmitter.IsAvaible && _workInEditorProperty.boolValue)
            {
                _transmitter.Connect();
            }
        }

        protected void OnDisable()
        {
            if (_transmitter == null)
                _transmitter = target as OSCTransmitter;

            if (!Application.isPlaying && _transmitter.IsAvaible)
            {
                _transmitter.Close();
            }
        }

        public override void OnInspectorGUI()
        {
            _defaultColor = GUI.color;

            serializedObject.Update();

            // LOGO
            GUILayout.Space(10);
            OSCEditorLayout.DrawLogo();
            GUILayout.Space(5);

            EditorGUILayout.LabelField("Active: " + _transmitter.IsAvaible, EditorStyles.boldLabel);

            // SETTINGS BLOCK
            GUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Transmitter Settings:", EditorStyles.boldLabel);

            // SETTINGS BOX
            GUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();

            IPAddress tempAddress;

            var remoteFieldColor = IPAddress.TryParse(_remoteHostProperty.stringValue, out tempAddress) ? _defaultColor : Color.red;

            // REMOTE HOST
            GUI.color = remoteFieldColor;
            EditorGUILayout.PropertyField(_remoteHostProperty, _hostContent);
            GUI.color = _defaultColor;

            // REMOTE PORT
            EditorGUILayout.PropertyField(_remotePortProperty, _portContent);

            // MAP BUNDLE
            EditorGUILayout.PropertyField(_mapBundleProperty, _mapBundleContent);

            // USE BUNDLE
            GUI.color = _useBundleProperty.boolValue ? Color.green : Color.red;
            if (GUILayout.Button("Use Bundle"))
            {
                _useBundleProperty.boolValue = !_useBundleProperty.boolValue;
            }
            GUI.color = _defaultColor;

            // SETTINGS BOX END
            EditorGUILayout.EndVertical();

            // PARAMETETS BLOCK
            EditorGUILayout.BeginHorizontal("box");

            GUI.color = _autoConnectProperty.boolValue ? Color.green : Color.red;
            if (GUILayout.Button("Auto Connect"))
            {
                _autoConnectProperty.boolValue = !_autoConnectProperty.boolValue;
            }
            GUI.color = _defaultColor;

            GUI.color = _closeOnPauseProperty.boolValue? Color.green : Color.red;
            if (GUILayout.Button("Close On Pause"))
            {
                _closeOnPauseProperty.boolValue = !_closeOnPauseProperty.boolValue;
            }
            GUI.color = _defaultColor;

            // PARAMETERS BLOCK END
            EditorGUILayout.EndHorizontal();

			// ADVANCED SETTIGS BOX
			EditorGUILayout.LabelField(_advancedContent, EditorStyles.boldLabel);
			GUILayout.BeginVertical("box");
			EditorGUI.BeginChangeCheck();

	        if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.WSA)
	        {
		        GUI.color = Color.yellow;
				EditorGUILayout.HelpBox("Currently \"Advanced settings\" are not available for UWP (WSA).", MessageType.Info);
		        GUI.color = _defaultColor;
	        }

	        // LOCAL PORT MODE
			EditorGUILayout.PropertyField(_localPortMode, _localPortModeContent);

			// LOCAL PORT
			if (_transmitter.LocalPortMode == OSCLocalPortMode.FromRemotePort)
			{
				// LOCAL FROM REMOTE PORT
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(_localPortContent, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
				EditorGUILayout.SelectableLabel(_transmitter.RemotePort.ToString(), GUILayout.Height(EditorGUIUtility.singleLineHeight));
				GUILayout.EndHorizontal();
			}
			else if (_transmitter.LocalPortMode == OSCLocalPortMode.FromReceiver)
			{
				// LOCAL RECEIVER
				EditorGUILayout.PropertyField(_localReceiver, _localReceiverContent);

				var localPort = _transmitter.RemotePort.ToString();
				var receiver = _localReceiver.objectReferenceValue as OSCReceiver;
				if (receiver != null) localPort = receiver.LocalPort.ToString();

				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(_localPortContent, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
				EditorGUILayout.SelectableLabel(localPort, GUILayout.Height(EditorGUIUtility.singleLineHeight));
				GUILayout.EndHorizontal();
			}
			else if (_transmitter.LocalPortMode == OSCLocalPortMode.Custom)
			{
				// LOCAL PORT
				EditorGUILayout.PropertyField(_localPort, _localPortContent);
			}

			EditorGUILayout.EndVertical();


            // CONTROLS
            EditorGUILayout.LabelField(Application.isPlaying ? _inGameContent : _inEditorContent, EditorStyles.boldLabel);

            if (Application.isPlaying) DrawControllsInGame();
            else DrawControllsInEditor();

            // CONTROLS END
            EditorGUILayout.EndVertical();

            // EDITOR BUTTONS
            GUI.color = Color.white;


            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Private Methods

        protected void DrawControllsInGame()
        {
            EditorGUILayout.BeginHorizontal("box");

            GUI.color = _transmitter.IsAvaible ? Color.green : Color.red;
            var connection = GUILayout.Button(_transmitter.IsAvaible ? "Connected" : "Disconnected");

            GUI.color = Color.yellow;
            EditorGUI.BeginDisabledGroup(!_transmitter.IsAvaible);
            var reconect = GUILayout.Button("Reconnect");
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            if (connection)
            {
                if (_transmitter.IsAvaible) _transmitter.Close();
                else _transmitter.Connect();
            }

            if (reconect)
            {
                if (_transmitter.IsAvaible) _transmitter.Close();
                _transmitter.Connect();
            }
        }

        protected void DrawControllsInEditor()
        {
            EditorGUILayout.BeginHorizontal("box");

            GUI.color = _workInEditorProperty.boolValue ? Color.green : Color.red;
            var connection = GUILayout.Button("Work In Editor");

            GUI.color = Color.yellow;
            EditorGUI.BeginDisabledGroup(!_workInEditorProperty.boolValue);
            var reconect = GUILayout.Button("Reconnect");
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            if (connection)
            {
                _workInEditorProperty.boolValue = !_workInEditorProperty.boolValue;

                if (_workInEditorProperty.boolValue)
                {
                    if (_transmitter.IsAvaible) _transmitter.Close();

                    _transmitter.Connect();
                }
                else
                {
                    if (_transmitter.IsAvaible) _transmitter.Close();
                }
            }

            if (reconect)
            {
                if (!_workInEditorProperty.boolValue) return;

                if (_transmitter.IsAvaible) _transmitter.Close();

                _transmitter.Connect();
            }
        }

        #endregion
    }
}
