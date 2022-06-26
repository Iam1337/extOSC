/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

using extOSC.UI;

namespace extOSC.Editor.Components.UI
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(OSCPad), true)]
	public class OSCPadEditor : SelectableEditor
	{
		#region Static Private Vars

		private static readonly GUIContent _maxContent = new GUIContent("Max");

		private static readonly GUIContent _minContent = new GUIContent("Min");

		private static readonly GUIContent _settingsContent = new GUIContent("Settings:");

		private static readonly GUIContent _padSettingsContent = new GUIContent("Pad Settings:");

		private static readonly GUIContent _valueSettingsContent = new GUIContent("Value Settings:");

		private static readonly GUIContent _resetValueContent = new GUIContent("Reset Value Settings:");

		private static readonly GUIContent _valueContent = new GUIContent("Value");

		private static readonly GUIContent _xAxisContent = new GUIContent("X Axis");

		private static readonly GUIContent _yAxisContent = new GUIContent("Y Axis");

		#endregion

		#region Private Vars

		private SerializedProperty _handleRectProperty;

		private SerializedProperty _minValueProperty;

		private SerializedProperty _maxValueProperty;

		private SerializedProperty _wholeNumbersProperty;

		private SerializedProperty _valueProperty;

		private SerializedProperty _onValueChangedProperty;

		private SerializedProperty _handleAlignmentProperty;

		private SerializedProperty _xAxisRectProperty;

		private SerializedProperty _yAxisRectProperty;

		private SerializedProperty _resetValueProperty;

		private SerializedProperty _resetValueTimeProperty;

		private SerializedProperty _callbackOnResetProperty;

		#endregion

		#region Unity Methods

		protected override void OnEnable()
		{
			base.OnEnable();

			_handleRectProperty = serializedObject.FindProperty("_handleRect");
			_minValueProperty = serializedObject.FindProperty("_minValue");
			_maxValueProperty = serializedObject.FindProperty("_maxValue");
			_wholeNumbersProperty = serializedObject.FindProperty("_wholeNumbers");
			_valueProperty = serializedObject.FindProperty("_value");
			_onValueChangedProperty = serializedObject.FindProperty("_onValueChanged");
			_handleAlignmentProperty = serializedObject.FindProperty("_handleAlignment");
			_xAxisRectProperty = serializedObject.FindProperty("_xAxisRect");
			_yAxisRectProperty = serializedObject.FindProperty("_yAxisRect");
			_resetValueProperty = serializedObject.FindProperty("_resetValue");
			_resetValueTimeProperty = serializedObject.FindProperty("_resetValueTime");
			_callbackOnResetProperty = serializedObject.FindProperty("_callbackOnReset");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			OSCEditorInterface.LogoLayout();

			GUILayout.Label($"Value: {_valueProperty.vector2Value}", EditorStyles.boldLabel);
			GUILayout.BeginVertical(OSCEditorStyles.Box);

			GUILayout.Label(_settingsContent, EditorStyles.boldLabel);
			GUILayout.BeginVertical(OSCEditorStyles.Box);
			base.OnInspectorGUI();
			GUILayout.EndVertical();

			GUILayout.Label(_padSettingsContent, EditorStyles.boldLabel);
			GUILayout.BeginVertical(OSCEditorStyles.Box);
			EditorGUILayout.PropertyField(_handleRectProperty);

			if (_handleRectProperty.objectReferenceValue != null)
			{
				EditorGUILayout.PropertyField(_handleAlignmentProperty);
				GUILayout.EndVertical();

				var minX = _minValueProperty.FindPropertyRelative("x");
				var maxX = _maxValueProperty.FindPropertyRelative("x");
				var minY = _minValueProperty.FindPropertyRelative("y");
				var maxY = _maxValueProperty.FindPropertyRelative("y");

				GUILayout.Label(_valueSettingsContent, EditorStyles.boldLabel);
				GUILayout.BeginVertical(OSCEditorStyles.Box);
				EditorGUILayout.PropertyField(_xAxisRectProperty);
				EditorGUILayout.PropertyField(_yAxisRectProperty);

				EditorGUILayout.Space();

				EditorGUILayout.LabelField(_xAxisContent);
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(minX, _minContent);
				EditorGUILayout.PropertyField(maxX, _maxContent);
				EditorGUI.indentLevel--;

				EditorGUILayout.Space();

				EditorGUILayout.LabelField(_yAxisContent);
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(minY, _minContent);
				EditorGUILayout.PropertyField(maxY, _maxContent);
				EditorGUI.indentLevel--;

				EditorGUILayout.Space();

				EditorGUILayout.PropertyField(_wholeNumbersProperty);

				EditorGUILayout.Space();

				EditorGUILayout.LabelField(_valueContent);
				EditorGUI.indentLevel++;
				EditorGUILayout.Slider(_valueProperty.FindPropertyRelative("x"), minX.floatValue, maxX.floatValue);
				EditorGUILayout.Slider(_valueProperty.FindPropertyRelative("y"), minY.floatValue, maxY.floatValue);
				EditorGUI.indentLevel--;

				GUILayout.EndVertical();

				GUILayout.Label(_resetValueContent, EditorStyles.boldLabel);
				GUILayout.BeginVertical(OSCEditorStyles.Box);
				EditorGUILayout.PropertyField(_resetValueProperty);
				EditorGUILayout.PropertyField(_resetValueTimeProperty);
				EditorGUILayout.PropertyField(_callbackOnResetProperty);
				GUILayout.EndVertical();

				EditorGUILayout.Space();

				EditorGUILayout.PropertyField(_onValueChangedProperty);
			}
			else
			{
				GUILayout.EndVertical();
				EditorGUILayout.HelpBox("You need to set \"RectTransform Group\" for correct work of the component.", MessageType.Info);
			}

			GUILayout.EndVertical();
			serializedObject.ApplyModifiedProperties();
		}

		#endregion
	}
}