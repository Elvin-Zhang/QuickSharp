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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using QuickSharp.Core;

namespace QuickSharp.BuildTools
{
    /// <summary>
    /// The build tool management page in the main options editor.
    /// </summary>
    public class BuildToolOptionsPage : OptionsPage
    {
        private BuildToolManager _buildToolManager;
        private BuildToolCollection _buildTools;
        private ListView _listView;
        private ColumnHeader _typeColumnHeader;
        private ColumnHeader _actionColumnHeader;
        private ColumnHeader _nameColumnHeader;
        private ContextMenuStrip _listViewContextMenuStrip;
        private ToolStripMenuItem UI_MENU_TOOL_NEW;
        private ToolStripMenuItem UI_MENU_TOOL_EDIT;
        private ToolStripMenuItem UI_MENU_TOOL_CLONE;
        private ToolStripMenuItem UI_MENU_TOOL_DELETE;

        #region Form Control Names

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_listView = "listView";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_typeColumnHeader = "typeColumnHeader";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_actionColumnHeader = "actionColumnHeader";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_nameColumnHeader = "nameColumnHeader";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_listViewContextMenuStrip = "listViewContextMenuStrip";

        #endregion

        /// <summary>
        /// Create a new instance of the options page.
        /// </summary>
        public BuildToolOptionsPage()
        {
            _buildToolManager = BuildToolManager.GetInstance();

            /*
             * Copy the build tools so we can abandon changes on cancel.
             */

            _buildTools = _buildToolManager.BuildTools.Clone();

            Name = Constants.UI_OPTIONS_BUILD_TOOLS_PAGE;
            PageText = Resources.OptionsPageTextTools;
            GroupText = Resources.OptionsGroupText;

            _listView = new ListView();
            _typeColumnHeader = new ColumnHeader();
            _actionColumnHeader = new ColumnHeader();
            _nameColumnHeader = new ColumnHeader();
            _listViewContextMenuStrip = new ContextMenuStrip();
            UI_MENU_TOOL_NEW = new ToolStripMenuItem();
            UI_MENU_TOOL_EDIT = new ToolStripMenuItem();
            UI_MENU_TOOL_CLONE = new ToolStripMenuItem();
            UI_MENU_TOOL_DELETE = new ToolStripMenuItem();

            Controls.Add(this._listView);

            #region Form layout

            _listView.Columns.AddRange(new ColumnHeader[] {
            _typeColumnHeader,
            _actionColumnHeader,
            _nameColumnHeader});
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

            _typeColumnHeader.Text = Resources.ToolColumnType;
            _typeColumnHeader.Name = m_typeColumnHeader;
            _typeColumnHeader.Width = 45;

            _actionColumnHeader.Text = Resources.ToolColumnAction;
            _actionColumnHeader.Name = m_actionColumnHeader;
            _actionColumnHeader.Width = 80;

            _nameColumnHeader.Text = Resources.ToolColumnName;
            _nameColumnHeader.Name = m_nameColumnHeader;
            _nameColumnHeader.Width = 280;
 
            _listViewContextMenuStrip.Items.AddRange(new ToolStripItem[] {
            UI_MENU_TOOL_NEW,
            UI_MENU_TOOL_EDIT,
            UI_MENU_TOOL_CLONE,
            UI_MENU_TOOL_DELETE});
            _listViewContextMenuStrip.Name = m_listViewContextMenuStrip;
            _listViewContextMenuStrip.Size = new Size(153, 92);
 
            UI_MENU_TOOL_NEW.Name = Constants.UI_MENU_TOOL_NEW;
            UI_MENU_TOOL_NEW.Text = Resources.ToolMenuNew;

            UI_MENU_TOOL_EDIT.Name = Constants.UI_MENU_TOOL_EDIT;
            UI_MENU_TOOL_EDIT.Text = Resources.ToolMenuEdit;
 
            UI_MENU_TOOL_CLONE.Name = Constants.UI_MENU_TOOL_CLONE;
            UI_MENU_TOOL_CLONE.Text = Resources.ToolMenuClone;

            UI_MENU_TOOL_DELETE.Name = Constants.UI_MENU_TOOL_DELETE;
            UI_MENU_TOOL_DELETE.Text = Resources.ToolMenuDelete;

            #endregion

            _listViewContextMenuStrip.Opening += delegate { OpenContextMenu(); };
            UI_MENU_TOOL_NEW.Click += delegate { CreateTool(); };
            UI_MENU_TOOL_EDIT.Click += delegate { EditTool(); };
            UI_MENU_TOOL_CLONE.Click += delegate { CloneTool(); };
            UI_MENU_TOOL_DELETE.Click += delegate { DeleteTool(); };
            _listView.DoubleClick += delegate { EditTool(); };

            Load();
        }

