/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEditor;
using UnityEngine;

using extOSC.Components.Misc;
using extOSC.Editor.Drawers;

namespace extOSC.Editor.Components
{
	[CustomEditor(typeof(OSCTransmitterSeparateVector3), true)]
	public class OSCTransmitterSeparateVector3Editor : UnityEditor.Editor
	{
		#region Static Private Vars

		private static readonly GUIContent _targetTitleContent = new GUIContent("Target:");

		private static readonly GUIContent _settingsTitleContent = new GUIContent("Informer Settings:");

		private static readonly GUIContent _informOnChangedContent = new GUIContent("Inform on changed");

		private static readonly GUIContent _informIntervalContent = new GUIContent("Send interval:");

		#endregion

		#region Static Private Vars

		private static readonly GUIContent _transmitterComponentSettingsContent = new GUIContent("Transmitter Component Settings:");

		private static readonly GUIContent _otherSettingsContent = new GUIContent("Other Settings:");

		private static readonly GUIContent _mapBundleContent = new GUIContent("Map Bundle:");

		#endregion

		#region Private Vars

		private OSCTransmitterSeparateVector3 _transmitterComponent;

		private SerializedProperty _transmitterProperty;

		private SerializedProperty _mapBundleProperty;

		private SerializedProperty _reflectionMemberProperty;

		private SerializedProperty _informOnChangedProperty;

		private SerializedProperty _informIntervalProperty;

		private SerializedProperty _addressXProperty;

		private SerializedProperty _addressYProperty;

		private SerializedProperty _addressZProperty;
		
		private OSCReflectionMemberDrawer _reflectionDrawer;

		#endregion

		#region Unity Methods

		protected virtual void OnEnable()
		{
			_transmitterComponent = target as OSCTransmitterSeparateVector3;

			_transmitterProperty = serializedObject.FindProperty("_transmitter");
			_mapBundleProperty = serializedObject.FindProperty("_mapBundle");

			_reflectionMemberProperty = serializedObject.FindProperty("_reflectionMember");
			_informOnChangedProperty = serializedObject.FindProperty("_informOnChanged");
			_informIntervalProperty = serializedObject.FindProperty("_informInterval");

			_addressXProperty = serializedObject.FindProperty("AddressX");
			_addressYProperty = serializedObject.FindProperty("AddressY");
			_addressZProperty = serializedObject.FindProperty("AddressZ");

			// Create reflection member editor.
			_reflectionDrawer = new OSCReflectionMemberDrawer(_reflectionMemberProperty, typeof(Vector3));
			_reflectionDrawer.ReflectionAccess = OSCReflectionAccess.Read;
		}

		protected virtual void OnDisable()
		{ }

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUI.BeginChangeCheck();

			// LOGO
			OSCEditorInterface.LogoLayout();

			using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
			{
				EditorGUILayout.LabelField(_transmitterComponentSettingsContent, EditorStyles.boldLabel);
				using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
				{
					//OSCEditorInterface.TransmitterSettings(_transmitterProperty, _addressProperty, false);
					EditorGUILayout.PropertyField(_transmitterProperty, new GUIContent("OSC Transmitter:"));
					EditorGUILayout.PropertyField(_addressXProperty);
					EditorGUILayout.PropertyField(_addressYProperty);
					EditorGUILayout.PropertyField(_addressZProperty);

					EditorGUILayout.PropertyField(_mapBundleProperty, _mapBundleContent);

					if (_transmitterComponent.Transmitter != null &&
						_transmitterComponent.Transmitter.MapBundle != null &&
						_transmitterComponent.MapBundle != null)
					{
						EditorGUILayout.HelpBox("OSCTransmitter already has MapBundle.", MessageType.Info);
					}
				}

				DrawSettings();
			}

			if (EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}

		#endregion

		#region Protected Methods

		protected virtual void DrawSettings()
		{
			var _defaultColor = GUI.color;

			// TARGET
			EditorGUILayout.LabelField(_targetTitleContent, EditorStyles.boldLabel);
			using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
			{
				_reflectionDrawer.DrawLayout();
			}

			//SETTINGS
			EditorGUILayout.LabelField(_settingsTitleContent, EditorStyles.boldLabel);
			using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
			{
				GUI.color = _informOnChangedProperty.boolValue ? Color.green : Color.red;
				if (GUILayout.Button(_informOnChangedContent))
				{
					_informOnChangedProperty.boolValue = !_informOnChangedProperty.boolValue;
				}

				GUI.color = _defaultColor;

				if (!_informOnChangedProperty.boolValue)
				{
					EditorGUILayout.PropertyField(_informIntervalProperty, _informIntervalContent);

					if (_informIntervalProperty.floatValue < 0)
						_informIntervalProperty.floatValue = 0;

					EditorGUILayout.HelpBox("Set to 0 for send message with each frame.", MessageType.Info);
				}
			}
		}

		#endregion
	}
}