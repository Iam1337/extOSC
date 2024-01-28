/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

using extOSC.UI;

namespace extOSC.Editor.Components.UI
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(OSCRotary), true)]
	public class OSCRotaryEditor : SelectableEditor
	{
		#region Static Private Vars

		private static readonly GUIContent _settingsContent = new GUIContent("Settings:");

		private static readonly GUIContent _rotartySettingsContent = new GUIContent("Rotary Settings:");

		private static readonly GUIContent _valueSettingsContent = new GUIContent("Value Settings:");

		private static readonly GUIContent _resetValueContent = new GUIContent("Reset Value Settings:");

		#endregion

		#region Private Vars

		private SerializedProperty _handleImageProperty;

		private SerializedProperty _minValueProperty;

		private SerializedProperty _maxValueProperty;

		private SerializedProperty _wholeNumbersProperty;

		private SerializedProperty _valueProperty;

		private SerializedProperty _onValueChangedProperty;

		private SerializedProperty _reverseProperty;

		private SerializedProperty _resetValueProperty;

		private SerializedProperty _resetValueTimeProperty;

		private SerializedProperty _callbackOnResetProperty;

		#endregion

		#region Unity Methods

		protected override void OnEnable()
		{
			base.OnEnable();

			_handleImageProperty = serializedObject.FindProperty("_handleImage");
			_minValueProperty = serializedObject.FindProperty("_minValue");
			_maxValueProperty = serializedObject.FindProperty("_maxValue");
			_wholeNumbersProperty = serializedObject.FindProperty("_wholeNumbers");
			_valueProperty = serializedObject.FindProperty("_value");
			_onValueChangedProperty = serializedObject.FindProperty("_onValueChanged");
			_resetValueProperty = serializedObject.FindProperty("_resetValue");
			_resetValueTimeProperty = serializedObject.FindProperty("_resetValueTime");
			_callbackOnResetProperty = serializedObject.FindProperty("_callbackOnReset");
			_reverseProperty = serializedObject.FindProperty("_reverse");

		}

		public override void OnInspectorGUI()
		{
			OSCEditorInterface.LogoLayout();

			GUILayout.Label($"Value: {_valueProperty.floatValue}", EditorStyles.boldLabel);
			GUILayout.BeginVertical(OSCEditorStyles.Box);

			GUILayout.Label(_settingsContent, EditorStyles.boldLabel);
			GUILayout.BeginVertical(OSCEditorStyles.Box);
			base.OnInspectorGUI();
			GUILayout.EndVertical();

			serializedObject.Update();

			GUILayout.Label(_rotartySettingsContent, EditorStyles.boldLabel);
			GUILayout.BeginVertical(OSCEditorStyles.Box);
			EditorGUILayout.PropertyField(_handleImageProperty);
			EditorGUILayout.PropertyField(_reverseProperty);

			GUILayout.EndVertical();

			GUILayout.Label(_valueSettingsContent, EditorStyles.boldLabel);
			GUILayout.BeginVertical(OSCEditorStyles.Box);

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(_minValueProperty);
			EditorGUILayout.PropertyField(_maxValueProperty);

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(_wholeNumbersProperty);

			EditorGUILayout.Space();

			EditorGUILayout.Slider(_valueProperty, _minValueProperty.floatValue, _maxValueProperty.floatValue);

			GUILayout.EndVertical();

			GUILayout.Label(_resetValueContent, EditorStyles.boldLabel);
			GUILayout.BeginVertical(OSCEditorStyles.Box);
			EditorGUILayout.PropertyField(_resetValueProperty);
			EditorGUILayout.PropertyField(_resetValueTimeProperty);
			EditorGUILayout.PropertyField(_callbackOnResetProperty);
			GUILayout.EndVertical();

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(_onValueChangedProperty);

			GUILayout.EndVertical();
			serializedObject.ApplyModifiedProperties();
		}

		#endregion
	}
}