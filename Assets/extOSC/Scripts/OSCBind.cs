/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.Events;

using extOSC.Core;
using extOSC.Core.Events;

namespace extOSC
{
    public class OSCBind : IOSCBind
    {
        #region Public Vars

        public string ReceiverAddress
        {
            get { return address; }
        }

        public OSCEventMessage Callback
        {
            get { return callback; }
            set { callback = value; }
        }

        #endregion

        #region Protected Vars

        [SerializeField]
        protected string address;

        [SerializeField]
        protected OSCEventMessage callback = new OSCEventMessage();

        #endregion

        #region Public Methods

        public OSCBind(string address)
        {
            this.address = address;
        }

        public OSCBind(string address, UnityAction<OSCMessage> callback)
        {
            this.address = address;
            this.callback.AddListener(callback);
        }

        #endregion

        #region Private Methods

        protected OSCBind() { }

        #endregion
    }
}