/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEditor;
using UnityEngine;

using System;

using extOSC.Editor.Panels;

namespace extOSC.Editor.Windows
{
	public class OSCWindowControlCreator : OSCWindow<OSCWindowControlCreator, OSCPanelControlCreator>
	{
		#region Extensions

		public struct ControlData
		{
			public Color ControlColor;

			public bool UseInformer;

			public string InformAddress;

			public bool InformOnChanged;

			public float InformInterval;

			public OSCTransmitter InformerTransmitter;
		}

		#endregion

		#region Static Public Methods

		public static void Open(MenuCommand menuCommand, Action<ControlData, MenuCommand> callback)
		{
			var instance = GetWindow<OSCWindowControlCreator>(false);
			instance.titleContent = new GUIContent("Control Creator", OSCEditorTextures.IronWallSmall);
			instance.minSize = new Vector2(200, 180);

			instance._createCallback = callback;
			instance._menuCommand = menuCommand;

			instance.LoadWindowSettings();

			instance.ShowUtility();
			instance.Repaint();
			Instance.Focus();
		}


		public static void CreateControl(ControlData data)
		{
			var instance = GetWindow<OSCWindowControlCreator>(false);
			if (instance._createCallback != null)
				instance._createCallback(data, instance._menuCommand);

			instance.Close();
		}

		#endregion

		#region Public Vars

		public bool IsValid => _createCallback != null && _menuCommand != null;

		#endregion

		#region Private Vars

		private MenuCommand _menuCommand;

		private Action<ControlData, MenuCommand> _createCallback;

		private readonly string _controlColorSettings = OSCEditorSettings.ControlCreator + "controlcolor";

		private readonly string _addInformerSettings = OSCEditorSettings.ControlCreator + "addinformer";

		private readonly string _informerAddressSettings = OSCEditorSettings.ControlCreator + "informeraddress";

		private readonly string _informOnChangedSettings = OSCEditorSettings.ControlCreator + "onchanged";

		private readonly string _informerIntervalSettings = OSCEditorSettings.ControlCreator + "informerinterval";

		private readonly string _informerTransmitterSettings = OSCEditorSettings.ControlCreator + "informertransmitter";

		#endregion

		#region Unity Methods

		protected override void OnDestroy()
		{
			_createCallback = null;
			_menuCommand = null;

			SaveWindowSettings();

			base.OnDestroy();
		}

		#endregion

		#region Protected Methods

		protected override void LoadWindowSettings()
		{
			base.LoadWindowSettings();

			rootPanel.ControlColor = OSCEditorSettings.GetColor(_controlColorSettings, Color.white);
			rootPanel.AddInformer = OSCEditorSettings.GetBool(_addInformerSettings, true);
			rootPanel.InformerAddress = OSCEditorSettings.GetString(_informerAddressSettings, "/address");
			rootPanel.InformOnChanged = OSCEditorSettings.GetBool(_informOnChangedSettings, true);
			rootPanel.InformerInterval = OSCEditorSettings.GetFloat(_informerIntervalSettings, 0f);
			rootPanel.InformerTransmitter = OSCEditorSettings.GetTransmitter(_informerTransmitterSettings, null);
		}

		protected override void SaveWindowSettings()
		{
			base.SaveWindowSettings();

			OSCEditorSettings.SetColor(_controlColorSettings, rootPanel.ControlColor);
			OSCEditorSettings.SetBool(_addInformerSettings, rootPanel.AddInformer);
			OSCEditorSettings.GetString(_informerAddressSettings, rootPanel.InformerAddress);
			OSCEditorSettings.GetBool(_informOnChangedSettings, rootPanel.InformOnChanged);
			OSCEditorSettings.GetFloat(_informerIntervalSettings, rootPanel.InformerInterval);
			OSCEditorSettings.SetTransmitter(_informerTransmitterSettings, rootPanel.InformerTransmitter);
		}

		#endregion
	}
}