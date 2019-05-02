/* Copyright (c) 2019 ExT (V.Sigalkin) */
using UnityEngine;

namespace extOSC.Components.Ping
{
    [AddComponentMenu("extOSC/Components/Ping/Ping Server")]
    public class OSCPingServer : OSCComponent
    {
        #region Public Vars

        public override string TransmitterAddress
        {
            get { return transmitterAddress; }
        }

        #endregion

        #region Protected Methods

        protected void Awake()
        {
            // Idk, how to make this better! Please, halp!11 
            transmitterAddress = "- None -";
        }

        protected override bool FillMessage(OSCMessage message)
        {
            message.AddValue(OSCValue.Impulse());

            return true;
        }

        protected override void Invoke(OSCMessage message)
        {
            var address = string.Empty;
            var host = message.Ip.ToString();
            var port = 0;

            if (message.ToString(out address) && message.ToInt(out port))
            {
                transmitterAddress = address;

                var backupUseBundle = transmitter.UseBundle;
                var backupRemoteHost = transmitter.RemoteHost;
                var backupRemotePort = transmitter.RemotePort;

                transmitter.UseBundle = false;
                transmitter.RemoteHost = host;
                transmitter.RemotePort = port;

                Send();

                transmitter.UseBundle = backupUseBundle;
                transmitter.RemoteHost = backupRemoteHost;
                transmitter.RemotePort = backupRemotePort;
            }
        }

        #endregion
    }
}