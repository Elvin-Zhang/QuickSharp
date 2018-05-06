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
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WeifenLuo.WinFormsUI.Docking;
using ICSharpCode.SharpZipLib.Zip;
using QuickSharp.Core;

namespace QuickSharp.Explorer
{
    /// <summary>
    /// The explorer window form.
    /// </summary>
    public partial class ExplorerForm : BaseDockedForm
    {
        #region Nested Classes

        private class DummyNode : TreeNode { }

        private class FileNamePattern
        {
            public Regex Regex;
            public bool MatchResult = true;
        }

        #endregion

        private ApplicationManager _applicationManager;
        private SettingsManager _settingsManager;
        private string _rootFolder;
        private Dictionary<string, string> _documentTypeIconMap;
        private List<string> _expandedPaths;
        private bool _enableTitleBarUpdate;
        private bool _currentDirectoryFollowsRoot;
        private bool _applyFileNameFilter;
        private List<FileNamePattern> _allowedFileNamePatterns;

        /// <summary>
        /// Event raised when the tree view is about to be refreshed.
        /// </summary>
        public event MessageHandler TreeViewPreRefresh;

        /// <summary>
        /// Event raised when the tree view has been refreshed.
        /// </summary>
        public event MessageHandler TreeViewPostRefresh;

        /// <summary>
        /// Event raised when a tree node has been updated.
        /// </summary>
        public event TreeViewNodeUpdateHandler TreeViewNodeUpdate;

        /// <summary>
        /// Create the explorer window form.
        /// </summary>
        /// <param name="formKey">The GUID key used to identify the form internally.</param>
        public ExplorerForm(string formKey) : base(formKey)
        {
            _applicationManager = ApplicationManager.GetInstance();
            _settingsManager = SettingsManager.GetInstance();

            /*
             * Get client flags.
             */

            _enableTitleBarUpdate = _applicationManager.ClientProfile.
                HaveFlag(ClientFlags.ExplorerEnableTitleBarUpdate);

            _currentDirectoryFollowsRoot = _applicationManager.ClientProfile.
                HaveFlag(ClientFlags.ExplorerCurrentDirectoryFollowsRoot);

            /*
             * Restore the start directory from the previous session.
             */

            _rootFolder = _settingsManager.RootFolder;

            if (!Directory.Exists(_rootFolder))
                _rootFolder = Directory.GetCurrentDirectory();

            Directory.SetCurrentDirectory(_rootFolder);

            /*
             * If we want to start in the current directory we must do it
             * after all the plugins have loaded so that we use the same
             * directory as any plugin that moves it (the Workspace for example).
             */

            if (_applicationManager.ClientProfile.HaveFlag(
                ClientFlags.ExplorerStartFromCurrentDirectory))
            {
                PluginManager.GetInstance().PluginPostActivate += delegate
                {
                    _rootFolder = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(_rootFolder);
                };
            }

            #region Form Setup and Events

            InitializeComponent();

            Text = Resources.ExplorerWindowTitle;
            Icon = Resources.ExplorerIcon;

            VisibleChanged += delegate { RefreshView(); };

            _mainTreeView.AllowDrop = true;

            _mainTreeView.ItemDrag += 
                new ItemDragEventHandler(TreeView_ItemDrag);

            _mainTreeView.DragOver += 
                new DragEventHandler(TreeView_DragOver);

            _mainTreeView.DragDrop += 
                new DragEventHandler(TreeView_DragDrop);

            _mainTreeView.BeforeExpand +=
                new TreeViewCancelEventHandler(TreeView_BeforeExpand);

            _mainTreeView.BeforeCollapse +=
                new TreeViewCancelEventHandler(TreeView_BeforeCollapse);

            _mainTreeView.MouseClick +=
                new MouseEventHandler(TreeView_MouseClick);

            _mainTreeView.MouseDoubleClick += 
                new MouseEventHandler(TreeView_MouseDoubleClick);

            _mainTreeView.KeyDown += 
                new KeyEventHandler(TreeView_KeyDown);

            mainForm.FormClosing += 
                new FormClosingEventHandler(MainForm_FormClosing);

            /*
             * Reset the MainForm title bar if Explorer is hidden.
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

            #endregion

            /*
             * The explorer shows a view of the file system so register
             * to be notified if any plugin makes file system changes.
             */

            _applicationManager.FileSystemChange +=
                new MessageHandler(RefreshView);

            #region Document Type Images

            /* 
             * Associate the appropriate filetype image with appropriate
             * supported file extensions. 
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
             * Keep a list of any expanded nodes for repopulating on a refresh
             */

            _expandedPaths = new List<string>();

            mainForm.ThemeActivated +=
                new MessageHandler(MainForm_ThemeActivated);

            /*
             * Restore file filter state.
             */

            if (_settingsManager.FileFilters.Count > 0)
            {
                _applyFileNameFilter = _settingsManager.ApplyFilter;
                _filterButton.Checked = _applyFileNameFilter;
                SetFileNameFilter();
            }

            /*
             * Allow updates to be requested from the settings manager.
             */

            _settingsManager.OnUpdateUI += new MessageHandler(UpdateUI);

            /*
             * Initialize the tree.
             */

            RefreshView();
        }

