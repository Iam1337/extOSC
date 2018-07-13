﻿/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

#if NETFX_CORE
using System.Reflection;
#endif

namespace extOSC
{
    public static class OSCUtilities
    {
        #region Private Static Vars

        private static readonly Dictionary<string, List<string>> _cachedAddress = new Dictionary<string, List<string>>();

        #endregion

        #region Public Static Methods

        public static int ClampPort(int port)
        {
            return Mathf.Clamp(port, 1, 65535);
        }

        public static string GetLocalHost()
        {
#if !NETFX_CORE
			try
			{
				var hostName = Dns.GetHostName();
				var host = Dns.GetHostEntry(hostName);

				foreach (var address in host.AddressList)
				{
					if (address.AddressFamily == AddressFamily.InterNetwork)
					{
						return address.ToString();
					}
				}
			}
			catch { }
#endif

            return "127.0.0.1";
        }

        public static float Map(float value, float inputMin, float inputMax, float outputMin, float outputMax,
            bool clamp = true)
        {
            if (Mathf.Abs(inputMin - inputMax) < Mathf.Epsilon) return outputMin;

            float outputValue = ((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin);

            if (clamp)
            {
                if (outputMax < outputMin) outputValue = Mathf.Clamp(outputValue, outputMax, outputMin);
                else outputValue = Mathf.Clamp(outputValue, outputMin, outputMax);
            }

            return outputValue;
        }

        public static bool CompareAddresses(string bindAddress, string messageAddress)
        {
            if (bindAddress == "*")
                return true;

            if (!bindAddress.Contains("*"))
                return bindAddress == messageAddress;

            if (!_cachedAddress.ContainsKey(bindAddress))
                _cachedAddress.Add(bindAddress, new List<string>());

            if (_cachedAddress[bindAddress].Contains(messageAddress))
                return true;

            var regular = new Regex("^" + bindAddress.Replace("*", "(.+)") + "$");
            if (regular.IsMatch(messageAddress))
            {
                _cachedAddress[bindAddress].Add(messageAddress);
                return true;
            }

            return false;
        }

/*
        // Old Marshal method.
        public static byte[] StructToByte(object structure)
        {
            var structureType = structure.GetType();
            if (IsEnumOrClass(structureType))
                throw new ArgumentException("Invalid argument type. Must be struct.");

            var structureSize = Marshal.SizeOf(structure);
            var data = new byte[structureSize];
            var pointer = Marshal.AllocHGlobal(structureSize);

            Marshal.StructureToPtr(structure, pointer, true);
            Marshal.Copy(pointer, data, 0, structureSize);
            Marshal.FreeHGlobal(pointer);

            return data;
        }
*/

        public static byte[] StructToByte<T>(T structure) where T : struct
        {
            var structureSize = StructSizeOf(structure);
            var data = new byte[structureSize];
            var pointer = Marshal.AllocHGlobal(structureSize);

            Marshal.StructureToPtr(structure, pointer, true);
            Marshal.Copy(pointer, data, 0, structureSize);
            Marshal.FreeHGlobal(pointer);

            return data;
        }

/*
        // Old Marshal method.
        public static object ByteToStruct(Type structureType, byte[] data)
        {
            if (IsEnumOrClass(structureType))
                throw new ArgumentException("Invalid type. Must be struct.");

            var structureSize = Marshal.SizeOf(structureType);
            var pointer = Marshal.AllocHGlobal(structureSize);

            Marshal.Copy(data, 0, pointer, structureSize);
            var structure = Marshal.PtrToStructure(pointer, structureType);
            Marshal.FreeHGlobal(pointer);

            return structure;
        }
*/

        public static T ByteToStruct<T>(byte[] data) where T : struct
        {
            var structureSize = StructSizeOf<T>();
            var pointer = Marshal.AllocHGlobal(structureSize);

            Marshal.Copy(data, 0, pointer, structureSize);
            var structure = PtrToStructure<T>(pointer);
            Marshal.FreeHGlobal(pointer);

            return structure;
        }

        #endregion

        #region Public Extensions Static Methods

        public static bool IsMatch(this OSCMessage message, OSCMatchPattern pattern)
        {
            var messageTypes = message.GetTypes();
            if (messageTypes.Length != pattern.Types.Length)
                return false;

            for (var i = 0; i < messageTypes.Length; i++)
            {
                if (pattern.Types[i] == OSCValueType.True ||
                    pattern.Types[i] == OSCValueType.False)
                {
                    if (messageTypes[i] != OSCValueType.True &&
                        messageTypes[i] != OSCValueType.False)
                    {
                        return false;
                    }
                }
                else if ((pattern.Types[i] != messageTypes[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ToFloat(this OSCMessage message, out float value)
        {
            var values = message.GetValues(OSCValueType.Float);
            if (values.Length > 0)
            {
                var firstValue = values[0];

                value = firstValue.FloatValue;
                return true;
            }

            value = 0;
            return false;
        }

        public static bool HasImpulse(this OSCMessage message)
        {
            var values = message.GetValues(OSCValueType.Impulse);
            return values.Length > 0;
        }

        public static bool HasNull(this OSCMessage message)
        {
            var values = message.GetValues(OSCValueType.Null);
            return values.Length > 0;
        }

        public static bool ToInt(this OSCMessage message, out int value)
        {
            var values = message.GetValues(OSCValueType.Int);
            if (values.Length > 0)
            {
                var firstValue = values[0];

                value = firstValue.IntValue;
                return true;
            }

            value = 0;
            return false;
        }

        public static bool ToDouble(this OSCMessage message, out double value)
        {
            var values = message.GetValues(OSCValueType.Double);
            if (values.Length > 0)
            {
                var firstValue = values[0];

                value = firstValue.DoubleValue;
                return true;
            }

            value = 0;
            return false;
        }

        public static bool ToLong(this OSCMessage message, out long value)
        {
            var values = message.GetValues(OSCValueType.Long);
            if (values.Length > 0)
            {
                var firstValue = values[0];

                value = firstValue.LongValue;
                return true;
            }

            value = 0;
            return false;
        }

        public static bool ToChar(this OSCMessage message, out char value)
        {
            var values = message.GetValues(OSCValueType.Char);
            if (values.Length > 0)
            {
                var firstValue = values[0];

                value = firstValue.CharValue;
                return true;
            }

            value = default(char);
            return false;
        }

        public static bool ToBool(this OSCMessage message, out bool value)
        {
            var values = message.GetValues(OSCValueType.True, OSCValueType.False);
            if (values.Length > 0)
            {
                var firstValue = values[0];

                value = firstValue.BoolValue;
                return true;
            }

            value = default(bool);
            return false;
        }

        public static bool ToBlob(this OSCMessage message, out byte[] value)
        {
            var values = message.GetValues(OSCValueType.Blob);
            if (values.Length > 0)
            {
                var firstValue = values[0];

                value = firstValue.BlobValue;
                return true;
            }

            value = default(byte[]);
            return false;
        }

        public static bool ToString(this OSCMessage message, out string value)
        {
            var values = message.GetValues(OSCValueType.String);
            if (values.Length > 0)
            {
                var firstValue = values[0];

                value = firstValue.StringValue;
                return true;
            }

            value = default(string);
            return false;
        }

        public static bool ToTimeTag(this OSCMessage message, out DateTime value)
        {
            var values = message.GetValues(OSCValueType.TimeTag);
            if (values.Length > 0)
            {
                var firstValue = values[0];

                value = firstValue.TimeTagValue;
                return true;
            }

            value = default(DateTime);
            return false;
        }

        public static bool ToColor(this OSCMessage message, out Color value, bool force = false)
        {
            var values = message.GetValues(OSCValueType.Color);
            if (values.Length > 0)
            {
                var firstValue = values[0];

                value = firstValue.ColorValue;
                return true;
            }

            if (force)
            {
                Vector3 vector3;
                if (message.ToVector3(out vector3))
                {
                    value = new Color(vector3.x, vector3.y, vector3.z);
                    return true;
                }

                Vector4 vector4;
                if (message.ToVector4(out vector4))
                {
                    value = new Color(vector4.x, vector4.y, vector4.z, vector4.w);
                    return true;
                }
            }

            value = Color.white;
            return false;
        }

        public static bool ToMidi(this OSCMessage message, out OSCMidi value)
        {
            var values = message.GetValues(OSCValueType.Midi);
            if (values.Length > 0)
            {
                var firstValue = values[0];

                value = firstValue.MidiValue;
                return true;
            }

            value = default(OSCMidi);
            return false;
        }

        public static bool ToVector2(this OSCMessage message, out Vector2 value, bool force = false)
        {
            var values = message.GetValues(OSCValueType.Float);
            if (values.Length >= 2)
            {
                var firstValue = values[0];
                var secondValue = values[1];

                value = new Vector2(firstValue.FloatValue, secondValue.FloatValue);

                return true;
            }

            if (force)
            {
                Vector3 vector3;
                if (message.ToVector3(out vector3))
                {
                    value = vector3;
                    return true;
                }

                Vector4 vector4;
                if (message.ToVector4(out vector4))
                {
                    value = vector4;
                    return true;
                }
            }

            value = Vector2.zero;
            return false;
        }

        public static bool ToVector3(this OSCMessage message, out Vector3 value, bool force = false)
        {
            var values = message.GetValues(OSCValueType.Float);
            if (values.Length >= 3)
            {
                var firstValue = values[0];
                var secondValue = values[1];
                var thirdValue = values[2];

                value = new Vector3(firstValue.FloatValue, secondValue.FloatValue, thirdValue.FloatValue);

                return true;
            }

            if (force)
            {
                Vector2 vector2;
                if (message.ToVector2(out vector2))
                {
                    value = vector2;
                    return true;
                }

                Vector4 vector4;
                if (message.ToVector4(out vector4))
                {
                    value = vector4;
                    return true;
                }
            }

            value = Vector3.zero;
            return false;
        }

        public static bool ToVector4(this OSCMessage message, out Vector4 value, bool force = false)
        {
            var values = message.GetValues(OSCValueType.Float);
            if (values.Length >= 4)
            {
                var firstValue = values[0];
                var secondValue = values[1];
                var thirdValue = values[2];
                var fourthValue = values[3];

                value = new Vector4(firstValue.FloatValue, secondValue.FloatValue, thirdValue.FloatValue,
                    fourthValue.FloatValue);

                return true;
            }

            if (force)
            {
                Vector2 vector2;
                if (message.ToVector2(out vector2))
                {
                    value = vector2;
                    return true;
                }

                Vector3 vector3;
                if (message.ToVector3(out vector3))
                {
                    value = vector3;
                    return true;
                }
            }

            value = Vector4.zero;
            return false;
        }

        public static bool ToQuaternion(this OSCMessage message, out Quaternion value)
        {
            Vector4 vector4;
            if (message.ToVector4(out vector4))
            {
                value = new Quaternion(vector4.x, vector4.y, vector4.z, vector4.w);
                return true;
            }

            value = new Quaternion();
            return false;
        }

        public static bool ToRect(this OSCMessage message, out Rect value)
        {
            Vector4 vector4;
            if (message.ToVector4(out vector4))
            {
                value = new Rect(vector4.x, vector4.y, vector4.z, vector4.w);
                return true;
            }

            value = new Rect();
            return false;
        }

        public static bool ToArray(this OSCMessage message, out List<OSCValue> value)
        {
            var values = message.GetValues(OSCValueType.Array);
            if (values.Length > 0)
            {
                var firstValue = values[0];

                value = firstValue.ArrayValue;
                return true;
            }

            value = new List<OSCValue>();
            return false;
        }

        public static bool IsSubclassOf(Type subType, Type baseType)
        {
#if !NETFX_CORE
            return (subType.IsSubclassOf(baseType) || subType == baseType);
#else
            return (subType.GetTypeInfo().IsSubclassOf(baseType) || subType == baseType);
#endif
        }

        public static bool IsSubclassOf(object target, Type targetType)
        {
            return IsSubclassOf(target.GetType(), targetType);
        }

        #endregion

        #region Private Static Class

        private static bool IsEnumOrClass(Type type)
        {
#if !NETFX_CORE
            return !type.IsValueType || type.IsEnum;
#else
            return type.GetTypeInfo().IsValueType || type.GetTypeInfo().IsEnum;
#endif
        }

        private static int StructSizeOf<T>(T structure) where T : struct
        {
#if !NETFX_CORE
            return Marshal.SizeOf(structure);
#else
            return Marshal.SizeOf<T>(structure);
#endif
        }

        private static int StructSizeOf<T>() where T : struct
        {
#if !NETFX_CORE
            return Marshal.SizeOf(typeof(T));
#else
            return Marshal.SizeOf<T>();
#endif
        }

        private static T PtrToStructure<T>(IntPtr pointer) where T : struct
        {
#if !NETFX_CORE
            return (T)Marshal.PtrToStructure(pointer, typeof(T));
#else
            return Marshal.PtrToStructure<T>(pointer);
#endif
        }

        #endregion
    }
}