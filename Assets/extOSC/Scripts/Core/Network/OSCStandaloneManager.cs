/* Copyright (c) 2018 ExT (V.Sigalkin) */

#if !NETFX_CORE
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Collections.Generic;

namespace extOSC.Core.Network
{
	public static class OSCStandaloneManager
	{
		#region Extensions

		private class ClientData
		{
			public UdpClient Client;

			public int Links;
		}

		#endregion

		#region Static Private Vars

		private static IPEndPoint _localEndPoint;

		private static List<ClientData> _clientsData = new List<ClientData>();

		#endregion

		#region Static Public Methods

		public static UdpClient CreateClient(IPEndPoint localEndPoint)
		{
			var clientData = RequestClientData(localEndPoint);
			clientData.Links++;

			return clientData.Client;
		}

		public static void RemoveClient(UdpClient client)
		{
			var clientData = _clientsData.FirstOrDefault(c => c.Client == client);
			if (clientData == null) return;

			clientData.Links--;

			if (clientData.Links <= 0)
			{
				clientData.Client.Close();
				clientData.Client = null;

				_clientsData.Remove(clientData);
			}
		}

		public static IPEndPoint CreateLocalEndPoint(int localPort)
		{
			if (localPort == 0)
				return null;

			return new IPEndPoint(IPAddress.Any, localPort);
		}

		public static IPEndPoint CreateRemoteEndPoint(string remoteHost, int remotePort)
		{
			IPAddress ipAddress;

			if (!IPAddress.TryParse(remoteHost, out ipAddress))
				return null;

			return new IPEndPoint(ipAddress, remotePort);
		}

		#endregion

		#region Static Private Methods

		private static ClientData RequestClientData(IPEndPoint localEndPoint)
		{
			var clientData = (ClientData)null;

			if (localEndPoint != null)
			{
				clientData = _clientsData.FirstOrDefault(c => c.Client.Client.LocalEndPoint != null &&
				                                         c.Client.Client.LocalEndPoint.Equals(localEndPoint));
			}

			if (clientData == null)
			{
				clientData = new ClientData();

				if (localEndPoint != null)
				{
					clientData.Client = new UdpClient(localEndPoint);
				}
				else
				{
					clientData.Client = new UdpClient();
				}

				clientData.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
				clientData.Client.DontFragment = true;

				_clientsData.Add(clientData);
			}

			return clientData;
		}

		#endregion
	}
}
#endif