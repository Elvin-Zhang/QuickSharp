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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace QuickSharp.Core
{
    /// <summary>
    /// Provides a container for docked toolbars.
    /// </summary>
    public class DockedToolStrip
    {
        /// <summary>
        /// The toolbar.
        /// </summary>
        public ToolStrip ToolStrip { get; private set; }

        /// <summary>
        /// The intial row the toolbar is to be docked in. This is just a hint, the
        /// actual contents of the toolbar panel will determine the position
        /// of the toolbar when it is displayed.
        /// </summary>
        public int RowHint { get; set; }

        /// <summary>
        /// The initial horizontal position of the toolbar; the higher the value
        /// the further to the right the toolbar will appear. This is just a hint, the
        /// actual contents of the toolbar panel will determine the position
        /// of the toolbar when it is displayed.
        /// </summary>
        public int ColHint { get; set; }

        /// <summary>
        /// Hide the toolbar on startup. (Internal use only.)
        /// </summary>
        public bool HideOnRestore { get; set; }

        /// <summary>
        /// Create a docked toolbar container.
        /// </summary>
        /// <param name="toolStrip">The toolbar.</param>
        /// <param name="rowHint">A row hint.</param>
        /// <param name="colHint">A column hint.</param>
        public DockedToolStrip(ToolStrip toolStrip, int rowHint, int colHint)
        {
            ToolStrip = toolStrip;
            RowHint = rowHint;
            ColHint = colHint;
        }

        /// <summary>
        /// The name of the toolbar.
        /// </summary>
        public string Name
        {
            get { return ToolStrip.Name; }
        }

        /// <summary>
        /// The text of the toolbar View menu item.
        /// </summary>
        public string Text
        {
            get { return ToolStrip.Text; }
        }

        /// <summary>
        /// Get or set the toolbar visibility.
        /// </summary>
        public bool Visible
        {
            get { return ToolStrip.Visible; }
            set { ToolStrip.Visible = value; }
        }
    }
}
