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
using System.IO;
using System.Collections.Generic;
using QuickSharp.Core;

namespace QuickSharp.Explorer
{
    public class SettingsManager
    {
        #region Singleton

        private static SettingsManager _singleton;

        public static SettingsManager GetInstance()
        {
            if (_singleton == null)
                _singleton = new SettingsManager();

            return _singleton;
        }

        #endregion

        private IPersistenceManager _persistenceManager;

        /// <summary>
        /// The explorer view root folder.
        /// </summary>
        public string RootFolder { get; set; }

        /// <summary>
        /// The file type filters.
        /// </summary>
        public Dictionary<string, FileFilter> FileFilters { get; set; }

        /// <summary>
        /// The currently selected file type filter.
        /// </summary>
        public string SelectedFilter { get; set; }

        /// <summary>
        /// True if the apply filter button is selected. Used to
        /// save state between sessions.
        /// </summary>
        public bool ApplyFilter { get; set; }

        /// <summary>
        /// A list of folders selected by the move to folder dialog.
        /// </summary>
        public List<string> VisitedFoldersList { get; set; }

        /// <summary>
        /// The maximum number of visited folders saved in the list.
        /// </summary>
        public int VisitedFoldersLimit { get; private set; }

        /// <summary>
        /// The directory where explorer backup archives are saved.
        /// </summary>
        public string BackupDirectory { get; set; }
        
        /// <summary>
        /// Show hidden files in the Explorer tree view.
        /// </summary>
        public bool ShowHiddenFiles { get; set; }

        /// <summary>
        /// Show system files in the Explorer tree view.
        /// </summary>
        public bool ShowSystemFiles { get; set; }

        /// <summary>
        /// Show the full path when shown in title bar.
        /// </summary>
        public bool ShowFullPath { get; set; }

        /// <summary>
        /// Raised to request a file filters UI update.
        /// </summary>
        public event MessageHandler OnUpdateUI;

        private SettingsManager()
        {
            _persistenceManager = ApplicationManager.GetInstance().
                GetPersistenceManager(Constants.PLUGIN_NAME);

            Update();
        }

        /// <summary>
        /// Save the settings to the session persistence store (e.g. registry).
        /// </summary>
        public void Save()
        {
            List<string> filterList = FileFilter.GetFilterList(FileFilters);

            _persistenceManager.WriteString(
                Constants.KEY_ROOT_FOLDER, RootFolder);
            _persistenceManager.WriteStrings(
                Constants.KEY_FILE_FILTERS, filterList);
            _persistenceManager.WriteString(
                Constants.KEY_SELECTED_FILE_FILTER, SelectedFilter);
            _persistenceManager.WriteBoolean(
                Constants.KEY_APPLY_FILE_FILTER, ApplyFilter);
            _persistenceManager.WriteStrings(
                Constants.KEY_VISITED_FOLDERS_LIST, VisitedFoldersList);
            _persistenceManager.WriteString(
                Constants.KEY_BACKUP_DIRECTORY, BackupDirectory);
            _persistenceManager.WriteBoolean(
                Constants.KEY_SHOW_HIDDEN_FILES, ShowHiddenFiles);
            _persistenceManager.WriteBoolean(
                Constants.KEY_SHOW_SYSTEM_FILES, ShowSystemFiles);
            _persistenceManager.WriteBoolean(
                Constants.KEY_SHOW_FULL_PATH, ShowFullPath);
        }

        /// <summary>
        /// Retrieve the settings from the session persistence store (e.g. registry).
        /// </summary>
        public void Update()
        {
            RootFolder = _persistenceManager.ReadString(
                Constants.KEY_ROOT_FOLDER, Directory.GetCurrentDirectory());

            bool defaultFiltersApplied = _persistenceManager.ReadBoolean(
                Constants.KEY_DEFAULT_FILTERS_APPLIED, false);

            List<string> filterList = _persistenceManager.ReadStrings(
                Constants.KEY_FILE_FILTERS);

            SelectedFilter = _persistenceManager.ReadString(
                Constants.KEY_SELECTED_FILE_FILTER, String.Empty);

            if (filterList.Count == 0 && !defaultFiltersApplied)
            {
                filterList.Add("55F01123-747C-423B-BDAB-3ED92B6A466D|Hide All Files|");
                filterList.Add("692BB91D-FA37-4826-9483-E4CD53AF9DAB|Web Files|(?i:\\.html$) (?i:\\.css$) (?i:\\.js$)");

                SelectedFilter = String.Empty;
                
                _persistenceManager.WriteBoolean(
                    Constants.KEY_DEFAULT_FILTERS_APPLIED, true);
            }

            FileFilters = FileFilter.GetFileFilters(filterList);

            ApplyFilter = _persistenceManager.ReadBoolean(
                Constants.KEY_APPLY_FILE_FILTER, false);

            if (SelectedFilter == String.Empty)
                ApplyFilter = false;

            VisitedFoldersList = _persistenceManager.ReadStrings(
                Constants.KEY_VISITED_FOLDERS_LIST);
            VisitedFoldersLimit = _persistenceManager.ReadInt(
                Constants.KEY_VISITED_FOLDERS_LIMIT, 10);

            // Trim the list if too big.
            while (VisitedFoldersList.Count > VisitedFoldersLimit)
                VisitedFoldersList.RemoveAt(0);

            BackupDirectory = _persistenceManager.ReadString(
                Constants.KEY_BACKUP_DIRECTORY, String.Empty);
            ShowHiddenFiles = _persistenceManager.ReadBoolean(
                Constants.KEY_SHOW_HIDDEN_FILES, false);
            ShowSystemFiles = _persistenceManager.ReadBoolean(
                Constants.KEY_SHOW_SYSTEM_FILES, false);
            ShowFullPath = _persistenceManager.ReadBoolean(
                Constants.KEY_SHOW_FULL_PATH, false);
        }

        /// <summary>
        /// Request a UI update.
        /// </summary>
        public void UpdateUI()
        {
            if (OnUpdateUI != null) OnUpdateUI();
        }

        #region Visited Folders List

        /// <summary>
        /// Add a folder to the recently visited folders list.
        /// </summary>
        /// <param name="path">The path of the folder.</param>
        public void AddFolderToVisitedList(string path)
        {
            if (VisitedFoldersList.Contains(path))
                VisitedFoldersList.Remove(path);

            VisitedFoldersList.Add(path);

            if (VisitedFoldersList.Count > VisitedFoldersLimit)
                VisitedFoldersList.RemoveAt(0);
        }

        /// <summary>
        /// Remove a folder from the recently visited folders list.
        /// </summary>
        /// <param name="path">The path of the folder.</param>
        public void RemoveFolderFromVisitedList(string path)
        {
            if (VisitedFoldersLimit > 0 &&
                VisitedFoldersList.Contains(path))
                VisitedFoldersList.Remove(path);
        }

        #endregion
    }
}
