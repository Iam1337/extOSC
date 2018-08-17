/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Experimental
{
    // Base experimental component.
    public class OSCExperimentalComponent : MonoBehaviour
    {
        #region Protected Vars

        protected OSCReceiver receiver;

        protected OSCTransmitter transmitter;

        #endregion

        #region Unity Methods

        protected virtual void Awake()
        {
            receiver = FindObjectOfType<OSCReceiver>();
            transmitter = FindObjectOfType<OSCTransmitter>();
        }

        #endregion
    }
}