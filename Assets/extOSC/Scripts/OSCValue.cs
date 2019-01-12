/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System;
using System.Collections.Generic;

namespace extOSC
{
    public class OSCValue
    {
        #region Static Public Methods

        public static OSCValue Long(long value)
        {
            return new OSCValue(OSCValueType.Long, value);
        }

        public static OSCValue Char(char value)
        {
            return new OSCValue(OSCValueType.Char, value);
        }

        public static OSCValue Color(Color value)
        {
            return new OSCValue(OSCValueType.Color, value);
        }

        public static OSCValue Blob(byte[] value)
        {
            return new OSCValue(OSCValueType.Blob, value);
        }

        public static OSCValue Int(int value)
        {
            return new OSCValue(OSCValueType.Int, value);
        }

        public static OSCValue Bool(bool value)
        {
            return new OSCValue(value ? OSCValueType.True : OSCValueType.False, value);
        }

        public static OSCValue Float(float value)
        {
            return new OSCValue(OSCValueType.Float, value);
        }

        public static OSCValue Double(double value)
        {
            return new OSCValue(OSCValueType.Double, value);
        }

        public static OSCValue String(string value)
        {
            if (value == null) value = string.Empty;
            return new OSCValue(OSCValueType.String, value);
        }

        public static OSCValue Null()
        {
            return new OSCValue(OSCValueType.Null, null);
        }

        public static OSCValue Impulse()
        {
            return new OSCValue(OSCValueType.Impulse, null);
        }

        public static OSCValue TimeTag(DateTime value)
        {
            return new OSCValue(OSCValueType.TimeTag, value);
        }

        public static OSCValue Midi(OSCMidi value)
        {
            return new OSCValue(OSCValueType.Midi, value);
        }

        public static OSCValue Array(params OSCValue[] values)
        {
            return new OSCValue(OSCValueType.Array, new List<OSCValue>(values));
        }

        public static char GetTag(Type type)
        {
            return GetTag(GetValueType(type));
        }

        public static char GetTag(OSCValueType valueType)
        {
            switch (valueType)
            {
                case OSCValueType.Unknown:
                    return 'N';
                case OSCValueType.Int:
                    return 'i';
                case OSCValueType.Long:
                    return 'h';
                case OSCValueType.True:
                    return 'T';
                case OSCValueType.False:
                    return 'F';
                case OSCValueType.Float:
                    return 'f';
                case OSCValueType.Double:
                    return 'd';
                case OSCValueType.String:
                    return 's';
                case OSCValueType.Null:
                    return 'N';
                case OSCValueType.Impulse:
                    return 'I';
                case OSCValueType.Blob:
                    return 'b';
                case OSCValueType.Char:
                    return 'c';
                case OSCValueType.Color:
                    return 'r';
                case OSCValueType.TimeTag:
                    return 't';
                case OSCValueType.Midi:
                    return 'm';
                case OSCValueType.Array:
                    return 'N';
                default:
                    return 'N';
            }
        }

        public static OSCValueType GetValueType(char valueTag)
        {
            switch (valueTag)
            {
                case 'i':
                    return OSCValueType.Int;
                case 'h':
                    return OSCValueType.Long;
                case 'T':
                    return OSCValueType.True;
                case 'F':
                    return OSCValueType.False;
                case 'f':
                    return OSCValueType.Float;
                case 'd':
                    return OSCValueType.Double;
                case 's':
                    return OSCValueType.String;
                case 'N':
                    return OSCValueType.Null;
                case 'I':
                    return OSCValueType.Impulse;
                case 'b':
                    return OSCValueType.Blob;
                case 'c':
                    return OSCValueType.Char;
                case 'r':
                    return OSCValueType.Color;
                case 't':
                    return OSCValueType.TimeTag;
                case 'm':
                    return OSCValueType.Midi;
                case '[':
                    return OSCValueType.Array;
                case ']':
                    return OSCValueType.Array;
                default:
                    return OSCValueType.Unknown;
            }
        }

        public static Type GetType(OSCValueType valueType)
        {
            if (valueType == OSCValueType.Unknown)
                return null;
            if (valueType == OSCValueType.Int)
                return typeof(int);
            if (valueType == OSCValueType.Long)
                return typeof(long);
            if (valueType == OSCValueType.True)
                return typeof(bool);
            if (valueType == OSCValueType.False)
                return typeof(bool);
            if (valueType == OSCValueType.Float)
                return typeof(float);
            if (valueType == OSCValueType.Double)
                return typeof(double);
            if (valueType == OSCValueType.String)
                return typeof(string);
            if (valueType == OSCValueType.Null)
                return null;
            if (valueType == OSCValueType.Impulse)
                return null;
            if (valueType == OSCValueType.Blob)
                return typeof(byte[]);
            if (valueType == OSCValueType.Char)
                return typeof(char);
            if (valueType == OSCValueType.Color)
                return typeof(Color);
            if (valueType == OSCValueType.TimeTag)
                return typeof(DateTime);
            if (valueType == OSCValueType.Midi)
                return typeof(OSCMidi);
            if (valueType == OSCValueType.Array)
                return typeof(List<OSCValue>);

            return null;
        }

