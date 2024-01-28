/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEngine.Serialization;

namespace extOSC.Components.Misc
{
	[AddComponentMenu("extOSC/Components/Misc/Gyro")]
	public class OSCReceiverGyro : OSCReceiverComponent
	{
		#region Extensions

		public enum GyroMode
		{
			Other,

			TouchOSC
		}

		#endregion

		#region Public Vars

		public float Speed
		{
			get => _speed;
			set => _speed = value;
		}

		public GyroMode Mode
		{
			get => _mode;
			set => _mode = value;
		}

		#endregion

		#region Private Vars

		[SerializeField]
		[FormerlySerializedAs("speed")]
		private float _speed = 1;

		[SerializeField]
		[FormerlySerializedAs("mode")]
		private GyroMode _mode = GyroMode.TouchOSC;

		private Quaternion _defaultRotation;

		#endregion

		#region Unity Methods

		protected virtual void Start()
		{
			_defaultRotation = transform.localRotation;
		}

		#endregion

		#region Protected Methods

		protected override void Invoke(OSCMessage message)
		{
			if (message.ToVector3(out var value))
			{
				var rotation = _mode == GyroMode.Other ? OtherProcess(value) : TouchOSCProcess(value);

				rotation *= _defaultRotation;

				transform.localRotation = Quaternion.Slerp(transform.localRotation, rotation, Time.deltaTime * Speed);
			}
		}

		protected Quaternion TouchOSCProcess(Vector3 value)
		{
			return Quaternion.Euler(value.y * 90, 0, -value.x * 90);
		}

		protected Quaternion OtherProcess(Vector3 value)
		{
			return Quaternion.Euler(value.x, value.y, value.z);
		}

		#endregion
	}
}