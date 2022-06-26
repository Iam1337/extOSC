/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using System;

using extOSC.Core;

namespace extOSC.Editor
{
	public static class OSCSharpCode
	{
		#region Static Public Methods

		public static string GeneratePacket(IOSCPacket packet)
		{
			if (packet is OSCBundle bundle)
			{
				return GenerateBundle(bundle);
			}

			if (packet is OSCMessage message)
			{
				return GenerateMessage(message);
			}

			return string.Empty;
		}

		public static string GenerateBundle(OSCBundle bundle)
		{
			return GenerateBundle(bundle, "bundle", true);
		}

		public static string GenerateMessage(OSCMessage message)
		{
			return GenerateMessage(message, "message", true);
		}

		public static string GenerateBundle(OSCBundle bundle, string name, bool init)
		{
			var prefix = init ? "var " : string.Empty;
			var sharpCode = $"{prefix}{name} = new OSCBundle();\n";
			var bundleInit = true;
			var packetInit = true;

			for (var i = 0; i < bundle.Packets.Count; i++)
			{
				var packetName = string.Empty;
				var packet = bundle.Packets[i];

				if (packet is OSCBundle packetBundle)
				{
					packetName = $"{name}Bundle";
					sharpCode += GenerateBundle(packetBundle, packetName, bundleInit);
					bundleInit = false;
				}

				if (packet is OSCMessage packetMessage)
				{
					packetName = $"{name}Message";
					sharpCode += GenerateMessage(packetMessage, packetName, packetInit);
					packetInit = false;
				}

				if (string.IsNullOrEmpty(packetName))
					continue;

				sharpCode += $"{name}.Add({packetName});\n\n";
			}

			if (sharpCode.EndsWith("\n\n", StringComparison.Ordinal))
				sharpCode = sharpCode.Remove(sharpCode.Length - 1);

			return sharpCode;
		}

		public static string GenerateMessage(OSCMessage message, string name, bool init)
		{
			var prefix = init ? "var " : string.Empty;
			var sharpCode = $"{prefix}{name} = new OSCMessage(\"{message.Address}\");\n";

			foreach (var value in message.Values)
			{
				sharpCode += $"{name}.AddValue({GenerateValue(value)});\n";
			}

			return sharpCode;
		}

		public static string GenerateValue(OSCValue value)
		{
			var type = value.Type;

			if (type == OSCValueType.Unknown)
				return string.Empty;

			if (type == OSCValueType.Array)
			{
				var stringValues = string.Empty;

				foreach (var arrayValue in value.ArrayValue)
				{
					stringValues += GenerateValue(arrayValue) + ", ";
				}

				if (stringValues.Length > 2)
					stringValues = stringValues.Remove(stringValues.Length - 2);

				return $"OSCValue.Array({stringValues})";
			}

			return GenerateValue(type, value.Value);
		}

		#endregion

		#region Static Private Methods

		private static string GenerateValue(OSCValueType type, object value)
		{

			if (type == OSCValueType.Impulse || type == OSCValueType.Null)
			{
				return $"OSCValue.{type}()";
			}

			if (type == OSCValueType.True || type == OSCValueType.False)
			{
				return $"OSCValue.Bool({value.ToString().ToLower()})";
			}

			if (type == OSCValueType.Float)
			{
				return $"OSCValue.Float({value}f)";
			}

			if (type == OSCValueType.Char)
			{
				return $"OSCValue.Char(\'{value}\')";
			}

			if (type == OSCValueType.String)
			{
				return $"OSCValue.String(\"{value}\")";
			}

			if (type == OSCValueType.TimeTag)
			{
				return $"OSCValue.TimeTag(DateTime.Parse(\"{value}\"))";
			}

			if (type == OSCValueType.Color)
			{
				var color = (Color) value;

				return $"OSCValue.Color(new Color({color.r}f, {color.g}f, {color.b}f, {color.a}f))";
			}

			if (type == OSCValueType.Midi)
			{
				var midi = (OSCMidi) value;

				return $"OSCValue.Midi(new OSCMidi({midi.Channel}, {midi.Status}, {midi.Data1}, {midi.Data2}))";
			}

			if (type == OSCValueType.Blob)
			{
				var stringValue = "new byte[] {";
				var datas = (byte[]) value;

				if (datas.Length > 0)
				{
					foreach (var data in datas)
					{
						stringValue += $"{data:x2}, ";
					}

					stringValue = stringValue.Remove(stringValue.Length - 2);
				}

				stringValue += "}";

				return $"OSCValue.Blob({stringValue})";
			}

			return $"OSCValue.{type}({value})";
		}

		#endregion
	}
}