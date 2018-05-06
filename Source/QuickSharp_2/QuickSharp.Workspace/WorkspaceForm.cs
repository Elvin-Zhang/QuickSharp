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
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using QuickSharp.Core;
using WeifenLuo.WinFormsUI.Docking;
using ICSharpCode.SharpZipLib.Zip;

namespace QuickSharp.Workspace
{
    /// <summary>
    /// The workspace window form.
    /// </summary>
    public partial class WorkspaceForm : BaseDockedForm
    {
        private ApplicationManager _applicationManager;
        private SettingsManager _settingsManager;
        private Dictionary<String, String> _documentTypeIconMap;
        private List<String> _displayedTypesList;
        private List<String> _visitedFoldersList;
        private int _visitedFoldersLimit;
        private bool _showSourceOnly;
        private bool _enableTitleBarUpdate;

        /// <summary>
        /// Event raised before the file view is refreshed.
        /// </summary>
        public event MessageHandler FileViewPreRefresh;

        /// <summary>
        /// Event raised after the file view has been refreshed.
        /// </summary>
        public event MessageHandler FileViewPostRefresh;

        /// <summary>
        /// Event raised when a list view item has been added to the view during the refresh.
        /// </summary>
        public event ListViewItemUpdateHandler FileViewItemUpdate;

        /// <summary>
        /// Create the workspace form.
        /// </summary>
        /// <param name="formKey">The GUID key used to identify the form internally.</param>
        public WorkspaceForm(string formKey) : base(formKey)
        {
            _applicationManager = ApplicationManager.GetInstance();
            _settingsManager = SettingsManager.GetInstance();

            /*
             * The workspace shows a view of the file system so register
             * to be notified if any plugin makes file system changes.
             */

            _applicationManager.FileSystemChange +=
                new MessageHandler(RefreshFileView);

            /* 
             * Associate the appropriate filetype image with appropriate
             * supported file extensions. 
             */

            #region Document Type Images

            /*
             * The workspace presents icons for file types it knows about.
             * Note these are purely for presentation and are not related
             * to document types; an extension included here is not
             * necessarily a registered document type and registered
             * documents might not be in this list.
             * 
             * The image names here must match the text keys used in
             * the tree view control's image list.
             */

            _documentTypeIconMap = new Dictionary<String, String>();
            _documentTypeIconMap.Add(".asax", "ASHX");
            _documentTypeIconMap.Add(".ascx", "ASCX");
            _documentTypeIconMap.Add(".ashx", "ASHX");
            _documentTypeIconMap.Add(".asmx", "ASHX");
            _documentTypeIconMap.Add(".aspx", "ASPX");
            _documentTypeIconMap.Add(".bat", "SCRIPT");
            _documentTypeIconMap.Add(".bmp", "IMAGE");
            _documentTypeIconMap.Add(".c", "C");
            _documentTypeIconMap.Add(".cc", "CPP");
            _documentTypeIconMap.Add(".class", "OBJ");
            _documentTypeIconMap.Add(".cmd", "SCRIPT");
            _documentTypeIconMap.Add(".conf", "TEXT");
            _documentTypeIconMap.Add(".config", "XML");
            _documentTypeIconMap.Add(".cpp", "CPP");
            _documentTypeIconMap.Add(".cs", "CSHARP");
            _documentTypeIconMap.Add(".css", "CSS");
            _documentTypeIconMap.Add(".cxx", "CPP");
            _documentTypeIconMap.Add(".dbml", "DBML");
            _documentTypeIconMap.Add(".dll", "DLL");
            _documentTypeIconMap.Add(".erb", "HTML");
            _documentTypeIconMap.Add(".exe", "EXE");
            _documentTypeIconMap.Add(".gif", "IMAGE");
            _documentTypeIconMap.Add(".h", "H");
            _documentTypeIconMap.Add(".hh", "H");
            _documentTypeIconMap.Add(".hpp", "H");
            _documentTypeIconMap.Add(".htm", "HTML");
            _documentTypeIconMap.Add(".html", "HTML");
            _documentTypeIconMap.Add(".ico", "IMAGE");
            _documentTypeIconMap.Add(".il", "MSIL");
            _documentTypeIconMap.Add(".ini", "TEXT");
            _documentTypeIconMap.Add(".java", "JAVA");
            _documentTypeIconMap.Add(".jpeg", "IMAGE");
            _documentTypeIconMap.Add(".jpg", "IMAGE");
            _documentTypeIconMap.Add(".js", "SCRIPT");
            _documentTypeIconMap.Add(".lua", "SCRIPT");
            _documentTypeIconMap.Add(".master", "ASPX");
            _documentTypeIconMap.Add(".o", "OBJ");
            _documentTypeIconMap.Add(".obj", "OBJ");
            _documentTypeIconMap.Add(".pdb", "PDB");
            _documentTypeIconMap.Add(".php", "HTML");
            _documentTypeIconMap.Add(".pl", "SCRIPT");
            _documentTypeIconMap.Add(".png", "IMAGE");
            _documentTypeIconMap.Add(".proj", "PROJ");
            _documentTypeIconMap.Add(".py", "SCRIPT");
            _documentTypeIconMap.Add(".rb", "SCRIPT");
            _documentTypeIconMap.Add(".resx", "RESOURCE");
            _documentTypeIconMap.Add(".rhtml", "HTML");
            _documentTypeIconMap.Add(".sdf", "DATABASE");
            _documentTypeIconMap.Add(".sitemap", "XML");
            _documentTypeIconMap.Add(".sql", "SQL");
            _documentTypeIconMap.Add(".sqlite", "DATABASE");
            _documentTypeIconMap.Add(".txt", "TEXT");
            _documentTypeIconMap.Add(".vb", "VBNET");
            _documentTypeIconMap.Add(".wsdl", "XML");
            _documentTypeIconMap.Add(".xaml", "XAML");
            _documentTypeIconMap.Add(".xap", "CAB");
            _documentTypeIconMap.Add(".xml", "XML");
            _documentTypeIconMap.Add(".xsd", "XSD");
            _documentTypeIconMap.Add(".xsl", "XML");
            _documentTypeIconMap.Add(".yml", "TEXT");
            _documentTypeIconMap.Add(".zip", "CAB");

            #endregion

            /*
             * The file view can be restricted to certain types
             * only. If this list is empty show all files; if not
             * show only those that have extensions in the list.
             */

            _displayedTypesList = new List<String>();

            // This will be overridden if ShowSource disabled by client flag.
            _showSourceOnly = _settingsManager.ShowSourceOnly;

            /*
             * Maintain a list of folders visited using the move workspace dialog.
             */

            _visitedFoldersList = _settingsManager.VisitedFoldersList;
            _visitedFoldersLimit = _settingsManager.VisitedFoldersLimit;

            /*
             * Enable the title bar update.
             */

            _enableTitleBarUpdate = _applicationManager.ClientProfile.
                HaveFlag(ClientFlags.WorkspaceEnableTitleBarUpdate);

            /*
             * Update the view once all the plugins have registered
             * their source types.
             */

            PluginManager.GetInstance().PluginPostActivate += delegate
            {
                UpdateDisplayedTypesList();
                RefreshFileView();
            };

            /*
             * Setup the main form.
             */

            InitForm();

            _mainListView.AllowDrop = true;
            _mainListView.ItemDrag += new ItemDragEventHandler(MainListView_ItemDrag);
            _mainListView.DragOver += new DragEventHandler(MainListView_DragOver);
            _mainListView.DragDrop += new DragEventHandler(MainListView_DragDrop);

            mainForm.ThemeActivated += 
                new MessageHandler(MainForm_ThemeActivated);

            mainForm.FormClosed +=
                new FormClosedEventHandler(MainForm_FormClosed);

            /*
             * Reset the MainForm title if the Workspace is hidden.
             */

            if (_enableTitleBarUpdate)
            {
                VisibleChanged += delegate
                {
                    if (!Visible)
                        mainForm.Text = _applicationManager.
                            ClientProfile.ClientTitle;
                };
            }

            /*
             * Allow updates to be requested from the settings manager.
             */

            _settingsManager.OnUpdateUI +=
                new MessageHandler(RefreshFileView);
        }

