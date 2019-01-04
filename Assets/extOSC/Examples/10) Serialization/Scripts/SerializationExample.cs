/* Copyright (c) 2019 ExT (V.Sigalkin) */

#if !NETFX_CORE
using UnityEngine;
using UnityEngine.UI;

using extOSC.UI;
using extOSC.Serialization;

namespace extOSC.Examples
{
    public class ExampleClass
    {
        [OSCSerialize]
        public string StringValue;

        [OSCSerialize]
        public float FloatValue;

        [OSCSerialize]
        public bool BoolValue;

        [OSCSerialize]
        public Vector2 VectorValue;
    }

    public class SerializationExample : MonoBehaviour
    {
        #region Public Vars

        [Header("OSC Settings")]
        public OSCReceiver Receiver;

        public OSCTransmitter Transmitter;

        [Header("Transmitter UI Settings")]
        public Text TransmitterAddress;

        public InputField TransmitterClassString;

        public Slider TransmitterClassFloat;

        public Text TransmitterClassFloatText;

        public Toggle TransmitterClassBool;

        public Text TransmitterClassBoolText;

        public OSCPad TransmitterClassVector2;

        public Text TransmitterClassVector2Text;

        [Header("Receiver UI Settings")]
        public InputField ReceiverClassString;

        public Slider ReceiverClassFloat;

        public Text ReceiverClassFloatText;

        public Toggle ReceiverClassBool;

        public Text ReceiverClassBoolText;

        public OSCPad ReceiverClassVector2;

        public Text ReceiverClassVector2Text;

        #endregion

        #region Private Vars

        private const string _address = "/example/10/class";

        #endregion

        #region Unity Methods

        public void Start()
        {
            TransmitterAddress.text = _address;

            ReceiverClassVector2Text.text = Vector3.zero.ToString();

            Receiver.Bind(_address, ReceiveClass);
        }

        #endregion

        #region Public Methods

        public void ChangeFloat(float value)
        {
            TransmitterClassFloatText.text = value.ToString();
        }

        public void ChangeBool(bool value)
        {
            TransmitterClassBoolText.text = value.ToString();
        }

        public void ChangeVector2(Vector2 value)
        {
            TransmitterClassVector2Text.text = value.ToString();
        }

        public void SendClass()
        {
            var exampleClass = new ExampleClass();
            exampleClass.StringValue = TransmitterClassString.text;
            exampleClass.FloatValue = TransmitterClassFloat.value;
            exampleClass.BoolValue = TransmitterClassBool.isOn;
            exampleClass.VectorValue = TransmitterClassVector2.Value;

            var message = OSCSerializer.Serialize(_address, exampleClass);

            Transmitter.Send(message);
        }

        public void ReceiveClass(OSCMessage message)
        {
            var exampleClass = OSCSerializer.Deserialize<ExampleClass>(message);
            if (exampleClass != null)
            {
                ReceiverClassString.text = exampleClass.StringValue;
                ReceiverClassFloat.value = exampleClass.FloatValue;
                ReceiverClassBool.isOn = exampleClass.BoolValue;
                ReceiverClassVector2.Value = exampleClass.VectorValue;

                ReceiverClassFloatText.text = exampleClass.FloatValue.ToString();
                ReceiverClassBoolText.text = exampleClass.BoolValue.ToString();
                ReceiverClassVector2Text.text = exampleClass.VectorValue.ToString();
            }
        }

        #endregion
    }
}
#endif