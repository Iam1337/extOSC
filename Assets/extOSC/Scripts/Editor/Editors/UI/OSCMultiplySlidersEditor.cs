/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

using extOSC.UI;

namespace extOSC.Editor.Components.UI
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(OSCMultiplySliders), true)]
	public class OSCMultiplySlidersEditor : SelectableEditor
	{
		#region Private Vars

		private SerializedProperty _minValueProperty;

		private SerializedProperty _maxValueProperty;

		private SerializedProperty _wholeNumbersProperty;
		
		private SerializedProperty _addressProperty;

		private SerializedProperty _transmitterProperty;

		private SerializedProperty _layoutGroupProperty;

		private SerializedProperty _slidersProperty;

		private SerializedProperty _defaultColorProperty;

		#endregion

		#region Unity Methods

		protected override void OnEnable()
		{
			base.OnEnable();

			_addressProperty = serializedObject.FindProperty("_address");
			_transmitterProperty = serializedObject.FindProperty("_transmitter");
			_layoutGroupProperty = serializedObject.FindProperty("_layoutGroup");
			_slidersProperty = serializedObject.FindProperty("_sliders");
			_defaultColorProperty = serializedObject.FindProperty("_defaultColor");
			_minValueProperty = serializedObject.FindProperty("_minValue");
			_maxValueProperty = serializedObject.FindProperty("_maxValue");
			_wholeNumbersProperty = serializedObject.FindProperty("_wholeNumbers");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			OSCEditorInterface.LogoLayout();

			GUILayout.Label($"Sliders: {_slidersProperty.arraySize}", EditorStyles.boldLabel);
			GUILayout.BeginVertical(OSCEditorStyles.Box);

			GUILayout.Label("Settings:", EditorStyles.boldLabel);
			GUILayout.BeginVertical(OSCEditorStyles.Box);
			base.OnInspectorGUI();
			GUILayout.EndVertical();

			GUILayout.Label("Multiply Sliders Settings:", EditorStyles.boldLabel);
			GUILayout.BeginVertical(OSCEditorStyles.Box);
			EditorGUILayout.PropertyField(_layoutGroupProperty);

			if (_layoutGroupProperty.objectReferenceValue != null)
			{
				GUILayout.EndVertical();

				GUILayout.Label("Sliders Settings:", EditorStyles.boldLabel);
				GUILayout.BeginVertical(OSCEditorStyles.Box);
				EditorGUILayout.PropertyField(_defaultColorProperty);
				EditorGUILayout.PropertyField(_addressProperty);

				//OSCEditorLayout.TransmittersPopup(_transmitterProperty, new GUIContent("Transmitter"));
				EditorGUILayout.PropertyField(_transmitterProperty, new GUIContent("Transmitter"));
				GUILayout.EndVertical();

				GUILayout.BeginHorizontal(OSCEditorStyles.Box);
				GUILayout.BeginVertical(OSCEditorStyles.Box);
				GUI.color = Color.red;
				var removeButton = GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20));
				GUI.color = Color.white;
				GUILayout.EndVertical();
				GUILayout.BeginVertical(OSCEditorStyles.Box);
				EditorGUILayout.LabelField(_slidersProperty.arraySize.ToString(), OSCEditorStyles.CenterLabel, GUILayout.Height(20));
				GUILayout.EndVertical();
				GUILayout.BeginVertical(OSCEditorStyles.Box);
				GUI.color = Color.green;
				var createButton = GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20));
				GUI.color = Color.white;
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();

				GUILayout.Label("Value Settings:", EditorStyles.boldLabel);
				GUILayout.BeginVertical(OSCEditorStyles.Box);
				EditorGUILayout.PropertyField(_minValueProperty);
				EditorGUILayout.PropertyField(_maxValueProperty);
				EditorGUILayout.PropertyField(_wholeNumbersProperty);
				GUILayout.EndVertical();

				GUILayout.EndVertical();

				if (createButton) AddSlider();
				if (removeButton) RemoveSlider();
			}
			else
			{
				EditorGUILayout.HelpBox("You need to set \"HorizontalOrVerticalLayoutGroup Group\" for correct work of the component.", MessageType.Warning);
				GUILayout.EndVertical();
			}

			serializedObject.ApplyModifiedProperties();
		}

		#endregion

		#region Private Methods

		private void AddSlider()
		{
			var multiplySliders = (OSCMultiplySliders) target;
			var layoutTransform = ((Component) _layoutGroupProperty.objectReferenceValue).transform;

			var resources = OSCEditorUtils.GetStandardResources();
			resources.Color = multiplySliders.DefaultColor;

			var index = _slidersProperty.arraySize;

			var sliderObject = OSCControls.CreateSlider(resources);
			sliderObject.name = $"Slider: {index}";

			var slider = sliderObject.GetComponent<OSCSlider>();
			slider.MultiplyController = multiplySliders;

			var sliderRect = sliderObject.GetComponent<RectTransform>();
			sliderRect.SetParent(layoutTransform);
			sliderRect.localScale = Vector3.one;
			sliderRect.localPosition = Vector3.zero;

			_slidersProperty.InsertArrayElementAtIndex(index);

			var element = _slidersProperty.GetArrayElementAtIndex(index);
			element.objectReferenceValue = sliderObject;
		}

		private void RemoveSlider()
		{
			var index = _slidersProperty.arraySize - 1;
			if (index < 0) return;

			var slider = (OSCSlider) _slidersProperty.GetArrayElementAtIndex(index).objectReferenceValue;
			if (slider != null)
			{
				var sliderObject = slider.gameObject;

				DestroyImmediate(sliderObject);

				_slidersProperty.GetArrayElementAtIndex(index).objectReferenceValue = null;
			}

			_slidersProperty.DeleteArrayElementAtIndex(index);
		}

		#endregion
	}
}