/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System;

using extOSC.Core;

namespace extOSC.Editor
{
    public static class OSCSharpCode
    {
        #region Static Public Methods

        public static string GeneratePacket(OSCPacket packet)
        {
            if (packet.IsBundle())
                return GenerateBundle(packet as OSCBundle);
            else
                return GenerateMessage(packet as OSCMessage);
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
            var sharpCode = string.Format("{0}{1} = new OSCBundle();\n", prefix, name);
            var bundleInit = true;
            var packetInit = true;

            for (var i = 0; i < bundle.Packets.Count; i++)
            {
                var packetName = string.Empty;
                var packet = bundle.Packets[i];

                if (packet is OSCBundle)
                {
                    packetName = string.Format("{0}Bundle", name);
                    sharpCode += GenerateBundle(packet as OSCBundle, packetName, bundleInit);
                    bundleInit = false;
                }

                if (packet is OSCMessage)
                {
                    packetName = string.Format("{0}Message", name);
                    sharpCode += GenerateMessage(packet as OSCMessage, packetName, packetInit);
                    packetInit = false;
                }

                if (string.IsNullOrEmpty(packetName))
                    continue;

                sharpCode += string.Format("{0}.AddPacket({1});\n\n", name, packetName);
            }

            if (sharpCode.EndsWith("\n\n", StringComparison.Ordinal))
                sharpCode = sharpCode.Remove(sharpCode.Length - 1);

            return sharpCode;
        }

        public static string GenerateMessage(OSCMessage message, string name, bool init)
        {
            var prefix = init ? "var " : string.Empty;
            var sharpCode = string.Format("{0}{1} = new OSCMessage(\"{2}\");\n", prefix, name, message.Address);

            foreach (var value in message.Values)
            {
                sharpCode += string.Format("{0}.AddValue({1});\n", name, GenerateValue(value));
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

                return string.Format("OSCValue.Array({0})", stringValues);
            }

            return GenerateValue(type, value.Value);
        }

        #endregion

        #region Static Private Methods

        private static string GenerateValue(OSCValueType type, object value)
        {

            if (type == OSCValueType.Impulse || type == OSCValueType.Null)
            {
                return string.Format("OSCValue.{0}()", type);
            }

            if (type == OSCValueType.True || type == OSCValueType.False)
            {
                return string.Format("OSCValue.Bool({0})", value.ToString().ToLower());
            }

            if (type == OSCValueType.Float)
            {
                return string.Format("OSCValue.Float({0}f)", value);
            }

            if (type == OSCValueType.Char)
            {
                return string.Format("OSCValue.Char(\'{0}\')", value);
            }

            if (type == OSCValueType.String)
            {
                return string.Format("OSCValue.String(\"{0}\")", value);
            }

            if (type == OSCValueType.TimeTag)
            {
                return string.Format("OSCValue.TimeTag(DateTime.Parse(\"{0}\"))", value);
            }

            if (type == OSCValueType.Color)
            {
                var color = (Color)value;

                return string.Format("OSCValue.Color(new Color({0}f, {1}f, {2}f, {3}f))", color.r, color.g, color.b, color.a);
            }

            if (type == OSCValueType.Midi)
            {
                var midi = (OSCMidi)value;

                return string.Format("OSCValue.Midi(new OSCMidi({0}, {1}, {2}, {3}))", midi.Channel, midi.Status, midi.Data1, midi.Data2);
            }

            if (type == OSCValueType.Blob)
            {
                var stringValue = "new byte[] {";
                var datas = (byte[])value;

                if (datas.Length > 0)
                {
                    foreach (var data in datas)
                    {
                        stringValue += string.Format("{0:x2}, ", data);
                    }

                    stringValue = stringValue.Remove(stringValue.Length - 2);
                }

                stringValue += "}";

                return string.Format("OSCValue.Blob({0})", stringValue);
            }

            return string.Format("OSCValue.{0}({1})", type, value);
        }

        #endregion
    }
}