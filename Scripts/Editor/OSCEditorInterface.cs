/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using System.Linq;

using extOSC.Core;

namespace extOSC.Editor
{
	public static class OSCEditorInterface
	{
		#region Static Private Vars

		private static readonly GUIContent _transmitterAddressContent = new GUIContent("OSC Transmitter Address:");

		private static readonly GUIContent _transmitterAddressContentSmall = new GUIContent("OSC Address:");

		private static readonly GUIContent _receiverAddressContent = new GUIContent("OSC Receiver Address:");

		private static readonly GUIContent _receiverAddressContentSmall = new GUIContent("OSC Address:");

		private static readonly GUIContent _transmitterContent = new GUIContent("OSC Transmitter:");

		private static readonly GUIContent _receiverContent = new GUIContent("OSC Receiver:");

		private static readonly GUIContent _emptyContent = new GUIContent("- Empty -");

		#endregion

		#region Static Public Methods

		public static void LogoLayout()
		{
			if (OSCEditorTextures.IronWall == null)
				return;

			GUILayout.Space(10);
			EditorGUILayout.Space();

			var rect = GUILayoutUtility.GetRect(0, 0);
			var width = OSCEditorTextures.IronWall.width * 0.2f;
			var height = OSCEditorTextures.IronWall.height * 0.2f;

			rect.x = rect.width * 0.5f - width * 0.5f;
			rect.y = rect.y + rect.height * 0.5f - height * 0.5f;
			rect.width = width;
			rect.height = height;

			GUI.DrawTexture(rect, OSCEditorTextures.IronWall);
			EditorGUILayout.Space();
			GUILayout.Space(5);
		}

		public static void DrawProperties(SerializedObject serializedObject, params string[] exceptionsNames)
		{
			var serializedProperty = serializedObject.GetIterator();
			var isEmpty = true;
			var enterChild = true;

			while (serializedProperty.NextVisible(enterChild))
			{
				enterChild = false;

				if (serializedProperty.name == "m_Script" ||
					exceptionsNames.Contains(serializedProperty.name))
					continue;

				EditorGUILayout.PropertyField(serializedProperty, true);

				isEmpty = false;
			}

			if (isEmpty)
				EditorGUILayout.LabelField(_emptyContent, OSCEditorStyles.CenterLabel);
		}

		public static void TransmitterSettings(SerializedProperty property, SerializedProperty addressProperty, bool drawBox = true)
		{
			if (drawBox) GUILayout.BeginVertical(OSCEditorStyles.Box);

			var addressContent = EditorGUIUtility.currentViewWidth > 410
				? _transmitterAddressContent
				: _transmitterAddressContentSmall;

			EditorGUILayout.PropertyField(addressProperty, addressContent);
			EditorGUILayout.PropertyField(property, _transmitterContent);

			if (drawBox) EditorGUILayout.EndVertical();
		}

		public static void ReceiverSettings(SerializedProperty property, SerializedProperty addressProperty, bool drawBox = true)
		{
			if (drawBox) GUILayout.BeginVertical(OSCEditorStyles.Box);

			var addressContent = EditorGUIUtility.currentViewWidth > 380
				? _receiverAddressContent
				: _receiverAddressContentSmall;

			EditorGUILayout.PropertyField(addressProperty, addressContent);
			EditorGUILayout.PropertyField(property, _receiverContent);

			if (drawBox) EditorGUILayout.EndVertical();
		}

		// POPUP
		public static TOSC Popup<TOSC>(Rect position,
									   GUIContent label,
									   TOSC currentObject,
									   GUIContent[] content,
									   TOSC[] objects) where TOSC : OSCBase
		{
			return objects[EditorGUI.Popup(position, label, Mathf.Max(objects.IndexOf(currentObject), 0), content)];
		}

		public static TOSC PopupLayout<TOSC>(GUIContent label,
											 TOSC currentObject,
											 GUIContent[] content,
											 TOSC[] objects) where TOSC : OSCBase
		{
			return objects[EditorGUILayout.Popup(label, Mathf.Max(objects.IndexOf(currentObject), 0), content)];
		}

		#endregion
	}
}