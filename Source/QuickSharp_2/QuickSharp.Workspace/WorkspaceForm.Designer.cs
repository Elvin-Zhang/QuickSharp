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
using System.Windows.Forms;
using System.ComponentModel;
using QuickSharp.Core;

namespace QuickSharp.Workspace
{
    partial class WorkspaceForm
    {
        private IContainer _components = null;
        private ToolStrip _mainToolStrip;
        private ListView _mainListView;
        private ImageList _fileViewImageList;
        private ContextMenuStrip _fileViewMenu;
        private ToolStripButton _parentToolStripButton;
        private ToolStripButton _refreshToolStripButton;
        private ToolStripButton _moveToolStripButton;
        private ToolStripDropDownButton _visitedFolderDropDownButton;
        private ToolStripButton _backupWorkspaceButton;
        private ToolStripButton _launchShellButton;
        private ToolStripButton _launchExplorerButton;
        private ToolStripButton _showSourceOnlyButton;
        private ToolStripMenuItem UI_FILE_VIEW_MENU_OPEN;
        private ToolStripSeparator UI_FILE_VIEW_MENU_SEP_1;
        private ToolStripMenuItem UI_FILE_VIEW_MENU_RENAME;
        private ToolStripMenuItem UI_FILE_VIEW_MENU_CLONE;
        private ToolStripMenuItem UI_FILE_VIEW_MENU_DELETE;
        private ToolStripSeparator UI_FILE_VIEW_MENU_SEP_2;
        private ToolStripMenuItem UI_FILE_VIEW_MENU_CREATE_FOLDER;
        private ToolStripSeparator UI_FILE_VIEW_MENU_SEP_3;
        private ToolStripMenuItem UI_FILE_VIEW_MENU_MOVE_TO_PARENT;
        private ToolStripSeparator UI_FILE_VIEW_MENU_SEP_4;
        private ToolStripMenuItem UI_FILE_VIEW_MENU_SELECT_SIMILAR;
        private ToolStripMenuItem UI_FILE_VIEW_MENU_INVERT_SELECTION;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
                _components.Dispose();

            base.Dispose(disposing);
        }

        #region Form Layout

