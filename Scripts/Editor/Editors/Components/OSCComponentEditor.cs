/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using extOSC.Components;

namespace extOSC.Editor.Components
{
    [CustomEditor(typeof(OSCComponent), true)]
    public class OSCComponentEditor : UnityEditor.Editor
    {
        #region Static Private Vars

        private static readonly GUIContent _transmitterComponentSettingsContent = new GUIContent("Transmitter Settings:");

        private static readonly GUIContent _receiverComponentSettingsContent = new GUIContent("Receiver Settings:");

        private static readonly GUIContent _otherSettingsContent = new GUIContent("Other Settings:");

        private static readonly GUIContent _settingsTitleContent = new GUIContent("Settings:");

        #endregion

        #region Private Vars

        private SerializedProperty _transmitterProperty;

        private SerializedProperty _transmitterAddressProperty;

        private SerializedProperty _receiverProperty;

        private SerializedProperty _receiverAddressProperty;

        #endregion

        #region Unity Methods

        protected virtual void OnEnable()
        {
            _transmitterProperty = serializedObject.FindProperty("transmitter");
            _transmitterAddressProperty = serializedObject.FindProperty("transmitterAddress");
            _receiverProperty = serializedObject.FindProperty("receiver");
            _receiverAddressProperty = serializedObject.FindProperty("receiverAddress");
            _settingsTitleContent.text = string.Format("{0} Settings:", target.GetType().Name);
        }

        protected virtual void OnDisable()
        { }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // LOGO
            GUILayout.Space(10);
            OSCEditorLayout.DrawLogo();
            GUILayout.Space(5);


            EditorGUILayout.LabelField(_settingsTitleContent, EditorStyles.boldLabel);
            GUILayout.BeginVertical("box");

            // TRANSMITTER SETTINGS BLOCK
            EditorGUILayout.LabelField(_transmitterComponentSettingsContent, EditorStyles.boldLabel);
            OSCEditorLayout.TransmitterSettings(_transmitterProperty, _transmitterAddressProperty);
            // TRANSMITTER SETTINGS BOX END

            // RECEIVER SETTINGS BLOCK
            EditorGUILayout.LabelField(_receiverComponentSettingsContent, EditorStyles.boldLabel);
            OSCEditorLayout.ReceiverSettings(_receiverProperty, _receiverAddressProperty);
            // SETTINGS BOX END

            DrawSettings();

            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Protected Methods

        protected virtual void DrawSettings()
        {
            // CUSTOM SETTINGS
            EditorGUILayout.LabelField(_otherSettingsContent, EditorStyles.boldLabel);
            GUILayout.BeginVertical("box");

            OSCEditorLayout.DrawProperties(serializedObject, _transmitterAddressProperty.name,
                                           _transmitterProperty.name, _receiverAddressProperty.name,
                                           _receiverProperty.name);

            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}