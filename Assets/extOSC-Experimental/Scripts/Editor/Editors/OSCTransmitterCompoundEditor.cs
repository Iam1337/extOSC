/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using System;
using System.Collections.Generic;
using extOSC.Components;
using extOSC.Components.Compounds;
using extOSC.Core.Reflection;

namespace extOSC.Editor.Components
{
	[CustomEditor(typeof(OSCTransmitterCompound))]
	public class OSCTransmitterCompoundEditor : OSCTransmitterComponentEditor
	{
		#region Static Private Vars

		//private static readonly GUIContent _targetTitleContent = new GUIContent("Target:");

		private static readonly GUIContent _settingsTitleContent = new GUIContent("Compound Settings:");

		//private static readonly GUIContent _sendOnChangedContent = new GUIContent("Send on changed");

		private static readonly GUIContent _sendIntervalContent = new GUIContent("Send interval:");

		private static readonly GUIContent _elementsContent = new GUIContent("Elements:");

		private static readonly GUIContent _addValueContent = new GUIContent("Add Value");

		private static readonly GUIContent _emptyContent = new GUIContent("- None -");

		private static readonly GUIContent _valueTypeContent = new GUIContent("Value Type:");

		private static readonly GUIContent _valueSourceContent = new GUIContent("Value Source:");

		private static readonly GUIContent _targetContent = new GUIContent("Target:");

		private static readonly GUIContent[] _propertyPopupOptions = new GUIContent[] { new GUIContent("- None -") };

		#endregion

		#region Private Vars

		private OSCTransmitterCompound _compound;

		private SerializedProperty _elementsProperty;

		private SerializedProperty _sendOnChangedProperty;

		private SerializedProperty _sendIntervalProperty;

		private ReorderableList _elementsList;

		private Color _defaultColor;

		#endregion

		#region Unity Methods

		protected override void OnEnable()
		{
			base.OnEnable();

			_compound = target as OSCTransmitterCompound;

			//_targetTitleContent.text = string.Format("Target ({0}):", _informer.InformerType.Name);

			_elementsProperty = serializedObject.FindProperty("elements");
			//_sendOnChangedProperty = serializedObject.FindProperty("sendOnChanged");
			_sendIntervalProperty = serializedObject.FindProperty("sendInterval");

			_elementsList = new ReorderableList(serializedObject, _elementsProperty);
			_elementsList.drawElementCallback += DrawElementCallback;
			_elementsList.elementHeight = EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 6;
			_elementsList.onAddCallback += OnAddCallback;
			_elementsList.drawHeaderCallback += DrawHeaderCallback;
		}

		#endregion

		#region Protected Methods

		protected override void DrawSettings()
		{
			_defaultColor = GUI.color;

			// SETTINGS
			EditorGUILayout.LabelField(_settingsTitleContent, EditorStyles.boldLabel);
			GUILayout.BeginVertical("box");

			/*
			GUI.color = _sendOnChangedProperty.boolValue ? Color.green : Color.red;
			if (GUILayout.Button(_sendOnChangedContent))
			{
				_sendOnChangedProperty.boolValue = !_sendOnChangedProperty.boolValue;
			}
			GUI.color = _defaultColor;

			if (!_sendOnChangedProperty.boolValue)
			{
			*/
			EditorGUILayout.PropertyField(_sendIntervalProperty, _sendIntervalContent);

			if (_sendIntervalProperty.floatValue < 0) _sendIntervalProperty.floatValue = 0;

			EditorGUILayout.HelpBox("Set to 0 for send message with each frame.", MessageType.Info);

			GUILayout.EndVertical();

			EditorGUILayout.LabelField(_elementsContent, EditorStyles.boldLabel);
			_elementsList.DoLayoutList();
		}

		#endregion

		#region Private Methods

		private void DrawHeaderCallback(Rect rect)
		{
			var tags = string.Empty;

			var elements = _compound.GetElements();
			foreach (var element in elements)
			{
				tags += OSCValue.GetTag(element.ValueType);
			}

			GUI.Label(rect, "Elements: " + tags);
		}

		private void OnAddCallback(ReorderableList list)
		{
			_elementsProperty.InsertArrayElementAtIndex(_elementsProperty.arraySize);

			var element = _elementsProperty.GetArrayElementAtIndex(_elementsProperty.arraySize - 1);
			element.FindPropertyRelative("valueType").enumValueIndex = (int) OSCValueType.Unknown;

			var reflectionMember = element.FindPropertyRelative("valueSource");
			reflectionMember.FindPropertyRelative("Target").objectReferenceValue = null;
			reflectionMember.FindPropertyRelative("MemberName").stringValue = string.Empty;
		}

