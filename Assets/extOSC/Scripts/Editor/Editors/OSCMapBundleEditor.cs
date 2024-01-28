/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using System.Linq;
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

			using (new EditorGUILayout.VerticalScope(OSCEditorStyles.Box))
			{
				var openButton = GUILayout.Button(_openButton, GUILayout.Height(40));
				if (openButton)
				{
					OSCWindowMapping.OpenBundle(_bundle);
				}
			}

			using (new EditorGUILayout.VerticalScope())
			{
				if (_bundle.Messages.Count > 0)
				{
					var index = 0;
					foreach (var message in _bundle.Messages)
					{
						var types = message.Values.Select(v => v.Type.ToString());

						using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box))
						{
							using (new EditorGUILayout.VerticalScope(OSCEditorStyles.Box))
							{
								EditorGUILayout.LabelField((++index).ToString(), OSCEditorStyles.CenterBoldLabel, GUILayout.Width(40));
							}

							using (new EditorGUILayout.VerticalScope(OSCEditorStyles.Box))
							{
								EditorGUILayout.LabelField($"{message.Address} {string.Join(", ", types)}");
							}
						}
					}
				}
				else
				{
					using (new EditorGUILayout.HorizontalScope(OSCEditorStyles.Box))
					{
						GUILayout.Label(_emptyBundleContent, OSCEditorStyles.CenterLabel, GUILayout.Height(40));
					}
				}
			}
		}

		#endregion
	}
}