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

using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace QuickSharp.Output
{
    partial class OutputForm
    {
        private IContainer _components = null;
        private ToolStrip _mainToolStrip;
        private ToolStripButton _listViewToolStripButton;
        private ToolStripButton _textViewToolStripButton;
        private ToolStripSeparator _clearViewToolStripSeparator;
        private ToolStripButton _clearViewToolStripButton;
        private TextBox _textViewTextBox;
        private ListView _listViewListView;
        private ColumnHeader _displayLine;
        private ImageList _listViewImageList;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
                _components.Dispose();

            base.Dispose(disposing);
        }

        #region Form Layout

        private void InitializeComponent()
        {
            #region Controls

            _components = new Container();
            _mainToolStrip = new ToolStrip();
            _listViewToolStripButton = new ToolStripButton();
            _textViewToolStripButton = new ToolStripButton();
            _clearViewToolStripSeparator = new ToolStripSeparator();
            _clearViewToolStripButton = new ToolStripButton();
            _textViewTextBox = new TextBox();
            _listViewListView = new ListView();
            _displayLine = new ColumnHeader();
            _listViewImageList = new ImageList(_components);

            #endregion

            _mainToolStrip.SuspendLayout();
            SuspendLayout();

            _mainToolStrip.Items.AddRange(new ToolStripItem[] {
                _listViewToolStripButton,
                _textViewToolStripButton,
                _clearViewToolStripSeparator,
                _clearViewToolStripButton
            });
            _mainToolStrip.Location = new Point(0, 0);
            _mainToolStrip.Name = "mainToolStrip";
            _mainToolStrip.TabIndex = 0;
            _mainToolStrip.Text = "toolStrip1";

            _listViewToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _listViewToolStripButton.Image = Resources.ListView;
            _listViewToolStripButton.ImageTransparentColor = Color.Fuchsia;
            _listViewToolStripButton.Name = "listViewToolStripButton";
            _listViewToolStripButton.Text = Resources.ToolbarViewAsList;
            _listViewToolStripButton.Click += new System.EventHandler(listViewToolStripButton_Click);

            _textViewToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _textViewToolStripButton.Image = Resources.TextView;
            _textViewToolStripButton.ImageTransparentColor = Color.Fuchsia;
            _textViewToolStripButton.Name = "textViewToolStripButton";
            _textViewToolStripButton.Text = Resources.ToolbarViewAsText;
            _textViewToolStripButton.Click += new System.EventHandler(textViewToolStripButton_Click);

            _clearViewToolStripSeparator.Name = "toolStripSeparator1";

            _clearViewToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _clearViewToolStripButton.Image = Resources.ClearOutput;
            _clearViewToolStripButton.ImageTransparentColor = Color.Fuchsia;
            _clearViewToolStripButton.Name = "clearViewToolStripButton";
            _clearViewToolStripButton.Text = Resources.ToolbarClearView;
            _clearViewToolStripButton.Click += new System.EventHandler(clearViewToolStripButton_Click);

            _textViewTextBox.BackColor = SystemColors.Window;
            _textViewTextBox.Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            _textViewTextBox.Location = new Point(460, 76);
            _textViewTextBox.Multiline = true;
            _textViewTextBox.Name = "textViewTextBox";
            _textViewTextBox.ReadOnly = true;
            _textViewTextBox.ScrollBars = ScrollBars.Both;
            _textViewTextBox.TabIndex = 1;
            _textViewTextBox.WordWrap = false;
            _textViewTextBox.KeyDown += new KeyEventHandler(TextViewTextBox_KeyDown);

            _listViewListView.Columns.AddRange(new ColumnHeader[] {
                _displayLine
            });
            _listViewListView.Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            _listViewListView.FullRowSelect = true;
            _listViewListView.HeaderStyle = ColumnHeaderStyle.None;
            _listViewListView.Location = new Point(288, 76);
            _listViewListView.MultiSelect = false;
            _listViewListView.Name = "listViewListView";
            _listViewListView.SmallImageList = _listViewImageList;
            _listViewListView.TabIndex = 2;
            _listViewListView.UseCompatibleStateImageBehavior = false;
            _listViewListView.View = View.Details;
            _listViewListView.MouseDoubleClick += new MouseEventHandler(ListViewListView_MouseDoubleClick);

            _listViewImageList.TransparentColor = Color.Fuchsia;
            _listViewImageList.Images.Add("ERROR", Resources.Error);

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(701, 233);
            Controls.Add(_listViewListView);
            Controls.Add(_textViewTextBox);
            Controls.Add(_mainToolStrip);
            DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)
                ((((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            HideOnClose = true;
            Icon = Resources.OutputIcon;
            Name = "OutputForm";
            ShowInTaskbar = false;
            Text = Resources.OutputWindowTitle;

            _mainToolStrip.ResumeLayout(false);
            _mainToolStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}