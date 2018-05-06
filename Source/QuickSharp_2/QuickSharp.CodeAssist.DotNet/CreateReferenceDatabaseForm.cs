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
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using QuickSharp.Core;

namespace QuickSharp.CodeAssist.DotNet
{
    public partial class CreateReferenceDatabaseForm : Form
    {
        private ReferenceManager _referenceManager;
        private ReferenceDatabase _referenceDatabase;

        public ReferenceDatabase ReferenceDatabase
        {
            get { return _referenceDatabase; }
        }

        public CreateReferenceDatabaseForm()
        {
            InitializeComponent();

            Text = ApplicationManager.GetInstance().
                ClientProfile.ClientTitle; 

            Shown += new EventHandler(
                CreateReferenceDatabaseForm_Shown);

            _referenceManager = ReferenceManager.GetInstance();
        }

        #region Create Reference Database

        private void CreateReferenceDatabaseForm_Shown(
            object sender, EventArgs e)
        {
            Refresh();

            CreateReferenceDatabase();

            Close();
        }

        /*
         * Populate the database property with the namespaces
         * from the Global Assembly Cache.
         */

        private void CreateReferenceDatabase()
        {
            List<string> assemblyList = new List<string>();

            string gac = Path.Combine(
                Environment.SystemDirectory, @"..\assembly\GAC");
            string gac32 = Path.Combine(
                Environment.SystemDirectory, @"..\assembly\GAC_32");
            string gac64 = Path.Combine(
                Environment.SystemDirectory, @"..\assembly\GAC_64");
            string gacMSIL = Path.Combine(
                Environment.SystemDirectory, @"..\assembly\GAC_MSIL");

            assemblyList.AddRange(GetGACAssemblies(gac));
            assemblyList.AddRange(GetGACAssemblies(gac32));
            assemblyList.AddRange(GetGACAssemblies(gac64));
            assemblyList.AddRange(GetGACAssemblies(gacMSIL));

            _referenceDatabase = new ReferenceDatabase();

            _progressBar.Maximum = assemblyList.Count;
            _progressBar.Step = 1;

            foreach (string name in assemblyList)
            {
                _progressBar.PerformStep();

                Assembly assembly = CodeAssistTools.LoadAssembly(name);
                if (assembly == null) continue;

                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (!type.IsPublic) continue;

                        string ns = type.Namespace;
                        if (String.IsNullOrEmpty(ns)) continue;

                        if (!_referenceDatabase.ContainsKey(ns))
                            _referenceDatabase[ns] = new ReferenceNamespace(ns);

                        _referenceDatabase[ns].AddAssembly(name);
                    }
                }
                catch
                {
                    // Ignore unloadable assemblies
                }

                Refresh();
            }
        }

        /*
         * Writing the database is a separate operation to allow the
         * created database to be retrieved via its property and used
         * elsewhere if writing is not required.
         */

        public void WriteReferenceDatabase()
        {
            StreamWriter sw = null;
            String path = _referenceManager.ReferenceDatabasePath;

            try
            {
                sw = new StreamWriter(path);

                foreach (string key in _referenceDatabase.Keys)
                {
                    /*
                     * Start with just System namespaces active.
                     */

                    if (key.StartsWith("System"))
                        sw.WriteLine("+" + key);
                    else
                        sw.WriteLine("-" + key);

                    foreach (string assmembly in 
                        _referenceDatabase[key].AssemblyList)
                        sw.WriteLine(assmembly);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}:\r\n{1}",
                        Resources.CreateDbErrorMessage,
                        ex.Message),
                    Resources.CreateDbErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                if (sw != null) sw.Close();
            }
        }

        #endregion

        #region Helpers

        private List<string> GetGACAssemblies(string root)
        {
            List<string> list = new List<string>();

            if (!Directory.Exists(root)) return list;

            string[] paths = Directory.GetDirectories(root);

            foreach (string path in paths)
            {
                string name = Path.GetFileName(path);
                if (String.IsNullOrEmpty(name)) continue;

                string[] sigs = Directory.GetDirectories(path);
                if (sigs.Length < 1) continue;

                string sig = Path.GetFileName(sigs[0]);
                sig = sig.Replace("__", ", Culture=neutral, PublicKeyToken=");

                list.Add(String.Format("{0}, Version={1}", name, sig));
            }

            return list;
        }

        #endregion
    }
}
