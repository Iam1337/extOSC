/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;

using System.Runtime.InteropServices;

namespace extOSC.Examples
{
	// Marshalling works only with structures
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct MarshallingStructure
	{
		public int IntValue;

		[MarshalAs(UnmanagedType.LPStr, SizeConst = 20)]
		public string StringValue;

		public float FloatValue;
	}

	public class MarshallingExample : MonoBehaviour
	{
		#region Public Vars

		[Header("OSC Settings")]
		public OSCReceiver Receiver;

		public OSCTransmitter Transmitter;

		#endregion

		#region Private Vars

		private const string _address = "/example/14/";

		#endregion

		#region Unity Methods

		public void Start()
		{
			Receiver.Bind(_address, ReceiveMessage);

			// Create Message
			var message = new OSCMessage(_address);

			// Create structure
			var structure = new MarshallingStructure();
			structure.IntValue = 1337;
			structure.StringValue = "Hello, OSC World!";
			structure.FloatValue = 13.37f;

			// Convert structure to bytes by marshalling!
			// Marshalling can sometimes be quicker, than any other form of conversion of data in OSC
			var bytes = OSCUtilities.StructToByte(structure);

			// Add bytes to message
			message.AddValue(OSCValue.Blob(bytes));

			// Send message
			Transmitter.Send(message);
		}

		#endregion

		#region Public Methods

		public void ReceiveMessage(OSCMessage message)
		{
			byte[] bytes;

			// Get bytes from message
			if (!message.ToBlob(out bytes))
				return;

			// Convert bytes to structure!
			var structure = OSCUtilities.ByteToStruct<MarshallingStructure>(bytes);

			Debug.LogFormat("Received structure with data:\nIntValue: {0}\nStringValue: {1}\nFloatValue: {2}",
							structure.IntValue, structure.StringValue, structure.FloatValue);
		}

		#endregion
	}
}