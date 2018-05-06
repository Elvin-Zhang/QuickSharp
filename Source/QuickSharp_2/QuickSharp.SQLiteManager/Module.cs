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
using System.Windows.Forms;
using QuickSharp.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.SQLiteManager
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "EDE0B4E0-0373-4345-B142-75D060C683EC";
        }

        public string GetName()
        {
            return "QuickSharp SQLite Manager";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return
                "Provides a document handler for SQLite databases allowing " +
                "them to be opened with a database management utility.";
        }

        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            return deps;
        }

        public void Activate(MainForm mainForm)
        {
            ActivatePlugin();
        }

        #endregion

        private IPersistenceManager _persistenceManager;

        public void ActivatePlugin()
        {
            ApplicationManager applicationManager =
                ApplicationManager.GetInstance();

            /*
             * Get the path to the SQLite utility. The default value is
             * blank to signal that we need to create the default value
             * from the QuickSharp installation path.
             */

            _persistenceManager = applicationManager.
                GetPersistenceManager(Constants.PLUGIN_NAME);

            string sqliteManagerPath = _persistenceManager.ReadString(
                Constants.KEY_SQLITE_MANAGER_PATH, String.Empty);

            if (sqliteManagerPath == String.Empty)
            {
                sqliteManagerPath = Path.Combine(
                    applicationManager.QuickSharpHome,
                    Constants.SQLITE_MANAGER_EXE);

                _persistenceManager.WriteString(
                    Constants.KEY_SQLITE_MANAGER_PATH, sqliteManagerPath);
            }

            /*
             * Register the document open handler.
             */

            applicationManager.RegisterOpenDocumentHandler(
                Constants.DOCUMENT_TYPE_SQLITE, OpenSQLiteDocument);

            /*
             * Register the options page.
             */

            applicationManager.RegisterOptionsPageFactory(
                delegate { return new SQLiteManagerOptionsPage(); });
        }

        private IDockContent OpenSQLiteDocument(string path, bool readOnly)
        {
            string sqliteManagerPath = _persistenceManager.ReadString(
                Constants.KEY_SQLITE_MANAGER_PATH, String.Empty);

            if (File.Exists(sqliteManagerPath))
                FileTools.LaunchApplication(true,
                    sqliteManagerPath, String.Format("\"{0}\"", path));

            /*
             * No document is created so return null.
             */

            return null;
        }
    }
}
