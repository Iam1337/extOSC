/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEditor;

using extOSC.Core.Reflection;
using UnityEngine;

namespace Assets.extOSC.Scripts.Editor.Properties
{
    [CustomPropertyDrawer(typeof(OSCReflectionMember))]
    public class OSCReflectionMemberProperty : PropertyDrawer
    {
        #region Static Public Methods

        private static GUIContent _targetContent = new GUIContent("Target:");

        #endregion

        #region Public Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var verticalSpacing = EditorGUIUtility.standardVerticalSpacing; // Default Vertical Space
            var defaultHeight = EditorGUIUtility.singleLineHeight; // Default Property Height

            return defaultHeight * 2 + verticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get properties.
            var targetProperty = property.FindPropertyRelative("Target");
            var memberProperty = property.FindPropertyRelative("MemberName");

            // Get default values.
            var verticalSpacing = EditorGUIUtility.standardVerticalSpacing; // Default Vertical Space
            var defaultHeight = EditorGUIUtility.singleLineHeight; // Default Property Height

            if (string.IsNullOrEmpty(label.text))
                label = _targetContent;

            // Get positions.
            var fieldSize = new Vector2(position.height, defaultHeight);
            var targetPosition = new Rect(position.position, fieldSize);
            var memberPosition = new Rect();

            
            // Draw properties.
            EditorGUI.PropertyField(targetPosition, targetProperty);


            //EditorGUILayout.PropertyField(targetProperty, _targetContent);
            //base.OnGUI(position, property, label);
        }

        #endregion
    }
}