        /// <summary>
        /// Set the form's initial state when there is no saved configuration
        /// to be restored from the previous session.
        /// </summary>
        protected override void SetFormDefaultState()
        {
            ClientProfile profile = _applicationManager.ClientProfile;

            if (profile.HaveFlag(ClientFlags.WorkspaceDockLeftByDefault))
                DockState = DockState.DockLeft;
            else
                DockState = DockState.DockRight;

            if (profile.HaveFlag(ClientFlags.WorkspaceHideByDefault))
                Hide();
            else
                Show();
        }

        #region MainForm Events

        private void MainForm_ThemeActivated()
        {
            ThemeFlags flags = _applicationManager.ClientProfile.ThemeFlags;

            if (flags != null)
            {
                if (flags.MainBackColor != Color.Empty)
                    BackColor = flags.MainBackColor;

                if (flags.ViewBackColor != Color.Empty)
                    FileView.BackColor = flags.ViewBackColor;

                if (flags.ViewForeColor != Color.Empty)
                    FileView.ForeColor = flags.ViewForeColor;

                if (flags.ViewShowBorder == false)
                    _mainListView.BorderStyle = BorderStyle.None;

                if (flags.MenuForeColor != Color.Empty)
                    MenuTools.ContextMenuStripItemsSetForeColor(
                        FileViewContextMenu, 
                        flags.MenuForeColor,
                        flags.MenuHideImages);
            }

            SetBackgroundImage();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                _settingsManager.CurrentWorkspace = Directory.GetCurrentDirectory();
                _settingsManager.ShowSourceOnly = _showSourceOnly;
                _settingsManager.Save();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Public UI Elements

        /// <summary>
        /// The workspace toolbar.
        /// </summary>
        public ToolStrip Toolbar
        {
            get { return _mainToolStrip; }
        }


        /// <summary>
        /// The file view.
        /// </summary>
        public ListView FileView
        {
            get { return _mainListView; }
        }

        /// <summary>
        /// The file view context menu.
        /// </summary>
        public ContextMenuStrip FileViewContextMenu
        {
            get { return _fileViewMenu; }
        }

        /// <summary>
        /// The image list used to provide icons for the document types displayed in the file view.
        /// </summary>
        public ImageList DocumentImageList
        {
            get { return _fileViewImageList; }
        }

        /// <summary>
        /// A dictionary mapping file types to the names of the images used to represent them.
        /// The names are the keys of the images in the image list.
        /// </summary>
        public Dictionary<String, String> DocumentTypeIconMap
        {
            get { return _documentTypeIconMap; }
        }

        /// <summary>
        /// A list of the document types displayed by the file view. Used to filter out
        /// non-source file types when the 'show source only' view is selected.
        /// </summary>
        public List<String> DisplayedTypesList
        {
            get { return _displayedTypesList; }
        }

        #endregion

        #region File View

        /// <summary>
        /// Refresh the file view.
        /// </summary>
        public void RefreshFileView()
        {
            if (!Visible) return;

            _mainListView.BeginUpdate();

            if (FileViewPreRefresh != null)
                FileViewPreRefresh();

            List<string> selectedItemPaths = new List<string>();
            foreach (ListViewItem item in _mainListView.SelectedItems)
                selectedItemPaths.Add(item.Tag as string);

            string topItemPath = null;
            if (_mainListView.TopItem != null)
                topItemPath = _mainListView.TopItem.Tag as string;

            _mainListView.Items.Clear();

            string rootPath = Directory.GetCurrentDirectory();
            string rootName = Path.GetFileName(rootPath);
            if (rootName == String.Empty)
                rootName = Path.GetPathRoot(rootPath);

            if (_enableTitleBarUpdate)
            {
                // Update the main window title.
                mainForm.Text = String.Format("{0} - {1}",
                    _settingsManager.ShowFullPath ? rootPath : rootName,
                    _applicationManager.ClientProfile.ClientTitle);
            }

            bool hideHiddenFiles = !_settingsManager.ShowHiddenFiles;
            bool hideSystemFiles = !_settingsManager.ShowSystemFiles;

            foreach (string folder in Directory.GetDirectories(rootPath))
            {
                DirectoryInfo di = new DirectoryInfo(folder);

                if (hideHiddenFiles &&
                    ((di.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden))
                    continue;
                
                if (hideSystemFiles &&
                    ((di.Attributes & FileAttributes.System) == FileAttributes.System))
                    continue;

                ListViewItem item = new ListViewItem();
                item.Name = di.Name;
                item.Text = " " + di.Name;
                item.Tag = di.FullName;
                item.ImageKey = Constants.FOLDER_IMAGE;

                if (selectedItemPaths.Contains(folder))
                    item.Selected = true;

                /*
                 * Allow item to be modified before it is added
                 * to the list.
                 */

                if (FileViewItemUpdate != null)
                    FileViewItemUpdate(ref item);

                if (item != null)
                    _mainListView.Items.Add(item);
            }

            foreach (string file in Directory.GetFiles(rootPath, "*.*"))
            {
                FileInfo fi = new FileInfo(file);

                if (hideHiddenFiles &&
                    ((fi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden))
                    continue;

                if (hideSystemFiles &&
                    ((fi.Attributes & FileAttributes.System) == FileAttributes.System))
                    continue;

                if (!ShowFile(fi.Extension)) continue;

                ListViewItem item = new ListViewItem();
                item.Name = fi.Name;
                item.Text = " " + fi.Name;
                item.Tag = fi.FullName;

                DocumentType documentType =
                    new DocumentType(fi.Extension);

                if (_documentTypeIconMap.ContainsKey(documentType.ToString()))
                    item.ImageKey = _documentTypeIconMap[documentType.ToString()];
                else
                    item.ImageKey = Constants.DOCUMENT_IMAGE;

                if (selectedItemPaths.Contains(file))
                    item.Selected = true;

                /*
                 * Allow item to be modified before it is added
                 * to the list.
                 */

                if (FileViewItemUpdate != null)
                    FileViewItemUpdate(ref item);

                if (item != null)
                    _mainListView.Items.Add(item);
            }

            if (topItemPath != null)
            {
                foreach (ListViewItem item in _mainListView.Items)
                {
                    string itemPath = item.Tag as string;
                    if (itemPath == topItemPath)
                    {
                        _mainListView.TopItem = item;
                        item.EnsureVisible();
                        break;
                    }
                }
            }

            _mainListView.AutoResizeColumns(
                ColumnHeaderAutoResizeStyle.ColumnContent);

            if (FileViewPostRefresh != null)
                FileViewPostRefresh();

            // Update the visited folder list button state.
            _visitedFolderDropDownButton.Enabled = 
                _visitedFoldersList.Count > 0;

            _mainListView.EndUpdate();
        }

        private void ClearSelectedItems()
        {
            foreach (ListViewItem item in _mainListView.Items)
                item.Selected = false;
        }

        private bool ShowFile(string ext)
        {
            if (_displayedTypesList.Count == 0)
                return true;
            else
                return _displayedTypesList.Contains(ext);
        }

        private void MainListView_MouseDoubleClick(
            object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ListView.SelectedListViewItemCollection items =
                    _mainListView.SelectedItems;

                if (items.Count > 0)
                    OpenItem(items[0]);
            }
        }

        private void MainListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (_mainListView.SelectedItems.Count == 1)
                {
                    OpenItem(_mainListView.SelectedItems[0]);
                }
                else
                {
                    foreach (ListViewItem item in _mainListView.SelectedItems)
                        if (!ItemIsFolder(item))
                            OpenItem(item);
                }
            }
            else if (e.KeyCode == Keys.A && e.Control)
            {
                foreach (ListViewItem item in _mainListView.Items)
                    item.Selected = true;
            }
        }

