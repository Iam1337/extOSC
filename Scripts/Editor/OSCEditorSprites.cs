/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Editor
{
	public static class OSCEditorSprites
	{
		#region Static Private Vars

		private const string _defaultFolder = "extOSC/";

		private static Sprite _panelFilledSprite;

		private static Sprite _panelBorderSprite;

		private static Sprite _rotaryFilledSprite;

		private static Sprite _rotaryFilledMaskSprite;

		private static Sprite _rotaryBorderSprite;

		#endregion

		#region Static Public Vars

		public static Sprite PanelFilled
		{
			get
			{
				if (_panelFilledSprite == null)
				{
					_panelFilledSprite = LoadUISprite("OSC_UI_panel_filled");
				}

				return _panelFilledSprite;
			}
		}

		public static Sprite PanelBorder
		{
			get
			{
				if (_panelBorderSprite == null)
				{
					_panelBorderSprite = LoadUISprite("OSC_UI_panel_border");
				}

				return _panelBorderSprite;
			}
		}

		public static Sprite RotaryFilled
		{
			get
			{
				if (_rotaryFilledSprite == null)
				{
					_rotaryFilledSprite = LoadUISprite("OSC_UI_rotary_filled");
				}

				return _rotaryFilledSprite;
			}
		}

		public static Sprite RotaryFilledMask
		{
			get
			{
				if (_rotaryFilledMaskSprite == null)
				{
					_rotaryFilledMaskSprite = LoadUISprite("OSC_UI_rotary_filled_mask");
				}

				return _rotaryFilledMaskSprite;
			}
		}

		public static Sprite RotaryBorder
		{
			get
			{
				if (_rotaryBorderSprite == null)
				{
					_rotaryBorderSprite = LoadUISprite("OSC_UI_rotary_border");
				}

				return _rotaryBorderSprite;
			}
		}

		#endregion

		#region Static Private Methods

		private static Sprite LoadUISprite(string fileName)
		{
			return Resources.Load<Sprite>(_defaultFolder + fileName);
		}

		#endregion
	}
}