/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEngine.Events;

using extOSC.Core;
using extOSC.Core.Events;

namespace extOSC
{
	public class OSCBindBundle : IOSCBindBundle
	{
		#region Public Vars

		public OSCEventBundle Callback
		{
			get => _callback;
			set => _callback = value;
		}

		#endregion

		#region Protected Vars

		[SerializeField]
		private OSCEventBundle _callback = new OSCEventBundle();

		#endregion

		#region Public Methods

		public OSCBindBundle(UnityAction<OSCBundle> callback)
		{
			_callback.AddListener(callback);
		}

		#endregion
	}
}