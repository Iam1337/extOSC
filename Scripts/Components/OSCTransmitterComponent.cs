/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.Serialization;

using extOSC.Mapping;

namespace extOSC.Components
{
	public abstract class OSCTransmitterComponent : MonoBehaviour
	{
		#region Public Vars

		public OSCTransmitter Transmitter
		{
			get => _transmitter;
			set => _transmitter = value;
		}

		public virtual string TransmitterAddress
		{
			get => _address;
			set => _address = value;
		}

		public OSCMapBundle MapBundle
		{
			get => _mapBundle;
			set => _mapBundle = value;
		}

		#endregion

		#region Private Vars

		[OSCSelector]
		[SerializeField]
		[FormerlySerializedAs("transmitter")]
		private OSCTransmitter _transmitter;

		[SerializeField]
		[FormerlySerializedAs("address")]
		private string _address = "/address";

		[SerializeField]
		[FormerlySerializedAs("mapBundle")]
		private OSCMapBundle _mapBundle;

		#endregion

		#region Public Methods

		public void Send()
		{
			var message = new OSCMessage(_address);

			if (FillMessage(message))
			{
				if (_mapBundle != null)
					_mapBundle.Map(message);

				if (_transmitter != null)
					_transmitter.Send(message);
			}
		}

		#endregion

		#region Protected Methods

		protected abstract bool FillMessage(OSCMessage message);

		#endregion
	}
}