        private void MainListView_LostFocus(object sender, EventArgs e)
        {
            _mainListView.SelectedItems.Clear();
        }

        private void MainListView_ItemSelectionChanged(
            object sender, ListViewItemSelectionChangedEventArgs e)
        {
            UI_FILE_VIEW_MENU_OPEN.Enabled = false;
            UI_FILE_VIEW_MENU_RENAME.Enabled = false;
            UI_FILE_VIEW_MENU_DELETE.Enabled = false;

            ListView.SelectedListViewItemCollection items =
                _mainListView.SelectedItems;

            if (items.Count == 0) return;

            UI_FILE_VIEW_MENU_OPEN.Enabled = true;
            UI_FILE_VIEW_MENU_RENAME.Enabled = true;
            UI_FILE_VIEW_MENU_DELETE.Enabled = true;
        }

        #endregion

        #region File View Context Menu Events

        private void UI_FILE_VIEW_MENU_OPEN_Click(
            object sender, EventArgs e)
        {
            foreach (ListViewItem item in _mainListView.SelectedItems)
                OpenItem(item);
        }

        private void UI_FILE_VIEW_MENU_RENAME_Click(
            object sender, EventArgs e)
        {
            if (_mainListView.SelectedItems.Count > 0)
                RenameItem(_mainListView.SelectedItems[0]);
        }

