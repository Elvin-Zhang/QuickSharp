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

namespace QuickSharp.Explorer
{
    /// <summary>
    /// Provides acces to the constants used in the plugin.
    /// </summary>
    public class Constants
    {
        public const string PLUGIN_NAME = "QuickSharp.Explorer";
        public const string DOCUMENT_IMAGE = "DOCUMENT";
        public const string CLOSED_FOLDER_IMAGE = "CLOSEDFOLDER";
        public const string OPENED_FOLDER_IMAGE = "OPENEDFOLDER";
        public const string PSEUDO_PATTERN_GLOBAL = "*";

        // Registry Keys
        public const string KEY_ROOT_FOLDER = "RootFolder";
        public const string KEY_VISITED_FOLDERS_LIST = "VisitedFoldersList";
        public const string KEY_VISITED_FOLDERS_LIMIT = "VisitedFoldersLimit";
        public const string KEY_FILE_FILTERS = "FileFilters";
        public const string KEY_SELECTED_FILE_FILTER = "SelectedFileFilter";
        public const string KEY_DEFAULT_FILTERS_APPLIED = "DefaultFiltersApplied";
        public const string KEY_APPLY_FILE_FILTER = "ApplyFileFilter";
        public const string KEY_BACKUP_DIRECTORY = "BackupDirectory";
        public const string KEY_SHOW_HIDDEN_FILES = "ShowHiddenFiles";
        public const string KEY_SHOW_SYSTEM_FILES = "ShowSystemFiles";
        public const string KEY_SHOW_FULL_PATH = "ShowFullPath";
        
        // UI Elements
        public const string DOCKED_FORM_KEY = "84EC9AE7-0630-441E-8264-0D7A8FC20499";
        public const string UI_OPTIONS_PAGE_EXPLORER = "UI_OPTIONS_PAGE_EXPLORER";
        public const string UI_OPTIONS_PAGE_FILTERS = "UI_OPTIONS_PAGE_FILTERS";
        public const string UI_TOOLBAR_REFRESH = "UI_TOOLBAR_REFRESH";
        public const string UI_TOOLBAR_MOVE_PARENT = "UI_TOOLBAR_MOVE_PARENT";
        public const string UI_TOOLBAR_MOVE_NEW = "UI_TOOLBAR_MOVE_NEW";
        public const string UI_TOOLBAR_SELECT_VISITED_FOLDER = "UI_TOOLBAR_SELECT_VISITED_FOLDER";
        public const string UI_TOOLBAR_SEP_1 = "UI_TOOLBAR_SEP_1";
        public const string UI_TOOLBAR_LAUNCH_SHELL = "UI_TOOLBAR_LAUNCH_SHELL";
        public const string UI_TOOLBAR_LAUNCH_EXPLORER = "UI_TOOLBAR_LAUNCH_EXPLORER";
        public const string UI_TOOLBAR_SEP_2 = "UI_TOOLBAR_SEP_2";
        public const string UI_TOOLBAR_BACKUP = "UI_TOOLBAR_BACKUP";
        public const string UI_TOOLBAR_SEP_3 = "UI_TOOLBAR_SEP_3";
        public const string UI_TOOLBAR_FILTER_FILES = "UI_TOOLBAR_FILTER_FILES";
        public const string UI_TOOLBAR_SELECT_FILTER = "UI_TOOLBAR_SELECT_FILTER";
        public const string UI_VIEW_MENU_EXPLORER = "UI_VIEW_MENU_EXPLORER";
        public const string UI_TREE_MENU_OPEN = "UI_TREE_MENU_OPEN";
        public const string UI_TREE_MENU_SEP_1 = "UI_TREE_MENU_SEP_1";
        public const string UI_TREE_MENU_RENAME = "UI_TREE_MENU_RENAME";
        public const string UI_TREE_MENU_CLONE = "UI_TREE_MENU_CLONE";
        public const string UI_TREE_MENU_DELETE = "UI_TREE_MENU_DELETE";
        public const string UI_TREE_MENU_SEP_2 = "UI_TREE_MENU_SEP_2";
        public const string UI_TREE_MENU_CREATE_FOLDER = "UI_TREE_MENU_CREATE_FOLDER";
        public const string UI_TREE_MENU_SEP_3 = "UI_TREE_MENU_SEP_3";
        public const string UI_TREE_MENU_SET_AS_CURRENT_DIR = "UI_TREE_MENU_SET_AS_CURRENT_DIR";
        public const string UI_MENU_FILTER_NEW = "UI_MENU_FILTER_NEW";
        public const string UI_MENU_FILTER_EDIT = "UI_MENU_FILTER_EDIT";
        public const string UI_MENU_FILTER_CLONE = "UI_MENU_FILTER_CLONE";
        public const string UI_MENU_FILTER_DELETE = "UI_MENU_FILTER_DELETE";
    }
}
