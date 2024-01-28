/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;

using extOSC.UI;
using extOSC.Core;

using Object = UnityEngine.Object;

namespace extOSC.Editor
{
	public static class OSCEditorUtils
	{
		#region Static Public Vars

		public static string DebugFolder
		{
			get
			{
				if (!Directory.Exists(_debugFolder))
					Directory.CreateDirectory(_debugFolder);

				return _debugFolder;
			}
		}

		public static string BackupFolder
		{
			get
			{
				if (!Directory.Exists(_backupFolder))
					Directory.CreateDirectory(_backupFolder);

				return _backupFolder;
			}
		}

		public static string LogsFilePath
		{
			get
			{
				if (!Directory.Exists(Path.GetDirectoryName(_logsFilePath)))
					Directory.CreateDirectory(Path.GetDirectoryName(_logsFilePath));

				return _logsFilePath;
			}
		}

		#endregion

		#region Static Private Vars

		private static string _debugFolder = "./extOSC/Debug/";

		private static string _backupFolder = "./extOSC/";

		private static string _logsFilePath = "./extOSC/logs.xml";

		private static OSCControls.Resources _defaultResources;

		#endregion

		#region Static Public Methods

		public static void FindObjects<T>(Func<T, string> namingCallback, bool withNone, out GUIContent[] contents, out T[] objects) where T : Object
		{
			var sceneObjects = Object.FindObjectsOfType<T>();
			var offset = 0;
			var count = sceneObjects.Length;

			if (withNone)
			{
				offset++;
				count++;
			}

			objects = new T[count];
			contents = new GUIContent[count];

			if (withNone)
			{
				objects[0] = null;
				contents[0] = new GUIContent("- None -");
			}

			for (var i = 0; i < sceneObjects.Length; ++i)
			{
				var obj = sceneObjects[i];
				var name = namingCallback != null ? namingCallback.Invoke(sceneObjects[i]) : obj.ToString();

				objects[i + offset] = sceneObjects[i];
				contents[i + offset] = new GUIContent(name);
			}
		}

		public static OSCReceiver FindReceiver(int localPort)
		{
			var receivers = Object.FindObjectsOfType<OSCReceiver>();

			foreach (var receiver in receivers)
			{
				if (receiver.LocalPort == localPort)
					return receiver;
			}

			return null;
		}

		public static OSCTransmitter FindTransmitter(string remoteHost, int remotePort)
		{
			var transmitters = Object.FindObjectsOfType<OSCTransmitter>();

			foreach (var transmitter in transmitters)
			{
				if (transmitter.RemoteHost == remoteHost &&
					transmitter.RemotePort == remotePort)
					return transmitter;
			}

			return null;
		}

		public static List<OSCConsolePacket> LoadConsoleMessages(string filePath)
		{
			var list = new List<OSCConsolePacket>();

			if (!File.Exists(filePath))
			{
				SaveConsoleMessages(filePath, list);

				return list;
			}

			var document = new XmlDocument();
			try
			{
				document.Load(filePath);

				var rootElement = document.FirstChild;

				foreach (XmlNode messageElement in rootElement.ChildNodes)
				{
					var consoleMessage = new OSCConsolePacket();

					var instanceAttribute = messageElement.Attributes["info"];
					consoleMessage.Info = instanceAttribute.InnerText;

					var typeAttribute = messageElement.Attributes["type"];
					consoleMessage.PacketType = (OSCConsolePacketType) Enum.Parse(typeof(OSCConsolePacketType), typeAttribute.InnerText);

					var timestampAttribute = messageElement.Attributes["timestamp"];
					if (timestampAttribute != null)
					{
						consoleMessage.TimeStamp = timestampAttribute.InnerText;
					}

					var packetElement = messageElement["packet"];
					consoleMessage.Packet = OSCUtilities.FromBase64String(packetElement.InnerText);

					list.Add(consoleMessage);
				}
			}
			catch (Exception e)
			{
				Debug.LogFormat("[OSCConsole] Error: {0}", e);
				list.Clear();
			}

			return list;
		}

