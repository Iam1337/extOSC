/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEngine;
using UnityEditor;

using extOSC.Core;
using extOSC.Core.Console;
using extOSC.Editor.Windows;

namespace extOSC.Editor.Panels
{
    public class OSCPanelConsoleFilter : OSCPanel
    {
        #region Public Methods

        public OSCPanelConsoleFilter(OSCWindow parentWindow, string panelId) : base(parentWindow, panelId)
        {
        }

        #endregion
    }
}