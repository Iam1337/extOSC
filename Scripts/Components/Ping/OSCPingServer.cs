/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Ping
{
	[AddComponentMenu("extOSC/Components/Ping/Ping Server")]
	public class OSCPingServer : OSCComponent
	{
		#region Protected Methods

		protected void Awake()
		{
			// Idk, how to make this better! Please, halp!11 
			TransmitterAddress = "- None -";
		}

		protected override bool FillMessage(OSCMessage message)
		{
			message.AddValue(OSCValue.Impulse());

			return true;
		}

		protected override void Invoke(OSCMessage message)
		{
			var host = message.Ip.ToString();

			if (message.ToString(out var address) && message.ToInt(out var port))
			{
				TransmitterAddress = address;

				var backupUseBundle = Transmitter.UseBundle;
				var backupRemoteHost = Transmitter.RemoteHost;
				var backupRemotePort = Transmitter.RemotePort;

				Transmitter.UseBundle = false;
				Transmitter.RemoteHost = host;
				Transmitter.RemotePort = port;

				Send();

				Transmitter.UseBundle = backupUseBundle;
				Transmitter.RemoteHost = backupRemoteHost;
				Transmitter.RemotePort = backupRemotePort;
			}
		}

		#endregion
	}
}