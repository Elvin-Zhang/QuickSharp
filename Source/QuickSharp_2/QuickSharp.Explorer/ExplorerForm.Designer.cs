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
using QuickSharp.Core;

namespace QuickSharp.Explorer
{
    partial class ExplorerForm
    {
        private IContainer _components = null;
        private ToolStrip _mainToolStrip;
        private TreeView _mainTreeView;
        private ToolStripButton _refreshButton;
        private ToolStripButton _parentButton;
        private ToolStripButton _moveButton;
        private ToolStripDropDownButton _visitedFolderSelectButton;
        private ToolStripButton _launchShellButton;
        private ToolStripButton _launchExplorerButton;
        private ToolStripButton _backupButton;
        private ToolStripButton _filterButton;
        private ToolStripDropDownButton _filterSelectButton;
        private ImageList _treeViewImageList;
        private ContextMenuStrip _treeViewMenu;
        private ToolStripMenuItem UI_TREE_MENU_OPEN;
        private ToolStripSeparator UI_TREE_MENU_SEP_1;
        private ToolStripMenuItem UI_TREE_MENU_RENAME;
        private ToolStripMenuItem UI_TREE_MENU_CLONE;
        private ToolStripMenuItem UI_TREE_MENU_DELETE;
        private ToolStripSeparator UI_TREE_MENU_SEP_2;
        private ToolStripMenuItem UI_TREE_MENU_CREATE_FOLDER;
        private ToolStripSeparator UI_TREE_MENU_SEP_3;
        private ToolStripMenuItem UI_TREE_MENU_SET_AS_CURRENT_DIR;

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
            _refreshButton = new ToolStripButton();
            _parentButton = new ToolStripButton();
            _moveButton = new ToolStripButton();
            _visitedFolderSelectButton = new ToolStripDropDownButton();
            _launchExplorerButton = new ToolStripButton();
            _launchShellButton = new ToolStripButton();
            _backupButton = new ToolStripButton();
            _filterButton = new ToolStripButton();
            _filterSelectButton = new ToolStripDropDownButton();
            _mainTreeView = new TreeView();
            _treeViewImageList = new ImageList(_components);
            _treeViewMenu = new ContextMenuStrip(_components);
            UI_TREE_MENU_OPEN = new ToolStripMenuItem();
            UI_TREE_MENU_SEP_1 = new ToolStripSeparator();
            UI_TREE_MENU_RENAME = new ToolStripMenuItem();
            UI_TREE_MENU_CLONE = new ToolStripMenuItem();
            UI_TREE_MENU_DELETE = new ToolStripMenuItem();
            UI_TREE_MENU_SEP_2 = new ToolStripSeparator();
            UI_TREE_MENU_CREATE_FOLDER = new ToolStripMenuItem();
            UI_TREE_MENU_SEP_3 = new ToolStripSeparator();
            UI_TREE_MENU_SET_AS_CURRENT_DIR = new ToolStripMenuItem();

            #endregion

            _mainToolStrip.SuspendLayout();
            _treeViewMenu.SuspendLayout();
            SuspendLayout();

            _mainToolStrip.Items.Add(_refreshButton);
            _mainToolStrip.Items.Add(_parentButton);
            _mainToolStrip.Items.Add(_moveButton);
            _mainToolStrip.Items.Add(_visitedFolderSelectButton);

            if (!_applicationManager.ClientProfile.HaveFlag(
                ClientFlags.ExplorerDisableWindowsShell))
            {
                _mainToolStrip.Items.Add(MenuTools.CreateSeparator(
                    Constants.UI_TOOLBAR_SEP_1));
                _mainToolStrip.Items.Add(_launchExplorerButton);
                _mainToolStrip.Items.Add(_launchShellButton);
            }

            _mainToolStrip.Location = new Point(0, 0);
            _mainToolStrip.Name = "mainToolStrip";
            _mainToolStrip.TabIndex = 0;
            _mainToolStrip.Text = "toolStrip1";

            _refreshButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _refreshButton.Image = Resources.Refresh;
            _refreshButton.ImageTransparentColor = Color.Magenta;
            _refreshButton.Name = Constants.UI_TOOLBAR_REFRESH;
            _refreshButton.Text = Resources.ToolbarRefresh;
            _refreshButton.Click += delegate { _applicationManager.NotifyFileSystemChange(); };

            _parentButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _parentButton.Image = Resources.ParentFolder;
            _parentButton.ImageTransparentColor = Color.Magenta;
            _parentButton.Name = Constants.UI_TOOLBAR_MOVE_PARENT;
            _parentButton.Text = Resources.ToolbarParentFolder;
            _parentButton.Click += delegate { MoveToParent(); };

