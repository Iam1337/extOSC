/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

using extOSC.UI;

namespace extOSC.Examples
{
	public class InformersExample : MonoBehaviour
	{
		#region Public Vars

		public OSCReceiver Receiver;

		public RectTransform InformerItem;

		[Header("UI Settings")]
		public Text ReceiverTextRotate;

		public Slider ReceiverSliderRotate;

		public Text ReceiverTextScale;

		public Slider ReceiverSliderScale;

		public OSCPad ReceiverPadPosition;

		public Text ReceiverTextPosition;

		[Header("Noise Settings")]
		public float PositionSpeed = 0.5f;

		public float PositionDepth = 200;

		public float RotationSpeed = 0.6f;

		public float RotationDepth = 360;

		public float ScaleSpeed = 0.2f;

		public float ScaleDepth = 5;

		#endregion

		#region Private Vars

		private const string _rotateAddress = "/example/4/rotate";

		private const string _scaleAddress = "/example/4/scale";

		private const string _positionAddress = "/example/4/position";

		private Vector3 _noisePosition = Vector3.zero;

		private Vector3 _noiseRotation = Vector3.zero;

		private Vector3 _noiseScale = Vector3.one;

		#endregion

		#region Unity Methods

		protected virtual void Start()
		{
			var scale = Random.Range(1f, 5f);

			_noiseScale = new Vector3(scale, scale, scale);
			_noisePosition = new Vector3(Random.Range(0f, 100f), Random.Range(0f, 100f), 0);
			_noiseRotation = new Vector3(0, 0, Random.Range(0f, 360f));

			ReceiverTextScale.text = Vector3.one.ToString();
			ReceiverTextRotate.text = Vector3.zero.ToString();

			Receiver.Bind(_positionAddress, ReceivePosition);
			Receiver.Bind(_rotateAddress, ReceiveRotate);
			Receiver.Bind(_scaleAddress, ReceiveScale);
		}

		protected virtual void Update()
		{
			// POSITION
			_noisePosition.x += PositionSpeed * Time.deltaTime;
			_noisePosition.y += PositionSpeed * Time.deltaTime;

			var position = InformerItem.anchoredPosition3D;

			position.x = Mathf.PerlinNoise(_noisePosition.x, _noisePosition.x) * PositionDepth;
			position.y = Mathf.PerlinNoise(_noisePosition.y, _noisePosition.y) * PositionDepth;

			position.x = OSCUtilities.Map(position.x, 0, PositionDepth, -100, 100);
			position.y = OSCUtilities.Map(position.y, 0, PositionDepth, -100, 100);

			InformerItem.anchoredPosition3D = position;

			//ROTATION
			_noiseRotation.z += RotationSpeed * Time.deltaTime;

			var rotation = InformerItem.localEulerAngles;

			rotation.z = Mathf.PerlinNoise(_noiseRotation.z, _noiseRotation.z) * RotationDepth;

			rotation.z = OSCUtilities.Map(rotation.z, 0, RotationDepth, 0, 360);

			InformerItem.localEulerAngles = rotation;

			//SCALE
			_noiseScale.x += ScaleSpeed * Time.deltaTime;

			var scale = InformerItem.localScale;

			scale.x = Mathf.PerlinNoise(_noiseScale.x, _noiseScale.x) * ScaleDepth;

			scale.x = scale.y = scale.z = OSCUtilities.Map(scale.x, 0, ScaleDepth, 1, 5);

			InformerItem.localScale = scale;
		}

		#endregion

		#region Public Methods

		public void ReceiveRotate(OSCMessage message)
		{
			if (message.ToVector3(out var vector))
			{
				ReceiverTextRotate.text = vector.ToString();
				ReceiverSliderRotate.value = OSCUtilities.Map(vector.z, 0, 360, 0, 1);
			}
		}

		public void ReceiveScale(OSCMessage message)
		{
			if (message.ToVector3(out var vector))
			{
				ReceiverTextScale.text = vector.ToString();
				ReceiverSliderScale.value = OSCUtilities.Map(vector.x, 1, 5, 0, 1);
			}
		}

		public void ReceivePosition(OSCMessage message)
		{
			if (message.ToVector3(out var vector))
			{
				vector = new Vector2(OSCUtilities.Map(vector.x, -100, 100, -1, 1), OSCUtilities.Map(vector.y, -100, 100, -1, 1));

				ReceiverTextPosition.text = ((Vector2) vector).ToString();

				ReceiverPadPosition.Value = vector;

			}
		}

		#endregion
	}
}