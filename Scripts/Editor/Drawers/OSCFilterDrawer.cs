/* Copyright (c) 2018 ExT (V.Sigalkin) */

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

        #endregion

        #region Public Methods

        public void Draw()
        {
            _filterValue = GUILayout.TextField(_filterValue, OSCEditorStyles.SearchField, GUILayout.MaxWidth(350));

            //TODO: WTF?
            var fieldPosition = GUILayoutUtility.GetLastRect();
            
            var buttonPosition = fieldPosition;
            buttonPosition.x += fieldPosition.width - 10;
            buttonPosition.width = 10;

            var hasValue = !string.IsNullOrEmpty(_filterValue);
            var buttonStyle = hasValue
                ? OSCEditorStyles.SearchFieldCleanButton
                : OSCEditorStyles.SearchFieldCleanButtonNone;

            if (GUI.Button(buttonPosition, GUIContent.none, buttonStyle))
            {
                _filterValue = string.Empty;
            }
        }

        #endregion
    }
}