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

namespace QuickSharp.Output
{
    /// <summary>
    /// Provides access to the constants used in the plugin.
    /// </summary>
    public class Constants
    {
        public const string PLUGIN_NAME = "QuickSharp.Output";
        public const string BACKGROUND_IMAGE_FOLDER = "Themes";
        public const string BACKGROUND_IMAGE_PATTERN = "outputbackground*.bmp";
        public const string BACKGROUND_IMAGE_TILE = "tile";
        public const string BACKGROUND_IMAGE_DARK = "dark";
        public const string WINDOWS_COMSPEC = "COMSPEC";
        public const string WINDOWS_SHELL_PROCESSOR = "cmd.exe";
        public const string REGEX_GROUP_PATH = "path";
        public const string REGEX_GROUP_LINE = "line";

        // UI Elements
        public const string DOCKED_FORM_KEY = "8CA12548-12D5-4CE8-9E12-57E17B4EAB70";
        public const string ERROR_IMAGE = "ERROR";
        public const string UI_VIEW_MENU_OUTPUT = "UI_VIEW_MENU_OUTPUT";
    }
}
