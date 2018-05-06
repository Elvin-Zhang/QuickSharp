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
using QuickSharp.Editor;

namespace QuickSharp.CodeAssist
{
    /// <summary>
    /// Defines the interface to a code assist provider.
    /// </summary>
    public interface ICodeAssistProvider
    {
        /// <summary>
        /// The document type for which assistance is provided.
        /// </summary>
        DocumentType DocumentType { get; }

        /// <summary>
        /// True if code assist is available.
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Called when the active document changes.
        /// </summary>
        /// <param name="document">The active document.</param>
        void DocumentActivated(ScintillaEditForm document);

        /// <summary>
        /// Get the data for the code assist pop-up window.
        /// </summary>
        /// <param name="document">The active document.</param>
        /// <returns>A list of lookup items.</returns>
        LookupList GetLookupList(ScintillaEditForm document);
    }
}
