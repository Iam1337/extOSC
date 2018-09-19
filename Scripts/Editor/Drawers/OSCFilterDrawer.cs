/* Copyright (c) 2018 ExT (V.Sigalkin) */

using System;
using UnityEditor;
using UnityEngine;

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
            GUI.SetNextControlName(_controlName);

            var fieldPosition = GUILayoutUtility.GetRect(0, 350, 0, 100);
            fieldPosition.y = 2;

            _filterValue = GUI.TextField(fieldPosition, _filterValue, OSCEditorStyles.SearchField);

            if (Event.current.type != EventType.Layout)
            {
                var controlName = GUI.GetNameOfFocusedControl();
                if (controlName != _controlName && string.IsNullOrEmpty(_filterValue))
                {
                    GUI.Label(fieldPosition, "Packet Filter", OSCEditorStyles.SearchFieldPlaceholder);
                }
                else
                {
                    GUIUtility.keyboardControl = 0;
                }
            }

            
        }

        #endregion
    }
}