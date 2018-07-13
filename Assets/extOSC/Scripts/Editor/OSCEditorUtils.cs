/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;

using extOSC.UI;
using extOSC.Core;
using extOSC.Core.Console;

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
                if (!Directory.Exists(_backupFoder))
                    Directory.CreateDirectory(_backupFoder);

                return _backupFoder;
            }
        }

        public static string LogsFilePath
        {
            get
            {
                if (!Directory.Exists(Path.GetDirectoryName(_logsFilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(_logsFilePath));

                return _logsFilePath; ;
            }
        }

        #endregion

        #region Static Private Vars

        private static string _debugFolder = "./extOSC/Debug/";

        private static string _backupFoder = "./extOSC/";

        private static string _logsFilePath = "./extOSC/logs.xml";

        private static OSCControls.Resources _defaultResources;

        #endregion

        #region Static Public Methods

        public static OSCReceiver FindReceiver(int localPort)
        {
            var receivers = GameObject.FindObjectsOfType<OSCReceiver>();

            foreach (var receiver in receivers)
            {
                if (receiver.LocalPort == localPort)
                    return receiver;
            }

            return null;
        }

        public static OSCTransmitter FindTransmitter(string remoteHost, int remotePort)
        {
            var transmitters = GameObject.FindObjectsOfType<OSCTransmitter>();

            foreach (var transmitter in transmitters)
            {
                if (transmitter.RemoteHost == remoteHost &&
                    transmitter.RemotePort == remotePort)
                    return transmitter;
            }

            return null;
        }

        public static Dictionary<string, OSCReceiver> GetReceivers()
        {
            return GetOSC<OSCReceiver>((receiver) =>
            {
                return string.Format("Receiver: {0}", receiver.LocalPort);
            });
        }

        public static Dictionary<string, OSCTransmitter> GetTransmitters()
        {
            return GetOSC<OSCTransmitter>((transmitter) =>
            {
                return string.Format("Transmitter: {0}:{1}", transmitter.RemoteHost, transmitter.RemotePort);
            });
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
                    consoleMessage.PacketType = (OSCConsolePacketType)Enum.Parse(typeof(OSCConsolePacketType), typeAttribute.InnerText);

                    var packetElement = messageElement["packet"];
                    consoleMessage.Packet = OSCPacket.FromBase64String(packetElement.InnerText);

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
            var rootElement = (XmlElement)document.AppendChild(document.CreateElement("root"));

            foreach (var consoleMessage in list)
            {
                var messageElement = rootElement.AppendChild(document.CreateElement("message"));

                var instanceAttribute = document.CreateAttribute("info");
                instanceAttribute.InnerText = consoleMessage.Info;

                var typeAttribute = document.CreateAttribute("type");
                typeAttribute.InnerText = consoleMessage.PacketType.ToString();

                messageElement.Attributes.Append(instanceAttribute);
                messageElement.Attributes.Append(typeAttribute);

                var packetElement = document.CreateElement("packet");
                packetElement.InnerText = OSCPacket.ToBase64String(consoleMessage.Packet);

                messageElement.AppendChild(packetElement);
            }

            document.Save(filePath);
        }

        public static OSCPacket LoadPacket(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            try
            {
                return OSCConverter.Unpack(File.ReadAllBytes(filePath));
            }
            catch (Exception e)
            {
                Debug.LogFormat("[OSCEditorUtils] Load Packet error: {0}", e);

                try
                {
                    var document = new XmlDocument();
                    document.Load(filePath);

                    return OSCPacket.FromBase64String(document.FirstChild.InnerText);
                }
                catch (Exception e2)
                {
                    Debug.LogFormat("[OSCEditorUtils] Load Old Format Packet Error: {0}", e2);
                }
            }

            return null;
        }

        public static void SavePacket(string filePath, OSCPacket packet)
        {
            File.WriteAllBytes(filePath, OSCConverter.Pack(packet));
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

        public static void PingObject(UnityEngine.Object @object, bool selectObject = true)
        {
            EditorGUIUtility.PingObject(@object);

            if (selectObject)
                Selection.activeObject = @object;
        }

        public static OSCValue CreateOSCValue(OSCValueType valueType)
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

        public static OSCPacket CopyPacket(OSCPacket packet)
        {
            return OSCConverter.Unpack(OSCConverter.Pack(packet));
        }

        public static string NicifyName(string name, bool removePrefix = true)
        {
            name = ObjectNames.NicifyVariableName(name);

            if (removePrefix)
            {
                if (name.StartsWith("OSC", StringComparison.Ordinal))
                    name = name.Remove(0, 3);
            }

            return name.Trim();
        }

        public static string MemberName(MemberInfo memberInfo)
        {
            var prefix = string.Empty;
            var postfix = string.Empty;

            if (memberInfo is FieldInfo)
            {
                var fieldInfo = (FieldInfo)memberInfo;

                prefix = "[F] " + fieldInfo.FieldType.Name;
            }
            else if (memberInfo is PropertyInfo)
            {
                var propertyInfo = (PropertyInfo)memberInfo;

                prefix = "[P] " + propertyInfo.PropertyType.Name;
            }
            else if (memberInfo is MethodInfo)
            {
                var methodInfo = (MethodInfo)memberInfo;
                var parameters = methodInfo.GetParameters();

                foreach (var parameter in parameters)
                {
                    postfix += parameter.ParameterType.Name + ", ";
                }

                if (postfix.Length > 2)
                    postfix = postfix.Remove(postfix.Length - 2);

                prefix = "[M] " + methodInfo.ReturnType.Name;
                postfix = string.Format("({0})", postfix);
            }

            return string.Format("{0} \t{1}{2}", prefix, memberInfo.Name, postfix);
        }

        #endregion

        #region Static Private Methods

        private static Dictionary<string, T> GetOSC<T>(Func<T, string> namingCallback) where T : OSCBase
        {
            var dictionary = new Dictionary<string, T>();
            var objects = GameObject.FindObjectsOfType<T>();

            foreach (var osc in objects)
            {
                var name = osc.gameObject.name;

                if (namingCallback != null)
                    name = namingCallback(osc);

                if (!dictionary.ContainsKey(name))
                    dictionary.Add(name, osc);
            }

            return dictionary;
        }

        #endregion
    }
}