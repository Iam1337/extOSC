/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.Serialization;

using System;
using System.Collections.Generic;

namespace extOSC.Mapping
{
	[Serializable]
	public class OSCMapMessage
	{
		#region Public Vars

		public string Address
		{
			get => _address;
			set => _address = value;
		}

		public List<OSCMapValue> Values
		{
			get => _values;
			set => _values = value;
		}

		#endregion

		#region Protected Vars

		[SerializeField]
		[FormerlySerializedAs("address")]
		private string _address = "/address";

		[SerializeField]
		[FormerlySerializedAs("values")]
		private List<OSCMapValue> _values = new List<OSCMapValue>();

		#endregion

		#region Public Methods

		public void Map(OSCMessage message)
		{
			if (OSCUtilities.CompareAddresses(Address, message.Address) && message.Values.Count == Values.Count)
			{
				var failed = false;

				for (var i = 0; i < message.Values.Count; i++)
				{
					var messageValue = message.Values[i];
					var valueType = messageValue.Type != OSCValueType.False ? messageValue.Type : OSCValueType.True;

					if (messageValue.Type != valueType)
					{
						failed = true;
						break;
					}
				}

				if (failed) return;

				for (var i = 0; i < message.Values.Count; i++)
				{
					var messageValue = message.Values[i];
					var mapMessageValue = Values[i];

					message.Values[i] = mapMessageValue.Map(messageValue);
				}
			}
		}

		#endregion
	}
}