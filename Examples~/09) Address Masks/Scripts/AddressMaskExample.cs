/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

namespace extOSC.Examples
{
	public class AddressMaskExample : MonoBehaviour
	{
		#region Public Vars

		public OSCTransmitter Transmitter;

		public OSCReceiver Receiver;

		[Header("Transmitter UI Settings")]
		public Text TransmitterAddressFirst;

		public Text TransmitterAddressSecond;

		public Text TransmitterTextFirst;

		public Text TransmitterTextSecond;

		[Header("Receiver UI Settings")]
		public Text ReceiverAddressMask;

		public Text ReceiverText;

		public Text ReceiverAddress;

		public Slider ReceiverSlider;

		#endregion

		#region Private Vars

		// Address with mask!
		private const string _maskAddress = "/example/9/*";

		private const string _firstSlider = "/example/9/first";

		private const string _secondSlider = "/example/9/second";

		#endregion

		#region Unity Methods

		protected virtual void Start()
		{
			ReceiverAddressMask.text = _maskAddress;
			TransmitterAddressFirst.text = _firstSlider;
			TransmitterAddressSecond.text = _secondSlider;

			Receiver.Bind(_maskAddress, MessageReceived);
		}

		#endregion

		#region Public Methods

		public void SendFirst(float value)
		{
			var message = new OSCMessage(_firstSlider, OSCValue.Float(value));

			Transmitter.Send(message);
			TransmitterTextFirst.text = value.ToString();
		}

		public void SendSecond(float value)
		{
			var message = new OSCMessage(_secondSlider, OSCValue.Float(value));

			Transmitter.Send(message);
			TransmitterTextSecond.text = value.ToString();
		}

		#endregion

		#region Protected Methods

		protected void MessageReceived(OSCMessage message)
		{
			if (message.ToFloat(out var value))
			{
				ReceiverAddress.text = message.Address;
				ReceiverText.text = value.ToString();
				ReceiverSlider.value = value;
			}
		}

		#endregion
	}
}