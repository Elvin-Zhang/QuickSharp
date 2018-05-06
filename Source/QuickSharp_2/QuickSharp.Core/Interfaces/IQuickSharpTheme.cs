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
using QuickSharp.Core;

namespace QuickSharp.Core
{
    /// <summary>
    /// Defines the interface to a theme provider.
    /// </summary>
    public interface IQuickSharpTheme
    {
        /// <summary>
        /// Get the provider ID. This must be unique and is
        /// usually a GUID.
        /// </summary>
        /// <returns>The provider ID.</returns>
        string GetID();

        /// <summary>
        /// Get the display name of the theme provider.
        /// </summary>
        /// <returns>The provider display name.</returns>
        string GetName();

        /// <summary>
        /// Get the provider key. This is optional and a provider
        /// may return a null or empty key. The key is used to locate
        /// theme-specific locations (such as sub-folders) allowing the
        /// theme to provide it's own resources for various application
        /// features. Returning a null or empty key will cause the
        /// provider to be treated the same as for the default theme in
        /// cases where the key is used.
        /// </summary>
        /// <returns>The provider key.</returns>
        string GetKey();

        /// <summary>
        /// Applies the theme to the relevant UI elements.
        /// </summary>
        void ApplyTheme();
    }
}
