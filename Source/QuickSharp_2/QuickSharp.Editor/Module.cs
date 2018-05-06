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
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using QuickSharp.Core;
using ScintillaNet;

namespace QuickSharp.Editor
{
    /// <summary>
    /// The editor plugin module.
    /// </summary>
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        /// <summary>
        /// Get the ID of the plugin.
        /// </summary>
        /// <returns>The plugin ID.</returns>
        public string GetID()
        {
            return "72F1F89B-F7D7-425B-B617-9E0B70597475";
        }

        /// <summary>
        /// Get the name of the plugin.
        /// </summary>
        /// <returns>The plugin name.</returns>
        public string GetName()
        {
            return "QuickSharp Editor";
        }

        /// <summary>
        /// Get the version of the plugin.
        /// </summary>
        /// <returns>The plugin version.</returns>
        public int GetVersion()
        {
            return 1;
        }

        /// <summary>
        /// Get a description of the plugin.
        /// </summary>
        /// <returns>The plugin description.</returns>
        public string GetDescription()
        {
            string txt = 
                "Provides the basic user interface elements and services " +
                "required by Scintilla-based text editor plugins. This " +
                "plugin uses Scintilla by Neil Hodgson and ScintillaNET " +
                "by Garrett Serack and the ScintillaNET team. " +
                "For more information visit the project websites at " +
                "http://www.scintilla.org and http://www.codeplex.com/ScintillaNET.";

            return txt;
        }

