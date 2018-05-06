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

namespace QuickSharp.CodeAssist
{
    /// <summary>
    /// An item to be displayed in the list presented by the code assis pop-up window.
    /// </summary>
    public class LookupMenuItem
    {
        /// <summary>
        /// The text displayed for the lookup item.
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// The text inserted into the document when the item is selected.
        /// </summary>
        public string InsertText { get; set; }

        /// <summary>
        /// The index of the insertion template used to insert the selected item.
        /// </summary>
        public int TemplateIndex { get; set; }

        /// <summary>
        /// Create a lookup item.
        /// </summary>
        public LookupMenuItem()
        {
            TemplateIndex = 0;
        }
    }
}
