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
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using QuickSharp.Core;

namespace QuickSharp.CodeAssist.DotNet
{
    public class ReferenceManager
    {
        #region Singleton

        private static ReferenceManager _singleton;

        public static ReferenceManager GetInstance()
        {
            if (_singleton == null)
                _singleton = new ReferenceManager();

            return _singleton;
        }

        #endregion

        private ApplicationManager _applicationManager;
        private string _referenceDatabasePath;
        private List<string> _fullNamespaceList;
        private List<string> _rootNamespaceList;

        public ReferenceDatabase ActiveNamespaces { get; set; }
        public ReferenceDatabase InactiveNamespaces { get; set; }

        private ReferenceManager()
        {
            _applicationManager = ApplicationManager.GetInstance();

            IPersistenceManager persistenceManager = _applicationManager.
                GetPersistenceManager(Constants.PLUGIN_NAME);

            string databaseFilename = persistenceManager.ReadString(
                Constants.KEY_DATABASE_FILENAME,
                Constants.DEFAULT_DATABASE_FILENAME);

            if (FileTools.FilenameIsInvalid(databaseFilename))
                databaseFilename = Constants.DEFAULT_DATABASE_FILENAME;

            _referenceDatabasePath = Path.Combine(
                ApplicationManager.GetInstance().QuickSharpUserHome,
                databaseFilename);

            _fullNamespaceList = new List<string>();
            _rootNamespaceList = new List<string>();
        }

        public void CreateReferenceDatabaseFile()
        {
            if (!File.Exists(_referenceDatabasePath))
            {
                /*
                 * If auto-populate is enabled create the file
                 * and get the reference assemblies. If not just
                 * create an empty file.
                 */

                if (_applicationManager.ClientProfile.
                    HaveFlag(ClientFlags.CodeAssistDotNetAutoPopulateDatabase))
                {
                    CreateReferenceDatabaseForm form =
                        new CreateReferenceDatabaseForm();

                    form.ShowDialog();

                    form.WriteReferenceDatabase();
                }
                else
                {
                    FileTools.CreateFile(_referenceDatabasePath);
                }
            }

            LoadReferenceDatabaseFromFile();
        }

        #region Properties

        public string ReferenceDatabasePath
        {
            get { return _referenceDatabasePath; }
        }

        public List<string> FullNamespaceList
        {
            get { return _fullNamespaceList; }
        }

        public List<string> RootNamespaceList
        {
            get { return _rootNamespaceList; }
        }

        #endregion

        #region Helpers

        public List<string> GetNamespaces()
        {
            return ActiveNamespaces.Keys.ToList<string>();
        }

        public List<string> GetNamespaceAssemblies(string ns)
        {
            if (ActiveNamespaces.ContainsKey(ns))
                return ActiveNamespaces[ns].AssemblyList;
            else
                return new List<string>();
        }

        public List<string> GetAllAssemblies()
        {
            List<string> assemblies = new List<string>();

            foreach (ReferenceNamespace ns in ActiveNamespaces.Values)
            {
                foreach (string assembly in ns.AssemblyList)
                    if (!assemblies.Contains(assembly))
                        assemblies.Add(assembly);
            }

            return assemblies;
        }

        #endregion

        #region Load reference database from file

        internal void LoadReferenceDatabaseFromFile()
        {
            if (!File.Exists(_referenceDatabasePath)) return;

            ActiveNamespaces = new ReferenceDatabase();
            InactiveNamespaces = new ReferenceDatabase();

            StreamReader sr = null;

            try
            {
                sr = new StreamReader(_referenceDatabasePath);

                string line = null;
                string ns = null;
                bool readingActive = true;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line == String.Empty) continue;

                    if (line[0] == '+')
                    {
                        ns = line.Substring(1);
                        readingActive = true;
                        ActiveNamespaces[ns] = new ReferenceNamespace(ns);
                    }
                    else if (line[0] == '-')
                    {
                        ns = line.Substring(1);
                        readingActive = false;
                        InactiveNamespaces[ns] = new ReferenceNamespace(ns);
                    }
                    else
                    {
                        if (readingActive)
                            ActiveNamespaces[ns].AddAssembly(line);
                        else
                            InactiveNamespaces[ns].AddAssembly(line);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}:\r\n{1}",
                        Resources.ReadDbErrorMessage,
                        ex.Message),
                    Resources.ReadDbErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                if (sr != null) sr.Close();
            }

            /*
             * Update the namespace lists (will be
             * empty if file read fails).
             */

            _fullNamespaceList.Clear();
            _rootNamespaceList.Clear();

            foreach (string ns in ActiveNamespaces.Keys)
            {
                _fullNamespaceList.Add(ns);

                string[] split = ns.Split('.');

                if (String.IsNullOrEmpty(split[0])) continue;

                if (!_rootNamespaceList.Contains(split[0]))
                    _rootNamespaceList.Add(split[0]);
            }
        }

        #endregion
    }
}