        /// <summary>
        /// Set the form's initial state when there is no saved configuration
        /// to be restored from the previous session.
        /// </summary>
        protected override void SetFormDefaultState()
        {
            ClientProfile profile = _applicationManager.ClientProfile;

            if (profile.HaveFlag(ClientFlags.ExplorerDockRightByDefault))
                DockState = DockState.DockRight;
            else
                DockState = DockState.DockLeft;

            if (profile.HaveFlag(ClientFlags.ExplorerHideByDefault))
                Hide();
            else
                Show();
        }

        private void UpdateUI()
        {
            SetFileNameFilter();
            RefreshView();
        }

        #region Toolbar Events

        private void MoveToolStripButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = Resources.BrowseFolderDialogText;

            if (fbd.ShowDialog() == DialogResult.OK)
                MoveToSelectedFolder(fbd.SelectedPath);
        }

        #endregion

        #region Form Events

        private void MainForm_ThemeActivated()
        {
            ThemeFlags flags = _applicationManager.
                ClientProfile.ThemeFlags;

            if (flags != null)
            {
                if (flags.MainBackColor != Color.Empty)
                    BackColor = flags.MainBackColor;

                if (flags.ViewBackColor != Color.Empty)
                    TreeView.BackColor = flags.ViewBackColor;

                if (flags.ViewForeColor != Color.Empty)
                    TreeView.ForeColor = flags.ViewForeColor;

                if (flags.ViewShowBorder == false)
                    _mainTreeView.BorderStyle = BorderStyle.None;

                if (flags.MenuForeColor != Color.Empty)
                    MenuTools.ContextMenuStripItemsSetForeColor(
                        TreeViewContextMenu,
                        flags.MenuForeColor,
                        flags.MenuHideImages);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                _settingsManager.ApplyFilter = _applyFileNameFilter;
                _settingsManager.RootFolder = _rootFolder;
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
        /// The root folder of the explorer window tree view.
        /// </summary>
        public string RootFolder
        {
            get { return _rootFolder; }
            set { _rootFolder = value; }
        }

        /// <summary>
        /// The explorer window toolbar.
        /// </summary>
        public ToolStrip Toolbar
        {
            get { return _mainToolStrip; }
        }

        /// <summary>
        /// The explorer window tree view.
        /// </summary>
        public TreeView TreeView
        {
            get { return _mainTreeView; }
        }

        /// <summary>
        /// The explorer window tree view's context menu.
        /// </summary>
        public ContextMenuStrip TreeViewContextMenu
        {
            get { return _treeViewMenu; }
        }

        /// <summary>
        /// The image list used to provide images to the explorer window tree view.
        /// </summary>
        public ImageList DocumentImageList
        {
            get { return _treeViewImageList; }
        }

        /// <summary>
        /// A dictionary mapping file types to the names of the images used to represent them.
        /// The names are the keys of the images in the image list.
        /// </summary>
        public Dictionary<String, String> DocumentTypeIconMap
        {
            get { return _documentTypeIconMap; }
        }

        #endregion

        #region Drag and Drop

        private void TreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            /*
             * Allow drag and drop with left mouse button.
             * Actual effect is selected in DragOver.
             */

            if (e.Button == MouseButtons.Left)
                DoDragDrop(e.Item, DragDropEffects.All);
        }

        private void TreeView_DragOver(object sender, DragEventArgs e)
        {
            /*
             * Get the source and target nodes.
             */

            TreeNode targetNode = _mainTreeView.GetNodeAt(
                _mainTreeView.PointToClient(new Point(e.X, e.Y)));

            TreeNode draggedNode =
                (TreeNode)e.Data.GetData(typeof(TreeNode));

            /*
             * Items may only be moved to folders. Files
             * may be moved or copied. Folders may only be
             * moved. Copy if Ctrl key pressed, otherwise move.
             */

            if (NodeIsFolder(targetNode))
            {
                if ((e.KeyState & 8) == 8)
                {
                    if (NodeIsFolder(draggedNode))
                        e.Effect = DragDropEffects.None;
                    else
                        e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.Move;
                }
            }
            else
                e.Effect = DragDropEffects.None;

            /*
             * Select the target node as the mouse is dragged
             * over it.
             */

            _mainTreeView.SelectedNode = targetNode;
        }

        private void TreeView_DragDrop(object sender, DragEventArgs e)
        {
            /*
             * Get the source and target nodes.
             */

            TreeNode targetNode = _mainTreeView.GetNodeAt(
                _mainTreeView.PointToClient(new Point(e.X, e.Y)));

            TreeNode draggedNode = 
                (TreeNode)e.Data.GetData(typeof(TreeNode));

            /*
             * Check that the node at the drop location is not 
             * the dragged node or its descendant.
             */

            if (draggedNode.Equals(targetNode)) return;
            if (NodeContainsNode(draggedNode, targetNode)) return;

            string sourcePath = draggedNode.Tag as string;
            string targetFolder = targetNode.Tag as string;
            string targetPath = Path.Combine(
                targetFolder, Path.GetFileName(sourcePath));

            /*
             * Move or copy the source node to the target node.
             */

            if (e.Effect == DragDropEffects.Move)
            {
                try
                {
                    if (NodeIsFolder(draggedNode))
                    {
                        /*
                         * Can't move a folder if the current directory is
                         * within the path being moved so set the current
                         * directory to the root as a precaution.
                         */

                        Directory.SetCurrentDirectory(_rootFolder);
                        Directory.Move(sourcePath, targetPath);
                    }
                    else
                    {
                        File.Move(sourcePath, targetPath);
                    }

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

                    /*
                     * Mark target folder for expansion on refresh so we can
                     * see the moved file/folder.
                     */

                    if (!_expandedPaths.Contains(targetFolder))
                        _expandedPaths.Add(targetFolder);
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

                    /*
                     * Mark target folder for expansion on refresh so we can
                     * see the new file.
                     */

                    if (!_expandedPaths.Contains(targetFolder))
                        _expandedPaths.Add(targetFolder);
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

        #region Visited Folder List

        /// <summary>
        /// Refresh the list of visited folders in the explorer window toolbar.
        /// </summary>
        public void RefreshVisitedFolderList()
        {
            _visitedFolderSelectButton.DropDownItems.Clear();
            _visitedFolderSelectButton.Enabled = false;

            // Create a new list of only the folders that exist.
            List<string> validFoldersList = new List<string>();

            ThemeFlags flags = _applicationManager.
                ClientProfile.ThemeFlags;
            
            bool applyTheme = flags != null &&
                flags.MenuForeColor != Color.Empty;

            foreach (string folder in _settingsManager.VisitedFoldersList)
            {
                if (!Directory.Exists(folder)) continue;

                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = Path.GetFileName(folder);
                if (item.Text == String.Empty) item.Text = folder;
                item.ToolTipText = folder;
                item.Click += new EventHandler(
                    VisitedFolderDropDownItem_Click);

                // Apply theme color if available.
                if (applyTheme) item.ForeColor = flags.MenuForeColor;

                // Insert so that latest file appears first.
                _visitedFolderSelectButton.DropDownItems.Insert(0, item);

                validFoldersList.Add(folder);
            }

            if (_visitedFolderSelectButton.DropDownItems.Count > 0)
                _visitedFolderSelectButton.Enabled = true;

            // Retain the updated list.
            _settingsManager.VisitedFoldersList = validFoldersList;
        }

        private void VisitedFolderDropDownItem_Click(
            object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null) return;

            string path = item.ToolTipText;

            if (Directory.Exists(path))
            {
                MoveToSelectedFolder(path);
            }   
            else
            {
                _settingsManager.RemoveFolderFromVisitedList(path);
                if (_settingsManager.VisitedFoldersList.Count == 0)
                    _visitedFolderSelectButton.Enabled = false;
            }
        }

        #endregion

        #region File Name Filters

        /*
         * Entry point - the tree refresh view calls this method
         */

        private bool ApplyNameFileFilter(string name)
        {
            if (!_applyFileNameFilter || _allowedFileNamePatterns == null)
                return true;

            /*
             * Each file starts hidden and each filter can make it visible.
             * If none do so the file remains hidden. Once a file is visible
             * it can't be hidden again by the next filter. To use a negative
             * match preceed the pattern with "!". This will return false for
             * a match instead of true. Remember the filters "opt in" so if
             * you have a negative filter it has to be followed by something
             * to opt the remaining files back in (use "(?i:\.+)" as a
             * universal match). The filters are applied in the order they
             * are listed in the pattern, the first to create a match
             * determines the visibility of the file.
             */

            foreach (FileNamePattern pattern in _allowedFileNamePatterns)
                if (pattern.Regex.Match(name).Success == true)
                    return pattern.MatchResult;
                    
            return false;
        }

        /*
         * Update the list of file patterns and the filter status.
         */

        private void SetFileNameFilter()
        {
            string selectedFilter = 
                _settingsManager.SelectedFilter;

            // Validate the selected filter key
            if (String.IsNullOrEmpty(selectedFilter) ||
                !_settingsManager.FileFilters.ContainsKey(selectedFilter))
            {
                _settingsManager.SelectedFilter = String.Empty;
                _allowedFileNamePatterns = null;
                _applyFileNameFilter = false;
                _filterButton.Checked = false;
                _filterButton.Enabled = false;
                _filterButton.ToolTipText = Resources.ToolbarFilter;
                return;
            }

            // Get the filter
            FileFilter filter = _settingsManager.
                FileFilters[selectedFilter];

            // Update the button 
            _filterButton.ToolTipText = String.Format("{0}: {1}",
                Resources.ToolbarFilter, filter.Name);

            // Convert to a list of file patterns
            List<FileNamePattern> list = new List<FileNamePattern>();

            string[] split = filter.Filter.Split(' ');

            if (split.Length > 0)
            {
                foreach (string s in split)
                {
                    if (s == String.Empty)
                        continue;

                    try
                    {
                        FileNamePattern pattern = new FileNamePattern();
                        string re = s;

                        if (re.StartsWith("!"))
                        {
                            pattern.MatchResult = false;
                            re = re.Substring(1);
                        }

                        // Pseudo pattern - global match
                        if (re == Constants.PSEUDO_PATTERN_GLOBAL)
                            re = "(?!:.+)"; 

                        pattern.Regex = new Regex(re);

                        list.Add(pattern);
                    }
                    catch
                    {
                        /*
                         * Ignore errors here, just carry on with what's left.
                         * Regex is validated in management UI - presenting errors
                         * here causes dialogs to popup too frequently with no
                         * real clue as to why.
                         */
                    }
                }

                _allowedFileNamePatterns = list;
            }
        }

        /*
         * Toggle the filter via the toolbar button.
         */

        private void ToggleFileFilter()
        {
            _filterButton.Checked = !_filterButton.Checked;
            _applyFileNameFilter = _filterButton.Checked;

            UpdateUI();
        }

        /*
         * Update the drop-down list when opened.
         */

        private void RefreshFileFilterList()
        {
            /*
             * Enable/disable the buttons.
             */

            if (_settingsManager.FileFilters.Keys.Count == 0)
            {
                _filterButton.Enabled = false;
                _filterButton.Checked = false;
                _filterSelectButton.Enabled = false;
                return;
            }

            _filterButton.Enabled = _settingsManager.
                SelectedFilter != String.Empty;
            
            _filterSelectButton.Enabled = true;

            /*
             * Refresh the menu.
             */

            _filterSelectButton.DropDownItems.Clear();

            ThemeFlags flags = _applicationManager.
                ClientProfile.ThemeFlags;

            bool applyTheme = flags != null &&
                flags.MenuForeColor != Color.Empty;

            /*
             * Sort the filters.
             */

            List<FileFilter> filters = _settingsManager.FileFilters.
                Values.ToList<FileFilter>();

            filters.Sort(new FileFilterComparer()); 

            foreach (FileFilter filter in filters)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Name = filter.ID;
                item.Text = filter.Name;
                item.Tag = filter.Filter;
                item.Click += delegate
                {
                    _settingsManager.SelectedFilter = item.Name;
                    UpdateUI();
                };

                if (_settingsManager.SelectedFilter == filter.ID)
                    item.Checked = true;

                // Apply theme color if available.
                if (applyTheme) item.ForeColor = flags.MenuForeColor;

                _filterSelectButton.DropDownItems.Add(item);
            }
        }

        #endregion

        #region Tree View

        /// <summary>
        /// Refresh the explorer window tree view.
        /// </summary>
        public void RefreshView()
        {
            if (!Visible) return;

            /*
             * Call pre refresh event handlers.
             */

            if (TreeViewPreRefresh != null)
                TreeViewPreRefresh();

            /*
             * Save the top visible node for restoration after the refresh.
             * We can't use the TopNode property directly as we will have
             * a completely new set of nodes.
             */

            string topVisibleNodeKey = null;
            string topVisibleNodePath = null;

            if (_mainTreeView.TopNode != null)
            {
                topVisibleNodeKey = _mainTreeView.TopNode.Name;
                topVisibleNodePath = _mainTreeView.TopNode.FullPath;
            }

            /*
             * Save the currently selected node so that it can be
             * restored after the refresh. We can't save the node
             * itself as the refresh creates an entirely new set
             * of nodes.
             */

            string selectedNodePath = null;

            if (_mainTreeView.SelectedNode != null)
                selectedNodePath = 
                    _mainTreeView.SelectedNode.Tag as String;

            /*
             * Update the title bar with the current location.
             */

            if (_enableTitleBarUpdate)
            {
                string folder = _rootFolder;

                if (!_settingsManager.ShowFullPath)
                {
                    folder = Path.GetFileName(_rootFolder);
                    if (folder == String.Empty)
                        folder = Path.GetPathRoot(_rootFolder);
                }

                mainForm.Text = String.Format("{0} - {1}",
                    folder, _applicationManager.ClientProfile.ClientTitle);
            }

            /*
             * Create the root node.
             */
            
            string rootName = Path.GetFileName(_rootFolder);
            if (rootName == String.Empty)
                rootName = Path.GetPathRoot(_rootFolder);

            TreeNode rootNode = new TreeNode();
            rootNode.Text = rootNode.Name = rootName;
            rootNode.Tag = _rootFolder;

            if (_rootFolder == Directory.GetCurrentDirectory())
                rootNode.ImageKey = rootNode.SelectedImageKey =
                    Constants.OPENED_FOLDER_IMAGE;
            else
                rootNode.ImageKey = rootNode.SelectedImageKey =
                    Constants.CLOSED_FOLDER_IMAGE;

            if (TreeViewNodeUpdate != null)
                TreeViewNodeUpdate(ref rootNode);

            if (rootNode == null) return;

            _mainTreeView.BeginUpdate();
            _mainTreeView.Nodes.Clear();
            _mainTreeView.Nodes.Add(rootNode);
            
            /*
             * populate the tree.
             */

            PopulateTreeNode(rootNode.Nodes, _rootFolder, selectedNodePath);

            rootNode.Expand();
            
            _mainTreeView.EndUpdate();

            if (_mainTreeView.SelectedNode == null)
                _mainTreeView.SelectedNode = rootNode;

            /*
             * Restore the top node if still present in the tree.
             */

            if (topVisibleNodeKey != null && topVisibleNodePath != null)
            {
                TreeNode[] nodes = 
                    _mainTreeView.Nodes.Find(topVisibleNodeKey, true);

                foreach (TreeNode node in nodes)
                {
                    if (node.FullPath == topVisibleNodePath)
                    {
                        _mainTreeView.TopNode = node;
                        break;
                    }
                }
            }

            /*
             * Update the visited folder list button state.
             */

            _visitedFolderSelectButton.Enabled =
                _settingsManager.VisitedFoldersList.Count > 0;

            /*
             * Update the file filters button state.
             */

            _filterButton.Enabled =
                _settingsManager.FileFilters.Count > 0 &&
                _settingsManager.SelectedFilter != String.Empty;

            _filterSelectButton.Enabled =
                _settingsManager.FileFilters.Count > 0;

            /*
             * Call post refresh event handlers.
             */

            if (TreeViewPostRefresh != null)
                TreeViewPostRefresh();
        }

        private void PopulateTreeNode(TreeNodeCollection root,
            String path, String selectedNodePath)
        {
            bool hideHiddenFiles = !_settingsManager.ShowHiddenFiles;
            bool hideSystemFiles = !_settingsManager.ShowSystemFiles;

            root.Clear();

            foreach (string folder in Directory.GetDirectories(path))
            {
                DirectoryInfo di = new DirectoryInfo(folder);
                
                if (hideHiddenFiles &&
                    ((di.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden))
                    continue;

                if (hideSystemFiles &&
                    ((di.Attributes & FileAttributes.System) == FileAttributes.System))
                    continue;

                TreeNode node = new TreeNode();
                node.Text = node.Name = Path.GetFileName(folder);
                node.Tag = folder;

                if (folder == Directory.GetCurrentDirectory())
                    node.ImageKey = node.SelectedImageKey = 
                        Constants.OPENED_FOLDER_IMAGE;
                else
                    node.ImageKey = node.SelectedImageKey =
                        Constants.CLOSED_FOLDER_IMAGE;

                if (TreeViewNodeUpdate != null)
                    TreeViewNodeUpdate(ref node);

                if (node != null)
                {
                    root.Add(node);

                    if (selectedNodePath != null &&
                        selectedNodePath == node.Tag as String)
                        _mainTreeView.SelectedNode = node;

                    if (_expandedPaths.Contains(folder))
                    {
                        PopulateTreeNode(node.Nodes, folder, selectedNodePath);
                        node.Expand();
                    }
                    else
                        node.Nodes.Add(new DummyNode());
                }
            }

            foreach (string file in Directory.GetFiles(path))
            {
                FileInfo fi = new FileInfo(file);

                if (hideHiddenFiles &&
                    ((fi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden))
                    continue;
                
                if (hideSystemFiles &&
                    ((fi.Attributes & FileAttributes.System) == FileAttributes.System))
                    continue;

                TreeNode node = new TreeNode();
                node.Text = node.Name = Path.GetFileName(file);
                node.Tag = file;

                DocumentType documentType = new DocumentType(fi.Extension);
                if (_documentTypeIconMap.ContainsKey(documentType.ToString()))
                    node.ImageKey = node.SelectedImageKey =
                        _documentTypeIconMap[documentType.ToString()];
                else
                    node.ImageKey = node.SelectedImageKey =
                        Constants.DOCUMENT_IMAGE;

                if (TreeViewNodeUpdate != null)
                    TreeViewNodeUpdate(ref node);

                if (node != null && ApplyNameFileFilter(node.Name))
                {
                    root.Add(node);

                    if (selectedNodePath != null &&
                        selectedNodePath == node.Tag as String)
                        _mainTreeView.SelectedNode = node;
                }
            }
        }

        #endregion

        #region Tree View Events

        private void TreeView_BeforeExpand(
            object sender, TreeViewCancelEventArgs e)
        {
            string path = e.Node.Tag as string;
            
            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0] is DummyNode)
                PopulateTreeNode(e.Node.Nodes, path, null);

            if (!_expandedPaths.Contains(path))
                _expandedPaths.Add(path);
        }

        private void TreeView_BeforeCollapse(
            object sender, TreeViewCancelEventArgs e)
        {
            string path = e.Node.Tag as string;

            if (_expandedPaths.Contains(path))
                _expandedPaths.Remove(path);
        }

        private void TreeView_MouseClick(
            object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode node = _mainTreeView.GetNodeAt(e.Location);
                if (node == null) return;

                _mainTreeView.SelectedNode = node;
            }            
        }

        private void TreeView_MouseDoubleClick(
            object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                /*
                 * Deliberately NOT using NodeMouseClick or e.Node
                 * here. Node returned is incorrect if the control
                 * scrolls as a result of the double click.
                 */

                TreeNode node = _mainTreeView.SelectedNode;
                if (node == null) return;

                if (!NodeIsFolder(node))
                {
                    /*
                     * Don't use OpenNode - we don't want to
                     * open folders by double clicking as this
                     * conflicts with expanding/collapsing the node.
                     */

                    string path = node.Tag as string;

                    if (path != null)
                        mainForm.LoadDocumentIntoWindow(path, true);
                }
            }
        }

