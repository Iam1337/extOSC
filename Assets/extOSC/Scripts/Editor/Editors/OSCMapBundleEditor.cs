/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using extOSC.Mapping;
using extOSC.Editor.Windows;

namespace extOSC.Editor
{
	[CustomEditor(typeof(OSCMapBundle))]
	public class OSCMapBundleEditor : UnityEditor.Editor
	{
		#region Static Private Vars

		private static readonly GUIContent _emptyBundleContent = new GUIContent("Map Bundle is empty!");

		private static readonly GUIContent _openButton = new GUIContent("Open in Mapper");

		private static readonly GUIContent _typeContents = new GUIContent("Type:");

		#endregion

		#region Private Vars

		private OSCMapBundle _bundle;

		#endregion

		#region Unity Methods

		protected void OnEnable()
		{
			_bundle = target as OSCMapBundle;
		}

		public override void OnInspectorGUI()
		{
			OSCEditorInterface.LogoLayout();

			GUILayout.BeginVertical(OSCEditorStyles.Box);

			var openButton = GUILayout.Button(_openButton, GUILayout.Height(40));
			if (openButton)
			{
				OSCWindowMapping.OpenBundle((OSCMapBundle) target);
			}

			GUILayout.EndVertical();

			GUILayout.BeginVertical();

			if (_bundle.Messages.Count > 0)
			{
				foreach (var message in _bundle.Messages)
				{
					GUILayout.BeginVertical(OSCEditorStyles.Box);

					GUILayout.BeginVertical(OSCEditorStyles.Box);
					EditorGUILayout.LabelField("Address: " + message.Address, EditorStyles.boldLabel);
					GUILayout.EndVertical();

					foreach (var value in message.Values)
					{
						DrawValue(value);
					}

					GUILayout.EndVertical();
				}
			}
			else
			{
				EditorGUILayout.BeginHorizontal(OSCEditorStyles.Box);
				GUILayout.Label(_emptyBundleContent, OSCEditorStyles.CenterLabel, GUILayout.Height(40));
				EditorGUILayout.EndHorizontal();
			}

			GUILayout.EndVertical();
		}

		#endregion

		#region Private Methods

		private void DrawValue(OSCMapValue value)
		{
			GUILayout.BeginVertical();
			GUILayout.BeginVertical(OSCEditorStyles.Box);

			EditorGUILayout.BeginHorizontal();

			GUI.color = Color.yellow;
			EditorGUILayout.BeginHorizontal(OSCEditorStyles.Box);
			GUILayout.Label(_typeContents, GUILayout.Width(50));
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;

			EditorGUILayout.BeginHorizontal(OSCEditorStyles.Box);
			GUILayout.Label(value.Type.ToString());
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndHorizontal();

			GUILayout.EndVertical();
			GUILayout.EndVertical();
		}

		#endregion
	}
}