            _moveButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _moveButton.Image = Resources.OpenFolder;
            _moveButton.ImageTransparentColor = Color.Magenta;
            _moveButton.Name = Constants.UI_TOOLBAR_MOVE_NEW;
            _moveButton.Text = Resources.ToolbarMoveRoot;
            _moveButton.Click += new System.EventHandler(MoveToolStripButton_Click);

            _visitedFolderSelectButton.Name = Constants.UI_TOOLBAR_SELECT_VISITED_FOLDER;
            _visitedFolderSelectButton.Enabled = false;
            _visitedFolderSelectButton.DropDownOpening += delegate { RefreshVisitedFolderList(); };

            _launchExplorerButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _launchExplorerButton.Image = Resources.Explorer;
            _launchExplorerButton.ImageTransparentColor = Color.Fuchsia;
            _launchExplorerButton.Name = Constants.UI_TOOLBAR_LAUNCH_EXPLORER;
            _launchExplorerButton.Text = Resources.ToolbarLaunchExplorer;
            _launchExplorerButton.Click += delegate { LaunchExplorer(); };

            _launchShellButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _launchShellButton.Image = Resources.Console;
            _launchShellButton.ImageTransparentColor = Color.Fuchsia;
            _launchShellButton.Name = Constants.UI_TOOLBAR_LAUNCH_SHELL;
            _launchShellButton.Text = Resources.ToolbarLaunchShell;
            _launchShellButton.Click += delegate { LaunchShell(); };

            _backupButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _backupButton.Image = Resources.FileBackup;
            _backupButton.ImageTransparentColor = Color.Fuchsia;
            _backupButton.Name = Constants.UI_TOOLBAR_BACKUP;
            _backupButton.Text = Resources.ToolbarBackup;
            _backupButton.Click += delegate { BackupExplorer(); };

            _mainToolStrip.Items.Add(MenuTools.CreateSeparator(
                Constants.UI_TOOLBAR_SEP_2));
            _mainToolStrip.Items.Add(_backupButton);

            _filterButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _filterButton.Image = Resources.FilterFiles;
            _filterButton.ImageTransparentColor = Color.Fuchsia;
            _filterButton.Name = Constants.UI_TOOLBAR_FILTER_FILES;
            _filterButton.Text = Resources.ToolbarFilter;
            _filterButton.Click += delegate { ToggleFileFilter(); };

            _filterSelectButton.Name = Constants.UI_TOOLBAR_SELECT_FILTER;
            _filterSelectButton.Enabled = false;
            _filterSelectButton.DropDownOpening += delegate { RefreshFileFilterList(); };

            _mainToolStrip.Items.Add(MenuTools.CreateSeparator(
                Constants.UI_TOOLBAR_SEP_3));
            _mainToolStrip.Items.Add(_filterButton);
            _mainToolStrip.Items.Add(_filterSelectButton);

            _mainTreeView.Dock = DockStyle.Fill;
            _mainTreeView.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            _mainTreeView.ImageList = _treeViewImageList;
            _mainTreeView.Location = new Point(0, 25);
            _mainTreeView.Name = "mainTreeView";
            _mainTreeView.TabIndex = 1;
            _mainTreeView.ContextMenuStrip = _treeViewMenu;

            _treeViewImageList.ColorDepth = ColorDepth.Depth24Bit;
            _treeViewImageList.ImageSize = new Size(16, 16);
            _treeViewImageList.TransparentColor = Color.Fuchsia;
            _treeViewImageList.Images.Add("CLOSEDFOLDER", Resources.ClosedFolder);
            _treeViewImageList.Images.Add("OPENEDFOLDER", Resources.OpenedFolder);
            _treeViewImageList.Images.Add("DOCUMENT", Resources.Document);
            _treeViewImageList.Images.Add("EXE", Resources.Exe);
            _treeViewImageList.Images.Add("DLL", Resources.Dll);
            _treeViewImageList.Images.Add("PDB", Resources.Pdb);
            _treeViewImageList.Images.Add("TEXT", Resources.Text);
            _treeViewImageList.Images.Add("CSHARP", Resources.CSharp);
            _treeViewImageList.Images.Add("MSIL", Resources.Msil);
            _treeViewImageList.Images.Add("VBNET", Resources.VBNet);
            _treeViewImageList.Images.Add("SCRIPT", Resources.Script);
            _treeViewImageList.Images.Add("IMAGE", Resources.Image);
            _treeViewImageList.Images.Add("HTML", Resources.Html);
            _treeViewImageList.Images.Add("RESOURCE", Resources.Resource);
            _treeViewImageList.Images.Add("CSS", Resources.Css);
            _treeViewImageList.Images.Add("XAML", Resources.Xaml);
            _treeViewImageList.Images.Add("XML", Resources.Xml);
            _treeViewImageList.Images.Add("XSD", Resources.Xsd);
            _treeViewImageList.Images.Add("CPP", Resources.Cpp);
            _treeViewImageList.Images.Add("C", Resources.C);
            _treeViewImageList.Images.Add("H", Resources.H);
            _treeViewImageList.Images.Add("JAVA", Resources.Java);
            _treeViewImageList.Images.Add("OBJ", Resources.Obj);
            _treeViewImageList.Images.Add("ASHX", Resources.Ashx);
            _treeViewImageList.Images.Add("ASCX", Resources.Ascx);
            _treeViewImageList.Images.Add("ASPX", Resources.Aspx);
            _treeViewImageList.Images.Add("SQL", Resources.Sql);
            _treeViewImageList.Images.Add("PROJ", Resources.Proj);
            _treeViewImageList.Images.Add("CAB", Resources.Cabinet);
            _treeViewImageList.Images.Add("DBML", Resources.Dbml);
            _treeViewImageList.Images.Add("DATABASE", Resources.Database);

