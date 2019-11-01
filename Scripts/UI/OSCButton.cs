/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using extOSC.Core.Events;

namespace extOSC.UI
{
	[AddComponentMenu("extOSC/UI/Button")]
	[RequireComponent(typeof(RectTransform))]
	public class OSCButton : Selectable, ICanvasElement, ISubmitHandler
	{
		#region Extensions

		public enum ButtonTransition
		{
			None,

			Fade
		}

		public enum Type
		{
			Push,

			Toggle
		}

		#endregion

		#region Public Vars

		public virtual bool Value
		{
			get => _value;
			set => Set(value);
		}

		public Graphic Graphic
		{
			get => _graphic;
			set => _graphic = value;
		}

		public ButtonTransition GraphicTransition
		{
			get => _graphicTransition;
			set => _graphicTransition = value;
		}

		public Type ButtonType
		{
			get => _buttonType;
			set
			{
				if (_buttonType == value)
					return;

				_buttonType = value;

				Set(false, false);
			}
		}

		#endregion

		#region Private Vars

		[SerializeField]
		protected bool _value;

		[SerializeField]
		private OSCEventBool _onValueChanged = new OSCEventBool();

		[SerializeField]
		private Graphic _graphic;

		[SerializeField]
		private ButtonTransition _graphicTransition;

		[SerializeField]
		private Type _buttonType;

		private bool _isPressed;

		#endregion

		#region Unity Methods

		protected override void OnEnable()
		{
			base.OnEnable();

			if (_buttonType == Type.Push)
				Set(false, false);

			UpdateMarker(true);
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (_isPressed)
				return;

			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			base.OnPointerDown(eventData);

			_isPressed = true;

			if (_buttonType == Type.Push)
			{
				Set(true);
			}
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			if (!_isPressed)
				return;

			base.OnPointerUp(eventData);

			_isPressed = false;

			if (_buttonType == Type.Push)
			{
				Set(false);
			}
			else if (_buttonType == Type.Toggle)
			{
				Set(!_value);
			}
		}

		protected override void OnDidApplyAnimationProperties()
		{
			if (_graphic != null)
			{
				bool oldValue = !Mathf.Approximately(_graphic.canvasRenderer.GetColor().a, 0);
				if (_value != oldValue)
				{
					_value = oldValue;
					Set(!oldValue);
				}
			}

			base.OnDidApplyAnimationProperties();
		}

		public void OnSubmit(BaseEventData eventData)
		{
			if (_buttonType == Type.Toggle)
			{
				Set(!_value);
			}
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();

			Set(_value, false);

			if (_buttonType == Type.Push)
				Set(false, false);

			UpdateMarker(_graphicTransition == ButtonTransition.None);

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

		protected virtual void Set(bool input)
		{
			Set(input, true);
		}

		protected virtual void Set(bool input, bool sendCallback)
		{
			_value = input;

			UpdateMarker(_graphicTransition == ButtonTransition.None);

			if (sendCallback)
				_onValueChanged.Invoke(_value);
		}

		#endregion

		#region Private Methods

		private void UpdateMarker(bool force)
		{
			if (_graphic == null)
				return;

#if UNITY_EDITOR
			if (!Application.isPlaying)
				_graphic.canvasRenderer.SetAlpha(_value ? 1f : 0f);
			else
#endif
				_graphic.CrossFadeAlpha(_value ? 1f : 0f, force ? 0f : 0.1f, true);
		}

		#endregion
	}
}