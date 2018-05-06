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
using System.Text.RegularExpressions;

namespace QuickSharp.Explorer
{
    /// <summary>
    /// The file type filter management page in the main options editor.
    /// </summary>
    public class FileFiltersOptionsPage : OptionsPage
    {
        private SettingsManager _settingsManager;
        private List<FileFilter> _filters;
        private ListView _listView;
        private ColumnHeader _nameColumnHeader;
        private ColumnHeader _filterColumnHeader;
        private ContextMenuStrip _listViewContextMenuStrip;
        private ToolStripMenuItem UI_MENU_FILTER_NEW;
        private ToolStripMenuItem UI_MENU_FILTER_EDIT;
        private ToolStripMenuItem UI_MENU_FILTER_CLONE;
        private ToolStripMenuItem UI_MENU_FILTER_DELETE;

        #region From Control Names

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_listView = "listView";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_nameColumnHeader = "nameColumnHeader";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_filterColumnHeader = "filterColumnHeader";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_listViewContextMenuStrip = "listViewContextMenuStrip";

        #endregion

        /// <summary>
        /// Create a new instance of the options page.
        /// </summary>
        public FileFiltersOptionsPage()
        {
            _settingsManager = SettingsManager.GetInstance();

            /*
             * Copy the filters so we can abandon changes on cancel.
             */

            _filters = new List<FileFilter>();

            foreach (FileFilter f in _settingsManager.FileFilters.Values)
                _filters.Add(f.Clone());

            /*
             * Setup the page.
             */

            Name = Constants.UI_OPTIONS_PAGE_FILTERS;
            PageText = Resources.OptionsPageTextFilters;
            GroupText = Resources.OptionsGroupText;

            _listView = new ListView();
            _nameColumnHeader = new ColumnHeader();
            _filterColumnHeader = new ColumnHeader();
            _listViewContextMenuStrip = new ContextMenuStrip();
            UI_MENU_FILTER_NEW = new ToolStripMenuItem();
            UI_MENU_FILTER_EDIT = new ToolStripMenuItem();
            UI_MENU_FILTER_CLONE = new ToolStripMenuItem();
            UI_MENU_FILTER_DELETE = new ToolStripMenuItem();

            Controls.Add(this._listView);

            #region Form Layout

            _listView.Columns.AddRange(new ColumnHeader[] {
            _nameColumnHeader,
            _filterColumnHeader});
            _listView.ContextMenuStrip = _listViewContextMenuStrip;
            _listView.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            _listView.FullRowSelect = true;
            _listView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            _listView.Location = new Point(0, 0);
            _listView.MultiSelect = false;
            _listView.Name = m_listView;
            _listView.Size = new Size(430, 250);
            _listView.TabIndex = 0;
            _listView.UseCompatibleStateImageBehavior = false;
            _listView.View = View.Details;

            _nameColumnHeader.Text = Resources.ListViewNameColumn;
            _nameColumnHeader.Name = m_nameColumnHeader;
            _nameColumnHeader.Width = 180;

            _filterColumnHeader.Text = Resources.ListViewFilterColumn;
            _filterColumnHeader.Name = m_filterColumnHeader;
            _filterColumnHeader.Width = 225;
 
            _listViewContextMenuStrip.Items.AddRange(new ToolStripItem[] {
            UI_MENU_FILTER_NEW,
            UI_MENU_FILTER_EDIT,
            UI_MENU_FILTER_CLONE,
            UI_MENU_FILTER_DELETE});
            _listViewContextMenuStrip.Name = m_listViewContextMenuStrip;
            _listViewContextMenuStrip.Size = new Size(153, 92);

            UI_MENU_FILTER_NEW.Name = Constants.UI_MENU_FILTER_NEW;
            UI_MENU_FILTER_NEW.Text = Resources.FilterMenuNew;

            UI_MENU_FILTER_EDIT.Name = Constants.UI_MENU_FILTER_EDIT;
            UI_MENU_FILTER_EDIT.Text = Resources.FilterMenuEdit;

            UI_MENU_FILTER_CLONE.Name = Constants.UI_MENU_FILTER_CLONE;
            UI_MENU_FILTER_CLONE.Text = Resources.FilterMenuClone;

            UI_MENU_FILTER_DELETE.Name = Constants.UI_MENU_FILTER_DELETE;
            UI_MENU_FILTER_DELETE.Text = Resources.FilterMenuDelete;

            #endregion

            _listViewContextMenuStrip.Opening += delegate { OpenContextMenu(); };
            UI_MENU_FILTER_NEW.Click += delegate { CreateFilter(); };
            UI_MENU_FILTER_EDIT.Click += delegate { EditFilter(); };
            UI_MENU_FILTER_CLONE.Click += delegate { CloneFilter(); };
            UI_MENU_FILTER_DELETE.Click += delegate { DeleteFilter(); };
            _listView.DoubleClick += delegate { EditFilter(); };

            Load();
        }

