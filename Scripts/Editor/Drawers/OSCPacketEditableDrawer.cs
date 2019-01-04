/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEditor;
using UnityEngine;

using System.Collections.Generic;

using extOSC.Core;

namespace extOSC.Editor.Drawers
{
    public class OSCPacketEditableDrawer
    {
        #region Static Private Vars

        private static readonly GUIContent _bundleEmptyContent = new GUIContent("Bundle is empty!");

        private static readonly GUIContent _addressContent = new GUIContent("Address:");

        private static readonly GUIContent _addBundleContent = new GUIContent("Add Bundle");

        private static readonly GUIContent _addMessageContent = new GUIContent("Add Message");

        private static readonly GUIContent _addValueContent = new GUIContent("Add Value");

        private static readonly GUIContent _arrayContent = new GUIContent("Array");

        private static readonly GUIContent _addToArrayContent = new GUIContent("Array:");

        #endregion

        #region Private Vars

        private static Dictionary<object, OSCValueType> _valueTypeTemp = new Dictionary<object, OSCValueType>();

        #endregion

        #region Public Methods

        public void DrawLayout(OSCPacket packet)
        {
            if (packet.IsBundle())
            {
                DrawBundle((OSCBundle) packet);
            }
            else
            {
                DrawMessage((OSCMessage) packet);
            }
        }

        #endregion

        #region Private Methods

        private void DrawBundle(OSCBundle bundle)
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

                    DrawLayout(bundlePacket);
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

        private void DrawMessage(OSCMessage message)
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
                DrawValue(value, ref removeValue);
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

        private void DrawArray(OSCValue value, ref OSCValue removeValue)
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
                DrawValue(arrayValues, ref removeArrayValue);
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

        private void DrawValue(OSCValue value, ref OSCValue removeValue)
        {
            if (value.Type == OSCValueType.Array)
            {
                DrawArray(value, ref removeValue);
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

        private OSCValue CreateValueButton(object sender)
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
