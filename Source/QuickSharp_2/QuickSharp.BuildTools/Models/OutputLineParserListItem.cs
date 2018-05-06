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
using QuickSharp.Output;

namespace QuickSharp.BuildTools
{
    /// <summary>
    /// Represents an OutputLineParser as an item in a list.
    /// </summary>
    public class OutputLineParserListItem
    {
        /// <summary>
        /// The parser's display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The parser's document type.
        /// </summary>
        public DocumentType DocumentType { get; set; }

        /// <summary>
        /// The parser's build action.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// The parser instance.
        /// </summary>
        public IOutputLineParser Parser { get; set; }
    }
}
