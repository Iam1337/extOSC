/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System.Collections.Generic;

using extOSC.Core;

namespace extOSC.Mapping
{
    public class OSCMapBundle : ScriptableObject
    {
        #region Public Vars

        public List<OSCMapMessage> Messages
        {
            get { return messages; }
            set { messages = value; }
        }

        #endregion

        #region Protected Vars

        [SerializeField]
        protected List<OSCMapMessage> messages = new List<OSCMapMessage>();

        #endregion

        #region Public Methods

        public void Map(IOSCPacket ioscPacket)
        {
            if (ioscPacket is OSCBundle)
            {
                var bundle = ioscPacket as OSCBundle;

                foreach (var bundlePacket in bundle.Packets)
                    Map(bundlePacket);
            }
            else if (ioscPacket is OSCMessage)
            {
                var message = ioscPacket as OSCMessage;

                foreach (var mapMessage in messages)
                    mapMessage.Map(message);
            }
        }

        #endregion
    }
}