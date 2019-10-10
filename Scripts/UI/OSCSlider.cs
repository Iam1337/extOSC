/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System;
using System.Collections;

namespace extOSC.UI
{
	[AddComponentMenu("extOSC/UI/Slider")]
	[RequireComponent(typeof(RectTransform))]
	public class OSCSlider : Slider
	{
		#region Public Vars

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

		public OSCMultiplySliders MultiplyController
		{
			get => _multiplyController;
			set
			{
				if (_multiplyController == value)
					return;

				_multiplyController = value;

				SetMultiplyController(value);
			}
		}

		#endregion

		#region Private Vars

		[SerializeField]
		private bool _resetValue;

		[SerializeField]
		private float _resetValueTime;

		[SerializeField]
		private bool _callbackOnReset;

		[SerializeField]
		private OSCMultiplySliders _multiplyController;

		private float _defaultValue;

		private bool _dragged;

		private Coroutine _resetAnimationCoroutine;

		#endregion

		#region Unity Methods

		protected override void Awake()
		{
			base.Awake();

			_defaultValue = m_Value;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			SetMultiplyController(_multiplyController);
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!MayDrag(eventData))
				return;

			if (_resetAnimationCoroutine != null)
				StopCoroutine(_resetAnimationCoroutine);

			base.OnPointerDown(eventData);

			_dragged = true;
		}

		public override void OnDrag(PointerEventData eventData)
		{
			if (_multiplyController != null && eventData.pointerDrag != _multiplyController.gameObject)
			{
				eventData.selectedObject = _multiplyController.gameObject;
				eventData.pointerDrag = _multiplyController.gameObject;

				return;
			}

			base.OnDrag(eventData);
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

			var tempValue = value;

			base.OnMove(eventData);

			if (Math.Abs(tempValue - value) < float.Epsilon) return;

			if (direction == Direction.RightToLeft || direction == Direction.LeftToRight)
			{
				if (eventData.moveDir == MoveDirection.Left || eventData.moveDir == MoveDirection.Right)
				{
					TryResetValue();
				}
			}
			else if (direction == Direction.TopToBottom || direction == Direction.BottomToTop)
			{
				if (eventData.moveDir == MoveDirection.Up || eventData.moveDir == MoveDirection.Down)
				{
					TryResetValue();
				}
			}
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();

			MultiplyController = _multiplyController;
		}
#endif

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

		private void SetMultiplyController(OSCMultiplySliders controller)
		{
			if (_multiplyController == null)
				return;

			if (_multiplyController.LayoutDirection == OSCMultiplySliders.Direction.Vertical)
			{
				if (direction == Direction.BottomToTop || direction == Direction.TopToBottom)
				{
					SetDirection(direction == Direction.BottomToTop ? Direction.LeftToRight : Direction.RightToLeft, true);
				}
			}
			else
			{
				if (direction == Direction.LeftToRight || direction == Direction.RightToLeft)
				{
					SetDirection(direction == Direction.LeftToRight ? Direction.BottomToTop : Direction.TopToBottom, true);
				}
			}
		}

		private IEnumerator ResetAnimationCoroutine()
		{
			var timer = 0f;
			var currentValue = value;

			while (timer < _resetValueTime)
			{
				timer += Time.deltaTime;

				currentValue = Mathf.Lerp(currentValue, _defaultValue, timer / _resetValueTime);

				Set(currentValue, _callbackOnReset);

				yield return null;
			}

			_resetAnimationCoroutine = null;
		}

		private bool MayDrag(PointerEventData eventData)
		{
			if (IsActive() && IsInteractable())
				return eventData.button == PointerEventData.InputButton.Left;

			return false;
		}

		private float ClampValue(float input)
		{
			var newValue = Mathf.Clamp(input, minValue, maxValue);

			if (wholeNumbers)
				newValue = Mathf.Round(newValue);

			return newValue;
		}

		#endregion
	}
}