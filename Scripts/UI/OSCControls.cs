/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Reflection;

namespace extOSC.UI
{
	public static class OSCControls
	{
		#region Extensions

		public struct Resources
		{
			public Sprite PanelFilled;

			public Sprite PanelBorder;

			public Sprite RotaryFilled;

			public Sprite RotaryFilledMask;

			public Sprite RotaryBorder;

			public Color Color;
		}

		#endregion

		#region Static Private Vars

		private static MethodInfo _createUIElementRootMethod;

		private static MethodInfo _createUIObjectMethod;

		private static MethodInfo _setDefaultColorTransitionMethod;

		#endregion

		#region Static Public Methods

		public static GameObject CreateManager()
		{
			var root = new GameObject("OSC Manager");

			var remotePort = 7000;
			var localPort = 7000;

			var transmitters = GameObject.FindObjectsOfType<OSCTransmitter>();
			foreach (var transmitter in transmitters)
			{
				if (remotePort <= transmitter.RemotePort)
					remotePort = transmitter.RemotePort + 1;
			}

			var receivers = GameObject.FindObjectsOfType<OSCReceiver>();
			foreach (var receiver in receivers)
			{
				if (localPort <= receiver.LocalPort)
					localPort = receiver.LocalPort + 1;
			}

			var managerTransmitter = root.AddComponent<OSCTransmitter>();
			managerTransmitter.RemoteHost = "127.0.0.1";
			managerTransmitter.RemotePort = remotePort;

			var managerReceiver = root.AddComponent<OSCReceiver>();
			managerReceiver.LocalPort = localPort;

			return root;
		}

		public static GameObject CreatePad(Resources resources)
		{
			Color normalColor;
			Color backgroundColor;
			PrepareColors(resources.Color, out normalColor, out backgroundColor);

			var root = CreateUIElementRoot("Pad", new Vector2(200, 200));

			var backgroundObject = CreateUIObject("Background", root);
			var backgroundBorderObject = CreateUIObject("Background Border", backgroundObject);
			var handleAreaObject = CreateUIObject("Handle Pad Area", root);
			var xAxisObject = CreateUIObject("X Axis Image", handleAreaObject);
			var yAxisObject = CreateUIObject("Y Axis Image", handleAreaObject);
			var handleObject = CreateUIObject("Handle", handleAreaObject);

			var backgroundImage = backgroundObject.AddComponent<Image>();
			backgroundImage.sprite = resources.PanelFilled;
			backgroundImage.type = Image.Type.Sliced;
			backgroundImage.color = backgroundColor;

			var backgroundRectTransform = backgroundObject.GetComponent<RectTransform>();
			backgroundRectTransform.anchorMin = Vector2.zero;
			backgroundRectTransform.anchorMax = Vector2.one;
			backgroundRectTransform.sizeDelta = Vector2.zero;

			var backgroundBorderImage = backgroundBorderObject.AddComponent<Image>();
			backgroundBorderImage.sprite = resources.PanelBorder;
			backgroundBorderImage.type = Image.Type.Sliced;
			backgroundBorderImage.fillCenter = false;
			backgroundBorderImage.color = normalColor;

			var backgroundBorderRectTransform = backgroundBorderObject.GetComponent<RectTransform>();
			backgroundBorderRectTransform.anchorMin = Vector2.zero;
			backgroundBorderRectTransform.anchorMax = Vector2.one;
			backgroundBorderRectTransform.sizeDelta = Vector2.zero;

			var handleAreaRectTransform = handleAreaObject.GetComponent<RectTransform>();
			handleAreaRectTransform.sizeDelta = new Vector2(-60f, -60);
			handleAreaRectTransform.anchorMin = Vector2.zero;
			handleAreaRectTransform.anchorMax = Vector2.one;

			var xAxisImage = xAxisObject.AddComponent<Image>();
			xAxisImage.sprite = null;
			xAxisImage.type = Image.Type.Sliced;
			xAxisImage.color = normalColor;

			var yAxisImage = yAxisObject.AddComponent<Image>();
			yAxisImage.sprite = null;
			yAxisImage.type = Image.Type.Sliced;
			yAxisImage.color = normalColor;

			var xAxisRectTransform = xAxisObject.GetComponent<RectTransform>();
			xAxisRectTransform.anchorMin = Vector2.zero;
			xAxisRectTransform.anchorMax = new Vector2(0, 1);
			xAxisRectTransform.sizeDelta = new Vector2(2, 60);

			var yAxisRectTransform = yAxisObject.GetComponent<RectTransform>();
			yAxisRectTransform.anchorMin = Vector2.zero;
			yAxisRectTransform.anchorMax = new Vector2(1, 0);
			yAxisRectTransform.sizeDelta = new Vector2(60, 2);

			var handleImage = handleObject.AddComponent<Image>();
			handleImage.sprite = resources.PanelFilled;
			handleImage.type = Image.Type.Sliced;
			handleImage.color = normalColor;
			handleObject.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);

