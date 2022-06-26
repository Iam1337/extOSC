/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System;
using System.Collections;

using extOSC.Core.Events;

namespace extOSC.UI
{
	[AddComponentMenu("extOSC/UI/Pad")]
	[RequireComponent(typeof(RectTransform))]
	public class OSCPad : Selectable, IInitializePotentialDragHandler, IDragHandler, ICanvasElement
	{
		#region Extensions

		public enum PointAlignment
		{
			BottomLeft,

			BottomRight,

			TopLeft,

			TopRight
		}

		#endregion

		#region Static Private Methods

		private static Vector2 RoundValue(Vector2 input)
		{
			input.x = Mathf.Round(input.x);
			input.y = Mathf.Round(input.y);

			return input;
		}

		private static Vector2 ConvertVector(Vector2 input, PointAlignment alignment)
		{
			switch (alignment)
			{
				case PointAlignment.BottomRight:
					input.x = 1f - input.x;
					break;
				case PointAlignment.TopLeft:
					input.y = 1f - input.y;
					break;
				case PointAlignment.TopRight:
					input.y = 1f - input.y;
					input.x = 1f - input.x;
					break;
				case PointAlignment.BottomLeft: 
					break;
				default:                        
					throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null);
			}

			return input;
		}

		#endregion

		#region Public Vars

		public RectTransform HandleRect
		{
			get => _handleRect;
            set
			{
				if (_handleRect == value)
					return;

				_handleRect = value;

				UpdateCachedReferences();
				UpdateVisuals();
			}
		}

		public RectTransform XAxisRect
		{
			get => _xAxisRect;
            set
			{
				if (_xAxisRect == value)
					return;

				_xAxisRect = value;

				UpdateVisuals();
			}
		}

		public RectTransform YAxisRect
		{
			get => _yAxisRect;
            set
			{
				if (_yAxisRect == value)
					return;

				_yAxisRect = value;

				UpdateVisuals();
			}
		}

		public virtual Vector2 Value
		{
			get
			{
				if (WholeNumbers)
					return RoundValue(_value);

				return _value;
			}
			set => Set(value);
        }

		public Vector2 MinValue
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

		public Vector2 MaxValue
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

		public OSCEventVector2 OnValueChanged
		{
			get => _onValueChanged;
            set => _onValueChanged = value;
        }

		public PointAlignment HandleAlignment
		{
			get => _handleAlignment;
            set
			{
				if (_handleAlignment == value)
					return;

				_handleAlignment = value;

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

		public Vector2 DefaultValue
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

		public Vector2 NormalizedValue
		{
			get
			{
				var vector = Vector2.zero;
				vector.x = Mathf.InverseLerp(MinValue.x, MaxValue.x, Value.x);
				vector.y = Mathf.InverseLerp(MinValue.y, MaxValue.y, Value.y);

				return vector;
			}
			set
			{
				var vector = Vector2.zero;
				vector.x = Mathf.Lerp(MinValue.x, MaxValue.x, value.x);
				vector.y = Mathf.Lerp(MinValue.y, MaxValue.y, value.y);

				Value = vector;
			}
		}

		#endregion

		#region Private Vars

		[SerializeField]
		protected Vector2 _value;

		[SerializeField]
		private Vector2 _maxValue = Vector2.one;

		[SerializeField]
		private Vector2 _minValue;

		[SerializeField]
		private OSCEventVector2 _onValueChanged = new OSCEventVector2();

		[SerializeField]
		private RectTransform _handleRect;

		[SerializeField]
		private RectTransform _xAxisRect;

		[SerializeField]
		private RectTransform _yAxisRect;

		[SerializeField]
		private bool _wholeNumbers;

		[SerializeField]
		private bool _resetValue;

		[SerializeField]
		private float _resetValueTime;

		[SerializeField]
		private bool _callbackOnReset;

		[SerializeField]
		private PointAlignment _handleAlignment;

		private Vector2 _defaultValue;

		private Vector2 _offset = Vector2.zero;

		private Transform _handleTransform;

		private RectTransform _handleContainerRect;

		private DrivenRectTransformTracker _tracker;

		private bool _dragged;

		private Coroutine _resetAnimationCoroutine;

		private Vector2 _stepSize
		{
			get
			{
				if (WholeNumbers) return Vector2.one;

				var step = new Vector2();
				step.x = (MaxValue.x - MinValue.x) * 0.1f;
				step.y = (MaxValue.y - MinValue.y) * 0.1f;

				return step;
			}
		}

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

		protected override void OnDisable()
		{
			_tracker.Clear();

			base.OnDisable();
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!MayDrag(eventData)) return;

			UpdateDrag(eventData, eventData.pressEventCamera);
		}

		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			eventData.useDragThreshold = true;
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

			if (_handleContainerRect != null)
			{
				normal = _handleRect.anchorMin;
			}

			UpdateVisuals();

			if (Math.Abs(normal.x - _value.x) < float.Epsilon &&
				Math.Abs(normal.y - _value.y) < float.Epsilon)
				return;

			_onValueChanged.Invoke(_value);
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!MayDrag(eventData))
				return;

