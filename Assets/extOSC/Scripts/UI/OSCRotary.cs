/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System;
using System.Collections;

using extOSC.Core.Events;

namespace extOSC.UI
{
	[AddComponentMenu("extOSC/UI/Rotary")]
	[RequireComponent(typeof(RectTransform))]
	public class OSCRotary : Selectable, IInitializePotentialDragHandler, IDragHandler, ICanvasElement
	{
		#region Public Vars

		public Image HandleImage
		{
			get => _handleImage;
			set
			{
				if (_handleImage == value)
					return;

				_handleImage = value;

				UpdateCachedReferences();
				UpdateVisuals();
			}
		}

		public virtual float Value
		{
			get => WholeNumbers ? Mathf.Round(_value) : _value;
			set => Set(value);
		}

		public float MinValue
		{
			get => _minValue;
			set
			{
				if (_minValue.Equals(value))
					return;

				_minValue = value;

				Set(_value);
				UpdateVisuals();
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

				Set(_value);
				UpdateVisuals();
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

				Set(_value);
				UpdateVisuals();
			}
		}

		public OSCEventFloat OnValueChanged
		{
			get => _onValueChanged;
			set => _onValueChanged = value;
		}

		public float NormalizedValue
		{
			get => Mathf.Approximately(MinValue, MaxValue) ? 0f : Mathf.InverseLerp(MinValue, MaxValue, Value);
			set => Value = Mathf.Lerp(MinValue, MaxValue, value);
		}

		public bool Inverse
		{
			get => _reverse;
			set
			{
				if (_reverse == value)
					return;

				_reverse = value;

				UpdateVisuals();
			}
		}

		public bool ResetValue
		{
			get => _resetValue;
			set => _resetValue = value;
		}

		public float ResetValueTime
		{
			get => _resetValueTime;
			set
			{
				if (Math.Abs(_resetValueTime - value) < float.Epsilon)
					return;

				_resetValueTime = value;

				if (_resetValueTime < 0)
					_resetValueTime = 0;
			}
		}

		public bool CallbackOnReset
		{
			get => _callbackOnReset;
			set => _callbackOnReset = value;
		}

		public float DefaultValue
		{
			get => _defaultValue;
			set
			{
				if (_defaultValue.Equals(value))
					return;

				_defaultValue = ClampValue(value);

				if (_resetValue && !_dragged)
				{
					if (_resetAnimationCoroutine != null)
						StopCoroutine(_resetAnimationCoroutine);

					_resetAnimationCoroutine = StartCoroutine(ResetAnimationCoroutine());
				}
			}
		}

		#endregion

		#region Private Vars

		[SerializeField]
		private float _value;

		[SerializeField]
		private float _maxValue = 1;

		[SerializeField]
		private float _minValue;

		[SerializeField]
		private Image _handleImage;

		[SerializeField]
		private OSCEventFloat _onValueChanged;

		[SerializeField]
		private bool _wholeNumbers;

		[SerializeField]
		private bool _resetValue;

		[SerializeField]
		private float _resetValueTime;

		[SerializeField]
		private bool _callbackOnReset;

		[SerializeField]
		private bool _reverse;

		private float _defaultValue;

		private bool _dragged;

		private Coroutine _resetAnimationCoroutine;

		private RectTransform _handleContainerRectTransform;

		private float _previousDegrees;

		private float _stepSize => _wholeNumbers ? 1 : (_maxValue - _minValue) * 0.1f;

		#endregion

		#region Unity Methods

		protected override void Awake()
		{
			_defaultValue = Value;

			base.Awake();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			UpdateCachedReferences();

			Set(_value, false);

			UpdateVisuals();
		}

		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			eventData.useDragThreshold = true;
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!MayDrag(eventData))
				return;

