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
using System.Windows.Forms;
using System.Reflection;
using QuickSharp.Core;

namespace QuickSharp.CodeAssist.DotNet
{
    public partial class ManageReferenceDatabaseForm : Form
    {
        private ReferenceManager _referenceManager;
        private ReferenceDatabase _activeNamespaces;
        private ReferenceDatabase _inactiveNamespaces;

        #region Properties

        public ReferenceDatabase ActiveNamespaces
        {
            get { return _activeNamespaces; }
        }

        public ReferenceDatabase InactiveNamespaces
        {
            get { return _inactiveNamespaces; }
        }

        #endregion

        public ManageReferenceDatabaseForm(
            ReferenceDatabase activeNamespaces,
            ReferenceDatabase inactiveNamespaces)
        {
            InitializeComponent();

            _referenceManager = ReferenceManager.GetInstance();
            _activeNamespaces = activeNamespaces;
            _inactiveNamespaces = inactiveNamespaces;

            /*
             * Populate the list box with the database contents.
             */

            foreach (ReferenceNamespace ns in
                _activeNamespaces.Values)
                _checkedListBox.Items.Add(ns, true);

            foreach (ReferenceNamespace ns in
                _inactiveNamespaces.Values)
                _checkedListBox.Items.Add(ns, false);

            /*
             * Allow client apps to modify the form.
             */

            ManageReferenceDatabaseFormProxy.GetInstance().
                UpdateFormControls(Controls);
        }

        #region Form Event Handlers

        private void AddFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.RestoreDirectory = true;
            ofd.Title = Resources.AddFileDialogTitle;
            ofd.Filter = Resources.AddFileDialogFilter;

            if (ofd.ShowDialog() == DialogResult.OK)
                AddNamespacesFromFile(ofd.FileName);
        }

        private void AddFolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = Resources.AddFolderDialogText;
            fbd.ShowNewFolderButton = false;

            if (fbd.ShowDialog() == DialogResult.OK)
                AddNamespacesFromFolder(fbd.SelectedPath);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            /*
             * Recreate the database from the list.
             */

            _activeNamespaces = new ReferenceDatabase();
            _inactiveNamespaces = new ReferenceDatabase();

            foreach (ReferenceNamespace ns in _checkedListBox.Items)
            {
                if (_checkedListBox.CheckedItems.Contains(ns))
                    _activeNamespaces[ns.Name] = ns;
                else
                    _inactiveNamespaces[ns.Name] = ns;
            }
        }

        #endregion

        #region Add File/Folder

        private void AddNamespacesFromFile(string path)
        {
            path = "@" + path;

            Assembly assembly = CodeAssistTools.LoadAssembly(path);
            if (assembly == null) return;

            try
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsPublic) continue;

                    string ns = type.Namespace;
                    if (String.IsNullOrEmpty(ns)) continue;

                    ReferenceNamespace item = FindListBoxItem(ns);

                    if (item == null)
                    {
                        item = new ReferenceNamespace(ns);
                        _checkedListBox.Items.Add(item, true);
                    }
                    
                    item.AddAssembly(path);
                }
            }
            catch
            {
                // Ignore unloadable assemblies
            }
        }

        private void AddNamespacesFromFolder(string path)
        {
            foreach (string file in Directory.GetFiles(path, "*.dll"))
                AddNamespacesFromFile(file);
        }

        private ReferenceNamespace FindListBoxItem(string name)
        {
            foreach (ReferenceNamespace ns in _checkedListBox.Items)
                if (ns.Name == name)
                    return ns;

            return null;
        }

        #endregion
    }
}
