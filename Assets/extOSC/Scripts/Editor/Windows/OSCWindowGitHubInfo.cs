/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEditor;
using UnityEngine;

using extOSC.Editor.Panels;


namespace extOSC.Editor.Windows
{
    public class OSCWindowGitHubInfo : OSCWindow<OSCWindowGitHubInfo, OSCPanelGitHubInfo>
    {
        #region Public Vars

        #endregion

        #region Public Methods

        [MenuItem("Window/extOSC/GitHub Info", false, 1040)]
        public static void ShowWindow()
        {
            Instance.titleContent = new GUIContent("OSC Console", OSCEditorTextures.IronWallSmall);
            Instance.minSize = new Vector2(610, 200);
            Instance.maxSize = Instance.minSize;
            Instance.ShowPopup();
        }

        #endregion
    }
}