        private void UI_FILE_VIEW_MENU_CLONE_Click(
            object sender, EventArgs e)
        {
            foreach (ListViewItem item in _mainListView.SelectedItems)
                CloneItem(item);
        }

        private void UI_FILE_VIEW_MENU_DELETE_Click(
            object sender, EventArgs e)
        {
            foreach (ListViewItem item in _mainListView.SelectedItems)
                DeleteItem(item);
        }

        private void UI_FILE_VIEW_MENU_CREATE_FOLDER_Click(
            object sender, EventArgs e)
        {
            CreateFolder();
        }

        private void UI_FILE_VIEW_MENU_MOVE_TO_PARENT_Click(
            object sender, EventArgs e)
        {
            if (_mainListView.SelectedItems.Count > 0)
                MoveItemToParentFolder(_mainListView.SelectedItems[0]);
        }

        private void UI_FILE_VIEW_MENU_SELECT_SIMILAR_Click(
            object sender, EventArgs e)
        {
            if (_mainListView.SelectedItems.Count != 1) return;
            
            string path = _mainListView.SelectedItems[0].Tag as string;
            
            if (path != null)
            {
                string ext = Path.GetExtension(path);

                foreach (ListViewItem item in _mainListView.Items)
                {
                    string itemPath = item.Tag as string;
                    if (itemPath != null)
                    {
                        if (Path.GetExtension(itemPath) == ext)
                            item.Selected = true;
                    }
                }
            }
        }

        private void UI_FILE_VIEW_MENU_INVERT_SELECTION_Click(
            object sender, EventArgs e)
        {
            foreach (ListViewItem item in _mainListView.Items)
                item.Selected = !item.Selected;
        }

        private void listViewMenu_Opening(object sender, CancelEventArgs e)
        {
            UI_FILE_VIEW_MENU_OPEN.Enabled = false;
            UI_FILE_VIEW_MENU_RENAME.Enabled = false;
            UI_FILE_VIEW_MENU_CLONE.Enabled = false;
            UI_FILE_VIEW_MENU_DELETE.Enabled = false;
            UI_FILE_VIEW_MENU_CREATE_FOLDER.Enabled = true;
            UI_FILE_VIEW_MENU_MOVE_TO_PARENT.Enabled = false;
            UI_FILE_VIEW_MENU_SELECT_SIMILAR.Enabled = false;
            UI_FILE_VIEW_MENU_INVERT_SELECTION.Enabled = false;

            if (_mainListView.Items.Count > 0)
                UI_FILE_VIEW_MENU_INVERT_SELECTION.Enabled = true;
            
            ListView.SelectedListViewItemCollection selectedItems =
                _mainListView.SelectedItems;

            if (selectedItems.Count == 0) return;

            UI_FILE_VIEW_MENU_DELETE.Enabled = true;
            UI_FILE_VIEW_MENU_CREATE_FOLDER.Enabled = false;

            if (selectedItems.Count == 1)
            {
                UI_FILE_VIEW_MENU_OPEN.Enabled = true;
                UI_FILE_VIEW_MENU_RENAME.Enabled = true;

                if (Directory.GetParent(Directory.GetCurrentDirectory()) != null)
                    UI_FILE_VIEW_MENU_MOVE_TO_PARENT.Enabled = true;

                if (!ItemIsFolder(selectedItems[0]))
                {
                    UI_FILE_VIEW_MENU_CLONE.Enabled = true;
                    UI_FILE_VIEW_MENU_SELECT_SIMILAR.Enabled = true;
                }
            }
            else
            {
                if (!HaveFolderInCollection(selectedItems))
                {
                    UI_FILE_VIEW_MENU_OPEN.Enabled = true;
                    UI_FILE_VIEW_MENU_CLONE.Enabled = true;
                }
            }
        }

