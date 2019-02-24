/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using System.Reflection;

namespace extOSC.Editor
{
    [CustomEditor(typeof(OSCReceiver))]
	public class OSCReceiverEditor : UnityEditor.Editor
    {
        #region Static Private Vars

        private static readonly GUIContent _portContent = new GUIContent("Local Port:");

        private static readonly GUIContent _hostContent = new GUIContent("Local Host:");

        private static readonly GUIContent _mapBundleContent = new GUIContent("Map Bundle:");

        private static readonly GUIContent _inGameContent = new GUIContent("In Game Controls:");

        private static readonly GUIContent _inEditorContent = new GUIContent("In Editor Controls:");

        private static MethodInfo _updateMethod;

        #endregion

        #region Private Vars

        private SerializedProperty _localPortProperty;

        private SerializedProperty _autoConnectProperty;

        private SerializedProperty _workInEditorProperty;

        private SerializedProperty _mapBundleProperty;

        private SerializedProperty _closeOnPauseProperty;

        private OSCReceiver _receiver;

        private string _localHost;

        #endregion

        #region Unity Methods

        protected void OnEnable()
        {
            _receiver = target as OSCReceiver;
            _localHost = OSCUtilities.GetLocalHost();

            _localPortProperty = serializedObject.FindProperty("localPort");
            _autoConnectProperty = serializedObject.FindProperty("autoConnect");
            _workInEditorProperty = serializedObject.FindProperty("workInEditor");
            _mapBundleProperty = serializedObject.FindProperty("mapBundle");
            _closeOnPauseProperty = serializedObject.FindProperty("closeOnPause");

            EditorApplication.update += ReceiverEditorUpdate;

            if (!Application.isPlaying && !_receiver.IsAvailable && _workInEditorProperty.boolValue)
            {
                _receiver.Connect();
            }
        }

        protected void OnDisable()
        {
            if (_receiver == null)
                _receiver = target as OSCReceiver;

            EditorApplication.update -= ReceiverEditorUpdate;

            if (!Application.isPlaying && _receiver.IsAvailable)
            {
                _receiver.Close();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // LOGO
            GUILayout.Space(10);
            OSCEditorLayout.DrawLogo();
            GUILayout.Space(5);

            // STATUS
            EditorGUILayout.LabelField("Active: " + _receiver.IsAvailable, EditorStyles.boldLabel);

            // SETTINGS BLOCK
            GUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Receiver Settings:", EditorStyles.boldLabel);

            // SETTINGS BOX
            GUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();

            //LOCAL HOST
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(_hostContent, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            EditorGUILayout.SelectableLabel(_localHost, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            GUILayout.EndHorizontal();

            // LOCAL PORT
            EditorGUILayout.PropertyField(_localPortProperty, _portContent);

            // MAP BUNDLE
            EditorGUILayout.PropertyField(_mapBundleProperty, _mapBundleContent);

            // SETTINGS BOX END
            EditorGUILayout.EndVertical();

            // PARAMETERS BLOCK
            EditorGUILayout.BeginHorizontal("box");

            GUI.color = _autoConnectProperty.boolValue ? Color.green : Color.red;
            if (GUILayout.Button("Auto Connect"))
            {
                _autoConnectProperty.boolValue = !_autoConnectProperty.boolValue;
            }
            GUI.color = Color.white;

            GUI.color = _closeOnPauseProperty.boolValue? Color.green : Color.red;
            if (GUILayout.Button("Close On Pause"))
            {
                _closeOnPauseProperty.boolValue = !_closeOnPauseProperty.boolValue;
            }
            GUI.color = Color.white;

            // SETTINGS BLOCK END
            EditorGUILayout.EndHorizontal();

            // CONTROLS
            EditorGUILayout.LabelField(Application.isPlaying ? _inGameContent : _inEditorContent, EditorStyles.boldLabel);

            if (Application.isPlaying) DrawControllsInGame();
            else DrawControllsInEditor();

            // CONTROLS END
            EditorGUILayout.EndVertical();

            // EDITOR BUTTONS
            GUI.color = Color.white;

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }


		#endregion

		#region Private Methods

		protected void DrawControllsInGame()
        {
            EditorGUILayout.BeginHorizontal("box");

            GUI.color = _receiver.IsAvailable ? Color.green : Color.red;
            var connection = GUILayout.Button(_receiver.IsAvailable ? "Connected" : "Disconnected");

            GUI.color = Color.yellow;
            EditorGUI.BeginDisabledGroup(!_receiver.IsAvailable);
            var reconect = GUILayout.Button("Reconnect");
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            if (connection)
            {
                if (_receiver.IsAvailable) _receiver.Close();
                else _receiver.Connect();
            }

            if (reconect)
            {
                if (_receiver.IsAvailable) _receiver.Close();

                _receiver.Connect();
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
                    if (_receiver.IsAvailable) _receiver.Close();

                    _receiver.Connect();
                }
                else
                {
                    if (_receiver.IsAvailable) _receiver.Close();
                }
            }

            if (reconect)
            {
                if (!_workInEditorProperty.boolValue) return;

                if (_receiver.IsAvailable) _receiver.Close();

                _receiver.Connect();
            }
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