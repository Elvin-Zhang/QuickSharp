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
using System.Linq;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.Core
{
    /// <summary>
    /// The application main form.
    /// </summary>
    public partial class MainForm : Form
    {
        private ApplicationManager _applicationManager;
        private IPersistenceManager _persistenceManager;
        private PluginManager _pluginManager;
        private ClientProfile _profile;
        private List<String> _recentlyUsedDocuments;
        private bool _restoreDocumentsFromLastSession;
        private string _selectedOptionsPageName;
        private bool _allowShellOpen;
        private bool _showNoHandlerMessage;
        private bool _showFullScreen;
        private bool _showDocumentChangeStatusMessage;
        private FormWindowState _restoredWindowState;

        /// <summary>
        /// Event raised when the theme has been activated.
        /// </summary>
        public event MessageHandler ThemeActivated;

        /// <summary>
        /// Event raised when the docking window manager has restored
        /// the window layout.
        /// </summary>
        public event MessageHandler DockPanelPostLoad;

        /// <summary>
        /// Create the main form.
        /// </summary>
        public MainForm()
        {
            _applicationManager = ApplicationManager.GetInstance();
            _applicationManager.MainForm = this;

            /*
             * Get the ClientProfile.
             */

            _profile = _applicationManager.ClientProfile;

            if (_profile == null)
                throw new Exception(Resources.NullClientProfileMessage);

            /*
             * Register the default persistence provider and
             * create the settings persistence manager.
             */

            _applicationManager.RegisterPersistenceProvider(
                Constants.REGISTRY_PERSISTENCE_PROVIDER,
                RegistryPersistenceManager.GetInstance);

            _persistenceManager = _applicationManager.
                GetPersistenceManager(Constants.MODULE_NAME);

            /*
             * Set default form providers.
             */

            if (_profile.UpdateCheckFormFactory == null)
                _profile.UpdateCheckFormFactory = delegate
                { return new UpdateCheckForm(); };

            if (_profile.AboutBoxFactory == null)
                _profile.AboutBoxFactory = delegate
                { return new AboutForm(); };

            /*
             * Get any command line flags and values.
             */

            foreach (string arg in _profile.CommandLineArgs)
            {
                if (arg[0] != '/' && arg[0] != '-') continue;
                if (arg.Length < 2) continue;
                _applicationManager.AddCommandLineSwitch(arg.Substring(1));
            }

            /*
             * Determine if documents should be opened by the Windows shell
             * and if "no handler" messages should be shown.
             */

            _allowShellOpen = _persistenceManager.ReadBoolean(
                Constants.KEY_DOCUMENT_ALLOW_SHELL_OPEN, true);

            _showNoHandlerMessage = _persistenceManager.ReadBoolean(
                Constants.KEY_DOCUMENT_SHOW_NO_HANDLER_MESSAGE, true);

            _showDocumentChangeStatusMessage = _persistenceManager.ReadBoolean(
                Constants.KEY_SHOW_DOCUMENT_PATH, false); 

            /*
             * Set up the MRU documents list.
             */

            _recentlyUsedDocuments = _persistenceManager.ReadStrings(
                Constants.KEY_MRU_DOCUMENTS_LIST);

            // Trim down to size if too big
            while (_recentlyUsedDocuments.Count > _profile.MRUDocumentListMax)
                _recentlyUsedDocuments.RemoveAt(0);

            /*
             * Optionally allow the documents open at the end of the last
             * session to be restored.
             */
            
            if (_persistenceManager.ReadBoolean(
                Constants.KEY_DOCUMENT_RESTORE_LAST_SESSION, false))
                _restoreDocumentsFromLastSession = true;

            /*
             * Optionally reset the window configuration.
             */

            if (_applicationManager.HaveCommandLineSwitch(Resources.SwitchClean) &&
                _applicationManager.HaveDockPanelConfig)
                File.Delete(_applicationManager.DockPanelConfigFile);

            /*
             * Initialize the form.
             */
            
            InitializeComponent();

            /*
             * Add the Help and UpdateCheck menu items if needed.
             * Do this here to avoid adding the menu items after the
             * plugins and confusing their menu placement strategies.
             */

            SetApplicationHelpState();
            SetUpdateCheckState();

            /*
             * Prepare the main toolbar. Create the main toolbar
             * before the plugins so items can be added if required.
             */

            CreateMainToolbar();

            /*
             * Load the available plugins.
             */

            _pluginManager = PluginManager.GetInstance();
            _pluginManager.LoadPlugins();
            _pluginManager.RegisterPlugins();
            _pluginManager.ActivatePlugins(this);

            /*
             * Load the toolbars and setup the menu.
             */

            LoadDockedToolStrips();

            /*
             * Enable the document management UI if documents and document
             * handlers have been registered.
             */

            UI_FILE_MENU_NEW.Enabled = UI_TOOLBAR_NEW.Enabled = false;

            if (_applicationManager.NewDocumentType != null &&
                _applicationManager.NewDocumentHandler != null)
                UI_FILE_MENU_NEW.Enabled = UI_TOOLBAR_NEW.Enabled = true;

            UI_FILE_MENU_OPEN.Enabled = UI_TOOLBAR_OPEN.Enabled = false;

            if (_applicationManager.OpenDocumentHandlers.Count > 0)
                UI_FILE_MENU_OPEN.Enabled = UI_TOOLBAR_OPEN.Enabled = true;

            /*
             * Optionally display the current document path on the status bar
             * for 3 seconds whenever the it changes.
             */

            if (_showDocumentChangeStatusMessage)
            {
                ClientWindow.ActiveDocumentChanged += delegate
                {
                    Document doc = ClientWindow.ActiveDocument as Document;

                    if (doc != null && doc.FilePath != null)
                        SetStatusBarMessage(doc.FilePath, 5);
                    else
                        SetStatusBarMessage(String.Empty);
                };
            }

            /*
             * Set the theme.
             */

            // Register the default 'do nothing' theme.
            _applicationManager.RegisterThemeProvider(
                new DefaultTheme());

            // Register the built in Visual Studio 2008 theme.
            if (!_profile.HaveFlag(ClientFlags.CoreDisableVisualStudio2008Theme))
                _applicationManager.RegisterThemeProvider(
                    new VS2008Theme());

            // Register the built in Visual Studio 2010 themes.
            if (!_profile.HaveFlag(ClientFlags.CoreDisableVisualStudio2010Theme))
            {
                _applicationManager.RegisterThemeProvider(
                    new VS2010ThemeBasic());
                _applicationManager.RegisterThemeProvider(
                    new VS2010ThemeEnhanced());
            }

            // Set the currently selected theme, use VS2010 for
            // initially selected theme. If this isn't available
            // SelectTheme will bump it down to the default.
            _applicationManager.SelectedTheme =
                _persistenceManager.ReadString(
                    Constants.KEY_SELECTED_THEME,
                    Constants.VS2010_ENHANCED_THEME_ID);

            IQuickSharpTheme provider = 
                _applicationManager.GetSelectedThemeProvider();

            if (provider != null)
                provider.ApplyTheme();

            if (_profile.ThemeFlags != null &&
                _profile.ThemeFlags.MenuForeColor != Color.Empty)
            {
                MenuTools.MenuStripItemsSetForeColor(MainMenu,
                    _profile.ThemeFlags.MenuForeColor,
                    _profile.ThemeFlags.MenuHideImages);
            }

            // Send notification that the theme has been activated.
            if (ThemeActivated != null) ThemeActivated();

            /*
             * Set or restore the layout of the main form.
             */

            SetMainFormState();
            UpdateMenuItems();
            UpdateToolbarItems();

            /*
             * Register the option pages. (This needs to be after
             * the plugins and default themes are loaded/registered.)
             */

            _applicationManager.RegisterOptionsPageFactory(
                delegate { return new GeneralOptionsPage(); });

            /*
             * Restore the document state after the window is loaded
             * otherwise the drop-down window list doesn't get shown.
             */

            Load += delegate
            {
                /*
                 * Restore the saved dockpanel state.
                 */

                SetDockPanelState();

                /*
                 * Allow plugins to set default window states
                 * if not restored from the saved config.
                 */

                if (DockPanelPostLoad != null)
                    DockPanelPostLoad();

                /*
                 * Load documents from the command line.
                 */

                LoadDocuments();
            };
        }

        #region Public UI Elements

        /// <summary>
        /// The application main menu.
        /// </summary>
        public MenuStrip MainMenu
        {
            get { return _mainMenuStrip; }
        }

        /// <summary>
        /// The application main toolbar.
        /// </summary>
        public ToolStrip MainToolbar
        {
            get { return GetDockedToolStrip(Constants.MAIN_TOOLBAR_NAME).ToolStrip; }
        }

        /// <summary>
        /// The application toolbars.
        /// </summary>
        public List<DockedToolStrip> DockedToolbars
        {
            get { return _dockedToolStrips.Values.ToList<DockedToolStrip>(); }
        }

        /// <summary>
        /// The docking window manager.
        /// </summary>
        public DockPanel ClientWindow
        {
            get { return _dockPanel; }
        }

        /// <summary>
        /// The application status bar.
        /// </summary>
        public StatusStrip StatusBar
        {
            get { return _mainStatusStrip; }
        }

        /// <summary>
        /// The status bar text message.
        /// </summary>
        public string StatusBarMessage
        {
            get { return _mainStatusStripMessage.Text; }
        }

        /// <summary>
        /// The currently active document.
        /// </summary>
        public Document ActiveDocument
        {
            get { return _dockPanel.ActiveDocument as Document; }
        }

        #endregion

        #region Form State Updates

        private void SetMainFormState()
        {
            WindowState =
                (FormWindowState)Enum.Parse(typeof(FormWindowState),
                    _persistenceManager.ReadString(
                        Constants.KEY_WINDOW_STATE, "Normal"));

            Height = _persistenceManager.ReadInt(
                Constants.KEY_WINDOW_HEIGHT, 500);

            Width = _persistenceManager.ReadInt(
                Constants.KEY_WINDOW_WIDTH, 700);

            Location = new Point(
                _persistenceManager.ReadInt(Constants.KEY_WINDOW_X, 40),
                _persistenceManager.ReadInt(Constants.KEY_WINDOW_Y, 40));

            _mainStatusStrip.Visible = UI_VIEW_MENU_STATUSBAR.Checked =
                _persistenceManager.ReadBoolean(
                    Constants.KEY_STATUSBAR_VISIBLE, true);

            Text = _profile.ClientTitle;
            Icon = _profile.ClientIcon;
        }

        private void SetDockPanelState()
        {
            if (!_applicationManager.HaveDockPanelConfig)
                return;

            try
            {
                ClientWindow.SuspendLayout(true);
                ClientWindow.LoadFromXml(
                    _applicationManager.DockPanelConfigFile,
                    new DeserializeDockContent(GetDockPanelFromName));
                ClientWindow.ResumeLayout(true, true);
            }
            catch (Exception ex)
            {
                File.Delete(_applicationManager.DockPanelConfigFile);
                throw new Exception(ex.Message);
            }
        }

        private IDockContent GetDockPanelFromName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return null;

            /*
             * Get the form if it's a registered docked form.
             */

            IDockContent dockContent =
                _applicationManager.GetDockedForm(name) as IDockContent;

            /*
             * Not found as a docked form, try loading as a document
             * if document reload is enabled.
             */

            if (dockContent == null && _restoreDocumentsFromLastSession)
                dockContent = LoadDocument(name);

            return dockContent;
        }

        private void SetApplicationHelpState()
        {
            if (String.IsNullOrEmpty(_profile.HelpFileName) ||
                String.IsNullOrEmpty(_profile.HelpFileTitle))
                return;

            ToolStripMenuItem helpMenuHelp =
                MenuTools.CreateMenuItem(
                    Constants.UI_HELP_MENU_HELP,
                    _profile.HelpFileTitle, null,
                    _profile.HelpShortcutKeys, null,
                    UI_HELP_MENU_HELP_Click, true);

            if (_profile.HelpMenuImage != null)
            {
                helpMenuHelp.Image = _profile.HelpMenuImage;
                helpMenuHelp.ImageTransparentColor = Color.Fuchsia;
            }

            UI_HELP_MENU.DropDownItems.Insert(0, helpMenuHelp);

            string helpFilePath = Path.Combine(
                _applicationManager.QuickSharpHome,
                _profile.HelpFileName);

            if (!File.Exists(helpFilePath))
                helpMenuHelp.Enabled = false;
        }

        private void SetUpdateCheckState()
        {
            if (String.IsNullOrEmpty(_profile.UpdateCheckURL))
                return;

            ToolStripMenuItem helpMenuUpdateCheck = 
                MenuTools.CreateMenuItem(
                    Constants.UI_HELP_MENU_UPDATE_CHECK,
                    Resources.MainHelpMenuCheckForUpdates,
                    null, Keys.None, null,
                    UI_HELP_MENU_CHECK_FOR_UPDATES_Click, true);

            int i = UI_HELP_MENU.DropDownItems.IndexOf(UI_HELP_MENU_ABOUT);

            if (i != -1)
                UI_HELP_MENU.DropDownItems.Insert(i, helpMenuUpdateCheck);
        }

        private void LoadDocuments()
        {
            if (_applicationManager.OpenDocumentHandlers.Count == 0)
                return;

            /*
             * Attempt to load any files passed on the command line.
             */

            foreach (string doc in _profile.CommandLineArgs)
            {
                if (doc[0] == '/' || doc[0] == '-') continue;

                FileInfo fi = new FileInfo(doc);

                LoadDocumentIntoWindow(fi.FullName, true);
            }
        }

        #endregion

        #region Menu and Toolbar Item Updates

        private void UpdateMenuItems()
        {
            /*
             * Go through each menu item in the main menus and add a
             * separator after every item with the 'IncludeSeparator'
             * flag set.
             */

            foreach (ToolStripMenuItem menu in MainMenu.Items)
                UpdateMenuDropDownItems(menu);
        }

        private void UpdateMenuDropDownItems(ToolStripMenuItem menu)
        {
            for (int i = 0; i < menu.DropDownItems.Count; i++)
            {
                ToolStripItem item = menu.DropDownItems[i];

                ToolStripItemTag tag = item.Tag as ToolStripItemTag;
                if (tag != null && tag.IncludeSeparator)
                {
                    ToolStripSeparator sep =
                        MenuTools.CreateSeparator(item.Name + "_SEP");

                    int j = menu.DropDownItems.IndexOf(item);

                    /*
                     * Menu separator occurs after the menu.
                     */

                    if (j != -1)
                    {
                        menu.DropDownItems.Insert(j + 1, sep);
                        i++; // increment i to skip new item
                    }
                }

                /*
                 * If the item is a menu recurse into any submenus.
                 */

                if (item is ToolStripMenuItem)
                {
                    ToolStripMenuItem menuItem = item as ToolStripMenuItem;

                    if (menuItem.DropDownItems.Count > 0)
                        UpdateMenuDropDownItems(menuItem);
                }
            }

            /*
             * The rule is that evey menu item group should end on a
             * separator. In some cases this will mean the last item
             * is a separator so we go through each menu and remove them.
             */

            int count = menu.DropDownItems.Count;

            if (count > 0)
            {
                ToolStripItem lastItem = menu.DropDownItems[count - 1];
                if (lastItem is ToolStripSeparator)
                    menu.DropDownItems.RemoveAt(count - 1);
            }
        }

        private void UpdateToolbarItems()
        {
            foreach (DockedToolStrip toolStrip in _dockedToolStrips.Values)
            {
                /*
                 * Go through each button in the toolbars and add a
                 * separator after every item with the 'IncludeSeparator'
                 * flag set.
                 */

                for (int i = 0; i < toolStrip.ToolStrip.Items.Count; i++)
                {
                    ToolStripItem item = toolStrip.ToolStrip.Items[i];

                    ToolStripItemTag tag = item.Tag as ToolStripItemTag;
                    if (tag != null && tag.IncludeSeparator)
                    {
                        ToolStripSeparator sep =
                            MenuTools.CreateSeparator(item.Name + "_SEP");

                        /*
                         * Toolbar separator occurs before the button.
                         */

                        int j = toolStrip.ToolStrip.Items.IndexOf(item);
                        if (j != -1)
                        {
                            toolStrip.ToolStrip.Items.Insert(j, sep);
                            i++;  // increment i to skip button
                        }
                    }
                }
            }
        }

        #endregion

        #region Form Event Handlers

        private void MainForm_ActiveDocumentChanged(
            object sender, EventArgs e)
        {
            /*
             * Need to enable/disable these independently
             * of the menus for the shortcut keys
             */

            UI_FILE_MENU_SAVE.Enabled =
                IsMenuEnabled(UI_FILE_MENU_SAVE);
            UI_TOOLBAR_SAVE.Enabled =
                IsToolbarButtonEnabled(UI_TOOLBAR_SAVE);

            int saveDocumentCount = 0;
            foreach (Document doc in _dockPanel.Documents)
                if (doc.IsMenuItemEnabled(UI_FILE_MENU_SAVE))
                    saveDocumentCount++;

            UI_FILE_MENU_SAVE_ALL.Enabled =
                (saveDocumentCount > 0);
            UI_TOOLBAR_SAVE_ALL.Enabled =
                (saveDocumentCount > 0);
        }

        private void MainForm_FormClosing(
            object sender, FormClosingEventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                /*
                 * Write the registry settings.
                 */

                _persistenceManager.WriteString(
                    Constants.KEY_QUICKSHARP_HOME,
                    _applicationManager.QuickSharpHome);

                _persistenceManager.WriteString(
                    Constants.KEY_WINDOW_STATE, WindowState.ToString());

                if (WindowState == FormWindowState.Normal)
                {
                    _persistenceManager.WriteInt(
                        Constants.KEY_WINDOW_HEIGHT, this.Height);
                    _persistenceManager.WriteInt(
                        Constants.KEY_WINDOW_WIDTH, this.Width);
                    _persistenceManager.WriteInt(
                        Constants.KEY_WINDOW_X, this.Location.X);
                    _persistenceManager.WriteInt(
                        Constants.KEY_WINDOW_Y, this.Location.Y);
                }

                _persistenceManager.WriteStrings(
                    Constants.KEY_TOOLBAR_STATES, EncodeToolStripStatus());

                _persistenceManager.WriteBoolean(
                    Constants.KEY_STATUSBAR_VISIBLE, _mainStatusStrip.Visible);

                _persistenceManager.WriteStrings(
                    Constants.KEY_MRU_DOCUMENTS_LIST, _recentlyUsedDocuments);

                /*
                 * Save the dock panel layout.
                 */

                ClientWindow.SaveAsXml(
                    _applicationManager.DockPanelConfigFile);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Menu Event Handlers

        private void UI_FILE_MENU_DropDownOpening(
            object sender, EventArgs e)
        {
            UI_FILE_MENU_SAVE.Enabled =
                IsMenuEnabled(UI_FILE_MENU_SAVE);
            UI_FILE_MENU_SAVE_AS.Enabled =
                IsMenuEnabled(UI_FILE_MENU_SAVE_AS);

            int saveDocumentCount = 0;
            foreach (Document doc in _dockPanel.Documents)
                if (doc.IsMenuItemEnabled(UI_FILE_MENU_SAVE))
                    saveDocumentCount++;

            UI_FILE_MENU_SAVE_ALL.Enabled =
                (saveDocumentCount > 0);

            UI_FILE_MENU_RECENT_FILES.Enabled = false;

            if (_profile.MRUDocumentListMax > 0)
            {
                UpdateRecentDocumentsList();

                if (_recentlyUsedDocuments.Count > 0)
                {
                    UI_FILE_MENU_RECENT_FILES.Enabled = true;
                    UI_FILE_MENU_RECENT_FILES.DropDownItems.Clear();

                    int documentCount = _recentlyUsedDocuments.Count;

                    bool applyThemeColor = 
                        _profile.ThemeFlags != null &&
                        _profile.ThemeFlags.MenuForeColor != Color.Empty;

                    foreach (string path in _recentlyUsedDocuments)
                    {
                        ToolStripMenuItem item = new ToolStripMenuItem();

                        item.Text = String.Format("{0}{1} {2}",
                            documentCount < 10 ? "&" : "",
                            documentCount--,
                            Path.GetFileName(path));

                        item.ToolTipText = path;

                        item.Click += new EventHandler(
                            UI_FILE_MENU_RECENT_FILES_Item_Click);

                        // Apply theme color if available.
                        if (applyThemeColor)
                            item.ForeColor = _profile.ThemeFlags.MenuForeColor;

                        // Insert so that latest file appears first.
                        UI_FILE_MENU_RECENT_FILES.DropDownItems.Insert(0, item);
                    }
                }
            }
        }

        private void UpdateRecentDocumentsList()
        {
            List<String> newList = new List<String>();

            foreach(string document in _recentlyUsedDocuments)
                if (File.Exists(document))
                    newList.Add(document);

            _recentlyUsedDocuments = newList;
        }

        private void UI_FILE_MENU_NEW_Click(
            object sender, EventArgs e)
        {
            NewDocument();
        }

        private void UI_FILE_MENU_OPEN_Click(
            object sender, EventArgs e)
        {
            OpenDocument();
        }

        private void UI_FILE_MENU_SAVE_Click(
            object sender, EventArgs e)
        {
            if (ActiveDocument == null) return;
            ActiveDocument.PerformMenuAction(UI_FILE_MENU_SAVE);
        }

        private void UI_FILE_MENU_SAVE_AS_Click(
            object sender, EventArgs e)
        {
            if (ActiveDocument == null) return;
            ActiveDocument.PerformMenuAction(UI_FILE_MENU_SAVE_AS);
        }

        private void UI_FILE_MENU_SAVE_ALL_Click(
            object sender, EventArgs e)
        {
            SaveAllDocuments();
        }

        private void UI_FILE_MENU_RECENT_FILES_Item_Click(
            object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null) return;

            LoadDocumentIntoWindow(item.ToolTipText, true);
        }

        private void UI_FILE_MENU_EXIT_Click(
            object sender, EventArgs e)
        {
            Refresh();
            Close();
        }

        private void UI_VIEW_MENU_DropDownOpeningSingle(
            object sender, EventArgs e)
        {
            UI_VIEW_MENU_TOOLBAR.Checked = 
                _dockedToolStrips[Constants.MAIN_TOOLBAR_NAME].Visible;
            UI_VIEW_MENU_FULLSCREEN.Checked = _showFullScreen;
        }

        private void UI_VIEW_MENU_DropDownOpeningMulti(
            object sender, EventArgs e)
        {
            UI_VIEW_MENU_TOOLBAR.Enabled = _dockedToolStrips.Count > 0;
            UI_VIEW_MENU_FULLSCREEN.Checked = _showFullScreen;
        }

        private void UI_VIEW_MENU_STATUSBAR_Click(
            object sender, EventArgs e)
        {
            _mainStatusStrip.Visible = UI_VIEW_MENU_STATUSBAR.Checked;
        }

        private void UI_VIEW_MENU_FULLSCREEN_Click(
            object sender, EventArgs e)
        {
            SuspendLayout();

            /*
             * WindowState transitions go though Normal
             * each way to ensure correct transition from
             * Maximized to full screen and back.
             */

            if (!_showFullScreen)
            {
                _showFullScreen = true;
                _restoredWindowState = WindowState;
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Normal;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                _showFullScreen = false;
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;
                WindowState = _restoredWindowState;
            }

            ResumeLayout();
        }

        private void UI_TOOLS_MENU_OPTIONS_Click(
            object sender, EventArgs e)
        {
            OptionsForm of = new OptionsForm();

            of.SelectPage(_selectedOptionsPageName);
            of.ShowDialog();
            _selectedOptionsPageName = of.SelectedPageName;
        }

        private void UI_WINDOW_MENU_DropDownOpening(
            object sender, EventArgs e)
        {
            UI_WINDOW_MENU_CLOSE.Enabled =
                (_dockPanel.DocumentsCount > 0);
            UI_WINDOW_MENU_CLOSE_ALL.Enabled =
                (_dockPanel.DocumentsCount > 1);
            UI_WINDOW_MENU_CLOSE_ALL_BUT_ACTIVE.Enabled =
                (_dockPanel.DocumentsCount > 1);
        }

        private void UI_WINDOW_MENU_CLOSE_Click(
            object sender, EventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.Close();
        }

        private void UI_WINDOW_MENU_CLOSE_ALL_Click(
            object sender, EventArgs e)
        {
            IDockContent[] docs = _dockPanel.DocumentsToArray();
            foreach (Document doc in docs)
                doc.Close();
        }

        private void UI_WINDOW_MENU_CLOSE_ALL_BUT_ACTIVE_Click(
            object sender, EventArgs e)
        {
            Document activeDocument = ActiveDocument;
            IDockContent[] docs = _dockPanel.DocumentsToArray();
            foreach (Document doc in docs)
                if (doc != activeDocument)
                    doc.Close();
        }

        private void UI_HELP_MENU_HELP_Click(object sender, EventArgs e)
        {
            string helpFilePath = Path.Combine(
                _applicationManager.QuickSharpHome,
                _profile.HelpFileName);

            if (!File.Exists(helpFilePath)) return;

            try
            {
                FileTools.ShellOpenFile(helpFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}:\r\n{1}",
                        Resources.HelpOpenErrorMessage,
                        ex.Message),
                    Resources.HelpOpenErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void UI_HELP_MENU_CHECK_FOR_UPDATES_Click(object sender, EventArgs e)
        {
            _profile.UpdateCheckFormFactory().ShowDialog();
        }
        
        private void UI_HELP_MENU_ABOUT_Click(
            object sender, EventArgs e)
        {
            _profile.AboutBoxFactory().ShowDialog();
        }

        #endregion

        #region Document Operations

        /// <summary>
        /// Create a document. Uses the registered new document handler to
        /// create a new document.
        /// </summary>
        public void NewDocument()
        {
            NewDocumentHandler handler = 
                _applicationManager.NewDocumentHandler;

            if (handler != null)
            {
                IDockContent doc = handler();

                if (doc != null)
                    doc.DockHandler.Show(ClientWindow, DockState.Document);
            }
        }

        /// <summary>
        /// Open a document. Presents a file open dialog to get the path of the document
        /// and uses the open document handler registered for the file's document type
        /// to open the file.
        /// </summary>
        public void OpenDocument()
        {
            string originalDirectory = Directory.GetCurrentDirectory();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = _applicationManager.DocumentFilter;
            ofd.InitialDirectory = Directory.GetCurrentDirectory();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (LoadDocumentIntoWindow(ofd.FileName, true) &&
                    originalDirectory != Directory.GetCurrentDirectory())
                    _applicationManager.NotifyFileSystemChange();
            }
        }

        /// <summary>
        /// Save all the currently active documents.
        /// </summary>
        /// <returns>True if all documents were saved successfully.</returns>
        public bool SaveAllDocuments()
        {
            bool res = true;

            foreach (IDockContent dockContent in _dockPanel.Documents)
            {
                Document document = dockContent as Document;
                if (document != null)
                    if (!document.PerformAction("UI_FILE_MENU_SAVE"))
                        res = false;
            }

            return res;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Determines if a docked form has been loaded.
        /// </summary>
        /// <param name="key">The docked form key.</param>
        /// <returns>True if the docked form is present.</returns>
        public bool HaveDockedFormContent(string key)
        {
            foreach (DockContent content in ClientWindow.Contents)
            {
                string persistName =
                    content.DockHandler.GetPersistStringCallback();

                if (persistName == key)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Get the index of a main menu item. This provides access to the top
        /// level menus only.
        /// </summary>
        /// <param name="name">The name of the menu.</param>
        /// <returns>The index of the menu or -1 if not found.</returns>
        public int GetMainMenuItemIndexByName(string name)
        {
            return _mainMenuStrip.Items.IndexOfKey(name);
        }

        /// <summary>
        /// Get a menu item by name. This provides access to all the menu items
        /// present in the application main menu.
        /// </summary>
        /// <param name="name">The name of the menu item.</param>
        /// <returns>The menu item or null if not found.</returns>
        public ToolStripMenuItem GetMenuItemByName(string name)
        {
            ToolStripItem[] items =
                _mainMenuStrip.Items.Find(name, true);
            
            if (items.Length < 1)
                return null;
            else
                return items[0] as ToolStripMenuItem;
        }

        /// <summary>
        /// Determine if a menu item is enabled for the active document.
        /// Calls the document's ActionState handler to determine the menu state.
        /// </summary>
        /// <param name="menuItem">The name of the menu item.</param>
        /// <returns>True if the menu is enabled. False if not or if
        /// there is no active document.</returns>
        public bool IsMenuEnabled(ToolStripMenuItem menuItem)
        {
            if (ActiveDocument == null)
                return false;
            else
                return ActiveDocument.IsMenuItemEnabled(menuItem);
        }

        /// <summary>
        /// Determine if a toolbar button is enabled for the active document.
        /// Calls the document's ActionState handler to determine the button state.
        /// </summary>
        /// <param name="button">The name of the button.</param>
        /// <returns>True if the button is enabled. False if not or if
        /// there is no active document.</returns>
        public bool IsToolbarButtonEnabled(ToolStripButton button)
        {
            if (ActiveDocument == null)
                return false;
            else
                return ActiveDocument.IsToolbarButtonEnabled(button);
        }

        /// <summary>
        /// Determine if a file with the specified path has been loaded
        /// into a document.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>True if the document has been loaded.</returns>
        public bool HaveDocument(string path)
        {
            foreach (IDockContent dc in ClientWindow.Documents)
            {
                Document doc = dc as Document;
                if (dc != null && doc.FilePath == path)
                    return true;
            }

            return false;
        }

        #endregion

        #region Load Document

        private void AddDocumentToMRUList(string path)
        {
            if (_profile.MRUDocumentListMax > 0)
            {
                // If already in the list remove.
                if (_recentlyUsedDocuments.Contains(path))
                    _recentlyUsedDocuments.Remove(path);

                // Add to list as latest document.
                _recentlyUsedDocuments.Add(path);

                // Trim the list if over limit.
                if (_recentlyUsedDocuments.Count > _profile.MRUDocumentListMax)
                    _recentlyUsedDocuments.RemoveAt(0);
            }
        }

        private void RemoveDocumentFromMRUList(string path)
        {
            if (_profile.MRUDocumentListMax > 0 &&
                _recentlyUsedDocuments.Contains(path))
                _recentlyUsedDocuments.Remove(path);
        }

        /// <summary>
        /// Call LoadDocument with the specified path and display the loaded file
        /// in a document window. Can optionally add the loaded file path to the most
        /// recently used (MRU) file list (if enabled).
        /// </summary>
        /// <param name="path">The path of the document.</param>
        /// <param name="useMRUList">Use the MRU document list.</param>
        /// <returns>True if the document is loaded successfully.</returns>
        public bool LoadDocumentIntoWindow(string path, bool useMRUList)
        {
            IDockContent doc = LoadDocument(path);

            if (useMRUList)
            {
                if (doc != null) // Load successful
                {
                    AddDocumentToMRUList(path);
                }
                else // Load failed or document already open.
                {
                    RemoveDocumentFromMRUList(path);

                    if (HaveDocument(path))
                        AddDocumentToMRUList(path);
                }
            }

            if (doc != null)
                doc.DockHandler.Show(ClientWindow, DockState.Document);

            return (doc != null);
        }

        /// <summary>
        /// Loads a document from the specified path. This performs several steps in the following
        /// order. The path is converted to a DocumentType. Next, each of the registered
        /// document preload handlers is called, if any return false the load is cancelled.
        /// The document type is used to retrieve the open document handler registered for that
        /// type and the handler is used to open the document.
        /// If no handler is available the document is (otionally) passed to the Windows shell
        /// to be opened externally. If this fails the unknown document handler is
        /// retrieved and used to open the document. If this is not available
        /// an optional message is shown and the load fails.
        /// </summary>
        /// <param name="path">The path of the document.</param>
        /// <returns>The loaded document.</returns>
        public IDockContent LoadDocument(string path)
        {
            ClientProfile profile = _applicationManager.ClientProfile;

            /*
             * Look for readonly flag - path will end with '?'. This is really
             * only for the benefit of the document serialisation which needs
             * to be able to restore a readonly file from the save document
             * configuration.
             */

            bool readOnly = false;

            if (path.EndsWith("?"))
            {
                readOnly = true;
                path = path.TrimEnd('?');
            }

            DocumentType documentType = new DocumentType(path);

            /*
             * Document PreLoad Handlers
             * 
             * The idea here is to allow applications to create a chain
             * of filters each being responsible for performing some
             * action on a particular document type. Each handler
             * should immediately return true if it's not interested
             * in the document type passed. The chain will end when the
             * first handler returns false and the document load will be
             * cancelled. Since there is no guarantee of the order in
             * which the handlers are called (registration order will
             * depend on plugin load order which can vary) there should
             * be only one handler for a particular type unless they are
             * completely independent.
             */

            foreach (DocumentPreLoadHandler d in
                _applicationManager.DocumentPreLoadHandlers)
                if (!d(documentType)) return null;

            OpenDocumentHandler handler = 
                _applicationManager.GetOpenDocumentHandler(documentType);

            if (handler == null && _allowShellOpen)
            {
                try
                {
                    FileTools.ShellOpenFile(path);
                    return null;
                }
                catch
                {
                    /*
                     * Suppress error - if the load fails the document
                     * gets passed to the unknown document handler.
                     */
                }
            }

            if (handler == null)
                handler = _applicationManager.UnknownDocumentHandler;

            if (handler == null)
            {
                if (_showNoHandlerMessage)
                    MessageBox.Show(
                        Resources.NoHandlerMessage,
                        Resources.NoHandlerTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                return null;
            }
            else
            {
                return handler(path, readOnly);
            }
        }

        #endregion

        #region Main Toolbar

        private ToolStrip _mainToolStrip;
        private ToolStripButton UI_TOOLBAR_NEW;
        private ToolStripButton UI_TOOLBAR_OPEN;
        private ToolStripButton UI_TOOLBAR_SAVE;
        private ToolStripButton UI_TOOLBAR_SAVE_ALL;

        private void CreateMainToolbar()
        {
            _mainToolStrip = new ToolStrip();
            _mainToolStrip.Name = Constants.MAIN_TOOLBAR_NAME;
            _mainToolStrip.Text = Resources.MainToolbarText;

            UI_TOOLBAR_NEW = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_NEW, Resources.MainToolbarNew,
                Resources.New, UI_TOOLBAR_NEW_Click);

            UI_TOOLBAR_OPEN = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_OPEN, Resources.MainToolbarOpen,
                Resources.Open, UI_TOOLBAR_OPEN_Click);

            UI_TOOLBAR_SAVE = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_SAVE, Resources.MainToolbarSave,
                Resources.Save, UI_TOOLBAR_SAVE_Click);

            UI_TOOLBAR_SAVE_ALL = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_SAVE_ALL, Resources.MainToolbarSaveAll,
                Resources.SaveAll, UI_TOOLBAR_SAVE_ALL_Click);

            UI_TOOLBAR_SAVE.Enabled = false;
            UI_TOOLBAR_SAVE_ALL.Enabled = false;

            _mainToolStrip.Items.AddRange(new ToolStripItem[] {
                UI_TOOLBAR_NEW, UI_TOOLBAR_OPEN,
                UI_TOOLBAR_SAVE, UI_TOOLBAR_SAVE_ALL});
            
            AddDockedToolStrip(_mainToolStrip, 0, 0);
        }

        private void UI_VIEW_MENU_TOOLBAR_DropDownOpening(
            object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem menuItem in
                UI_VIEW_MENU_TOOLBAR.DropDownItems)
            {
                string name = menuItem.Tag as string;
                if (String.IsNullOrEmpty(name)) continue;

                if (_dockedToolStrips.ContainsKey(name))
                    menuItem.Checked = _dockedToolStrips[name].Visible;
            }
        }

        private void UI_VIEW_MENU_TOOLBAR_ITEM_Click(
            object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (sender == null) return;

            string name = menuItem.Tag as string;
            if (String.IsNullOrEmpty(name)) return;

            if (_dockedToolStrips.ContainsKey(name))
                _dockedToolStrips[name].Visible = !_dockedToolStrips[name].Visible;
        }

        private void UI_TOOLBAR_NEW_Click(
            object sender, EventArgs e)
        {
            NewDocument();
        }

        private void UI_TOOLBAR_OPEN_Click(
            object sender, EventArgs e)
        {
            OpenDocument();
        }

        private void UI_TOOLBAR_SAVE_Click(
            object sender, EventArgs e)
        {
            if (ActiveDocument == null) return;
            ActiveDocument.PerformToolbarAction(UI_TOOLBAR_SAVE);
        }

        private void UI_TOOLBAR_SAVE_ALL_Click(
            object sender, EventArgs e)
        {
            SaveAllDocuments();
        }

        #endregion
    }
}
