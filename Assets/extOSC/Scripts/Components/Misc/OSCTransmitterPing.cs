/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.Serialization;

namespace extOSC.Components.Misc
{
	[AddComponentMenu("extOSC/Components/Misc/Ping")]
	public class OSCTransmitterPing : OSCTransmitterComponent
	{
		#region Public Vars

		public bool AutoStart
		{
			get => _autoStart;
			set => _autoStart = value;
		}

		public float Interval
		{
			get => _interval;
			set
			{
				_interval = value;

				if (_interval < 0)
					_interval = 0;
			}
		}

		public bool IsRunning => _isRunning;

		#endregion

		#region Private Vars

		[Range(0, 60)]
		[SerializeField]
		[FormerlySerializedAs("interval")]
		private float _interval = 1;

		[SerializeField]
		[FormerlySerializedAs("autoStart")]
		private bool _autoStart = true;

		private float _timer;

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
				_timer = 0;

				Send();
			}
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
			return true;
		}

		#endregion
	}
}