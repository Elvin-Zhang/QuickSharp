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
using System.Data;
using System.Windows.Forms;
using System.ComponentModel;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.SqlEditor
{
    partial class SqlEditForm
    {
        private IContainer _components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
                _components.Dispose();

            base.Dispose(disposing);
        }

        #region Form Layout

        private Splitter _splitter;
        private ContextMenuStrip _dataGridMenu;
        private DataGridView _dataGrid;
        private DataSet _dataSet;

        private void InitializeComponent()
        {
            SuspendLayout();

            _splitter = new Splitter();
            _splitter.Name = "splitter";
            _splitter.Dock = DockStyle.Bottom;

            _dataGridMenu = new ContextMenuStrip();
            _dataGridMenu.Name = Constants.UI_DATA_GRID_MENU;

            // Add a dummy item to make sure the menu shows first time
            _dataGridMenu.Items.Add(new ToolStripMenuItem());

            _dataGridMenu.Opening += 
                new CancelEventHandler(DataGridContextMenuStrip_Opening);

            _dataGrid = new DataGridView();
            _dataGrid.Name = "dataGrid";
            _dataGrid.Dock = DockStyle.Bottom;
            _dataGrid.BorderStyle = BorderStyle.Fixed3D;
            _dataGrid.AllowDrop = false;
            _dataGrid.AllowUserToAddRows = false;
            _dataGrid.AllowUserToDeleteRows = false;
            _dataGrid.MultiSelect = false;
            _dataGrid.ReadOnly = true;
            _dataGrid.Height = 200;
            _dataGrid.ContextMenuStrip = _dataGridMenu;
 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(443, 302);
            Controls.Add(scintilla);
            Controls.Add(_splitter);
            Controls.Add(_dataGrid);
            DockAreas = DockAreas.Document;
            Font = new Font("Microsoft Sans Serif", 8.25F,
                FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            Icon = Resources.DocumentIcon;
            Name = "SqlEditForm";
            ShowInTaskbar = false;
            Text = "SqlEditForm";

            ResumeLayout(false);
        }

        #endregion
    }
}