/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

using extOSC.Core;
using extOSC.Editor.Windows;
using System.Reflection;

namespace extOSC.Editor.Panels
{
    public class OSCPanelContollers : OSCPanel
    {
        #region Private Static Vars

        private static readonly GUIContent _sendActionContent = new GUIContent("Send");

        private static readonly GUIContent _receiveActionContent = new GUIContent("Receive");

        private static readonly GUIContent _selectActionContent = new GUIContent("Select");

        private static readonly GUIContent _transmittersContent = new GUIContent("Transmitters:");

        private static readonly GUIContent _receiversContent = new GUIContent("Receivers:");

        private static readonly GUIContent _actionsContent = new GUIContent("Actions:");

        #endregion

        #region Private Vars

        private Dictionary<string, OSCTransmitter> _transmitters = new Dictionary<string, OSCTransmitter>();

        private Dictionary<string, OSCReceiver> _receivers = new Dictionary<string, OSCReceiver>();

        private Vector2 _scrollPosition;

        private MethodInfo _receiveMethod;

        #endregion

        #region Unity Methods

        protected override void DrawContent(Rect contentRect)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.HelpBox("For component activation in Edit Mode you need to choose appropriate GameObject and have \"Work In Editor\" turned on.", MessageType.Info);

            var expand = contentRect.width > 350;
            if (expand) GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label(_transmittersContent, OSCEditorStyles.ConsoleBoldLabel);

            if (_transmitters.Count > 0)
            {
                foreach (var transmitter in _transmitters)
                {
                    DrawElement(transmitter.Key, transmitter.Value);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Scene doesn't have OSCTransmitter.", MessageType.Info);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label(_receiversContent, OSCEditorStyles.ConsoleBoldLabel);

            if (_receivers.Count > 0)
            {
                foreach (var receiver in _receivers)
                {
                    DrawElement(receiver.Key, receiver.Value);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Scene doesn't have OSCReceiver.", MessageType.Info);
            }

            GUILayout.EndVertical();

            if (expand) GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
        }

        #endregion

        #region Public Methods

        public OSCPanelContollers(OSCWindow parentWindow, string panelId) : base(parentWindow, panelId) 
        { }

        public void Refresh()
        {
            _transmitters.Clear();
            _transmitters = OSCEditorUtils.GetTransmitters();

            _receivers.Clear();
            _receivers = OSCEditorUtils.GetReceivers();
        }

        #endregion

        #region Private Methods

        private void DrawElement(string name, OSCBase osc)
        {
            var defaultColor = GUI.color;
            var elementColor = osc.IsAvaible ? Color.green : Color.red;

            GUI.color = elementColor;

            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label(name);

            GUILayout.BeginVertical("box");
            GUILayout.Label("Active: " + osc.IsAvaible);
            GUILayout.EndVertical();

            GUILayout.Label(_actionsContent);
            GUILayout.BeginHorizontal("box");
            DrawActions(osc, elementColor);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUI.color = defaultColor;
        }

        private void DrawActions(OSCBase osc, Color elementColor)
        {
            GUI.color = Color.yellow;
            GUI.enabled = osc.IsAvaible;

            if (osc is OSCTransmitter) DrawTransmitterActions((OSCTransmitter)osc);
            else if (osc is OSCReceiver) DrawReceiverActions((OSCReceiver)osc);

            GUI.enabled = true;
            GUI.color = Color.white;

            var selectButton = GUILayout.Button(_selectActionContent, GUILayout.MaxWidth(60));
            if (selectButton) OSCEditorUtils.PingObject(osc);

            GUI.color = elementColor;

        }

        private void DrawTransmitterActions(OSCTransmitter transmitter)
        {
            var actionButton = GUILayout.Button(_sendActionContent);
            if (actionButton)
            {
                var debugPacket = OSCWindowDebug.CurrentPacket;
                if (debugPacket != null)
                {
                    transmitter.Send(OSCEditorUtils.CopyPacket(debugPacket));
                }
            }
        }

        private void DrawReceiverActions(OSCReceiver receiver)
        {
            var actionButton = GUILayout.Button(_receiveActionContent);
            if (actionButton)
            {
                var debugPacket = OSCWindowDebug.CurrentPacket;
                if (debugPacket != null)
                {
                    if (_receiveMethod == null)
                        _receiveMethod = typeof(OSCReceiver).GetMethod("PacketReceived", BindingFlags.Instance | BindingFlags.NonPublic);

                    _receiveMethod.Invoke(receiver, new object[] { OSCEditorUtils.CopyPacket(debugPacket) });
                }
            }
        }

        #endregion
    }
}