			UpdateDrag(eventData, eventData.pressEventCamera);
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!MayDrag(eventData))
				return;

			if (_resetAnimationCoroutine != null)
				StopCoroutine(_resetAnimationCoroutine);

			_dragged = true;

			base.OnPointerDown(eventData);

			UpdateDrag(eventData, eventData.pressEventCamera, true);
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);

			if (!_dragged) return;
			_dragged = false;

			if (_resetValue)
			{
				if (_resetAnimationCoroutine != null)
					StopCoroutine(_resetAnimationCoroutine);

				if (Math.Abs(_resetValueTime) < float.Epsilon)
					Set(_defaultValue, _callbackOnReset);
				else
					_resetAnimationCoroutine = StartCoroutine(ResetAnimationCoroutine());
			}
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();

			if (!IsActive())
				return;

			UpdateVisuals();
		}

		protected override void OnDidApplyAnimationProperties()
		{
			_value = ClampValue(_value);
			var normal = NormalizedValue;

			if (_handleImage != null)
			{
				normal = OSCUtilities.Map(_handleImage.fillAmount, 0.1f, 0.9f, 0, 1);
			}

			UpdateVisuals();

			if (Math.Abs(normal - _value) < float.Epsilon)
				return;

			_onValueChanged.Invoke(_value);
		}

		public override void OnMove(AxisEventData eventData)
		{
			if (!IsActive() || !IsInteractable())
			{
				base.OnMove(eventData);
				return;
			}

			switch (eventData.moveDir)
			{
				case MoveDirection.Left:
				case MoveDirection.Right:
					base.OnMove(eventData);
					break;
				case MoveDirection.Up:
					if (_value >= _maxValue)
						base.OnMove(eventData);
					else
						Set(Value + _stepSize);
					TryResetValue();
					break;
				case MoveDirection.Down:
					if (_value <= _minValue)
						base.OnMove(eventData);
					else
						Set(Value - _stepSize);
					TryResetValue();
					break;
			}
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();

			if (WholeNumbers)
			{
				_minValue = Mathf.Round(_minValue);
				_maxValue = Mathf.Round(_maxValue);
			}

			if (IsActive())
			{
				UpdateCachedReferences();
				Set(_value, false);
				UpdateVisuals();
			}

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

		public void Rebuild(CanvasUpdate executing)
		{
#if UNITY_EDITOR
			if (executing == CanvasUpdate.Prelayout && _onValueChanged != null)
				_onValueChanged.Invoke(_value);
#endif
		}

		public void GraphicUpdateComplete()
		{ }

		public void LayoutComplete()
		{ }

		#endregion

		#region Private Methods

		private void TryResetValue()
		{
			if (_resetValue)
			{
				if (_resetAnimationCoroutine != null)
					StopCoroutine(_resetAnimationCoroutine);

				if (Math.Abs(_resetValueTime) < float.Epsilon)
					Set(_defaultValue, _callbackOnReset);
				else
					_resetAnimationCoroutine = StartCoroutine(ResetAnimationCoroutine());
			}
		}

		private void UpdateCachedReferences()
		{
			if (_handleImage != null)
			{
				_handleContainerRectTransform = _handleImage.transform.parent.GetComponent<RectTransform>();
			}
		}

		private void Set(float input, bool sendCallback = true)
		{
			var clampValue = ClampValue(input);

			if (Mathf.Abs(clampValue - _value) < float.Epsilon)
				return;

			_value = clampValue;

			UpdateVisuals();

			if (sendCallback)
				_onValueChanged.Invoke(_value);
		}

		private bool MayDrag(PointerEventData eventData)
		{
			if (IsActive() && IsInteractable())
				return eventData.button == PointerEventData.InputButton.Left;

			return false;
		}

		private void UpdateDrag(PointerEventData eventData, Camera camera, bool force = false)
		{
			if (_handleContainerRectTransform == null ||
				_handleContainerRectTransform.rect.size.x <= 0.0 ||
				_handleContainerRectTransform.rect.size.y <= 0.0 ||
				!RectTransformUtility.ScreenPointToLocalPointInRectangle(_handleContainerRectTransform, eventData.position, camera, out var eventPosition))
				return;

			eventPosition -= _handleContainerRectTransform.rect.position;

			var position = Vector2.zero;
			position.x = eventPosition.x / _handleContainerRectTransform.rect.size.x;
			position.y = eventPosition.y / _handleContainerRectTransform.rect.size.y;

			var degrees = Mathf.Atan2(position.x - 0.5f, position.y - 0.5f) / Mathf.PI;

			var difference = Mathf.Abs(degrees - _previousDegrees);
			if (difference > 1.25f && !force) return;

			_previousDegrees = degrees;

			NormalizedValue = _reverse ? 1f - OSCUtilities.Map(degrees, -0.8f, 0.8f, 0f, 1f) : OSCUtilities.Map(degrees, -0.8f, 0.8f, 0f, 1f);
		}

		private void UpdateVisuals()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				UpdateCachedReferences();
#endif

			if (_handleImage != null)
			{
				var fillAmount = OSCUtilities.Map(NormalizedValue, 0, 1, 0.1f, 0.9f);

				if (_reverse != !_handleImage.fillClockwise)
				{
					_handleImage.fillClockwise = !_reverse;
				}

				_handleImage.fillAmount = fillAmount;
			}
		}

		private float ClampValue(float input)
		{
			input = Mathf.Clamp(input, MinValue, MaxValue);
			return WholeNumbers ? Mathf.Round(input) : input;
		}

		private IEnumerator ResetAnimationCoroutine()
		{
			var timer = 0f;
			var currentValue = _value;

			while (timer < _resetValueTime)
			{
				timer += Time.deltaTime;

				currentValue = Mathf.Lerp(currentValue, _defaultValue, timer / _resetValueTime);

				Set(currentValue, _callbackOnReset);

				yield return null;
			}

			_resetAnimationCoroutine = null;
		}

		#endregion
	}
}