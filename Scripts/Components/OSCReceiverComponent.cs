/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core;
using extOSC.Core.Events;
using extOSC.Mapping;

namespace extOSC.Components
{
    public abstract class OSCReceiverComponent : MonoBehaviour, IOSCBind, IOSCReceiverComponent
    {
        #region Public Vars

        public OSCReceiver Receiver
        {
            get { return receiver; }
            set
            {
                if (receiver == value)
                    return;

                Unbind();

                receiver = value;

                Bind();
            }
        }

        public string ReceiverAddress
        {
            get { return address; }
            set
            {
                if (address == value)
                    return;

                Unbind();

                address = value;

                Bind();
            }
        }

	    public OSCMapBundle MapBundle
	    {
		    get { return mapBundle; }
		    set { mapBundle = value; }
	    }

        public OSCEventMessage Callback
        {
            get
            {
                if (callback == null)
                {
                    callback = new OSCEventMessage();
                    callback.AddListener(InvokeMessage);
                }

                return callback;
            }
        }

		#endregion

		#region Protected Vars

		[OSCSelector]
		[SerializeField]
        protected OSCReceiver receiver;

        [SerializeField]
        protected string address = "/address";

		[SerializeField]
	    protected OSCMapBundle mapBundle;

        protected OSCEventMessage callback;

        protected OSCReceiver bindedReceiver;

        #endregion

        #region Unity Methods

        protected virtual void OnEnable()
        {
            Bind();
        }

        protected virtual void OnDisable()
        {
            Unbind();
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (Application.isPlaying)
            {
                Unbind();
                Bind();
            }
        }
#endif

        #endregion

        #region Public Methods

        public virtual void Bind()
        {
            if (receiver != null)
                receiver.Bind(this);

            bindedReceiver = receiver;
        }

        public virtual void Unbind()
        {
            if (bindedReceiver != null)
                bindedReceiver.Unbind(this);

            bindedReceiver = null;
        }

        #endregion

        #region Protected Methods

        protected abstract void Invoke(OSCMessage message);

        #endregion

        #region Private Methods

        private void InvokeMessage(OSCMessage message)
        {
            if (!enabled) return;

			if (mapBundle != null)
				mapBundle.Map(message);

            Invoke(message);
        }

        #endregion
    }
}