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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using QuickSharp.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.Core
{
    /// <summary>
    /// The Visual Studio 2010 basic theme provider.
    /// </summary>
    public class VS2010ThemeBasic : VS2010Theme, IQuickSharpTheme
    {
        /// <summary>
        /// Get the provider ID. This must be unique and is
        /// usually a GUID.
        /// </summary>
        /// <returns>The provider ID.</returns>
        public string GetID()
        {
            return Constants.VS2010_BASIC_THEME_ID;
        }

        /// <summary>
        /// Get the display name of the theme provider.
        /// </summary>
        /// <returns>The provider display name.</returns>
        public string GetName()
        {
            return Resources.ThemeVS2010Basic;
        }

        /// <summary>
        /// Get the provider key.
        /// </summary>
        /// <returns>The provider key.</returns>
        public string GetKey()
        {
            return null;
        }
    }
}
