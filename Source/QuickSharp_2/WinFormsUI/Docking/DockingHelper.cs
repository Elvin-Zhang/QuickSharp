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
using System.Drawing;

namespace WeifenLuo.WinFormsUI.Docking
{
    public static class DockingHelper
    {
        public static Color ThemeMenuForeColor { get; set; }
        public static Color ThemeDocumentTabActiveBorder { get; set; }
        public static Color ThemeDocumentTabInactiveBorder { get; set; }
        public static Color ThemeToolWindowTabBorder { get; set; }
        public static bool HideDocumentWindowBorder { get; set; }
        public static bool UseLightDockStripImages { get; set; }

        static DockingHelper()
        {
            ThemeMenuForeColor = Color.Empty;
            ThemeDocumentTabActiveBorder = Color.Empty;
            ThemeDocumentTabInactiveBorder = Color.Empty;
            ThemeToolWindowTabBorder = Color.Empty;
            HideDocumentWindowBorder = false;
            UseLightDockStripImages = false;
        }
    }
}
