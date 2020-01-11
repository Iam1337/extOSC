/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

using System;
using System.Collections.Generic;

using extOSC.Core;
using extOSC.Core.Network;


namespace extOSC
{
	[AddComponentMenu("extOSC/OSC Receiver")]
	public class OSCReceiver : OSCBase
	{
		#region Public Vars

		public override bool IsStarted => _receiverBackend.IsAvailable;

		public OSCLocalHostMode LocalHostMode
		{
			get => _localHostMode;
			set
			{
				if (_localHostMode == value)
					return;

				_localHostMode = value;

				if (_receiverBackend.IsRunning && IsStarted)
				{
					Close();
					Connect();
				}
			}
		}

		public string LocalHost
		{
			get => RequestLocalHost();
			set
			{
				if (_localHost == value)
					return;

				_localHost = value;

				if (_receiverBackend.IsRunning && IsStarted)
				{
					Close();
					Connect();
				}
			}
		}

		public int LocalPort
		{
			get => _localPort;
			set
			{
				value = OSCUtilities.ClampPort(value);

				if (_localPort == value)
					return;

				_localPort = value;

				if (_receiverBackend.IsRunning && IsStarted)
				{
					Close();
					Connect();
				}
			}
		}

		#endregion

		#region Private Vars

		private OSCReceiverBackend _receiverBackend
		{
			get
			{
				if (__receiverBackend == null)
				{
					__receiverBackend = OSCReceiverBackend.Create();
					__receiverBackend.ReceivedCallback = PacketReceived;
				}

				return __receiverBackend;
			}
		}

		[SerializeField]
		[FormerlySerializedAs("localHostMode")]
		private OSCLocalHostMode _localHostMode = OSCLocalHostMode.Any;

		[SerializeField]
		[FormerlySerializedAs("localHost")]
		private string _localHost;

		[SerializeField]
		[FormerlySerializedAs("localPort")]
		private int _localPort = 7001;

		private readonly Queue<IOSCPacket> _packets = new Queue<IOSCPacket>();

		private readonly List<IOSCBind> _bindings = new List<IOSCBind>();

		private readonly Stack<IOSCBind> _bindStack = new Stack<IOSCBind>();

		private readonly Stack<IOSCBind> _unbindStack = new Stack<IOSCBind>();

		private readonly object _lock = new object();

		private OSCReceiverBackend __receiverBackend;

		private bool _processMessage;

		#endregion

		#region Unity Methods

		protected virtual void Update()
		{
			if (!IsStarted || !_receiverBackend.IsRunning) return;

			lock (_lock)
			{
				while (_packets.Count > 0)
				{
					var packet = _packets.Dequeue();

					if (MapBundle != null)
						MapBundle.Map(packet);

					OSCConsole.Received(this, packet);

					InvokePacket(packet);
				}
			}
		}

#if UNITY_EDITOR
		protected void OnValidate()
		{
			if (string.IsNullOrEmpty(_localHost))
				_localHost = OSCUtilities.GetLocalHost();

			_localPort = OSCUtilities.ClampPort(_localPort);

			if (_receiverBackend.IsRunning && IsStarted)
			{
				Close();
				Connect();
			}
		}
#endif

		#endregion

		#region Public Methods

		public override string ToString()
		{
			return $"<{GetType().Name} (LocalHost: {_localHost} LocalPort: {_localPort})>";
		}

		public override void Connect()
		{
			_receiverBackend.Connect(RequestLocalHost(), _localPort);
		}

		public override void Close()
		{
			if (_receiverBackend.IsAvailable)
				_receiverBackend.Close();
		}

		public void Bind(IOSCBind bind)
		{
			if (bind == null) return;

			if (string.IsNullOrEmpty(bind.ReceiverAddress))
			{
				Debug.Log("[OSCReceiver] Address can not be empty!");
				return;
			}

			if (_processMessage)
			{
				_bindStack.Push(bind);

				return;
			}

			if (!_bindings.Contains(bind))
				_bindings.Add(bind);
		}

		public OSCBind Bind(string address, UnityAction<OSCMessage> callback)
		{
			var bind = new OSCBind(address, callback);

			Bind(bind);

			return bind;
		}

		public void Unbind(IOSCBind bind)
		{
			if (bind == null) return;

			if (_processMessage)
			{
				_unbindStack.Push(bind);

				return;
			}

			if (_bindings.Contains(bind))
				_bindings.Remove(bind);
		}

		public void ClearBinds()
		{
			_bindings.Clear();
		}

		[Obsolete("Use ClearBinds() method.")]
		public void UnbindAll()
		{
			ClearBinds();
		}

		#endregion

		#region Private Methods

		private void InvokePacket(IOSCPacket packet)
		{
			if (packet.IsBundle())
			{
				InvokeBundle(packet as OSCBundle);
			}
			else
			{
				InvokeMessage(packet as OSCMessage);
			}
		}

		private void InvokeBundle(OSCBundle bundle)
		{
			if (bundle == null) return;

			foreach (var packet in bundle.Packets)
			{
				InvokePacket(packet);
			}
		}

		private void InvokeMessage(OSCMessage message)
		{
			if (message == null) return;

			_bindStack.Clear();
			_bindStack.Clear();

			_processMessage = true;

			foreach (var bind in _bindings)
			{
				if (bind == null) continue;

				if (OSCUtilities.CompareAddresses(bind.ReceiverAddress, message.Address))
				{
					if (bind.Callback != null)
						bind.Callback.Invoke(message);
				}
			}

			_processMessage = false;

			while (_bindStack.Count > 0)
			{
				Bind(_bindStack.Pop());
			}

			while (_unbindStack.Count > 0)
			{
				Unbind(_unbindStack.Pop());
			}
		}

		private void PacketReceived(IOSCPacket packet)
		{
			lock (_lock)
			{
				_packets.Enqueue(packet);
			}
		}


		private string RequestLocalHost()
		{
			return _localHostMode == OSCLocalHostMode.Any ? "0.0.0.0" : _localHost;
		}

		#endregion
	}
}