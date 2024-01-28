/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEngine.Serialization;

using System.Collections.Generic;

using extOSC.Core;

namespace extOSC.Mapping
{
	public class OSCMapBundle : ScriptableObject
	{
		#region Public Vars

		public List<OSCMapMessage> Messages
		{
			get => _messages;
			set => _messages = value;
		}

		#endregion

		#region Protected Vars

		[SerializeField]
		[FormerlySerializedAs("messages")]
		protected List<OSCMapMessage> _messages = new List<OSCMapMessage>();

		#endregion

		#region Public Methods

		public void Map(IOSCPacket packet)
		{
			if (packet is OSCBundle bundle)
			{
				foreach (var bundlePacket in bundle.Packets)
					Map(bundlePacket);
			}
			else if (packet is OSCMessage message)
			{
				foreach (var mapMessage in _messages)
					mapMessage.Map(message);
			}
		}

		#endregion
	}
}