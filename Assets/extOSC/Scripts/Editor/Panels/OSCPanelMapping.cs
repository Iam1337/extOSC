/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using System.IO;
using System.Collections.Generic;

using extOSC.Editor.Windows;
using extOSC.Mapping;

namespace extOSC.Editor.Panels
{
    public class OSCPanelMapping : OSCPanel
    {
        #region Static Private Vars

        private static readonly GUIContent _createContent = new GUIContent("Create");

        private static readonly GUIContent _openContent = new GUIContent("Open");

        private static readonly GUIContent _emptyContent = new GUIContent("Map Bundle is empty!");

        private static readonly GUIContent _addAddressContent = new GUIContent("Add Address");

        private static readonly GUIContent _infoContent = new GUIContent("Create or load map bundle!");

        private static readonly GUIContent _addressContent = new GUIContent("Address:");

        private static readonly GUIContent _valuesContent = new GUIContent("Values:");

        private static readonly GUIContent _addMapValueContent = new GUIContent("Add Map Value");

        private static readonly GUIContent _maximusContent = new GUIContent("Maximum:");

        private static readonly GUIContent _minimumContent = new GUIContent("Minimum: ");

        private static readonly GUIContent _inputContent = new GUIContent("Input:");

        private static readonly GUIContent _outputContent = new GUIContent("Output:");

        private static readonly GUIContent _clampContent = new GUIContent("Clamp:");

        private static readonly GUIContent _toBoolContent = new GUIContent("True if:");

        private static readonly GUIContent _valueContent = new GUIContent("Value:");

        private static readonly GUIContent _trueContent = new GUIContent("True:");

        private static readonly GUIContent _falseContent = new GUIContent("False:");

        #endregion

        #region Public Vars

        public OSCMapBundle CurrentMapBundle
        {
            get
            {
                return _currentMapBundle;
            }
            set
            {
                if (_currentMapBundle == value)
                    return;

                SaveCurrentMapBundle();

                _currentMapBundle = value;
            }
        }

        #endregion

        #region Private Vars

        private Color _defaultColor;

        private Vector2 _scrollPosition;

        private OSCMapMessage _deleteMessage;

        private OSCMapValue _deleteValue;

        private OSCMapBundle _currentMapBundle;

        private int _frameCounter = 0;

        private Dictionary<object, OSCMapType> _mapTypeTemp = new Dictionary<object, OSCMapType>();

        #endregion

        #region Public Methods

        public OSCPanelMapping(OSCWindow parentWindow, string panelId) : base(parentWindow, panelId) 
        { }

        public void SaveCurrentMapBundle()
        {
            if (_currentMapBundle == null) return;

            EditorUtility.SetDirty(_currentMapBundle);
            AssetDatabase.SaveAssets();
        }

        public override void Update()
        {
            if (!EditorApplication.isPlaying)
            {
                _frameCounter++;

                if (_frameCounter > 200)
                {
                    _frameCounter = 0;

                    SaveCurrentMapBundle();
                }
            }
        }

        #endregion

        #region Protected Methods

        protected override void DrawContent(Rect contentRect)
        {
            _defaultColor = GUI.color;

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            var createMapButton = GUILayout.Button(_createContent, EditorStyles.toolbarButton);

            GUILayout.Space(5);

            if (GUILayout.Button(_openContent, EditorStyles.toolbarDropDown))
            {
                GUIContent[] popupItems;
                string[] patches;

                GetMappingAssets(out popupItems, out patches);

                var customMenuRect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0);

                EditorUtility.DisplayCustomMenu(customMenuRect, popupItems, -1, OpenMapBundle, patches);
            }

            GUILayout.FlexibleSpace();

            if (_currentMapBundle != null) GUILayout.Label(string.Format("Name: {0}", _currentMapBundle.name));

            EditorGUILayout.EndHorizontal();

            var expand = contentRect.width > 810;

            if (_currentMapBundle != null)
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                GUILayout.Label(string.Format("Path: {0}", AssetDatabase.GetAssetPath(_currentMapBundle)));

