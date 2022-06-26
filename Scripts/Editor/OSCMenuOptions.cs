/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

using System;
using System.Reflection;

using extOSC.UI;
using extOSC.Editor.Windows;
using extOSC.Core.Reflection;
using extOSC.Components.Informers;

namespace extOSC.Editor
{
	public static class OSCMenuOptions
	{
		#region Static Private Vars

		private const string _toolsRoot = "Tools/extOSC/";

		private const string _windowsRoot = _toolsRoot;

		private const int _windowsIndex = 0;

		private const string _settingsRoot = _toolsRoot + "Settings/";

		private const string _settingsEncoding = _settingsRoot + "OSCValue.String Encoding/";

		private const string _settingsASCII = _settingsEncoding + "ASCII";

		private const string _settingsUTF8 = _settingsEncoding + "UTF8";

		private const string _settingsDrown = _settingsRoot + "Detect Receiver Drown";

		private const string _encodingDefine = "EXTOSC_UTF8";

		private const string _drownDefine = "EXTOSC_DISABLE_DROWN";

		private const int _settingsIndex = _windowsIndex + 100;

		private const string _linksRoot = _toolsRoot + "Links/";

		private const int _linksIndex = _settingsIndex + 100;

		private const string _objectRoot = "GameObject/extOSC/";

		private const int _objectIndex = 40;

		private static MethodInfo _placeUIElementMethod;

		#endregion

		#region Static Public Methods

		// WINDOWS
		[MenuItem(_windowsRoot + "OSC Console", false, _windowsIndex + 0)]
		public static void ShowConsole()
		{
			OSCWindowConsole.Open();
		}

		[MenuItem(_windowsRoot + "OSC Debug", false, _windowsIndex + 1)]
		public static void ShowDebug()
		{
			OSCWindowDebug.Open();
		}

		[MenuItem(_windowsRoot + "OSC Mapping", false, _windowsIndex + 2)]
		public static void ShowMapping()
		{
			OSCWindowMapping.Open();
		}

		// SETTINGS
		[MenuItem(_settingsASCII, false, _settingsIndex)]
		public static void SettingsSwitchASCII()
		{
			OSCDefinesManager.SetDefine(_encodingDefine, false);
		}

		[MenuItem(_settingsASCII, true)]
		public static bool SettingsSwitchASCIIValidate()
		{
			Menu.SetChecked(_settingsASCII, !OSCDefinesManager.HasDefine(_encodingDefine));
			return true;
		}

		[MenuItem(_settingsUTF8, false, _settingsIndex + 1)]
		public static void SettingsSwitchUTF8()
		{
			OSCDefinesManager.SetDefine(_encodingDefine, true);
		}

		[MenuItem(_settingsUTF8, true)]
		public static bool SettingsSwitchUTF8Validate()
		{
			Menu.SetChecked(_settingsUTF8, OSCDefinesManager.HasDefine(_encodingDefine));
			return true;
		}

		[MenuItem(_settingsDrown, false, _settingsIndex + 2)]
		public static void SettingsDrown()
		{
			OSCDefinesManager.SetDefine(_drownDefine, !OSCDefinesManager.HasDefine(_drownDefine));
		}

		[MenuItem(_settingsDrown, true)]
		public static bool SettingsDrownValidate()
		{
			Menu.SetChecked(_settingsDrown, !OSCDefinesManager.HasDefine(_drownDefine));
			return true;
		}

		// LINKS
		[MenuItem(_linksRoot + "GitHub: Repository", false, _linksIndex)]
		public static void ShowRepository(MenuCommand menuCommand)
		{
			Application.OpenURL("https://github.com/iam1337/extOSC");
		}

		[MenuItem(_linksRoot + "GitHub: Roadmap", false, _linksIndex + 1)]
		public static void ShowRoadmap(MenuCommand menuCommand)
		{
			Application.OpenURL("https://github.com/Iam1337/extOSC/projects/1");
		}

		[MenuItem(_linksRoot + "GitHub: Wiki", false, _linksIndex + 2)]
		public static void ShowWiki(MenuCommand menuCommand)
		{
			Application.OpenURL("https://github.com/Iam1337/extOSC/wiki");
		}

		[MenuItem(_linksRoot + "Unity Forums: Thread", false, _linksIndex + 10)]
		public static void ShowForum(MenuCommand menuCommand)
		{
			Application.OpenURL("https://forum.unity.com/threads/436159/");
		}

		// GAME OBJECTS
		[MenuItem(_objectRoot + "OSC Manager", false, _objectIndex + 1)]
		public static void AddManager(MenuCommand menuCommand)
		{
			var gameObject = OSCControls.CreateManager();
			Undo.RegisterCreatedObjectUndo(gameObject, "Create OSC Manager");
		}

