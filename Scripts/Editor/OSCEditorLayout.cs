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

        private static Dictionary<object, OSCValueType> _valueTypeTemp = new Dictionary<object, OSCValueType>();

        private static readonly GUIContent _transmitterAddressContent = new GUIContent("OSC Transmitter Address:");

        private static readonly GUIContent _transmitterAddressContentSmall = new GUIContent("Transmitter Address:");

        private static readonly GUIContent _receiverAddressContent = new GUIContent("OSC Receiver Address:");

        private static readonly GUIContent _receiverAddressContentSmall = new GUIContent("Receiver Address:");

        private static readonly GUIContent _transmitterContent = new GUIContent("OSC Transmitter:");

        private static readonly GUIContent _receiverContent = new GUIContent("OSC Receiver:");

        private static readonly GUIContent _bundleEmptyContent = new GUIContent("Bundle is empty!");

        private static readonly GUIContent _bundleContent = new GUIContent("Bundle:");

        private static readonly GUIContent _addressContent = new GUIContent("Address:");

        private static readonly GUIContent _addBundleContent = new GUIContent("Add Bundle");

        private static readonly GUIContent _addMessageContent = new GUIContent("Add Message");

        private static readonly GUIContent _addValueContent = new GUIContent("Add Value");

        private static readonly GUIContent _arrayContent = new GUIContent("Array");

        private static readonly GUIContent _addToArrayContent = new GUIContent("Array:");

        private static readonly GUIContent _beginArrayContent = new GUIContent("Begin Array");

        private static readonly GUIContent _endArrayContent = new GUIContent("End Array");

        private static readonly GUIContent _targetContent = new GUIContent("Target:");

        private static readonly GUIContent _propertyContent = new GUIContent("Property:");

        private static readonly GUIContent _emptyContent = new GUIContent("- Empty -");

        private static readonly GUIContent[] _propertyPopupOptions = new GUIContent[] { new GUIContent("- None -") };
 
        #endregion

        #region Static Public Methods

        /// <summary>
        /// Draw Iron-Wall Logo.
        /// </summary>
        public static void DrawLogo()
        {
            if (OSCEditorTextures.IronWall != null)
            {
                EditorGUILayout.Space();

                var rect = GUILayoutUtility.GetRect(0, 0);
                var width = OSCEditorTextures.IronWall.width * 0.2f;
                var height = OSCEditorTextures.IronWall.height * 0.2f;

                rect.x = rect.width * 0.5f - width * 0.5f;
                rect.y = rect.y + rect.height * 0.5f - height * 0.5f;
                rect.width = width;
                rect.height = height;

                GUI.DrawTexture(rect, OSCEditorTextures.IronWall);
                EditorGUILayout.Space();
            }
        }


        public static void Packet(OSCPacket packet)
        {
            if (packet.IsBundle()) DrawBundle(packet as OSCBundle);
            else DrawMessage(packet as OSCMessage);
        }

        public static void EditablePacket(OSCPacket packet)
        {
            if (packet.IsBundle()) DrawEditableBundle(packet as OSCBundle);
            else DrawEditableMessage(packet as OSCMessage);
        }

        public static void DrawProperties(SerializedObject serializedObject, params string[] exceptionsNames)
        {
            var serializedProperty = serializedObject.GetIterator();
            var isEmpty = true;
            var enterChild = true;

            while (serializedProperty.NextVisible(enterChild))
            {
                enterChild = false;

                if (serializedProperty.name == "m_Script" ||
                    exceptionsNames.Contains(serializedProperty.name))
                    continue;

                EditorGUILayout.PropertyField(serializedProperty, true);

                isEmpty = false;
            }

            if (isEmpty)
                EditorGUILayout.LabelField(_emptyContent, OSCEditorStyles.CenterLabel);
        }

        public static void TransmitterSettings(SerializedProperty transmitterProperty, SerializedProperty addressProperty, bool drawBox = true)
        {
	        if (drawBox) GUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(addressProperty, EditorGUIUtility.currentViewWidth > 410 ?
                                          _transmitterAddressContent : _transmitterAddressContentSmall);

            TransmittersPopup(transmitterProperty, _transmitterContent);

	        if (drawBox) EditorGUILayout.EndVertical();
        }

        public static void ReceiverSettings(SerializedProperty transmitterProperty, SerializedProperty addressProperty, bool drawBox = true)
        {
			if (drawBox) GUILayout.BeginVertical("box");

            EditorGUILayout.PropertyField(addressProperty, EditorGUIUtility.currentViewWidth > 380 ?
                                          _receiverAddressContent : _receiverAddressContentSmall);

            ReceiversPopup(transmitterProperty, _receiverContent);

			if (drawBox) EditorGUILayout.EndVertical();
        }

        public static void ReflectionMember(SerializedProperty reflectionMemberProperty, Type reflectionType, OSCReflectionAccess access)
        {
            var targetProperty = reflectionMemberProperty.FindPropertyRelative("Target");
            var memberNameProperty = reflectionMemberProperty.FindPropertyRelative("MemberName");

            EditorGUILayout.PropertyField(targetProperty, _targetContent);

            if (targetProperty.objectReferenceValue != null)
            {
				GUI.enabled = memberNameProperty.stringValue != "- None -";

                memberNameProperty.stringValue = PropertiesPopup(targetProperty.objectReferenceValue,
																 memberNameProperty.stringValue,
                                                                 reflectionType,
                                                                 _propertyContent,
                                                                 access);
            }
            else
            {
                GUI.enabled = false;

                EditorGUILayout.Popup(_propertyContent, 0, _propertyPopupOptions);
            }

            GUI.enabled = true;
        }

        #endregion

        #region Static Private Methods


        private static void DrawEditableBundle(OSCBundle bundle)
        {
            if (bundle.Packets.Count > 0)
            {
                OSCPacket removePacket = null;

                foreach (var bundlePacket in bundle.Packets)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(bundlePacket.GetType().Name + ":", EditorStyles.boldLabel);

                    GUI.color = Color.red;

                    var deleteButton = GUILayout.Button("x", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(20));
                    if (deleteButton)
                    {
                        removePacket = bundlePacket;
                    }

                    GUI.color = Color.white;

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginVertical("box");

                    EditablePacket(bundlePacket);
                    EditorGUILayout.EndVertical();

                    GUILayout.Space(10);
                }

                if (removePacket != null)
                {
                    bundle.Packets.Remove(removePacket);

                    if (_valueTypeTemp.ContainsKey(removePacket))
                        _valueTypeTemp.Remove(removePacket);
                }
            }
            else
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField(_bundleEmptyContent, OSCEditorStyles.CenterLabel);
                EditorGUILayout.EndVertical();
            }

            // ADD PACKET
            EditorGUILayout.BeginHorizontal("box");
            GUI.color = Color.green;

            if (GUILayout.Button(_addBundleContent))
            {
                bundle.AddPacket(new OSCBundle());
            }

            if (GUILayout.Button(_addMessageContent))
            {
                bundle.AddPacket(new OSCMessage("/address"));
            }

            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawEditableMessage(OSCMessage message)
        {
            EditorGUILayout.LabelField(_addressContent, EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");
            message.Address = EditorGUILayout.TextField(message.Address, GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.EndVertical();

            OSCValue removeValue = null;

            EditorGUILayout.LabelField(string.Format("Values ({0}):", message.GetTags()), EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical();

            foreach (var value in message.Values)
            {
                DrawEditableValue(value, ref removeValue);
            }

            EditorGUILayout.EndVertical();

            var includeValue = CreateValueButton(message);

            if (removeValue != null)
            {
                message.Values.Remove(removeValue);
            }

            if (includeValue != null)
            {
                message.AddValue(includeValue);
            }
        }

        private static void DrawEditableArray(OSCValue value, ref OSCValue removeValue)
        {
            OSCValue removeArrayValue = null;

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField(_arrayContent, OSCEditorStyles.CenterBoldLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.red;

            var deleteButton = GUILayout.Button("x", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(20));
            if (deleteButton)
            {
                removeValue = value;
            }

            GUI.color = Color.white;
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            foreach (var arrayValues in value.ArrayValue)
            {
                DrawEditableValue(arrayValues, ref removeArrayValue);
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(_addToArrayContent, GUILayout.Width(40));
            EditorGUILayout.EndVertical();

            var includeArrayValue = CreateValueButton(value);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            if (includeArrayValue != null)
            {
                value.ArrayValue.Add(includeArrayValue);
            }

            if (removeArrayValue != null)
            {
                value.ArrayValue.Remove(removeArrayValue);
            }
        }

        private static void DrawEditableValue(OSCValue value, ref OSCValue removeValue)
        {
            if (value.Type == OSCValueType.Array)
            {
                DrawEditableArray(value, ref removeValue);
                return;
            }

            var firstColumn = 40f;
            var secondColumn = 60f;

            // FIRST COLUMN
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical("box");

            GUILayout.Label(string.Format("Tag: {0}", value.Tag), OSCEditorStyles.CenterLabel, GUILayout.Width(firstColumn));

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();

            if (value.Type == OSCValueType.Blob ||
                value.Type == OSCValueType.Impulse ||
                value.Type == OSCValueType.Null)
            {
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField(value.Type.ToString(), OSCEditorStyles.CenterLabel);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField(value.Type + ":", GUILayout.Width(secondColumn));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal("box");

                switch (value.Type)
                {
                    case OSCValueType.Color:
                        value.ColorValue = EditorGUILayout.ColorField(value.ColorValue);
                        break;
                    case OSCValueType.True:
                    case OSCValueType.False:
                        value.BoolValue = EditorGUILayout.Toggle(value.BoolValue);
                        break;
                    case OSCValueType.Char:
                        var rawString = EditorGUILayout.TextField(value.CharValue.ToString());
                        value.CharValue = rawString.Length > 0 ? rawString[0] : ' ';
                        break;
                    case OSCValueType.Int:
                        value.IntValue = EditorGUILayout.IntField(value.IntValue);
                        break;
                    case OSCValueType.Double:
                        value.DoubleValue = EditorGUILayout.DoubleField(value.DoubleValue);
                        break;
                    case OSCValueType.Long:
                        value.LongValue = EditorGUILayout.LongField(value.LongValue);
                        break;
                    case OSCValueType.Float:
                        value.FloatValue = EditorGUILayout.FloatField(value.FloatValue);
                        break;
                    case OSCValueType.String:
                        value.StringValue = EditorGUILayout.TextField(value.StringValue);
                        break;
                    case OSCValueType.Midi:
                        var midi = value.MidiValue;
                        midi.Channel = (byte)Mathf.Clamp(EditorGUILayout.IntField(midi.Channel), 0, 255);
                        midi.Status = (byte)Mathf.Clamp(EditorGUILayout.IntField(midi.Status), 0, 255);
                        midi.Data1 = (byte)Mathf.Clamp(EditorGUILayout.IntField(midi.Data1), 0, 255);
                        midi.Data2 = (byte)Mathf.Clamp(EditorGUILayout.IntField(midi.Data2), 0, 255);
                        value.MidiValue = midi;
                        break;
                    default:
                        EditorGUILayout.SelectableLabel(value.Value.ToString(), GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        break;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.red;

            var deleteButton = GUILayout.Button("x", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(20));
            if (deleteButton)
            {
                removeValue = value;
            }

            GUI.color = Color.white;
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private static void DrawBundle(OSCBundle bundle)
        {
            if (bundle != null)
            {
                if (bundle.Packets.Count > 0)
                {
                    foreach (var bundlePacket in bundle.Packets)
                    {
                        EditorGUILayout.LabelField(_bundleContent, EditorStyles.boldLabel);

                        EditorGUILayout.BeginVertical("box");

                        Packet(bundlePacket);

                        EditorGUILayout.EndVertical();
                    }
                }
                else
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField(_bundleEmptyContent, OSCEditorStyles.CenterLabel);
                    EditorGUILayout.EndVertical();
                }
            }
        }

        private static void DrawMessage(OSCMessage message)
        {
            if (message != null)
            {
                EditorGUILayout.LabelField(_addressContent, EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.SelectableLabel(message.Address, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                EditorGUILayout.EndVertical();

                if (message.Values.Count > 0)
                {
                    EditorGUILayout.LabelField(string.Format("Values ({0}):", message.GetTags()), EditorStyles.boldLabel);

                    EditorGUILayout.BeginVertical();

                    foreach (var value in message.Values)
                    {
                        DrawValue(value);
                    }

                    EditorGUILayout.EndVertical();
                }
            }
        }

        private static void DrawArray(OSCValue value)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField(_beginArrayContent, OSCEditorStyles.CenterBoldLabel);
            EditorGUILayout.EndHorizontal();

            foreach (var arrayValues in value.ArrayValue)
            {

                DrawValue(arrayValues);
            }

            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField(_endArrayContent, OSCEditorStyles.CenterBoldLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private static void DrawValue(OSCValue value)
        {
            if (value.Type == OSCValueType.Array)
            {
                DrawArray(value);
                return;
            }

            var firstColumn = 40f;
            var secondColumn = 60f;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical("box");

            GUILayout.Label(string.Format("Tag: {0}", value.Tag), OSCEditorStyles.CenterLabel, GUILayout.Width(firstColumn));

            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginHorizontal();

            if (value.Type == OSCValueType.Blob ||
                value.Type == OSCValueType.Impulse ||
                value.Type == OSCValueType.Null)
            {
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField(value.Type.ToString(), OSCEditorStyles.CenterLabel);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField(value.Type + ":", GUILayout.Width(secondColumn));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal("box");

                switch (value.Type)
                {
                    case OSCValueType.Color:
                        EditorGUILayout.ColorField(value.ColorValue);
                        break;
                    case OSCValueType.True:
                    case OSCValueType.False:
                        EditorGUILayout.Toggle(value.BoolValue);
                        break;
                    default:
                        EditorGUILayout.SelectableLabel(value.Value.ToString(), GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        break;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
        }

        private static OSCValue CreateValueButton(object sender)
        {
            EditorGUILayout.BeginHorizontal("box");

            if (!_valueTypeTemp.ContainsKey(sender))
            {
                _valueTypeTemp.Add(sender, OSCValueType.Float);
            }

            _valueTypeTemp[sender] = (OSCValueType)EditorGUILayout.EnumPopup(_valueTypeTemp[sender]);

            GUI.color = Color.green;

            var addButton = GUILayout.Button(_addValueContent, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            if (addButton)
            {
                var value = OSCEditorUtils.CreateOSCValue(_valueTypeTemp[sender]);
                if (value != null)
                {
                    return value;
                }
                else
                {
                    Debug.LogFormat("[extOSC] You can't add this ({0}) value type!", _valueTypeTemp[sender]);
                }
            }

            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();

            return null;
        }

        #endregion
    }
}