            _treeViewMenu.Opening += new CancelEventHandler(treeViewMenu_Opening);
            _treeViewMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                UI_TREE_MENU_OPEN,
                UI_TREE_MENU_SEP_1,
                UI_TREE_MENU_RENAME,
                UI_TREE_MENU_CLONE,
                UI_TREE_MENU_DELETE,
                UI_TREE_MENU_SEP_2,
                UI_TREE_MENU_CREATE_FOLDER,
                UI_TREE_MENU_SEP_3,
                UI_TREE_MENU_SET_AS_CURRENT_DIR
            });

            UI_TREE_MENU_OPEN.Name = Constants.UI_TREE_MENU_OPEN;
            UI_TREE_MENU_OPEN.Text = Resources.TreeMenuOpen;
            UI_TREE_MENU_OPEN.Image = Resources.OpenFolder;
            UI_TREE_MENU_OPEN.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            UI_TREE_MENU_OPEN.ShortcutKeyDisplayString = Resources.EnterKey;
            UI_TREE_MENU_OPEN.Click += new System.EventHandler(UI_TREE_MENU_OPEN_Click);

            UI_TREE_MENU_SEP_1.Name = Constants.UI_TREE_MENU_SEP_1;

            UI_TREE_MENU_RENAME.Name = Constants.UI_TREE_MENU_RENAME;
            UI_TREE_MENU_RENAME.Text = Resources.TreeMenuRename;
            UI_TREE_MENU_RENAME.ShortcutKeys = Keys.F2;
            UI_TREE_MENU_RENAME.Click += new System.EventHandler(UI_TREE_MENU_RENAME_Click);

            UI_TREE_MENU_CLONE.Name = Constants.UI_TREE_MENU_CLONE;
            UI_TREE_MENU_CLONE.Text = Resources.TreeMenuClone;
            UI_TREE_MENU_CLONE.Click += new System.EventHandler(UI_TREE_MENU_CLONE_Click);

            UI_TREE_MENU_DELETE.Name = Constants.UI_TREE_MENU_DELETE;
            UI_TREE_MENU_DELETE.Text = Resources.TreeMenuDelete;
            UI_TREE_MENU_DELETE.ShortcutKeys = Keys.Delete;
            UI_TREE_MENU_DELETE.Click += new System.EventHandler(UI_TREE_MENU_DELETE_Click);

            UI_TREE_MENU_SEP_1.Name = Constants.UI_TREE_MENU_SEP_1;

            UI_TREE_MENU_CREATE_FOLDER.Name = Constants.UI_TREE_MENU_CREATE_FOLDER;
            UI_TREE_MENU_CREATE_FOLDER.Text = Resources.TreeMenuCreateFolder;
            UI_TREE_MENU_CREATE_FOLDER.Image = Resources.NewFolder;
            UI_TREE_MENU_CREATE_FOLDER.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            UI_TREE_MENU_CREATE_FOLDER.Click += new System.EventHandler(UI_TREE_MENU_CREATE_FOLDER_Click);

            UI_TREE_MENU_SEP_3.Name = Constants.UI_TREE_MENU_SEP_3;

            UI_TREE_MENU_SET_AS_CURRENT_DIR.Name = Constants.UI_TREE_MENU_SET_AS_CURRENT_DIR;
            UI_TREE_MENU_SET_AS_CURRENT_DIR.Text = Resources.TreeMenuSetAsCurrentDir;
            UI_TREE_MENU_SET_AS_CURRENT_DIR.Click += new System.EventHandler(UI_TREE_MENU_SET_AS_CURRENT_DIR_Click);

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_mainTreeView);
            Controls.Add(_mainToolStrip);
            DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)
                (((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom));
            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            HideOnClose = true;
            Name = "ExplorerForm";
            ShowInTaskbar = false;
            Text = "ExplorerForm";
            _mainToolStrip.ResumeLayout(false);
            _mainToolStrip.PerformLayout();
            _treeViewMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}