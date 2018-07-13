/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Mapping;

namespace extOSC.Components
{
    public abstract class OSCTransmitterComponent : MonoBehaviour, IOSCTransmitterComponent
    {
        #region Public Vars

        public OSCTransmitter Transmitter
        {
            get { return transmitter; }
            set { transmitter = value; }
        }

	    [System.Obsolete("\"Address\" is deprecated, please use \"TransmitterAddress\" instead.")]
	    public virtual string Address
	    {
		    get { return TransmitterAddress; }
		    set { TransmitterAddress = value; }
	    }

		public virtual string TransmitterAddress
        {
            get { return address; }
            set { address = value; }
        }

		public OSCMapBundle MapBundle
	    {
		    get { return mapBundle; }
		    set { mapBundle = value; }
	    }

		#endregion

		#region Protected Vars

		[SerializeField]
        protected OSCTransmitter transmitter;

        [SerializeField]
        protected string address = "/address";

	    [SerializeField]
	    protected OSCMapBundle mapBundle;

		#endregion

		#region Public Methods

		public void Send()
        {
            var message = new OSCMessage(address);

            if (FillMessage(message))
            {
				if (mapBundle != null)
					mapBundle.Map(message);

                if (transmitter != null)
                    transmitter.Send(message);
            }
        }

        #endregion

        #region Protected Methods

        protected abstract bool FillMessage(OSCMessage message);

        #endregion
    }
}