		private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
		{
			var elementProperty = _elementsProperty.GetArrayElementAtIndex(index);
			var valueTypeProperty = elementProperty.FindPropertyRelative("valueType");
			var valueSourceProperty = elementProperty.FindPropertyRelative("valueSource");

			var valueType = (OSCValueType)valueTypeProperty.enumValueIndex;
			var type = OSCValue.GetType(valueType);

			rect.y += EditorGUIUtility.standardVerticalSpacing / 2;
			rect.height -= EditorGUIUtility.standardVerticalSpacing * 2;

			GUI.Box(rect, GUIContent.none);

			var splitterRect = new Rect(rect);
			splitterRect.height = 1f;
			splitterRect.y += EditorGUIUtility.standardVerticalSpacing * 4 + EditorGUIUtility.singleLineHeight;
			GUI.DrawTexture(splitterRect, OSCEditorTextures.Splitter);

			rect.y += EditorGUIUtility.standardVerticalSpacing * 2;
			rect.x += EditorGUIUtility.standardVerticalSpacing * 2;
			rect.height -= EditorGUIUtility.standardVerticalSpacing * 2;
			rect.width -= EditorGUIUtility.standardVerticalSpacing * 5;

			var typeRect = new Rect(rect);
			typeRect.height = EditorGUIUtility.singleLineHeight;

			EditorGUI.PropertyField(typeRect, valueTypeProperty, _valueTypeContent);

			rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 4;
			rect.height -= EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 6;

			if (valueType == OSCValueType.Unknown)
			{
				EditorGUI.HelpBox(rect, "Select a value type.", MessageType.Warning);
			}
			else if (valueType == OSCValueType.Array)
			{
				EditorGUI.HelpBox(rect, "Array type is not support.", MessageType.Warning);
			}
			else if (valueType == OSCValueType.Impulse || valueType == OSCValueType.Null)
			{
				EditorGUI.HelpBox(rect, "ValueSource is not required.", MessageType.Info);
			}
			else
			{
				rect.y += EditorGUIUtility.standardVerticalSpacing * 2;
				rect.height -= EditorGUIUtility.standardVerticalSpacing * 2;
				ReflectionMember(rect, valueSourceProperty, type, OSCReflectionAccess.Read);
			}
		}


		// Maybe move to another class?
		private string PropertiesPopup(Rect rect, object target, string memberName, Type propertyType, GUIContent content, OSCReflectionAccess access)
		{
			var members = OSCReflection.GetMembersByType(target, propertyType, access, OSCReflectionType.All);
			var clearName = new List<GUIContent>();

			var currentIndex = 0;

			// GET INDEX
			foreach (var member in members)
			{
				if (member.Name == memberName)
					currentIndex = clearName.Count;

				clearName.Add(new GUIContent(OSCEditorUtils.MemberName(member)));
			}

			if (clearName.Count == 0)
				clearName.Add(new GUIContent("- None -"));

			currentIndex = EditorGUI.Popup(rect, content, currentIndex, clearName.ToArray());
			currentIndex = Mathf.Clamp(currentIndex, 0, clearName.Count - 1);

			return members.Length > 0 ? members[currentIndex].Name : "- None -";
		}

		private void ReflectionMember(Rect rect, SerializedProperty reflectionMemberProperty, Type reflectionType, OSCReflectionAccess access)
		{
			var targetProperty = reflectionMemberProperty.FindPropertyRelative("Target");
			var memberNameProperty = reflectionMemberProperty.FindPropertyRelative("MemberName");

			var firstLine = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
			var secondLine = new Rect(rect.x, rect.y + rect.height / 2f, rect.width, EditorGUIUtility.singleLineHeight);

			EditorGUI.PropertyField(firstLine, targetProperty, _targetContent);

			var propertyContent = new GUIContent("Property (" + reflectionType.Name + "):");

			if (targetProperty.objectReferenceValue != null)
			{
				var cacheValue = memberNameProperty.stringValue;
				GUI.enabled = memberNameProperty.stringValue != "- None -";

				memberNameProperty.stringValue = PropertiesPopup(secondLine,
																 targetProperty.objectReferenceValue,
																 cacheValue,
																 reflectionType,
																 propertyContent,
																 access);

				if (cacheValue != memberNameProperty.stringValue)
					serializedObject.ApplyModifiedProperties();
			}
			else
			{
				GUI.enabled = false;

				EditorGUI.Popup(secondLine, propertyContent, 0, _propertyPopupOptions);
			}

			GUI.enabled = true;
		}

		#endregion
	}
}