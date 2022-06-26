/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

#if !UNITY_WSA || UNITY_EDITOR

using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;

namespace extOSC.Core.Network
{
	internal class OSCTransmitterStandaloneBackend : OSCTransmitterBackend
	{
		#region Public Vars

		public override bool IsAvailable => _client != null;

        #endregion

		#region Private Vars

		private IPEndPoint _remoteEndPoint;

		private UdpClient _client;

		#endregion

		#region Public Methods

		public override void Connect(string localHost, int localPort)
		{
			if (_client != null)
				Close();

			try
			{
				_client = OSCStandaloneManager.Create(localHost, localPort);
			}
			catch (SocketException e)
			{
				if (e.ErrorCode == 10048)
				{
					Debug.LogError($"[OSCTransmitter] Socket Error: Could not use local port {localPort} because another application is listening on it.");
				}
				else if (e.ErrorCode == 10049)
				{
					Debug.LogError($"[OSCTransmitter] Socket Error: Could not use local host \"{localHost}\". Cannot assign requested address.");
				}
				else
				{
					Debug.LogError($"[OSCTransmitter] Socket Error: Error Code {e.ErrorCode}.\n{e.Message}");
				}

				Close();
			}
			catch (ArgumentOutOfRangeException)
			{
				Debug.LogError($"[OSCTransmitter] Invalid port: {localPort}");

				Close();
			}
			catch (Exception e)
			{
				Debug.LogError($"[OSCTransmitter] Error: {e}");

				Close();
			}
		}

		public override void RefreshRemote(string remoteHost, int remotePort)
		{
			_remoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteHost), remotePort);
		}

		public override void Close()
		{
			if (_client != null)
				OSCStandaloneManager.Close(_client);

			_client = null;
		}

		public override void Send(byte[] data, int length)
		{
			if (_client == null) return;

			try
			{
				_client.Send(data, length, _remoteEndPoint);
			}
			catch (SocketException exception)
			{
				Debug.LogWarningFormat("[OSCTransmitter] Error: {0}", exception);
			}
		}

		#endregion
	}
}

#endif