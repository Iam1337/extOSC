/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
    public abstract class OSCReceiverEvent : OSCReceiverComponent
    {
        #region Public Vars

        public abstract Type ReceiverType { get; }

        #endregion
    }

    public abstract class OSCReceiverEvent<T> : OSCReceiverEvent where T : IOSCEvent
    {
        #region Public Vars

        public override Type ReceiverType
        {
            get { return typeof(T); }
        }

        public T OnReceive
        {
            get { return onReceive; }
            set { onReceive = value; }
        }

        #endregion

        #region Protected Vars

        [SerializeField]
        protected T onReceive;

        #endregion
    }
}