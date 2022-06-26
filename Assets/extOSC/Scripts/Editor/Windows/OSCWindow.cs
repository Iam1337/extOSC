/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEditor;
using UnityEngine;

using System;

using extOSC.Editor.Panels;

namespace extOSC.Editor.Windows
{
	public abstract class OSCWindow : EditorWindow
	{
		#region Unity Methods

		protected virtual void Awake()
		{ }

		protected virtual void OnEnable()
		{
			LoadWindowSettings();
		}

		protected virtual void OnDisable()
		{
			SaveWindowSettings();
		}

		protected virtual void OnDestroy()
		{ }

		protected abstract void OnGUI();

		#endregion

		#region Protected Methods

		protected virtual void LoadWindowSettings()
		{ }

		protected virtual void SaveWindowSettings()
		{ }

		#endregion
	}

	public class OSCWindow<TWindow, TPanel> : OSCWindow where TWindow : OSCWindow where TPanel : OSCPanel
	{
		#region Public Vars

		public static TWindow Instance => GetWindow<TWindow>(false, "", false);

		#endregion

		#region Protected Vars

		protected TPanel rootPanel
		{
			get
			{
				if (_rootPanel == null)
					_rootPanel = CreateRoot();

				return _rootPanel;
			}
		}

		#endregion

		#region Private Vars

		private TPanel _rootPanel;

		#endregion

		#region Unity Methods

		protected override void OnGUI()
		{
			DrawRootPanel(new Rect(0, 0, position.width, position.height));
		}

		#endregion

		#region Protected Methods

		protected TPanel CreateRoot()
		{
			if (_rootPanel != null)
			{
				Debug.LogErrorFormat("[{0}] Already has root panel!", GetType());
				return default;
			}

			var panel = (TPanel) Activator.CreateInstance(typeof(TPanel), new object[] {this});

			_rootPanel = panel;

			return panel;
		}

		protected void DrawRootPanel(Rect contentRect)
		{
			if (rootPanel == null) return;

			rootPanel.Rect = contentRect;
			rootPanel.Draw();
		}

		#endregion
	}
}