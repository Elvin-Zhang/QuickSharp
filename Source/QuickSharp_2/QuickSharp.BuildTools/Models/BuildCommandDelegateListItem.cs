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

using QuickSharp.Core;

namespace QuickSharp.BuildTools
{
    /// <summary>
    /// Represents a build command provider as an item in a list. Associates
    /// the BuildCommand with a document type/build action combination.
    /// </summary>
    public class BuildCommandDelegateListItem
    {
        /// <summary>
        /// A document type.
        /// </summary>
        public DocumentType DocumentType { get; set; }

        /// <summary>
        /// A build action.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// The build command provider.
        /// </summary>
        public BuildCommandDelegate BuildCommand { get; set; }
    }
}
