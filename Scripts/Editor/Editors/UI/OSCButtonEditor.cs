/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

using extOSC.UI;

namespace extOSC.Editor.Components.UI
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(OSCButton), true)]
	public class OSCButtonEditor : SelectableEditor
	{
		#region Static Private Vars

		private static readonly GUIContent _settingsContent = new GUIContent("Settings:");

		private static readonly GUIContent _buttonSettingsContent = new GUIContent("Button Settings:");

		private static readonly GUIContent _valueSettingsContent = new GUIContent("Value Settings:");

		#endregion

		#region Private Vars

		private SerializedProperty _valueProperty;

		private SerializedProperty _onValueChangedProperty;

		private SerializedProperty _buttonTypeProperty;

		private SerializedProperty _graphicTransitionProperty;

		private SerializedProperty _graphicProperty;

		#endregion

		#region Unity Methods

		protected override void OnEnable()
		{
			base.OnEnable();

			_valueProperty = serializedObject.FindProperty("_value");
			_onValueChangedProperty = serializedObject.FindProperty("_onValueChanged");
			_buttonTypeProperty = serializedObject.FindProperty("_buttonType");
			_graphicTransitionProperty = serializedObject.FindProperty("_graphicTransition");
			_graphicProperty = serializedObject.FindProperty("_graphic");
		}

		public override void OnInspectorGUI()
		{
			OSCEditorInterface.LogoLayout();

			GUILayout.Label($"Value: {_valueProperty.boolValue}", EditorStyles.boldLabel);
			GUILayout.BeginVertical(OSCEditorStyles.Box);

			GUILayout.Label(_settingsContent, EditorStyles.boldLabel);
			GUILayout.BeginVertical(OSCEditorStyles.Box);
			base.OnInspectorGUI();
			GUILayout.EndVertical();

			serializedObject.Update();

			GUILayout.Label(_buttonSettingsContent, EditorStyles.boldLabel);
			GUILayout.BeginVertical(OSCEditorStyles.Box);
			EditorGUILayout.PropertyField(_graphicProperty);
			EditorGUILayout.PropertyField(_graphicTransitionProperty);
			EditorGUILayout.PropertyField(_buttonTypeProperty);
			GUILayout.EndVertical();

			if ((OSCButton.Type) _buttonTypeProperty.enumValueIndex == OSCButton.Type.Toggle)
			{
				GUILayout.Label(_valueSettingsContent, EditorStyles.boldLabel);
				GUILayout.BeginVertical(OSCEditorStyles.Box);
				EditorGUILayout.PropertyField(_valueProperty);
				GUILayout.EndVertical();
			}

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(_onValueChangedProperty);

			GUILayout.EndVertical();

			serializedObject.ApplyModifiedProperties();
		}

		#endregion
	}
}