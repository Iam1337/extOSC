/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.Serialization;

namespace extOSC.Components
{
	public abstract class OSCComponent : MonoBehaviour
	{
		#region Public Vars

		public OSCReceiver Receiver
		{
			get => _receiver;
			set
			{
				if (_receiver == value)
					return;

				Unbind();
				_receiver = value;
				Bind();
			}
		}

		public virtual string ReceiverAddress
		{
			get => _receiverAddress;
			set
			{
				if (_receiverAddress == value)
					return;

				Unbind();
				_receiverAddress = value;
				Bind();
			}
		}

		public OSCTransmitter Transmitter
		{
			get => _transmitter;
			set => _transmitter = value;
		}

		public virtual string TransmitterAddress
		{
			get => _transmitterAddress;
			set => _transmitterAddress = value;
		}

		#endregion

		#region Private Vars

		[OSCSelector]
		[SerializeField]
		[FormerlySerializedAs("receiver")]
		private OSCReceiver _receiver;

		[OSCSelector]
		[SerializeField]
		[FormerlySerializedAs("transmitter")]
		private OSCTransmitter _transmitter;

		[SerializeField]
		[FormerlySerializedAs("receiverAddress")]
		private string _receiverAddress = "/address/receiver";

		[SerializeField]
		[FormerlySerializedAs("transmitterAddress")]
		private string _transmitterAddress = "/address/transmitter";

		private OSCBind _receiverBind;

		private OSCReceiver _bindedReceiver;

		#endregion

		#region Unity Methods

		protected virtual void OnEnable()
		{
			Bind();
		}

		protected virtual void OnDisable()
		{
			Unbind();
		}

#if UNITY_EDITOR
		protected virtual void OnValidate()
		{
			if (Application.isPlaying)
			{
				Unbind();
				Bind();
			}
		}
#endif

		#endregion

		#region Public Methods

		public void Send()
		{
			var message = new OSCMessage(_transmitterAddress);

			if (FillMessage(message))
			{
				if (_transmitter != null)
					_transmitter.Send(message);
			}
		}

		public void Bind()
		{
			if (_receiverBind == null || _receiverBind.ReceiverAddress != _receiverAddress)
			{
				Unbind();

				_receiverBind = new OSCBind(_receiverAddress, InvokeMessage);
			}

			_bindedReceiver = _receiver;

			if (_bindedReceiver != null)
				_bindedReceiver.Bind(_receiverBind);
		}

		public void Unbind()
		{
			if (_bindedReceiver != null && _receiverBind != null)
				_bindedReceiver.Unbind(_receiverBind);

			_bindedReceiver = null;
		}

		#endregion

		#region Protected Methods

		protected abstract void Invoke(OSCMessage message);

		protected abstract bool FillMessage(OSCMessage message);

		#endregion

		#region Private Methods

		private void InvokeMessage(OSCMessage message)
		{
			if (!enabled) return;

			Invoke(message);
		}

		#endregion
	}
}