/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEditor;
using UnityEngine;

using System;
using System.Reflection;

using extOSC.Core.Reflection;

namespace extOSC.Editor.Drawers
{
	public class OSCReflectionMemberDrawer
	{
		#region Static Public Methods

		private static GUIContent _memberNoneContent = new GUIContent("- None -");

		private static GUIContent[] _nullOptions = {_memberNoneContent};

		private static GUIContent _targetDefaultContent = new GUIContent("Target:");

		private static GUIContent _memberDefaultContent = new GUIContent("Member:");

		#endregion

		#region Public Methods

		public SerializedProperty SerializedProperty
		{
			get => _serializedProperty;
			set
			{
				if (_serializedProperty == value)
					return;

				_serializedProperty = value;

				// Find internal properties.
				_targetProperty = _serializedProperty.FindPropertyRelative("Target");
				_memberProperty = _serializedProperty.FindPropertyRelative("MemberName");

				// Recache members.
				RecacheMember();
			}
		}

		//public Type MemberType
		//{
		//	get => _memberType;
		//	set
		//	{
		//		if (_memberType == value)
		//			return;

		//		_memberType = value;

		//		// Recache all members.
		//		RecacheMember();
		//	}
		//}

		public OSCReflectionAccess ReflectionAccess
		{
			get => _reflectionAccess;
			set
			{
				if (_reflectionAccess == value)
					return;

				_reflectionAccess = value;

				// Recache all members.
				RecacheMember();
			}
		}

		public OSCReflectionType ReflectionType
		{
			get => _reflectionType;
			set
			{
				if (_reflectionType == value)
					return;

				_reflectionType = value;

				// Recache all members.
				RecacheMember();
			}
		}

		public GUIContent TargetContent;

		public GUIContent MemberContent;

		public Func<MemberInfo, string> OnMemberName;

		#endregion

		#region Private Methods

		private int _permanentId;

		private SerializedProperty _serializedProperty;

		private SerializedProperty _targetProperty;

		private SerializedProperty _memberProperty;

		private Type _memberType;

		private UnityEngine.Object _previousObject;

		private OSCReflectionAccess _reflectionAccess;

		private OSCReflectionType _reflectionType;

		private MemberInfo[] _members = new MemberInfo[0];

		private GUIContent[] _membersNames = new GUIContent[0];

		#endregion

		#region Public Methods

		public OSCReflectionMemberDrawer(SerializedProperty serializedProperty, Type memberType) :
			this(serializedProperty, memberType, OSCReflectionAccess.Any, OSCReflectionType.All)
		{ }

		public OSCReflectionMemberDrawer(SerializedProperty serializedProperty,
										 Type memberType,
										 OSCReflectionAccess reflectionAccess,
										 OSCReflectionType reflectionType)
		{
			// Setup.
			_serializedProperty = serializedProperty;
			_memberType = memberType;
			_reflectionAccess = reflectionAccess;
			_reflectionType = reflectionType;

			// Setup contents.
			MemberContent = _memberDefaultContent;
			TargetContent = _targetDefaultContent;

			// Find internal properties.
			_targetProperty = _serializedProperty.FindPropertyRelative("Target");
			_memberProperty = _serializedProperty.FindPropertyRelative("MemberName");
		}

		public void DrawLayout()
		{
			// Get positions.
			var height = EditorGUIUtility.singleLineHeight;
			var targetPosition = GUILayoutUtility.GetRect(0f, height, GUILayout.ExpandWidth(true));
			GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
			var memberPosition = GUILayoutUtility.GetRect(0f, height, GUILayout.ExpandWidth(true));

			// Draw elements.
			DrawTarget(targetPosition);
			DrawMember(memberPosition);
		}

		public void Draw(Rect position)
		{
			// Get positions.
			var height = EditorGUIUtility.singleLineHeight;
			var space = EditorGUIUtility.standardVerticalSpacing;
			var halfSpace = space * 0.5f;

			var targetPosition = new Rect(position.x,
										  position.y,
										  position.width,
										  height - halfSpace);

			var memberPosition = new Rect(position.x,
										  position.y + targetPosition.height + space,
										  position.width,
										  height - halfSpace);

			// Draw elements.
			DrawTarget(targetPosition);
			DrawMember(memberPosition);
		}

		#endregion

		#region Private Methods

		private void DrawTarget(Rect position)
		{
			EditorGUI.PropertyField(position, _targetProperty, TargetContent);
		}

		private void DrawMember(Rect position)
		{
			var targetObject = _targetProperty.objectReferenceValue;
			if (targetObject != _previousObject) RecacheMember(); // Recache members if target object changed.

			if (targetObject != null && _members.Length > 0)
			{
				// Draw normal members.
				// Get member index.
				var index = GetMemberIndex(_memberProperty.stringValue);
				index = EditorGUI.Popup(position, MemberContent, index, _membersNames);

				// Set member. (Set none if index zero)
				_memberProperty.stringValue = index > 0 ? _members[index - 1].Name : string.Empty;
			}
			else
			{
				// Draw fake popup.
				GUI.enabled = false;
				EditorGUI.Popup(position, MemberContent, 0, _nullOptions);
				GUI.enabled = true;

				// Clean member property.
				_memberProperty.stringValue = string.Empty;
			}

			// Save previous target.
			_previousObject = targetObject;
		}

		private void RecacheMember()
		{
			// Check is we have target.
			var targetObject = _targetProperty.objectReferenceValue;
			if (targetObject == null) return;

			_members = OSCReflection.GetMembersByType(targetObject, _memberType, _reflectionAccess, _reflectionType);

			// Stop method is we do not have any members.
			var membersLength = _members.Length;
			if (membersLength == 0) return;

			_membersNames = new GUIContent[membersLength + 1];
			_membersNames[0] = _memberNoneContent;

			// Creating member names.
			for (var i = 0; i < membersLength; i++)
			{
				var member = _members[i];
				var content = new GUIContent();
				content.text = OnMemberName != null ? OnMemberName.Invoke(member) : OSCEditorUtils.MemberName(member);

				_membersNames[i + 1] = content;
			}
		}

		private int GetMemberIndex(string currentName)
		{
			var membersLength = _members.Length;
			if (membersLength != 0)
			{
				for (var i = 0; i < membersLength; i++)
				{
					var memberName = _members[i].Name;
					if (memberName == currentName)
					{
						return i + 1;
					}
				}
			}

			return 0;
		}

		#endregion
	}
}