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

namespace QuickSharp.Core
{
    /// <summary>
    /// Presents theme properties for use throughout the
    /// application. Rather than directly manipulate plugin UI
    /// elements to enforce theme colors this presents the values
    /// for plugins to read if available. Like ClientFlags this allows
    /// settings to be communicated to plugins without having to create
    /// dependencies between them.
    /// </summary>
    public class ThemeFlags
    {
        /// <summary>
        /// The main form background color.
        /// </summary>
        public Color MainBackColor { get; set; }

        /// <summary>
        /// The background color for views (such as docked forms).
        /// Actual usage is plugin dependent.
        /// </summary>
        public Color ViewBackColor { get; set; }

        /// <summary>
        /// The foreground color for views (such as docked forms).
        /// Actual usage is plugin dependent.
        /// </summary>
        public Color ViewForeColor { get; set; }

        /// <summary>
        /// An alternative background color for views (such as
        /// docked forms). Actual usage is plugin dependent.
        /// </summary>
        public Color ViewAltBackColor { get; set; }

        /// <summary>
        /// An alternative foreground color for views (such as
        /// docked forms). Actual usage is plugin dependent.
        /// </summary>
        public Color ViewAltForeColor { get; set; }

        /// <summary>
        /// Show view elements (such as tree views) with a border.
        /// Actual usage is plugin dependent.
        /// </summary>
        public bool ViewShowBorder { get; set; }

        /// <summary>
        /// main menu foreground (text) color.
        /// </summary>
        public Color MenuForeColor { get; set; }

        /// <summary>
        /// Hide main menu images if true.
        /// </summary>
        public bool MenuHideImages { get; set; }

        /// <summary>
        /// Create a ThemeFlags instance.
        /// </summary>
        public ThemeFlags()
        {
            MainBackColor = Color.Empty;
            ViewBackColor = Color.Empty;
            ViewForeColor = Color.Empty;
            ViewAltBackColor = Color.Empty;
            ViewAltForeColor = Color.Empty;
            ViewShowBorder = true;
            MenuForeColor = Color.Empty;
            MenuHideImages = false;
        }
    }
}
