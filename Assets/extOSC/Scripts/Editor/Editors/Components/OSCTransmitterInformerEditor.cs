/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using extOSC.Components.Informers;
using extOSC.Editor.Drawers;

namespace extOSC.Editor.Components
{
	[CustomEditor(typeof(OSCTransmitterInformer), true)]
	public class OSCTransmitterInformerEditor : OSCTransmitterComponentEditor
	{
		#region Static Private Vars

		private static readonly GUIContent _targetTitleContent = new GUIContent("Target:");

		private static readonly GUIContent _settingsTitleContent = new GUIContent("Informer Settings:");

		private static readonly GUIContent _informOnChangedContent = new GUIContent("Inform on changed");

		private static readonly GUIContent _informIntervalContent = new GUIContent("Send interval:");

		#endregion

		#region Private Vars

		private SerializedProperty _reflectionMemberProperty;

		private SerializedProperty _informOnChangedProperty;

		private SerializedProperty _informIntervalProperty;

		private OSCTransmitterInformer _informer;

		private OSCReflectionMemberDrawer _reflectionDrawer;

		private Color _defaultColor;

		#endregion

		#region Unity Methods

		protected override void OnEnable()
		{
			base.OnEnable();

			_informer = target as OSCTransmitterInformer;

			_targetTitleContent.text = $"Target ({_informer.InformerType.Name}):";

			_reflectionMemberProperty = serializedObject.FindProperty("reflectionMember");
			_informOnChangedProperty = serializedObject.FindProperty("informOnChanged");
			_informIntervalProperty = serializedObject.FindProperty("informInterval");

			// Create reflection member editor.
			_reflectionDrawer = new OSCReflectionMemberDrawer(_reflectionMemberProperty, _informer.InformerType);
			_reflectionDrawer.ReflectionAccess = OSCReflectionAccess.Read;
		}

		#endregion

		#region Public Methods

		#endregion

		#region Protected Methods

		protected override void DrawSettings()
		{
			_defaultColor = GUI.color;

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