/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System.Collections.Generic;

using extOSC.Core;

namespace extOSC
{
    public class OSCMessage : OSCPacket
    {
        #region Static Public Methods

        public static OSCMessage Create(string address, params OSCValue[] values)
        {
            return new OSCMessage(address, values);
        }

        #endregion

        #region Public Vars

        public List<OSCValue> Values
        {
            get { return values; }
            set
            {
                if (values == value)
                    return;

                values = value;
            }
        }

        #endregion

        #region Protected Vars

        protected List<OSCValue> values = new List<OSCValue>();

        #endregion

        #region Public Methods

        public OSCMessage(string address) : this(address, null) { }

        public OSCMessage(string address, params OSCValue[] values)
        {
            this.address = address;

            if (values != null)
            {
                foreach (var value in values)
                    AddValue(value);
            }
        }

        public void AddValue(OSCValue value)
        {
            if (value != null)
                values.Add(value);
        }

        public OSCValue[] GetValues(params OSCValueType[] types)
        {
            var tempValues = new List<OSCValue>();

            foreach (var value in values)
            {
                foreach (var type in types)
                {
                    if (value.Type == type)
                    {
                        tempValues.Add(value);
                    }
                }
            }

            return tempValues.ToArray();
        }

        public OSCValueType[] GetTypes()
        {
            var types = new OSCValueType[values.Count];

            for (var i = 0; i < values.Count; i++)
            {
                types[i] = values[i].Type;
            }

            return types;
        }

        public string GetTags()
        {
            var tags = string.Empty;

            foreach (var value in values)
            {
                tags += value.Tag;
            }

            return tags;
        }

	    public override string ToString()
        {
            var stringValues = string.Empty;

            if (values.Count > 0)
            {
                foreach (var value in values)
                {
                    stringValues += string.Format("{0}({1}) : \"{2}\", ", value.GetType().Name, value.Type, value.Value);
                }

                stringValues = string.Format("({0})", stringValues.Remove(stringValues.Length - 2));
            }

            return string.Format("<{0}:\"{1}\"> : {2}", GetType().Name, address, string.IsNullOrEmpty(stringValues) ? "null" : stringValues);
        }

        #endregion
    }
}