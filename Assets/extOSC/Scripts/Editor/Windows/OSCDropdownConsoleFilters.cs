/* Copyright (c) 2018 ExT (V.Sigalkin) */

using extOSC.Editor.Panels;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace extOSC.Editor.Windows
{
    public class OSCDropdownConsoleFilters : OSCDropdownWindow<OSCDropdownConsoleFilters, OSCPanelConsoleFilter>
    {


        public static OSCDropdownConsoleFilters Show(Rect rect)
        {
            var instance = CreateInstance<OSCDropdownConsoleFilters>();
            instance.Init(rect);

            return instance;
        }

        #region Public Methods

        #endregion
    }
}