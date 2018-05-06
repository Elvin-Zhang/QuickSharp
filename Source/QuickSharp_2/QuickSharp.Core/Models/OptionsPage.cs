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
using System.Windows.Forms;
using System.ComponentModel;

namespace QuickSharp.Core
{
    /// <summary>
    /// Base class for options editor pages.
    /// </summary>
    public abstract class OptionsPage : Panel
    {
        /*
         * OptionsPages are grouped according to the GroupText value.
         * If more than one page belongs to a group they will be listed
         * under the GroupText with their PageText values. If only one
         * exists in a group it will be listed as the GroupText.
         * Groups are matched on this text value rather than an ID as
         * coordinating IDs across plugin provides no more advantages
         * than just coordinating the text values themselves. Also
         * different GroupText values might end up being associated
         * with the same ID; in such cases there would be no way to decide
         * which GroupText would be the correct one.
         */

        /// <summary>
        /// The title for the page. Appears as the text of the tree node
        /// when the page appears as a child node of a group in the options
        /// editor.
        /// </summary>
        public string PageText { get; set; }

        /// <summary>
        /// The title of the group the page belongs to. When more than one
        /// page belongs to the group each page appears as a child node of
        /// a group. Where only one page exists in the group the page appears
        /// as a single node with the group title as its text.
        /// </summary>
        public string GroupText { get; set; }

        /// <summary>
        /// Create a new option page. Option pages are 430 by 250 pixels.
        /// </summary>
        public OptionsPage()
        {
            Size = new System.Drawing.Size(430, 250);
        }

        /*
         * Option pages can override this method to provide
         * validation for the page content. If the method returns
         * false the option form will not be closed.
         */

        /// <summary>
        /// Override to provides validation for the option page content.
        /// Called when the user click save to exit the options editor.
        /// Return true to accept the values, false to cancel and
        /// prevent the options editor from closing.
        /// </summary>
        /// <returns>True to accept and save the values, false to cancel.</returns>
        public virtual bool Validate()
        {
            return true;
        }

        /// <summary>
        /// Save the option page content. 
        /// All option pages must implement the Save method.
        /// </summary>
        public abstract void Save();
    }
}
