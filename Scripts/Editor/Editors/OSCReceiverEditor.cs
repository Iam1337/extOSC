﻿/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using System.Reflection;

namespace extOSC.Editor
{
    [CustomEditor(typeof(OSCReceiver))]
	public class OSCReceiverEditor : UnityEditor.Editor
    {
        #region Static Private Vars

        private static readonly GUIContent _portContent = new GUIContent("Local Port:");

        private static readonly GUIContent _hostContent = new GUIContent("Local Host:");

        private static readonly GUIContent _hostModeContent = new GUIContent("Local Host Mode:");

        private static readonly GUIContent _advancedContent = new GUIContent("Advanced Settings:");

        private static readonly GUIContent _mapBundleContent = new GUIContent("Map Bundle:");

        private static readonly GUIContent _inGameContent = new GUIContent("In Game Controls:");

        private static readonly GUIContent _inEditorContent = new GUIContent("In Editor Controls:");

	    private static readonly GUIContent _receiverSettingsContent = new GUIContent("Receiver Settings:");

        private static readonly GUIContent _autoConnectContent = new GUIContent("Auto Connect");

	    private static readonly GUIContent _closeOnPauseContent = new GUIContent("Close On Pause");

        private static string _advancedHelp = "Currently \"Advanced settings\" are not available for UWP (WSA).";

        private static MethodInfo _updateMethod;

        #endregion

        #region Private Vars

        private SerializedProperty _localHostModeProperty;

        private SerializedProperty _localHostProperty;

        private SerializedProperty _localPortProperty;

        private SerializedProperty _autoConnectProperty;

        private SerializedProperty _workInEditorProperty;

        private SerializedProperty _mapBundleProperty;

        private SerializedProperty _closeOnPauseProperty;

        private OSCReceiver _receiver;

        private string _localHostCache;

	    private Color _defaultColor;

        #endregion

        #region Unity Methods

        protected void OnEnable()
        {
            _receiver = target as OSCReceiver;
            _localHostCache = OSCUtilities.GetLocalHost();

            _localHostModeProperty = serializedObject.FindProperty("localHostMode");
            _localHostProperty = serializedObject.FindProperty("localHost");
            _localPortProperty = serializedObject.FindProperty("localPort");
            _autoConnectProperty = serializedObject.FindProperty("autoConnect");
            _workInEditorProperty = serializedObject.FindProperty("workInEditor");
            _mapBundleProperty = serializedObject.FindProperty("mapBundle");
            _closeOnPauseProperty = serializedObject.FindProperty("closeOnPause");

            EditorApplication.update += ReceiverEditorUpdate;

            if (!Application.isPlaying && !_receiver.IsAvailable && _workInEditorProperty.boolValue)
            {
                _receiver.Connect();
            }
        }

        protected void OnDisable()
        {
            if (_receiver == null)
                _receiver = target as OSCReceiver;

            EditorApplication.update -= ReceiverEditorUpdate;

            if (!Application.isPlaying && _receiver.IsAvailable)
            {
                _receiver.Close();
            }
        }

