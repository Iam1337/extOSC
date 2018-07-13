/* Copyright (c) 2018 ExT (V.Sigalkin) */

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

            _intervalProperty = serializedObject.FindProperty("interval");
            _autoStartProperty = serializedObject.FindProperty("autoStart");
        }

        protected override void DrawSettings()
        {
            _defaultColor = GUI.color;

            // INTERVALL
            EditorGUILayout.LabelField(_settingsContent, EditorStyles.boldLabel);
            GUILayout.BeginVertical("box");

            EditorGUILayout.PropertyField(_intervalProperty, _intervalContent);

            if (_intervalProperty.floatValue < 0) _intervalProperty.floatValue = 0;

            EditorGUILayout.HelpBox("Set to 0 for send message with each frame.", MessageType.Info);

            GUILayout.EndVertical();

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
        }

        #endregion
    }
}