        private void TreeView_KeyDown(
            object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TreeNode node = _mainTreeView.SelectedNode;
                if (node == null) return;

                OpenNode(node);
            }

            if (e.KeyCode == Keys.Delete)
            {
                TreeNode node = _mainTreeView.SelectedNode;
                if (node == null) return;
                if (node.Parent == null) return;

                DeleteNode(node);
            }

            if (e.KeyCode == Keys.F2)
            {
                TreeNode node = _mainTreeView.SelectedNode;
                if (node == null) return;
                if (node.Parent == null) return;

                RenameNode(node);
            }

            if (e.KeyCode == Keys.Back)
                MoveToParent();
        }

        #endregion

        #region Tree View Menu Events

        private void treeViewMenu_Opening(object sender, CancelEventArgs e)
        {
            UI_TREE_MENU_OPEN.Enabled = false;
            UI_TREE_MENU_RENAME.Enabled = false;
            UI_TREE_MENU_CLONE.Enabled = false;
            UI_TREE_MENU_DELETE.Enabled = false;
            UI_TREE_MENU_CREATE_FOLDER.Enabled = false;
            UI_TREE_MENU_SET_AS_CURRENT_DIR.Enabled = false;

            if (_mainTreeView.Nodes.Count == 0) return;
            if (_mainTreeView.SelectedNode == null) return;

            UI_TREE_MENU_OPEN.Enabled = true;
            UI_TREE_MENU_RENAME.Enabled = true;
            UI_TREE_MENU_DELETE.Enabled = true;

            TreeNode node = _mainTreeView.SelectedNode;
            if (node == null) return;

            if (NodeIsFolder(node))
            {
                UI_TREE_MENU_CREATE_FOLDER.Enabled = true;
                UI_TREE_MENU_SET_AS_CURRENT_DIR.Enabled = true;

                /*
                 * Don't allow the root node to be deleted or renamed.
                 */

                if (node.Parent == null)
                {
                    UI_TREE_MENU_RENAME.Enabled = false;
                    UI_TREE_MENU_DELETE.Enabled = false;
                }
            }
            else
            {
                UI_TREE_MENU_CLONE.Enabled = true;
            }
        }

        private void UI_TREE_MENU_OPEN_Click(object sender, EventArgs e)
        {
            TreeNode node = _mainTreeView.SelectedNode;
            if (node == null) return;

            OpenNode(node);
        }

        private void UI_TREE_MENU_RENAME_Click(object sender, EventArgs e)
        {
            TreeNode node = _mainTreeView.SelectedNode;
            if (node == null) return;
            if (node.Parent == null) return;

            RenameNode(node);
        }

        private void UI_TREE_MENU_CLONE_Click(object sender, EventArgs e)
        {
            TreeNode node = _mainTreeView.SelectedNode;
            if (node == null) return;

            CloneNode(node);
        }

        private void UI_TREE_MENU_DELETE_Click(object sender, EventArgs e)
        {
            TreeNode node = _mainTreeView.SelectedNode;
            if (node == null) return;
            if (node.Parent == null) return;

            DeleteNode(node);
        }

        private void UI_TREE_MENU_CREATE_FOLDER_Click(object sender, EventArgs e)
        {
            TreeNode node = _mainTreeView.SelectedNode;
            if (node == null) return;

            string path = node.Tag as string;
            if (path == null) return;

            CreateFolder(path);
        }

        private void UI_TREE_MENU_SET_AS_CURRENT_DIR_Click(object sender, EventArgs e)
        {
            string path = null;

            TreeNode node = _mainTreeView.SelectedNode;

            if (node == null)
                path = _rootFolder;
            else if (NodeIsFolder(node))
                path = node.Tag as string;

            if (path == null) return;

            Directory.SetCurrentDirectory(path); 
            _applicationManager.NotifyFileSystemChange();
        }

        #endregion

        #region File System Operations

        /// <summary>
        /// Notify the explorer window that the root folder has changed; the
        /// view will be updated according to the settings in effect.
        /// </summary>
        public void RootChangeUpdate()
        {
            if (_currentDirectoryFollowsRoot &&
                Directory.Exists(_rootFolder))
            {
                Directory.SetCurrentDirectory(_rootFolder);
                _applicationManager.NotifyFileSystemChange();
            }
        }

        private void MoveToSelectedFolder(string path)
        {
            /*
             * Test the new folder can be opened.
             */

            string savePath = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(path);
                Directory.SetCurrentDirectory(savePath);

                _rootFolder = path;

                _settingsManager.AddFolderToVisitedList(path);

                _applicationManager.NotifyFileSystemChange();
                _expandedPaths.Clear();

                RootChangeUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}\r\n{1}",
                        Resources.MoveRootErrorMessage,
                        ex.Message),
                    Resources.MoveRootErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void MoveToParent()
        {
            DirectoryInfo newRoot = Directory.GetParent(_rootFolder);

            if (newRoot != null)
            {
                _rootFolder = newRoot.FullName;
                _applicationManager.NotifyFileSystemChange();
                _expandedPaths.Clear();

                RootChangeUpdate();
            }
        }

        private void OpenNode(TreeNode node)
        {
            string path = node.Tag as string;
            if (path == null) return;

            if (NodeIsFolder(node))
            {
                _rootFolder = path;
                _applicationManager.NotifyFileSystemChange();
                _expandedPaths.Clear();

                RootChangeUpdate();
            }
            else
            {
                mainForm.LoadDocumentIntoWindow(path, true);
            }
        }

        private void RenameNode(TreeNode node)
        {
            RenameForm rf = new RenameForm();
            rf.NewName = node.Name;

            if (rf.ShowDialog() == DialogResult.OK)
            {
                String newName = rf.NewName.Trim();
                if (newName == String.Empty) return;
                if (newName == node.Name) return;

                string oldPath = node.Tag as string;
                if (oldPath == null) return;

                string newPath = String.Empty;

                try
                {
                    if (NodeIsFolder(node))
                    {
                        /*
                         * We cant't rename a folder if it's the current
                         * directory or a child of the current directory so we
                         * need to move the current directory to the root
                         * folder so that it's out of the way of the rename
                         * operation. We can restore it afterwards if it still
                         * exists (i.e. was not originally within the path
                         * of the renamed folder).
                         */

                        string currentDir = Directory.GetCurrentDirectory();
                        Directory.SetCurrentDirectory(_rootFolder);

                        newPath = FileTools.ChangeDirectoryName(
                            oldPath, newName);

                        /*
                         * Update the paths of any open editors.
                         */

                        List<Document> documents = new List<Document>();

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

                        /*
                         * If the old node was expanded make the new one
                         * expanded. If any child folders were expanded
                         * they will remain in the list but will no longer
                         * be valid paths.
                         */

                        if (_expandedPaths.Contains(oldPath))
                        {
                            _expandedPaths.Remove(oldPath);
                            _expandedPaths.Add(newPath);
                        }

                        if (Directory.Exists(currentDir))
                            Directory.SetCurrentDirectory(currentDir);
                    }
                    else
                    {
                        newPath = FileTools.ChangeFileName(
                            oldPath, newName);

                        /*
                         * Update the filename in any open editors.
                         */

                        List<Document> documents = new List<Document>();

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

                    _applicationManager.NotifyFileSystemChange();
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
            }
        }
        
        private void CloneNode(TreeNode node)
        {
            try
            {
                string source = node.Tag as string;
                if (source == null) return;

                int i = 1;
                string folder = Path.GetDirectoryName(source);
                string filebase = Path.GetFileNameWithoutExtension(source);
                string extension = Path.GetExtension(source);

                string destination = source;

                while (File.Exists(destination))
                {
                    string filename =
                        String.Format("{0}_{1}{2}", filebase, i++, extension);

                    destination = Path.Combine(folder, filename);
                }

                File.Copy(source, destination);
                _applicationManager.NotifyFileSystemChange();
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
        }

        private void DeleteNode(TreeNode node)
        {
            try
            {
                string path = node.Tag as string;
                if (path == null) return;

                if (NodeIsFolder(node))
                {
                    /*
                     * Can't delete folder if current directory or it's
                     * child. Need to move the current dir to the root
                     * folder and restore it after the delete where possible.
                     * As with the folder rename the current directory is
                     * moved regardless of where it is - this is easier than
                     * working out if the folder to be deleted is the current
                     * directory or one of it's children. If it's not it will
                     * be restored after the delete.
                     */

                    string currentDir = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(_rootFolder);

                    FileTools.DeleteWithUndo(path);

                    if (_expandedPaths.Contains(path))
                        _expandedPaths.Remove(path);

                    if (Directory.Exists(currentDir))
                        Directory.SetCurrentDirectory(currentDir);
                }
                else
                {
                    FileTools.DeleteWithUndo(path);

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

                _applicationManager.NotifyFileSystemChange();
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
        }

        private void CreateFolder(string parentFolder)
        {
            string name = Resources.NewFolderName;
            int i = 1;
            while (Directory.Exists(name))
                name = String.Format("{0} ({1})",
                    Resources.NewFolderName, i++);

            RenameForm rf = new RenameForm();
            rf.NewName = name;
            rf.Title = Resources.CreateFolderTitle;

            if (rf.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    String folderName = Path.Combine(
                        parentFolder, rf.NewName.Trim());

                    Directory.CreateDirectory(folderName);

                    if (!_expandedPaths.Contains(parentFolder))
                        _expandedPaths.Add(parentFolder);

                    _applicationManager.NotifyFileSystemChange();
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
            }
        }

        #endregion

        #region File Backup

        /// <summary>
        /// Backup the contents of the root folder to a zip archive.
        /// </summary>
        public void BackupExplorer()
        {
            String sourceDirectory = RootFolder;

            /*
             * Validate the source directory - user's need to use
             * common sense, i.e. not selecting huge folder structures
             * for backing up. We can trap the most obvious case
             * of trying to backup a whole disk.
             */

            if (String.IsNullOrEmpty(Path.GetFileName(sourceDirectory)))
            {
                MessageBox.Show(
                    Resources.BackupDialogNoDisk,
                    Resources.BackupDialogTitle,
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
                        Resources.BackupDialogSuccess,
                        zipFileName),
                    Resources.BackupDialogTitle,
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
                        Resources.BackupDialogError,
                        ex.Message),
                    Resources.BackupDialogTitle,
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
                fbd.Description = Resources.BackupFolderMessage;

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

        #endregion

        #region Helpers

        private bool NodeContainsNode(TreeNode node1, TreeNode node2)
        {
            if (node2.Parent == null) return false;
            if (node2.Parent.Equals(node1)) return true;

            return NodeContainsNode(node1, node2.Parent);
        }

        private bool NodeIsFolder(TreeNode node)
        {
            if (node == null) return false;
            if (node.ImageKey == Constants.CLOSED_FOLDER_IMAGE) return true;
            if (node.ImageKey == Constants.OPENED_FOLDER_IMAGE) return true;
            return false;
        }

        private void LaunchExplorer()
        {
            string explorerPath = Path.Combine(
                Environment.SystemDirectory, "..\\explorer.exe");

            string explorerArgs = _rootFolder;

            if (!File.Exists(explorerPath))
            {
                MessageBox.Show(String.Format(
                        Resources.LaunchExplorerErrorMessage,
                        "\r\n", explorerPath),
                    Resources.LaunchExplorerErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            FileTools.LaunchApplication(
                false, explorerPath, explorerArgs);
        }

        private void LaunchShell()
        {
            string shellPath = Path.Combine(
                Environment.SystemDirectory, "cmd.exe");

            string shellArgs = "/D/K PROMPT $N:$G&&CD";

            if (!File.Exists(shellPath))
            {
                MessageBox.Show(String.Format(
                        Resources.LaunchShellErrorMessage,
                        "\r\n", shellPath),
                    Resources.LaunchShellErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            FileTools.LaunchApplication(
                true, shellPath, shellArgs, _rootFolder);
        }

        #endregion
    }
}
