/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using extOSC.Editor.Windows;

namespace extOSC.Editor.Panels
{
    public class OSCPanelControlCreator : OSCPanel
    {
        #region Static Private Vars

        private static readonly GUIContent _oscAddressContent = new GUIContent("OSC Address:");

        private static readonly GUIContent _oscTransmitterContent = new GUIContent("OSC Transmitter:");

        private static readonly GUIContent _informerIntervalContent = new GUIContent("Informer Interval:");

        private static readonly GUIContent _addInformerContent = new GUIContent("Add Informer");

        private static readonly GUIContent _controlSettingsContent = new GUIContent("Create New Control:");

        private static readonly GUIContent _controlColorContent = new GUIContent("Control Color:");

        private static readonly GUIContent _informOnChangedContent = new GUIContent("Inform On Changed");

        private static readonly GUIContent _informerSettingsContent = new GUIContent("Informer Settings:");

        private static readonly GUIContent _createContent = new GUIContent("Create");

        private static readonly GUIContent _errorCreateContent = new GUIContent("Create object again.");

        #endregion

        #region Public Vars

        public Color ContolColor
        {
            get { return _controlColor; }
            set { _controlColor = value; }
        }

        public bool AddInformer
        {
            get { return _addInformer; }
            set { _addInformer = value; }
        }

        public string InformerAddress
        {
            get { return _informerAddress; }
            set { _informerAddress = value; }
        }

        public bool InformOnChanged
        {
            get { return _informOnChanged; }
            set { _informOnChanged = value; }
        }

        public float InformerInterval
        {
            get { return _informerInterval; }
            set { _informerInterval = value; }
        }

        public OSCTransmitter InformerTransmitter
        {
            get { return _informerTransmitter; }
            set { _informerTransmitter = value; }
        }

        #endregion

        #region Private Vars

        private Color _controlColor;

        private bool _addInformer;

        private string _informerAddress;

        private bool _informOnChanged;

        private float _informerInterval;

        private OSCTransmitter _informerTransmitter;

        private OSCWindowControlCreator _controlCreator;

        #endregion

        #region Public Methods

        public OSCPanelControlCreator(OSCWindow window, string panelId) : base(window, panelId)
        {
            _controlCreator = window as OSCWindowControlCreator;
        }

        #endregion

        #region Protected Methods

        protected override void DrawContent(ref Rect contentRect)
        {
            if (!_controlCreator.IsValid)
            {
                EditorGUILayout.LabelField(_errorCreateContent, OSCEditorStyles.CenterLabel, GUILayout.Height(contentRect.height));

                return;
            }

            contentRect.x += 2;
            contentRect.y += 2;
            contentRect.width -= 4;
            contentRect.height -= 4;

            GUILayout.BeginArea(contentRect);

			OSCEditorInterface.LogoLayout();

            GUILayout.Label(_controlSettingsContent, EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(OSCEditorStyles.Box);

            _controlColor = EditorGUILayout.ColorField(_controlColorContent, _controlColor);

            EditorGUILayout.EndVertical();

            GUI.color = _addInformer ? Color.green : Color.red;
            if (GUILayout.Button(_addInformerContent))
            {
                _addInformer = !_addInformer;
            }
            GUI.color = Color.white;

            if (_addInformer)
            {
                GUILayout.Label(_informerSettingsContent, EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(OSCEditorStyles.Box);

	            var content = (GUIContent[])null;
	            var objects = (OSCTransmitter[])null;

	            OSCEditorUtils.FindObjects(TransmitterCallback, true, out content, out objects);

                _informerAddress = EditorGUILayout.TextField(_oscAddressContent, _informerAddress);
	            _informerTransmitter = OSCEditorInterface.PopupLayout(_oscTransmitterContent,
	                                                               _informerTransmitter,
	                                                               content,
	                                                               objects);

                GUI.color = _informOnChanged ? Color.green : Color.red;
                if (GUILayout.Button(_informOnChangedContent))
                {
                    _informOnChanged = !_informOnChanged;
                }
                GUI.color = Color.white;

                if (!_informOnChanged)
                {
                    _informerInterval = EditorGUILayout.FloatField(_informerIntervalContent, _informerInterval);

                    if (_informerInterval < 0) _informerInterval = 0;

                    EditorGUILayout.HelpBox("Set to 0 for send message with each frame.", MessageType.Info);
                }

                EditorGUILayout.EndVertical();
            }

            GUI.color = Color.green;
            if (GUILayout.Button(_createContent))
            {
                var data = new OSCWindowControlCreator.ControlData();
                data.ControlColor = _controlColor;
                data.UseInformer = _addInformer;
                data.InformAddress = _informerAddress;
                data.InformInterval = _informerInterval;
                data.InformOnChanged = _informOnChanged;
                data.InformerTransmitter = _informerTransmitter;

                OSCWindowControlCreator.CreateControl(data);
            }
            GUI.color = Color.white;

            GUILayout.EndArea();
        }

        #endregion

        #region Private Methods

	    private string TransmitterCallback(OSCTransmitter transmitter)
	    {
		    return string.Format("Transmitter: {0}:{1}", transmitter.RemoteHost, transmitter.RemotePort);
	    }

        #endregion
    }
}