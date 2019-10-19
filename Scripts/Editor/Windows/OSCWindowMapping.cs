/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEditor;
using UnityEngine;

using extOSC.Mapping;
using extOSC.Editor.Panels;

namespace extOSC.Editor.Windows
{
	public class OSCWindowMapping : OSCWindow<OSCWindowMapping, OSCPanelMapping>
	{
		#region Static Public Methods

		public static void Open()
		{
			Instance.titleContent = new GUIContent("OSC Mapping", OSCEditorTextures.IronWallSmall);
			Instance.minSize = new Vector2(550, 200);
			Instance.Show();
			Instance.Focus();
		}

		public static void OpenBundle(OSCMapBundle bundle)
		{
			Open();

			Instance.rootPanel.CurrentMapBundle = bundle;
		}

		#endregion

		#region Private Vars

		private readonly string _lastFileSettings = OSCEditorSettings.Mapping + "lastfile";

		private int _frameCounter = 0;

		#endregion

		#region Unity Methods

		protected void Update()
		{
			if (!EditorApplication.isPlaying)
			{
				_frameCounter++;

				if (_frameCounter > 200)
				{
					_frameCounter = 0;

					rootPanel.SaveCurrentMapBundle();
				}
			}
		}

		protected override void OnDestroy()
		{
			SaveWindowSettings();

			base.OnDestroy();
		}

		#endregion

		#region Protected Methods

		protected override void LoadWindowSettings()
		{
			var assetPath = OSCEditorSettings.GetString(_lastFileSettings, "");

			if (!string.IsNullOrEmpty(assetPath))
			{
				rootPanel.CurrentMapBundle = AssetDatabase.LoadAssetAtPath<OSCMapBundle>(assetPath);
			}
		}

		protected override void SaveWindowSettings()
		{
			rootPanel.SaveCurrentMapBundle();

			if (rootPanel.CurrentMapBundle != null)
			{
				OSCEditorSettings.SetString(_lastFileSettings, AssetDatabase.GetAssetPath(rootPanel.CurrentMapBundle));
			}
			else
			{
				OSCEditorSettings.SetString(_lastFileSettings, "");
			}
		}

		#endregion
	}
}