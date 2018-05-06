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

namespace QuickSharp.Editor
{
    /// <summary>
    /// A recorded macro event.
    /// </summary>
    [Serializable]
    public class MacroEvent
    {
        /// <summary>
        /// The recorded message.
        /// </summary>
        public int Message { get; set; }

        /// <summary>
        /// The message's wParam value.
        /// </summary>
        public IntPtr wParam { get; set; }

        /// <summary>
        /// The message's lParam value.
        /// </summary>
        public IntPtr lParam { get; set; }

        /// <summary>
        /// The text associated with the message.
        /// </summary>
        public String Text { get; set; }
    }
}
