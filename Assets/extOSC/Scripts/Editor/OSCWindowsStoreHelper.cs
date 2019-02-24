﻿/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEditor;
using UnityEngine;

namespace extOSC.Editor
{
    [InitializeOnLoad]
    public static class OSCWindowsStoreHelper
    {
        #region Static Public Methods

        static OSCWindowsStoreHelper()
        {
            EditorApplication.update += CheckSettings;
        }

        #endregion

        #region Static Private Methods

        private static void CheckSettings()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WSAPlayer)
                return;

            if (!PlayerSettings.WSA.GetCapability(PlayerSettings.WSACapability.InternetClientServer))
            {
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClientServer, true);

                Debug.Log("[OSCWindowsStoreHelper] WSACapability: InternetClientServer now is true.");
            }
        }

        #endregion
    }
}