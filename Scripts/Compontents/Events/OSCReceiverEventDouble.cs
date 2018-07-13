﻿/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

namespace extOSC.Components.Events
{
    [AddComponentMenu("extOSC/Components/Receiver/Double Event")]
    public class OSCReceiverEventDouble : OSCReceiverEvent<OSCEventDouble>
    {
        #region Protected Methods

        protected override void Invoke(OSCMessage message)
        {
            double value;

            if (message.ToDouble(out value))
            {
                if (onReceive != null)
                    onReceive.Invoke(value);
            }
        }

        #endregion
    }
}