        private bool HaveFolderInCollection(
            ListView.SelectedListViewItemCollection items)
        {
            foreach (ListViewItem item in items)
                if (ItemIsFolder(item)) return true;

            return false;
        }

        #endregion

        #region File System Operations

        private bool ItemIsFolder(ListViewItem item)
        {
            return (item.ImageKey == Constants.FOLDER_IMAGE);
        }

        private void OpenItem(ListViewItem item)
        {
            string path = item.Tag as string;
            if (path == null) return;

            if (ItemIsFolder(item))
            {
                Directory.SetCurrentDirectory(path);
                _applicationManager.NotifyFileSystemChange();
            }
            else
            {
                mainForm.LoadDocumentIntoWindow(path, true);
            }
        }

        private void RenameItem(ListViewItem item)
        {
            RenameForm rf = new RenameForm();
            rf.NewName = item.Name;

            if (rf.ShowDialog() == DialogResult.OK)
            {
                String newName = rf.NewName.Trim();
                if (newName == String.Empty) return;
                if (newName == item.Name) return;

                string oldPath = item.Tag as string;
                if (oldPath == null) return;

                string newPath = String.Empty;

                try
                {
                    if (ItemIsFolder(item))
                    {
                        newPath = FileTools.ChangeDirectoryName(
                            oldPath, newName);

                        /*
                         * Update the paths of any open editors.
                         */

                        string oldPathLC = oldPath.ToLower();

                        foreach (Document d in mainForm.ClientWindow.Documents)
                        {
                            if (d.FilePath != null &&
                                d.FilePath.ToLower().StartsWith(oldPathLC,
                                    StringComparison.CurrentCultureIgnoreCase))
                            {
                                d.FilePath = newPath + d.FilePath.Substring(oldPath.Length);
                            }
                        }
                    }
                    else
                    {
                        newPath = FileTools.ChangeFileName(
                            oldPath, newName);

                        /*
                         * Update the filename in any open editors.
                         */

                        foreach (Document d in mainForm.ClientWindow.Documents)
                        {
                            if (d.FilePath != null &&
                                FileTools.MatchPaths(d.FilePath, oldPath))
                            {
                                d.FileName = newName;
                                d.FilePath = newPath;
                            }
                        }
                    }

                    item.Tag = newPath; // for refresh
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0}\r\n{1}",
                            Resources.RenameErrorMessage,
                            ex.Message),
                        Resources.RenameErrorTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                _applicationManager.NotifyFileSystemChange();
            }
        }