			var padXY = root.AddComponent<OSCPad>();
			padXY.targetGraphic = handleImage;
			padXY.HandleRect = handleObject.GetComponent<RectTransform>();
			padXY.XAxisRect = xAxisRectTransform;
			padXY.YAxisRect = yAxisRectTransform;

			SetDefaultColorTransitionValues(padXY);

			return root;
		}

		public static GameObject CreateSlider(Resources resources)
		{
			Color normalColor;
			Color backgroundColor;
			PrepareColors(resources.Color, out normalColor, out backgroundColor);

			var root = CreateUIElementRoot("Slider", new Vector2(400, 90));

			var backgroundObject = CreateUIObject("Background", root);
			var backgroundBorderObject = CreateUIObject("Background Border", backgroundObject);
			var handleAreaObject = CreateUIObject("Handle Slide Area", root);
			var handleObject = CreateUIObject("Handle", handleAreaObject);

			var backgroundImage = backgroundObject.AddComponent<Image>();
			backgroundImage.sprite = resources.PanelFilled;
			backgroundImage.type = Image.Type.Sliced;
			backgroundImage.color = backgroundColor;

			var backgroundRectTransform = backgroundObject.GetComponent<RectTransform>();
			backgroundRectTransform.anchorMin = Vector2.zero;
			backgroundRectTransform.anchorMax = Vector2.one;
			backgroundRectTransform.sizeDelta = Vector2.zero;

			var backgroundBorderImage = backgroundBorderObject.AddComponent<Image>();
			backgroundBorderImage.sprite = resources.PanelBorder;
			backgroundBorderImage.type = Image.Type.Sliced;
			backgroundBorderImage.fillCenter = false;
			backgroundBorderImage.color = normalColor;

			var backgroundBorderRectTransform = backgroundBorderObject.GetComponent<RectTransform>();
			backgroundBorderRectTransform.anchorMin = Vector2.zero;
			backgroundBorderRectTransform.anchorMax = Vector2.one;
			backgroundBorderRectTransform.sizeDelta = Vector2.zero;

			var handleAreaRectTransform = handleAreaObject.GetComponent<RectTransform>();
			handleAreaRectTransform.sizeDelta = new Vector2(-32, 0);
			handleAreaRectTransform.anchorMin = new Vector2(0, 0);
			handleAreaRectTransform.anchorMax = new Vector2(1, 1);

			var handleImage = handleObject.AddComponent<Image>();
			handleImage.sprite = resources.PanelFilled;
			handleImage.type = Image.Type.Sliced;
			handleImage.color = normalColor;

			var handleRectTransform = handleObject.GetComponent<RectTransform>();
			handleRectTransform.sizeDelta = new Vector2(32, 0);

			var slider = root.AddComponent<OSCSlider>();
			slider.handleRect = handleObject.GetComponent<RectTransform>();
			slider.targetGraphic = handleImage;
			slider.direction = Slider.Direction.LeftToRight;

			SetDefaultColorTransitionValues(slider);

			return root;
		}

