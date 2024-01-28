/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

using extOSC.Editor.Windows;

namespace extOSC.Editor.Panels
{
	public class OSCSplitPanel : OSCPanel
	{
		#region Extensions

		public enum SplitOrientation
		{
			Horizontal,
			Vertical
		}

		#endregion

		#region Private Vars

		private readonly float _splitterSize = 1f;

		private readonly float _splitterMargin = 0f;

		private readonly float _splitterTouchMargin = 5f;

		private OSCPanel _leftPanel;

		private OSCPanel _rightPanel;

		private Rect _splitterRect;

		private float _splitterPosition = 0.5f;

		private bool _splitterPress;
		
		public SplitOrientation _orientation;
		
		#endregion

		#region Public Methods

		public OSCSplitPanel(OSCWindow window) : base(window)
		{ }

		public void Setup(OSCPanel leftPanel, OSCPanel rightPanel, SplitOrientation orientation = default)
		{
			_leftPanel = leftPanel;
			_rightPanel = rightPanel;
			_orientation = orientation;

			LoadData();
		}

		#endregion

		#region Protected Methods

		protected override void DrawContent(ref Rect contentRect)
		{
			var splitterSize = 0f;
			if (_splitterSize > 0)
			{
				splitterSize = _splitterSize + _splitterMargin * 2;
				
				if (_orientation == SplitOrientation.Horizontal)
				{
					contentRect.width -= splitterSize;
				}
				else if (_orientation == SplitOrientation.Vertical)
				{
					contentRect.height -= splitterSize;
				}
			}
			
			// left panel
			var leftRect = new Rect();
			if (_orientation == SplitOrientation.Horizontal)
			{
				leftRect.width = contentRect.width * _splitterPosition;
				leftRect.height = contentRect.height;
			}
			else if (_orientation == SplitOrientation.Vertical)
			{
				leftRect.width = contentRect.width;
				leftRect.height = contentRect.height * _splitterPosition;
			}
			
			_leftPanel.Rect = leftRect;
			_leftPanel.Draw();
			
			//right panel
			var rightRect = new Rect();
			if (_orientation == SplitOrientation.Horizontal)
			{
				rightRect.x = leftRect.width + splitterSize;

				rightRect.width = contentRect.width - leftRect.width;
				rightRect.height = contentRect.height;
			}
			else if (_orientation == SplitOrientation.Vertical)
			{
				rightRect.y = leftRect.height +  splitterSize;
				
				rightRect.width = contentRect.width;
				rightRect.height = contentRect.height - leftRect.height;
			}
			
			_rightPanel.Rect = rightRect;
			_rightPanel.Draw();
			
			// splitter
			var splitterRect = new Rect();

			if (_orientation == SplitOrientation.Horizontal)
			{
				splitterRect.x = leftRect.width + _splitterMargin;
				splitterRect.width = _splitterSize;
				splitterRect.height = contentRect.height;
			}
			else if (_orientation == SplitOrientation.Vertical)
			{
				splitterRect.y = leftRect.height + _splitterMargin;
				splitterRect.height = _splitterSize;
				splitterRect.width = contentRect.width;
			}

			GUI.DrawTexture(splitterRect, OSCEditorTextures.Splitter);

			if (_orientation == SplitOrientation.Horizontal)
			{
				splitterRect.x -= _splitterTouchMargin;
				splitterRect.width += _splitterTouchMargin * 2;
				splitterRect.height = contentRect.height;
			}
			else if (_orientation == SplitOrientation.Vertical)
			{
				splitterRect.y -= _splitterTouchMargin;
				splitterRect.height += _splitterTouchMargin * 2;
				splitterRect.width = contentRect.width;
			}
			
			EditorGUIUtility.AddCursorRect(splitterRect, _orientation == SplitOrientation.Horizontal ? MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical);

			_splitterRect = splitterRect;
		}

		protected override void PostDrawContent()
		{
			if (Event.current != null)
			{
				switch (Event.current.type)
				{
					case EventType.MouseDown:
						if (_splitterRect.Contains(Event.current.mousePosition))
						{
							_splitterPress = true;

							Event.current.Use();
						}

						break;
					case EventType.MouseDrag:
						if (_splitterPress)
						{
							var mousePosition = Event.current.mousePosition;
 
							if (_orientation == SplitOrientation.Horizontal)
							{
								_splitterPosition = (mousePosition / Rect.size).x;
							}
							else if (_orientation == SplitOrientation.Vertical)
							{
								_splitterPosition = (mousePosition / Rect.size).y;
							}

							_splitterPosition = Mathf.Clamp(_splitterPosition, 0.15f, 0.85f);

							Window.Repaint();
							Event.current.Use();
						}

						break;
					case EventType.MouseUp:
					{
						if (_splitterPress)
						{
							_splitterPress = false;

							SaveData();
							Event.current.Use();
						}
					}

					break;
				}
			}
		}

		#endregion

		#region Private Methods

		private void LoadData()
		{
			if (Window == null) 
				return;

			var key = $"osc.editor.panels.split:{Window.GetType().FullName}>{_leftPanel.GetType().FullName}:{_rightPanel.GetType().FullName}";
			_splitterPosition = EditorPrefs.GetFloat(key, 0.5f);
		}
		
		private void SaveData()
		{
			if (Window == null) return;
		
			var key = $"osc.editor.panels.split:{Window.GetType().FullName}>{_leftPanel.GetType().FullName}:{_rightPanel.GetType().FullName}";
			EditorPrefs.SetFloat(key, _splitterPosition);
		}
		
		#endregion
	}
}