			base.OnPointerDown(eventData);

			if (_resetAnimationCoroutine != null)
				StopCoroutine(_resetAnimationCoroutine);

			_dragged = true;
			_offset = Vector2.zero;

			if (_handleContainerRect != null &&
				RectTransformUtility.RectangleContainsScreenPoint(_handleRect, eventData.position, eventData.enterEventCamera))
			{
				Vector2 localPoint;

				if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_handleRect, eventData.position, eventData.pressEventCamera, out localPoint))
					return;

				_offset = localPoint;
			}
			else
				UpdateDrag(eventData, eventData.pressEventCamera);
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
					if ((_handleAlignment == PointAlignment.BottomLeft || _handleAlignment == PointAlignment.TopLeft))
					{
						if (_value.x <= _minValue.x)
							base.OnMove(eventData);
						else
						{
							Set(Value - new Vector2(_stepSize.x, 0));
							TryResetValue();
						}
					}
					else
					{
						if (_value.x >= _maxValue.x)
							base.OnMove(eventData);
						else
						{
							Set(Value + new Vector2(_stepSize.x, 0));
							TryResetValue();
						}
					}

					break;
				case MoveDirection.Right:
					if ((_handleAlignment == PointAlignment.BottomLeft || _handleAlignment == PointAlignment.TopLeft))
					{
						if (_value.x >= _maxValue.x)
							base.OnMove(eventData);
						else
						{
							Set(Value + new Vector2(_stepSize.x, 0));
							TryResetValue();
						}
					}
					else
					{
						if (_value.x <= _minValue.x)
							base.OnMove(eventData);
						else
						{
							Set(Value - new Vector2(_stepSize.x, 0));
							TryResetValue();
						}
					}

					break;
				case MoveDirection.Up:
					if ((_handleAlignment == PointAlignment.BottomLeft || _handleAlignment == PointAlignment.BottomRight))
					{
						if (_value.y >= _maxValue.y)
							base.OnMove(eventData);
						else
						{
							Set(Value + new Vector2(0, _stepSize.y));
							TryResetValue();
						}
					}
					else
					{
						if (_value.y <= _minValue.y)
							base.OnMove(eventData);
						else
						{
							Set(Value - new Vector2(0, _stepSize.y));
							TryResetValue();
						}
					}

					break;

				case MoveDirection.Down:
					if ((_handleAlignment == PointAlignment.BottomLeft || _handleAlignment == PointAlignment.BottomRight))
					{
						if (_value.y <= _minValue.y)
							base.OnMove(eventData);
						else
						{
							Set(Value - new Vector2(0, _stepSize.y));
							TryResetValue();
						}
					}
					else
					{
						if (_value.y >= _maxValue.y)
							base.OnMove(eventData);
						else
						{
							Set(Value + new Vector2(0, _stepSize.y));
							TryResetValue();
						}
					}

					break;
			}
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();

			if (WholeNumbers)
			{
				_minValue = RoundValue(_minValue);
				_maxValue = RoundValue(_maxValue);
			}

			if (_resetValueTime < 0)
				_resetValueTime = 0;

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

		public void GraphicUpdateComplete()
		{ }

		public void LayoutComplete()
		{ }

		public void Rebuild(CanvasUpdate executing)
		{
#if UNITY_EDITOR
			if (executing == CanvasUpdate.Prelayout && _onValueChanged != null)
				_onValueChanged.Invoke(_value);
#endif
		}

		#endregion

		#region Protected Methods

		protected OSCPad()
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

		private void Set(Vector2 input)
		{
			Set(input, true);
		}

		private void Set(Vector2 input, bool sendCallback)
		{
			var clampValue = ClampValue(input);

			if (Math.Abs(clampValue.x - _value.x) < float.Epsilon &&
				Math.Abs(clampValue.y - _value.y) < float.Epsilon)
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

		private void UpdateDrag(PointerEventData eventData, Camera camera)
		{
			if (_handleContainerRect == null ||
				_handleContainerRect.rect.size.x <= 0.0 ||
				_handleContainerRect.rect.size.y <= 0.0 ||
				!RectTransformUtility.ScreenPointToLocalPointInRectangle(_handleContainerRect, eventData.position, camera, out var localPoint))
				return;

			localPoint -= _handleContainerRect.rect.position;

			var vector = Vector2.zero;
			vector.x = Mathf.Clamp01((localPoint - _offset).x / _handleContainerRect.rect.size.x);
			vector.y = Mathf.Clamp01((localPoint - _offset).y / _handleContainerRect.rect.size.y);

			NormalizedValue = ConvertVector(vector, _handleAlignment);
		}

		private void UpdateVisuals()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				UpdateCachedReferences();
#endif

			_tracker.Clear();

			if (_handleContainerRect == null) return;

			_tracker.Add(this, _handleRect, DrivenTransformProperties.Anchors);

			var normal = ConvertVector(NormalizedValue, _handleAlignment);

			if (_xAxisRect != null)
			{
				_xAxisRect.anchorMin = new Vector2(normal.x, _xAxisRect.anchorMin.y);
				_xAxisRect.anchorMax = new Vector2(normal.x, _xAxisRect.anchorMax.y);

				_tracker.Add(this, _xAxisRect, DrivenTransformProperties.AnchorMaxX);
				_tracker.Add(this, _xAxisRect, DrivenTransformProperties.AnchorMinX);
			}

			if (_yAxisRect != null)
			{
				_yAxisRect.anchorMin = new Vector2(_yAxisRect.anchorMin.x, normal.y);
				_yAxisRect.anchorMax = new Vector2(_yAxisRect.anchorMax.x, normal.y);

				_tracker.Add(this, _yAxisRect, DrivenTransformProperties.AnchorMaxY);
				_tracker.Add(this, _yAxisRect, DrivenTransformProperties.AnchorMinY);
			}

			_handleRect.anchorMin = _handleRect.anchorMax = normal;
		}

		private void UpdateCachedReferences()
		{
			if (_handleRect != null)
			{
				_handleTransform = _handleRect.transform;
				if (_handleTransform == null)
					return;

				_handleContainerRect = _handleTransform.parent.GetComponent<RectTransform>();
			}
			else
				_handleContainerRect = null;
		}

		private Vector2 ClampValue(Vector2 input)
		{
			input.x = Mathf.Clamp(input.x, MinValue.x, MaxValue.x);
			input.y = Mathf.Clamp(input.y, MinValue.y, MaxValue.y);

			if (WholeNumbers)
				input = RoundValue(input);

			return input;
		}

		private IEnumerator ResetAnimationCoroutine()
		{
			var timer = 0f;
			var currentValue = _value;

			while (timer < _resetValueTime)
			{
				timer += Time.deltaTime;

				currentValue.x = Mathf.Lerp(currentValue.x, _defaultValue.x, timer / _resetValueTime);
				currentValue.y = Mathf.Lerp(currentValue.y, _defaultValue.y, timer / _resetValueTime);

				Set(currentValue, _callbackOnReset);

				yield return null;
			}

			_resetAnimationCoroutine = null;
		}

		#endregion
	}
}