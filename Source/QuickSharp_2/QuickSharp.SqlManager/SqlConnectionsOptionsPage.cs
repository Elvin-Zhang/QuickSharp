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

namespace QuickSharp.SqlManager
{
    public class SqlConnectionsOptionsPage : OptionsPage
    {
        private SqlConnectionManager _sqlConnectionManager;
        private Dictionary<String, SqlConnection> _sqlConnections;
        private string _selectedConnection;
        private ListView _listView;
        private ColumnHeader _nameColumnHeader;
        private ContextMenuStrip listViewContextMenuStrip;
        private ToolStripMenuItem UI_CONTEXT_MENU_NEW;
        private ToolStripMenuItem UI_CONTEXT_MENU_EDIT;
        private ToolStripMenuItem UI_CONTEXT_MENU_CLONE;
        private ToolStripMenuItem UI_CONTEXT_MENU_DELETE;

        #region Form Control Names

        public const string m_listView = "listView";
        public const string m_nameColumnHeader = "nameColumnHeader";
        public const string m_listViewContextMenuStrip = "listViewContextMenuStrip";

        #endregion

        public SqlConnectionsOptionsPage()
        {
            _sqlConnectionManager = SqlConnectionManager.GetInstance();
            _sqlConnections = new Dictionary<String, SqlConnection>();

            /*
             * Make a local copy of the connections to allow cancel.
             */

            foreach (string id in _sqlConnectionManager.SqlConnections.Keys)
                _sqlConnections[id] =
                    _sqlConnectionManager.SqlConnections[id];

            _selectedConnection = _sqlConnectionManager.SelectedConnectionId;

            Name = Constants.UI_OPTIONS_PAGE_CONNECTIONS;
            PageText = Resources.OptionsPageTextConnections;
            GroupText = Resources.OptionsGroupText;

            _listView = new ListView();
            _nameColumnHeader = new ColumnHeader();
            listViewContextMenuStrip = new ContextMenuStrip();
            UI_CONTEXT_MENU_NEW = new ToolStripMenuItem();
            UI_CONTEXT_MENU_EDIT = new ToolStripMenuItem();
            UI_CONTEXT_MENU_CLONE = new ToolStripMenuItem();
            UI_CONTEXT_MENU_DELETE = new ToolStripMenuItem();

            Controls.Add(this._listView);

            #region Form Layout

            _listView.Columns.AddRange(new ColumnHeader[] {
                _nameColumnHeader
            });
            _listView.ContextMenuStrip = listViewContextMenuStrip;
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

            _nameColumnHeader.Text = Resources.ListColumnName;
            _nameColumnHeader.Name = m_nameColumnHeader;
            _nameColumnHeader.Width = 405;
 
            listViewContextMenuStrip.Items.AddRange(new ToolStripItem[] {
                UI_CONTEXT_MENU_NEW,
                UI_CONTEXT_MENU_EDIT,
                UI_CONTEXT_MENU_CLONE,
                UI_CONTEXT_MENU_DELETE
            });
            listViewContextMenuStrip.Name = m_listViewContextMenuStrip;
            listViewContextMenuStrip.Size = new Size(153, 92);

            UI_CONTEXT_MENU_NEW.Name = Constants.UI_CONTEXT_MENU_NEW;
            UI_CONTEXT_MENU_NEW.Text = Resources.ContextMenuNew;

            UI_CONTEXT_MENU_EDIT.Name = Constants.UI_CONTEXT_MENU_EDIT;
            UI_CONTEXT_MENU_EDIT.Text = Resources.ContextMenuEdit;

            UI_CONTEXT_MENU_CLONE.Name = Constants.UI_CONTEXT_MENU_CLONE;
            UI_CONTEXT_MENU_CLONE.Text = Resources.ContextMenuClone;
 
            UI_CONTEXT_MENU_DELETE.Name = Constants.UI_CONTEXT_MENU_DELETE;
            UI_CONTEXT_MENU_DELETE.Text = Resources.ContextMenuDelete;

            #endregion

            listViewContextMenuStrip.Opening +=
                new CancelEventHandler(ListViewContextMenuStrip_Opening);

            UI_CONTEXT_MENU_NEW.Click +=
                new EventHandler(UI_CONTEXT_MENU_NEW_Click);
            UI_CONTEXT_MENU_EDIT.Click +=
                new EventHandler(UI_CONTEXT_MENU_EDIT_Click);
            UI_CONTEXT_MENU_CLONE.Click +=
                new EventHandler(UI_CONTEXT_MENU_CLONE_Click);
            UI_CONTEXT_MENU_DELETE.Click +=
                new EventHandler(UI_CONTEXT_MENU_DELETE_Click);
            _listView.DoubleClick +=
                new EventHandler(ListView_DoubleClick);

            UpdateListView();
        }

