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
using System.IO;
using QuickSharp.Core;

namespace QuickSharp.Workspace
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
        /// The current workspace directory.
        /// </summary>
        public string CurrentWorkspace { get; set; }

        /// <summary>
        /// The directory where workspace backup archives are saved.
        /// </summary>
        public string BackupDirectory { get; set; }

        /// <summary>
        /// Recently visited folders list.
        /// </summary>
        public List<String> VisitedFoldersList { get; set; }

        /// <summary>
        /// Maximum number of recently visited folders maintained.
        /// </summary>
        public int VisitedFoldersLimit { get; private set; }

        /// <summary>
        /// Show source files only.
        /// </summary>
        public bool ShowSourceOnly { get; set; }

        /// <summary>
        ///  Show hidden files in the workspace.
        /// </summary>
        public bool ShowHiddenFiles { get; set; }

        /// <summary>
        /// Show system files in the workspace.
        /// </summary>
        public bool ShowSystemFiles { get; set; }

        /// <summary>
        /// Show full path when show in title bar.
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
            _persistenceManager.WriteString(
                Constants.KEY_CURRENT_WORKSPACE, CurrentWorkspace);
            _persistenceManager.WriteString(
                Constants.KEY_BACKUP_DIRECTORY, BackupDirectory);
            _persistenceManager.WriteStrings(
                Constants.KEY_VISITED_FOLDERS_LIST, VisitedFoldersList);
            _persistenceManager.WriteInt(
                Constants.KEY_VISITED_FOLDERS_LIMIT, VisitedFoldersLimit);
            _persistenceManager.WriteBoolean(
                Constants.KEY_SHOW_SOURCE_ONLY, ShowSourceOnly);
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
            CurrentWorkspace = _persistenceManager.ReadString(
                Constants.KEY_CURRENT_WORKSPACE, Directory.GetCurrentDirectory());
            BackupDirectory = _persistenceManager.ReadString(
                Constants.KEY_BACKUP_DIRECTORY, String.Empty);
            VisitedFoldersList = _persistenceManager.ReadStrings(
                Constants.KEY_VISITED_FOLDERS_LIST);
            VisitedFoldersLimit = _persistenceManager.ReadInt(
                Constants.KEY_VISITED_FOLDERS_LIMIT, 10);
            ShowSourceOnly = _persistenceManager.ReadBoolean(
                Constants.KEY_SHOW_SOURCE_ONLY, false);
            ShowHiddenFiles = _persistenceManager.ReadBoolean(
                Constants.KEY_SHOW_HIDDEN_FILES, false);
            ShowSystemFiles = _persistenceManager.ReadBoolean(
                Constants.KEY_SHOW_SYSTEM_FILES, false);
            ShowFullPath = _persistenceManager.ReadBoolean(
                Constants.KEY_SHOW_FULL_PATH, false);

            // Trim the list if too big.
            while (VisitedFoldersList.Count > VisitedFoldersLimit)
                VisitedFoldersList.RemoveAt(0);
        }

        /// <summary>
        /// Request a UI update.
        /// </summary>
        public void UpdateUI()
        {
            if (OnUpdateUI != null) OnUpdateUI();
        }
    }
}
