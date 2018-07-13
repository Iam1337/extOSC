﻿/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using System;

using extOSC.Components.Misc;

namespace extOSC.Editor.Components
{
    [CustomEditor(typeof(OSCReceiverGyro), true)]
    public class OSCReceiverGyroEditor : OSCReceiverComponentEditor
    {
        #region Static Private Vars

        private static readonly GUIContent _modeContent = new GUIContent("Mode:");

        private static readonly GUIContent _lerpSpeedContent = new GUIContent("Lerp Speed:");

        private static readonly GUIContent _eventsSettingsContent = new GUIContent("Events Settings:");

        #endregion

        #region Static Protected Methods

        protected static OSCReceiverGyro.GyroMode GetGyroModeEnum(SerializedProperty property)
        {
            var enumValues = Enum.GetValues(typeof(OSCReceiverGyro.GyroMode));

            return (OSCReceiverGyro.GyroMode)enumValues.GetValue(property.enumValueIndex);
        }

        #endregion

        #region Private Vars

        private SerializedProperty _speedProperty;

        private SerializedProperty _modeProperty;

        #endregion

        #region Unity Methods

        protected override void OnEnable()
        {
            base.OnEnable();

            _speedProperty = serializedObject.FindProperty("speed");
            _modeProperty = serializedObject.FindProperty("mode");
        }

        protected override void DrawSettings()
        {
            // EVENT SETTINGS
            EditorGUILayout.LabelField(_eventsSettingsContent, EditorStyles.boldLabel);
            GUILayout.BeginVertical("box");

            EditorGUILayout.PropertyField(_speedProperty, _lerpSpeedContent);
            EditorGUILayout.PropertyField(_modeProperty, _modeContent);

            if (GetGyroModeEnum(_modeProperty) == OSCReceiverGyro.GyroMode.TouchOSC)
            {
                EditorGUILayout.HelpBox("Use this mode only with TouchOSC app.", MessageType.Info);
            }

            // EVENT SETTINGS END
            EditorGUILayout.EndVertical();
        }

        #endregion

    }
}