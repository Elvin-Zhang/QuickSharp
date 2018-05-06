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

namespace QuickSharp.Workspace
{
    /// <summary>
    /// Provides access to constants used in the plugin.
    /// </summary>
    public class Constants
    {
        public const string PLUGIN_NAME = "QuickSharp.Workspace";
        public const string BACKGROUND_IMAGE_FOLDER = "Themes";
        public const string BACKGROUND_IMAGE_PATTERN = "workspacebackground*.bmp";
        public const string BACKGROUND_IMAGE_TILE = "tile";
        public const string BACKGROUND_IMAGE_DARK = "dark";

        // Registry Keys
        public const string KEY_CURRENT_WORKSPACE = "Workspace";
        public const string KEY_BACKUP_DIRECTORY = "WorkspaceBackupDirectory";
        public const string KEY_SHOW_SOURCE_ONLY = "ShowSourceOnly";
        public const string KEY_VISITED_FOLDERS_LIST = "VisitedFoldersList";
        public const string KEY_VISITED_FOLDERS_LIMIT = "VisitedFoldersLimit";
        public const string KEY_SHOW_HIDDEN_FILES = "ShowHiddenFiles";
        public const string KEY_SHOW_SYSTEM_FILES = "ShowSystemFiles";
        public const string KEY_SHOW_FULL_PATH = "ShowFullPath";

        // AppStore Keys
        public const string APP_STORE_KEY_TOOL_SOURCE_TYPES = "APP_STORE_KEY_TOOL_SOURCE_TYPES";
        public const string APP_STORE_KEY_NON_TOOL_SOURCE_TYPES = "APP_STORE_KEY_NON_TOOL_SOURCE_TYPES";

        // UI Elements
        public const string DOCKED_FORM_KEY = "C8E86F1B-3A02-4FBD-9F7E-AEC4AD338558";
        public const string DOCUMENT_IMAGE = "DOCUMENT";
        public const string FOLDER_IMAGE = "FOLDER";
        public const string UI_OPTIONS_PAGE_WORKSPACE = "UI_OPTIONS_PAGE_WORKSPACE";
        public const string UI_VIEW_MENU_WORKSPACE = "UI_VIEW_MENU_WORKSPACE";
        public const string UI_FILE_VIEW_MENU_OPEN = "UI_FILE_VIEW_MENU_OPEN";
        public const string UI_FILE_VIEW_MENU_SEP_1 = "UI_FILE_VIEW_MENU_SEP_1";
        public const string UI_FILE_VIEW_MENU_RENAME = "UI_FILE_VIEW_MENU_RENAME";
        public const string UI_FILE_VIEW_MENU_CLONE = "UI_FILE_VIEW_MENU_CLONE";
        public const string UI_FILE_VIEW_MENU_DELETE = "UI_FILE_VIEW_MENU_DELETE";
        public const string UI_FILE_VIEW_MENU_SEP_2 = "UI_FILE_VIEW_MENU_SEP_2";
        public const string UI_FILE_VIEW_MENU_CREATE_FOLDER = "UI_FILE_VIEW_MENU_CREATE_FOLDER";
        public const string UI_FILE_VIEW_MENU_SEP_3 = "UI_FILE_VIEW_MENU_SEP_3";
        public const string UI_FILE_VIEW_MENU_MOVE_TO_PARENT = "UI_FILE_VIEW_MENU_MOVE_TO_PARENT";
        public const string UI_FILE_VIEW_MENU_SEP_4 = "UI_FILE_VIEW_MENU_SEP_4";
        public const string UI_FILE_VIEW_MENU_SELECT_SIMILAR = "UI_FILE_VIEW_MENU_SELECT_SIMILAR";
        public const string UI_FILE_VIEW_MENU_INVERT_SELECTION = "UI_FILE_VIEW_MENU_INVERT_SELECTION";
        public const string UI_TOOLBAR_LAUNCH_SHELL = "UI_TOOLBAR_LAUNCH_SHELL";
        public const string UI_TOOLBAR_LAUNCH_EXPLORER = "UI_TOOLBAR_LAUNCH_EXPLORER";
        public const string UI_TOOLBAR_REFRESH = "UI_TOOLBAR_REFRESH";
        public const string UI_TOOLBAR_MOVE_TO_PARENT = "UI_TOOLBAR_MOVE_TO_PARENT";
        public const string UI_TOOLBAR_LOCATE_NEW_FOLDER = "UI_TOOLBAR_LOCATE_NEW_FOLDER";
        public const string UI_TOOLBAR_SELECT_VISITED_FOLDER = "UI_TOOLBAR_SELECT_VISITED_FOLDER";
        public const string UI_TOOLBAR_CREATE_FOLDER = "UI_TOOLBAR_CREATE_FOLDER";
        public const string UI_TOOLBAR_BACKUP_WORKSPACE = "UI_TOOLBAR_BACKUP_WORKSPACE";
        public const string UI_TOOLBAR_SHOW_SOURCE_ONLY = "UI_TOOLBAR_SHOW_SOURCE_ONLY";
        public const string UI_TOOLBAR_SEP_1 = "UI_TOOLBAR_SEP_1";
        public const string UI_TOOLBAR_SEP_2 = "UI_TOOLBAR_SEP_2";
        public const string UI_TOOLBAR_SEP_3 = "UI_TOOLBAR_SEP_3";
    }
}
