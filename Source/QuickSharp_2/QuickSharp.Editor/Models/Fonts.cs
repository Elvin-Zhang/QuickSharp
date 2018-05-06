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
using System.Drawing;
using System.Drawing.Text;

namespace QuickSharp.Editor
{
    /// <summary>
    /// Manage the representation of fonts.
    /// </summary>
    public class Fonts
    {
        /// <summary>
        /// Get a list of installed fonts.
        /// </summary>
        /// <returns>A list of font names.</returns>
        public static List<string> GetFontList()
        {
            List<string> fonts = new List<string>();

            InstalledFontCollection installedFonts =
                new InstalledFontCollection();

            FontFamily[] families = installedFonts.Families;

            foreach (FontFamily family in families)
                fonts.Add(family.Name);

            return fonts;
        }

        /// <summary>
        /// Determine if a font is currently installed.
        /// </summary>
        /// <param name="name">The name of the font.</param>
        /// <returns>True if it is installed.</returns>
        public static bool FontIsAvailable(string name)
        {
            List<string> fonts = GetFontList();

            return fonts.Contains(name);
        }

        /// <summary>
        /// Get a valid font name. Returns the preferred font name if it
        /// is installed or the default (see Constants.DEFAULT_FONT_NAME).
        /// </summary>
        /// <param name="name">The name of the preferred font</param>
        /// <returns>The preferred font or the default if not available.</returns>
        public static string ValidateFontName(string name)
        {
            if (FontIsAvailable(name))
                return name;
            else
                return Constants.DEFAULT_FONT_NAME;
        }

        /// <summary>
        /// Get a valid font size. Returns the preferred font size if within acceptable limits or
        /// the default (10).
        /// </summary>
        /// <param name="size">The preferred font size.</param>
        /// <returns>The preferred size if within acceptable limits or the default.</returns>
        public static float ValidateFontSize(float size)
        {
            if (size < 5F || size > 30F)
                return 10F;
            else
                return size;
        }
    }
}
