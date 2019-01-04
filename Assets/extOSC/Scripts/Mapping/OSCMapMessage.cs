/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System;
using System.Collections.Generic;

namespace extOSC.Mapping
{
    [Serializable]
    public class OSCMapMessage
    {
        #region Public Vars

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public List<OSCMapValue> Values
        {
            get { return values; }
            set { values = value; }
        }

        #endregion

        #region Protected Vars

        [SerializeField]
        protected string address = "/address";

        [SerializeField]
        protected List<OSCMapValue> values = new List<OSCMapValue>();

        #endregion

        #region Public Methods

        public void Map(OSCMessage message)
        {
            if (OSCUtilities.CompareAddresses(Address, message.Address) &&
                        message.Values.Count == Values.Count)
            {
                var failed = false;

                for (int index = 0; index < message.Values.Count; index++)
                {
                    var mapMessageValue = Values[index];
                    var messageValue = message.Values[index];

                    var valueType = messageValue.Type != OSCValueType.False ? messageValue.Type : OSCValueType.True;

                    if (messageValue.Type != valueType)
                    {
                        failed = true;
                        break;
                    }
                }

                if (failed) return;

                for (int index = 0; index < message.Values.Count; index++)
                {
                    var messageValue = message.Values[index];
                    var mapMessageValue = Values[index];

                    message.Values[index] = mapMessageValue.Map(messageValue);
                }
            }
        }

        #endregion
    }
}