/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using System.IO;
using System.Collections.Generic;

using extOSC.Mapping;
using extOSC.Editor.Windows;

namespace extOSC.Editor.Panels
{
	public class OSCPanelMapping : OSCPanel
	{
		#region Static Private Vars

		private static readonly GUIContent _createContent = new GUIContent("Create");

		private static readonly GUIContent _openContent = new GUIContent("Open");

		private static readonly GUIContent _bundleEmptyContent = new GUIContent("Map Bundle is empty!");

		private static readonly GUIContent _addAddressContent = new GUIContent("Add Address");

		private static readonly GUIContent _infoContent = new GUIContent("Create or load map bundle!");

		private static readonly GUIContent _addressContent = new GUIContent("Address:");

		private static readonly GUIContent _valuesContent = new GUIContent("Values:");

		private static readonly GUIContent _addMapValueContent = new GUIContent("Add Map Value");

		private static readonly GUIContent _maximumContent = new GUIContent("Maximum:");

		private static readonly GUIContent _minimumContent = new GUIContent("Minimum: ");

		private static readonly GUIContent _inputContent = new GUIContent("Input:");

		private static readonly GUIContent _outputContent = new GUIContent("Output:");

		private static readonly GUIContent _clampContent = new GUIContent("Clamp:");

		private static readonly GUIContent _toBoolContent = new GUIContent("True if:");

		private static readonly GUIContent _valueContent = new GUIContent("Value:");

		private static readonly GUIContent _trueContent = new GUIContent("True:");

		private static readonly GUIContent _falseContent = new GUIContent("False:");

		private static readonly GUIContent _emptyContent = new GUIContent("- Empty -");

		private static readonly GUIContent _closeContent = new GUIContent("x");

		#endregion

		#region Public Vars

		public OSCMapBundle CurrentMapBundle
		{
			get => _currentMapBundle;
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

		private Vector2 _scrollPosition;

		private OSCMapMessage _deleteMessage;

		private OSCMapValue _deleteValue;

		private OSCMapBundle _currentMapBundle;

		private readonly Dictionary<object, OSCMapType> _mapTypeTemp = new Dictionary<object, OSCMapType>();

		#endregion

		#region Public Methods

		public OSCPanelMapping(OSCWindow window) : base(window)
		{ }

		public void SaveCurrentMapBundle()
		{
			if (_currentMapBundle == null) return;

			EditorUtility.SetDirty(_currentMapBundle);
			AssetDatabase.SaveAssets();
		}

		#endregion

		#region Protected Methods

		protected override void DrawContent(ref Rect contentRect)
		{
			var defaultColor = GUI.color;
			var createMapButton = false;

			using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
			{
				createMapButton = GUILayout.Button(_createContent, EditorStyles.toolbarButton);

				GUILayout.Space(5);

				if (GUILayout.Button(_openContent, EditorStyles.toolbarDropDown))
				{
					GetMappingAssets(out var popupItems, out var patches);

					var customMenuRect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0);

					EditorUtility.DisplayCustomMenu(customMenuRect, popupItems, -1, OpenMapBundle, patches);
				}

				GUILayout.FlexibleSpace();

				if (_currentMapBundle != null)
					GUILayout.Label($"Name: {_currentMapBundle.name}");
			}

			var expand = contentRect.width > 810;
			if (_currentMapBundle != null)
			{
				using (var scroll = new GUILayout.ScrollViewScope(_scrollPosition))
				{
					GUILayout.Label($"Path: {AssetDatabase.GetAssetPath(_currentMapBundle)}");

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
						using (new GUILayout.HorizontalScope(OSCEditorStyles.Box))
						{
							GUILayout.Label(_bundleEmptyContent, OSCEditorStyles.CenterLabel);
						}
					}

					var createMessageButton = false;
					using (new GUILayout.HorizontalScope(OSCEditorStyles.Box))
					{
						GUI.color = Color.green;
						createMessageButton = GUILayout.Button(_addAddressContent);
						GUI.color = defaultColor;
					}

					// ACTIONS
					if (createMessageButton)
					{
						_currentMapBundle.Messages.Add(new OSCMapMessage
						{
							Address = "/address/" + _currentMapBundle.Messages.Count
						});
					}

					if (_deleteMessage != null)
					{
						_currentMapBundle.Messages.Remove(_deleteMessage);
					}

					_scrollPosition = scroll.scrollPosition;
				}
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
			var defaultColor = GUI.color;

			using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
			{
				using (new GUILayout.HorizontalScope())
				{
					GUILayout.Label(_addressContent);

					GUI.color = Color.red;
					if (GUILayout.Button(_closeContent, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorGUIUtility.singleLineHeight)))
						_deleteMessage = mapMessage;

					GUI.color = defaultColor;
				}

				using (new GUILayout.HorizontalScope(OSCEditorStyles.Box))
				{
					mapMessage.Address = EditorGUILayout.TextField(mapMessage.Address);
				}

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
					using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
					{
						EditorGUILayout.LabelField(_emptyContent, OSCEditorStyles.CenterLabel);
					}
				}