        public static OSCValueType GetValueType(Type type)
        {
            if (type == typeof(int))
                return OSCValueType.Int;
            if (type == typeof(long))
                return OSCValueType.Long;
            if (type == typeof(bool))
                return OSCValueType.False;
            if (type == typeof(float))
                return OSCValueType.Float;
            if (type == typeof(double))
                return OSCValueType.Double;
            if (type == typeof(string))
                return OSCValueType.String;
            if (type == typeof(byte[]))
                return OSCValueType.Blob;
            if (type == typeof(char))
                return OSCValueType.Char;
            if (type == typeof(Color))
                return OSCValueType.Color;
            if (type == typeof(DateTime))
                return OSCValueType.TimeTag;
            if (type == typeof(OSCMidi))
                return OSCValueType.Midi;
            if (type == typeof(List<OSCValue>))
                return OSCValueType.Array;

            return OSCValueType.Unknown;
        }


        #endregion

        #region Public Vars

        public object Value
        {
            get { return value; }
        }

        public OSCValueType Type
        {
            get { return type; }
        }

        public string Tag
        {
            get
            {
                if (type == OSCValueType.Array)
                    return GetArrayTag();

                return GetTag(type).ToString();
            }
        }

        public long LongValue
        {
            get { return GetValue<long>(OSCValueType.Long); }
            set { this.value = value; }
        }

        public char CharValue
        {
            get { return GetValue<char>(OSCValueType.Char); }
            set { this.value = value; }
        }

        public Color ColorValue
        {
            get { return GetValue<Color>(OSCValueType.Color); }
            set { this.value = value; }
        }

        public byte[] BlobValue
        {
            get { return GetValue<byte[]>(OSCValueType.Blob); }
            set { this.value = value; }
        }

        public int IntValue
        {
            get { return GetValue<int>(OSCValueType.Int); }
            set { this.value = value; }
        }

        public bool BoolValue
        {
            get { return type == OSCValueType.True; }
            set
            {
                this.value = value;
                this.type = value ? OSCValueType.True : OSCValueType.False;
            }
        }

        public float FloatValue
        {
            get { return GetValue<float>(OSCValueType.Float); }
            set { this.value = value; }
        }

        public double DoubleValue
        {
            get { return GetValue<double>(OSCValueType.Double); }
            set { this.value = value; }
        }

        public string StringValue
        {
            get { return GetValue<string>(OSCValueType.String); }
            set { this.value = value; }
        }

        public bool IsNull
        {
            get { return type == OSCValueType.Null; }
        }

        public bool IsImpulse
        {
            get { return type == OSCValueType.Impulse; }
        }

        public DateTime TimeTagValue
        {
            get { return GetValue<DateTime>(OSCValueType.TimeTag); }
            set { this.value = value; }
        }

        public OSCMidi MidiValue
        {
            get { return GetValue<OSCMidi>(OSCValueType.Midi); }
            set { this.value = value; }
        }

        public List<OSCValue> ArrayValue
        {
            get { return GetValue<List<OSCValue>>(OSCValueType.Array); }
            set { this.value = value; }
        }

        #endregion

        #region Protected Vars

        protected object value;

        protected OSCValueType type;

        #endregion

        #region Public Methods

        public OSCValue(OSCValueType type, object value)
        {
            this.value = value;
            this.type = type;
        }

        public void AddValue(OSCValue arrayValue)
        {
            if (type != OSCValueType.Array)
                throw new Exception("OSCValue must be \"Array\" type.");

            if (arrayValue == this)
                throw new Exception("OSCValue with \"Array\" type cannot store itself.");

            ArrayValue.Add(arrayValue);
        }

        public override string ToString()
        {
            if (type == OSCValueType.True || type == OSCValueType.False || type == OSCValueType.Null || type == OSCValueType.Impulse)
            {
                return string.Format("<OSCValue {0}>", Tag);
            }
            if (type == OSCValueType.Array)
            {
                var stringValues = string.Empty;

                if (ArrayValue.Count > 0)
                {
                    foreach (var arrayValue in ArrayValue)
                    {
                        stringValues += arrayValue + ", ";
                    }

                    if (stringValues.Length > 2)
                        stringValues = stringValues.Remove(stringValues.Length - 2);
                }

                return string.Format("<OSCValue Arrray [{0}]>", stringValues);
            }

            return string.Format("<OSCValue {0}: {1}>", Tag, value);
        }

        #endregion

        #region Private Methods

        private OSCValue() { }

        private T GetValue<T>(OSCValueType requiredType)
        {
            if (requiredType == type)
            {
                return (T)value;
            }

            return default(T);
        }

        private string GetArrayTag()
        {
            var arrayTag = "[";

            foreach (var arrayValue in ArrayValue)
            {
                arrayTag += arrayValue.Tag;
            }

            arrayTag += "]";

            return arrayTag;
        }

        #endregion
    }
}