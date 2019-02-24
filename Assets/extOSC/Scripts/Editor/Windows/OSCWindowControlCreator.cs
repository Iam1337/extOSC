/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEditor;
using UnityEngine;

using extOSC.Editor.Panels;

namespace extOSC.Editor.Windows
{
    public class OSCWindowControlCreator : OSCWindow<OSCWindowControlCreator, OSCPanelControlCreator>
    {
        #region Extensions

        public delegate void Callback(ControlData data, MenuCommand menuCommand);

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

        public static void ShowWindow(MenuCommand menuCommand, Callback callback)
        {
            var instance = GetWindow<OSCWindowControlCreator>(false);
            instance.titleContent = new GUIContent("Control Creator", OSCEditorTextures.IronWallSmall);
            instance.minSize = new Vector2(200, 180);

            instance.createCallback = callback;
            instance.menuCommand = menuCommand;
			
            instance.LoadWindowSettings();

            instance.ShowUtility();
            instance.Repaint();
        }


        public static void Invoke(ControlData data)
        {
            var instance = GetWindow<OSCWindowControlCreator>(false);

            if (instance.createCallback != null)
                instance.createCallback(data, instance.menuCommand);

            instance.Close();
        }

        #endregion

        #region Public Vars

        public bool IsValid
        {
            get { return createCallback != null && menuCommand != null; }
        }

        #endregion

        #region Protected Vars

        protected Callback createCallback;

        protected MenuCommand menuCommand;

        #endregion

        #region Private Vars

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
            createCallback = null;
            menuCommand = null;

            SaveWindowSettings();

            base.OnDestroy();
        }

        #endregion

        #region Protected Methods

        protected override void LoadWindowSettings()
        {
            base.LoadWindowSettings();

            rootPanel.ContolColor = OSCEditorSettings.GetColor(_controlColorSettings, Color.white);
            rootPanel.AddInformer = OSCEditorSettings.GetBool(_addInformerSettings, true);
            rootPanel.InformerAddress = OSCEditorSettings.GetString(_informerAddressSettings, "/address");
            rootPanel.InformOnChanged = OSCEditorSettings.GetBool(_informOnChangedSettings, true);
            rootPanel.InformerInterval = OSCEditorSettings.GetFloat(_informerIntervalSettings, 0f);
            rootPanel.InformerTransmitter = OSCEditorSettings.GetTransmitter(_informerTransmitterSettings, null);
        }

        protected override void SaveWindowSettings()
        {
            base.SaveWindowSettings();

            OSCEditorSettings.SetColor(_controlColorSettings, rootPanel.ContolColor);
            OSCEditorSettings.SetBool(_addInformerSettings, rootPanel.AddInformer);
            OSCEditorSettings.GetString(_informerAddressSettings, rootPanel.InformerAddress);
            OSCEditorSettings.GetBool(_informOnChangedSettings, rootPanel.InformOnChanged);
            OSCEditorSettings.GetFloat(_informerIntervalSettings, rootPanel.InformerInterval);
            OSCEditorSettings.SetTransmitter(_informerTransmitterSettings, rootPanel.InformerTransmitter);
        }

        #endregion
    }
}