		public static void SaveConsoleMessages(string filePath, List<OSCConsolePacket> list)
		{
			var document = new XmlDocument();
			var rootElement = (XmlElement) document.AppendChild(document.CreateElement("root"));

			foreach (var consoleMessage in list)
			{
				var messageElement = rootElement.AppendChild(document.CreateElement("message"));

				var instanceAttribute = document.CreateAttribute("info");
				instanceAttribute.InnerText = consoleMessage.Info;

				var typeAttribute = document.CreateAttribute("type");
				typeAttribute.InnerText = consoleMessage.PacketType.ToString();

				var timestampAttribute = document.CreateAttribute("timestamp");
				timestampAttribute.InnerText = consoleMessage.TimeStamp;

				messageElement.Attributes.Append(instanceAttribute);
				messageElement.Attributes.Append(typeAttribute);
				messageElement.Attributes.Append(timestampAttribute);

				var packetElement = document.CreateElement("packet");
				packetElement.InnerText = OSCUtilities.ToBase64String(consoleMessage.Packet);

				messageElement.AppendChild(packetElement);
			}

			document.Save(filePath);
		}

		public static IOSCPacket LoadPacket(string filePath)
		{
			if (!File.Exists(filePath))
				return null;

			try
			{
				return OSCConverter.Unpack(File.ReadAllBytes(filePath));
			}
			catch (Exception e)
			{
				Debug.LogFormat("[extOSC:OSCEditorUtils] Load packet exception: {0}", e);

				try
				{
					var document = new XmlDocument();
					document.Load(filePath);

					return OSCUtilities.FromBase64String(document.FirstChild.InnerText);
				}
				catch (Exception e2)
				{
					Debug.LogFormat("[extOSC:OSCEditorUtils] Old format unpack exception: {0}", e2);
				}
			}

			return null;
		}

		public static void SavePacket(string filePath, IOSCPacket packet)
		{
			using (var fileWriter = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
			{
				var length = OSCConverter.Pack(packet, out var buffer);
				fileWriter.Write(buffer, 0, length);
			}
		}

		public static OSCControls.Resources GetStandardResources()
		{
			if (_defaultResources.PanelFilled == null)
			{
				_defaultResources.PanelFilled = OSCEditorSprites.PanelFilled;
				_defaultResources.PanelBorder = OSCEditorSprites.PanelBorder;
				_defaultResources.RotaryFilled = OSCEditorSprites.RotaryFilled;
				_defaultResources.RotaryFilledMask = OSCEditorSprites.RotaryFilledMask;
				_defaultResources.RotaryBorder = OSCEditorSprites.RotaryBorder;
			}

			return _defaultResources;
		}

		public static OSCValue CreateValue(OSCValueType valueType)
		{
			switch (valueType)
			{
				case OSCValueType.Unknown:
					return null;
				case OSCValueType.Int:
					return OSCValue.Int(0);
				case OSCValueType.Long:
					return OSCValue.Long(0);
				case OSCValueType.True:
					return OSCValue.Bool(true);
				case OSCValueType.False:
					return OSCValue.Bool(false);
				case OSCValueType.Float:
					return OSCValue.Float(0);
				case OSCValueType.Double:
					return OSCValue.Double(0);
				case OSCValueType.String:
					return OSCValue.String("");
				case OSCValueType.Null:
					return OSCValue.Null();
				case OSCValueType.Impulse:
					return OSCValue.Impulse();
				case OSCValueType.Blob:
					return OSCValue.Blob(new byte[0]);
				case OSCValueType.Char:
					return OSCValue.Char(' ');
				case OSCValueType.Color:
					return OSCValue.Color(Color.white);
				case OSCValueType.TimeTag:
					return OSCValue.TimeTag(DateTime.Now);
				case OSCValueType.Midi:
					return OSCValue.Midi(new OSCMidi(0, 0, 0, 0));
				case OSCValueType.Array:
					return OSCValue.Array();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static string MemberName(MemberInfo memberInfo)
		{
			var prefix = string.Empty;
			var postfix = string.Empty;

			if (memberInfo is FieldInfo fieldInfo)
			{
				prefix = "[F] " + fieldInfo.FieldType.Name;
			}
			else if (memberInfo is PropertyInfo propertyInfo)
			{
				prefix = "[P] " + propertyInfo.PropertyType.Name;
			}
			else if (memberInfo is MethodInfo methodInfo)
			{
				var parameters = methodInfo.GetParameters();

				foreach (var parameter in parameters)
				{
					postfix += parameter.ParameterType.Name + ", ";
				}

				if (postfix.Length > 2)
					postfix = postfix.Remove(postfix.Length - 2);

				prefix = "[M] " + methodInfo.ReturnType.Name;
				postfix = $"({postfix})";
			}

			return $"{prefix} \t{memberInfo.Name}{postfix}";
		}

		public static string GetValueTags(OSCMessage message)
		{
			var values = message.Values;
			var tags = string.Empty;

			foreach (var value in values)
			{
				tags += value.Tag;
			}

			return tags;
		}

		#endregion
	}
}