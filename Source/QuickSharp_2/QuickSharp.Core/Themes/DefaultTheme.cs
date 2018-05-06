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

namespace QuickSharp.Core
{
    /// <summary>
    /// The default 'do nothing' built-in theme provider.
    /// </summary>
    public class DefaultTheme : IQuickSharpTheme
    {
        /// <summary>
        /// Get the provider ID. This must be unique and is
        /// usually a GUID.
        /// </summary>
        /// <returns>The provider ID.</returns>
        public string GetID()
        {
            return Constants.DEFAULT_THEME_ID;
        }

        /// <summary>
        /// Get the display name of the theme provider.
        /// </summary>
        /// <returns>The provider display name.</returns>
        public string GetName()
        {
            return Resources.ThemeDefault;
        }

        /// <summary>
        /// Get the provider key.
        /// </summary>
        /// <returns>The provider key.</returns>
        public string GetKey()
        {
            return null;
        }

        /// <summary>
        /// Applies the theme to the relevant UI elements.
        /// </summary>
        public void ApplyTheme()
        {
        }
    }
}
