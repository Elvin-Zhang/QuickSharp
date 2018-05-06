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
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using QuickSharp.Core;
using QuickSharp.Editor;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.TextEditor
{
    public partial class EditForm : QuickSharp.Editor.ScintillaEditForm
    {
        public EditForm()
        {
            InitializeComponent();

            #region Action Handler Registration

            // Register an interest in the Editor UI actions
            RegisterActionStateHandler(
                QuickSharp.Core.Constants.UI_FILE_MENU_SAVE, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Core.Constants.UI_FILE_MENU_SAVE_AS, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_FILE_MENU_PAGE_SETUP, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_FILE_MENU_PRINT_PREVIEW, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_FILE_MENU_PRINT, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_UNDO, CanUndo);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_REDO, CanRedo);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_CUT, HaveSelection);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_COPY, HaveSelection);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_PASTE, CanPaste);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_SELECT_ALL, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_FIND, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_REPLACE, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_GOTO, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_MACROS, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_SNIPPETS, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_ADVANCED, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_MAKE_UPPERCASE, HaveSelection);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_MAKE_LOWERCASE, HaveSelection);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_LINE_COMMENT, CanLineComment);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_BLOCK_COMMENT, CanBlockComment);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_VIEW_WHITESPACE, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_TRIM_WHITESPACE, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_WORD_WRAP, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_SET_READ_ONLY, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_BOOKMARKS, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_TOGGLE_BOOKMARK, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_CLEAR_BOOKMARKS, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_NEXT_BOOKMARK, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_PREV_BOOKMARK, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_FOLDING, HaveFolding);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_TOGGLE_FOLD, HaveFolding);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_COLLAPSE_ALL, HaveFolding);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_EXPAND_ALL, HaveFolding);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_ENCODING, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_ENCODING_ANSI, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_ENCODING_UTF8, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_ENCODING_UTF16BE, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_ENCODING_UTF16LE, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_LINE_ENDING, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_LINE_ENDING_CRLF, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_LINE_ENDING_LF, ActionEnabled);
            RegisterActionStateHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_LINE_ENDING_CR, ActionEnabled);

            RegisterActionHandler(
                QuickSharp.Core.Constants.UI_FILE_MENU_SAVE, Save);
            RegisterActionHandler(
                QuickSharp.Core.Constants.UI_FILE_MENU_SAVE_AS, SaveAs);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_FILE_MENU_PAGE_SETUP, PageSetup);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_FILE_MENU_PRINT_PREVIEW, PrintPreview);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_FILE_MENU_PRINT, Print);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_UNDO, Undo);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_REDO, Redo);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_CUT, Cut);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_COPY, Copy);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_PASTE, Paste);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_SELECT_ALL, SelectAll);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_FIND, Find);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_REPLACE, Replace);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_GOTO, GoTo);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_MACRO_RECORD, MacroRecord);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_MACRO_PLAY, MacroPlay);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_MACRO_LOAD, MacroLoad);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_MACRO_SAVE, MacroSave);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_MAKE_UPPERCASE, MakeUppercase);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_MAKE_LOWERCASE, MakeLowercase);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_LINE_COMMENT, LineComment);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_BLOCK_COMMENT, BlockComment);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_VIEW_WHITESPACE, ViewWhitespace);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_TRIM_WHITESPACE, TrimWhitespace);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_WORD_WRAP, WordWrap);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_SET_READ_ONLY, SetReadOnly);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_TOGGLE_BOOKMARK, BookMarkToggle);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_CLEAR_BOOKMARKS, BookMarkClear);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_NEXT_BOOKMARK, BookmarkNext);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_PREV_BOOKMARK, BookmarkPrevious);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_TOGGLE_FOLD, FoldToggle);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_COLLAPSE_ALL, FoldCollapseAll);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_EXPAND_ALL, FoldExpandAll);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_ENCODING_ANSI, SetEncodingANSI);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_ENCODING_UTF8, SetEncodingUTF8);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_ENCODING_UTF16BE, SetEncodingUTF16BE);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_ENCODING_UTF16LE, SetEncodingUTF16LE);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_LINE_ENDING_CRLF, SetLineEndingCRLF);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_LINE_ENDING_LF, SetLineEndingLF);
            RegisterActionHandler(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_LINE_ENDING_CR, SetLineEndingCR);

            RegisterActionStateHandler(
                QuickSharp.Core.Constants.UI_TOOLBAR_SAVE, ActionEnabled);

            RegisterActionHandler(
                QuickSharp.Core.Constants.UI_TOOLBAR_SAVE, Save);

            #endregion
        }

        #region Reload

        public override void Reload()
        {
            // Can't reload if no file.
            if (String.IsNullOrEmpty(FilePath) || !File.Exists(FilePath))
                return;

            /*
             * Reload the document from disk and update file
             * characteristics such as encoding and line ending.
             */

            MainForm mainForm = ApplicationManager.GetInstance().MainForm;

            try
            {
                mainForm.Refresh();
                mainForm.Cursor = Cursors.WaitCursor;

                string fileText = null;
                Encoding encoding;

                try
                {
                    fileText = FileTools.ReadFile(FilePath, out encoding);
                    if (fileText == null) return;
                }
                catch (Exception ex)
                {
                    mainForm.Cursor = Cursors.Default;

                    MessageBox.Show(String.Format("{0}\r\n{1}",
                        Resources.OpenErrorMessage,
                        ex.Message),
                        Resources.OpenErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                    return;
                }

                bool isReadOnly = Editor.IsReadOnly;
                Editor.IsReadOnly = false;

                FileEncoding = encoding;
                LineEnding = documentManager.DetectLineEnding(fileText);
                Editor.EndOfLine.Mode = LineEnding;
                Editor.Text = fileText;
                Editor.Modified = false;
                Editor.IsReadOnly = isReadOnly;

                /*
                 * Note the caret position will be restored to (1,1).
                 * We could try to preserve it but as the file has changed
                 * there's no knowing how valid the old location will be
                 * in the new file.
                 */

                FileTimeStamp = DateTime.Now;
            }
            finally
            {
                mainForm.Cursor = Cursors.Default;
            }

            /*
             * Refresh the UI.
             */

            UpdateTab();
            UpdateCursorPosition();
        }

        #endregion
    }
}
