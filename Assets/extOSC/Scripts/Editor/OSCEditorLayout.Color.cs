/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System.Collections.Generic;

namespace extOSC.Editor
{
    public static partial class OSCEditorLayout
    {
        #region Static Private Vars

        private static Stack<Color> _colorStack = new Stack<Color>();

        #endregion

        #region Static Public Methods

        public static void BeginColor(bool custom, Color customColor)
        {
            if (custom)
            {
                // Use custom color.
                BeginColor(customColor);
            }
            else
            {
                // Use current color.
                BeginColor(GUI.color);
            }
        }

        public static void BeginColor(bool custom, Color defaultColor, Color customColor)
        {
            if (custom)
            {
                // Use custom color.
                BeginColor(customColor);
            }
            else
            {
                // Use default color.
                BeginColor(defaultColor);
            }
        }

        public static void BeginColor(Color customColor)
        {
            // Pull color in stack.
            _colorStack.Push(GUI.color);

            // Change color.
            GUI.color = customColor;
        }

        public static void EndColor()
        {
            // Return color from stack.
            GUI.color = _colorStack.Pop();
        }

        #endregion
    }
}