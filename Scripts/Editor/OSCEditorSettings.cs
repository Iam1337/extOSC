/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEditor;

namespace extOSC.Editor
{
	public static class OSCEditorSettings
	{
		#region Static Public Vars

		public static string Console => _consoleRoot;

		public static string Debug => _debugRoot;

		public static string Mapping => _mappingRoot;

		public static string ControlCreator => _controlCreatorRoot;

		#endregion

		#region Static Private Vars

		private const string _settingRoot = "extOSC.";

		private const string _consoleRoot = _settingRoot + "console.";

		private const string _debugRoot = _settingRoot + "debug.";

		private const string _mappingRoot = _settingRoot + "mapping.";

		private const string _controlCreatorRoot = _settingRoot + "controlcreator.";

		#endregion

		#region Static Public Methods

		// FLOAT
		public static void SetFloat(string settingPath, float value)
		{
			EditorPrefs.SetFloat(settingPath + ".float", value);
		}

		public static float GetFloat(string settingPath, float defaultSetting)
		{
			return EditorPrefs.GetFloat(settingPath + ".float", defaultSetting);
		}

		// BOOL
		public static void SetBool(string settingPath, bool value)
		{
			EditorPrefs.SetBool(settingPath + ".bool", value);
		}

		public static bool GetBool(string settingPath, bool defaultSetting)
		{
			return EditorPrefs.GetBool(settingPath + ".bool", defaultSetting);
		}

		// INT
		public static void SetInt(string settingPath, int value)
		{
			EditorPrefs.SetInt(settingPath + ".int", value);
		}

		public static int GetInt(string settingPath, int defaultSetting)
		{
			return EditorPrefs.GetInt(settingPath + ".int", defaultSetting);
		}

		// STRING
		public static void SetString(string settingPath, string value)
		{
			EditorPrefs.SetString(settingPath + ".string", value);
		}

		public static string GetString(string settingPath, string defaultSetting)
		{
			return EditorPrefs.GetString(settingPath + ".string", defaultSetting);
		}

		// COLOR
		public static void SetColor(string settingPath, Color color)
		{
			EditorPrefs.SetFloat(settingPath + ".r", color.r);
			EditorPrefs.SetFloat(settingPath + ".g", color.g);
			EditorPrefs.SetFloat(settingPath + ".b", color.b);
			EditorPrefs.SetFloat(settingPath + ".a", color.a);
		}

		public static Color GetColor(string settingPath, Color defaultColor)
		{
			var keyR = settingPath + ".r";
			var keyG = settingPath + ".g";
			var keyB = settingPath + ".b";
			var keyA = settingPath + ".a";

			if (!EditorPrefs.HasKey(keyR) || !EditorPrefs.HasKey(keyG) ||
				!EditorPrefs.HasKey(keyB) || !EditorPrefs.HasKey(keyA))
			{
				return defaultColor;
			}

			var color = new Color();

			color.r = EditorPrefs.GetFloat(keyR, 1);
			color.g = EditorPrefs.GetFloat(keyG + ".g", 1);
			color.b = EditorPrefs.GetFloat(keyB + ".b", 1);
			color.a = EditorPrefs.GetFloat(keyA + ".a", 1);

			return color;
		}

		// TRANSMITTER
		public static void SetTransmitter(string settingPath, OSCTransmitter transmitter)
		{
			EditorPrefs.SetString(settingPath + ".remotehost", transmitter != null ? transmitter.RemoteHost : "");
			EditorPrefs.SetInt(settingPath + ".remoteport", transmitter != null ? transmitter.RemotePort : 0);
		}

		public static OSCTransmitter GetTransmitter(string settingPath, OSCTransmitter defaultTransmitter)
		{
			var keyHost = settingPath + ".remotehost";
			var keyPort = settingPath + ".remoteport";

			if (!EditorPrefs.HasKey(keyHost) || !EditorPrefs.HasKey(keyPort))
			{
				return defaultTransmitter;
			}

			var remoteHost = EditorPrefs.GetString(keyHost, "");
			var remotePort = EditorPrefs.GetInt(keyPort + ".remoteport", 0);

			return OSCEditorUtils.FindTransmitter(remoteHost, remotePort);
			;
		}

		// RECEIVER
		public static void SetReceiver(string settingPath, OSCReceiver receiver)
		{
			EditorPrefs.SetInt(settingPath + ".localport", receiver != null ? receiver.LocalPort : 0);
		}

		public static OSCReceiver GetReceiver(string settingPath, OSCReceiver defaultReceiver)
		{
			var keyPort = settingPath + ".localport";

			if (!EditorPrefs.HasKey(keyPort))
			{
				return defaultReceiver;
			}

			var localPort = EditorPrefs.GetInt(settingPath + ".localport", 0);

			return OSCEditorUtils.FindReceiver(localPort);
		}

		#endregion
	}
}