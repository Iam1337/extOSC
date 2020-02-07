/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

using extOSC.Core;
using extOSC.Core.Events;

namespace extOSC
{
	public class OSCBind : IOSCBind
	{
		#region Public Vars

		public string ReceiverAddress => _address;

		public OSCEventMessage Callback
		{
			get => _callback;
			set => _callback = value;
		}

		#endregion

		#region Protected Vars

		[SerializeField]
		[FormerlySerializedAs("address")]
		private string _address;

		[SerializeField]
		[FormerlySerializedAs("callback")]
		private OSCEventMessage _callback = new OSCEventMessage();

		#endregion

		#region Public Methods

		public OSCBind(string address, UnityAction<OSCMessage> callback)
		{
			_address = address;
			_callback.AddListener(callback);
		}

		#endregion
	}
}