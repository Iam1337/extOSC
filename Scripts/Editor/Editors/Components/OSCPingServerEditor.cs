/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using extOSC.Components.Ping;

namespace extOSC.Editor.Components
{
    [CustomEditor(typeof(OSCPingServer), true)]
    public class OSCPingServerEditor : OSCComponentEditor
    {
        #region Static Private Vars

        private static readonly GUIContent _transmitterContent = new GUIContent("OSC Transmitter:");

        private static readonly GUIContent _transmitterAddressContent = new GUIContent("OSC Transmitter Address:");

        private static readonly GUIContent _transmitterAddressContentSmall = new GUIContent("Transmitter Address:");

        private static readonly GUIContent _transmitterComponentSettingsContent = new GUIContent("Transmitter Settings:");

        private static readonly GUIContent _receiverComponentSettingsContent = new GUIContent("Receiver Settings:");

        #endregion

        #region Private Vars

        private SerializedProperty _transmitterProperty;

        private SerializedProperty _receiverProperty;

        private SerializedProperty _receiverAddressProperty;

        private OSCPingServer _ping;

        #endregion

        #region Unity Methods

        protected override void OnEnable()
        {
            _ping = target as OSCPingServer;

            _transmitterProperty = serializedObject.FindProperty("transmitter");
            _receiverProperty = serializedObject.FindProperty("receiver");
            _receiverAddressProperty = serializedObject.FindProperty("receiverAddress");
        }

        protected override void OnDisable()
        { }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            // LOGO
            GUILayout.Space(10);
            OSCEditorLayout.DrawLogo();
            GUILayout.Space(5);

            EditorGUILayout.LabelField(string.Format("{0} Settings:", target.GetType().Name), EditorStyles.boldLabel);
            GUILayout.BeginVertical("box");

            // RECEIVER SETTINGS BLOCK
            EditorGUILayout.LabelField(_receiverComponentSettingsContent, EditorStyles.boldLabel);
            OSCEditorLayout.ReceiverSettings(_receiverProperty, _receiverAddressProperty);

            // TRANSMITTER SETTINGS BLOCK
            EditorGUILayout.LabelField(_transmitterComponentSettingsContent, EditorStyles.boldLabel);
            GUILayout.BeginVertical("box");

            OSCEditorLayout.TransmittersPopup(_transmitterProperty, _transmitterContent);

            var transmitterAddress = "- None -";

            if (Application.isPlaying)
            {
                transmitterAddress = _ping.TransmitterAddress;
            }

            EditorGUILayout.LabelField(EditorGUIUtility.currentViewWidth > 410 ?
                                       _transmitterAddressContent.text : _transmitterAddressContentSmall.text,
                                       transmitterAddress);

            // SETTINGS BOX END
            EditorGUILayout.EndVertical();

            DrawSettings();

            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Protected Methods

        protected override void DrawSettings()
        { }

        #endregion
    }
}