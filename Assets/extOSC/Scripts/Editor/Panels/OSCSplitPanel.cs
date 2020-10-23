/* Copyright (c) 2020 ExT (V.Sigalkin) */

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

		private class PanelContainer
		{
			public OSCPanel Panel;

			public float MinimumSize;

			public float MinimumSizePixel;

			public float Size = 1f;
		}

		private class Splitter
		{
			public OSCPanel FirstPanel;

			public OSCPanel SecondPanel;

			public PanelContainer LeftTopContainer;

			public PanelContainer RightDownContainer;

			public Rect SplitterRect;
		}

		#endregion

		#region Public Vars

		public float SplitterSize = 1;

		public SplitOrientation Orientation;

		#endregion

		#region Private Vars

		private float _splitterMargin = 0f;

		private float _splitterTouchMargin = 5f;

		private readonly List<Splitter> _splitters = new List<Splitter>();

		private readonly List<PanelContainer> _panelContainers = new List<PanelContainer>();

		private Splitter _selectedSplitter;

		#endregion

		#region Public Methods

		public OSCSplitPanel(OSCWindow window) : base(window)
		{ }

		public void AddPanel(OSCPanel panel, float minimalSize, float size = 0.5f)
		{
			var data = new PanelContainer();
			data.Panel = panel;
			data.MinimumSizePixel = minimalSize;
			data.Size = size;

			_panelContainers.Add(data);

			var fullSize = 0f;

			foreach (var storedData in _panelContainers)
			{
				fullSize += storedData.Size;
			}

			if (fullSize > 1f)
			{
				foreach (var panelContainer in _panelContainers)
				{
					panelContainer.Size = 1f / _panelContainers.Count;
				}
			}

			if (_panelContainers.Count < 2)
				return;

			_splitters.Clear();
			_selectedSplitter = null;

			Splitter previousSplitter = null;

			foreach (var panelContainer in _panelContainers)
			{
				if (previousSplitter != null)
				{
					previousSplitter.SecondPanel = panelContainer.Panel;
					previousSplitter.RightDownContainer = panelContainer;

					_splitters.Add(previousSplitter);
				}

				previousSplitter = new Splitter()
				{
					FirstPanel = panelContainer.Panel,
					LeftTopContainer = panelContainer
				};
			}

			LoadData();
		}

		public float CalculateMinimumSize(SplitOrientation orientation)
		{
			var minimumSize = 0f;

			foreach (var panelContainer in _panelContainers)
			{
				if (panelContainer.Panel is OSCSplitPanel splitPanel)
				{
					if (splitPanel.Orientation == orientation)
					{
						minimumSize += splitPanel.CalculateMinimumSize(orientation);
					}
				}

				if (orientation == Orientation)
				{
					minimumSize += panelContainer.MinimumSizePixel;
				}

				if (SplitterSize > 0)
					minimumSize += _splitterMargin * 2f + SplitterSize;
			}

			if (SplitterSize > 0)
				minimumSize -= _splitterMargin * 2f + SplitterSize;

			return minimumSize;
		}

		#endregion

		#region Protected Methods

		protected override void DrawContent(ref Rect contentRect)
		{
			if (SplitterSize > 0)
			{
				if (Orientation == SplitOrientation.Horizontal)
				{
					contentRect.width -= Mathf.Max(_panelContainers.Count - 1, 0) * (SplitterSize + _splitterMargin * 2);
				}
				else if (Orientation == SplitOrientation.Vertical)
				{
					contentRect.height -= Mathf.Max(_panelContainers.Count - 1, 0) * (SplitterSize + _splitterMargin * 2);
				}
			}

			var step = 0f;

			foreach (var data in _panelContainers)
			{
				var panel = data.Panel;
				var panelRect = new Rect();

				if (panel is OSCSplitPanel splitPanel)
				{
					var realMinimumSize = splitPanel.CalculateMinimumSize(Orientation);

					if (data.MinimumSizePixel < realMinimumSize)
						data.MinimumSizePixel = realMinimumSize;
				}

				if (Orientation == SplitOrientation.Horizontal)
				{
					panelRect.x = step;

					step += panelRect.width = contentRect.width * data.Size;
					panelRect.height = contentRect.height;

					data.MinimumSize = data.MinimumSizePixel / contentRect.width;
				}
				else if (Orientation == SplitOrientation.Vertical)
				{
					panelRect.y = step;

					panelRect.width = contentRect.width;
					step += panelRect.height = contentRect.height * data.Size;

					data.MinimumSize = data.MinimumSizePixel / contentRect.height;
				}

				panel.Rect = panelRect;
				panel.Draw();

				if (SplitterSize > 0)
				{
					var splitter = GetSplitter(panel);
					if (splitter != null)
					{
						var splitterRect = new Rect();

						if (Orientation == SplitOrientation.Horizontal)
						{
							splitterRect.x = step + _splitterMargin;
							splitterRect.width = SplitterSize;
							splitterRect.height = contentRect.height;
						}
						else if (Orientation == SplitOrientation.Vertical)
						{
							splitterRect.y = step + _splitterMargin;
							splitterRect.height = SplitterSize;
							splitterRect.width = contentRect.width;
						}

						GUI.DrawTexture(splitterRect, OSCEditorTextures.Splitter);

						if (Orientation == SplitOrientation.Horizontal)
						{
							splitterRect.x -= _splitterTouchMargin;
							splitterRect.width += _splitterTouchMargin * 2;
							splitterRect.height = contentRect.height;
						}
						else if (Orientation == SplitOrientation.Vertical)
						{
							splitterRect.y -= _splitterTouchMargin;
							splitterRect.height += _splitterTouchMargin * 2;
							splitterRect.width = contentRect.width;
						}

						splitter.SplitterRect = splitterRect;

						EditorGUIUtility.AddCursorRect(splitterRect, Orientation == SplitOrientation.Horizontal ? MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical);
					}

					step += SplitterSize + (_splitterMargin * 2);
				}
			}
		}

		protected override void PostDrawContent()
		{
			if (Event.current != null)
			{
				switch (Event.current.type)
				{
					case EventType.MouseDown:
						foreach (var splitter in _splitters)
						{
							if (splitter.SplitterRect.Contains(Event.current.mousePosition))
							{
								_selectedSplitter = splitter;

								break;
							}
						}

						break;
					case EventType.MouseDrag:
						if (_selectedSplitter != null)
						{
							var pixelSize = 0f;
							var newPixelSize = 0f;
							var fullSize = _selectedSplitter.LeftTopContainer.Size + _selectedSplitter.RightDownContainer.Size;

							if (Orientation == SplitOrientation.Horizontal)
							{
								pixelSize = _selectedSplitter.FirstPanel.Rect.width +
											_selectedSplitter.SecondPanel.Rect.width;

								newPixelSize = _selectedSplitter.FirstPanel.Rect.width + Event.current.delta.x;
							}
							else if (Orientation == SplitOrientation.Vertical)
							{
								pixelSize = _selectedSplitter.FirstPanel.Rect.height +
											_selectedSplitter.SecondPanel.Rect.height;

								newPixelSize = _selectedSplitter.FirstPanel.Rect.height + Event.current.delta.y;
							}

							var firstSize = Mathf.Clamp(newPixelSize / pixelSize, _selectedSplitter.LeftTopContainer.MinimumSize / fullSize, 1);
							var secondSize = Mathf.Clamp(1f - firstSize, _selectedSplitter.RightDownContainer.MinimumSize / fullSize, 1);
							firstSize = 1f - secondSize; // Idk how it work...

							firstSize = fullSize * firstSize;
							secondSize = fullSize * secondSize;

							_selectedSplitter.LeftTopContainer.Size = firstSize;
							_selectedSplitter.RightDownContainer.Size = secondSize;

							Window.Repaint();
						}

						break;
					case EventType.MouseUp:
						if (_selectedSplitter != null)
						{
							_selectedSplitter = null;

							SaveData();
						}

						break;
				}
			}
		}

		#endregion

		#region Private Methods

		private void LoadData()
		{
			if (Window == null) return;

			foreach (var storedData in _panelContainers)
			{
				var key = string.Format($"osc.editor.splitPanel:{Window.GetType().FullName}>{storedData.Panel.GetType().FullName}");

				storedData.Size = EditorPrefs.GetFloat(key, storedData.Size);
			}
		}

		private void SaveData()
		{
			if (Window == null) return;

			foreach (var storedData in _panelContainers)
			{
				var key = string.Format($"osc.editor.splitPanel:{Window.GetType().FullName}>{storedData.Panel.GetType().FullName}");

				EditorPrefs.SetFloat(key, storedData.Size);
			}
		}

		private Splitter GetSplitter(OSCPanel firstPanel)
		{
			foreach (var splitter in _splitters)
			{
				if (splitter.FirstPanel == firstPanel)
					return splitter;
			}

			return null;
		}

		#endregion
	}
}