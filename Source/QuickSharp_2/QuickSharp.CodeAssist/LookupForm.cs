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
using System.Drawing;
using System.Windows.Forms;
using QuickSharp.Core;

namespace QuickSharp.CodeAssist
{
    /// <summary>
    /// The code assist pop-up window.
    /// </summary>
    public partial class LookupForm : Form
    {
        private string _selectedText;
        private string _lookAheadText;
        private string _incrementalSearchText;
        private List<string> _insertionTemplates;
        private int _insertionTemplateIndex;
        private ThemeFlags _themeFlags;

        /// <summary>
        /// Create the pop-up window.
        /// </summary>
        /// <param name="p">The location of the pop-up window.</param>
        /// <param name="list">The code assist information to be presented.</param>
        public LookupForm(Point p, LookupList list)
        {
            _themeFlags = ApplicationManager.GetInstance().
                ClientProfile.ThemeFlags;

            InitializeComponent();
            
            _listView.MultiSelect = false;
            _listView.Columns[0].Width = -1;

            this.StartPosition = FormStartPosition.Manual;
            this.Location = p;

            PopulateLookupList(list);

            // Remember items start with " " (it looks better)
            _incrementalSearchText = " " + list.LookAheadText;

            // Allow client apps to modify the form.
            LookupFormProxy.GetInstance().UpdateFormControls(Controls);

        }

        #region Properties

        /// <summary>
        /// The text to be inserted in the document when an item is selected.
        /// </summary>
        public string SelectedText
        {
            get { return _selectedText; }
        }

        /// <summary>
        /// The text fragement used to preselect an item in the lookup list.
        /// </summary>
        public string LookAheadText
        {
            get { return _lookAheadText; }
        }

        /// <summary>
        /// The template to be used to insert the selected text into the document.
        /// </summary>
        public string InsertionTemplate
        {
            get
            {
                if (_insertionTemplates.Count == 0)
                    return null;
                else
                    return _insertionTemplates[_insertionTemplateIndex];
            }
        }

        #endregion

        /// <summary>
        /// Display the form without a title bar.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                // Hide the title bar
                CreateParams cp = base.CreateParams;
                cp.Style &= ~0xc00000; // WS_CAPTION;
                return cp;
            }
        }

        private void PopulateLookupList(LookupList lookupList)
        {
            _lookAheadText = lookupList.LookAheadText;
            _insertionTemplates = lookupList.InsertionTemplates;

            _listView.SuspendLayout();

            foreach (LookupListItem item in lookupList.Items)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Tag = item;
                lvi.Text = " " + item.DisplayText;
                lvi.ImageKey = item.Category;
                lvi.ToolTipText = item.ToolTipText;

                // Apply theme colors if available
                if (_themeFlags != null)
                {
                    if (_themeFlags.ViewAltBackColor != Color.Empty)
                        _listView.BackColor = _themeFlags.ViewAltBackColor;

                    if (_themeFlags.ViewAltForeColor != Color.Empty)
                        _listView.ForeColor = _themeFlags.ViewAltForeColor;
                }

                _listView.Items.Add(lvi);
            }

            _listView.ResumeLayout(true);

            if (!String.IsNullOrEmpty(lookupList.LookAheadText) &&
                (_listView.Items.Count > 0))
            {
                ListViewItem item = _listView.FindItemWithText(
                    " " + lookupList.LookAheadText, false, 0, true);

                if (item != null)
                {
                    item.Selected = true;
                    item.Focused = true;
                    item.EnsureVisible();
                }
            }
        }

        #region Event Handlers

        private void LookupForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape ||
                e.KeyCode == Keys.Back)
            {
                DialogResult = DialogResult.Cancel;
                return;
            }

            if (e.KeyCode == Keys.Enter ||
                e.KeyCode == Keys.Space)
            {
                if (_listView.SelectedItems.Count > 0)
                {
                    LookupListItem li =
                        _listView.SelectedItems[0].Tag as LookupListItem;

                    _selectedText = li.InsertText;
                    _insertionTemplateIndex = li.TemplateIndex;
                    DialogResult = DialogResult.OK;
                }

                return;
            }

            if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z)
            {
                _incrementalSearchText += (char)e.KeyValue;

                ListViewItem item = _listView.FindItemWithText(
                    _incrementalSearchText, false, 0, true);

                if (item != null)
                {
                    item.Selected = true;
                    item.Focused = true;
                    item.EnsureVisible();
                }
            }
            else
            {
                _incrementalSearchText = " ";
            }
        }

        private void ListView_MouseDoubleClick(
            object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_listView.SelectedItems.Count > 0)
                {
                    LookupListItem li =
                        _listView.SelectedItems[0].Tag as LookupListItem;

                    _selectedText = li.InsertText;
                    _insertionTemplateIndex = li.TemplateIndex;
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void LookupContextMenuStrip_Opening(
            object sender, CancelEventArgs e)
        {
            LookupListItem li = null;

            if (_listView.SelectedItems.Count > 0)
                li = _listView.SelectedItems[0].Tag as LookupListItem;

            if (li == null || li.MenuItems.Count == 0)
            {
                e.Cancel = true;
                return;
            }

            _lookupContextMenuStrip.Items.Clear();

            foreach (LookupMenuItem lmi in li.MenuItems)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = lmi.DisplayText;
                item.Tag = lmi;
                item.Click += LookupToolStripMenuItem_Click;

                // Apply theme color if available.
                if (_themeFlags != null && _themeFlags.MenuForeColor != Color.Empty)
                    item.ForeColor = _themeFlags.MenuForeColor;

                _lookupContextMenuStrip.Items.Add(item);
            }
        }

        private void LookupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tmi = sender as ToolStripMenuItem;
            if (tmi == null) return;

            LookupMenuItem lmi = tmi.Tag as LookupMenuItem;
            if (lmi == null) return;

            _selectedText = lmi.InsertText;
            _insertionTemplateIndex = lmi.TemplateIndex;
            DialogResult = DialogResult.OK;
        }

        #endregion
    }
}
