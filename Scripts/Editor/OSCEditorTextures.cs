/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEditor;
using UnityEngine;

namespace extOSC.Editor
{
	public static class OSCEditorTextures
	{
		#region Static Private Vars

		private const string _defaultFolder = "extOSC/";

		private static Texture2D _iwIcon;

		private static Texture2D _iwIconSmall;

		private static Texture2D _splitterTexture;

		private static Texture2D _receiverTexture;

		private static Texture2D _transmitterTexture;

		private static Texture2D _messageTexture;

		private static Texture2D _bundleTexture;

		private static bool _isProSkin;

		#endregion

		#region Static Public Vars

		public static Texture2D IronWall
		{
			get
			{
				if (_iwIcon == null || EditorGUIUtility.isProSkin != _isProSkin)
				{
					_isProSkin = EditorGUIUtility.isProSkin;

					if (_iwIcon != null)
					{
						Resources.UnloadAsset(_iwIcon);
					}

					_iwIcon = LoadTexture(_isProSkin ? "IW_logo_light" : "IW_logo_dark");
				}

				return _iwIcon;
			}
		}

		public static Texture2D IronWallSmall
		{
			get
			{
				if (_iwIconSmall == null || EditorGUIUtility.isProSkin != _isProSkin)
				{
					_isProSkin = EditorGUIUtility.isProSkin;

					if (_iwIconSmall != null)
					{
						Resources.UnloadAsset(_iwIconSmall);
					}

					_iwIconSmall = LoadTexture(_isProSkin ? "IW_small_logo_light" : "IW_small_logo_dark");
				}

				return _iwIconSmall;
			}
		}

		public static Texture2D Splitter
		{
			get
			{
				if (_splitterTexture == null)
				{
					_splitterTexture = new Texture2D(2, 2);
					_splitterTexture.hideFlags = HideFlags.DontSave;

					var colors = new Color32[_splitterTexture.height * _splitterTexture.width];

					for (var i = 0; i < colors.Length; i++)
					{
						colors[i] = new Color(0f, 0f, 0f, 0.5f);
					}

					_splitterTexture.SetPixels32(colors);
					_splitterTexture.Apply();
				}

				return _splitterTexture;
			}
		}

		public static Texture2D Transmitter
		{
			get
			{
				if (_transmitterTexture == null || EditorGUIUtility.isProSkin != _isProSkin)
				{
					_isProSkin = EditorGUIUtility.isProSkin;

					if (_transmitterTexture != null)
					{
						Resources.UnloadAsset(_transmitterTexture);
					}

					_transmitterTexture = LoadTexture(_isProSkin ? "OSC_transmitter_light" : "OSC_transmitter_dark");
				}

				return _transmitterTexture;
			}
		}

		public static Texture2D Receiver
		{
			get
			{
				if (_receiverTexture == null || EditorGUIUtility.isProSkin != _isProSkin)
				{
					_isProSkin = EditorGUIUtility.isProSkin;

					if (_receiverTexture != null)
					{
						Resources.UnloadAsset(_receiverTexture);
					}

					_receiverTexture = LoadTexture(_isProSkin ? "OSC_receiver_light" : "OSC_receiver_dark");
				}

				return _receiverTexture;
			}
		}

		public static Texture2D Message
		{
			get
			{
				if (_messageTexture == null || EditorGUIUtility.isProSkin != _isProSkin)
				{
					_isProSkin = EditorGUIUtility.isProSkin;

					if (_messageTexture != null)
					{
						Resources.UnloadAsset(_messageTexture);
					}

					_messageTexture = LoadTexture(_isProSkin ? "OSC_message_light" : "OSC_message_dark");
				}

				return _messageTexture;
			}
		}

		public static Texture2D Bundle
		{
			get
			{
				if (_bundleTexture == null || EditorGUIUtility.isProSkin != _isProSkin)
				{
					_isProSkin = EditorGUIUtility.isProSkin;

					if (_bundleTexture != null)
					{
						Resources.UnloadAsset(_bundleTexture);
					}

					_bundleTexture = LoadTexture(_isProSkin ? "OSC_bundle_light" : "OSC_bundle_dark");
				}

				return _bundleTexture;
			}
		}

		#endregion

		#region Static Private Methods

		private static Texture2D LoadTexture(string fileName)
		{
			return Resources.Load<Texture2D>(_defaultFolder + fileName);
		}

		#endregion
	}
}