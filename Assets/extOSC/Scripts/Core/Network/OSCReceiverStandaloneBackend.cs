/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

#if !UNITY_WSA || UNITY_EDITOR

using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;

namespace extOSC.Core.Network
{
	internal class OSCReceiverStandaloneBackend : OSCReceiverBackend
	{
		#region Public Vars

		public override OSCReceivedCallback ReceivedCallback
		{
			get => _receivedCallback;
			set => _receivedCallback = value;
		}

		public override bool IsAvailable => _client != null;

		public override bool IsRunning => _isRunning;

		#endregion

		#region Private Vars

		private bool _isRunning;

		private UdpClient _client;

		private AsyncCallback _controllerThreadAsync;

		private OSCReceivedCallback _receivedCallback;

		#endregion

		#region Public Methods

		public override void Connect(string localHost, int localPort)
		{
			if (_client != null)
				Close();

			try
			{
				_client = OSCStandaloneManager.Create(localHost, localPort);

				_controllerThreadAsync = ControllerThread;
				_client.BeginReceive(_controllerThreadAsync, _client);

				_isRunning = true;
			}
			catch (SocketException e)
			{
				if (e.ErrorCode == 10048)
				{
					Debug.LogErrorFormat($"[OSCReceiver] Socket Error: Could not use port {localPort} because another application is listening on it.");
				}
				else if (e.ErrorCode == 10049)
				{
					Debug.LogError($"[OSCReceiver] Socket Error: Could not use local host \"{localHost}\". Cannot assign requested address.");
				}
				else
				{
					Debug.LogErrorFormat($"[OSCReceiver] Socket Error: Error Code {e.ErrorCode}.\n{e.Message}");
				}

				Close();
			}
			catch (ArgumentOutOfRangeException)
			{
				Debug.LogErrorFormat($"[OSCReceiver] Invalid port: {localPort}!");

				Close();
			}
			catch (Exception e)
			{
				Debug.LogErrorFormat($"[OSCReceiver] Error: {e}");

				Close();
			}
		}

		public override void Close()
		{
			if (_client != null)
				OSCStandaloneManager.Close(_client);

			_isRunning = false;
			_client = null;
		}

		#endregion

		#region Protected Methods

		protected void ControllerThread(IAsyncResult result)
		{
			if (!_isRunning) return;

			try
			{
				var receivedClient = result.AsyncState as UdpClient;
				if (receivedClient == null) return;

				var packet = Receive(receivedClient, result);
				if (packet != null)
				{
					if (_receivedCallback != null)
						_receivedCallback.Invoke(packet);
				}

				if (IsAvailable)
					receivedClient.BeginReceive(_controllerThreadAsync, receivedClient);
			}
			catch (ObjectDisposedException)
			{ }
			catch (Exception e)
			{
				Debug.LogError($"[OSCReceiver] Error: {e}");
			}
		}

		protected IOSCPacket Receive(UdpClient receivedClient, IAsyncResult result)
		{
			try
			{
				IPEndPoint ip = null;
				var bytes = receivedClient.EndReceive(result, ref ip);

				if (bytes != null && bytes.Length > 0)
				{
					var packet = OSCConverter.Unpack(bytes);
					if (packet != null)
					{
						packet.Ip = ip.Address;
						packet.Port = ip.Port;
					}

					return packet;
				}
			}
			catch (ObjectDisposedException)
			{ }
			catch (Exception e)
			{
				Debug.LogError($"[OSCReceiver] Receive error: {e}");
			}

			return null;
		}

		#endregion
	}
}

#endif