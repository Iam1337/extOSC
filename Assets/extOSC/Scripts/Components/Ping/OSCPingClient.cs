/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEngine.Serialization;

namespace extOSC.Components.Ping
{
	[AddComponentMenu("extOSC/Components/Ping/Ping Client")]
	public class OSCPingClient : OSCComponent
	{
		#region Public Vars

		public float Interval
		{
			get => _interval;
			set => _interval = value;
		}

		public float Timeout
		{
			get => _timeout;
			set => _timeout = value;
		}

		public bool AutoStart
		{
			get => _autoStart;
			set => _autoStart = value;
		}

		public bool IsAvailable => _isAvailable;

		public float Timer => _timer;

		public float LastReceiveTime => _lastReceiveTime;

		public bool IsRunning => _isRunning;

		#endregion

		#region Private Vars

		[SerializeField]
		[FormerlySerializedAs("interval")]
		private float _interval;

		[SerializeField]
		[FormerlySerializedAs("timeout")]
		private float _timeout;

		[SerializeField]
		[FormerlySerializedAs("autoStart")]
		private bool _autoStart = true;

		private float _timer;

		private float _lastReceiveTime;

		private bool _isAvailable;

		private bool _isRunning;

		#endregion

		#region Unity Methods

		protected virtual void Start()
		{
			if (_autoStart) StartPing();
		}

		protected virtual void Update()
		{
			if (!_isRunning) return;

			_timer += Time.deltaTime;

			if (_timer >= _interval)
			{
				Send();

				_timer = 0;
			}

			_lastReceiveTime += Time.deltaTime;
			_isAvailable = _timeout > _lastReceiveTime;
		}

		#endregion

		#region Public Methods

		public void StartPing()
		{
			_isRunning = true;
		}

		public void StopPing()
		{
			_isRunning = false;
			_timer = 0;
		}

		public void PausePing()
		{
			_isRunning = false;
		}

		#endregion

		#region Protected Methods

		protected override bool FillMessage(OSCMessage message)
		{
			if (Receiver == null) return false;

			message.AddValue(OSCValue.String(ReceiverAddress));
			message.AddValue(OSCValue.Int(Receiver.LocalPort));

			return true;
		}

		protected override void Invoke(OSCMessage message)
		{
			if (message.HasImpulse())
				_lastReceiveTime = 0;
		}

		#endregion
	}
}