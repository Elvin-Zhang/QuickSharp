/*
 * QuickSharp Copyright (C) 2008-2011 Steve Walker.
 *
 * This file is part of QuickSharp.
 *
 * QuickSharp is free software: you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation, either version 3 of the License, or (at your option)
 * any later version.
 *
 * QuickSharp is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
 * for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with QuickSharp. If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms.VisualStyles;
using Microsoft.Win32;

namespace QuickSharp.Core
{
    /// <summary>
    /// Utility class for theme color tables.
    /// </summary>
    public static class DisplayInformation
    {
        /// <summary>
        /// Enable the UserPreferenceChanged event handler and set the ColorScheme.
        /// </summary>
        static DisplayInformation()
        {
            SystemEvents.UserPreferenceChanged +=
                new UserPreferenceChangedEventHandler(
                    OnUserPreferenceChanged);

            SetScheme();
        }

        private static void OnUserPreferenceChanged(
            object sender, UserPreferenceChangedEventArgs e)
        {
            SetScheme();
        }

        private static void SetScheme()
        {
            isLunaTheme = false;

            if (VisualStyleRenderer.IsSupported)
            {
                colorScheme = VisualStyleInformation.ColorScheme;

                if (!VisualStyleInformation.IsEnabledByUser)
                    return;

                StringBuilder builder1 = new StringBuilder(0x200);
                GetCurrentThemeName(builder1, builder1.Capacity, null, 0, null, 0);

                string text1 = builder1.ToString();

                isLunaTheme = string.Equals(
                    "luna.msstyles", Path.GetFileName(text1), 
                    StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                colorScheme = null;
            }
        }

        /// <summary>
        /// The color scheme.
        /// </summary>
        public static string ColorScheme
        {
            get
            {
                return colorScheme;
            }
        }

        /// <summary>
        /// Is Luna the current theme.
        /// </summary>
        public static bool IsLunaTheme
        {
            get
            {
                return isLunaTheme;
            }
        }

        [ThreadStatic]
        private static string colorScheme;
        [ThreadStatic]
        private static bool isLunaTheme;
        private const string lunaFileName = "luna.msstyles";
        [DllImport("uxtheme.dll", CharSet = CharSet.Auto)]
        public static extern int GetCurrentThemeName(
            StringBuilder pszThemeFileName,
            int dwMaxNameChars,
            StringBuilder pszColorBuff,
            int dwMaxColorChars,
            StringBuilder pszSizeBuff,
            int cchMaxSizeChars);
    }
}
