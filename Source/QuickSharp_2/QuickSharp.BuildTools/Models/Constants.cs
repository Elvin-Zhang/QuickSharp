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

namespace QuickSharp.BuildTools
{
    /// <summary>
    /// Provides access to constants used within the plugin.
    /// </summary>
    public class Constants
    {
        public const string PLUGIN_NAME = "QuickSharp.BuildTools";
        public const string SERIALIZATION_DELIMITER = "¬";

        public const string ACTION_CMD_RUN_IN_OWN_WINDOW = "RunInOwnWindow";
        public const string ACTION_CMD_DO_PRE_COMPILE = "DoPreCompile";
        public const string ACTION_CMD_DO_POST_COMPILE = "DoPostCompile";
        public const string ACTION_CMD_DO_PRE_RUN = "DoPreRun";
        public const string ACTION_CMD_DO_POST_RUN = "DoPostRun";
        public const string ACTION_COMPILE = "COMPILE";
        public const string ACTION_RUN = "RUN";

        // Registry Keys
        public const string KEY_TOOLS_COLLECTION = "BuildTools";
        public const string KEY_SELECTED_TOOLS_COLLECTION = "SelectedBuildTools";
        public const string KEY_PINNED_FILES = "PinnedFiles";
        public const string KEY_ALWAYS_BUILD_ON_COMPILE = "AlwaysBuildOnCompile";
        public const string KEY_ALWAYS_BUILD_ON_RUN = "AlwaysBuildOnRun";

        // AppStore Keys
        public const string APP_STORE_KEY_TOOL_SOURCE_TYPES = "APP_STORE_KEY_TOOL_SOURCE_TYPES";
        public const string APP_STORE_KEY_NON_TOOL_SOURCE_TYPES = "APP_STORE_KEY_NON_TOOL_SOURCE_TYPES";

        // UI Elements
        public const string UI_TOOLS_MENU_COMPILE = "UI_TOOLS_MENU_COMPILE";
        public const string UI_TOOLS_MENU_COMPILE_ALL = "UI_TOOLS_MENU_COMPILE_ALL";
        public const string UI_TOOLS_MENU_RUN = "UI_TOOLS_MENU_RUN";
        public const string UI_OPTIONS_BUILD_SETTINGS_PAGE = "UI_OPTIONS_BUILD_SETTINGS_PAGE";
        public const string UI_OPTIONS_BUILD_TOOLS_PAGE = "UI_OPTIONS_BUILD_TOOLS_PAGE";
        public const string UI_MENU_TOOL_NEW = "UI_MENU_TOOL_NEW";
        public const string UI_MENU_TOOL_EDIT = "UI_MENU_TOOL_EDIT";
        public const string UI_MENU_TOOL_CLONE = "UI_MENU_TOOL_CLONE";
        public const string UI_MENU_TOOL_DELETE = "UI_MENU_TOOL_DELETE";
        public const string UI_WORKSPACE_SHOW_SOURCE_ONLY = "UI_WORKSPACE_SHOW_SOURCE_ONLY";
        public const string UI_TOOLBAR_COMPILE = "UI_TOOLBAR_COMPILE";
        public const string UI_TOOLBAR_COMPILE_SELECT = "UI_TOOLBAR_COMPILE_SELECT";
        public const string UI_TOOLBAR_RUN = "UI_TOOLBAR_RUN";
        public const string UI_TOOLBAR_RUN_SELECT = "UI_TOOLBAR_RUN_SELECT";
        public const string UI_TOOLBAR_PIN = "UI_TOOLBAR_PIN";
        public const string UI_TOOLBAR_PIN_SELECT = "UI_TOOLBAR_PIN_SELECT";
    }
}
