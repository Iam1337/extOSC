/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEditor;
using UnityEngine;

using extOSC.Mapping;
using extOSC.Editor.Panels;

namespace extOSC.Editor.Windows
{
    public class OSCWindowMapping : OSCWindow<OSCWindowMapping, OSCPanelMapping>
    {
        #region Static Public Methods

        [MenuItem("Window/extOSC/Mapping Window", false, 0)]
        public static void ShowWindow()
        {
            Instance.titleContent = new GUIContent("OSC Mapping", OSCEditorTextures.IronWall);
            Instance.minSize = new Vector2(550, 200);
            Instance.Show();
        }

        public static void OpenBundle(OSCMapBundle bundle)
        {
            ShowWindow();

            Instance.Focus();
            Instance.rootPanel.CurrentMapBundle = bundle;
        }

        #endregion

        #region Private Vars

        private readonly string _lastFileSettings = OSCEditorSettings.Mapping + "lastfile";

        #endregion

        #region Unity Methods

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