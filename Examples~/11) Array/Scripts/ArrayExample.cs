/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;

using System.Collections.Generic;

namespace extOSC.Examples
{
	public class ArrayExample : MonoBehaviour
	{
		#region Public Vars

		[Header("OSC Settings")]
		public OSCReceiver Receiver;

		public OSCTransmitter Transmitter;

		#endregion

		#region Private Vars

		private const string _address = "/example/12/";

		#endregion

		#region Unity Methods

		protected virtual void Start()
		{
			// Register receive callback.
			Receiver.Bind(_address, MessageReceived);

			// Create message
			var message = OSCMessage.Create(_address);

			// Create array
			var array = OSCValue.Array();
			array.AddValue(OSCValue.Int(1)); // You can use AddValue(OSCValue) method only with OSCValue what stored Array type.
			array.AddValue(OSCValue.Float(2.5f));
			array.AddValue(OSCValue.Color(Color.red));

			// You can store another array inside array.
			// Warning! OSCValue with "Array" type cannot store itself. It can do infinite loop.
			var secondArray = OSCValue.Array();
			secondArray.AddValue(OSCValue.String("This array..."));
			secondArray.AddValue(OSCValue.String("...inside another array!"));
			array.AddValue(secondArray);

			// Add array in message
			message.AddValue(array);

			// Send message
			Transmitter.Send(message);
		}

		#endregion

		#region Protected Methods

		protected void MessageReceived(OSCMessage message)
		{
			if (message.ToArray(out var arrayValues)) // Get all values from first array in message.
			{
				Debug.Log("Array values:");

				foreach (var value in arrayValues)
					Debug.LogFormat("\t {0}", value);
			}
		}

		#endregion
	}
}