        public override void Save()
        {
            _sqlConnectionManager.SqlConnections = _sqlConnections;
            _sqlConnectionManager.SelectedConnectionId = _selectedConnection;

            UpdateUI();
        }

        private void UpdateUI()
        {
            ModuleProxy moduleProxy = ModuleProxy.GetInstance();

            if (moduleProxy != null)
                moduleProxy.Module.UpdateUI();
        }

        private void UpdateListView()
        {
            _listView.Items.Clear();

            foreach (SqlConnection cnn in _sqlConnections.Values)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = cnn.Name;
                lvi.Tag = cnn.ID;

                _listView.Items.Add(lvi);
            }
        }

        private ListViewItem GetSelectedItem()
        {
            if (_listView.SelectedItems.Count == 0) return null;
            ListViewItem lvi = _listView.SelectedItems[0];
            if (lvi == null) return null;
            return lvi;
        }

        #region Event Handlers

        private void ListViewContextMenuStrip_Opening(
            object sender, EventArgs e)
        {
            UI_CONTEXT_MENU_EDIT.Enabled = false;
            UI_CONTEXT_MENU_CLONE.Enabled = false;
            UI_CONTEXT_MENU_DELETE.Enabled = false;

            ListViewItem lvi = GetSelectedItem();
            if (lvi == null) return;

            UI_CONTEXT_MENU_EDIT.Enabled = true;
            UI_CONTEXT_MENU_CLONE.Enabled = true;
            UI_CONTEXT_MENU_DELETE.Enabled = true;
        }

        private void ListView_DoubleClick(object sender, EventArgs e)
        {
            EditItem();
        }

        private void UI_CONTEXT_MENU_NEW_Click(object sender, EventArgs e)
        {
            NewItem();
        }

        private void UI_CONTEXT_MENU_EDIT_Click(object sender, EventArgs e)
        {
            EditItem();
        }

        private void UI_CONTEXT_MENU_CLONE_Click(object sender, EventArgs e)
        {
            CloneItem();
        }

        private void UI_CONTEXT_MENU_DELETE_Click(object sender, EventArgs e)
        {
            DeleteItem();
        }

        #endregion

        #region Item Operations

        private void NewItem()
        {
            SqlConnectionForm scf = 
                new SqlConnectionForm(_sqlConnections, null);

            if (scf.ShowDialog() == DialogResult.OK)
                UpdateListView();
        }

        private void EditItem()
        {
            ListViewItem lvi = GetSelectedItem();
            if (lvi == null) return;
            if (lvi.Tag == null) return;

            string id = lvi.Tag as string;
            if (id == null) return;

            SqlConnectionForm scf =
                new SqlConnectionForm(_sqlConnections, id);

            if (scf.ShowDialog() == DialogResult.OK)
                UpdateListView();
        }

        private void CloneItem()
        {
            ListViewItem lvi = GetSelectedItem();
            if (lvi == null) return;

            string oldId = lvi.Tag as string;
            if (oldId == null) return;

            SqlConnection oldCnn = _sqlConnections[oldId];

            SqlConnection newCnn = new SqlConnection(
                Guid.NewGuid().ToString(),
                String.Format(
                    Resources.ConnectionCloneNewName, oldCnn.Name),
                oldCnn.ConnectionStringForEditing,
                oldCnn.Provider);

            _sqlConnections[newCnn.ID] = newCnn;

            UpdateListView();
        }

        private void DeleteItem()
        {
            ListViewItem lvi = GetSelectedItem();
            if (lvi == null) return;
            if (lvi.Tag == null) return;

            string id = lvi.Tag as string;
            if (id == null) return;

            if (MessageBox.Show(
                String.Format(
                    Resources.ConnectionDeleteMessage,
                    lvi.Text),
                Resources.ConnectionDeleteTitle,
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
                DialogResult.Yes)
            {
                _sqlConnections.Remove(id);

                if (_selectedConnection == id)
                    _selectedConnection = String.Empty;

                UpdateListView();
            }
        }

        #endregion
    }
}
