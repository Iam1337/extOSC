/* Copyright (c) 2020 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Editor.Windows;

namespace extOSC.Editor.Panels
{
	public class OSCPanel
	{
		#region Public Vars

		public Rect Rect;

		public readonly OSCWindow Window;

		#endregion

		#region Public Methods

		public OSCPanel(OSCWindow window)
		{
			Window = window;
		}

		public virtual void Draw()
		{
			using (new GUILayout.AreaScope(Rect))
			{
				var contentRect = Rect;
				contentRect.x = contentRect.y = 0;

				DrawContent(ref contentRect);
				PostDrawContent();
			}
		}

		#endregion

		#region Protected Methods

		protected virtual void DrawContent(ref Rect contentRect)
		{ }

		protected virtual void PostDrawContent()
		{ }

		#endregion
	}
}