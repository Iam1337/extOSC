/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Examples
{
	public class ScriptingExample : MonoBehaviour
	{
		#region Private Vars

		private OSCTransmitter _transmitter;

		private OSCReceiver _receiver;

		private const string _oscAddress = "/example/7/"; // Also, you cam use mask in address: /example/*/

		#endregion

		#region Unity Methods

		protected virtual void Start()
		{
			// Creating a transmitter.
			_transmitter = gameObject.AddComponent<OSCTransmitter>();

			// Set remote host address.
			_transmitter.RemoteHost = "127.0.0.1";

			// Set remote port;
			_transmitter.RemotePort = 7001;


			// Creating a receiver.
			_receiver = gameObject.AddComponent<OSCReceiver>();

			// Set local port.
			_receiver.LocalPort = 7001;

			// Bind "MessageReceived" method to special address.
			_receiver.Bind(_oscAddress, MessageReceived);
		}

		protected virtual void Update()
		{
			if (_transmitter == null) return;

			// Create message
			var message = new OSCMessage(_oscAddress);
			message.AddValue(OSCValue.String("Hello, world!"));
			message.AddValue(OSCValue.Float(Random.Range(0f, 1f)));

			// Send message
			_transmitter.Send(message);
		}

		#endregion

		#region Protected Methods

		protected void MessageReceived(OSCMessage message)
		{
			Debug.Log(message);
		}

		#endregion
	}
}