        private void InitForm()
        {
            bool enableShell = !_applicationManager.ClientProfile.
                HaveFlag(ClientFlags.WorkspaceDisableWindowsShell);

            bool enableShowSource = _applicationManager.ClientProfile.
                HaveFlag(ClientFlags.WorkspaceEnableShowSource);

            #region Controls

            _components = new Container();
            _mainToolStrip = new ToolStrip();
            _refreshToolStripButton = new ToolStripButton();
            _parentToolStripButton = new ToolStripButton();
            _moveToolStripButton = new ToolStripButton();
            _visitedFolderDropDownButton = new ToolStripDropDownButton();
            _backupWorkspaceButton = new ToolStripButton();
            _launchExplorerButton = new ToolStripButton();
            _launchShellButton = new ToolStripButton();
            _showSourceOnlyButton = new ToolStripButton();
            _mainListView = new ListView();
            _fileViewMenu = new ContextMenuStrip(_components);
            UI_FILE_VIEW_MENU_OPEN = new ToolStripMenuItem();
            UI_FILE_VIEW_MENU_SEP_1 = new ToolStripSeparator();
            UI_FILE_VIEW_MENU_RENAME = new ToolStripMenuItem();
            UI_FILE_VIEW_MENU_CLONE = new ToolStripMenuItem();
            UI_FILE_VIEW_MENU_DELETE = new ToolStripMenuItem();
            UI_FILE_VIEW_MENU_SEP_2 = new ToolStripSeparator();
            UI_FILE_VIEW_MENU_CREATE_FOLDER = new ToolStripMenuItem();
            UI_FILE_VIEW_MENU_SEP_3 = new ToolStripSeparator();
            UI_FILE_VIEW_MENU_MOVE_TO_PARENT = new ToolStripMenuItem();
            UI_FILE_VIEW_MENU_SEP_4 = new ToolStripSeparator();
            UI_FILE_VIEW_MENU_SELECT_SIMILAR = new ToolStripMenuItem();
            UI_FILE_VIEW_MENU_INVERT_SELECTION = new ToolStripMenuItem();
            _fileViewImageList = new ImageList(_components);

            #endregion

            _mainToolStrip.SuspendLayout();
            _fileViewMenu.SuspendLayout();
            SuspendLayout();

            _mainToolStrip.Items.Add(_refreshToolStripButton);
            _mainToolStrip.Items.Add(_parentToolStripButton);
            _mainToolStrip.Items.Add(_moveToolStripButton);
            _mainToolStrip.Items.Add(_visitedFolderDropDownButton);

            if (enableShell)
            {
                _mainToolStrip.Items.Add(MenuTools.CreateSeparator(
                    Constants.UI_TOOLBAR_SEP_1));
                _mainToolStrip.Items.Add(_launchExplorerButton);
                _mainToolStrip.Items.Add(_launchShellButton);
            }

            _mainToolStrip.Items.Add(MenuTools.CreateSeparator(
                Constants.UI_TOOLBAR_SEP_2));
            _mainToolStrip.Items.Add(_backupWorkspaceButton);

            if (enableShowSource)
            {
                _mainToolStrip.Items.Add(MenuTools.CreateSeparator(
                    Constants.UI_TOOLBAR_SEP_3));
                _mainToolStrip.Items.Add(_showSourceOnlyButton);
            }
            else
            {
                _showSourceOnly = false;
            }

            _mainToolStrip.Location = new Point(0, 0);
            _mainToolStrip.Name = "mainToolStrip";
            _mainToolStrip.TabIndex = 0;
            _mainToolStrip.Text = "toolStrip1";

            _refreshToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _refreshToolStripButton.Image = Resources.Refresh;
            _refreshToolStripButton.ImageTransparentColor = Color.Fuchsia;
            _refreshToolStripButton.Name = Constants.UI_TOOLBAR_REFRESH;
            _refreshToolStripButton.Text = Resources.WorkspaceToolbarRefresh;
            _refreshToolStripButton.Click += delegate { RefreshFileView(); };

            _parentToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _parentToolStripButton.Image = Resources.ParentFolder;
            _parentToolStripButton.ImageTransparentColor = Color.Fuchsia;
            _parentToolStripButton.Name = Constants.UI_TOOLBAR_MOVE_TO_PARENT;
            _parentToolStripButton.Text = Resources.WorkspaceToolbarParentFolder;
            _parentToolStripButton.Click += delegate { MoveToParent(); };

            _moveToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _moveToolStripButton.Image = Resources.SearchFolder;
            _moveToolStripButton.ImageTransparentColor = Color.Fuchsia;
            _moveToolStripButton.Name = Constants.UI_TOOLBAR_LOCATE_NEW_FOLDER;
            _moveToolStripButton.Text = Resources.WorkspaceToolbarOpenFolder;
            _moveToolStripButton.Click += delegate { MoveWorkspace(); };

            _visitedFolderDropDownButton.Name = Constants.UI_TOOLBAR_SELECT_VISITED_FOLDER;
            _visitedFolderDropDownButton.Enabled = false;
            _visitedFolderDropDownButton.DropDownOpening += delegate { RefreshVisitedFolderList(); };

            _backupWorkspaceButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _backupWorkspaceButton.Image = Resources.BackupWorkspace;
            _backupWorkspaceButton.ImageTransparentColor = Color.Fuchsia;
            _backupWorkspaceButton.Name = Constants.UI_TOOLBAR_BACKUP_WORKSPACE;
            _backupWorkspaceButton.Text = Resources.WorkspaceToolbarBackup;
            _backupWorkspaceButton.Click += delegate { BackupWorkspace(); };

            _launchExplorerButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _launchExplorerButton.Image = Resources.Explorer;
            _launchExplorerButton.ImageTransparentColor = Color.Fuchsia;
            _launchExplorerButton.Name = Constants.UI_TOOLBAR_LAUNCH_EXPLORER;
            _launchExplorerButton.Text = Resources.WorkspaceToolbarLaunchExplorer;
            _launchExplorerButton.Click += delegate { LaunchExplorer(); };

            _launchShellButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _launchShellButton.Image = Resources.Console;
            _launchShellButton.ImageTransparentColor = Color.Fuchsia;
            _launchShellButton.Name = Constants.UI_TOOLBAR_LAUNCH_SHELL;
            _launchShellButton.Text = Resources.WorkspaceToolbarLaunchShell;
            _launchShellButton.Click += delegate { LaunchShell(); };

            _showSourceOnlyButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _showSourceOnlyButton.Image = Resources.ShowSourceOnly;
            _showSourceOnlyButton.ImageTransparentColor = Color.Fuchsia;
            _showSourceOnlyButton.Name = Constants.UI_TOOLBAR_SHOW_SOURCE_ONLY;
            _showSourceOnlyButton.Text = Resources.WorkspaceToolbarShowSourceOnly;
            _showSourceOnlyButton.Checked = _showSourceOnly;
            _showSourceOnlyButton.Click += delegate { ShowSourceOnly(); };

            _mainListView.ContextMenuStrip = _fileViewMenu;
            _mainListView.Dock = DockStyle.Fill;
            _mainListView.View = View.Details;
            _mainListView.HeaderStyle = ColumnHeaderStyle.None;
            _mainListView.Columns.Add("item", -1);
            _mainListView.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            _mainListView.SmallImageList = _fileViewImageList;
            _mainListView.Location = new Point(0, 25);
            _mainListView.Name = "mainListView";
            _mainListView.TabIndex = 1;
            _mainListView.MouseDoubleClick += new MouseEventHandler(MainListView_MouseDoubleClick);
            _mainListView.KeyDown += new KeyEventHandler(MainListView_KeyDown);
            _mainListView.LostFocus += new System.EventHandler(MainListView_LostFocus);
            _mainListView.ItemSelectionChanged += 
                new ListViewItemSelectionChangedEventHandler(MainListView_ItemSelectionChanged);

            _fileViewMenu.Items.AddRange(new ToolStripItem[] {
                UI_FILE_VIEW_MENU_OPEN,
                UI_FILE_VIEW_MENU_SEP_1,
                UI_FILE_VIEW_MENU_RENAME,
                UI_FILE_VIEW_MENU_CLONE,
                UI_FILE_VIEW_MENU_DELETE,
                UI_FILE_VIEW_MENU_SEP_2,
                UI_FILE_VIEW_MENU_CREATE_FOLDER,
                UI_FILE_VIEW_MENU_SEP_3,
                UI_FILE_VIEW_MENU_MOVE_TO_PARENT,
                UI_FILE_VIEW_MENU_SEP_4,
                UI_FILE_VIEW_MENU_SELECT_SIMILAR,
                UI_FILE_VIEW_MENU_INVERT_SELECTION
            });
            _fileViewMenu.Name = "fileViewMenu";
            _fileViewMenu.Opening += new CancelEventHandler(listViewMenu_Opening);

            UI_FILE_VIEW_MENU_OPEN.Name = Constants.UI_FILE_VIEW_MENU_OPEN;
            UI_FILE_VIEW_MENU_OPEN.Text = Resources.WorkspaceMenuOpen;
            UI_FILE_VIEW_MENU_OPEN.ShortcutKeyDisplayString = Resources.EnterKey;
            UI_FILE_VIEW_MENU_OPEN.Image = Resources.OpenFolder;
            UI_FILE_VIEW_MENU_OPEN.ImageTransparentColor = Color.Fuchsia;
            UI_FILE_VIEW_MENU_OPEN.Click += new System.EventHandler(UI_FILE_VIEW_MENU_OPEN_Click);

            UI_FILE_VIEW_MENU_SEP_1.Name = Constants.UI_FILE_VIEW_MENU_SEP_1;
            
            UI_FILE_VIEW_MENU_RENAME.Name = Constants.UI_FILE_VIEW_MENU_RENAME;
            UI_FILE_VIEW_MENU_RENAME.Text = Resources.WorkspaceMenuRename;
            UI_FILE_VIEW_MENU_RENAME.ShortcutKeys = Keys.F2;
            UI_FILE_VIEW_MENU_RENAME.Click += new System.EventHandler(UI_FILE_VIEW_MENU_RENAME_Click);

            UI_FILE_VIEW_MENU_CLONE.Name = Constants.UI_FILE_VIEW_MENU_CLONE;
            UI_FILE_VIEW_MENU_CLONE.Text = Resources.WorkspaceMenuClone;
            UI_FILE_VIEW_MENU_CLONE.Click += new System.EventHandler(UI_FILE_VIEW_MENU_CLONE_Click);

            UI_FILE_VIEW_MENU_DELETE.Name = Constants.UI_FILE_VIEW_MENU_DELETE;
            UI_FILE_VIEW_MENU_DELETE.Text = Resources.WorkspaceMenuDelete;
            UI_FILE_VIEW_MENU_DELETE.ShortcutKeys = Keys.Delete;
            UI_FILE_VIEW_MENU_DELETE.Click += new System.EventHandler(UI_FILE_VIEW_MENU_DELETE_Click);

            UI_FILE_VIEW_MENU_SEP_2.Name = Constants.UI_FILE_VIEW_MENU_SEP_2;

            UI_FILE_VIEW_MENU_CREATE_FOLDER.Name = Constants.UI_FILE_VIEW_MENU_CREATE_FOLDER;
            UI_FILE_VIEW_MENU_CREATE_FOLDER.Text = Resources.WorkspaceMenuCreateFolder;
            UI_FILE_VIEW_MENU_CREATE_FOLDER.Image = Resources.NewFolder;
            UI_FILE_VIEW_MENU_CREATE_FOLDER.ImageTransparentColor = Color.Fuchsia;
            UI_FILE_VIEW_MENU_CREATE_FOLDER.Click += new System.EventHandler(UI_FILE_VIEW_MENU_CREATE_FOLDER_Click);

            UI_FILE_VIEW_MENU_SEP_3.Name = Constants.UI_FILE_VIEW_MENU_SEP_3;

            UI_FILE_VIEW_MENU_MOVE_TO_PARENT.Name = Constants.UI_FILE_VIEW_MENU_MOVE_TO_PARENT;
            UI_FILE_VIEW_MENU_MOVE_TO_PARENT.Text = Resources.WorkspaceMenuMoveToParent;
            UI_FILE_VIEW_MENU_MOVE_TO_PARENT.Click += new System.EventHandler(UI_FILE_VIEW_MENU_MOVE_TO_PARENT_Click);

            UI_FILE_VIEW_MENU_SEP_4.Name = Constants.UI_FILE_VIEW_MENU_SEP_4;
            
            UI_FILE_VIEW_MENU_SELECT_SIMILAR.Name = Constants.UI_FILE_VIEW_MENU_SELECT_SIMILAR;
            UI_FILE_VIEW_MENU_SELECT_SIMILAR.Text = Resources.WorkspaceMenuSelectSimilar;
            UI_FILE_VIEW_MENU_SELECT_SIMILAR.Click += new System.EventHandler(UI_FILE_VIEW_MENU_SELECT_SIMILAR_Click);

            UI_FILE_VIEW_MENU_INVERT_SELECTION.Name = Constants.UI_FILE_VIEW_MENU_INVERT_SELECTION;
            UI_FILE_VIEW_MENU_INVERT_SELECTION.Text = Resources.WorkspaceMenuInvertSelection;
            UI_FILE_VIEW_MENU_INVERT_SELECTION.Click += new System.EventHandler(UI_FILE_VIEW_MENU_INVERT_SELECTION_Click);

            _fileViewImageList.TransparentColor = Color.Fuchsia;
            _fileViewImageList.ColorDepth = ColorDepth.Depth24Bit; 
            _fileViewImageList.Images.Add("FOLDER", Resources.Folder);
            _fileViewImageList.Images.Add("DOCUMENT", Resources.Document);
            _fileViewImageList.Images.Add("EXE", Resources.Exe);
            _fileViewImageList.Images.Add("DLL", Resources.Dll);
            _fileViewImageList.Images.Add("PDB", Resources.Pdb);
            _fileViewImageList.Images.Add("TEXT", Resources.Text);
            _fileViewImageList.Images.Add("CSHARP", Resources.CSharp);
            _fileViewImageList.Images.Add("MSIL", Resources.Msil);
            _fileViewImageList.Images.Add("VBNET", Resources.VBNet);
            _fileViewImageList.Images.Add("SCRIPT", Resources.Script);
            _fileViewImageList.Images.Add("IMAGE", Resources.Image);
            _fileViewImageList.Images.Add("HTML", Resources.Html);
            _fileViewImageList.Images.Add("RESOURCE", Resources.Resource);
            _fileViewImageList.Images.Add("CSS", Resources.Css);
            _fileViewImageList.Images.Add("XAML", Resources.Xaml);
            _fileViewImageList.Images.Add("XML", Resources.Xml);
            _fileViewImageList.Images.Add("XSD", Resources.Xsd);
            _fileViewImageList.Images.Add("CPP", Resources.Cpp);
            _fileViewImageList.Images.Add("C", Resources.C);
            _fileViewImageList.Images.Add("H", Resources.H);
            _fileViewImageList.Images.Add("JAVA", Resources.Java);
            _fileViewImageList.Images.Add("OBJ", Resources.Obj);
            _fileViewImageList.Images.Add("ASHX", Resources.Ashx);
            _fileViewImageList.Images.Add("ASCX", Resources.Ascx);
            _fileViewImageList.Images.Add("ASPX", Resources.Aspx);
            _fileViewImageList.Images.Add("SQL", Resources.Sql);
            _fileViewImageList.Images.Add("PROJ", Resources.Proj);
            _fileViewImageList.Images.Add("CAB", Resources.Cabinet);
            _fileViewImageList.Images.Add("DBML", Resources.Dbml);
            _fileViewImageList.Images.Add("DATABASE", Resources.Database);

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(418, 385);
            Controls.Add(_mainListView);
            Controls.Add(_mainToolStrip);
            DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)
                ((((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            HideOnClose = true;
            Name = "WorkspaceForm";
            Icon = Resources.WorkspaceIcon;
            ShowInTaskbar = false;
            Text = Resources.WorkspaceWindowTitle;
            VisibleChanged += delegate { RefreshFileView(); };

            _mainToolStrip.ResumeLayout(false);
            _mainToolStrip.PerformLayout();
            _fileViewMenu.ResumeLayout(false);

            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}