        /// <summary>
        /// Get the plugin's dependencies. This provides a list of the
        /// plugins required by the current plugin.
        /// </summary>
        /// <returns>The plugin dependencies,</returns>
        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            return deps;
        }

        /// <summary>
        /// The plugin entry point. This is called by the PluginManager to
        /// activate the plugin.
        /// </summary>
        /// <param name="mainForm">The application main form.</param>
        public void Activate(MainForm mainForm)
        {
            _mainForm = mainForm;
            ActivatePlugin();
        }

        #endregion

        private ApplicationManager _applicationManager;
        private MacroManager _macroManager;
        private MainForm _mainForm;
        private string _snippetsFolderPath;
        private ToolStripStatusLabel _cursorPositionIndicator;
        private ToolStripMenuItem _fileMenu;
        private ToolStripMenuItem _fileMenuPageSetup;
        private ToolStripMenuItem _fileMenuPrintPreview;
        private ToolStripMenuItem _fileMenuPrint;
        private ToolStripMenuItem _editMenu;
        private ToolStripMenuItem _editMenuUndo;
        private ToolStripMenuItem _editMenuRedo;
        private ToolStripMenuItem _editMenuCut;
        private ToolStripMenuItem _editMenuCopy;
        private ToolStripMenuItem _editMenuPaste;
        private ToolStripMenuItem _editMenuSelectAll;
        private ToolStripMenuItem _editMenuFind;
        private ToolStripMenuItem _editMenuReplace;
        private ToolStripMenuItem _editMenuGoTo;
        private ToolStripMenuItem _editMenuMacros;
        private ToolStripMenuItem _editMenuMacroRecord;
        private ToolStripMenuItem _editMenuMacroPlay;
        private ToolStripMenuItem _editMenuMacroLoad;
        private ToolStripMenuItem _editMenuMacroSave;
        private ToolStripMenuItem _editMenuSnippets;
        private ToolStripMenuItem _editMenuSnippetsManage;
        private ToolStripMenuItem _editMenuBookmarks;
        private ToolStripMenuItem _editMenuBookmarkToggle;
        private ToolStripMenuItem _editMenuBookmarksClear;
        private ToolStripMenuItem _editMenuBookmarkNext;
        private ToolStripMenuItem _editMenuBookmarkPrevious;
        private ToolStripMenuItem _editMenuFolding;
        private ToolStripMenuItem _editMenuFoldToggle;
        private ToolStripMenuItem _editMenuFoldCollapseAll;
        private ToolStripMenuItem _editMenuFoldExpandAll;
        private ToolStripMenuItem _editMenuEncoding;
        private ToolStripMenuItem _editMenuEncodingANSI;
        private ToolStripMenuItem _editMenuEncodingUTF8;
        private ToolStripMenuItem _editMenuEncodingUTF16LE;
        private ToolStripMenuItem _editMenuEncodingUTF16BE;
        private ToolStripMenuItem _editMenuLineEnding;
        private ToolStripMenuItem _editMenuLineEndingCRLF;
        private ToolStripMenuItem _editMenuLineEndingLF;
        private ToolStripMenuItem _editMenuLineEndingCR;
        private ToolStripMenuItem _editMenuAdvanced;
        private ToolStripMenuItem _editMenuMakeUppercase;
        private ToolStripMenuItem _editMenuMakeLowercase;
        private ToolStripMenuItem _editMenuLineComment;
        private ToolStripMenuItem _editMenuBlockComment;
        private ToolStripMenuItem _editMenuViewWhitespace;
        private ToolStripMenuItem _editMenuTrimWhitespace;
        private ToolStripMenuItem _editMenuWordWrap;
        private ToolStripMenuItem _editMenuSetReadOnly;

        private void ActivatePlugin()
        {
            /*
             * As this plugin only provides support for a scintilla object, not
             * a form or any other UI it must not register any document types
             * or set any document handlers.
             * On its own this plugin will gracefully reject calls to open
             * documents via the 'no handler register' alert so it is safe to
             * include standalone but without a UI it won't be much use!
             */

            UserContent.DeployContent(Constants.PLUGIN_NAME);

            _applicationManager = ApplicationManager.GetInstance();

            _applicationManager.RegisterOptionsPageFactory(
                delegate { return new EditorOptionsPage(); });

            _applicationManager.RegisterOptionsPageFactory(
                delegate { return new GlobalOptionsPage(); });

            _macroManager = MacroManager.GetInstance();

            /*
             * Define the snippets folder.
             */

            _snippetsFolderPath = Path.Combine(
                ApplicationManager.GetInstance().QuickSharpUserHome,
                Constants.SNIPPETS_FOLDER);

            /*
             * Keyboard Shortcuts Note
             * 
             * Most menu commands are the sole access point for editor functions and are
             * enabled and disabled according to the state determined by their action state
             * handlers. In most cases the state of these commands changes only when the documents
             * are opened, closed or activated. This is important because the shortcut keys
             * only work when the corresponding menu is enabled; often a shortcut key is accessed
             * before the menu is opened (at which point the state is updated) and must be in the
             * correct state when the doucment is opened or activated. However, the status of
             * some commands (clipboard or selection dependent) changes continuously during
             * editing and it is not feasible to have these updated only when the menus are opened.
             * These commands are represented in the menus using text-only shortcut key assignments
             * and are activated using the key mappings provided by the Scintilla editor,
             * not via the menus. The action state handlers for these actions are provided only
             * to maintain the correct visual state of the corresponding menu commands.
             * 
             * This means that, whereas other menu-based commands can have their shortcut
             * keys re-mapped simply by changing the assignment on the menu item, these cannot:
             * the shortcut keys presented on the menus must match the Scintilla mappings. (See
             * 'commands.cs' in the ScintillaNet project to see the relevant mappings.)
             */

            #region Menu Creation

            bool enableCodeFeatures = !_applicationManager.ClientProfile.
                HaveFlag(ClientFlags.EditorDisableCodeFeatures);

            /*
             * File Menu
             */

            #region Create Items

            _fileMenuPageSetup = MenuTools.CreateMenuItem(
                Constants.UI_FILE_MENU_PAGE_SETUP,
                Resources.MainFileMenuPageSetup,
                Resources.PageSetup,
                Keys.None, null, UI_EDIT_MENU_ITEM_Click);

            _fileMenuPrintPreview = MenuTools.CreateMenuItem(
                Constants.UI_FILE_MENU_PRINT_PREVIEW,
                Resources.MainFileMenuPrintPreview,
                null,
                Keys.None, null,
                UI_EDIT_MENU_ITEM_Click);

            _fileMenuPrint = MenuTools.CreateMenuItem(
                Constants.UI_FILE_MENU_PRINT,
                Resources.MainFileMenuPrint,
                Resources.Print,
                Keys.Control | Keys.P, null, UI_EDIT_MENU_ITEM_Click,
                true);

            #endregion

            _fileMenu = _mainForm.GetMenuItemByName(
                QuickSharp.Core.Constants.UI_FILE_MENU);

            int i = _fileMenu.DropDownItems.IndexOfKey(
                QuickSharp.Core.Constants.UI_FILE_MENU_SAVE_ALL);

            if (i != -1)
            {
                i++; // Insert after
                _fileMenu.DropDownItems.Insert(i, _fileMenuPrint);
                _fileMenu.DropDownItems.Insert(i, _fileMenuPrintPreview);
                _fileMenu.DropDownItems.Insert(i, _fileMenuPageSetup);
            }

            /*
             * Edit Menu
             */

            #region Create Items

            _editMenu = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU,
                Resources.MainEditMenu,
                null,
                Keys.None, null, null);

            _editMenuUndo = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_UNDO,
                Resources.MainEditMenuUndo,
                Resources.Undo,
                Keys.None, "Ctrl+Z", UI_EDIT_MENU_ITEM_Click);

            _editMenuRedo = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_REDO,
                Resources.MainEditMenuRedo,
                Resources.Redo,
                Keys.None, "Ctrl+Y", UI_EDIT_MENU_ITEM_Click,
                true);

            _editMenuCut = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_CUT,
                Resources.MainEditMenuCut,
                Resources.Cut,
                Keys.None, "Ctrl+X", UI_EDIT_MENU_ITEM_Click);

            _editMenuCopy = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_COPY,
                Resources.MainEditMenuCopy,
                Resources.Copy,
                Keys.None, "Ctrl+C", UI_EDIT_MENU_ITEM_Click);

            _editMenuPaste = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_PASTE,
                Resources.MainEditMenuPaste,
                Resources.Paste,
                Keys.None, "Ctrl+V", UI_EDIT_MENU_ITEM_Click);

            _editMenuSelectAll = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_SELECT_ALL,
                Resources.MainEditMenuSelectAll,
                null,
                Keys.None, "Ctrl+A", UI_EDIT_MENU_ITEM_Click,
                true);

            _editMenuFind = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_FIND,
                Resources.MainEditMenuFind,
                Resources.Find,
                Keys.Control | Keys.F, null, UI_EDIT_MENU_ITEM_Click);

            _editMenuReplace = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_REPLACE,
                Resources.MainEditMenuReplace,
                null,
                Keys.Control | Keys.H, null, UI_EDIT_MENU_ITEM_Click,
                true);

            _editMenuGoTo = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_GOTO,
                Resources.MainEditMenuGoTo,
                null,
                Keys.Control | Keys.G, null, UI_EDIT_MENU_ITEM_Click,
                true);

            _editMenuMacros = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_MACROS,
                Resources.MainEditMenuMacros,
                null,
                Keys.None, null, null);

            _editMenuMacroRecord = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_MACRO_RECORD,
                Resources.MainEditMenuMacroStartRecord,
                null,
                Keys.Control | Keys.Shift | Keys.R, null,
                UI_EDIT_MENU_ITEM_Click);

            _editMenuMacroPlay = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_MACRO_PLAY,
                Resources.MainEditMenuMacroPlay,
                null,
                Keys.Control | Keys.Shift | Keys.P, null,
                UI_EDIT_MENU_ITEM_Click);

            _editMenuMacroLoad = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_MACRO_LOAD,
                Resources.MainEditMenuMacroLoad,
                null,
                Keys.None, null, UI_EDIT_MENU_ITEM_Click);

            _editMenuMacroSave = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_MACRO_SAVE,
                Resources.MainEditMenuMacroSave,
                null,
                Keys.None, null, UI_EDIT_MENU_ITEM_Click);

            _editMenuSnippets = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_SNIPPETS,
                Resources.MainEditMenuSnippets,
                null,
                Keys.None, null, null);

            _editMenuSnippetsManage = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_MANAGE_SNIPPETS,
                Resources.MainEditMenuManageSnippets,
                null,
                Keys.None, null, UI_EDIT_MENU_MANAGE_SNIPPETS_Click);

            _editMenuBookmarks = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_BOOKMARKS,
                Resources.MainEditMenuBookmarks,
                null,
                Keys.None, null, null);

            _editMenuBookmarkToggle = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_TOGGLE_BOOKMARK,
                Resources.MainEditMenuToggleBookmark,
                null,
                Keys.F3,
                null, UI_EDIT_MENU_ITEM_Click);

            _editMenuBookmarksClear = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_CLEAR_BOOKMARKS,
                Resources.MainEditMenuClearBookmarks,
                null,
                Keys.Shift | Keys.F3,
                null, UI_EDIT_MENU_ITEM_Click);

            _editMenuBookmarkNext = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_NEXT_BOOKMARK,
                Resources.MainEditMenuNextBookmark,
                null,
                Keys.F4,
                null, UI_EDIT_MENU_ITEM_Click);

            _editMenuBookmarkPrevious = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_PREV_BOOKMARK,
                Resources.MainEditMenuPreviousBookmark,
                null,
                Keys.Shift | Keys.F4,
                null, UI_EDIT_MENU_ITEM_Click);

            _editMenuFolding = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_FOLDING,
                Resources.MainEditMenuFolding,
                null,
                Keys.None, null, null);

            _editMenuFoldToggle = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_TOGGLE_FOLD,
                Resources.MainEditMenuToggleFold,
                null,
                Keys.F8,
                null, UI_EDIT_MENU_ITEM_Click);

            _editMenuFoldCollapseAll = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_COLLAPSE_ALL,
                Resources.MainEditMenuCollapseAll,
                null,
                Keys.Control | Keys.F8,
                null, UI_EDIT_MENU_ITEM_Click);

            _editMenuFoldExpandAll = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_EXPAND_ALL,
                Resources.MainEditMenuExpandAll,
                null,
                Keys.Shift | Keys.F8,
                null, UI_EDIT_MENU_ITEM_Click);

            _editMenuEncoding = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_ENCODING,
                Resources.MainEditMenuEncoding,
                null,
                Keys.None, null, null);

            _editMenuEncodingANSI = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_ENCODING_ANSI,
                Resources.MainEditMenuEncodingANSI,
                null,
                Keys.None, null, UI_EDIT_MENU_ITEM_Click);

            _editMenuEncodingUTF8 = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_ENCODING_UTF8,
                Resources.MainEditMenuEncodingUTF8,
                null,
                Keys.None, null, UI_EDIT_MENU_ITEM_Click);

            _editMenuEncodingUTF16BE = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_ENCODING_UTF16BE,
                Resources.MainEditMenuEncodingUTF16BE,
                null,
                Keys.None, null, UI_EDIT_MENU_ITEM_Click);

            _editMenuEncodingUTF16LE = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_ENCODING_UTF16LE,
                Resources.MainEditMenuEncodingUTF16LE,
                null,
                Keys.None, null, UI_EDIT_MENU_ITEM_Click);

            _editMenuLineEnding = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_LINE_ENDING,
                Resources.MainEditMenuLineEnding,
                null,
                Keys.None, null, UI_EDIT_MENU_ITEM_Click);

            _editMenuLineEndingCRLF = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_LINE_ENDING_CRLF,
                Resources.MainEditMenuLineEndingCRLF,
                null,
                Keys.None, null, UI_EDIT_MENU_ITEM_Click);

            _editMenuLineEndingLF = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_LINE_ENDING_LF,
                Resources.MainEditMenuLineEndingLF,
                null,
                Keys.None, null, UI_EDIT_MENU_ITEM_Click);

            _editMenuLineEndingCR = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_LINE_ENDING_CR,
                Resources.MainEditMenuLineEndingCR,
                null,
                Keys.None, null, UI_EDIT_MENU_ITEM_Click);

            _editMenuAdvanced = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_ADVANCED,
                Resources.MainEditMenuAdvanced,
                null,
                Keys.None, null, null,
                true);

            _editMenuMakeUppercase = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_MAKE_UPPERCASE,
                Resources.MainEditMenuMakeUppercase,
                null,
                Keys.None, "Ctrl+Shift+U", UI_EDIT_MENU_ITEM_Click);

            _editMenuMakeLowercase = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_MAKE_LOWERCASE,
                Resources.MainEditMenuMakeLowercase,
                null,
                Keys.None, "Ctrl+U", UI_EDIT_MENU_ITEM_Click,
                true);

            _editMenuLineComment = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_LINE_COMMENT,
                Resources.MainEditMenuLineComment,
                null,
                Keys.Control | Keys.Q, null, UI_EDIT_MENU_ITEM_Click);

            _editMenuBlockComment = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_BLOCK_COMMENT,
                Resources.MainEditMenuBlockComment,
                null,
                Keys.Control | Keys.Shift | Keys.Q, null,
                UI_EDIT_MENU_ITEM_Click,
                true);

            _editMenuViewWhitespace = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_VIEW_WHITESPACE,
                Resources.MainEditMenuViewWhitespace,
                null,
                Keys.Control | Keys.W, null, UI_EDIT_MENU_ITEM_Click);

            _editMenuTrimWhitespace = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_TRIM_WHITESPACE,
                Resources.MainEditMenuTrimWhitespace,
                null,
                Keys.None, null, UI_EDIT_MENU_ITEM_Click,
                true);

            _editMenuWordWrap = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_WORD_WRAP,
                Resources.MainEditMenuWordWrap,
                null,
                Keys.None, null, UI_EDIT_MENU_ITEM_Click);

            _editMenuSetReadOnly = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_SET_READ_ONLY,
                Resources.MainEditMenuSetReadOnly,
                null,
                Keys.None, null, UI_EDIT_MENU_ITEM_Click,
                true);

            #endregion

            int fileMenuIndex =
                _mainForm.MainMenu.Items.IndexOf(_fileMenu);

            if (fileMenuIndex == -1)
                _mainForm.MainMenu.Items.Add(_editMenu);
            else
                _mainForm.MainMenu.Items.Insert(
                    fileMenuIndex + 1, _editMenu);

            _editMenu.DropDownItems.Add(_editMenuUndo);
            _editMenu.DropDownItems.Add(_editMenuRedo);
            _editMenu.DropDownItems.Add(_editMenuCut);
            _editMenu.DropDownItems.Add(_editMenuCopy);
            _editMenu.DropDownItems.Add(_editMenuPaste);
            _editMenu.DropDownItems.Add(_editMenuSelectAll);
            _editMenu.DropDownItems.Add(_editMenuFind);
            _editMenu.DropDownItems.Add(_editMenuReplace);
            _editMenu.DropDownItems.Add(_editMenuGoTo);
            _editMenu.DropDownItems.Add(_editMenuMacros);
            _editMenuMacros.DropDownItems.Add(_editMenuMacroRecord);
            _editMenuMacros.DropDownItems.Add(_editMenuMacroPlay);
            _editMenuMacros.DropDownItems.Add(_editMenuMacroLoad);
            _editMenuMacros.DropDownItems.Add(_editMenuMacroSave);

            if (Directory.Exists(_snippetsFolderPath))
            {
                _editMenu.DropDownItems.Add(_editMenuSnippets);
                _editMenuSnippets.DropDownItems.Add(_editMenuSnippetsManage);
            }

            _editMenu.DropDownItems.Add(_editMenuBookmarks);
            _editMenuBookmarks.DropDownItems.Add(_editMenuBookmarkToggle);
            _editMenuBookmarks.DropDownItems.Add(_editMenuBookmarksClear);
            _editMenuBookmarks.DropDownItems.Add(_editMenuBookmarkNext);
            _editMenuBookmarks.DropDownItems.Add(_editMenuBookmarkPrevious);
            _editMenu.DropDownItems.Add(_editMenuFolding);
            _editMenuFolding.DropDownItems.Add(_editMenuFoldToggle);
            _editMenuFolding.DropDownItems.Add(_editMenuFoldCollapseAll);
            _editMenuFolding.DropDownItems.Add(_editMenuFoldExpandAll);
            _editMenu.DropDownItems.Add(_editMenuEncoding);
            _editMenuEncoding.DropDownItems.Add(_editMenuEncodingANSI);
            _editMenuEncoding.DropDownItems.Add(_editMenuEncodingUTF8);
            _editMenuEncoding.DropDownItems.Add(_editMenuEncodingUTF16BE);
            _editMenuEncoding.DropDownItems.Add(_editMenuEncodingUTF16LE);
            _editMenu.DropDownItems.Add(_editMenuLineEnding);
            _editMenuLineEnding.DropDownItems.Add(_editMenuLineEndingCRLF);
            _editMenuLineEnding.DropDownItems.Add(_editMenuLineEndingCR);
            _editMenuLineEnding.DropDownItems.Add(_editMenuLineEndingLF);
            _editMenu.DropDownItems.Add(_editMenuAdvanced);
            _editMenuAdvanced.DropDownItems.Add(_editMenuMakeUppercase);
            _editMenuAdvanced.DropDownItems.Add(_editMenuMakeLowercase);

            if (enableCodeFeatures)
            {
                _editMenuAdvanced.DropDownItems.Add(_editMenuLineComment);
                _editMenuAdvanced.DropDownItems.Add(_editMenuBlockComment);
            }

            _editMenuAdvanced.DropDownItems.Add(_editMenuViewWhitespace);
            _editMenuAdvanced.DropDownItems.Add(_editMenuTrimWhitespace);
            _editMenuAdvanced.DropDownItems.Add(_editMenuWordWrap);
            _editMenuAdvanced.DropDownItems.Add(_editMenuSetReadOnly);

            #endregion

            _fileMenu.DropDownOpening +=
                new EventHandler(UI_FILE_MENU_DropDownOpening);
            _editMenu.DropDownOpening +=
                new EventHandler(UI_EDIT_MENU_DropDownOpening);
            _editMenuSnippets.DropDownOpening +=
                new EventHandler(UI_EDIT_MENU_SNIPPETS_DropDownOpening);
            _editMenuSnippets.Enabled = false;
            _editMenuEncoding.DropDownOpening +=
                new EventHandler(UI_EDIT_MENU_ENCODING_DropDownOpening);
            _editMenuLineEnding.DropDownOpening +=
                new EventHandler(UI_EDIT_MENU_LINE_ENDING_DropDownOpening);

            _cursorPositionIndicator = new ToolStripStatusLabel();
            _cursorPositionIndicator.Name =
                Constants.UI_STATUSBAR_CURSOR_POSITION_INDICATOR;
            _cursorPositionIndicator.Spring = true;
            _cursorPositionIndicator.TextAlign = ContentAlignment.MiddleRight;

            _mainForm.StatusBar.Items.Add(_cursorPositionIndicator);

            _mainForm.ClientWindow.ActiveDocumentChanged += 
                new EventHandler(ClientWindow_ActiveDocumentChanged);

            /*
             * Create initial version of the snippets menu to
             * activate the shortcuts keys.
             */

            CreateSnippetsMenu();
        }

        #region Event Handlers

        private bool IsMenuEnabled(ToolStripMenuItem menuItem)
        {
            if (_mainForm.ActiveDocument == null)
                return false;
            else
                return _mainForm.ActiveDocument.IsMenuItemEnabled(menuItem);
        }

        private void ClientWindow_ActiveDocumentChanged(object sender, EventArgs e)
        {
            CancelMacroRecording();
            UpdateFileMenuState();
            UpdateEditMenuState();
        }

        private void UI_FILE_MENU_DropDownOpening(object sender, EventArgs e)
        {
            UpdateFileMenuState();
        }

        private void UI_EDIT_MENU_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEditMenuState();
        }

        private void CancelMacroRecording()
        {
            if (_macroManager.IsRecording)
            {
                _macroManager.StopRecording();
                _macroManager.ClearMacro();
                _mainForm.SetStatusBarMessage(Resources.MacroRecordCancelled, 5);
            }
        }

        private void UpdateFileMenuState()
        {
            _fileMenuPageSetup.Enabled = IsMenuEnabled(_fileMenuPageSetup);
            _fileMenuPrintPreview.Enabled = IsMenuEnabled(_fileMenuPrintPreview);
            _fileMenuPrint.Enabled = IsMenuEnabled(_fileMenuPrint);
        }

        private void UpdateEditMenuState()
        {
            _editMenuUndo.Enabled = IsMenuEnabled(_editMenuUndo);
            _editMenuRedo.Enabled = IsMenuEnabled(_editMenuRedo);
            _editMenuCut.Enabled = IsMenuEnabled(_editMenuCut);
            _editMenuCopy.Enabled = IsMenuEnabled(_editMenuCopy);
            _editMenuPaste.Enabled = IsMenuEnabled(_editMenuPaste);
            _editMenuSelectAll.Enabled = IsMenuEnabled(_editMenuSelectAll);
            _editMenuFind.Enabled = IsMenuEnabled(_editMenuFind);
            _editMenuReplace.Enabled = IsMenuEnabled(_editMenuReplace);
            _editMenuGoTo.Enabled = IsMenuEnabled(_editMenuGoTo);
            _editMenuMacros.Enabled = IsMenuEnabled(_editMenuMacros);
            _editMenuSnippets.Enabled = IsMenuEnabled(_editMenuSnippets);
            _editMenuBookmarks.Enabled = IsMenuEnabled(_editMenuBookmarks);
            _editMenuBookmarkToggle.Enabled = IsMenuEnabled(_editMenuBookmarkToggle);
            _editMenuBookmarksClear.Enabled = IsMenuEnabled(_editMenuBookmarksClear);
            _editMenuBookmarkNext.Enabled = IsMenuEnabled(_editMenuBookmarkNext);
            _editMenuBookmarkPrevious.Enabled = IsMenuEnabled(_editMenuBookmarkPrevious);
            _editMenuFolding.Enabled = IsMenuEnabled(_editMenuFolding);
            _editMenuFoldToggle.Enabled = IsMenuEnabled(_editMenuFoldToggle);
            _editMenuFoldCollapseAll.Enabled = IsMenuEnabled(_editMenuFoldCollapseAll);
            _editMenuFoldExpandAll.Enabled = IsMenuEnabled(_editMenuFoldExpandAll);
            _editMenuEncoding.Enabled = IsMenuEnabled(_editMenuEncoding);
            _editMenuEncodingANSI.Enabled = IsMenuEnabled(_editMenuEncodingANSI);
            _editMenuEncodingUTF8.Enabled = IsMenuEnabled(_editMenuEncodingUTF8);
            _editMenuEncodingUTF16BE.Enabled = IsMenuEnabled(_editMenuEncodingUTF16BE);
            _editMenuEncodingUTF16LE.Enabled = IsMenuEnabled(_editMenuEncodingUTF16LE);
            _editMenuLineEnding.Enabled = IsMenuEnabled(_editMenuLineEnding);
            _editMenuLineEndingCRLF.Enabled = IsMenuEnabled(_editMenuLineEndingCRLF);
            _editMenuLineEndingLF.Enabled = IsMenuEnabled(_editMenuLineEndingLF);
            _editMenuLineEndingCR.Enabled = IsMenuEnabled(_editMenuLineEndingCR);
            _editMenuAdvanced.Enabled = IsMenuEnabled(_editMenuAdvanced);
            _editMenuMakeUppercase.Enabled = IsMenuEnabled(_editMenuMakeUppercase);
            _editMenuMakeLowercase.Enabled = IsMenuEnabled(_editMenuMakeLowercase);
            _editMenuLineComment.Enabled = IsMenuEnabled(_editMenuLineComment);
            _editMenuBlockComment.Enabled = IsMenuEnabled(_editMenuBlockComment);
            _editMenuViewWhitespace.Enabled = IsMenuEnabled(_editMenuViewWhitespace);
            _editMenuTrimWhitespace.Enabled = IsMenuEnabled(_editMenuTrimWhitespace);
            _editMenuWordWrap.Enabled = IsMenuEnabled(_editMenuWordWrap);
            _editMenuSetReadOnly.Enabled = IsMenuEnabled(_editMenuSetReadOnly);

            if (_macroManager.IsRecording)
            {
                _editMenuMacroRecord.Text = Resources.MainEditMenuMacroStopRecord;
                _editMenuMacroLoad.Enabled = false;
                _editMenuMacroSave.Enabled = false;
            }
            else
            {
                _editMenuMacroRecord.Text = Resources.MainEditMenuMacroStartRecord;
                _editMenuMacroLoad.Enabled = true;
                _editMenuMacroSave.Enabled = (_macroManager.HaveMacro);
            }
        }

        private void UI_EDIT_MENU_ITEM_Click(object sender, EventArgs e)
        {
            if (_mainForm.ActiveDocument == null) return;
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem == null) return;

            _mainForm.ActiveDocument.PerformMenuAction(menuItem);
        }

        private void UI_EDIT_MENU_SNIPPETS_DropDownOpening(
            object sender, EventArgs e)
        {
            CreateSnippetsMenu();
        }

        private void UI_EDIT_MENU_MANAGE_SNIPPETS_Click(
            object sender, EventArgs e)
        {
            /*
             * Remember: any shortcut keys applied to the snippet filenames
             * via windows explorer won't become available until the
             * snippets menu is reopened.
             */

            try
            {
                Process p = new Process();
                p.StartInfo.FileName = _snippetsFolderPath;
                p.StartInfo.Verb = "Open";
                p.StartInfo.CreateNoWindow = true;
                p.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}\r\n{1}",
                        Resources.ManageSnippetsErrorDialogTitle,
                        ex.Message),
                    Resources.ManageSnippetsErrorDialogText,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void InsertSnippet_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null) return;

            ScintillaEditForm document =
                _mainForm.ActiveDocument as ScintillaEditForm;

            if (document == null) return;

            string snippet = FileTools.ReadFile(item.Name);
            if (String.IsNullOrEmpty(snippet)) return;

            document.InsertSnippet(snippet);
        }

        private void UI_EDIT_MENU_ENCODING_DropDownOpening(
            object sender, EventArgs e)
        {
            ScintillaEditForm document =
                _mainForm.ActiveDocument as ScintillaEditForm;

            if (document == null) return;

            foreach (ToolStripMenuItem item in 
                _editMenuEncoding.DropDownItems)
                item.Checked = false;

            if (document.FileEncoding == Encoding.ASCII)
                _editMenuEncodingANSI.Checked = true;
            if (document.FileEncoding == Encoding.UTF8)
                _editMenuEncodingUTF8.Checked = true;
            if (document.FileEncoding == Encoding.BigEndianUnicode)
                _editMenuEncodingUTF16BE.Checked = true;
            if (document.FileEncoding == Encoding.Unicode)
                _editMenuEncodingUTF16LE.Checked = true;
        }

        private void UI_EDIT_MENU_LINE_ENDING_DropDownOpening(
            object sender, EventArgs e)
        {
            ScintillaEditForm document =
                _mainForm.ActiveDocument as ScintillaEditForm;

            if (document == null) return;

            foreach (ToolStripMenuItem item in
                _editMenuLineEnding.DropDownItems)
                item.Checked = false;

            if (document.LineEnding == EndOfLineMode.Crlf)
                _editMenuLineEndingCRLF.Checked = true;
            if (document.LineEnding == EndOfLineMode.LF)
                _editMenuLineEndingLF.Checked = true;
            if (document.LineEnding == EndOfLineMode.CR)
                _editMenuLineEndingCR.Checked = true;
        }

        #endregion

        #region Snippets Menu

        private void CreateSnippetsMenu()
        {
            _editMenuSnippets.DropDownItems.Clear();
            _editMenuSnippets.DropDownItems.Add(
                _editMenuSnippetsManage);

            PopulateSnippetsMenu(
                _snippetsFolderPath, _editMenuSnippets);

            if (_editMenuSnippets.DropDownItems.Count > 1)
                _editMenuSnippets.DropDownItems.Insert(1,
                    MenuTools.CreateSeparator(
                        Constants.UI_EDIT_MENU_MANAGE_SNIPPETS_SEP));
        }

        private void PopulateSnippetsMenu(
            string rootFolder, ToolStripMenuItem rootMenuItem)
        {
            if (!Directory.Exists(rootFolder)) return;

            rootMenuItem.ShowShortcutKeys = true;

            ThemeFlags themeFlags = _applicationManager.
                ClientProfile.ThemeFlags;

            bool applyTheme = themeFlags != null &&
                themeFlags.MenuForeColor != Color.Empty;

            foreach (string folder in Directory.GetDirectories(rootFolder))
            {
                try
                {
                    ToolStripMenuItem tmi = new ToolStripMenuItem();
                    tmi.Text = tmi.Name = Path.GetFileName(folder);
                    tmi.Image = Resources.Folder;
                    tmi.ImageTransparentColor = Color.Fuchsia;

                    if (applyTheme)
                    {
                        tmi.ForeColor = themeFlags.MenuForeColor;
                        if (themeFlags.MenuHideImages)
                            tmi.Image = null;
                    }
                    
                    rootMenuItem.DropDownItems.Add(tmi);

                    PopulateSnippetsMenu(folder, tmi);
                }
                catch
                {
                    // Ignore menu entry on error
                }
            }

            string[] files = Directory.GetFiles(
                rootFolder, Constants.SNIPPET_FILE_PATTERN);

            Array.Sort(files, new NoShortcutComparer());

            foreach (string file in files)
            {
                try
                {
                    ToolStripMenuItem tmi = new ToolStripMenuItem();

                    string name = Path.GetFileNameWithoutExtension(file);
                    
                    // Decode any shorcut keys in the name
                    tmi.ShortcutKeys = GetShortcutKeys(ref name);

                    // Must have a name for the snippet
                    if (name == String.Empty) continue;

                    tmi.Text = name;
                    tmi.Name = file;
                    tmi.Click += new EventHandler(InsertSnippet_Click);

                    if (applyTheme)
                        tmi.ForeColor = themeFlags.MenuForeColor;

                    rootMenuItem.DropDownItems.Add(tmi);
                }
                catch
                {
                    // Ignore menu entry on error
                }
            }

            rootMenuItem.Enabled = (rootMenuItem.DropDownItems.Count > 0);
        }

        private class NoShortcutComparer : IComparer<String>
        {
            public int Compare(String x, String y)
            {
                string s1 = Path.GetFileName(x);
                string s2 = Path.GetFileName(y);

                string[] split1 = s1.Split('#');
                if (split1.Length == 2) s1 = split1[1];

                string[] split2 = s2.Split('#');
                if (split2.Length == 2) s2 = split2[1];

                return (s1.CompareTo(s2));
            }
        }

        private Keys GetShortcutKeys(ref string name)
        {
            /*
             * Shortcut keys can be encoded into the name of the snippet
             * file using a simple prefix code. This is a bit hackish but
             * prevents having to create a whole configuration management
             * system. Hopefully the try block will prevent any explosions.
             * If the key combination works it will show up in the menu; if
             * not the menu text will have the attempted code prefix still
             * intact.
             * 
             * File fFormat = "CSAx#File Name.txt"
             * C/S/A are Ctrl, Shift, and Alt, final letter is key.
             * Rule is the shortcut must have at least 2 modifiers.
             */

            try
            {
                if (String.IsNullOrEmpty(name)) return Keys.None;

                int index = name.IndexOf("#");

                if (index == -1) return Keys.None;

                string code = name.Substring(0, index);

                // Must have at least 3 chars and no more than 4
                if (code.Length < 3 || code.Length > 4) return Keys.None;

                code = code.PadLeft(4).ToUpper();

                string key = code.Substring(3);

                code = code.Remove(3);

                bool ctrl = (code.IndexOf('C') != -1);
                bool shift = (code.IndexOf('S') != -1);
                bool alt = (code.IndexOf('A') != -1);

                // Must have at least two
                if (!(ctrl & shift) & !(ctrl & alt) & !(shift & alt))
                    return Keys.None;

                KeysConverter kc = new KeysConverter();
                Keys shortcutKeys = (Keys)kc.ConvertFromString(key);

                if (ctrl) shortcutKeys |= Keys.Control;
                if (shift) shortcutKeys |= Keys.Shift;
                if (alt) shortcutKeys |= Keys.Alt;

                // Remove the code from the menu name on success
                name = name.Substring(index + 1);

                return shortcutKeys;
            }
            catch
            {
                // Just return no key
                return Keys.None;
            }
        }

        #endregion
    }
}
