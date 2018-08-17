/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using extOSC.Editor.Panels;

namespace extOSC.Editor.Windows
{
    public class OSCWindowUtils : OSCWindow<OSCWindowUtils, OSCPanelUtils>
    {
        #region Static Public Methods

        [MenuItem("Window/extOSC/Utils Window (Experimental)", false, 100)]
        public static void ShowWindow()
        {
            Instance.titleContent = new GUIContent("OSC Utils", OSCEditorTextures.IronWall);
            Instance.minSize = new Vector2(300, 200);
            Instance.Show();
        }

        #endregion

        #region Unity Methods

        protected override void OnEnable()
        {
            rootPanel.Refresh();

            base.OnEnable();
        }

        #endregion
    }
}