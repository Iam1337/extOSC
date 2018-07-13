/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using extOSC.Core.Packers;

namespace extOSC.Core
{
    public static class OSCConverter
    {
        #region Static Public Vars

        private static readonly Dictionary<OSCValueType, OSCPacker> _packersDictionary = new Dictionary<OSCValueType, OSCPacker>()
        {
            { OSCValueType.Int, new OSCPackerInt() },
            { OSCValueType.Null, new OSCPackerNull() },
            { OSCValueType.Blob, new OSCPackerBlob() },
            { OSCValueType.Long, new OSCPackerLong() },
            { OSCValueType.True, new OSCPackerTrue() },
            { OSCValueType.Char, new OSCPackerChar() },
            { OSCValueType.Midi, new OSCPackerMidi() },
            { OSCValueType.Color, new OSCPackerColor()},
            { OSCValueType.False, new OSCPackerFalse() },
            { OSCValueType.Float, new OSCPackerFloat() },
            { OSCValueType.Double, new OSCPackerDouble() },
            { OSCValueType.String, new OSCPackerString() },
            { OSCValueType.Impulse, new OSCPackerImpulse() },
            { OSCValueType.TimeTag, new OSCPackerTimeTag() }
        };

        #endregion

        #region Static Public Methods

        public static byte[] Pack(OSCPacket packet)
        {
            return packet.IsBundle() ? PackBundle((OSCBundle)packet) : PackMessage((OSCMessage)packet);
        }

        public static OSCPacket Unpack(byte[] bytes)
        {
            var start = 0; return Unpack(bytes, ref start, bytes.Length);
        }

        #endregion

        #region Static Private Methods

        private static bool IsBundle(byte[] bytes, ref int start)
        {
            return bytes[start] == '#';
        }

        //  PACK METHODS
        private static byte[] PackBundle(OSCBundle bundle)
        {
            var finalBytes = new List<byte>();
            var valuesBytes = new List<byte>();

            foreach (var packet in bundle.Packets)
            {
                if (packet != null)
                {
                    var bytes = Pack(packet);

                    InsertBytes(valuesBytes, PackValue(OSCValueType.Int, bytes.Length));
                    InsertBytes(valuesBytes, bytes);
                }
            }

            InsertBytes(finalBytes, PackValue(OSCValueType.String, bundle.Address));
            InsertBytes(finalBytes, PackValue(OSCValueType.Long, bundle.TimeStamp));
            InsertBytes(finalBytes, valuesBytes);

            return finalBytes.ToArray();
        }

        private static byte[] PackMessage(OSCMessage message)
        {
            var finalBytes = new List<byte>();
            var valuesBytes = new List<byte>();
            var typeTags = ",";

            foreach (var value in message.Values)
            {
                ProcessValue(ref typeTags, ref valuesBytes, value);
            }

            InsertBytes(finalBytes, PackValue(OSCValueType.String, message.Address));
            InsertBytes(finalBytes, PackValue(OSCValueType.String, typeTags));
            InsertBytes(finalBytes, valuesBytes);

            return finalBytes.ToArray();
        }

        private static void ProcessValue(ref string typeTags, ref List<byte> valuesBytes, OSCValue value)
        {
            if (value.Type == OSCValueType.Array)
            {
                typeTags += '[';

                foreach (var arrayValue in value.ArrayValue)
                {
                    ProcessValue(ref typeTags, ref valuesBytes, arrayValue);
                }

                typeTags += ']';

                return;
            }

            typeTags += value.Tag;
            InsertBytes(valuesBytes, PackValue(value));
        }

        //  UNPACK METHODS.
        private static OSCPacket Unpack(byte[] bytes, ref int start, int end)
        {
            if (IsBundle(bytes, ref start)) return UnpackBundle(bytes, ref start, end);
            return UnpackMessage(bytes, ref start);
        }

