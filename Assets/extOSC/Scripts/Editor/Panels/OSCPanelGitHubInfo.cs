/* Copyright (c) 2018 ExT (V.Sigalkin) */

using extOSC.Editor.Windows;
using UnityEditor;
using UnityEngine;

namespace extOSC.Editor.Panels
{
    public class OSCPanelGitHubInfo : OSCPanel
    {
        #region Public Vars 

        #endregion

        #region Private Vars

        #endregion

        #region Public Methods

        public OSCPanelGitHubInfo(OSCWindow parentWindow, string panelId) : base(parentWindow, panelId)
        { }

        #endregion

        #region Protected Methods

        protected override void DrawContent(Rect contentRect)
        {
            contentRect.position += new Vector2(5,5);
            contentRect.size -= new Vector2(10, 10);

            GUILayout.BeginArea(contentRect);

            GUILayout.BeginVertical("box");

            GUILayout.BeginVertical("box");
            GUILayout.Label("Приветствие", OSCEditorStyles.CenterBoldLabel);
            GUILayout.EndVertical();

            GUILayout.Label("Текст.");


            
            GUILayout.BeginHorizontal("box");

            var neverShow = EditorGUILayout.Toggle("Никогда не показывать", false);
            var close = GUILayout.Button("Close");
            
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.EndArea();
        }

        #endregion
    }
}