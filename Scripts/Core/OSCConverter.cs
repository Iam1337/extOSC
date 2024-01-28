/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using System;
using System.Collections.Generic;

using extOSC.Core.Packers;

namespace extOSC.Core
{
	public static class OSCConverter
	{
		#region Static Public Vars

		private static int _packetSize = 65507;

		private static int _bufferIndex = 0;

		private static readonly byte[] _valuesBuffer = new byte[_packetSize]; // Max UDP size. But better use available default MTU size - 1432.

		private static readonly object _lock = new object();

		private static readonly List<byte[]> _buffers = new List<byte[]>()
		{
			new byte[_packetSize],
			new byte[_packetSize],
			new byte[_packetSize],
			new byte[_packetSize],
			new byte[_packetSize],
			new byte[_packetSize],
			new byte[_packetSize],
			new byte[_packetSize],
		};

		private static readonly List<byte[]> _packetsBuffers = new List<byte[]>()
		{
			new byte[_packetSize],
			new byte[_packetSize],
			new byte[_packetSize],
			new byte[_packetSize],
			new byte[_packetSize],
			new byte[_packetSize],
			new byte[_packetSize],
			new byte[_packetSize],
		};

		private static readonly Dictionary<OSCValueType, OSCPacker> _packersDictionary = new Dictionary<OSCValueType, OSCPacker>()
		{
			{OSCValueType.Int, new OSCPackerInt()},
			{OSCValueType.Null, new OSCPackerNull()},
			{OSCValueType.Blob, new OSCPackerBlob()},
			{OSCValueType.Long, new OSCPackerLong()},
			{OSCValueType.True, new OSCPackerTrue()},
			{OSCValueType.Char, new OSCPackerChar()},
			{OSCValueType.Midi, new OSCPackerMidi()},
			{OSCValueType.Color, new OSCPackerColor()},
			{OSCValueType.False, new OSCPackerFalse()},
			{OSCValueType.Float, new OSCPackerFloat()},
			{OSCValueType.Double, new OSCPackerDouble()},
			{OSCValueType.String, new OSCPackerString()},
			{OSCValueType.Impulse, new OSCPackerImpulse()},
			{OSCValueType.TimeTag, new OSCPackerTimeTag()}
		};

		#endregion

		#region Static Public Methods

		public static void SetBuffersDepth(int depth)
		{
			if (_buffers.Count < depth)
			{
				var difference = depth - _buffers.Count;

				_buffers.RemoveRange(depth - 1, difference);
				_packetsBuffers.RemoveRange(depth - 1, difference);

				return;
			}

			while (_buffers.Count < depth)
			{
				_buffers.Add(new byte[_packetSize]);
				_packetsBuffers.Add(new byte[_packetSize]);
			}
		}

		/// <summary>
		/// Serializes a IOSCPacket and writes it to a buffer, returning the size of the packet.
		/// </summary>
		/// <param name="packet">packet</param>
		/// <param name="size">Serialized packet size</param>
		/// <returns>Buffer with a fixed value.</returns>
		public static int Pack(IOSCPacket packet, out byte[] buffer)
		{
			lock (_lock)
			{
				var size = 0;
				buffer = PackInternal(packet, ref size);
				return size;
			}
		}

		/// <summary>
		/// Serializes a packet and returns the packet data in byte array.
		/// </summary>
		/// <param name="packet">packet</param>
		/// <returns>packet data</returns>
		public static byte[] Pack(IOSCPacket packet)
		{
			lock (_lock)
			{
				var size = 0;
				var buffer = PackInternal(packet, ref size);

				var bytes = new byte[size];

				Buffer.BlockCopy(buffer, 0, bytes, 0, size);

				return bytes;
			}
		}

		public static IOSCPacket Unpack(byte[] bytes)
		{
			lock (_lock)
			{
				return Unpack(bytes, bytes.Length);
			}
		}

		public static IOSCPacket Unpack(byte[] buffer, int length)
		{
			lock (_lock)
			{
				var start = 0;
				return UnpackInternal(buffer, ref start, length);
			}
		}

		#endregion

		#region Static Private Methods

		private static bool IsBundle(byte[] bytes, ref int start)
		{
			return bytes[start] == '#';
		}

		//  PACK METHODS
		private static byte[] PackInternal(IOSCPacket packet, ref int index)
		{
			if (_bufferIndex >= _buffers.Count)
			{
				_bufferIndex = 0;

				throw new Exception($"[OSCConverter.PackInternal] You have reached the nesting package depth limit. Maximum depth of nested packages: {_buffers.Count}\nTo change the depth use: OSCConverter.SetBuffersDepth() method.");
			}

			var buffer = _buffers[_bufferIndex];
			_bufferIndex++;

			if (packet.IsBundle())
			{
				PackBundle((OSCBundle) packet, buffer, ref index);
			}
			else
			{
				PackMessage((OSCMessage) packet, buffer, ref index);
			}

			_bufferIndex--;

			return buffer;
		}

