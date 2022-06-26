/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using System;
using System.Collections.Generic;

using extOSC.Components.ReceiverReflections;
using extOSC.Core.Reflection;
using extOSC.Editor.Drawers;

namespace extOSC.Editor.Components
{
	[CustomEditor(typeof(OSCReceiverReflection), true)]
	public class OSCReceiverReflectionEditor : OSCReceiverComponentEditor
	{
		#region Static Private Vars

		private static readonly GUIContent _targetTitleContent = new GUIContent("Target:");

		private static readonly GUIContent _reflectionReceiverContent = new GUIContent("Receiver Reflection Settings:");

		#endregion

		#region Private Vars

		private readonly List<OSCReflectionMemberDrawer> _reflectionDrawers = new List<OSCReflectionMemberDrawer>();

		private OSCReceiverReflection _receiverReflection;

		private SerializedProperty _reflectionMembersProperty;

		private ReorderableList _reflectionMembersList;

		#endregion

		#region Unity Methods

		protected override void OnEnable()
		{
			base.OnEnable();

			_receiverReflection = target as OSCReceiverReflection;
			_targetTitleContent.text = $"Target ({_receiverReflection.ReceiverType.Name}):";
			_reflectionMembersProperty = serializedObject.FindProperty("_reflectionMembers");

			// Setup list.
			_reflectionMembersList = new ReorderableList(serializedObject, _reflectionMembersProperty);
			_reflectionMembersList.drawElementCallback += DrawElementCallback;
			_reflectionMembersList.elementHeight = EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 6;
			_reflectionMembersList.onAddCallback += AddCallback;
			_reflectionMembersList.onRemoveCallback += RemoveCallback;
			_reflectionMembersList.drawHeaderCallback += DrawHeaderCallback;
		}

		#endregion

		#region Protected Methods

		protected override void DrawSettings()
		{
			// MEMBERS LIST
			EditorGUILayout.LabelField(_reflectionReceiverContent, EditorStyles.boldLabel);

			_reflectionMembersList.DoLayoutList();
		}

		#endregion

		#region Private Methods

		private void AddCallback(ReorderableList list)
		{
			_reflectionMembersProperty.InsertArrayElementAtIndex(_reflectionMembersProperty.arraySize);

			var reflectionMember = _reflectionMembersProperty.GetArrayElementAtIndex(_reflectionMembersProperty.arraySize - 1);
			reflectionMember.FindPropertyRelative("Target").objectReferenceValue = null;
			reflectionMember.FindPropertyRelative("MemberName").stringValue = string.Empty;
		}

		private void RemoveCallback(ReorderableList list)
		{
			RemoveDrawer();
		}

		private void DrawElementCallback(Rect position, int index, bool isActive, bool isFocus)
		{
			var space = EditorGUIUtility.standardVerticalSpacing;

			// Decorate elements.
			position.y += space * 0.5f;
			position.height -= space * 2;

			GUI.Box(position, GUIContent.none);

			position.y += space * 2;
			position.x += space * 2;
			position.height -= space * 2;
			position.width -= space * 4;

			// Get and setup property drawer.
			var drawer = CreateDrawer(index);
			drawer.SerializedProperty = _reflectionMembersProperty.GetArrayElementAtIndex(index);

			// Draw
			drawer.Draw(position);
		}

		private void DrawHeaderCallback(Rect rect)
		{
			GUI.Label(rect, _targetTitleContent);
		}

		private OSCReflectionMemberDrawer CreateDrawer(int index)
		{
			var drawer = (OSCReflectionMemberDrawer) null;

			if (_reflectionDrawers.Count <= index)
			{
				var property = _reflectionMembersProperty.GetArrayElementAtIndex(index);

				// Create drawer.
				drawer = new OSCReflectionMemberDrawer(property,
													   _receiverReflection.ReceiverType,
													   OSCReflectionAccess.Write,
													   OSCReflectionType.All);

				_reflectionDrawers.Add(drawer);
			}
			else
			{
				drawer = _reflectionDrawers[index];
			}

			return drawer;
		}

		private void RemoveDrawer()
		{
			if (_reflectionDrawers.Count > 0)
				_reflectionDrawers.RemoveAt(0);
		}

		#endregion
	}
}