		public static GameObject CreateButton(Resources resources)
		{
			Color normalColor;
			Color backgroundColor;
			PrepareColors(resources.Color, out normalColor, out backgroundColor);

			var root = CreateUIElementRoot("Button", new Vector2(90, 90));

			var backgroundObject = CreateUIObject("Background", root);
			var backgroundBorderObject = CreateUIObject("Background Border", backgroundObject);
			var checkmarkObject = CreateUIObject("Checkmark", backgroundObject);

			var backgroundImage = backgroundObject.AddComponent<Image>();
			backgroundImage.sprite = resources.PanelFilled;
			backgroundImage.type = Image.Type.Sliced;
			backgroundImage.color = backgroundColor;

			var backgroundRectTransform = backgroundObject.GetComponent<RectTransform>();
			backgroundRectTransform.anchorMin = Vector2.zero;
			backgroundRectTransform.anchorMax = Vector2.one;
			backgroundRectTransform.sizeDelta = Vector2.zero;

			var backgroundBorderImage = backgroundBorderObject.AddComponent<Image>();
			backgroundBorderImage.sprite = resources.PanelBorder;
			backgroundBorderImage.type = Image.Type.Sliced;
			backgroundBorderImage.fillCenter = false;
			backgroundBorderImage.color = normalColor;

			var backgroundBorderRectTransform = backgroundBorderObject.GetComponent<RectTransform>();
			backgroundBorderRectTransform.anchorMin = Vector2.zero;
			backgroundBorderRectTransform.anchorMax = Vector2.one;
			backgroundBorderRectTransform.sizeDelta = Vector2.zero;

			var handleAreaRectTransform = checkmarkObject.GetComponent<RectTransform>();
			handleAreaRectTransform.sizeDelta = new Vector2(-8, -8);
			handleAreaRectTransform.anchorMin = Vector2.zero;
			handleAreaRectTransform.anchorMax = Vector2.one;

			var handleImage = checkmarkObject.AddComponent<Image>();
			handleImage.sprite = resources.PanelFilled;
			handleImage.type = Image.Type.Sliced;
			handleImage.color = normalColor;

			var button = root.AddComponent<OSCButton>();
			button.Graphic = handleImage;
			button.transition = Selectable.Transition.None;
			button.targetGraphic = backgroundBorderImage;
			button.ButtonType = OSCButton.Type.Toggle;
			button.Value = true;

			SetDefaultColorTransitionValues(button);

			return root;
		}

		public static GameObject CreateRotary(Resources resources)
		{
			Color normalColor;
			Color backgroundColor;
			PrepareColors(resources.Color, out normalColor, out backgroundColor);

			var root = CreateUIElementRoot("Rotary", new Vector2(200, 200));

			var handleAreaObject = CreateUIObject("Rotary Handle Area", root);
			var handleObject = CreateUIObject("Rotary Handle", handleAreaObject);
			var borderObject = CreateUIObject("Rotarty Border", root);

			var handleAreaImage = handleAreaObject.AddComponent<Image>();
			handleAreaImage.sprite = resources.RotaryFilledMask;
			handleAreaImage.type = Image.Type.Simple;
			handleAreaImage.color = backgroundColor;

			var handleAreaMask = handleAreaObject.AddComponent<Mask>();
			handleAreaMask.showMaskGraphic = true;

			var handleAreaRectTransform = handleAreaObject.GetComponent<RectTransform>();
			handleAreaRectTransform.anchorMin = Vector2.zero;
			handleAreaRectTransform.anchorMax = Vector2.one;
			handleAreaRectTransform.sizeDelta = Vector2.zero;

			var handleImage = handleObject.AddComponent<Image>();
			handleImage.sprite = resources.RotaryFilled;
			handleImage.type = Image.Type.Filled;
			handleImage.fillAmount = 0f;
			handleImage.fillCenter = false;
			handleImage.color = normalColor;

			var handleImageRectTransform = handleObject.GetComponent<RectTransform>();
			handleImageRectTransform.anchorMin = Vector2.zero;
			handleImageRectTransform.anchorMax = Vector2.one;
			handleImageRectTransform.sizeDelta = Vector2.zero;

			var borderImage = borderObject.AddComponent<Image>();
			borderImage.sprite = resources.RotaryBorder;
			borderImage.type = Image.Type.Sliced;
			borderImage.color = normalColor;

			var borderRectTransform = borderObject.GetComponent<RectTransform>();
			borderRectTransform.anchorMin = Vector2.zero;
			borderRectTransform.anchorMax = Vector2.one;
			borderRectTransform.sizeDelta = Vector2.zero;

			var rotary = root.AddComponent<OSCRotary>();
			rotary.HandleImage = handleImage;
			rotary.targetGraphic = handleImage;

			return root;
		}

		public static GameObject CreateMultiplySlidersVertical(Resources resources)
		{
			return CreateMultiplySliders<VerticalLayoutGroup>(resources, "Multiply Sliders Vertical");
		}

