/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components
{
    public abstract class OSCComponent : MonoBehaviour, IOSCReceiverComponent, IOSCTransmitterComponent
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

        public virtual string ReceiverAddress
        {
            get { return receiverAddress; }
            set
            {
                if (receiverAddress == value)
                    return;

                Unbind();

                receiverAddress = value;

                Bind();
            }
        }

        public OSCTransmitter Transmitter
        {
            get { return transmitter; }
            set { transmitter = value; }
        }

        public virtual string TransmitterAddress
        {
            get { return transmitterAddress; }
            set { transmitterAddress = value; }
        }

		#endregion

		#region Protected Vars

		[OSCSelector]
		[SerializeField]
        protected OSCReceiver receiver;

	    [OSCSelector]
		[SerializeField]
        protected OSCTransmitter transmitter;

        [SerializeField]
        protected string receiverAddress = "/address/receiver";

        [SerializeField]
        protected string transmitterAddress = "/address/transmitter";

        protected OSCBind receiverBind;

        protected OSCReceiver bindedReceiver;

        #endregion

        #region Private Vars

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

        public void Send()
        {
            var message = new OSCMessage(transmitterAddress);

            if (FillMessage(message))
            {
                if (transmitter != null)
                    transmitter.Send(message);
            }
        }

        public void Bind()
        {
            if (receiverBind == null || receiverBind.ReceiverAddress != receiverAddress)
            {
                Unbind();

                receiverBind = new OSCBind(receiverAddress, InvokeMessage);
            }

            bindedReceiver = receiver;

            if (bindedReceiver != null)
                bindedReceiver.Bind(receiverBind);
        }

        public void Unbind()
        {
            if (bindedReceiver != null && receiverBind != null)
                bindedReceiver.Unbind(receiverBind);

            bindedReceiver = null;
        }

        #endregion

        #region Protected Methods

        protected abstract void Invoke(OSCMessage message);

        protected abstract bool FillMessage(OSCMessage message);

        #endregion

        #region Private Methods

        private void InvokeMessage(OSCMessage message)
        {
            if (!enabled) return;

            Invoke(message);
        }

        #endregion
    }
}