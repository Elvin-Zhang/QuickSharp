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
using System.Drawing;
using System.Windows.Forms;
using QuickSharp.Core;

namespace QuickSharp.Workspace
{
    /// <summary>
    /// The rename file/folder form invoked by the rename command.
    /// </summary>
    public partial class RenameForm : Form
    {
        /// <summary>
        /// Create the form.
        /// </summary>
        public RenameForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The new name for the item to be renamed. Sets the default value
        /// and provides the new value on exit from the form.
        /// </summary>
        public string NewName
        {
            get { return _newNameTextBox.Text; }
            set { _newNameTextBox.Text = value; }
        }

        /// <summary>
        /// The form title.
        /// </summary>
        public string Title
        {
            set { Text = value; }
        }

        private void RenameForm_Load(object sender, EventArgs e)
        {
            string filename = Path.GetFileNameWithoutExtension(
                _newNameTextBox.Text);

            _newNameTextBox.SelectionStart = 0;
            _newNameTextBox.SelectionLength = filename.Length;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            _newNameTextBox.Text = _newNameTextBox.Text.Trim();

            if (FileTools.FilenameIsInvalid(_newNameTextBox.Text))
            {
                _newNameTextBox.BackColor = Color.Yellow;
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
