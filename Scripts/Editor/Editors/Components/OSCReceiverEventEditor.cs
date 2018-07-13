/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using extOSC.Components.Events;

namespace extOSC.Editor.Components
{
    [CustomEditor(typeof(OSCReceiverEvent), true)]
    public class OSCReceiverEventEditor : OSCReceiverComponentEditor
    {
        #region Static Private Vars

        private static readonly GUIContent _onReceiveContent = new GUIContent("On Receive:");

        private static readonly GUIContent _eventsSettingsContent = new GUIContent("Events Settings:");

        #endregion

        #region Private Vars

        private SerializedProperty _onReceiveProperty;

        #endregion

        #region Unity Methods

        protected override void OnEnable()
        {
            base.OnEnable();

            _onReceiveProperty = serializedObject.FindProperty("onReceive");
        }

        #endregion

        #region Protected Methods

        protected override void DrawSettings()
        {
            // EVENT SETTINGS
            EditorGUILayout.LabelField(_eventsSettingsContent, EditorStyles.boldLabel);
            GUILayout.BeginVertical();

            EditorGUILayout.PropertyField(_onReceiveProperty, _onReceiveContent);

            // EVENT SETTINGS END
            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}