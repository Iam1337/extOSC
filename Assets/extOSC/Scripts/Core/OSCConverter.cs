/* Copyright (c) 2018 ExT (V.Sigalkin) */

using System;

using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;

using extOSC.Core.Packers;

namespace extOSC.Core
{
    public static class OSCConverter
    {
        #region Static Public Vars

        private static readonly byte[] _packetBuffer = new byte[65507]; // Max UDP size. But better use available default MTU size - 1432.

        private static readonly byte[] _bundleBuffer = new byte[65507]; // Max UDP size. But better use available default MTU size - 1432.

        private static readonly byte[] _headerBuffer = new byte[65507]; // Max UDP size. But better use available default MTU size - 1432.

        private static readonly byte[] _valuesBuffer = new byte[65507]; // Max UDP size. But better use available default MTU size - 1432.

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
        private static int PackBundle(OSCBundle bundle, byte[] buffer)
        {
            //_bundleStream.Position = 0;
            //_bundleStream.SetLength(0);


            /*
            foreach (var packet in bundle.Packets)
            {
                if (packet != null)
                {
                    var bytes = Pack(packet);

                    InsertBytes(valuesBytes, PackValue(OSCValueType.Int, bytes.Length));
                    InsertBytes(valuesBytes, bytes);
                }
            }
            */

            var packets = bundle.Packets;
            var packetsIndex = 0;
            var packetsCount = packets.Count;

            for (var i = 0; i < packetsCount; ++i)
            {
                var packetSize = 0;

                Pack()
            }


            var headerIndex = 0;
            PackValue(_headerBuffer, ref headerIndex, OSCValueType.String, bundle.Address);
            PackValue(_headerBuffer, ref headerIndex, OSCValueType.Long, bundle.TimeStamp);
            


            InsertBytes(finalBytes, valuesBytes);

            //return finalBytes.ToArray();
        }

        private static void PackMessage(OSCMessage message, byte[] buffer, ref int index)
        {
            var typeTags = ",";

            var values = message.Values;
            var valuesIndex = 0;
            var valuesCount = values.Count;

            for (var i = 0; i < valuesCount; ++i)
            {
                ProcessValue(ref typeTags, _valuesBuffer, ref valuesIndex, values[i]);
            }

            var headerIndex = 0;
            PackValue(_headerBuffer, ref headerIndex, OSCValueType.String, message.Address);
            PackValue(_headerBuffer, ref headerIndex, OSCValueType.String, typeTags);

            Buffer.BlockCopy(_headerBuffer, 0, buffer, index, _headerBuffer.Length);
            index += _headerBuffer.Length;

            Buffer.BlockCopy(_valuesBuffer, 0, buffer, index, _valuesBuffer.Length);
            index += _valuesBuffer.Length;
        }

        private static void ProcessValue(ref string typeTags, byte[] buffer, ref int index, OSCValue value)
        {
            if (value.Type == OSCValueType.Array)
            {
                typeTags += '[';

                var array = value.ArrayValue;
                var arraySize = array.Count;

                for (var i = 0; i < arraySize; ++i)
                {
                    ProcessValue(ref typeTags, buffer, ref index, array[i]);
                }
                
                typeTags += ']';

                return;
            }

            typeTags += value.Tag;
            PackValue(buffer, ref index, value.Type, value);
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
                    var valueType = OSCValue.GetValueType(valueTag);
                    var @object = UnpackValue(valueType, bytes, ref start);

                    value = new OSCValue(valueType, @object); // TODO: Make pool.
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
        private static void PackValue(byte[] buffer, ref int index, OSCValueType valueType, object value)
        {
            if (!_packersDictionary.ContainsKey(valueType))
                throw new Exception("[OSCConverter.PackValue] Unknown value type: '" + valueType + "'.");

            _packersDictionary[valueType].Pack(buffer, ref index, value);
        }

        /*
        private static byte[] PackValue(OSCValueType valueType, object value)
        {
            if (!_packersDictionary.ContainsKey(valueType))
                throw new Exception("[OSCConverter.PackValue] Unknown value type: '" + valueType +"'.");

            //_packersDictionary[valueType].Pack(b);
            
            return default(byte[]);
        }*/

        //  UNPACK VALUE
        private static object UnpackValue(OSCValueType valueType, byte[] bytes, ref int start)
        {
            if (!_packersDictionary.ContainsKey(valueType))
                throw new Exception("[OSCConverter.UnpackValue] Unknown value type: '" + valueType + "'.");

            return _packersDictionary[valueType].Unpack(bytes, ref start);
        }

        //  External
        private static void InsertBytes(IList data, IEnumerable bytes)
        {
            // TODO: Lol.
            if (bytes == null) return;
            foreach (var b in bytes) data.Add(b);
        }

        #endregion
    }
}