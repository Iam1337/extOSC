/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEditor;
using UnityEngine;

namespace extOSC.Editor
{
	public static class OSCEditorStyles
	{
		#region Static Private Vars

		private static GUIStyle _windowTitle;

		private static GUIStyle _box;

		private static GUIStyle _segmentTitle;

		private static GUIStyle _consoleItemBackEven;

		private static GUIStyle _consoleItemBackOdd;

		private static GUIStyle _consoleLabel;

		private static GUIStyle _consoleTimeLabel;

		private static GUIStyle _consoleBoldLabel;

		private static GUIStyle _centerLabel;

		private static GUIStyle _centerBoldLabel;

		private static GUIStyle _searchField;

		private static GUIStyle _searchFieldPlaceholder;
		
		#endregion

		#region Static Public Vars

		public static GUIStyle Box
		{
			get
			{
				if (_box == null)
				{
					_box = new GUIStyle("box");
				}

				return _box;
			}
		}

		public static GUIStyle CenterLabel
		{
			get
			{
				if (_centerLabel == null)
				{
					_centerLabel = new GUIStyle(EditorStyles.label)
					{
						alignment = TextAnchor.MiddleCenter
					};
				}

				return _centerLabel;
			}
		}

		public static GUIStyle CenterBoldLabel
		{
			get
			{
				if (_centerBoldLabel == null)
				{
					_centerBoldLabel = new GUIStyle(EditorStyles.label)
					{
						alignment = TextAnchor.MiddleCenter,
						fontStyle = FontStyle.Bold
					};
				}

				return _centerBoldLabel;
			}
		}

		public static GUIStyle ConsoleItemBackEven
		{
			get
			{
				if (_consoleItemBackEven == null)
				{
					_consoleItemBackEven = new GUIStyle("CN EntryBackEven");
				}

				return _consoleItemBackEven;
			}
		}

		public static GUIStyle ConsoleItemBackOdd
		{
			get
			{
				if (_consoleItemBackOdd == null)
				{
					_consoleItemBackOdd = new GUIStyle("CN EntryBackOdd");
				}

				return _consoleItemBackOdd;
			}
		}

		public static GUIStyle ConsoleLabel
		{
			get
			{
				if (_consoleLabel == null)
				{
					_consoleLabel = new GUIStyle(EditorStyles.label);
					_consoleLabel.richText = true;
				}

				return _consoleLabel;
			}
		}

		public static GUIStyle ConsoleTimeLabel
		{
			get
			{
				if (_consoleTimeLabel == null)
				{
					_consoleTimeLabel = new GUIStyle(ConsoleLabel);
					_consoleTimeLabel.alignment = TextAnchor.MiddleRight;
				}

				return _consoleTimeLabel;
			}
		}

		public static GUIStyle ConsoleBoldLabel
		{
			get
			{
				if (_consoleBoldLabel == null)
				{
					_consoleBoldLabel = new GUIStyle(EditorStyles.boldLabel);
					_consoleBoldLabel.richText = true;
				}

				return _consoleBoldLabel;
			}
		}

		public static GUIStyle SearchField
		{
			get
			{
				if (_searchField == null)
				{
					_searchField = new GUIStyle("toolbarTextField");
				}

				return _searchField;
			}
		}

		public static GUIStyle SearchFieldPlaceholder
		{
			get
			{
				if (_searchFieldPlaceholder == null)
				{
					_searchFieldPlaceholder = new GUIStyle("toolbarTextField");
					_searchFieldPlaceholder.active.textColor = Color.gray;
					_searchFieldPlaceholder.normal.textColor = Color.gray;
				}

				return _searchFieldPlaceholder;
			}
		}

		#endregion
	}
}