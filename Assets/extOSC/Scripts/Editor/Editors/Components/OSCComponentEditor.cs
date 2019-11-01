/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using extOSC.Components;

namespace extOSC.Editor.Components
{
	[CustomEditor(typeof(OSCComponent), true)]
	public class OSCComponentEditor : UnityEditor.Editor
	{
		#region Static Private Vars

		private static readonly GUIContent _transmitterComponentSettingsContent = new GUIContent("Transmitter Settings:");

		private static readonly GUIContent _receiverComponentSettingsContent = new GUIContent("Receiver Settings:");

		private static readonly GUIContent _otherSettingsContent = new GUIContent("Other Settings:");

		private static readonly GUIContent _settingsTitleContent = new GUIContent("Settings:");

		#endregion

		#region Private Vars

		private SerializedProperty _transmitterProperty;

		private SerializedProperty _transmitterAddressProperty;

		private SerializedProperty _receiverProperty;

		private SerializedProperty _receiverAddressProperty;

		#endregion

		#region Unity Methods

		protected virtual void OnEnable()
		{
			_transmitterProperty = serializedObject.FindProperty("_transmitter");
			_transmitterAddressProperty = serializedObject.FindProperty("_transmitterAddress");
			_receiverProperty = serializedObject.FindProperty("_receiver");
			_receiverAddressProperty = serializedObject.FindProperty("_receiverAddress");
			_settingsTitleContent.text = $"{target.GetType().Name} Settings:";
		}

		protected virtual void OnDisable()
		{ }

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUI.BeginChangeCheck();

			// LOGO
			OSCEditorInterface.LogoLayout();

			EditorGUILayout.LabelField(_settingsTitleContent, EditorStyles.boldLabel);
			using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
			{
				EditorGUILayout.LabelField(_transmitterComponentSettingsContent, EditorStyles.boldLabel);
				OSCEditorInterface.TransmitterSettings(_transmitterProperty, _transmitterAddressProperty);

				EditorGUILayout.LabelField(_receiverComponentSettingsContent, EditorStyles.boldLabel);
				OSCEditorInterface.ReceiverSettings(_receiverProperty, _receiverAddressProperty);

				DrawSettings();
			}

			if (EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}

		#endregion

		#region Protected Methods

		protected virtual void DrawSettings()
		{
			// CUSTOM SETTINGS
			EditorGUILayout.LabelField(_otherSettingsContent, EditorStyles.boldLabel);
			using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
			{
				OSCEditorInterface.DrawProperties(serializedObject, _transmitterAddressProperty.name,
												  _transmitterProperty.name, _receiverAddressProperty.name,
												  _receiverProperty.name);
			}
		}

		#endregion
	}
}