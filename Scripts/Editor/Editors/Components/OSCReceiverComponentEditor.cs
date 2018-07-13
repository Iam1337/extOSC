/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using extOSC.Components;

namespace extOSC.Editor.Components
{
    [CustomEditor(typeof(OSCReceiverComponent), true)]
    public class OSCReceiverComponentEditor : UnityEditor.Editor
    {
        #region Static Private Vars

        private static readonly GUIContent _receiverComponentSettingsContent = new GUIContent("Receiver Component Settings:");

        private static readonly GUIContent _otherSettingsContent = new GUIContent("Other Settings:");

	    private static readonly GUIContent _mapBundleContent = new GUIContent("Map Bundle:");

		#endregion

		#region Private Vars

	    private OSCReceiverComponent _receiverComponent;

		private SerializedProperty _receiverProperty;

        private SerializedProperty _addressProperty;

	    private SerializedProperty _mapBundleProperty;

		#endregion

		#region Unity Methods

		protected virtual void OnEnable()
		{
			_receiverComponent = target as OSCReceiverComponent;

			_receiverProperty = serializedObject.FindProperty("receiver");
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
            EditorGUILayout.LabelField(_receiverComponentSettingsContent, EditorStyles.boldLabel);

	        GUILayout.BeginVertical("box");
			OSCEditorLayout.ReceiverSettings(_receiverProperty, _addressProperty, false);
	        EditorGUILayout.PropertyField(_mapBundleProperty, _mapBundleContent);

	        if (_receiverComponent.Receiver != null && _receiverComponent.Receiver.MapBundle != null &&
	            _receiverComponent.MapBundle != null)
	        {
				EditorGUILayout.HelpBox("OSCReceiver already has MapBundle.", MessageType.Info);
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

            OSCEditorLayout.DrawProperties(serializedObject, _addressProperty.name, _receiverProperty.name, _mapBundleProperty.name);

            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}