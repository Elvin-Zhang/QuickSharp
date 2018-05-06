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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using QuickSharp.Core;

namespace QuickSharp.Tools
{
    public partial class LibraryHelperForm : Form
    {
        ApplicationManager _applicationManager;
        string _homePath;
        string _workspacePath;

        public LibraryHelperForm()
        {
            InitializeComponent();

            Text = Resources.LibraryHelperTitle;
            _libraryHelperLabel.Text = Resources.LibraryHelperMessage;
            _okButton.Text = Resources.LibraryHelperOK;
            _closeButton.Text = Resources.LibraryHelperCancel;

            _applicationManager = ApplicationManager.GetInstance();
            _homePath = _applicationManager.QuickSharpHome;
            _workspacePath = Directory.GetCurrentDirectory();

            // Add QuickDriver
            string quickDriverPath = Path.Combine(_homePath, "QuickDriver.exe");
            if (File.Exists(quickDriverPath))
                _libraryCheckedListBox.Items.Add("QuickDriver.exe");

            // Add libraries
            foreach (string file in Directory.GetFiles(_homePath, "*.dll"))
                _libraryCheckedListBox.Items.Add(Path.GetFileName(file));
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            List<String> files = new List<String>();

            foreach (string name in _libraryCheckedListBox.Items)
                if (_libraryCheckedListBox.CheckedItems.Contains(name))
                    files.Add(name);

            if (files.Count == 0) return;

            try
            {
                foreach (string name in files)
                {
                    string copyFrom = Path.Combine(_homePath, name);
                    string copyTo = Path.Combine(_workspacePath, name);

                    File.Copy(copyFrom, copyTo, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}:\r\n{1}",
                    Resources.LibraryHelperCopyErrorMessage, ex.Message),
                    Resources.LibraryHelperCopyErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            _applicationManager.NotifyFileSystemChange();
        }
    }
}
