/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEditor;
using UnityEngine;

using extOSC.Core;

namespace extOSC.Editor.Drawers
{
    public class OSCPacketDrawer
    {
        #region Static Private Vars

        private static readonly GUIContent _addressContent = new GUIContent("Address:");

        private static readonly GUIContent _bundleContent = new GUIContent("Bundle:");

        private static readonly GUIContent _bundleEmptyContent = new GUIContent("Bundle is empty");

        private static readonly GUIContent _beginArrayContent = new GUIContent("Begin Array");

        private static readonly GUIContent _endArrayContent = new GUIContent("End Array");

        #endregion

        #region Private Methods

        #endregion

        #region Public 

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
            if (bundle != null)
            {
                if (bundle.Packets.Count > 0)
                {
                    foreach (var bundlePacket in bundle.Packets)
                    {
                        EditorGUILayout.LabelField(_bundleContent, EditorStyles.boldLabel);

                        EditorGUILayout.BeginVertical("box");

                        DrawLayout(bundlePacket);

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

        private void DrawMessage(OSCMessage message)
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
        
        private void DrawArray(OSCValue value)
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

        private void DrawValue(OSCValue value)
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

        #endregion
    }
}
