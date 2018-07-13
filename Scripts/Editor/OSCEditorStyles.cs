/* Copyright (c) 2018 ExT (V.Sigalkin) */

using UnityEditor;
using UnityEngine;

namespace extOSC.Editor
{
    public static class OSCEditorStyles
    {
        #region Static Private Vars

        private static GUIStyle _windowTitle;

        private static GUIStyle _segmentTitle;

        private static GUIStyle _splitter;

        private static GUIStyle _consoleItemBackEven;

        private static GUIStyle _consoleItemBackOdd;

        private static GUIStyle _consoleLabel;

        private static GUIStyle _consoleBoldLabel;

        private static GUIStyle _centerLabel;

        private static GUIStyle _centerBoldLabel;

        #endregion

        #region Static Public Vars

        public static GUIStyle OSCTitle
        {
            get
            {
                if (_windowTitle == null)
                {
                    _windowTitle = new GUIStyle(EditorStyles.label)
                    {
                        fixedHeight = OSCEditorTextures.IronWall.height
                    };
                }

                return _windowTitle;
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

        public static GUIStyle SegmentTitle
        {
            get
            {
                if (_segmentTitle == null)
                {
                    _segmentTitle = new GUIStyle(EditorStyles.toolbar)
                    {
                        fontSize = 1,
                        fixedHeight = 30
                    };
                }

                return _segmentTitle;
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

        #endregion
    }
}