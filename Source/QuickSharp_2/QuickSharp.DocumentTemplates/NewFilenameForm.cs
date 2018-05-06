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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using QuickSharp.Core;

namespace QuickSharp.DocumentTemplates
{
    public partial class NewFilenameForm : Form
    {
        public string Filename
        {
            get { return _filenameTextBox.Text; }
        }

        public NewFilenameForm(string filename, string template)
        {
            InitializeComponent();

            _templateLabel.Text = template;
            _filenameTextBox.Text = filename;
            _filenameTextBox.SelectionLength = filename.Length;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            _filenameTextBox.Text = _filenameTextBox.Text.Trim();

            if (FileTools.FilenameIsInvalid(_filenameTextBox.Text))
            {
                _filenameTextBox.BackColor = Color.Yellow;
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
