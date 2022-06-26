/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEditor;

using extOSC.Components.Ping;

namespace extOSC.Editor.Components
{
	[CustomEditor(typeof(OSCPingServer), true)]
	public class OSCPingServerEditor : OSCComponentEditor
	{
		#region Static Private Vars

		private static readonly GUIContent _transmitterContent = new GUIContent("OSC Transmitter:");

		private static readonly GUIContent _transmitterAddressContent = new GUIContent("OSC Transmitter Address:");

		private static readonly GUIContent _transmitterAddressContentSmall = new GUIContent("Transmitter Address:");

		private static readonly GUIContent _transmitterComponentSettingsContent = new GUIContent("Transmitter Settings:");

		private static readonly GUIContent _receiverComponentSettingsContent = new GUIContent("Receiver Settings:");

		#endregion

		#region Private Vars

		private SerializedProperty _transmitterProperty;

		private SerializedProperty _receiverProperty;

		private SerializedProperty _receiverAddressProperty;

		private OSCPingServer _ping;

		#endregion

		#region Unity Methods

		protected override void OnEnable()
		{
			_ping = target as OSCPingServer;

			_transmitterProperty = serializedObject.FindProperty("_transmitter");
			_receiverProperty = serializedObject.FindProperty("_receiver");
			_receiverAddressProperty = serializedObject.FindProperty("_receiverAddress");
		}

		protected override void OnDisable()
		{ }

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUI.BeginChangeCheck();

			// LOGO
			OSCEditorInterface.LogoLayout();

			EditorGUILayout.LabelField($"{target.GetType().Name} Settings:", EditorStyles.boldLabel);
			using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
			{
				EditorGUILayout.LabelField(_receiverComponentSettingsContent, EditorStyles.boldLabel);
				OSCEditorInterface.ReceiverSettings(_receiverProperty, _receiverAddressProperty);

				EditorGUILayout.LabelField(_transmitterComponentSettingsContent, EditorStyles.boldLabel);
				using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
				{
					EditorGUILayout.PropertyField(_transmitterProperty, _transmitterContent);

					var transmitterAddress = "- None -";

					if (Application.isPlaying)
					{
						transmitterAddress = _ping.TransmitterAddress;
					}

					EditorGUILayout.LabelField(EditorGUIUtility.currentViewWidth > 410
												   ? _transmitterAddressContent.text
												   : _transmitterAddressContentSmall.text,
											   transmitterAddress);
				}

				DrawSettings();
			}

			if (EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}

		#endregion

		#region Protected Methods

		protected override void DrawSettings()
		{ }

		#endregion
	}
}