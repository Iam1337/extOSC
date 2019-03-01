﻿/* Copyright (c) 2019 ExT (V.Sigalkin) */

#if !NETFX_CORE

using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Collections.Generic;

namespace extOSC.Core.Network
{
    internal static class OSCStandaloneManager
	{
		#region Extensions

		private class ClientInfo
		{
			public UdpClient Client;

			public int Links;
		}

		#endregion

		#region Static Private Vars

        private static List<ClientInfo> _clientsList = new List<ClientInfo>();

		#endregion

		#region Static Public Methods

		public static UdpClient Create(string localHost, int localPort)
		{
            var localEndPoint = new IPEndPoint(IPAddress.Parse(localHost), localPort);
		    var clientInfo = _clientsList.FirstOrDefault(c => c.Client.Client.LocalEndPoint != null &&
		                                                      c.Client.Client.LocalEndPoint.Equals(localEndPoint));

            if (clientInfo == null)
		    {
		        clientInfo = new ClientInfo();
                clientInfo.Client = new UdpClient(localEndPoint);
		        clientInfo.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		        clientInfo.Client.DontFragment = true;

		        _clientsList.Add(clientInfo);
            }

		    clientInfo.Links++;

            return clientInfo.Client;
		}

		public static void Close(UdpClient client)
		{
		    var clientInfo = _clientsList.FirstOrDefault(c => c.Client == client);
            if (clientInfo == null)
                throw new Exception();

			clientInfo.Links--;

			if (clientInfo.Links <= 0)
			{
				clientInfo.Client.Close();
				clientInfo.Client = null;

				_clientsList.Remove(clientInfo);
			}
		}

        #endregion
    }
}

#endif