		private static void PackBundle(OSCBundle bundle, byte[] buffer, ref int index)
		{
			var packets = bundle.Packets;
			var packetsIndex = 0;
			var packetsCount = packets.Count;
			var packetsBuffer = _packetsBuffers[_bufferIndex - 1];

			for (var i = 0; i < packetsCount; ++i)
			{
				var packetSize = 0;
				var packetBuffer = PackInternal(packets[i], ref packetSize);

				PackValue(packetsBuffer, ref packetsIndex, OSCValueType.Int, packetSize);

				Buffer.BlockCopy(packetBuffer, 0, packetsBuffer, packetsIndex, packetSize);
				packetsIndex += packetSize;
			}

			PackValue(buffer, ref index, OSCValueType.String, bundle.Address);
			PackValue(buffer, ref index, OSCValueType.Long, bundle.TimeStamp);

			Buffer.BlockCopy(packetsBuffer, 0, buffer, index, packetsIndex);
			index += packetsIndex;
		}

		private static void PackMessage(OSCMessage message, byte[] buffer, ref int index)
		{
			var typeTags = ",";

			var values = message.Values;
			var valuesIndex = 0;
			var valuesCount = values.Count;

			for (var i = 0; i < valuesCount; ++i)
			{
				ProcessValue(ref typeTags, _valuesBuffer, ref valuesIndex, values[i]);
			}

			PackValue(buffer, ref index, OSCValueType.String, message.Address);
			PackValue(buffer, ref index, OSCValueType.String, typeTags);

			Buffer.BlockCopy(_valuesBuffer, 0, buffer, index, valuesIndex);
			index += valuesIndex;
		}

		private static void ProcessValue(ref string typeTags, byte[] buffer, ref int index, OSCValue value)
		{
			if (value.Type == OSCValueType.Array)
			{
				typeTags += '[';

				var array = value.ArrayValue;
				var arraySize = array.Count;

				for (var i = 0; i < arraySize; ++i)
				{
					ProcessValue(ref typeTags, buffer, ref index, array[i]);
				}

				typeTags += ']';

				return;
			}

			typeTags += value.Tag;
			PackValue(buffer, ref index, value.Type, value.Value);
		}

		//  UNPACK METHODS.
		private static IOSCPacket UnpackInternal(byte[] bytes, ref int start, int end)
		{
			if (IsBundle(bytes, ref start))
				return UnpackBundle(bytes, ref start, end);

			return UnpackMessage(bytes, ref start);
		}

		private static OSCBundle UnpackBundle(byte[] bytes, ref int start, int end)
		{
			OSCBundle bundle = null;

			var address = (string) UnpackValue(OSCValueType.String, bytes, ref start);
			if (address.Equals(OSCBundle.BundleAddress))
			{
				var timeStamp = (long) UnpackValue(OSCValueType.Long, bytes, ref start);

				bundle = new OSCBundle();
				bundle.TimeStamp = timeStamp;

				while (start < end)
				{
					var packetLength = (int) UnpackValue(OSCValueType.Int, bytes, ref start);
					var packet = UnpackInternal(bytes, ref start, start + packetLength);

					bundle.AddPacket(packet);
				}
			}

			return bundle;
		}

		private static OSCMessage UnpackMessage(byte[] bytes, ref int start)
		{
			OSCMessage message = null;

			var address = (string) UnpackValue(OSCValueType.String, bytes, ref start);
			var typeTags = (string) UnpackValue(OSCValueType.String, bytes, ref start);
			var valuesArray = (Dictionary<int, OSCValue>) null;

			message = new OSCMessage(address);

			foreach (var valueTag in typeTags)
			{
				if (valueTag == ',') continue;

				OSCValue value = null;

				// START ARRAY
				if (valueTag == '[')
				{
					if (valuesArray == null)
						valuesArray = new Dictionary<int, OSCValue>();

					valuesArray.Add(valuesArray.Count, OSCValue.Array());

					continue;
				}

				// STOP ARRAY
				if (valueTag == ']')
				{
					if (valuesArray != null && valuesArray.Count > 0)
					{
						value = valuesArray[valuesArray.Count - 1];
						valuesArray.Remove(valuesArray.Count - 1);
					}
				}
				else
				{
					// DEFAULT VALUE
					var valueType = OSCValue.GetValueType(valueTag);
					var @object = UnpackValue(valueType, bytes, ref start);

					value = new OSCValue(valueType, @object); // TODO: Make pool.
				}

				if (valuesArray != null && valuesArray.Count > 0)
				{
					valuesArray[valuesArray.Count - 1].ArrayValue.Add(value);
					continue;
				}

				message.AddValue(value);
			}

			return message;
		}

		//  PACK VALUE
		private static void PackValue(byte[] buffer, ref int index, OSCValueType valueType, object value)
		{
			if (!_packersDictionary.ContainsKey(valueType))
				throw new Exception("[OSCConverter.PackValue] Unknown value type: '" + valueType + "'.");

			_packersDictionary[valueType].Pack(buffer, ref index, value);
		}

		//  UNPACK VALUE
		private static object UnpackValue(OSCValueType valueType, byte[] bytes, ref int start)
		{
			if (!_packersDictionary.ContainsKey(valueType))
				throw new Exception("[OSCConverter.UnpackValue] Unknown value type: '" + valueType + "'.");

			return _packersDictionary[valueType].Unpack(bytes, ref start);
		}

		#endregion
	}
}