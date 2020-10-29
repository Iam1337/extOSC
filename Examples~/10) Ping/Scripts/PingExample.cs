/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

namespace extOSC.Examples
{
	public class PingExample : MonoBehaviour
	{
		#region Public Vars

		public Text IntervalText;

		#endregion

		#region Public Methods

		public void ChangeInterval(float value)
		{
			IntervalText.text = value.ToString();
		}

		#endregion
	}
}