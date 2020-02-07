/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEditor;
using UnityEngine;

using System;


namespace extOSC.Editor.Drawers
{
	[CustomPropertyDrawer(typeof(OSCSelectorAttribute))]
	public class OSCSelectorDrawer : PropertyDrawer
	{
		#region Static Private Vars

		private static readonly Type _transmitterType = typeof(OSCTransmitter);

		private static readonly Type _receiverType = typeof(OSCReceiver);

		#endregion

		#region Public Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.ObjectReference)
			{
				return;
			}

			var fieldType = fieldInfo.FieldType;
			if (fieldType == _transmitterType)
			{
				OSCEditorUtils.FindObjects(TransmitterCallback, true, out var content, out OSCTransmitter[] objects);

				property.objectReferenceValue = OSCEditorInterface.Popup(position, label,
																		 (OSCTransmitter) property.objectReferenceValue,
																		 content,
																		 objects);
			}
			else if (fieldType == _receiverType)
			{
				OSCEditorUtils.FindObjects(ReceiverCallback, true, out var content, out OSCReceiver[] objects);

				property.objectReferenceValue = OSCEditorInterface.Popup(position, label,
																		 (OSCReceiver) property.objectReferenceValue,
																		 content,
																		 objects);
			}
		}

		#endregion

		#region Private Methods

		private string TransmitterCallback(OSCTransmitter transmitter)
		{
			return $"Transmitter: {transmitter.RemoteHost}:{transmitter.RemotePort}";
		}

		private string ReceiverCallback(OSCReceiver receiver)
		{
			return $"Receiver: {receiver.LocalPort}";
		}

		#endregion
	}
}