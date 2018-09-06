/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using System;
using System.Linq;
using System.Collections.Generic;

using extOSC.Core;
using extOSC.Core.Reflection;

namespace extOSC.Editor
{
    public static partial class OSCEditorLayout
    {
        #region Static Private Vars

        #endregion

        #region Static Public Methods

        public static OSCReceiver ReceiversPopup(OSCReceiver receiver, GUIContent content)
        {
            return OSCPopup(OSCEditorUtils.GetReceivers(), receiver, content);
        }

        public static void ReceiversPopup(SerializedProperty property, GUIContent content)
        {
            property.objectReferenceValue = OSCPopup(OSCEditorUtils.GetReceivers(),
                property.objectReferenceValue as OSCReceiver, content);
        }

        public static OSCTransmitter TransmittersPopup(OSCTransmitter transmitter, GUIContent content)
        {
            return OSCPopup(OSCEditorUtils.GetTransmitters(), transmitter, content);
        }

        public static void TransmittersPopup(SerializedProperty property, GUIContent content)
        {
            property.objectReferenceValue = OSCPopup(OSCEditorUtils.GetTransmitters(),
                property.objectReferenceValue as OSCTransmitter, content);
        }

        #endregion

        #region Static Private Methods

        private static string PropertiesPopup(object target, string memberName, Type propertyType, GUIContent content,
            OSCReflectionAccess access)
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

            currentIndex = EditorGUILayout.Popup(content, currentIndex, clearName.ToArray());
            currentIndex = Mathf.Clamp(currentIndex, 0, clearName.Count - 1);

            return members.Length > 0 ? members[currentIndex].Name : "- None -";
        }

        private static T OSCPopup<T>(Dictionary<string, T> dictionary, T osc, GUIContent content) where T : OSCBase
        {
            T[] objects = null;
            string[] names = null;

            FillOSCArrays(dictionary, out names, out objects);

            var currentIndex = 0;
            var currentReceiver = osc;

            for (var index = 0; index < objects.Length; index++)
            {
                if (objects[index] == currentReceiver)
                {
                    currentIndex = index;
                    break;
                }
            }

            if (content != null)
            {
                var contentNames = new GUIContent[names.Length];

                for (var index = 0; index < names.Length; index++)
                {
                    contentNames[index] = new GUIContent(names[index]);
                }

                currentIndex = EditorGUILayout.Popup(content, currentIndex, contentNames);
            }
            else
            {
                currentIndex = EditorGUILayout.Popup(currentIndex, names);
            }

            return objects[currentIndex];
        }

        private static void FillOSCArrays<T>(Dictionary<string, T> dictionary, out string[] names, out T[] objects)
            where T : OSCBase
        {
            var namesList = new List<string>();
            namesList.Add("- None -");
            namesList.AddRange(dictionary.Keys);

            var objectsList = new List<T>();
            objectsList.Add(null);
            objectsList.AddRange(dictionary.Values);

            names = namesList.ToArray();
            objects = objectsList.ToArray();
        }

        #endregion
    }
}