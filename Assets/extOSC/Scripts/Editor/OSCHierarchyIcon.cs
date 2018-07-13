﻿/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEditor;
using UnityEngine;

using System;

using extOSC.Core;

namespace extOSC.Editor
{
    [InitializeOnLoad]
    public static class OSCHierarchyIcon
    {
        #region Constructor Methods

        static OSCHierarchyIcon()
        {
            EditorApplication.hierarchyWindowItemOnGUI =
                (EditorApplication.HierarchyWindowItemCallback)
                    Delegate.Combine(EditorApplication.hierarchyWindowItemOnGUI,
                        (EditorApplication.HierarchyWindowItemCallback)DrawHierarchyIcon);
        }

        #endregion

        #region Private Methods

        private static void DrawHierarchyIcon(int instanceID, Rect selectionRect)
        {
            if (OSCEditorTextures.IronWall == null) return;

            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject == null) return;

            var oscBase = gameObject.GetComponent<OSCBase>();
            if (oscBase == null) return;

            var rect = new Rect(selectionRect.x + selectionRect.width - 18f, selectionRect.y, 16f, 16f);
            GUI.DrawTexture(rect, OSCEditorTextures.IronWall);
        }

        #endregion
    }
}