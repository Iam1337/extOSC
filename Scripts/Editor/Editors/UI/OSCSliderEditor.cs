/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;

using extOSC.UI;

namespace extOSC.Editor.Components.UI
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(OSCSlider), true)]
	public class OSCSliderEditor : SelectableEditor
	{
		#region Static Private Vars

		private static readonly GUIContent _settingsContent = new GUIContent("Settings:");

		private static readonly GUIContent _sliderSettingsContent = new GUIContent("Slider Settings:");

		private static readonly GUIContent _valueSettingsContent = new GUIContent("Value Settings:");

		private static readonly GUIContent _resetValueContent = new GUIContent("Reset Value Settings:");

		#endregion

		#region Private Vars

		private SerializedProperty _directionProperty;

		private SerializedProperty _fillRecrProperty;

		private SerializedProperty _handleProperty;

		private SerializedProperty _minValueProperty;

		private SerializedProperty _maxValueProperty;

		private SerializedProperty _wholeNumbersProperty;

		private SerializedProperty _valueProperty;

		private SerializedProperty _onValueChangedProperty;

		private SerializedProperty _resetValueProperty;

		private SerializedProperty _resetValueTimeProperty;

		private SerializedProperty _callbackOnResetProperty;

		private SerializedProperty _multiplyControllerProperty;

		#endregion

		#region Unity Methods

		protected override void OnEnable()
		{
			base.OnEnable();

			_fillRecrProperty = serializedObject.FindProperty("m_FillRect");
			_handleProperty = serializedObject.FindProperty("m_HandleRect");
			_directionProperty = serializedObject.FindProperty("m_Direction");
			_minValueProperty = serializedObject.FindProperty("m_MinValue");
			_maxValueProperty = serializedObject.FindProperty("m_MaxValue");
			_wholeNumbersProperty = serializedObject.FindProperty("m_WholeNumbers");
			_valueProperty = serializedObject.FindProperty("m_Value");
			_onValueChangedProperty = serializedObject.FindProperty("m_OnValueChanged");
			_resetValueProperty = serializedObject.FindProperty("_resetValue");
			_resetValueTimeProperty = serializedObject.FindProperty("_resetValueTime");
			_callbackOnResetProperty = serializedObject.FindProperty("_callbackOnReset");
			_multiplyControllerProperty = serializedObject.FindProperty("_multiplyController");
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

			GUILayout.Label(_sliderSettingsContent, EditorStyles.boldLabel);
			GUILayout.BeginVertical(OSCEditorStyles.Box);
			EditorGUILayout.PropertyField(_fillRecrProperty);
			EditorGUILayout.PropertyField(_handleProperty);
			EditorGUILayout.PropertyField(_multiplyControllerProperty);

			if (_fillRecrProperty.objectReferenceValue != null || _handleProperty.objectReferenceValue != null)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(_directionProperty);
				if (EditorGUI.EndChangeCheck())
				{
					Slider.Direction direction = (Slider.Direction) _directionProperty.enumValueIndex;
					foreach (var targetObject in serializedObject.targetObjects)
					{
						var slider = targetObject as Slider;
						if (slider != null) slider.SetDirection(direction, true);
					}
				}

				GUILayout.EndVertical();

				GUILayout.Label(_valueSettingsContent, EditorStyles.boldLabel);
				GUILayout.BeginVertical(OSCEditorStyles.Box);
				EditorGUILayout.PropertyField(_minValueProperty);
				EditorGUILayout.PropertyField(_maxValueProperty);
				EditorGUILayout.PropertyField(_wholeNumbersProperty);
				EditorGUILayout.Slider(_valueProperty, _minValueProperty.floatValue, _maxValueProperty.floatValue);
				GUILayout.EndVertical();

				GUILayout.Label(_resetValueContent, EditorStyles.boldLabel);
				GUILayout.BeginVertical(OSCEditorStyles.Box);
				EditorGUILayout.PropertyField(_resetValueProperty);
				EditorGUILayout.PropertyField(_resetValueTimeProperty);
				EditorGUILayout.PropertyField(_callbackOnResetProperty);
				GUILayout.EndVertical();

				bool warning = false;
				foreach (var targetObject in serializedObject.targetObjects)
				{
					if (targetObject is Slider slider)
					{
						var direction = slider.direction;
						if (direction == Slider.Direction.LeftToRight || direction == Slider.Direction.RightToLeft)
							warning = (slider.navigation.mode != Navigation.Mode.Automatic && (slider.FindSelectableOnLeft() != null || slider.FindSelectableOnRight() != null));
						else
							warning = (slider.navigation.mode != Navigation.Mode.Automatic && (slider.FindSelectableOnDown() != null || slider.FindSelectableOnUp() != null));
					}
				}

				if (warning)
					EditorGUILayout.HelpBox("The selected slider direction conflicts with navigation. Not all navigation options may work.", MessageType.Warning);

				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(_onValueChangedProperty);
			}
			else
			{
				GUILayout.EndVertical();
				EditorGUILayout.HelpBox("Specify a RectTransform for the slider fill or the slider handle or both. Each must have a parent RectTransform that it can slide within.", MessageType.Info);
			}

			GUILayout.EndVertical();

			serializedObject.ApplyModifiedProperties();
		}

		#endregion
	}
}