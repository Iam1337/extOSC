/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace extOSC.Examples
{
	public class ValueTypesExample : MonoBehaviour
	{
		#region Static Public Methods

		public static int RemapValue(float value, float inputMin, float inputMax, int outputMin, int outputMax)
		{
			return (int) ((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin);
		}

		#endregion

		#region Public Vars

		[Header("OSC Settings")]
		public OSCReceiver Receiver;

		public OSCTransmitter Transmitter;

		[Header("Transmitter UI Settings")]
		public Text TransmitterAddressBlob;

		public Text TransmitterAddressChar;

		public Text TransmitterAddressColor;

		public Text TransmitterAddressDouble;

		public Text TransmitterAddressBool;

		public Text TransmitterAddressFloat;

		public Text TransmitterAddressImpulse;

		public Text TransmitterAddressInt;

		public Text TransmitterAddressLong;

		public Text TransmitterAddressNull;

		public Text TransmitterAddressString;

		public Text TransmitterAddressTimeTag;

		public Text TransmitterAddressMidi;

		public Text TransmitterTextBlob;

		public Text TransmitterTextDouble;

		public Text TransmitterTextBool;

		public Text TransmitterTextFloat;

		public Text TransmitterTextInt;

		public Text TransmitterTextLong;

		public InputField TransmitterInputTimeTagMonth;

		public InputField TransmitterInputTimeTagDay;

		public InputField TransmitterInputTimeTagYear;

		public InputField TransmitterInputTimeTagHour;

		public InputField TransmitterInputTimeTagMinute;

		public InputField TransmitterInputTimeTagSecond;

		public InputField TransmitterInputTimeTagMillisecond;

		public InputField TransmitterInputMidiChannel;

		public InputField TransmitterInputMidiStatus;

		public InputField TransmitterInputMidiData1;

		public InputField TransmitterInputMidiData2;

		public Image TransmitterImageColor;

		[Header("Receiver UI Settings")]
		public Text ReceiverTextBlob;

		public InputField ReceiverInputChar;

		public Text ReceiverTextDouble;

		public Slider ReceiverSliderDouble;

		public Text ReceiverTextBool;

		public Toggle ReceiverToggleBool;

		public Text ReceiverTextFloat;

		public Slider ReceiverSliderFloat;

		public Image ReceiverImageImpulse;

		public Text ReceiverTextInt;

		public Slider ReceiverSliderInt;

		public Text ReceiverTextLong;

		public Slider ReceiverSliderLong;

		public Image ReceiverImageNull;

		public InputField ReceiverInputString;

		public InputField ReceiverInputTimeTagMonth;

		public InputField ReceiverInputTimeTagDay;

		public InputField ReceiverInputTimeTagYear;

		public InputField ReceiverInputTimeTagHour;

		public InputField ReceiverInputTimeTagMinute;

		public InputField ReceiverInputTimeTagSecond;

		public InputField ReceiverInputTimeTagMillisecond;

		public InputField ReceiverInputMidiChannel;

		public InputField ReceiverInputMidiStatus;

		public InputField ReceiverInputMidiData1;

		public InputField ReceiverInputMidiData2;

		public Image ReceiverImageColor;

		public Color BlinkImage;

		#endregion

		#region Private Vars

		private const string _blobAddress = "/example/2/blob";

		private const string _charAddress = "/example/2/char";

		private const string _colorAddress = "/example/2/color";

		private const string _doubleAddress = "/example/2/double";

		private const string _boolAddress = "/example/2/bool";

		private const string _floatAddress = "/example/2/float";

		private const string _impulseAddress = "/example/2/impulse";

		private const string _intAddress = "/example/2/int";

		private const string _longAddress = "/example/2/long";

		private const string _nullAddress = "/example/2/null";

		private const string _stringAddress = "/example/2/string";

		private const string _timetagAddress = "/example/2/timetag";

		private const string _midiAddress = "/example/2/midi";

		private DateTime _timeTag;

		private OSCMidi _midi;

		private Color _defaultImageColor;

		private Dictionary<Image, Coroutine> _blinkCoroutinePool = new Dictionary<Image, Coroutine>();

		#endregion

		#region Unity Methods

		public void Start()
		{
			TransmitterAddressBlob.text = string.Format("<color=grey>{0}</color>", _boolAddress);
			TransmitterAddressChar.text = string.Format("<color=grey>{0}</color>", _charAddress);
			TransmitterAddressColor.text = string.Format("<color=grey>{0}</color>", _colorAddress);
			TransmitterAddressDouble.text = string.Format("<color=grey>{0}</color>", _doubleAddress);
			TransmitterAddressBool.text = string.Format("<color=grey>{0}</color>", _boolAddress);
			TransmitterAddressFloat.text = string.Format("<color=grey>{0}</color>", _floatAddress);
			TransmitterAddressImpulse.text = string.Format("<color=grey>{0}</color>", _impulseAddress);
			TransmitterAddressInt.text = string.Format("<color=grey>{0}</color>", _intAddress);
			TransmitterAddressLong.text = string.Format("<color=grey>{0}</color>", _longAddress);
			TransmitterAddressNull.text = string.Format("<color=grey>{0}</color>", _nullAddress);
			TransmitterAddressString.text = string.Format("<color=grey>{0}</color>", _stringAddress);
			TransmitterAddressTimeTag.text = string.Format("<color=grey>{0}</color>", _timetagAddress);

			_timeTag = DateTime.Now;

			TransmitterInputTimeTagMonth.text = _timeTag.Month.ToString();
			TransmitterInputTimeTagDay.text = _timeTag.Day.ToString();
			TransmitterInputTimeTagYear.text = _timeTag.Year.ToString();
			TransmitterInputTimeTagHour.text = _timeTag.Hour.ToString();
			TransmitterInputTimeTagMinute.text = _timeTag.Minute.ToString();
			TransmitterInputTimeTagSecond.text = _timeTag.Second.ToString();
			TransmitterInputTimeTagMillisecond.text = _timeTag.Millisecond.ToString();

			_midi = new OSCMidi(0, 0, 0, 0);

			TransmitterInputMidiChannel.text = _midi.Channel.ToString();
			TransmitterInputMidiStatus.text = _midi.Status.ToString();
			TransmitterInputMidiData1.text = _midi.Data1.ToString();
			TransmitterInputMidiData2.text = _midi.Data2.ToString();

			Receiver.Bind(_blobAddress, ReceiveBlob);
			Receiver.Bind(_charAddress, ReceiveChar);
			Receiver.Bind(_colorAddress, ReceiveColor);
			Receiver.Bind(_doubleAddress, ReceiveDouble);
			Receiver.Bind(_boolAddress, ReceiveBool);
			Receiver.Bind(_floatAddress, ReceiveFloat);
			Receiver.Bind(_impulseAddress, ReceiveImpulse);
			Receiver.Bind(_intAddress, ReceiveInt);
			Receiver.Bind(_longAddress, ReceiveLong);
			Receiver.Bind(_nullAddress, ReceiveNull);
			Receiver.Bind(_stringAddress, ReceiveString);
			Receiver.Bind(_timetagAddress, ReceiveTimeTag);
			Receiver.Bind(_midiAddress, ReceiveMidi);

			_defaultImageColor = ReceiverImageImpulse.color;
		}

		#endregion

		#region Public Methods

		// TRANSMITTER
		public void SendBlob()
		{
			var bytes = new byte[30];
			var bytesString = string.Empty;

			for (var index = 0; index < bytes.Length; index++)
			{
				bytes[index] = (byte) (Random.value * 255);
				bytesString += "0x" + string.Format("{0:x2}", bytes[index]).ToUpper() + " ";
			}

			Send(_blobAddress, OSCValue.Blob(bytes));

			TransmitterTextBlob.text = bytesString.Remove(bytesString.Length - 1);
		}

		public void SendChar(string value)
		{
			if (value.Length == 0) return;

			Send(_charAddress, OSCValue.Char(value[0]));
		}

		public void SendColor()
		{
			var color = new Color(Random.value, Random.value, Random.value);

			Send(_colorAddress, OSCValue.Color(color));

			TransmitterImageColor.color = color;
		}

		public void SendDouble(float value)
		{
			var doubleValue = (double) value;

			if (doubleValue > double.Epsilon)
				doubleValue = Math.Min(doubleValue + double.Epsilon, 1);

			Send(_doubleAddress, OSCValue.Double(doubleValue));

			TransmitterTextDouble.text = doubleValue.ToString();
		}

		public void SendBool(bool value)
		{
			Send(_boolAddress, OSCValue.Bool(value));

			TransmitterTextBool.text = value.ToString();
		}

		public void SendFloat(float value)
		{
			Send(_floatAddress, OSCValue.Float(value));

			TransmitterTextFloat.text = value.ToString();
		}

		public void SendImpulse()
		{
			Send(_impulseAddress, OSCValue.Impulse());
		}

		public void SendInt(float value)
		{
			var integerValue = RemapValue(value, 0, 1, 0, int.MaxValue);

			Send(_intAddress, OSCValue.Int(integerValue));

			TransmitterTextInt.text = integerValue.ToString();
		}

		public void SendLong(float value)
		{
			var longValue = (long) (long.MaxValue * (double) value);

			if (longValue < 0)
				longValue = long.MaxValue;

			Send(_longAddress, OSCValue.Long(longValue));

			TransmitterTextLong.text = longValue.ToString();
		}

		public void SendNull()
		{
			Send(_nullAddress, OSCValue.Null());
		}

		public void SendString(string value)
		{
			Send(_stringAddress, OSCValue.String(value));
		}

		public void SendTimeTag()
		{
			Send(_timetagAddress, OSCValue.TimeTag(_timeTag));
		}

		public void ChangeTimeTagMonth(string value)
		{
			if (value.Length == 0)
				value = "0";

			var monthValue = Mathf.Clamp(Convert.ToInt32(value), 1, 12);

			_timeTag = new DateTime(_timeTag.Year,
									monthValue,
									_timeTag.Day,
									_timeTag.Hour,
									_timeTag.Minute,
									_timeTag.Second,
									_timeTag.Millisecond);

			SendTimeTag();

			TransmitterInputTimeTagMonth.text = monthValue.ToString();
		}

		public void ChangeTimeTagDay(string value)
		{
			if (value.Length == 0)
				value = "0";

			var dayValue = Mathf.Clamp(Convert.ToInt32(value), 1, DateTime.DaysInMonth(_timeTag.Year, _timeTag.Month));

			_timeTag = new DateTime(_timeTag.Year,
									_timeTag.Month,
									dayValue,
									_timeTag.Hour,
									_timeTag.Minute,
									_timeTag.Second,
									_timeTag.Millisecond);

			SendTimeTag();

			TransmitterInputTimeTagDay.text = dayValue.ToString();
		}

		public void ChangeTimeTagYear(string value)
		{
			if (value.Length == 0)
				value = "0";

			var yearValue = Mathf.Clamp(Convert.ToInt32(value), 1, 9999);

			_timeTag = new DateTime(yearValue,
									_timeTag.Month,
									_timeTag.Day,
									_timeTag.Hour,
									_timeTag.Minute,
									_timeTag.Second,
									_timeTag.Millisecond);

			SendTimeTag();

			TransmitterInputTimeTagYear.text = yearValue.ToString();
		}

		public void ChangeTimeTagHour(string value)
		{
			if (value.Length == 0)
				value = "0";

			var hourValue = Mathf.Clamp(Convert.ToInt32(value), 0, 23);

			_timeTag = new DateTime(_timeTag.Year,
									_timeTag.Month,
									_timeTag.Day,
									hourValue,
									_timeTag.Minute,
									_timeTag.Second,
									_timeTag.Millisecond);

			SendTimeTag();

			TransmitterInputTimeTagHour.text = hourValue.ToString();
		}

		public void ChangeTimeTagMinute(string value)
		{
			if (value.Length == 0)
				value = "0";

			var minuteValue = Mathf.Clamp(Convert.ToInt32(value), 0, 59);

			_timeTag = new DateTime(_timeTag.Year,
									_timeTag.Month,
									_timeTag.Day,
									_timeTag.Hour,
									minuteValue,
									_timeTag.Second,
									_timeTag.Millisecond);

			SendTimeTag();

			TransmitterInputTimeTagMinute.text = minuteValue.ToString();
		}

		public void ChangeTimeTagSecond(string value)
		{
			if (value.Length == 0)
				value = "0";

			var secondValue = Mathf.Clamp(Convert.ToInt32(value), 0, 59);

			_timeTag = new DateTime(_timeTag.Year,
									_timeTag.Month,
									_timeTag.Day,
									_timeTag.Hour,
									_timeTag.Minute,
									secondValue,
									_timeTag.Millisecond);

			SendTimeTag();

			TransmitterInputTimeTagSecond.text = secondValue.ToString();
		}

		public void ChangeTimeTagMillisecond(string value)
		{
			if (value.Length == 0)
				value = "0";

			var millisecondValue = Mathf.Clamp(Convert.ToInt32(value), 0, 999);

			_timeTag = new DateTime(_timeTag.Year,
									_timeTag.Month,
									_timeTag.Day,
									_timeTag.Hour,
									_timeTag.Minute,
									_timeTag.Second,
									millisecondValue);

			SendTimeTag();

			TransmitterInputTimeTagMillisecond.text = millisecondValue.ToString();
		}

		public void SendMidi()
		{
			Send(_midiAddress, OSCValue.Midi(_midi));
		}

		public void ChangeMidiChannel(string value)
		{
			if (value.Length == 0)
				value = "0";

			var channel = (byte) Mathf.Clamp(Convert.ToInt32(value), 0, 255);

			_midi.Channel = channel;

			SendMidi();

			TransmitterInputMidiChannel.text = channel.ToString();
		}

		public void ChangeMidiStatus(string value)
		{
			if (value.Length == 0)
				value = "0";

			var status = (byte) Mathf.Clamp(Convert.ToInt32(value), 0, 255);

			_midi.Status = status;

			SendMidi();

			TransmitterInputMidiStatus.text = status.ToString();
		}

		public void ChangeMidiData1(string value)
		{
			if (value.Length == 0)
				value = "0";

			var data1 = (byte) Mathf.Clamp(Convert.ToInt32(value), 0, 255);

			_midi.Data1 = data1;

			SendMidi();

			TransmitterInputMidiData1.text = data1.ToString();
		}

		public void ChangeMidiData2(string value)
		{
			if (value.Length == 0)
				value = "0";

			var data2 = (byte) Mathf.Clamp(Convert.ToInt32(value), 0, 255);

			_midi.Data2 = data2;

			SendMidi();

			TransmitterInputMidiData2.text = data2.ToString();
		}

		// RECEIVER
		public void ReceiveBlob(OSCMessage message)
		{
			if (message.ToBlob(out var value))
			{
				var bytesString = string.Empty;

				for (var index = 0; index < value.Length; index++)
				{
					bytesString += $"0x{value[index]:x2}".ToUpper() + " ";
				}

				ReceiverTextBlob.text = bytesString.Remove(bytesString.Length - 1);
			}
		}

		public void ReceiveChar(OSCMessage message)
		{
			if (message.ToChar(out var value))
			{
				ReceiverInputChar.text = value.ToString();
				;
				ReceiverInputChar.Select();
			}
		}

		public void ReceiveColor(OSCMessage message)
		{
			if (message.ToColor(out var value))
			{
				ReceiverImageColor.color = value;
			}
		}

		public void ReceiveDouble(OSCMessage message)
		{
			if (message.ToDouble(out var value))
			{
				ReceiverTextDouble.text = value.ToString();
				ReceiverSliderDouble.value = (float) value;
			}
		}

		public void ReceiveBool(OSCMessage message)
		{
			if (message.ToBool(out var value))
			{
				ReceiverTextBool.text = value.ToString();
				ReceiverToggleBool.isOn = value;
			}
		}

		public void ReceiveFloat(OSCMessage message)
		{
			if (message.ToFloat(out var value))
			{
				ReceiverTextFloat.text = value.ToString();
				ReceiverSliderFloat.value = value;
			}
		}

		public void ReceiveImpulse(OSCMessage message)
		{
			if (message.HasImpulse())
			{
				ImageBlink(ReceiverImageImpulse);
			}
		}

		public void ReceiveInt(OSCMessage message)
		{
			int value;
			if (message.ToInt(out value))
			{
				ReceiverTextInt.text = value.ToString();

				var floatValue = (float) (value / (double) int.MaxValue);
				ReceiverSliderInt.value = floatValue;
			}
		}

		public void ReceiveLong(OSCMessage message)
		{
			if (message.ToLong(out var value))
			{
				ReceiverTextLong.text = value.ToString();

				var floatValue = (float) (value / (double) long.MaxValue);
				ReceiverSliderLong.value = floatValue;
			}
		}

		public void ReceiveNull(OSCMessage message)
		{
			if (message.HasNull())
			{
				ImageBlink(ReceiverImageNull);
			}
		}

		public void ReceiveString(OSCMessage message)
		{
			if (message.ToString(out var value))
			{
				ReceiverInputString.text = value;
			}
		}

		public void ReceiveTimeTag(OSCMessage message)
		{
			if (message.ToTimeTag(out var value))
			{
				ReceiverInputTimeTagMonth.text = value.Month.ToString();
				ReceiverInputTimeTagDay.text = value.Day.ToString();
				ReceiverInputTimeTagYear.text = value.Year.ToString();
				ReceiverInputTimeTagHour.text = value.Hour.ToString();
				ReceiverInputTimeTagMinute.text = value.Minute.ToString();
				ReceiverInputTimeTagSecond.text = value.Second.ToString();
				ReceiverInputTimeTagMillisecond.text = value.Millisecond.ToString();
			}
		}

		public void ReceiveMidi(OSCMessage message)
		{
			if (message.ToMidi(out var value))
			{
				ReceiverInputMidiChannel.text = value.Channel.ToString();
				ReceiverInputMidiStatus.text = value.Status.ToString();
				ReceiverInputMidiData1.text = value.Data1.ToString();
				ReceiverInputMidiData2.text = value.Data2.ToString();
			}
		}

		#endregion

		#region Private Methods

		private void Send(string address, OSCValue value)
		{
			var message = new OSCMessage(address, value);

			Transmitter.Send(message);
		}

		private void ImageBlink(Image image)
		{
			if (_blinkCoroutinePool.ContainsKey(image))
			{
				StopCoroutine(_blinkCoroutinePool[image]);

				_blinkCoroutinePool.Remove(image);
			}

			_blinkCoroutinePool.Add(image, StartCoroutine(ImageBlinkCoroutine(image)));
		}

		private IEnumerator ImageBlinkCoroutine(Image image)
		{
			var blinkTimer = 0f;
			var blinkDuration = 0.05f;

			while (blinkTimer < blinkDuration)
			{
				blinkTimer += Time.deltaTime;

				image.color = Color.Lerp(image.color, BlinkImage, blinkTimer / blinkDuration);

				yield return null;
			}

			blinkTimer = 0;
			blinkDuration = 0.2f;

			while (blinkTimer < blinkDuration)
			{
				blinkTimer += Time.deltaTime;

				image.color = Color.Lerp(BlinkImage, _defaultImageColor, blinkTimer / blinkDuration);

				yield return null;
			}

			if (_blinkCoroutinePool.ContainsKey(image))
				_blinkCoroutinePool.Remove(image);
		}

		#endregion
	}
}