        private static OSCBundle UnpackBundle(byte[] bytes, ref int start, int end)
        {
            OSCBundle bundle = null;

            var address = (string)UnpackValue(OSCValueType.String, bytes, ref start);
            if (address.Equals(OSCBundle.KBundle))
            {
                var timeStamp = (long)UnpackValue(OSCValueType.Long, bytes, ref start);

                bundle = new OSCBundle();
                bundle.TimeStamp = timeStamp;

                while (start < end)
                {
                    var packetLength = (int)UnpackValue(OSCValueType.Int, bytes, ref start);
                    var packet = Unpack(bytes, ref start, start + packetLength);

                    bundle.AddPacket(packet);
                }
            }

            return bundle;
        }

        private static OSCMessage UnpackMessage(byte[] bytes, ref int start)
        {
            OSCMessage message = null;

            var address = (string)UnpackValue(OSCValueType.String, bytes, ref start);
            var typeTags = (string)UnpackValue(OSCValueType.String, bytes, ref start);
            var valuesArray = (Dictionary<int, OSCValue>)null;

            message = new OSCMessage(address);

            foreach (var valueTag in typeTags)
            {
                if (valueTag == ',') continue;

                OSCValue value = null;

                // START ARRAY
                if (valueTag == '[')
                {
                    if (valuesArray == null)
                        valuesArray = new Dictionary<int, OSCValue>();

                    valuesArray.Add(valuesArray.Count, OSCValue.Array());

                    continue;
                }

                // STOP ARRAY
                if (valueTag == ']')
                {
                    if (valuesArray != null && valuesArray.Count > 0)
                    {
                        value = valuesArray[valuesArray.Count - 1];
                        valuesArray.Remove(valuesArray.Count - 1);
                    }
                }
                else
                {
                    // DEFAULT VALUE
                    value = UnpackValue(valueTag, bytes, ref start);
                }

                if (valuesArray != null && valuesArray.Count > 0)
                {
                    valuesArray[valuesArray.Count - 1].ArrayValue.Add(value);
                    continue;
                }

                message.AddValue(value);
            }

            return message;
        }

        //  PACK VALUE
        private static byte[] PackValue(OSCValue value)
        {
            var packer = GetPacker(value);
            if (packer != null)
            {
                return packer.Pack(value);
            }

            Debug.LogErrorFormat("[OSCConverter] Unknown value type: '{0}' [1].", value.Value != null ? value.Value.GetType().ToString() : "null");
            return default(byte[]);
        }

        private static byte[] PackValue(OSCValueType valueType, object value)
        {
            var packer = GetPacker(valueType);
            if (packer != null)
            {
                return packer.PackValue(value);
            }

            Debug.LogErrorFormat("[OSCConverter] Unknown value type: '{0}' [2].", valueType);
            return default(byte[]);
        }

        //  UNPACK VALUE
        private static OSCValue UnpackValue(char valueTag, byte[] bytes, ref int start)
        {
            var packer = GetPacker(valueTag);
            if (packer != null)
            {
                return packer.Unpack(bytes, ref start);
            }

            Debug.LogErrorFormat("[OSCConverter] Unknown value tag: '{0}'.", valueTag);
            return default(OSCValue);
        }

        private static object UnpackValue(OSCValueType valueType, byte[] bytes, ref int start)
        {
            var packer = GetPacker(valueType);
            return packer != null ? packer.UnpackValue(bytes, ref start) : default(object);
        }

        //  Get packers.

        private static OSCPacker GetPacker(OSCValue value)
        {
            return GetPacker(value.Type);
        }

        private static OSCPacker GetPacker(char tag)
        {
            return GetPacker(OSCValue.GetValueType(tag));
        }

        private static OSCPacker GetPacker(OSCValueType valueType)
        {
            return _packersDictionary.ContainsKey(valueType) ? _packersDictionary[valueType] : null;
        }

        //  External

        private static void InsertBytes(IList data, IEnumerable bytes)
        {
            if (bytes == null) return;
            foreach (var b in bytes) data.Add(b);
        }

        #endregion
    }
}