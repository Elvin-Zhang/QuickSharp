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
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using QuickSharp.Core;
using QuickSharp.Editor;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.SqlEditor
{
    public partial class SqlEditForm : ScintillaEditForm
    {
        private ApplicationManager _applicationManager;
        private int _selectedTable;
        private ToolStripMenuItem UI_DATA_GRID_MENU_CLEAR;
        private ToolStripSeparator UI_DATA_GRID_MENU_SEP_1;
        private ToolStripSeparator UI_DATA_GRID_MENU_SEP_2;
        private ToolStripMenuItem UI_DATA_GRID_MENU_EXPORT_XSD;
        private ToolStripMenuItem UI_DATA_GRID_MENU_EXPORT_XML;

        public SqlEditForm()
        {
            InitializeComponent();

            _applicationManager = ApplicationManager.GetInstance();
            _selectedTable = -1;

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

            UI_DATA_GRID_MENU_CLEAR = new ToolStripMenuItem();
            UI_DATA_GRID_MENU_CLEAR.Name = Constants.UI_DATA_GRID_MENU_CLEAR;
            UI_DATA_GRID_MENU_CLEAR.Text = Resources.DataGridMenuClear;
            UI_DATA_GRID_MENU_CLEAR.Click += 
                new EventHandler(UI_DATA_GRID_MENU_CLEAR_Click);

            UI_DATA_GRID_MENU_SEP_1 = 
                MenuTools.CreateSeparator(Constants.UI_DATA_GRID_MENU_SEP_1);

            UI_DATA_GRID_MENU_SEP_2 =
                MenuTools.CreateSeparator(Constants.UI_DATA_GRID_MENU_SEP_2);

            UI_DATA_GRID_MENU_EXPORT_XSD = new ToolStripMenuItem();
            UI_DATA_GRID_MENU_EXPORT_XSD.Name = Constants.UI_DATA_GRID_MENU_EXPORT_XSD;
            UI_DATA_GRID_MENU_EXPORT_XSD.Text = Resources.DataGridMenuExportXsd;
            UI_DATA_GRID_MENU_EXPORT_XSD.Click +=
                new EventHandler(UI_DATA_GRID_MENU_EXPORT_XSD_Click);

            UI_DATA_GRID_MENU_EXPORT_XML = new ToolStripMenuItem();
            UI_DATA_GRID_MENU_EXPORT_XML.Name = Constants.UI_DATA_GRID_MENU_EXPORT_XML;
            UI_DATA_GRID_MENU_EXPORT_XML.Text = Resources.DataGridMenuExportXml;
            UI_DATA_GRID_MENU_EXPORT_XML.Click +=
                new EventHandler(UI_DATA_GRID_MENU_EXPORT_XML_Click);
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

            MainForm mainForm = _applicationManager.MainForm;

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

        #region Properties

        public DataGridView DataGrid
        {
            get { return _dataGrid; }
        }

        public DataSet DataSet
        {
            get { return _dataSet; }
            set { _dataSet = value; }
        }

        public int TableCount
        {
            get { return _dataSet.Tables.Count; }
        }

        #endregion

        #region Data Table Display

        public void ClearData()
        {
            _selectedTable = -1;
            _dataGrid.DataSource = null;
            _dataSet = null;
        }

        public void ShowTable()
        {
            ShowTable(0);
        }

        public void ShowTable(int index)
        {
            if (index > TableCount - 1)
            {
                _selectedTable = -1;
                return;
            }

            _selectedTable = index;

            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = _dataSet.Tables[index];

            _dataGrid.DataSource = bindingSource;
        }

        #endregion

        #region Event Handlers

        private void DataGridContextMenuStrip_Opening(
            object sender, System.ComponentModel.CancelEventArgs e)
        {
            _dataGridMenu.Items.Clear();
            _dataGridMenu.Items.Add(UI_DATA_GRID_MENU_CLEAR);
            
            UI_DATA_GRID_MENU_CLEAR.Enabled = false;

            if (_dataSet != null && TableCount > 0)
            {
                UI_DATA_GRID_MENU_CLEAR.Enabled = true;

                _dataGridMenu.Items.Add(UI_DATA_GRID_MENU_SEP_1);

                for (int i = 0; i < TableCount; i++)
                {
                    int rowCount = _dataSet.Tables[i].Rows.Count;

                    ToolStripMenuItem menuItem = new ToolStripMenuItem();
                    menuItem.Tag = i;
                    menuItem.Click += new EventHandler(DataTable_Click);
                    menuItem.Image = Resources.DataGrid;
                    menuItem.ImageTransparentColor = Color.Fuchsia;
                    menuItem.Text = String.Format(
                        Resources.DataGridMenuDataTable, i + 1,
                        rowCount, rowCount == 1 ?
                            Resources.DataRowsSingular :
                            Resources.DataRowsPlural);

                    if (i == _selectedTable)
                        menuItem.Checked = true;

                    _dataGridMenu.Items.Add(menuItem);
                }

                _dataGridMenu.Items.Add(UI_DATA_GRID_MENU_SEP_2);

                if (_applicationManager.ClientProfile.HaveFlag(
                    ClientFlags.SqlEditorEnableXsdExport))
                    _dataGridMenu.Items.Add(UI_DATA_GRID_MENU_EXPORT_XSD);

                _dataGridMenu.Items.Add(UI_DATA_GRID_MENU_EXPORT_XML);
            }
        }

        private void DataTable_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null) return;

            int index = (int) item.Tag;
            ShowTable(index);
        }

        private void UI_DATA_GRID_MENU_CLEAR_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        private void UI_DATA_GRID_MENU_EXPORT_XSD_Click(object sender, EventArgs e)
        {
            if (_selectedTable == -1) return;

            string fileName = GetFileName(GetTableName("xsd"), true);
            if (fileName == null) return;

            try
            {
                Cursor = Cursors.WaitCursor;
                Refresh();

                _dataSet.Tables[_selectedTable].WriteXmlSchema(fileName);
                _applicationManager.NotifyFileSystemChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    String.Format("{0}:\r\n{1}",
                        Resources.ExportErrorMessage,
                        ex.Message),
                    Resources.ExportDialogTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void UI_DATA_GRID_MENU_EXPORT_XML_Click(object sender, EventArgs e)
        {
            if (_selectedTable == -1) return;

            string fileName = GetFileName(GetTableName("xml"), false);
            if (fileName == null) return;

            try
            {
                Cursor = Cursors.WaitCursor;
                Refresh();

                _dataSet.Tables[_selectedTable].WriteXml(fileName);
                _applicationManager.NotifyFileSystemChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    String.Format("{0}:\r\n{1}",
                        Resources.ExportErrorMessage,
                        ex.Message),
                    Resources.ExportDialogTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private string GetTableName(string type)
        {
            string tableName = String.Format(
                "DataTable{0}.{1}", _selectedTable + 1, type);

            return tableName;
        }

        private string GetFileName(string name, bool isXsd)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = Resources.ExportDialogTitle;
            sfd.FileName = name;
            sfd.InitialDirectory = Directory.GetCurrentDirectory();

            if (isXsd)
            {
                sfd.DefaultExt = ".xsd";
                sfd.Filter = Resources.ExportDialogXsdFilter;
            }
            else
            {
                sfd.DefaultExt = ".xml";
                sfd.Filter = Resources.ExportDialogXmlFilter;
            }

            if (sfd.ShowDialog() == DialogResult.OK)
                return sfd.FileName;

            return null;
        }

        #endregion
    }
}