				if (!_mapTypeTemp.ContainsKey(mapMessage))
					_mapTypeTemp.Add(mapMessage, OSCMapType.Float);

				using (new GUILayout.HorizontalScope(OSCEditorStyles.Box))
				{
					_mapTypeTemp[mapMessage] = (OSCMapType) EditorGUILayout.EnumPopup(_mapTypeTemp[mapMessage]);

					GUI.color = Color.green;
					if (GUILayout.Button(_addMapValueContent, GUILayout.Height(EditorGUIUtility.singleLineHeight)))
						CreateMapValue(mapMessage, _mapTypeTemp[mapMessage]);

					GUI.color = defaultColor;
				}
			}
		}

		private void DrawMapValue(OSCMapValue mapValue, bool expand)
		{
			var defaultColor = GUI.color;

			if (expand) EditorGUILayout.BeginHorizontal();
			else EditorGUILayout.BeginHorizontal(OSCEditorStyles.Box);

			using (new GUILayout.HorizontalScope(OSCEditorStyles.Box, GUILayout.Width(80)))
			{
				GUILayout.Label(mapValue.Type + ":");
			}

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

			using (new GUILayout.HorizontalScope(OSCEditorStyles.Box, GUILayout.Width(25)))
			{
				GUI.color = Color.red;
				if (GUILayout.Button(_closeContent, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorGUIUtility.singleLineHeight)))
					_deleteValue = mapValue;

				GUI.color = defaultColor;
			}

			EditorGUILayout.EndHorizontal();
		}

		private void DrawMapValueFloat(OSCMapValue mapValue, bool expand)
		{
			var defaultColor = GUI.color;

			GUI.color = Color.yellow;
			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box, GUILayout.Width(50)))
			{
				GUILayout.Label(_inputContent);
			}

			GUI.color = defaultColor;

			if (!expand) EditorGUILayout.BeginVertical();

			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box))
			{
				GUILayout.Label(_minimumContent);
				mapValue.InputMin = EditorGUILayout.FloatField(mapValue.InputMin);
			}

			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box))
			{
				GUILayout.Label(_maximumContent);
				mapValue.InputMax = EditorGUILayout.FloatField(mapValue.InputMax);
			}

			if (!expand) EditorGUILayout.EndVertical();

			GUI.color = Color.yellow;
			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box, GUILayout.Width(50)))
			{
				GUILayout.Label(_outputContent);
			}

			GUI.color = defaultColor;

			if (!expand) EditorGUILayout.BeginVertical();

			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box))
			{
				GUILayout.Label(_minimumContent);
				mapValue.OutputMin = EditorGUILayout.FloatField(mapValue.OutputMin);
			}

			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box))
			{
				GUILayout.Label(_maximumContent);
				mapValue.OutputMax = EditorGUILayout.FloatField(mapValue.OutputMax);
			}

			if (!expand) EditorGUILayout.EndVertical();

			GUI.color = Color.yellow;
			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box, GUILayout.Width(50)))
			{
				GUILayout.Label(_clampContent);
			}

			GUI.color = defaultColor;

			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box))
			{
				mapValue.Clamp = EditorGUILayout.Toggle(mapValue.Clamp, GUILayout.Width(15));
			}
		}

		private void DrawMapValueInt(OSCMapValue mapValue, bool expand)
		{
			var defaultColor = GUI.color;

			GUI.color = Color.yellow;
			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box, GUILayout.Width(50)))
			{
				GUILayout.Label(_inputContent);
			}

			GUI.color = defaultColor;

			if (!expand) EditorGUILayout.BeginVertical();

			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box))
			{
				GUILayout.Label(_minimumContent);
				mapValue.InputMin = EditorGUILayout.IntField((int) mapValue.InputMin);
			}

			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box))
			{
				GUILayout.Label(_maximumContent);
				mapValue.InputMax = EditorGUILayout.IntField((int) mapValue.InputMax);
			}

			if (!expand) EditorGUILayout.EndVertical();

			GUI.color = Color.yellow;
			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box, GUILayout.Width(50)))
			{
				GUILayout.Label(_outputContent);
			}

			GUI.color = defaultColor;

			if (!expand) EditorGUILayout.BeginVertical();

			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box))
			{
				GUILayout.Label(_minimumContent);
				mapValue.OutputMin = EditorGUILayout.IntField((int) mapValue.OutputMin);
			}

			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box))
			{
				GUILayout.Label(_maximumContent);
				mapValue.OutputMax = EditorGUILayout.IntField((int) mapValue.OutputMax);
			}

			if (!expand) EditorGUILayout.EndVertical();

			GUI.color = Color.yellow;
			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box, GUILayout.Width(50)))
			{
				GUILayout.Label(_clampContent);
			}

			GUI.color = defaultColor;

			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box))
			{
				mapValue.Clamp = EditorGUILayout.Toggle(mapValue.Clamp, GUILayout.Width(15));
			}
		}

		private void DrawMapValueFloatToBool(OSCMapValue mapValue, bool expand)
		{
			var defaultColor = GUI.color;

			GUI.color = Color.yellow;
			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box, GUILayout.Width(50)))
			{
				GUILayout.Label(_toBoolContent);
			}

			GUI.color = defaultColor;

			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box, GUILayout.Width(150)))
			{
				mapValue.Logic = (OSCMapLogic) EditorGUILayout.EnumPopup(mapValue.Logic);
			}

			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box))
			{
				GUILayout.Label(_valueContent);
				mapValue.Value = EditorGUILayout.FloatField(mapValue.Value);
			}
		}

		private void DrawMapValueIntToBool(OSCMapValue mapValue, bool expand)
		{
			var defaultColor = GUI.color;

			GUI.color = Color.yellow;
			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box, GUILayout.Width(50)))
			{
				GUILayout.Label(_toBoolContent);
			}

			GUI.color = defaultColor;

			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box, GUILayout.Width(150)))
			{
				mapValue.Logic = (OSCMapLogic) EditorGUILayout.EnumPopup(mapValue.Logic);
			}

			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box))
			{
				GUILayout.Label(_valueContent);
				mapValue.Value = EditorGUILayout.IntField((int) mapValue.Value);
			}
		}

		private void DrawMapValueBoolToFloat(OSCMapValue mapValue, bool expand)
		{
			var defaultColor = GUI.color;

			if (!expand)
			{
				EditorGUILayout.BeginVertical();
				EditorGUILayout.BeginHorizontal();
			}

			GUI.color = Color.yellow;
			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box, GUILayout.Width(50)))
			{
				GUILayout.Label(_trueContent);
			}

			GUI.color = defaultColor;

			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box))
			{
				GUILayout.Label(_valueContent);
				mapValue.TrueValue = EditorGUILayout.FloatField(mapValue.TrueValue);
			}

			if (!expand)
			{
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
			}

			GUI.color = Color.yellow;
			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box, GUILayout.Width(50)))
			{
				GUILayout.Label(_falseContent);
			}

			GUI.color = defaultColor;

			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box))
			{
				GUILayout.Label(_valueContent);
				mapValue.FalseValue = EditorGUILayout.FloatField(mapValue.FalseValue);
			}

			if (!expand)
			{
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}
		}

		private void DrawMapValueBoolToInt(OSCMapValue mapValue, bool expand)
		{
			var defaultColor = GUI.color;

			if (!expand)
			{
				EditorGUILayout.BeginVertical();
				EditorGUILayout.BeginHorizontal();
			}

			GUI.color = Color.yellow;
			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box, GUILayout.Width(50)))
			{
				GUILayout.Label(_trueContent);
			}

			GUI.color = defaultColor;

			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box))
			{
				GUILayout.Label(_valueContent);
				mapValue.TrueValue = EditorGUILayout.IntField((int) mapValue.TrueValue);
			}

			if (!expand)
			{
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
			}

			GUI.color = Color.yellow;
			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box, GUILayout.Width(50)))
			{
				GUILayout.Label(_falseContent);
			}

			GUI.color = defaultColor;

			using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box))
			{
				GUILayout.Label(_valueContent);
				mapValue.FalseValue = EditorGUILayout.IntField((int) mapValue.FalseValue);
			}

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

			var patches = (string[]) userData;

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