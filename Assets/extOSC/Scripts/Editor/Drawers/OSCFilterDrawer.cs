/* Copyright (c) 2022 dr. ext (Vladimir Sigalkin) */

using UnityEngine;

using System;

namespace extOSC.Editor.Drawers
{
	public class OSCFilterDrawer
	{
		#region Public Vars

		public string FilterValue
		{
			get { return _filterValue; }
			set { _filterValue = value; }
		}

		#endregion

		#region Private Vars

		private string _filterValue = "";

		private string _controlName = "";

		#endregion

		#region Public Methods

		public OSCFilterDrawer()
		{
			_controlName = "oscfilter_" + Guid.NewGuid();
		}

		public void Draw()
		{
			var fieldPosition = GUILayoutUtility.GetRect(0, 200, 0, 100);
			fieldPosition.y = 2;

			var controlId = GUIUtility.GetControlID("TextField".GetHashCode(), FocusType.Keyboard) + 1;

			GUI.SetNextControlName(_controlName);

			_filterValue = GUI.TextField(fieldPosition, _filterValue, OSCEditorStyles.SearchField);

			ProcessKeys(controlId);

			var controlName = GUI.GetNameOfFocusedControl();
			if (controlName != _controlName && string.IsNullOrEmpty(_filterValue))
			{
				GUI.Label(fieldPosition, "Packet Filter", OSCEditorStyles.SearchFieldPlaceholder);
			}

		}

		#endregion

		#region Private Methods

		// Small hack.
		public void ProcessKeys(int controlId)
		{
			if (controlId == GUIUtility.keyboardControl)
			{
				if (Event.current.type == EventType.KeyUp && (Event.current.modifiers == EventModifiers.Control || Event.current.modifiers == EventModifiers.Command))
				{
					if (Event.current.keyCode == KeyCode.C)
					{
						var editor = (TextEditor) GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
						editor.Copy();

						Event.current.Use();
					}
					else if (Event.current.keyCode == KeyCode.V)
					{
						var textEditor = (TextEditor) GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
						textEditor.Paste();

						_filterValue = textEditor.text;

						Event.current.Use();
					}
					else if (Event.current.keyCode == KeyCode.A)
					{
						var textEditor = (TextEditor) GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
						textEditor.SelectAll();

						Event.current.Use();
					}
				}
			}
		}


		#endregion
	}
}