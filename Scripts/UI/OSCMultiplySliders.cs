/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections.Generic;

using extOSC.Core.Reflection;
using extOSC.Components.Informers;

namespace extOSC.UI
{
	[AddComponentMenu("extOSC/UI/Multiply Sliders")]
	[RequireComponent(typeof(RectTransform))]
	public class OSCMultiplySliders : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
	{
		#region Extensions

		public enum Direction
		{
			Horizontal,

			Vertical
		}

		#endregion

		#region Public Vars

		public Color DefaultColor
		{
			get => _defaultColor;
			set => _defaultColor = value;
		}

		public string Address
		{
			get => _address;
			set
			{
				if (_address == value)
					return;

				_address = value;

				UpdateSliders();
			}
		}

		public OSCTransmitter Transmitter
		{
			get => _transmitter;
			set
			{
				if (_transmitter == value)
					return;

				_transmitter = value;

				UpdateSliders();
			}
		}

		public float MinValue
		{
			get => _minValue;
			set
			{
				if (_minValue.Equals(value))
					return;

				_minValue = value;

				UpdateSliders();
			}
		}

		public float MaxValue
		{
			get => _maxValue;
			set
			{
				if (_maxValue.Equals(value))
					return;

				_maxValue = value;

				UpdateSliders();
			}
		}

		public bool WholeNumbers
		{
			get => _wholeNumbers;
			set
			{
				if (_wholeNumbers.Equals(value))
					return;

				_wholeNumbers = value;

				UpdateSliders();
			}
		}

		public Direction LayoutDirection => _layoutGroup is VerticalLayoutGroup ? Direction.Vertical : Direction.Horizontal;

		public HorizontalOrVerticalLayoutGroup LayoutGroup
		{
			get => _layoutGroup;
			set
			{
				if (_layoutGroup == value)
					return;

				_layoutGroup = value;
				_layoutTransform = _layoutGroup.GetComponent<RectTransform>();
			}
		}

		public OSCSlider[] Sliders => _sliders.ToArray();

		#endregion

		#region Private Vars

		[SerializeField]
		private string _address = "/address";

		[OSCSelector]
		[SerializeField]
		private OSCTransmitter _transmitter;

		[SerializeField]
		private HorizontalOrVerticalLayoutGroup _layoutGroup;

		[SerializeField]
		private List<OSCSlider> _sliders = new List<OSCSlider>();

		[SerializeField]
		private Color _defaultColor;

		[SerializeField]
		private float _maxValue = 1;

		[SerializeField]
		private float _minValue;

		[SerializeField]
		private bool _wholeNumbers;

		private OSCSlider _currentSlider;

		private RectTransform _layoutTransform;

		#endregion

		#region Unity Methods

		protected override void Awake()
		{
			if (_layoutGroup != null)
				_layoutTransform = _layoutGroup.GetComponent<RectTransform>();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

#if UNITY_EDITOR
			if (Application.isPlaying)
#endif

				UpdateSliders();
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!MayDrag(eventData))
				return;

			if (_layoutTransform == null ||
				_layoutTransform.rect.size.x <= 0.0 ||
				_layoutTransform.rect.size.y <= 0.0 ||
				!RectTransformUtility.ScreenPointToLocalPointInRectangle(_layoutTransform, eventData.position, eventData.pressEventCamera, out var localPoint))
				return;

			for (var index = 0; index < _sliders.Count; index++)
			{
				var slider = _sliders[index];
				if (slider == null) continue;

				var sliderRectTransform = slider.transform as RectTransform;
				var sliderPosition = (Vector2) sliderRectTransform.localPosition - sliderRectTransform.sizeDelta * 0.5f;
				var sliderRect = new Rect(sliderPosition, sliderRectTransform.rect.size);

				if (Contain(sliderRect, localPoint, index))
				{
					_currentSlider = slider;
					break;
				}
			}

			if (_currentSlider != null) _currentSlider.OnDrag(eventData);
		}

		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			eventData.useDragThreshold = true;
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();

			if (Application.isPlaying && IsActive())
				UpdateSliders();

			_layoutTransform = _layoutGroup.GetComponent<RectTransform>();

#if UNITY_2018_3_OR_NEWER
			var assetType = UnityEditor.PrefabUtility.GetPrefabAssetType(this);
			if (assetType == UnityEditor.PrefabAssetType.NotAPrefab && !Application.isPlaying)
				CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
#else
            var prefabType = UnityEditor.PrefabUtility.GetPrefabType(this);
            if (prefabType != UnityEditor.PrefabType.Prefab && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
#endif
		}
#endif

		#endregion

		#region Public Methods

		public void GraphicUpdateComplete()
		{ }

		public void LayoutComplete()
		{ }

		public void Rebuild(CanvasUpdate executing)
		{ }

		public void AddSlider(OSCSlider slider)
		{
			if (!_sliders.Contains(slider))
				_sliders.Add(slider);
		}

		public void RemoveSlider(OSCSlider slider)
		{
			if (_sliders.Contains(slider))
				_sliders.Remove(slider);
		}

		#endregion

		#region Private Methods

		private void UpdateSliders()
		{
			for (var index = 0; index < _sliders.Count; index++)
			{
				var slider = _sliders[index];
				if (slider == null) continue;

				slider.MultiplyController = this;
				slider.wholeNumbers = _wholeNumbers;
				slider.maxValue = _maxValue;
				slider.minValue = _minValue;

				var informerFloat = slider.gameObject.GetComponent<OSCTransmitterInformerFloat>();
				if (informerFloat == null)
				{
					informerFloat = slider.gameObject.AddComponent<OSCTransmitterInformerFloat>();
					informerFloat.ReflectionTarget = new OSCReflectionMember()
					{
						Target = slider,
						MemberName = "value"
					};
				}

				informerFloat.Transmitter = _transmitter;
				informerFloat.TransmitterAddress = $"{_address}/{index}";
			}
		}

		private bool Contain(Rect rect, Vector2 point, int index)
		{
			if (LayoutDirection == Direction.Horizontal)
			{
				if (index == 0)
					return rect.x + rect.width > point.x;
				if (index >= _sliders.Count - 1)
					return rect.x < point.x;

				return rect.x < point.x && rect.x + rect.width > point.x;
			}

			if (index == 0)
				return rect.y < point.y;
			if (index >= _sliders.Count - 1)
				return rect.y + rect.height > point.y;

			return rect.y < point.y && rect.y + rect.height > point.y;
		}

		private bool MayDrag(PointerEventData eventData)
		{
			if (IsActive() && IsInteractable())
				return eventData.button == PointerEventData.InputButton.Left;

			return false;
		}

		#endregion
	}
}