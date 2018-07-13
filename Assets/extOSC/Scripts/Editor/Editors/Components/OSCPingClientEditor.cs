/* Copyright (c) 2018 ExT (V.Sigalkin) */

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

        private Color _defaultColor;

        private bool _drawedState;

        #endregion

        #region Unity Methods

        protected override void OnEnable()
        {
            base.OnEnable();

            _ping = target as OSCPingClient;

            _intervalProperty = serializedObject.FindProperty("interval");
            _timeoutProperty = serializedObject.FindProperty("timeout");
            _autoStartProperty = serializedObject.FindProperty("autoStart");

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
            GUILayout.BeginVertical("box");

            EditorGUILayout.PropertyField(_intervalProperty, _intervalContent);
            EditorGUILayout.PropertyField(_timeoutProperty, _timeoutContent);

            // PING SETTINGS END
            EditorGUILayout.EndVertical();

            GUILayout.BeginVertical("box");

            GUI.color = _autoStartProperty.boolValue ? Color.green : Color.red;
            if (GUILayout.Button(_autoStartContent))
            {
                _autoStartProperty.boolValue = !_autoStartProperty.boolValue;
            }
            GUI.color = _defaultColor;

            GUILayout.EndVertical();

            GUI.enabled = Application.isPlaying;

            EditorGUILayout.LabelField(_inGameContent, EditorStyles.boldLabel);
            GUILayout.BeginVertical("box");

            if ((!_ping.IsRunning && (!Application.isPlaying && !_ping.AutoStart)) ||
                (Application.isPlaying && !_ping.IsRunning))
            {
                GUI.color = Color.green;

                var play = GUILayout.Button(_startContent);
                if (play)
                {
                    _ping.StartPing();
                }

                GUI.color = _defaultColor;
            }
            else
            {
                GUILayout.BeginHorizontal();

                GUI.color = Color.yellow;

                var pause = GUILayout.Button(_pauseContent);
                if (pause)
                {
                    _ping.PausePing();
                }

                GUI.color = Color.red;

                var stop = GUILayout.Button(_stopContent);
                if (stop)
                {
                    _ping.StopPing();
                }

                GUI.color = _defaultColor;

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUI.enabled = true;

            EditorGUILayout.LabelField(_pingStatusContent, EditorStyles.boldLabel);
            GUILayout.BeginVertical("box");

            _drawedState = _ping.IsAvaible;

            GUI.color = _drawedState ? Color.green : Color.red;
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(_drawedState ? "Avaible" : "Not Avaible", OSCEditorStyles.CenterLabel);
            GUILayout.EndVertical();
            GUI.color = _defaultColor;

            GUILayout.EndVertical();
        }

        #endregion

        #region Private Methods

        private void Update()
        {
            if (_ping == null)
                return;

            if (_ping.IsAvaible != _drawedState)
                Repaint();
        }

        #endregion
    }
}