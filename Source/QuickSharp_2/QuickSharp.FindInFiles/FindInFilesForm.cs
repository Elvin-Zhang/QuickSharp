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
using System.Drawing;
using System.Windows.Forms;
using ScintillaNet;
using QuickSharp.Core; 
using QuickSharp.Editor;

namespace QuickSharp.FindInFiles
{
    public partial class FindInFilesForm : Form
    {
        private bool _findOnly;

        public FindInFilesForm(bool findOnly)
        {
            // Allow client applications to modify the form.
            FindInFilesFormProxy.GetInstance().UpdateFormControls(Controls);

            _findOnly = findOnly;

            InitializeComponent();

            if (findOnly)
            {
                _replaceWithLabel.Visible = false;
                _replaceTextComboBox.Visible = false;
                _lookInLabel.Location = new Point(12, 49);
                _fileSpecComboBox.Location = new Point(15, 65);
            }
            else
            {
                Text = Resources.FindDialogReplaceTitle;
                _findButton.Text = Resources.FindDialogButtonReplace;
                _matchCaseCheckBox.Visible = false;
            }

            UpdateCheckBoxes();
        }

        private void UpdateCheckBoxes()
        {
            _matchCaseCheckBox.Enabled = !_useRegexCheckBox.Checked;
        }

        #region Properties

        public string FindText
        {
            get { return _findTextComboBox.Text.Trim(); }
        }

        public string ReplaceText
        {
            get { return _replaceTextComboBox.Text.Trim(); }
        }

        public string FileSpec
        {
            get { return _fileSpecComboBox.Text.Trim(); }
        }

        public bool UseRegex
        {
            get { return _useRegexCheckBox.Checked; }
            set { _useRegexCheckBox.Checked = value; }
        }

        public bool MatchCase
        {
            get { return _matchCaseCheckBox.Checked; }
            set { _matchCaseCheckBox.Checked = value; }
        }

        public bool SearchSubFolders
        {
            get { return _searchSubFoldersCheckBox.Checked; }
            set { _searchSubFoldersCheckBox.Checked = value; }
        }

        #endregion

        #region Form Events

        private void FindButton_Click(object sender, EventArgs e)
        {
            _findTextComboBox.BackColor = Color.Empty;
            _replaceTextComboBox.BackColor = Color.Empty;
            _fileSpecComboBox.BackColor = Color.Empty;
            bool validated = true;

            if (FindText == String.Empty)
            {
                validated = false;
                _findTextComboBox.BackColor = Color.Yellow;
            }

            // Empty space is allowed for replace text

            if (FileSpec == String.Empty)
            {
                validated = false;
                _fileSpecComboBox.BackColor = Color.Yellow;
            }

            if (!validated) return;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void FindInFilesForm_Shown(object sender, System.EventArgs e)
        {
            _replaceTextComboBox.SelectionLength = 0;
            _fileSpecComboBox.SelectionLength = 0;

            /*
             * If we have an editor see if we have any selected text.
             */

            ScintillaEditForm sci = GetActiveEditor();
            if (sci != null)
            {
                /*
                 * May be multiline so we split at first newline
                 * after normalizing for Windows/Mac/Unix differences.
                 */

                string text = sci.Editor.Selection.Text;
                text = text.Replace("\n", "\r");
                int i = text.IndexOf("\r");
                if (i != -1) text = text.Substring(0, i);

                _findTextComboBox.Text = text;
                _findTextComboBox.Focus();
                _findTextComboBox.SelectAll();
            }
        }

        #endregion

        #region Helpers

        public void SetFindTextHistory(List<String> list)
        {
            LoadItemsFromList(_findTextComboBox, list);
        }

        public void SetReplaceTextHistory(List<String> list)
        {
            LoadItemsFromList(_replaceTextComboBox, list);
        }

        public void SetFileSpecHistory(List<String> list)
        {
            LoadItemsFromList(_fileSpecComboBox, list);
        }

        private void LoadItemsFromList(ComboBox cb, List<String> list)
        {
            if (list.Count == 0) return;

            foreach (string s in list)
                cb.Items.Add(s);

            cb.SelectedIndex = 0;
            cb.SelectAll();
        }

        private ScintillaEditForm GetActiveEditor()
        {
            MainForm mainForm = ApplicationManager.
                GetInstance().MainForm;

            if (mainForm.ClientWindow.DocumentsCount == 0)
                return null;

            return mainForm.ClientWindow.ActiveDocument
                as ScintillaEditForm;
        }

        #endregion
    }
}