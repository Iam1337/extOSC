/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

using System;

namespace extOSC.Examples
{
	public class MappingExample : MonoBehaviour
	{
		#region Public Vars

		public OSCTransmitter Transmitter;

		[Header("UI Settings")]
		public Text TextRotate;

		public Text TextScale;

		public Text TextPositionX;

		public Text TextPositionY;

		#endregion

		#region Private Vars

		private const string _rotateAddress = "/example/6/rotate";

		private const string _scaleAddress = "/example/6/scale";

		private const string _positionAddress = "/example/6/position";

		private Vector3 _position = Vector3.zero;

		#endregion

		#region Unity Methods

		protected virtual void Start()
		{
			TextScale.text = $"<color=grey>{Vector3.one}</color>";
			TextRotate.text = $"<color=grey>{Vector3.zero}</color>";
		}

		#endregion

		#region Public Methods

		public void SendRotate(float value)
		{
			var vector = new Vector3(0, 0, value);

			SendVector(_rotateAddress, vector);

			TextRotate.text = vector.ToString();
		}

		public void SendScale(float value)
		{
			var vector = new Vector3(value, value, value);

			SendVector(_scaleAddress, vector);

			TextScale.text = vector.ToString();
		}

		public void SendPositionX(float value)
		{
			_position.x = value;

			SendVector(_positionAddress, _position);

			TextPositionX.text = _position.x.ToString();
		}

		public void SendPositionY(float value)
		{
			_position.y = value;

			SendVector(_positionAddress, _position);

			TextPositionY.text = _position.y.ToString();
		}

		#endregion

		#region Private Methods

		private void SendVector(string address, Vector3 vector)
		{
			var message = new OSCMessage(address);

			// Sending vector value
			message.AddValue(OSCValue.Float(vector.x));
			message.AddValue(OSCValue.Float(vector.y));
			message.AddValue(OSCValue.Float(vector.z));

			if (Transmitter != null)
				Transmitter.Send(message);
		}

		#endregion
	}
}