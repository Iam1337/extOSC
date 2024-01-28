/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEditor;
using UnityEngine;

using extOSC.Core;

namespace extOSC.Editor
{
	[InitializeOnLoad]
	public static class OSCHierarchyIcon
	{
		#region Constructor Methods

		static OSCHierarchyIcon()
		{
			EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyIcon;
		}

		#endregion

		#region Private Methods

		private static void DrawHierarchyIcon(int instanceId, Rect selectionRect)
		{
			if (OSCEditorTextures.IronWall == null) return;

			var gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
			if (gameObject == null) return;

			var oscBase = gameObject.GetComponent<OSCBase>();
			if (oscBase == null) return;

			var rect = new Rect(selectionRect.x + selectionRect.width - 18f, selectionRect.y, 16f, 16f);
			GUI.DrawTexture(rect, OSCEditorTextures.IronWallSmall);
		}

		#endregion
	}
}