		[MenuItem(_objectRoot + "Pad", false, _objectIndex + 10)]
		public static void AddPad(MenuCommand menuCommand)
		{
			OSCWindowControlCreator.Open(menuCommand, (data, command) => { InitUIElement<OSCPad, OSCTransmitterInformerVector2>(OSCControls.CreatePad, data, command); });
		}

		[MenuItem(_objectRoot + "Slider", false, _objectIndex + 11)]
		public static void AddSlider(MenuCommand menuCommand)
		{
			OSCWindowControlCreator.Open(menuCommand, (data, command) => { InitUIElement<OSCSlider, OSCTransmitterInformerFloat>(OSCControls.CreateSlider, data, command); });
		}

		[MenuItem(_objectRoot + "Button", false, _objectIndex + 12)]
		public static void AddButton(MenuCommand menuCommand)
		{
			OSCWindowControlCreator.Open(menuCommand, (data, command) => { InitUIElement<OSCButton, OSCTransmitterInformerBool>(OSCControls.CreateButton, data, command); });
		}

		[MenuItem(_objectRoot + "Rotary", false, _objectIndex + 13)]
		public static void AddRotary(MenuCommand menuCommand)
		{
			OSCWindowControlCreator.Open(menuCommand, (data, command) => { InitUIElement<OSCRotary, OSCTransmitterInformerFloat>(OSCControls.CreateRotary, data, command); });
		}

		[MenuItem(_objectRoot + "Multiply Sliders (Vertical)", false, _objectIndex + 14)]
		public static void AddMultiplySlidersVertical(MenuCommand menuCommand)
		{
			OSCWindowControlCreator.Open(menuCommand, (data, command) => { InitMultiplySlidersUIElement(OSCControls.CreateMultiplySlidersVertical, data, command); });
		}

		[MenuItem(_objectRoot + "Multiply Sliders (Horizontal)", false, _objectIndex + 15)]
		public static void AddMultiplySlidersHorizontal(MenuCommand menuCommand)
		{
			OSCWindowControlCreator.Open(menuCommand, (data, command) => { InitMultiplySlidersUIElement(OSCControls.CreateMultiplySlidersHorizontal, data, command); });
		}

		#endregion

		#region Static Private Methods

		private static void InitUIElement<T, K>(Func<OSCControls.Resources, GameObject> createAction,
												OSCWindowControlCreator.ControlData data,
												MenuCommand menuCommand) where K : OSCTransmitterInformer where T : Component
		{
			if (createAction == null)
				return;

			var resources = OSCEditorUtils.GetStandardResources();
			resources.Color = data.ControlColor;

			var element = createAction(resources);

			PlaceUIElement(element, menuCommand);

			if (data.UseInformer)
			{
				AddInformer<K>(element.GetComponent<T>(),
							   data.InformerTransmitter,
							   data.InformAddress,
							   data.InformOnChanged,
							   data.InformInterval);
			}
		}

		private static void InitMultiplySlidersUIElement(Func<OSCControls.Resources, GameObject> createAction, OSCWindowControlCreator.ControlData data, MenuCommand menuCommand)
		{
			if (createAction == null)
				return;

			var resources = OSCEditorUtils.GetStandardResources();
			resources.Color = data.ControlColor;

			var element = createAction(resources);

			if (data.UseInformer)
			{
				var multiplySliders = element.GetComponent<OSCMultiplySliders>();
				multiplySliders.Address = data.InformAddress;
				multiplySliders.Transmitter = data.InformerTransmitter;
			}

			PlaceUIElement(element, menuCommand);
		}

		private static void AddInformer<T>(Component component,
										   OSCTransmitter transmitter,
										   string address,
										   bool onChanged,
										   float interval) where T : OSCTransmitterInformer
		{
			var informer = component.gameObject.AddComponent<T>();
			informer.TransmitterAddress = address;
			informer.Transmitter = transmitter;
			informer.InformOnChanged = onChanged;
			informer.InformInterval = interval;

			var reflection = new OSCReflectionMember();
			reflection.Target = component;
			reflection.MemberName = "Value";

			if (!reflection.IsValid())
				reflection.MemberName = "value";

			informer.ReflectionTarget = reflection;
		}

		private static void PlaceUIElement(GameObject gameObject, MenuCommand menuCommand)
		{
			if (_placeUIElementMethod == null)
			{
				var assembly = Assembly.GetAssembly(typeof(SelectableEditor));
				var menuOptionsType = assembly.GetType("UnityEditor.UI.MenuOptions");

				_placeUIElementMethod = menuOptionsType.GetMethod("PlaceUIElementRoot", BindingFlags.Static | BindingFlags.NonPublic);
			}

			_placeUIElementMethod.Invoke(null, new object[] {gameObject, menuCommand});
		}

		#endregion
	}
}