                if (_currentMapBundle.Messages.Count > 0)
                {
                    _deleteMessage = null;

                    foreach (var mapMessage in _currentMapBundle.Messages)
                    {
                        DrawMapMessage(mapMessage, expand);
                    }

                    if (_deleteMessage != null) _currentMapBundle.Messages.Remove(_deleteMessage);
                }
                else
                {
                    EditorGUILayout.BeginHorizontal("box");
                    GUILayout.Label(_emptyContent, OSCEditorStyles.CenterLabel);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal("box");
                GUI.color = Color.green;
                var createMessageButton = GUILayout.Button(_addAddressContent);
                GUI.color = _defaultColor;
                EditorGUILayout.EndHorizontal();

                if (createMessageButton)
                {
                    _currentMapBundle.Messages.Add(new OSCMapMessage()
                    {
                        Address = "/address/" + _currentMapBundle.Messages.Count
                    });
                }
                if (_deleteMessage != null)
                {
                    _currentMapBundle.Messages.Remove(_deleteMessage);
                }

                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.LabelField(_infoContent, OSCEditorStyles.CenterLabel, GUILayout.Height(contentRect.height));
            }

            if (createMapButton) CreateMapBundle();
        }

        #endregion

        #region Private Methods

        private void DrawMapMessage(OSCMapMessage mapMessage, bool expand)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(_addressContent);
            GUI.color = Color.red;
            var deleteButton = GUILayout.Button("x", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(20));
            if (deleteButton) _deleteMessage = mapMessage;
            GUI.color = _defaultColor;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal("box");
            mapMessage.Address = EditorGUILayout.TextField(mapMessage.Address);
            EditorGUILayout.EndHorizontal();

            if (mapMessage.Values.Count > 0)
            {
                GUILayout.Label(_valuesContent);

                _deleteValue = null;

                foreach (var mapValue in mapMessage.Values)
                {
                    DrawMapValue(mapValue, expand);
                }

                if (_deleteValue != null) mapMessage.Values.Remove(_deleteValue);
            }
            else
            {
                GUILayout.Label(_valuesContent);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("- Empty -", OSCEditorStyles.CenterLabel);
                EditorGUILayout.EndVertical();
            }

            if (!_mapTypeTemp.ContainsKey(mapMessage))
                _mapTypeTemp.Add(mapMessage, OSCMapType.Float);

            EditorGUILayout.BeginHorizontal("box");

            _mapTypeTemp[mapMessage] = (OSCMapType)EditorGUILayout.EnumPopup(_mapTypeTemp[mapMessage]);