        private void CloneItem(ListViewItem item)
        {
            try
            {
                string filename = item.Name;
                string source = Path.Combine(
                    Directory.GetCurrentDirectory(), filename);

                int i = 1;
                string filebase = Path.GetFileNameWithoutExtension(filename);
                string extension = Path.GetExtension(filename);

                while (File.Exists(filename))
                    filename = String.Format(
                        "{0}_{1}{2}", filebase, i++, extension);

                string destination = Path.Combine(
                    Directory.GetCurrentDirectory(), filename);

                File.Copy(source, destination);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}\r\n{1}",
                        Resources.CloneErrorMessage,
                        ex.Message),
                    Resources.CloneErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                _applicationManager.NotifyFileSystemChange();
            }
        }

        private void DeleteItem(ListViewItem item)
        {
            try
            {
                string path = item.Tag as string;
                if (path == null) return;

                FileTools.DeleteWithUndo(path);

                if (ItemIsFolder(item)) return;

                /*
                 * See if we have the file in any open editors.
                 */

                List<Document> documents = new List<Document>();

                foreach (Document d in mainForm.ClientWindow.Documents)
                    if (d.FilePath != null &&
                        FileTools.MatchPaths(d.FilePath, path))
                        documents.Add(d);

                if (documents.Count > 0)
                {
                    if (MessageBox.Show(
                        String.Format(
                            Resources.DeleteFileOpenMessage,
                            documents[0].FileName),
                        Resources.DeleteErrorTitle,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        foreach (Document d in documents)
                            d.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}\r\n{1}",
                        Resources.DeleteErrorMessage,
                        ex.Message),
                    Resources.DeleteErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                _applicationManager.NotifyFileSystemChange();
            }
        }

        private void CreateFolder()
        {
            string name = Resources.NewFolderName;
            int i = 1;
            while (Directory.Exists(name))
                name = String.Format("{0} ({1})",
                    Resources.NewFolderName, i++);

            RenameForm rf = new RenameForm();
            rf.NewName = name;
            rf.Title = Resources.WorkspaceNewFolder;

            if (rf.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    String folderName = rf.NewName.Trim();
                    Directory.CreateDirectory(folderName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0}\r\n{1}",
                            Resources.NewFolderErrorMessage,
                            ex.Message),
                        Resources.NewFolderErrorTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                _applicationManager.NotifyFileSystemChange();
            }
        }

        private void MoveItemToParentFolder(ListViewItem item)
        {
            try
            {
                DirectoryInfo parent = Directory.GetParent(
                    Directory.GetCurrentDirectory());
                if (parent == null) return;

                string source = item.Tag as string;
                if (source == null) return;

                string destination = 
                    Path.Combine(parent.FullName, item.Name);

                if (ItemIsFolder(item))
                    Directory.Move(source, destination);
                else
                    File.Move(source, destination);

                /*
                 * Update the paths of any open editors.
                 */

                string sourceLC = source.ToLower();

                foreach (Document d in mainForm.ClientWindow.Documents)
                {
                    if (d.FilePath != null &&
                        d.FilePath.ToLower().StartsWith(sourceLC,
                            StringComparison.CurrentCultureIgnoreCase))
                    {
                        d.FilePath = destination + d.FilePath.Substring(source.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}\r\n{1}",
                        Resources.MoveErrorMessage,
                        ex.Message),
                    Resources.MoveErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            _applicationManager.NotifyFileSystemChange();
        }


        #endregion

        #region Toolbar Operations

        /// <summary>
        /// Move the workspace to the parent directory.
        /// </summary>
        public void MoveToParent()
        {
            Directory.SetCurrentDirectory("..");
            _applicationManager.NotifyFileSystemChange();
        }

        /// <summary>
        /// Move the workspace to a directory selected from a FolderBrowserDialog.
        /// </summary>
        public void MoveWorkspace()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = Resources.BrowseFolderDialogText;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Directory.SetCurrentDirectory(fbd.SelectedPath);
                    AddFolderToVisitedList(fbd.SelectedPath);
                    _applicationManager.NotifyFileSystemChange();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0}\r\n{1}",
                            Resources.MoveWorkspaceErrorMessage,
                            ex.Message),
                        Resources.MoveWorkspaceErrorTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /*
         * Display the list of visited folders as a drop-down menu.
         * Some folders could be invalid so they are removed as the menu is
         * created. If the list only contains invalid folders the dropdown
         * button will become disabled when clicked. This will look a little
         * odd to the user but we can only avoid it by constantly checking if
         * the folders exist. This is too great an IO burden for a minor
         * edge-case so we just ignore it and except some slightly quirky
         * behaviour.
         */

        /// <summary>
        /// Update the visited folder menu.
        /// </summary>
        public void RefreshVisitedFolderList()
        {
            _visitedFolderDropDownButton.DropDownItems.Clear();
            _visitedFolderDropDownButton.Enabled = false;

            // Create a new list of only the folders that exist.
            List<String> validFoldersList = new List<String>();

            ThemeFlags flags = _applicationManager.
                ClientProfile.ThemeFlags;

            bool applyTheme = flags != null &&
                flags.MenuForeColor != Color.Empty;

            foreach (string folder in _visitedFoldersList)
            {
                if (!Directory.Exists(folder)) continue;

                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = Path.GetFileName(folder);
                if (item.Text == String.Empty) item.Text = folder;
                item.ToolTipText = folder;
                item.Click += new EventHandler(VisitedFolderDropDownItem_Click);

                // Apply theme color if available.
                if (applyTheme) item.ForeColor = flags.MenuForeColor;

                // Insert so that latest file appears first.
                _visitedFolderDropDownButton.DropDownItems.Insert(0, item);

                validFoldersList.Add(folder); 
            }

            if (_visitedFolderDropDownButton.DropDownItems.Count > 0)
                _visitedFolderDropDownButton.Enabled = true;

            // Retain the updated list.
            _visitedFoldersList = validFoldersList;
        }

        private void AddFolderToVisitedList(string path)
        {
            if (_visitedFoldersList.Contains(path))
                _visitedFoldersList.Remove(path);

            _visitedFoldersList.Add(path);

            if (_visitedFoldersList.Count > _visitedFoldersLimit)
                _visitedFoldersList.RemoveAt(0);
        }

        private void RemoveFolderFromVisitedList(string path)
        {
            if (_visitedFoldersLimit > 0 &&
                _visitedFoldersList.Contains(path))
                _visitedFoldersList.Remove(path);

            if (_visitedFoldersList.Count == 0)
                _visitedFolderDropDownButton.Enabled = false;
        }

        private void VisitedFolderDropDownItem_Click(
            object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null) return;

            string path = item.ToolTipText;

            if (Directory.Exists(path))
            {
                // Do nothing if we're already there.
                if (Directory.GetCurrentDirectory() != path)
                {
                    Directory.SetCurrentDirectory(path);

                    // Add again so that item move to top of menu.
                    AddFolderToVisitedList(path);

                    _applicationManager.NotifyFileSystemChange();
                }
            }
            else
            {
                RemoveFolderFromVisitedList(path);
            }
        }

        /// <summary>
        /// Backup the workspace files to a zip archive.
        /// </summary>
        public void BackupWorkspace()
        {
            String sourceDirectory = Directory.GetCurrentDirectory();

            /*
             * Validate the source directory - user's need to use
             * common sense, i.e. not selecting huge folder structures
             * for backing up. We can trap the most obvious case
             * of trying to backup a whole disk.
             */

            if (String.IsNullOrEmpty(Path.GetFileName(sourceDirectory)))
            {
                MessageBox.Show(
                    Resources.WorkspaceBackupDialogNoDisk,
                    Resources.WorkspaceBackupDialogTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            /*
             * Get the archive file name.
             */

            String zipFileName = GetZipFileName(sourceDirectory);
            if (zipFileName == null) return;

            try
            {
                mainForm.Cursor = Cursors.WaitCursor;

                FastZip zip = new FastZip();
                zip.CreateEmptyDirectories = true;
                zip.CreateZip(zipFileName, sourceDirectory, true, ".");

                _applicationManager.NotifyFileSystemChange();

                MessageBox.Show(
                    String.Format("{0}:\r\n{1}",
                        Resources.WorkspaceBackupDialogSuccess,
                        zipFileName),
                    Resources.WorkspaceBackupDialogTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                try
                {
                    if (File.Exists(zipFileName))
                        File.Delete(zipFileName);
                }
                catch
                {
                    // Suppress this - not much to do to correct and
                    // user will already get an error message from the
                    // outer exception.
                }

                MessageBox.Show(
                    String.Format("{0}:\r\n{1}",
                        Resources.WorkspaceBackupDialogError,
                        ex.Message),
                    Resources.WorkspaceBackupDialogTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                mainForm.Cursor = Cursors.Default;
            }
        }

        private String GetZipFileName(String sourceDirectory)
        {
            /*
             * Check user settings for destination folder.
             */

            String userHome = _settingsManager.BackupDirectory;

            if (!Directory.Exists(userHome))
                userHome = String.Empty;

            /*
             * If no path request one from the user.
             */

            if (userHome == String.Empty)
            {
                userHome = Environment.GetFolderPath(
                    Environment.SpecialFolder.MyDocuments);

                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.SelectedPath = userHome;
                fbd.Description = Resources.WorkspaceBackupFolderMessage;

                if (fbd.ShowDialog() == DialogResult.Cancel)
                    return null;

                userHome = fbd.SelectedPath;
            }

            /*
             * Construct a name from the workspace name, date and time.
             */

            String baseName = Path.GetFileName(sourceDirectory);
            if (String.IsNullOrEmpty(baseName)) return null;

            DateTime now = DateTime.Now;

            String fileName = String.Format(
                "{0}_{1:D4}{2:D2}{3:D2}{4:D2}{5:D2}{6:D2}.zip",
                baseName, now.Year, now.Month, now.Day,
                now.Hour, now.Minute, now.Second);

            return Path.Combine(userHome, fileName);
        }

        /// <summary>
        /// Launch the windows shell in the currrent directory.
        /// </summary>
        public void LaunchShell()
        {
            string shellPath = Path.Combine(
                Environment.SystemDirectory, "cmd.exe");

            if (!File.Exists(shellPath))
            {
                MessageBox.Show(String.Format(
                        Resources.ShellErrorMessage,
                        "\r\n", shellPath),
                    Resources.ShellErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            FileTools.LaunchApplication(true, shellPath, "/D/K PROMPT $N:$G&&CD");
        }

        /// <summary>
        /// Launch windows explorer in the current directory.
        /// </summary>
        public void LaunchExplorer()
        {
            string explorerPath = Path.Combine(
                Environment.SystemDirectory, "..\\explorer.exe");

            if (!File.Exists(explorerPath))
            {
                MessageBox.Show(String.Format(
                        Resources.ExplorerErrorMessage,
                        "\r\n", explorerPath),
                    Resources.ExplorerErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            FileTools.LaunchApplication(false, explorerPath, ".");
        }

        /// <summary>
        /// Toggle between showing all files and just those registered as source-file
        /// types.
        /// </summary>
        public void ShowSourceOnly()
        {
            _showSourceOnly = !_showSourceOnly;
            _showSourceOnlyButton.Checked = _showSourceOnly;

            UpdateDisplayedTypesList();
            RefreshFileView();
        }

        /// <summary>
        /// Update the list of displayed file types. Used to toggle between showing
        /// all files and source-types only.
        /// </summary>
        public void UpdateDisplayedTypesList()
        {
            DisplayedTypesList.Clear();

            if (_showSourceOnly)
            {
                /*
                 * Retrieve the list of build tool source types and
                 * non-tool source types from the application store.
                 */

                ApplicationStorage appStore =
                    ApplicationStorage.GetInstance();

                List<String> toolSourceTypes =
                    appStore[Constants.APP_STORE_KEY_TOOL_SOURCE_TYPES]
                    as List<string>;

                List<String> nonToolSourceTypes =
                    appStore[Constants.APP_STORE_KEY_NON_TOOL_SOURCE_TYPES]
                    as List<string>;

                if (toolSourceTypes != null)
                    _displayedTypesList.AddRange(toolSourceTypes);

                if (nonToolSourceTypes != null)
                    _displayedTypesList.AddRange(nonToolSourceTypes);
            }
        }

        #endregion

        #region Background Image

        private void SetBackgroundImage()
        {
            string imagePath = Path.Combine(
                _applicationManager.QuickSharpUserHome,
                Constants.BACKGROUND_IMAGE_FOLDER);

            if (!Directory.Exists(imagePath)) return;

            /*
             * Try to find a theme specific folder, if not
             * use the default.
             */

            string themeName = _applicationManager.
                GetSelectedThemeProviderKey();

            if (!String.IsNullOrEmpty(themeName))
            {
                string themePath = 
                    Path.Combine(imagePath, themeName);

                if (Directory.Exists(themePath))
                    imagePath = themePath;
            }

            /*
             * Look for files matching the filename pattern.
             */

            string [] files = Directory.GetFiles(
                imagePath, Constants.BACKGROUND_IMAGE_PATTERN);

            if (files.Length == 0) return;

            /*
             * Use the first matching filename.
             */
            
            string filepath = files[0].ToLower();

            /*
             * If name contains 'dark' or 'tile' set the appropriate flags.
             */

            string filename = Path.GetFileName(filepath);
            
            bool darkBackground = 
                (filename.IndexOf(Constants.BACKGROUND_IMAGE_DARK) != -1);
            
            bool tiledBackground =
                (filename.IndexOf(Constants.BACKGROUND_IMAGE_TILE) != -1);

            /*
             * Try to show the image.
             */

            try
            {
                Bitmap img = new Bitmap(filepath);
                _mainListView.BackgroundImage = img;
                _mainListView.BackgroundImageTiled = tiledBackground;

                // Only do this if image set successfully.
                if (darkBackground)
                {
                    _mainListView.ForeColor = Color.White;
                    _mainListView.BorderStyle = BorderStyle.None;
                }
            }
            catch
            {
                // Do nothing
            }
        }

        #endregion

        #region Drag and Drop

        private void MainListView_ItemDrag(
            object sender, ItemDragEventArgs e)
        {
            /*
             * Allow drag and drop with left mouse button.
             * Don't allow drag when miltiple items selected.
             */

            if (e.Button == MouseButtons.Left &&
                _mainListView.SelectedItems.Count < 2)
                DoDragDrop(e.Item, DragDropEffects.All);
        }

        private void MainListView_DragOver(
            object sender, DragEventArgs e)
        {
            /*
             * Get the source and target items.
             */

            Point p = _mainListView.PointToClient(
                new Point(e.X, e.Y));
            
            ListViewItem targetItem =
                _mainListView.GetItemAt(p.X, p.Y);

            ListViewItem draggedItem =
                (ListViewItem)e.Data.GetData(typeof(ListViewItem));

            /*
             * Items may only be moved to folders. Files
             * may be moved or copied. Folders may only be
             * moved. Copy if Ctrl key pressed, otherwise move.
             */

            ClearSelectedItems();

            if (ItemIsFolder(targetItem))
            {
                if ((e.KeyState & 8) == 8)
                {
                    if (ItemIsFolder(draggedItem))
                        e.Effect = DragDropEffects.None;
                    else
                        e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.Move;
                }

                /*
                 * Select the target folder for feedback.
                 */

                targetItem.Selected = true;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        private void MainListView_DragDrop(object sender, DragEventArgs e)
        {
            /*
             * Get the source and target items.
             */

            Point p = _mainListView.PointToClient(
                new Point(e.X, e.Y));

            ListViewItem targetItem =
                _mainListView.GetItemAt(p.X, p.Y);

            ListViewItem draggedItem =
                (ListViewItem)e.Data.GetData(typeof(ListViewItem));

            /*
             * Check that the node at the drop location is not 
             * the dragged node.
             */

            if (draggedItem.Equals(targetItem)) return;

            string sourcePath = draggedItem.Tag as string;
            string targetFolder = targetItem.Tag as string;
            string targetPath = Path.Combine(
                targetFolder, Path.GetFileName(sourcePath));

            /*
             * Move or copy the source node to the target node.
             */

            if (e.Effect == DragDropEffects.Move)
            {
                try
                {
                    if (ItemIsFolder(draggedItem))
                        Directory.Move(sourcePath, targetPath);
                    else
                        File.Move(sourcePath, targetPath);

                    /*
                     * Update any open editors with new filepaths after move.
                     */

                    List<Document> documents = new List<Document>();

                    foreach (Document d in mainForm.ClientWindow.Documents)
                    {
                        if (d.FilePath.ToLower().StartsWith(sourcePath,
                                StringComparison.CurrentCultureIgnoreCase))
                            d.FilePath = targetPath +
                                d.FilePath.Remove(0, sourcePath.Length);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0}\r\n{1}",
                            Resources.MoveErrorMessage, ex.Message),
                        Resources.MoveErrorTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.Effect == DragDropEffects.Copy)
            {
                try
                {
                    File.Copy(sourcePath, targetPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0}\r\n{1}",
                            Resources.CopyErrorMessage, ex.Message),
                        Resources.CopyErrorTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            _applicationManager.NotifyFileSystemChange();
        }

        #endregion
    }
}
