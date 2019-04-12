/* Copyright (c) 2019 ExT (V.Sigalkin) */

namespace extOSC.Components
{
    public interface IOSCReceiverComponent
    {
		#region Public Vars

	    OSCReceiver Receiver { get; }

        string ReceiverAddress { get; }

        #endregion

        #region Public Methods

        void Bind();

        void Unbind();

        #endregion
    }
}