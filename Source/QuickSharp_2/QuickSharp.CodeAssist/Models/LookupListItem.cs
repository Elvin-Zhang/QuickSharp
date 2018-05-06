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

using System.Collections.Generic;

namespace QuickSharp.CodeAssist
{
    /// <summary>
    /// Represents an item in the code assist pop-up window.
    /// </summary>
    public class LookupListItem : LookupMenuItem
    {
        /// <summary>
        /// The item's tooltip text.
        /// </summary>
        public string ToolTipText { get; set; }

        /// <summary>
        /// The item's category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Context menu items associated with the item.
        /// </summary>
        public List<LookupMenuItem> MenuItems { get; set; }

        /// <summary>
        /// Create a LookupList item.
        /// </summary>
        public LookupListItem()
        {
            MenuItems = new List<LookupMenuItem>();
        }
    }
}
