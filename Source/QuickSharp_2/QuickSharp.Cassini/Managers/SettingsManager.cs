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

namespace QuickSharp.Cassini
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

        private ApplicationManager _applicationManager;
        private IPersistenceManager _persistenceManager;

        public string ServerPath { get; set; }
        public string ServerPort { get; set; }
        public string ServerRoot { get; set; }

        private SettingsManager()
        {
            _applicationManager = ApplicationManager.GetInstance();

            _persistenceManager = 
                _applicationManager.GetPersistenceManager(Constants.PLUGIN_NAME);

            Update();
        }

        public void Save()
        {
            _persistenceManager.WriteString(Constants.KEY_WEB_SERVER_PATH, ServerPath);
            _persistenceManager.WriteString(Constants.KEY_WEB_SERVER_PORT, ServerPort);
            _persistenceManager.WriteString(Constants.KEY_WEB_SERVER_ROOT, ServerRoot);
        }

        public void Update()
        {
            /*
             * The default server path is not required: the plugin
             * will populate the key with the installed path on
             * first-time startup. First time use is determined by
             * the String.Empty value.
             */

            ServerPath = _persistenceManager.ReadString(
                Constants.KEY_WEB_SERVER_PATH, String.Empty);

            ServerPort = _persistenceManager.ReadString(
                Constants.KEY_WEB_SERVER_PORT,
                "8080");

            ServerRoot = _persistenceManager.ReadString(
                Constants.KEY_WEB_SERVER_ROOT,
                "/");
        }
    }
}