            GUI.color = Color.green;
            var addButton = GUILayout.Button(_addMapValueContent, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            if (addButton) CreateMapValue(mapMessage, _mapTypeTemp[mapMessage]);
            GUI.color = _defaultColor;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawMapValue(OSCMapValue mapValue, bool expand)
        {
            if (expand) EditorGUILayout.BeginHorizontal();
            else EditorGUILayout.BeginHorizontal("box");

            EditorGUILayout.BeginHorizontal("box", GUILayout.Width(80));
            GUILayout.Label(mapValue.Type + ":");
            EditorGUILayout.EndHorizontal();

            if (mapValue.Type == OSCMapType.Float)
            {
                DrawMapValueFloat(mapValue, expand);
            }
            else if (mapValue.Type == OSCMapType.Int)
            {
                DrawMapValueInt(mapValue, expand);
            }
            else if (mapValue.Type == OSCMapType.FloatToBool)
            {
                DrawMapValueFloatToBool(mapValue, expand);
            }
            else if (mapValue.Type == OSCMapType.IntToBool)
            {
                DrawMapValueIntToBool(mapValue, expand);
            }
            else if (mapValue.Type == OSCMapType.BoolToFloat)
            {
                DrawMapValueBoolToFloat(mapValue, expand);
            }
            else if (mapValue.Type == OSCMapType.BoolToInt)
            {
                DrawMapValueBoolToInt(mapValue, expand);
            }

            EditorGUILayout.BeginHorizontal("box");
            GUI.color = Color.red;
            var deleteButton = GUILayout.Button("x", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(20));
            if (deleteButton) _deleteValue = mapValue;
            GUI.color = _defaultColor;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawMapValueFloat(OSCMapValue mapValue, bool expand)
        {
            GUI.color = Color.yellow;
            EditorGUILayout.BeginHorizontal("box", GUILayout.Width(50));
            GUILayout.Label(_inputContent);
            EditorGUILayout.EndHorizontal();
            GUI.color = _defaultColor;

            if (!expand) EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label(_minimumContent);
            mapValue.InputMin = EditorGUILayout.FloatField(mapValue.InputMin);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label(_maximusContent);
            mapValue.InputMax = EditorGUILayout.FloatField(mapValue.InputMax);
            EditorGUILayout.EndHorizontal();

            if (!expand) EditorGUILayout.EndVertical();

            GUI.color = Color.yellow;
            EditorGUILayout.BeginHorizontal("box", GUILayout.Width(50));
            GUILayout.Label(_outputContent);
            EditorGUILayout.EndHorizontal();
            GUI.color = _defaultColor;

            if (!expand) EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label(_minimumContent);
            mapValue.OutputMin = EditorGUILayout.FloatField(mapValue.OutputMin);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label(_maximusContent);
            mapValue.OutputMax = EditorGUILayout.FloatField(mapValue.OutputMax);
            EditorGUILayout.EndHorizontal();

            if (!expand) EditorGUILayout.EndVertical();

            GUI.color = Color.yellow;
            EditorGUILayout.BeginHorizontal("box", GUILayout.Width(50));
            GUILayout.Label(_clampContent);
            EditorGUILayout.EndHorizontal();
            GUI.color = _defaultColor;

            EditorGUILayout.BeginHorizontal("box");
            mapValue.Clamp = EditorGUILayout.Toggle(mapValue.Clamp, GUILayout.Width(15));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawMapValueInt(OSCMapValue mapValue, bool expand)
        {
            GUI.color = Color.yellow;
            EditorGUILayout.BeginHorizontal("box", GUILayout.Width(50));
            GUILayout.Label(_inputContent);
            EditorGUILayout.EndHorizontal();
            GUI.color = _defaultColor;

            if (!expand) EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label(_minimumContent);
            mapValue.InputMin = EditorGUILayout.IntField((int)mapValue.InputMin);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label(_maximusContent);
            mapValue.InputMax = EditorGUILayout.IntField((int)mapValue.InputMax);
            EditorGUILayout.EndHorizontal();

            if (!expand) EditorGUILayout.EndVertical();

            GUI.color = Color.yellow;
            EditorGUILayout.BeginHorizontal("box", GUILayout.Width(50));
            GUILayout.Label(_outputContent);
            EditorGUILayout.EndHorizontal();
            GUI.color = _defaultColor;

            if (!expand) EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label(_minimumContent);
            mapValue.OutputMin = EditorGUILayout.IntField((int)mapValue.OutputMin);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label(_maximusContent);
            mapValue.OutputMax = EditorGUILayout.IntField((int)mapValue.OutputMax);
            EditorGUILayout.EndHorizontal();

            if (!expand) EditorGUILayout.EndVertical();

            GUI.color = Color.yellow;
            EditorGUILayout.BeginHorizontal("box", GUILayout.Width(50));
            GUILayout.Label(_clampContent);
            EditorGUILayout.EndHorizontal();
            GUI.color = _defaultColor;

            EditorGUILayout.BeginHorizontal("box");
            mapValue.Clamp = EditorGUILayout.Toggle(mapValue.Clamp, GUILayout.Width(15));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawMapValueFloatToBool(OSCMapValue mapValue, bool expand)
        {
            GUI.color = Color.yellow;
            EditorGUILayout.BeginHorizontal("box", GUILayout.Width(50));
            GUILayout.Label(_toBoolContent);
            EditorGUILayout.EndHorizontal();
            GUI.color = _defaultColor;

            EditorGUILayout.BeginHorizontal("box",GUILayout.Width(120));
            mapValue.Logic = (OSCMapLogic)EditorGUILayout.EnumPopup(mapValue.Logic);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label(_valueContent);
            mapValue.Value = EditorGUILayout.FloatField(mapValue.Value);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawMapValueIntToBool(OSCMapValue mapValue, bool expand)
        {
            GUI.color = Color.yellow;
            EditorGUILayout.BeginHorizontal("box", GUILayout.Width(50));
            GUILayout.Label(_toBoolContent);
            EditorGUILayout.EndHorizontal();
            GUI.color = _defaultColor;

            EditorGUILayout.BeginHorizontal("box", GUILayout.Width(120));
            mapValue.Logic = (OSCMapLogic)EditorGUILayout.EnumPopup(mapValue.Logic);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label(_valueContent);
            mapValue.Value = EditorGUILayout.IntField((int)mapValue.Value);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawMapValueBoolToFloat(OSCMapValue mapValue, bool expand)
        {
            if (!expand)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
            }

            GUI.color = Color.yellow;
            EditorGUILayout.BeginHorizontal("box", GUILayout.Width(50));
            GUILayout.Label(_trueContent);
            EditorGUILayout.EndHorizontal();
            GUI.color = _defaultColor;

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label(_valueContent);
            mapValue.TrueValue = EditorGUILayout.FloatField(mapValue.TrueValue);
            EditorGUILayout.EndHorizontal();

            if (!expand)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            GUI.color = Color.yellow;
            EditorGUILayout.BeginHorizontal("box", GUILayout.Width(50));
            GUILayout.Label(_falseContent);
            EditorGUILayout.EndHorizontal();
            GUI.color = _defaultColor;

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label(_valueContent);
            mapValue.FalseValue = EditorGUILayout.FloatField(mapValue.FalseValue);
            EditorGUILayout.EndHorizontal();

            if (!expand)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawMapValueBoolToInt(OSCMapValue mapValue, bool expand)
        {
            if (!expand)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
            }

            GUI.color = Color.yellow;
            EditorGUILayout.BeginHorizontal("box", GUILayout.Width(50));
            GUILayout.Label(_trueContent);
            EditorGUILayout.EndHorizontal();
            GUI.color = _defaultColor;

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label(_valueContent);
            mapValue.TrueValue = EditorGUILayout.IntField((int)mapValue.TrueValue);
            EditorGUILayout.EndHorizontal();

            if (!expand)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            GUI.color = Color.yellow;
            EditorGUILayout.BeginHorizontal("box", GUILayout.Width(50));
            GUILayout.Label(_falseContent);
            EditorGUILayout.EndHorizontal();
            GUI.color = _defaultColor;

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label(_valueContent);
            mapValue.FalseValue = EditorGUILayout.IntField((int)mapValue.FalseValue);
            EditorGUILayout.EndHorizontal();

            if (!expand)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
        }

        private void CreateMapValue(OSCMapMessage mapMessage, OSCMapType mapType)
        {
            var mapValue = new OSCMapValue();
            mapValue.Type = mapType;

            if (mapType == OSCMapType.FloatToBool)
            {
                mapValue.Value = 0.5f;
            }
            else if (mapType == OSCMapType.IntToBool)
            {
                mapValue.Value = 1;
            }

            mapMessage.Values.Add(mapValue);
        }

        private void CreateMapBundle()
        {
            var assetPath = EditorUtility.SaveFilePanelInProject("Create Map Bundle", "New Map Bundle", "asset", "Save map bundle as...");
            if (!string.IsNullOrEmpty(assetPath))
            {
                SaveCurrentMapBundle();

                var mapBundle = ScriptableObject.CreateInstance<OSCMapBundle>();

                AssetDatabase.CreateAsset(mapBundle, assetPath);
                AssetDatabase.SaveAssets();

                _currentMapBundle = mapBundle;

                Selection.activeObject = mapBundle;
            }
        }

        private void OpenMapBundle(object userData, string[] options, int selected)
        {
            SaveCurrentMapBundle();

            var patches = (string[])userData;

            _currentMapBundle = AssetDatabase.LoadAssetAtPath<OSCMapBundle>(patches[selected]);
        }

        private void GetMappingAssets(out GUIContent[] popupItems, out string[] patches)
        {
            var guids = AssetDatabase.FindAssets("t:OSCMapBundle", null);
            popupItems = new GUIContent[guids.Length];
            patches = new string[guids.Length];

            for (var index = 0; index < guids.Length; index++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[index]);

                patches[index] = path;
                popupItems[index] = new GUIContent(Path.GetFileNameWithoutExtension(path));
            }
        }

        #endregion
    }
}