        private void Load()
        {
            _filters.Sort(new FileFilterComparer());

            _listView.Items.Clear();

            foreach (FileFilter f in _filters)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = f.Name;
                lvi.SubItems.Add(f.Filter);
                lvi.Tag = f.ID;

                _listView.Items.Add(lvi);
            }
        }

        /// <summary>
        /// Save the settings when requested by the options editor.
        /// </summary>
        public override void Save()
        {
            Dictionary<string, FileFilter> dict =
                new Dictionary<string, FileFilter>();

            foreach (FileFilter f in _filters)
                dict[f.ID] = f;

            _settingsManager.FileFilters = dict;

            /*
             * This is the second of two pages and gets called last
             * so we do the UI update here to cover both the pages.
             */

            _settingsManager.UpdateUI();
        }

        /// <summary>
        /// Validate the patterns.
        /// </summary>
        /// <returns>True if validated correctly.</returns>
        public override bool Validate()
        {
            foreach (FileFilter f in _filters)
            {
                f.Filter = f.Filter.Trim();

                foreach (string s in f.Filter.Split(' '))
                {
                    string pattern = s;

                    // Allow "!" prefix
                    if (pattern.StartsWith("!"))
                        pattern = pattern.Substring(1);

                    // Allow global pseudo pattern
                    if (pattern == Constants.PSEUDO_PATTERN_GLOBAL)
                        continue;

                    try
                    {
                        //validate the regex.
                        Regex re = new Regex(pattern);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("{0}:\r\n{1}",
                                Resources.FilterRegexErrorMessage, ex.Message),
                            Resources.FilterRegexErrorTitle,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        return false;
                    }
                }
            }

            return true;
        }

        private ListViewItem GetSelectedItem()
        {
            if (_listView.SelectedItems.Count == 0) return null;
            ListViewItem lvi = _listView.SelectedItems[0];
            if (lvi == null) return null;
            return lvi;
        }

        #region Event handlers

        private void OpenContextMenu()
        {
            UI_MENU_FILTER_EDIT.Enabled = false;
            UI_MENU_FILTER_CLONE.Enabled = false;
            UI_MENU_FILTER_DELETE.Enabled = false;

            ListViewItem lvi = GetSelectedItem();
            if (lvi == null) return;

            UI_MENU_FILTER_EDIT.Enabled = true;
            UI_MENU_FILTER_CLONE.Enabled = true;
            UI_MENU_FILTER_DELETE.Enabled = true;
        }

        private void CreateFilter()
        {
            FileFilterForm form = new FileFilterForm(_filters, null);
            if (form.ShowDialog() == DialogResult.OK) Load();
        }

        private void EditFilter()
        {
            ListViewItem lvi = GetSelectedItem();
            if (lvi == null) return;
            if (lvi.Tag == null) return;

            string id = lvi.Tag as string;
            if (id == null) return;

            FileFilterForm form = new FileFilterForm(_filters, id);
            if (form.ShowDialog() == DialogResult.OK) Load();
        }

        private void CloneFilter()
        {
            ListViewItem lvi = GetSelectedItem();
            if (lvi == null) return;

            string id = lvi.Tag as string;
            if (id == null) return;

            FileFilter f = new FileFilter();
            f.ID = Guid.NewGuid().ToString();
            f.Name = String.Format(Resources.CloneCopyName, lvi.Text);
            f.Filter = lvi.SubItems[1].Text;

            _filters.Add(f);

            Load();
        }

        private void DeleteFilter()
        {
            ListViewItem lvi = GetSelectedItem();
            if (lvi == null) return;
            if (lvi.Tag == null) return;

            if (MessageBox.Show(String.Format(
                    Resources.FilterDeleteMessage, lvi.Text),
                Resources.FilterDeleteTitle,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                string id = lvi.Tag as string;

                if (id != null)
                {
                    FileFilter f = FileFilter.GetFromList(_filters, id);
                    if (f != null)
                    {
                        _filters.Remove(f);

                        Load();
                    }
                }
            }
        }

        #endregion
    }
}