        private void Load()
        {
            _buildTools.SortTools();

            _listView.Items.Clear();

            foreach (string id in _buildTools.Tools.Keys)
            {
                BuildTool tool = _buildTools.Tools[id];

                ListViewItem lvi = new ListViewItem();
                lvi.Text = tool.DocumentType.ToString();
                lvi.SubItems.Add(tool.Action);
                lvi.SubItems.Add(tool.DisplayName);
                lvi.Tag = tool.Id;

                _listView.Items.Add(lvi);
            }
        }

        /// <summary>
        /// Save the settings when requested by the options editor.
        /// </summary>
        public override void Save()
        {
            _buildToolManager.BuildTools = _buildTools;
            _buildToolManager.UpdateBuildToolStatus();
        }

        private ListViewItem GetSelectedItem()
        {
            if (_listView.SelectedItems.Count == 0) return null;
            ListViewItem lvi = _listView.SelectedItems[0];
            if (lvi == null) return null;
            return lvi;
        }

        #region Event Handlers

        private void OpenContextMenu()
        {
            UI_MENU_TOOL_EDIT.Enabled = false;
            UI_MENU_TOOL_CLONE.Enabled = false;
            UI_MENU_TOOL_DELETE.Enabled = false;

            ListViewItem lvi = GetSelectedItem();
            if (lvi == null) return;

            UI_MENU_TOOL_EDIT.Enabled = true;
            UI_MENU_TOOL_CLONE.Enabled = true;
            UI_MENU_TOOL_DELETE.Enabled = true;
        }

        private void CreateTool()
        {
            BuildToolForm btf = 
                new BuildToolForm(_buildTools, null);

            if (btf.ShowDialog() == DialogResult.OK) Load();
        }

        private void EditTool()
        {
            ListViewItem lvi = GetSelectedItem();
            if (lvi == null) return;
            if (lvi.Tag == null) return;

            string toolId = lvi.Tag as string;
            if (toolId == null) return;

            BuildToolForm btf = new BuildToolForm(_buildTools, toolId);
            if (btf.ShowDialog() == DialogResult.OK) Load();
        }

        private void CloneTool()
        {
            ListViewItem lvi = GetSelectedItem();
            if (lvi == null) return;

            string toolId = lvi.Tag as string;
            if (toolId == null) return;

            BuildTool oldTool = _buildTools.GetTool(toolId);

            BuildTool newTool = new BuildTool(
                Guid.NewGuid().ToString(),
                oldTool.DocumentType,
                String.Format(Resources.CloneCopyName, oldTool.DisplayName));

            newTool.Action = oldTool.Action;
            newTool.Path = oldTool.Path;
            newTool.Args = oldTool.Args;
            newTool.UserArgs = oldTool.UserArgs;
            newTool.BuildCommand = oldTool.BuildCommand;
            newTool.LineParser = oldTool.LineParser;
            newTool.LineParserName = oldTool.LineParserName;

            _buildTools.AddTool(newTool);

            Load();
        }

        private void DeleteTool()
        {
            ListViewItem lvi = GetSelectedItem();
            if (lvi == null) return;
            if (lvi.Tag == null) return;

            if (MessageBox.Show(
                String.Format(
                    Resources.ToolDeleteMessage,
                    lvi.SubItems[2].Text),
                Resources.ToolDeleteTitle,
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
                DialogResult.Yes)
            {
                string id = lvi.Tag as string;
                
                if (id != null)
                    _buildTools.DeleteTool(id);
                
                Load();
            }
        }

        #endregion
    }
}
