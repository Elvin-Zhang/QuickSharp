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

namespace QuickSharp.Core
{
    /// <summary>
    /// Provides a string to string mapping.
    /// </summary>
    public class StringMap
    {
        /// <summary>
        /// The key.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Create a new StringMap.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value;
        }
    }

    /// <summary>
    /// Compares two StringMap objects by Name.
    /// </summary>
    public class StringMapNameComparer : IComparer<StringMap>
    {
        /// <summary>
        /// Compare the StringMaps.
        /// </summary>
        /// <param name="m1">A StringMap.</param>
        /// <param name="m2">A StringMap.</param>
        /// <returns>0 if they are equal.</returns>
        public int Compare(StringMap m1, StringMap m2)
        {
            return m1.Name.CompareTo(m2.Name);
        }
    }
}
