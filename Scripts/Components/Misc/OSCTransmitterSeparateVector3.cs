/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Mapping;
using extOSC.Core.Reflection;

namespace extOSC.Components.Misc
{
	[AddComponentMenu("extOSC/Components/Transmitter/Vector3 Separate")]
	public class OSCTransmitterSeparateVector3 : MonoBehaviour
	{
		#region Public Vars

		public OSCTransmitter Transmitter
		{
			get => _transmitter;
			set => _transmitter = value;
		}

		public OSCMapBundle MapBundle
		{
			get => _mapBundle;
			set => _mapBundle = value;
		}

		public OSCReflectionMember ReflectionTarget
		{
			get => _reflectionMember;
			set
			{
				_reflectionMember = value;
				UpdateCachedReferences();
			}
		}

		public string AddressX = "/address/x";

		public string AddressY = "/address/y";

        public string AddressZ = "/address/z";

		#endregion

		#region Private Vars

		[OSCSelector]
		[SerializeField]
		private OSCTransmitter _transmitter;

		[SerializeField]
		private OSCMapBundle _mapBundle;

		[SerializeField]
#pragma warning disable 649
		private OSCReflectionMember _reflectionMember;
#pragma warning restore 649

		[SerializeField]
		private bool _informOnChanged = true;

		[SerializeField]
		private float _informInterval = 0;

		private OSCReflectionProperty _cachedProperty;

		private Vector3 _previousValue;

		private float _sendTimer;

		#endregion

		#region Unity Methods

		protected void Awake()
		{
			UpdateCachedReferences();

			if (_cachedProperty != null)
				_previousValue = (Vector3) (_cachedProperty.GetValue() ?? Vector3.zero);
		}

#if UNITY_EDITOR
		protected void OnValidate()
		{
			UpdateCachedReferences();
		}
#endif

		protected virtual void Update()
		{
			if (_cachedProperty == null) return;

			if (_informOnChanged)
			{
				var currentValue = (Vector3) _cachedProperty.Value;

				if (!currentValue.Equals(_previousValue))
				{
					Send();

					_previousValue = currentValue;
				}
			}
			else
			{
				_informInterval = Mathf.Max(_informInterval, 0);
				if (_informInterval < float.Epsilon)
				{
					Send();
				}
				else
				{
					_sendTimer += Time.deltaTime;

					if (_informInterval < _sendTimer)
					{
						Send();

						_sendTimer = 0;
					}
				}
			}
		}

		#endregion

		#region Public Methods

		public void Send()
		{
			var vector = (Vector3) (_cachedProperty.GetValue() ?? Vector3.zero);

			Transmitter.Send(OSCMessage.Create(AddressX, OSCValue.Float(vector.x)));
			Transmitter.Send(OSCMessage.Create(AddressY, OSCValue.Float(vector.y)));
			Transmitter.Send(OSCMessage.Create(AddressZ, OSCValue.Float(vector.z)));
		}

		#endregion

		#region Private Methods

		private void UpdateCachedReferences()
		{
			if (_reflectionMember != null && _reflectionMember.IsValid())
			{
				_cachedProperty = _reflectionMember.GetProperty();
			}
			else
			{
				_cachedProperty = null;
			}
		}

		#endregion
	}
}