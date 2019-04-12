/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using System.Reflection;
using System.Collections.Generic;

using extOSC.Core;
using extOSC.Editor.Windows;


namespace extOSC.Editor.Panels
{
    public class OSCPanelControllers : OSCPanel
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

	    private Color _defaultColor;

        #endregion

        #region Unity Methods

	    protected override void DrawContent(ref Rect contentRect)
	    {
		    _defaultColor = GUI.color;

		    using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
		    {
			    GUILayout.FlexibleSpace();
		    }

		    using (var scroll = new GUILayout.ScrollViewScope(_scrollPosition))
		    {
				EditorGUILayout.HelpBox("For component activation in Edit Mode you need to choose appropriate GameObject and have \"Work In Editor\" turned on.",
				    MessageType.Info);

			    var expand = contentRect.width > 350;
			    if (expand)
				    GUILayout.BeginHorizontal();

			    using (new GUILayout.VerticalScope())
			    {
				    GUILayout.Label(_transmittersContent, OSCEditorStyles.ConsoleBoldLabel);

				    if (_transmitters.Count > 0)
				    {
					    foreach (var transmitter in _transmitters)
					    {
						    DrawElement(transmitter.Value);
					    }
				    }
				    else
				    {
					    EditorGUILayout.HelpBox("Scene doesn't have OSCTransmitter.", MessageType.Info);
				    }
			    }

			    using (new GUILayout.VerticalScope())
			    {
				    GUILayout.Label(_receiversContent, OSCEditorStyles.ConsoleBoldLabel);

				    if (_receivers.Count > 0)
				    {
					    foreach (var receiver in _receivers)
					    {
						    DrawElement(receiver.Value);
					    }
				    }
				    else
				    {
					    EditorGUILayout.HelpBox("Scene doesn't have OSCReceiver.", MessageType.Info);
				    }
			    }

			    if (expand)
				    GUILayout.EndHorizontal();

			    _scrollPosition = scroll.scrollPosition;
		    }
	    }

	    #endregion

        #region Public Methods

        public OSCPanelControllers(OSCWindow window, string panelId) : base(window, panelId) 
        { }

        public void Refresh()
        {
			// TODO: Replace Get methods.
            _transmitters.Clear();
            _transmitters = OSCEditorUtils.GetTransmitters();

            _receivers.Clear();
            _receivers = OSCEditorUtils.GetReceivers();
        }

        #endregion

        #region Private Methods

        private void DrawElement(OSCBase osc)
        {
            GUI.color = osc.IsAvailable ? Color.green : Color.red;
			using (new GUILayout.VerticalScope(EditorStyles.helpBox))
	        {
				DrawName(osc);
		        using (new GUILayout.HorizontalScope(OSCEditorStyles.Box))
		        {
			        GUILayout.Label("Active: " + osc.IsAvailable);
		        }

		        GUILayout.Label(_actionsContent);
		        using (new GUILayout.HorizontalScope(OSCEditorStyles.Box))
		        {
			        DrawActions(osc);
		        }
	        }

	        GUI.color = _defaultColor;
        }

	    private void DrawName(OSCBase osc)
	    {
		    var transmitter = osc as OSCTransmitter;
		    if (transmitter != null)
		    {
			    GUILayout.Label(string.Format("Transmitter: {0}:{1}", transmitter.RemoteHost, transmitter.RemotePort));
            }

			var receiver = osc as OSCReceiver;
		    if (receiver != null)
		    {
				GUILayout.Label(string.Format("Receiver: {0}", receiver.LocalPort));
		    }
	    }

        private void DrawActions(OSCBase osc)
        {
            GUI.color = Color.yellow;
            GUI.enabled = osc.IsAvailable;

	        var transmitter = osc as OSCTransmitter;
	        if (transmitter != null)
	        {
		        if (GUILayout.Button(_sendActionContent))
		        {
			        var debugPacket = OSCWindowDebug.CurrentPacket;
			        if (debugPacket != null)
			        {
				        transmitter.Send(OSCEditorUtils.CopyPacket(debugPacket));
			        }
		        }
            }

	        var receiver = osc as OSCReceiver;
	        if (receiver != null)
	        {
		        if (GUILayout.Button(_receiveActionContent))
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

            GUI.enabled = true;
            GUI.color = _defaultColor;

            var selectButton = GUILayout.Button(_selectActionContent, GUILayout.MaxWidth(60));
            if (selectButton) OSCEditorUtils.PingObject(osc);
        }

        #endregion
    }
}