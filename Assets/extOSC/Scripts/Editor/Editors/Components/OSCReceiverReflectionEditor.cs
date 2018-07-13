/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using System;
using System.Collections.Generic;

using extOSC.Components.ReceiverReflections;
using extOSC.Core.Reflection;

namespace extOSC.Editor.Components
{
    [CustomEditor(typeof(OSCReceiverReflection), true)]
    public class OSCReceiverReflectionEditor : OSCReceiverComponentEditor
    {
        #region Static Private Vars

        private static readonly GUIContent _targetTitleContent = new GUIContent("Target:");

        private static readonly GUIContent _targetContent = new GUIContent("Target:");

        private static readonly GUIContent _propertyContent = new GUIContent("Property:");

        private static readonly GUIContent _reflectionReceiverContent = new GUIContent("Receiver Reflection Settings:");

        private static readonly GUIContent[] _propertyPopupOptions = new GUIContent[] { new GUIContent("- None -") };

        #endregion

        #region Private Vars

        private OSCReceiverReflection _receiverReflection;

        private SerializedProperty _reflectionMembersProperty;

        private ReorderableList _reflectionMembersList;

        #endregion

        #region Unity Methods

        protected override void OnEnable()
        {
            base.OnEnable();

            _receiverReflection = target as OSCReceiverReflection;

            _targetTitleContent.text = string.Format("Target ({0}):", _receiverReflection.ReceiverType.Name);

            _reflectionMembersProperty = serializedObject.FindProperty("reflectionMembers");

            _reflectionMembersList = new ReorderableList(serializedObject, _reflectionMembersProperty);
            _reflectionMembersList.drawElementCallback += DrawElementCallback;
            _reflectionMembersList.elementHeight = EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 6;
            _reflectionMembersList.onAddCallback += OnAddCallback;
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

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocus)
        {
            rect.y += EditorGUIUtility.standardVerticalSpacing / 2;
            rect.height -= EditorGUIUtility.standardVerticalSpacing * 2;

            GUI.Box(rect, GUIContent.none);

            rect.y += EditorGUIUtility.standardVerticalSpacing * 2;
            rect.x += EditorGUIUtility.standardVerticalSpacing * 2;
            rect.height -= EditorGUIUtility.standardVerticalSpacing * 2;
            rect.width -= EditorGUIUtility.standardVerticalSpacing * 4;

            ReflectionMember(rect, _reflectionMembersProperty.GetArrayElementAtIndex(index), _receiverReflection.ReceiverType, OSCReflectionAccess.Write);
        }

        private void OnAddCallback(ReorderableList list)
        {
            _reflectionMembersProperty.InsertArrayElementAtIndex(_reflectionMembersProperty.arraySize);

            var reflectionMember = _reflectionMembersProperty.GetArrayElementAtIndex(_reflectionMembersProperty.arraySize - 1);
            reflectionMember.FindPropertyRelative("Target").objectReferenceValue = null;
            reflectionMember.FindPropertyRelative("MemberName").stringValue = string.Empty;
        }

        private void DrawHeaderCallback(Rect rect)
        {
            GUI.Label(rect, _targetTitleContent);
        }

        private static string PropertiesPopup(Rect rect, object target, string memberName, Type propertyType, GUIContent content, OSCReflectionAccess access)
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

        private static void ReflectionMember(Rect rect, SerializedProperty reflectionMemberProperty, Type reflectionType, OSCReflectionAccess access)
        {
            var targetProperty = reflectionMemberProperty.FindPropertyRelative("Target");
            var memberNameProperty = reflectionMemberProperty.FindPropertyRelative("MemberName");

            var firstLine = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            var secondLine = new Rect(rect.x, rect.y + rect.height / 2f, rect.width, EditorGUIUtility.singleLineHeight);


            EditorGUI.PropertyField(firstLine, targetProperty, _targetContent);

            if (targetProperty.objectReferenceValue != null)
            {
                GUI.enabled = memberNameProperty.stringValue != "- None -";

                memberNameProperty.stringValue = PropertiesPopup(secondLine, 
                                                                 targetProperty.objectReferenceValue,
                                                                 memberNameProperty.stringValue,
                                                                 reflectionType,
                                                                 _propertyContent,
                                                                 access);
            }
            else
            {
                GUI.enabled = false;

                EditorGUI.Popup(secondLine, _propertyContent, 0, _propertyPopupOptions);
            }

            GUI.enabled = true;
        }

        #endregion
    }
}