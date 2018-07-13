/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using extOSC.Components;

namespace extOSC.Editor.Components
{
    [CustomEditor(typeof(OSCTransmitterComponent), true)]
    public class OSCTransmitterComponentEditor : UnityEditor.Editor
    {
        #region Static Private Vars

        private static readonly GUIContent _transmitterComponentSettingsContent = new GUIContent("Transmitter Component Settings:");

        private static readonly GUIContent _otherSettingsContent = new GUIContent("Other Settings:");

		private static readonly GUIContent _mapBundleContent = new GUIContent("Map Bundle:");

		#endregion

		#region Private Vars

	    private OSCTransmitterComponent _transmitterComponent;

		private SerializedProperty _transmitterProperty;

        private SerializedProperty _addressProperty;

	    private SerializedProperty _mapBundleProperty;

		#endregion

		#region Unity Methods

		protected virtual void OnEnable()
        {
	        _transmitterComponent = target as OSCTransmitterComponent;
	        
			_transmitterProperty = serializedObject.FindProperty("transmitter");
            _addressProperty = serializedObject.FindProperty("address");
	        _mapBundleProperty = serializedObject.FindProperty("mapBundle");
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

            GUILayout.BeginVertical("box");

			// SETTINGS BLOCK
			EditorGUILayout.LabelField(_transmitterComponentSettingsContent, EditorStyles.boldLabel);

	        GUILayout.BeginVertical("box");
			OSCEditorLayout.TransmitterSettings(_transmitterProperty, _addressProperty, false);
	        EditorGUILayout.PropertyField(_mapBundleProperty, _mapBundleContent);

	        if (_transmitterComponent.Transmitter != null && _transmitterComponent.Transmitter.MapBundle != null &&
	            _transmitterComponent.MapBundle != null)
	        {
		        EditorGUILayout.HelpBox("OSCTransmitter already has MapBundle.", MessageType.Info);
	        }

	        EditorGUILayout.EndVertical();

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
            EditorGUILayout.BeginVertical("box");

            OSCEditorLayout.DrawProperties(serializedObject, _addressProperty.name, _transmitterProperty.name, _mapBundleProperty.name);

            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}