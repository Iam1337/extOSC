/* Copyright (c) 2019 ExT (V.Sigalkin) */

#if !NETFX_CORE

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using extOSC.Serialization.Packers;

namespace extOSC.Serialization
{
    public static class OSCSerializer
    {
        #region Static Private Vars

        private static readonly OSCPacker _defaultPacker = new OSCPackerAttribute();

        private static readonly Dictionary<Type, OSCPacker> _packersDictionary = new Dictionary<Type, OSCPacker>() {
            { typeof(int), new OSCPackerInt() },
            { typeof(char), new OSCPackerChar() },
            { typeof(bool), new OSCPackerBool() },
            { typeof(long), new OSCPackerLong() },
            { typeof(Rect), new OSCPackerRect() },
            { typeof(float), new OSCPackerFloat() },
            { typeof(Color), new OSCPackerColor() },
            { typeof(byte[]), new OSCPackerBlob() },
            { typeof(OSCMidi), new OSCPackerMidi() },
            { typeof(double), new OSCPackerDouble() },
            { typeof(string), new OSCPackerString() },
            { typeof(Bounds), new OSCPackerBounds() },
            { typeof(Vector2), new OSCPackerVector2() },
            { typeof(Vector3), new OSCPackerVector3() },
            { typeof(Vector4), new OSCPackerVector4() },
            { typeof(DateTime), new OSCPackerTimeTag() },
            { typeof(Quaternion), new OSCPackerQuaternion() },

            { typeof(Array), new OSCPackerArray() },
            { typeof(IList), new OSCPackerList() },
            { typeof(IDictionary), new OSCPackerDictionary() }
        };

        #endregion

        #region Static Public Methods

        public static void AddPacker(OSCPacker packer)
        {
            if (packer == null) return;
            if (_packersDictionary.ContainsKey(packer.GetPackerType())) return;

            _packersDictionary.Add(packer.GetPackerType(), packer);
        }

        public static bool HasPacker(Type valueType)
        {
            foreach (var packerType in _packersDictionary.Keys)
            {
                if (packerType == valueType || packerType.IsAssignableFrom(valueType) || valueType.IsSubclassOf(packerType))
                    return true;
            }

            return false;
        }

        public static Type GetTypeById(string packerId)
        {
            foreach (var packer in _packersDictionary.Values)
            {
                if (packer.GetId() == packerId)
                    return packer.GetPackerType();
            }

            return null;
        }

        public static OSCPacker GetPacker(Type valueType)
        {
            foreach (var packerType in _packersDictionary.Keys)
            {
                if (packerType == valueType || packerType.IsAssignableFrom(valueType) || valueType.IsSubclassOf(packerType))
                    return _packersDictionary[packerType];
            }

            return _defaultPacker;
        }

        public static OSCMessage Serialize(string address, object target)
        {
            var message = new OSCMessage(address);
            var values = new List<OSCValue>();

            Pack(values, target);

            message.Values = values;
            return message;
        }

        public static T Deserialize<T>(OSCMessage message)
        {
            return (T)Deserialize(message, typeof(T));
        }

        public static object Deserialize(OSCMessage message, Type type)
        {
            var start = 0;

            return Unpack(message.Values, ref start, type);
        }

        public static List<OSCValue> Pack(object target)
        {
            var values = new List<OSCValue>();

            Pack(values, target);

            return values;
        }

        public static void Pack(List<OSCValue> values, object target)
        {
            if (target == null) 
                return;

            var packer = GetPacker(target.GetType());
            if (packer != null)
            {
                packer.Pack(values, target);
            }
        }

        public static T Unpack<T>(List<OSCValue>values)
        {
            return (T)Unpack(values, typeof(T));
        }

        public static object Unpack(List<OSCValue> values, Type type)
        {
            var start = 0;

            return Unpack(values, ref start, type);
        }

        public static object Unpack(List<OSCValue> values, ref int start, Type type)
        {
            var packer = GetPacker(type);
            if (packer != null)
            {
                return packer.Unpack(values, ref start, type);
            }

            return default(object);
        }

        #endregion
    }
}

#endif