        public override void OnInspectorGUI()
        {
	        _defaultColor = GUI.color;

            serializedObject.Update();

			EditorGUI.BeginChangeCheck();

	        // LOGO
	        OSCEditorInterface.LogoLayout();

			// INSPECTOR
            EditorGUILayout.LabelField("Active: " + _receiver.IsAvailable, EditorStyles.boldLabel);
            using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
	        {
				// SETTINGS BLOCK
		        EditorGUILayout.LabelField(_receiverSettingsContent, EditorStyles.boldLabel);
		        using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
		        {
			        if (_receiver.LocalHostMode == OSCLocalHostMode.Any)
			        {
				        using (new GUILayout.HorizontalScope())
				        {
					        EditorGUILayout.LabelField(_hostContent, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
					        EditorGUILayout.SelectableLabel(_localHostCache,
					                                        GUILayout.Height(EditorGUIUtility.singleLineHeight));
				        }
			        }
			        else
			        {
				        EditorGUILayout.PropertyField(_localHostProperty, _hostContent);
			        }

			        EditorGUILayout.PropertyField(_localPortProperty, _portContent);
			        EditorGUILayout.PropertyField(_mapBundleProperty, _mapBundleContent);
		        }

		        // PARAMETERS BLOCK
		        using (new GUILayout.HorizontalScope(OSCEditorStyles.Box))
		        {
			        GUI.color = _autoConnectProperty.boolValue ? Color.green : Color.red;
			        if (GUILayout.Button(_autoConnectContent))
			        {
				        _autoConnectProperty.boolValue = !_autoConnectProperty.boolValue;
			        }

			        GUI.color = _closeOnPauseProperty.boolValue ? Color.green : Color.red;
			        if (GUILayout.Button(_closeOnPauseContent))
			        {
				        _closeOnPauseProperty.boolValue = !_closeOnPauseProperty.boolValue;
			        }

			        GUI.color = _defaultColor;
		        }

		        // ADVANCED BLOCK
		        EditorGUILayout.LabelField(_advancedContent, EditorStyles.boldLabel);
		        using (new GUILayout.VerticalScope(OSCEditorStyles.Box))
		        {
			        if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.WSA)
			        {
				        GUI.color = Color.yellow;
				        EditorGUILayout.HelpBox(_advancedHelp, MessageType.Info);
				        GUI.color = _defaultColor;
			        }

			        EditorGUILayout.PropertyField(_localHostModeProperty, _hostModeContent);
		        }

		        // CONTROLS
		        EditorGUILayout.LabelField(Application.isPlaying ? _inGameContent : _inEditorContent, EditorStyles.boldLabel);
				using (new GUILayout.HorizontalScope(OSCEditorStyles.Box))
		        {
			        if (Application.isPlaying) InGameControls();
			        else InEditorControls();
		        }
	        }

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }


		#endregion

		#region Private Methods

	    protected void InGameControls()
	    {
			GUI.color = _receiver.IsAvailable ? Color.green : Color.red;
		    if (GUILayout.Button(_receiver.IsAvailable ? "Connected" : "Disconnected"))
		    {
			    if (_receiver.IsAvailable) _receiver.Close();
			    else _receiver.Connect();
		    }
			
			GUI.color = Color.yellow;
		    GUI.enabled = !_receiver.IsAvailable;
			if (GUILayout.Button("Reconnect"))
		    {
			    if (_receiver.IsAvailable) _receiver.Close();

			    _receiver.Connect();
		    }

		    GUI.enabled = true;
	    }

	    protected void InEditorControls()
        {
            GUI.color = _workInEditorProperty.boolValue ? Color.green : Color.red;
			if (GUILayout.Button("Work In Editor"))
			{
				_workInEditorProperty.boolValue = !_workInEditorProperty.boolValue;

				if (_workInEditorProperty.boolValue)
				{
					if (_receiver.IsAvailable)
						_receiver.Close();

					_receiver.Connect();
				}
				else
				{
					if (_receiver.IsAvailable)
						_receiver.Close();
				}
			}

            GUI.color = Color.yellow;
	        GUI.enabled = !_workInEditorProperty.boolValue;
			if (GUILayout.Button("Reconnect"))
	        {
		        if (_workInEditorProperty.boolValue)
		        {
					if (_receiver.IsAvailable)
						_receiver.Close();

					_receiver.Connect();
		        }
	        }

	        GUI.enabled = true;
        }

        protected void ReceiverEditorUpdate()
        {
            if (_updateMethod == null)
                _updateMethod = typeof(OSCReceiver).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic);

            if (_receiver != null && _updateMethod != null)
                _updateMethod.Invoke(_receiver, null);
        }

		#endregion

	}
}