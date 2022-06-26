/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEngine.Serialization;

using extOSC.Core;
using extOSC.Core.Events;
using extOSC.Mapping;

namespace extOSC.Components
{
	public abstract class OSCReceiverComponent : MonoBehaviour, IOSCBind
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

		public string ReceiverAddress
		{
			get => _address;
			set
			{
				if (_address == value)
					return;

				Unbind();
				_address = value;
				Bind();
			}
		}

		public OSCMapBundle MapBundle
		{
			get => _mapBundle;
			set => _mapBundle = value;
		}

		public OSCEventMessage Callback
		{
			get
			{
				if (_callback == null)
				{
					_callback = new OSCEventMessage();
					_callback.AddListener(InvokeMessage);
				}

				return _callback;
			}
		}

		#endregion

		#region Protected Vars

		[OSCSelector]
		[SerializeField]
		[FormerlySerializedAs("receiver")]
		private OSCReceiver _receiver;

		[SerializeField]
		[FormerlySerializedAs("address")]
		private string _address = "/address";

		[SerializeField]
		[FormerlySerializedAs("mapBundle")]
		private OSCMapBundle _mapBundle;

		private OSCEventMessage _callback;

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

		public virtual void Bind()
		{
			if (_receiver != null)
				_receiver.Bind(this);

			_bindedReceiver = _receiver;
		}

		public virtual void Unbind()
		{
			if (_bindedReceiver != null)
				_bindedReceiver.Unbind(this);

			_bindedReceiver = null;
		}

		#endregion

		#region Protected Methods

		protected abstract void Invoke(OSCMessage message);

		#endregion

		#region Private Methods

		private void InvokeMessage(OSCMessage message)
		{
			if (!enabled) return;

			if (_mapBundle != null)
				_mapBundle.Map(message);

			Invoke(message);
		}

		#endregion
	}
}