		public static GameObject CreateMultiplySlidersHorizontal(Resources resources)
		{
			return CreateMultiplySliders<HorizontalLayoutGroup>(resources, "Multiply Sliders Horizontal");
		}

		#endregion

		#region Private Static Methods

		private static GameObject CreateMultiplySliders<T>(Resources resources, string name) where T : HorizontalOrVerticalLayoutGroup
		{
			Color normalColor;
			Color backgroundColor;
			PrepareColors(resources.Color, out normalColor, out backgroundColor);

			var root = CreateUIElementRoot(name, new Vector2(320, 320));

			var borderObject = CreateUIObject("Multiply Sliders Border", root);
			var containerObject = CreateUIObject("Multiply Sliders Container", root);

			var borderImage = borderObject.AddComponent<Image>();
			borderImage.sprite = resources.PanelBorder;
			borderImage.type = Image.Type.Sliced;
			borderImage.color = normalColor;

			var borderRectTransform = borderObject.GetComponent<RectTransform>();
			borderRectTransform.anchorMin = Vector2.zero;
			borderRectTransform.anchorMax = Vector2.one;
			borderRectTransform.sizeDelta = Vector2.zero;

			var containerLayout = containerObject.AddComponent<T>();
			containerLayout.spacing = 2f;

			var containerRectTransform = containerObject.GetComponent<RectTransform>();
			containerRectTransform.anchorMin = Vector2.zero;
			containerRectTransform.anchorMax = Vector2.one;
			containerRectTransform.sizeDelta = new Vector2(-8, -8);

			var multiplySliders = root.AddComponent<OSCMultiplySliders>();
			multiplySliders.LayoutGroup = containerLayout;
			multiplySliders.DefaultColor = resources.Color;

			for (int counter = 0; counter < 5; counter++)
			{
				var sliderObject = CreateSlider(resources);
				sliderObject.name = string.Format("Slider: {0}", counter);

				var slider = sliderObject.GetComponent<OSCSlider>();
				slider.MultiplyController = multiplySliders;

				var sliderRect = sliderObject.GetComponent<RectTransform>();
				sliderRect.SetParent(containerRectTransform);
				sliderRect.localPosition = Vector3.zero;
				sliderRect.localScale = Vector3.zero;

				multiplySliders.AddSlider(slider);
			}

			return root;
		}

		private static void PrepareColors(Color baseColor, out Color normalColor, out Color backgroundColor)
		{
			normalColor = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);
			backgroundColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0.1f);
		}

		private static GameObject CreateUIElementRoot(string name, Vector2 size)
		{

#if UNITY_2019_2_OR_NEWER
			var gameObject = DefaultControls.factory.CreateGameObject(name);
			gameObject.AddComponent<RectTransform>().sizeDelta = size;

			return gameObject;
#else
			if (_createUIElementRootMethod == null)
			{
				var defaultControlsType = typeof(DefaultControls);
				_createUIElementRootMethod = defaultControlsType.GetMethod("CreateUIElementRoot", BindingFlags.Static | BindingFlags.NonPublic);
			}

			return (GameObject) _createUIElementRootMethod.Invoke(null, new object[] {name, size});
#endif
		}

		private static GameObject CreateUIObject(string name, GameObject parent)
		{
			if (_createUIObjectMethod == null)
			{
				var defaultControls = typeof(DefaultControls);
				_createUIObjectMethod = defaultControls.GetMethod("CreateUIObject", BindingFlags.Static | BindingFlags.NonPublic);
			}

#if UNITY_2019_2_OR_NEWER
			var gameObject = (GameObject) _createUIObjectMethod.Invoke(null, new object[] {name, parent, new Type[0]});
			gameObject.AddComponent<RectTransform>();
			return gameObject;
#else
			return (GameObject) _createUIObjectMethod.Invoke(null, new object[] {name, parent});
#endif
		}

		private static void SetDefaultColorTransitionValues(Selectable slider)
		{
			if (_setDefaultColorTransitionMethod == null)
			{
				var defaultControls = typeof(DefaultControls);
				_setDefaultColorTransitionMethod = defaultControls.GetMethod("SetDefaultColorTransitionValues", BindingFlags.Static | BindingFlags.NonPublic);
			}

			_setDefaultColorTransitionMethod.Invoke(null, new object[] {slider});
		}

		#endregion
	}
}