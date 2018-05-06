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
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.Core
{
    partial class MainForm
    {
        #region Controls

        private MenuStrip _mainMenuStrip;
        private ToolStripPanel _mainToolStripPanel;
        private DockPanel _dockPanel;
        private StatusStrip _mainStatusStrip;
        private Timer _mainStatusStripTimer;
        private ToolStripStatusLabel _mainStatusStripMessage;
        private ToolStripMenuItem UI_FILE_MENU;
        private ToolStripMenuItem UI_FILE_MENU_EXIT;
        private ToolStripMenuItem UI_HELP_MENU;
        private ToolStripMenuItem UI_HELP_MENU_ABOUT;
        private ToolStripMenuItem UI_VIEW_MENU;
        private ToolStripMenuItem UI_VIEW_MENU_TOOLBAR;
        private ToolStripMenuItem UI_VIEW_MENU_STATUSBAR;
        private ToolStripMenuItem UI_VIEW_MENU_FULLSCREEN;
        private ToolStripMenuItem UI_FILE_MENU_NEW;
        private ToolStripMenuItem UI_FILE_MENU_OPEN;
        private ToolStripMenuItem UI_FILE_MENU_SAVE;
        private ToolStripMenuItem UI_FILE_MENU_SAVE_AS;
        private ToolStripMenuItem UI_FILE_MENU_SAVE_ALL;
        private ToolStripMenuItem UI_FILE_MENU_RECENT_FILES;
        private ToolStripMenuItem UI_TOOLS_MENU;
        private ToolStripMenuItem UI_TOOLS_MENU_OPTIONS;
        private ToolStripMenuItem UI_WINDOW_MENU;
        private ToolStripMenuItem UI_WINDOW_MENU_CLOSE;
        private ToolStripMenuItem UI_WINDOW_MENU_CLOSE_ALL;
        private ToolStripMenuItem UI_WINDOW_MENU_CLOSE_ALL_BUT_ACTIVE;

        #endregion

        private IContainer components = null;
        private Dictionary<String, DockedToolStrip> _dockedToolStrips; 

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Layout

        private void InitializeComponent()
        {
            _dockedToolStrips = new Dictionary<String, DockedToolStrip>();

            _mainMenuStrip = new MenuStrip();
            _mainToolStripPanel = new ToolStripPanel();
            _dockPanel = new DockPanel();
            _mainStatusStrip = new StatusStrip();
            _mainStatusStripMessage = new ToolStripStatusLabel();            

            _mainStatusStripTimer = new Timer();
            _mainStatusStripTimer.Enabled = false;
            _mainStatusStripTimer.Tick += 
                new EventHandler(MainStatusStripTimer_Tick);

            SuspendLayout();
            _mainMenuStrip.SuspendLayout();
            _mainToolStripPanel.SuspendLayout();
            _dockPanel.SuspendLayout();
            _mainStatusStrip.SuspendLayout();

            #region Main Menu

            UI_FILE_MENU = new ToolStripMenuItem();
            UI_FILE_MENU_NEW = new ToolStripMenuItem();
            UI_FILE_MENU_OPEN = new ToolStripMenuItem();
            UI_FILE_MENU_SAVE = new ToolStripMenuItem();
            UI_FILE_MENU_SAVE_AS = new ToolStripMenuItem();
            UI_FILE_MENU_SAVE_ALL = new ToolStripMenuItem();
            UI_FILE_MENU_RECENT_FILES = new ToolStripMenuItem();
            UI_FILE_MENU_EXIT = new ToolStripMenuItem();
            UI_VIEW_MENU = new ToolStripMenuItem();
            UI_VIEW_MENU_TOOLBAR = new ToolStripMenuItem();
            UI_VIEW_MENU_STATUSBAR = new ToolStripMenuItem();
            UI_VIEW_MENU_FULLSCREEN = new ToolStripMenuItem();
            UI_TOOLS_MENU = new ToolStripMenuItem();
            UI_TOOLS_MENU_OPTIONS = new ToolStripMenuItem();
            UI_WINDOW_MENU = new ToolStripMenuItem();
            UI_WINDOW_MENU_CLOSE = new ToolStripMenuItem();
            UI_WINDOW_MENU_CLOSE_ALL = new ToolStripMenuItem();
            UI_WINDOW_MENU_CLOSE_ALL_BUT_ACTIVE = new ToolStripMenuItem();
            UI_HELP_MENU = new ToolStripMenuItem();
            UI_HELP_MENU_ABOUT = new ToolStripMenuItem();

            _mainMenuStrip.Location = new Point(0, 0);
            _mainMenuStrip.Name = "mainMenuStrip";
            _mainMenuStrip.Size = new Size(692, 24);
            _mainMenuStrip.TabIndex = 0;
            _mainMenuStrip.Text = "menuStrip1";
            _mainMenuStrip.Items.AddRange(new ToolStripItem[] {
                UI_FILE_MENU,
                UI_VIEW_MENU,
                UI_TOOLS_MENU,
                UI_WINDOW_MENU,
                UI_HELP_MENU});

            // UI_FILE_MENU
            UI_FILE_MENU.DropDownItems.AddRange(new ToolStripItem[] {
            UI_FILE_MENU_NEW,
            UI_FILE_MENU_OPEN,
            UI_FILE_MENU_SAVE,
            UI_FILE_MENU_SAVE_AS,
            UI_FILE_MENU_SAVE_ALL});
            
            if (_profile.MRUDocumentListMax > 0)
                UI_FILE_MENU.DropDownItems.Add(UI_FILE_MENU_RECENT_FILES);

            UI_FILE_MENU.DropDownItems.Add(UI_FILE_MENU_EXIT);

            UI_FILE_MENU.Name = Constants.UI_FILE_MENU;
            UI_FILE_MENU.Text = Resources.MainFileMenu;
            UI_FILE_MENU.DropDownOpening += new System.EventHandler(UI_FILE_MENU_DropDownOpening);

            // UI_FILE_MENU_NEW
            UI_FILE_MENU_NEW.Image = Resources.New;
            UI_FILE_MENU_NEW.ImageTransparentColor = Color.Fuchsia;
            UI_FILE_MENU_NEW.Name = Constants.UI_FILE_MENU_NEW;
            UI_FILE_MENU_NEW.ShortcutKeys = ((Keys)((Keys.Control | Keys.N)));
            UI_FILE_MENU_NEW.Text = Resources.MainFileMenuNew;
            UI_FILE_MENU_NEW.Click += new System.EventHandler(UI_FILE_MENU_NEW_Click);

            // UI_FILE_MENU_OPEN
            UI_FILE_MENU_OPEN.Image = Resources.Open;
            UI_FILE_MENU_OPEN.ImageTransparentColor = Color.Fuchsia;
            UI_FILE_MENU_OPEN.Name = Constants.UI_FILE_MENU_OPEN;
            UI_FILE_MENU_OPEN.ShortcutKeys = ((Keys)((Keys.Control | Keys.O)));
            UI_FILE_MENU_OPEN.Text = Resources.MainFileMenuOpen;
            UI_FILE_MENU_OPEN.Click += new System.EventHandler(UI_FILE_MENU_OPEN_Click);
            UI_FILE_MENU_OPEN.Tag = new ToolStripItemTag() { IncludeSeparator = true };

            // UI_FILE_MENU_SAVE
            UI_FILE_MENU_SAVE.Image = Resources.Save;
            UI_FILE_MENU_SAVE.ImageTransparentColor = Color.Fuchsia;
            UI_FILE_MENU_SAVE.Name = Constants.UI_FILE_MENU_SAVE;
            UI_FILE_MENU_SAVE.ShortcutKeys = ((Keys)((Keys.Control | Keys.S)));
            UI_FILE_MENU_SAVE.Text = Resources.MainFileMenuSave;
            UI_FILE_MENU_SAVE.Click += new System.EventHandler(UI_FILE_MENU_SAVE_Click);

            // UI_FILE_MENU_SAVE_AS
            UI_FILE_MENU_SAVE_AS.Name = Constants.UI_FILE_MENU_SAVE_AS;
            UI_FILE_MENU_SAVE_AS.Text = Resources.MainFileMenuSaveAs;
            UI_FILE_MENU_SAVE_AS.Click += new System.EventHandler(UI_FILE_MENU_SAVE_AS_Click);

            // UI_FILE_MENU_SAVE_ALL
            UI_FILE_MENU_SAVE_ALL.Image = Resources.SaveAll;
            UI_FILE_MENU_SAVE_ALL.ImageTransparentColor = Color.Fuchsia;
            UI_FILE_MENU_SAVE_ALL.Name = Constants.UI_FILE_MENU_SAVE_ALL;
            UI_FILE_MENU_SAVE_ALL.ShortcutKeys = ((Keys)(((Keys.Control | Keys.Shift)
                        | Keys.S)));
            UI_FILE_MENU_SAVE_ALL.Text = Resources.MainFileMenuSaveAll;
            UI_FILE_MENU_SAVE_ALL.Click += new System.EventHandler(UI_FILE_MENU_SAVE_ALL_Click);
            UI_FILE_MENU_SAVE_ALL.Tag = new ToolStripItemTag() { IncludeSeparator = true };

            // UI_FILE_MENU_RECENT_FILES
            UI_FILE_MENU_RECENT_FILES.Name = Constants.UI_FILE_MENU_RECENT_FILES;
            UI_FILE_MENU_RECENT_FILES.Text = Resources.MainFileMenuRecentFiles;
            UI_FILE_MENU_RECENT_FILES.Tag = new ToolStripItemTag() { IncludeSeparator = true };

            // UI_FILE_MENU_EXIT
            UI_FILE_MENU_EXIT.Name = Constants.UI_FILE_MENU_EXIT;
            UI_FILE_MENU_EXIT.Text = Resources.MainFileMenuExit;
            UI_FILE_MENU_EXIT.Click += new System.EventHandler(UI_FILE_MENU_EXIT_Click);
            UI_FILE_MENU_EXIT.Tag = new ToolStripItemTag() { IncludeSeparator = true };

            // UI_VIEW_MENU
            UI_VIEW_MENU.DropDownItems.AddRange(new ToolStripItem[] {
            UI_VIEW_MENU_TOOLBAR,
            UI_VIEW_MENU_STATUSBAR,
            UI_VIEW_MENU_FULLSCREEN});
            UI_VIEW_MENU.Name = Constants.UI_VIEW_MENU;
            UI_VIEW_MENU.Text = Resources.MainViewMenu;

            // UI_VIEW_MENU_TOOLBARS
            UI_VIEW_MENU_TOOLBAR.Name = Constants.UI_VIEW_MENU_TOOLBAR;

            // UI_VIEW_MENU_STATUSBAR
            UI_VIEW_MENU_STATUSBAR.Checked = true;
            UI_VIEW_MENU_STATUSBAR.CheckOnClick = true;
            UI_VIEW_MENU_STATUSBAR.CheckState = CheckState.Checked;
            UI_VIEW_MENU_STATUSBAR.Name = Constants.UI_VIEW_MENU_STATUSBAR;
            UI_VIEW_MENU_STATUSBAR.Text = Resources.MainViewMenuStatusBar;
            UI_VIEW_MENU_STATUSBAR.Click += new System.EventHandler(UI_VIEW_MENU_STATUSBAR_Click);
            UI_VIEW_MENU_STATUSBAR.Tag = new ToolStripItemTag() { IncludeSeparator = true };

            // UI_VIEW_MENU_FULLSCREEN
            UI_VIEW_MENU_FULLSCREEN.Name = Constants.UI_VIEW_MENU_FULLSCREEN;
            UI_VIEW_MENU_FULLSCREEN.Text = Resources.MainViewMenuFullScreen;
            UI_VIEW_MENU_FULLSCREEN.Image = Resources.FullScreen;
            UI_VIEW_MENU_FULLSCREEN.ImageTransparentColor = Color.Fuchsia;
            UI_VIEW_MENU_FULLSCREEN.ShortcutKeys = Keys.F11;
            UI_VIEW_MENU_FULLSCREEN.Click += new System.EventHandler(UI_VIEW_MENU_FULLSCREEN_Click);
            UI_VIEW_MENU_FULLSCREEN.Tag = new ToolStripItemTag() { IncludeSeparator = true };

            // UI_TOOLS_MENU
            UI_TOOLS_MENU.DropDownItems.AddRange(new ToolStripItem[] {
            UI_TOOLS_MENU_OPTIONS});
            UI_TOOLS_MENU.Name = Constants.UI_TOOLS_MENU;
            UI_TOOLS_MENU.Text = Resources.MainToolsMenu;

            // UI_TOOLS_MENU_OPTIONS
            UI_TOOLS_MENU_OPTIONS.Name = Constants.UI_TOOLS_MENU_OPTIONS;
            UI_TOOLS_MENU_OPTIONS.Text = Resources.MainToolsMenuOptions;
            UI_TOOLS_MENU_OPTIONS.Click += new System.EventHandler(UI_TOOLS_MENU_OPTIONS_Click);
            UI_TOOLS_MENU_OPTIONS.Tag = new ToolStripItemTag() { IncludeSeparator = true };

            // UI_WINDOW_MENU
            UI_WINDOW_MENU.DropDownItems.AddRange(new ToolStripItem[] {
            UI_WINDOW_MENU_CLOSE,
            UI_WINDOW_MENU_CLOSE_ALL,
            UI_WINDOW_MENU_CLOSE_ALL_BUT_ACTIVE});
            UI_WINDOW_MENU.Name = Constants.UI_WINDOW_MENU;
            UI_WINDOW_MENU.Text = Resources.MainWindowMenu;
            UI_WINDOW_MENU.DropDownOpening += new System.EventHandler(UI_WINDOW_MENU_DropDownOpening);

            // UI_WINDOW_MENU_CLOSE
            UI_WINDOW_MENU_CLOSE.Name = Constants.UI_WINDOW_MENU_CLOSE;
            UI_WINDOW_MENU_CLOSE.Text = Resources.MainWindowMenuClose;
            UI_WINDOW_MENU_CLOSE.Click += new System.EventHandler(UI_WINDOW_MENU_CLOSE_Click);

            // UI_WINDOW_MENU_CLOSE_ALL
            UI_WINDOW_MENU_CLOSE_ALL.Name = Constants.UI_WINDOW_MENU_CLOSE_ALL;
            UI_WINDOW_MENU_CLOSE_ALL.Text = Resources.MainWindowMenuCloseAll;
            UI_WINDOW_MENU_CLOSE_ALL.Click += new System.EventHandler(UI_WINDOW_MENU_CLOSE_ALL_Click);

            // UI_WINDOW_MENU_CLOSE_ALL_BUT_ACTIVE
            UI_WINDOW_MENU_CLOSE_ALL_BUT_ACTIVE.Name = Constants.UI_WINDOW_MENU_CLOSE_ALL_BUT_ACTIVE;
            UI_WINDOW_MENU_CLOSE_ALL_BUT_ACTIVE.Text = Resources.MainWindowMenuCloseAllButActive;
            UI_WINDOW_MENU_CLOSE_ALL_BUT_ACTIVE.Click += new System.EventHandler(UI_WINDOW_MENU_CLOSE_ALL_BUT_ACTIVE_Click);
            UI_WINDOW_MENU_CLOSE_ALL_BUT_ACTIVE.Tag = new ToolStripItemTag() { IncludeSeparator = true };

            // UI_HELP_MENU
            UI_HELP_MENU.DropDownItems.AddRange(new ToolStripItem[] {
            UI_HELP_MENU_ABOUT});
            UI_HELP_MENU.Name = Constants.UI_HELP_MENU;
            UI_HELP_MENU.Text = Resources.MainHelpMenu;

            // UI_HELP_MENU_ABOUT
            UI_HELP_MENU_ABOUT.Name = Constants.UI_HELP_MENU_ABOUT;
            UI_HELP_MENU_ABOUT.Text = Resources.MainHelpMenuAbout;
            UI_HELP_MENU_ABOUT.Click += new System.EventHandler(UI_HELP_MENU_ABOUT_Click);
            UI_HELP_MENU_ABOUT.Tag = new ToolStripItemTag() { IncludeSeparator = true };

            #endregion

            #region ToolStrip Panel
                    
            _mainToolStripPanel.Dock = DockStyle.Top;

            #endregion

            #region DockPanel

            DockPanelSkin dockPanelSkin1 = new DockPanelSkin();
            AutoHideStripSkin autoHideStripSkin1 = new AutoHideStripSkin();
            DockPanelGradient dockPanelGradient1 = new DockPanelGradient();
            TabGradient tabGradient1 = new TabGradient();
            DockPaneStripSkin dockPaneStripSkin1 = new DockPaneStripSkin();
            DockPaneStripGradient dockPaneStripGradient1 = new DockPaneStripGradient();
            TabGradient tabGradient2 = new TabGradient();
            DockPanelGradient dockPanelGradient2 = new DockPanelGradient();
            TabGradient tabGradient3 = new TabGradient();
            DockPaneStripToolWindowGradient dockPaneStripToolWindowGradient1 = new DockPaneStripToolWindowGradient();
            TabGradient tabGradient4 = new TabGradient();
            TabGradient tabGradient5 = new TabGradient();
            DockPanelGradient dockPanelGradient3 = new DockPanelGradient();
            TabGradient tabGradient6 = new TabGradient();
            TabGradient tabGradient7 = new TabGradient();

            _dockPanel.ActiveAutoHideContent = null;
            _dockPanel.AllowEndUserNestedDocking = true;
            _dockPanel.ActiveDocumentChanged += new System.EventHandler(MainForm_ActiveDocumentChanged);
            _dockPanel.Dock = DockStyle.Fill;
            _dockPanel.DockBackColor = SystemColors.AppWorkspace;
            _dockPanel.Location = new Point(0, 49);
            _dockPanel.Name = "dockPanel";
            _dockPanel.Size = new Size(692, 395);
            dockPanelGradient1.EndColor = SystemColors.ControlLight;
            dockPanelGradient1.StartColor = SystemColors.ControlLight;
            autoHideStripSkin1.DockStripGradient = dockPanelGradient1;
            tabGradient1.EndColor = SystemColors.Control;
            tabGradient1.StartColor = SystemColors.Control;
            tabGradient1.TextColor = SystemColors.ControlDarkDark;
            autoHideStripSkin1.TabGradient = tabGradient1;
            dockPanelSkin1.AutoHideStripSkin = autoHideStripSkin1;
            tabGradient2.EndColor = SystemColors.ControlLightLight;
            tabGradient2.StartColor = SystemColors.ControlLightLight;
            tabGradient2.TextColor = SystemColors.ControlText;
            dockPaneStripGradient1.ActiveTabGradient = tabGradient2;
            dockPanelGradient2.EndColor = SystemColors.Control;
            dockPanelGradient2.StartColor = SystemColors.Control;
            dockPaneStripGradient1.DockStripGradient = dockPanelGradient2;
            tabGradient3.EndColor = SystemColors.ControlLight;
            tabGradient3.StartColor = SystemColors.ControlLight;
            tabGradient3.TextColor = SystemColors.ControlText;
            dockPaneStripGradient1.InactiveTabGradient = tabGradient3;
            dockPaneStripSkin1.DocumentGradient = dockPaneStripGradient1;
            tabGradient4.EndColor = SystemColors.ActiveCaption;
            tabGradient4.LinearGradientMode = LinearGradientMode.Vertical;
            tabGradient4.StartColor = SystemColors.GradientActiveCaption;
            tabGradient4.TextColor = SystemColors.ActiveCaptionText;
            dockPaneStripToolWindowGradient1.ActiveCaptionGradient = tabGradient4;
            tabGradient5.EndColor = SystemColors.Control;
            tabGradient5.StartColor = SystemColors.Control;
            tabGradient5.TextColor = SystemColors.ControlText;
            dockPaneStripToolWindowGradient1.ActiveTabGradient = tabGradient5;
            dockPanelGradient3.EndColor = SystemColors.ControlLight;
            dockPanelGradient3.StartColor = SystemColors.ControlLight;
            dockPaneStripToolWindowGradient1.DockStripGradient = dockPanelGradient3;
            tabGradient6.EndColor = SystemColors.GradientInactiveCaption;
            tabGradient6.LinearGradientMode = LinearGradientMode.Vertical;
            tabGradient6.StartColor = SystemColors.GradientInactiveCaption;
            tabGradient6.TextColor = SystemColors.ControlText;
            dockPaneStripToolWindowGradient1.InactiveCaptionGradient = tabGradient6;
            tabGradient7.EndColor = Color.Transparent;
            tabGradient7.StartColor = Color.Transparent;
            tabGradient7.TextColor = SystemColors.ControlDarkDark;
            dockPaneStripToolWindowGradient1.InactiveTabGradient = tabGradient7;
            dockPaneStripSkin1.ToolWindowGradient = dockPaneStripToolWindowGradient1;
            dockPanelSkin1.DockPaneStripSkin = dockPaneStripSkin1;
            _dockPanel.Skin = dockPanelSkin1;
            _dockPanel.TabIndex = 3;

            #endregion

            #region Status Bar

            _mainStatusStrip.Location = new Point(0, 444);
            _mainStatusStrip.Name = "mainStatusStrip";
            _mainStatusStrip.Size = new Size(692, 22);
            _mainStatusStrip.TabIndex = 1;
            _mainStatusStrip.Text = "statusStrip1";
            _mainStatusStrip.Items.AddRange(new ToolStripItem[] {
                _mainStatusStripMessage});

            _mainStatusStripMessage.Name = "mainStatusBarMessage";
            _mainStatusStripMessage.Size = new Size(0, 17);

            #endregion

            /*
             * MainForm
             */

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(692, 466);
            IsMdiContainer = true;
            MainMenuStrip = _mainMenuStrip;
            Name = "MainForm";
            Text = "MainForm";
            FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
            AllowDrop = true;

            if (!_applicationManager.ClientProfile.HaveFlag(
                ClientFlags.CoreDisableDragAndDropFileOpen))
            {
                DragEnter += new DragEventHandler(MainForm_DragEnter);
                DragDrop += new DragEventHandler(MainForm_DragDrop);
            }

            // Order is important here
            Controls.Add(_dockPanel);
            Controls.Add(_mainStatusStrip);
            Controls.Add(_mainToolStripPanel);
            Controls.Add(_mainMenuStrip);

            _mainStatusStrip.ResumeLayout(false);
            _mainStatusStrip.PerformLayout();
            _dockPanel.ResumeLayout(false);
            _dockPanel.PerformLayout();
            _mainToolStripPanel.ResumeLayout(false);
            _mainToolStripPanel.PerformLayout();
            _mainMenuStrip.ResumeLayout(false);
            _mainMenuStrip.PerformLayout();

            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        #region Drag and Drop

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string path in files)
                    LoadDocumentIntoWindow(path, true);
            }
        }

        #endregion

        #region DockedToolStrip Management

        /// <summary>
        /// Add a toolbar to the toolbar panel. The initial location
        /// is determined by the row and column hints. Note the hints are
        /// just that: there is no guarantee that the specified row or column
        /// will be available. The actual row and column positions will be
        /// determined by the number of toolbars in the toolbar panel.
        /// </summary>
        /// <param name="toolStrip">The toolbar.</param>
        /// <param name="rowHint">The row the toolbar should initially appear on.</param>
        /// <param name="colHint">The 'column' the toolbar should intially appear on. The
        /// column is determined by the number of toolbars on a row; the higher the hint the
        /// more to the right the toolbar will appear.</param>
        public void AddDockedToolStrip(
            ToolStrip toolStrip, int rowHint, int colHint)
        {
            /*
             * Use Add so that exceptions is thrown when duplicates
             * exist. Up to developer to make sure name is unique.
             */

            _dockedToolStrips.Add(toolStrip.Name, 
                new DockedToolStrip(toolStrip, rowHint, colHint));
        }

        /// <summary>
        /// Get a reference to a toolbar in the toolbar panel.
        /// </summary>
        /// <param name="name">The name of the toolbar.</param>
        /// <returns>A reference to the toolbar or null if the
        /// name is not found.</returns>
        public DockedToolStrip GetDockedToolStrip(string name)
        {
            if (_dockedToolStrips.ContainsKey(name))
                return _dockedToolStrips[name];
            else
                return null;
        }

        private void LoadDockedToolStrips()
        {
            /*
             * Modify the default states with stored states from 
             * the previous session.
             */
            
            DecodeToolStripStatus(_persistenceManager.ReadStrings(
                Constants.KEY_TOOLBAR_STATES));

            /*
             * At this point we know how many toolstrips we have. If we 
             * just have one present the simplified toolbar menu. If not
             * use the multi-toolbar presentation. Count cannot be 0 as
             * we will always have the main toolbar.
             */

            if (_dockedToolStrips.Count == 1)
                LoadSingleToolStrip();
            else
                LoadMultipleToolStrips();

            /*
             * Hide the toolstrips marked HideOnRestore.
             */

            foreach (DockedToolStrip toolStrip in _dockedToolStrips.Values)
                if (toolStrip.HideOnRestore)
                    toolStrip.ToolStrip.Visible = false;
        }

        private void LoadSingleToolStrip()
        {
            /*
             * Update the toolbar menu for single-item operation.
             */

            UI_VIEW_MENU.DropDownOpening +=
                new EventHandler(UI_VIEW_MENU_DropDownOpeningSingle);

            UI_VIEW_MENU_TOOLBAR.Text = Resources.MainViewMenuToolbarSingle;

            UI_VIEW_MENU_TOOLBAR.Click +=
                new EventHandler(UI_VIEW_MENU_TOOLBAR_ITEM_Click);

            UI_VIEW_MENU_TOOLBAR.Tag = Constants.MAIN_TOOLBAR_NAME;

            /*
             * Add the main toolbar to the panel.
             */

            DockedToolStrip dockedToolStrip =
                _dockedToolStrips[Constants.MAIN_TOOLBAR_NAME];

            // Make it full width.
            dockedToolStrip.ToolStrip.Stretch = true;

            _mainToolStripPanel.Join(dockedToolStrip.ToolStrip);
        }

        private void LoadMultipleToolStrips()
        {
            /*
             * Update the toolbar menu for multi-item operation.
             */
            
            UI_VIEW_MENU.DropDownOpening += 
                new EventHandler(UI_VIEW_MENU_DropDownOpeningMulti);

            UI_VIEW_MENU_TOOLBAR.Text = Resources.MainViewMenuToolbarMulti;
            
            UI_VIEW_MENU_TOOLBAR.DropDownOpening +=
                new EventHandler(UI_VIEW_MENU_TOOLBAR_DropDownOpening);

            /*
             * Create a list of toolStrips and sort into column hint order.
             */

            List<DockedToolStrip> list =
                _dockedToolStrips.Values.ToList<DockedToolStrip>();

            int rowMax = list.Max(n => n.RowHint);

            list.Sort(CompareByColHint);

            /*
             * Add the toolStrips to the toolStripPanel and create the 
             * entries for the main toolbar menu. Build up the toolbars
             * a row at a time.
             */

            for (int r = 0; r <= rowMax; r++)
            {
                foreach (DockedToolStrip toolStrip in list)
                {
                    if (toolStrip.RowHint != r) continue;

                    _mainToolStripPanel.Join(
                        toolStrip.ToolStrip, toolStrip.RowHint);

                    ToolStripMenuItem toolStripMenu = MenuTools.CreateMenuItem(
                        Constants.UI_VIEW_MENU_TOOLBAR + toolStrip.Name,
                        toolStrip.Text, null, Keys.None, null,
                        UI_VIEW_MENU_TOOLBAR_ITEM_Click);

                    toolStripMenu.Tag = toolStrip.Name;

                    UI_VIEW_MENU_TOOLBAR.DropDownItems.Add(toolStripMenu);
                }
            }
        }

        /*
         * Join seems to add in reverse 'column' order so we sort
         * in reverse to compensate.
         */

        private int CompareByColHint(DockedToolStrip x, DockedToolStrip y)
        {
            if (x.ColHint == y.ColHint)
                return 0;
            else if (x.ColHint < y.ColHint)
                return 1;
            else
                return -1;
        }

        /*
         * Get a string list representation of the current toolstrip
         * layout and states.
         */

        private List<String> EncodeToolStripStatus()
        {
            List<String> list = new List<String>();

            for (int row = 0; row < _mainToolStripPanel.Rows.Length; row++)
            {
                ToolStripPanelRow panelRow = _mainToolStripPanel.Rows[row];

                for (int col = 0; col < panelRow.Controls.Length; col++)
                {
                    list.Add(String.Format("{0}:{1}:{2}:{3}",
                        panelRow.Controls[col].Name,
                        panelRow.Controls[col].Visible ? "1" : "0",
                        row, col));
                }
            }

            return list;
        }

        /*
         * Restore a string list representaion of the toolstrip
         * states to the current toolstrip collection.
         */

        private void DecodeToolStripStatus(List<String> list)
        {
            foreach (string item in list)
            {
                string[] split = item.Split(':');
                if (split.Length != 4) continue;
                if (split[0] == String.Empty) continue;

                int row = 0;
                int col = 100;

                try
                {
                    row = Int32.Parse(split[2]);
                    col = Int32.Parse(split[3]);
                }
                catch
                {
                    // Just accept the defaults.
                }

                if (_dockedToolStrips.ContainsKey(split[0]))
                {
                    /*
                     * We need to keep the toolstrips visible until
                     * all have been restored then set the visibility
                     * according to HideOnRestore.
                     * This is a workaround: toolstips don't restore in
                     * the correct positions if they're not visible.
                     */

                    DockedToolStrip toolStrip = _dockedToolStrips[split[0]];
                    toolStrip.Visible = true;
                    toolStrip.HideOnRestore = (split[1] == "0");
                    toolStrip.RowHint = row;
                    toolStrip.ColHint = col;
                }
            }
        }

        #endregion

        #region Status Bar Management

        private void MainStatusStripTimer_Tick(object sender, EventArgs e)
        {
            _mainStatusStripMessage.Text = String.Empty;
        }

        /// <summary>
        /// Display a text message in the status bar message area.
        /// </summary>
        /// <param name="text">The message.</param>
        public void SetStatusBarMessage(string text)
        {
            SetStatusBarMessage(text, 0);
        }

        /// <summary>
        /// Display a temporary text message in the status bar message area.
        /// The message is cleared after the specified time period.
        /// </summary>
        /// <param name="text">The message.</param>
        /// <param name="clearAfterSeconds">The time period to display the message
        /// in seconds. The message is cleared after the period has elapsed; set a
        /// period of 0 seconds to prevent the message from being cleared by
        /// the timeout.</param>
        public void SetStatusBarMessage(string text, int clearAfterSeconds)
        {
            /*
             * Turn off any pending timer and reset for new timeout.
             */

            if (_mainStatusStripTimer.Enabled)
                _mainStatusStripTimer.Enabled = false;

            if (clearAfterSeconds > 0)
            {
                _mainStatusStripTimer.Interval = clearAfterSeconds * 1000;
                _mainStatusStripTimer.Enabled = true;
            }

            /*
             * Set the text.
             */

            _mainStatusStripMessage.Text = text;
        }

        #endregion
    }
}