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
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using QuickSharp.Core;
using ScintillaNet;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.Editor
{
    /// <summary>
    /// Provides the basic functionality for Scintilla-based text editors. As an
    /// abstract class this cannot be used directly; concrete editor classes must
    /// be defined to provide the actual editor user interface. See QuickSharp.TextEditor
    /// and QuickSharp.SqlEditor for examples.
    /// </summary>
    public abstract class ScintillaEditForm : QuickSharp.Core.Document
    {
        protected Scintilla scintilla;
        protected ApplicationManager applicationManager;
        protected DocumentManager documentManager;
        private SettingsManager _settingsManager;
        private MacroManager _macroManager;
        private MainForm _mainForm;
        private Encoding _fileEncoding;
        private EndOfLineMode _lineEnding;

        #region Properties

        /// <summary>
        /// A reference to the Scintilla editor.
        /// </summary>
        public Scintilla Editor
        {
            get { return scintilla; }
        }

        /// <summary>
        /// The lines of text being edited.
        /// </summary>
        public LinesCollection Lines
        {
            get { return scintilla.Lines; }
        }

        /// <summary>
        /// The name of the file being edited.
        /// </summary>
        public override string FileName
        {
            get { return documentFileName; }

            set
            {
                documentFileName = value;
                Editor.Printing.PrintDocument.DocumentName = value;
            }
        }

        /// <summary>
        /// The path of the file being edited.
        /// </summary>
        public override string FilePath
        {
            get { return documentFilePath; }

            set
            {
                documentFilePath = value;
                
                if (!scintilla.IsReadOnly)
                    scintilla.Modified = true;

                UpdateTab();
            }
        }

        /// <summary>
        /// The encoding of the file being edited.
        /// </summary>
        public Encoding FileEncoding
        {
            /*
             * Don't use scintilla.Encoding; setting it
             * only seems to upset the text encoding.
             */

            get
            {
                return _fileEncoding;
            }
            set
            {
                _fileEncoding = value;

                // Allow extended chars for anything except ANSI

                if (_fileEncoding == Encoding.ASCII)
                    scintilla.NativeInterface.SetCodePage(
                        ScintillaNet.Constants.SC_CHARSET_ANSI);
                else 
                    scintilla.NativeInterface.SetCodePage(
                        (int)ScintillaNet.Constants.SC_CP_UTF8);
            }
        }

        /// <summary>
        /// The encoding display name of the file being edited.
        /// </summary>
        public string FileEncodingName
        {
            get
            {
                if (_fileEncoding == Encoding.ASCII)
                    return "ANSI";
                if (_fileEncoding == Encoding.UTF8)
                    return "UTF-8";
                if (_fileEncoding == Encoding.Unicode)
                    return "UTF-16 (LE)";
                if (_fileEncoding == Encoding.BigEndianUnicode)
                    return "UTF-16 (BE)";

                return String.Empty;
            }
        }

        /// <summary>
        /// The line ending style of the file being edited.
        /// </summary>
        public EndOfLineMode LineEnding
        {
            get { return _lineEnding; }
            set { _lineEnding = value; }
        }

        /// <summary>
        /// The line ending display name of the file being edited.
        /// </summary>
        public string LineEndingName
        {
            get
            {
                switch (_lineEnding)
                {
                    case EndOfLineMode.Crlf:
                        return "CRLF";
                    case EndOfLineMode.LF:
                        return "LF";
                    case EndOfLineMode.CR:
                        return "CR";
                    default:
                        return String.Empty;
                }
            }
        }

        /// <summary>
        /// The text used to represent the line endings in the document being edited.
        /// </summary>
        public string LineEndingText
        {
            get
            {
                switch (_lineEnding)
                {
                    case EndOfLineMode.Crlf:
                        return "\r\n";
                    case EndOfLineMode.LF:
                        return "\n";
                    case EndOfLineMode.CR:
                        return "\r";
                    default:
                        return String.Empty;
                }
            }
        }

        #endregion

        /// <summary>
        /// Create the editor.
        /// </summary>
        public ScintillaEditForm()
        {
            applicationManager = ApplicationManager.GetInstance();
            documentManager = DocumentManager.GetInstance();
            _settingsManager = SettingsManager.GetInstance();
            _macroManager = MacroManager.GetInstance();
            _mainForm = applicationManager.MainForm;

            scintilla = new Scintilla();
            ((ISupportInitialize)(scintilla)).BeginInit();
            scintilla.Dock = DockStyle.Fill;
            scintilla.Name = "scintilla";
            scintilla.TabIndex = 0;
            scintilla.Enter += new EventHandler(Scintilla_Enter);
            scintilla.Leave += new EventHandler(Scintilla_Leave);
            scintilla.MouseClick += new MouseEventHandler(Scintilla_MouseClick);
            scintilla.ModifiedChanged += new EventHandler(Scintilla_ModifiedChanged);
            scintilla.KeyUp += new KeyEventHandler(Scintilla_KeyUp);
            ((ISupportInitialize)(scintilla)).EndInit();
            scintilla.MacroRecord += new EventHandler<MacroRecordEventArgs>(Scintilla_MacroRecord);

            if (!applicationManager.ClientProfile.HaveFlag(
                ClientFlags.EditorDisableDragAndDropFileOpen))
            {
                scintilla.AllowDrop = true;
                scintilla.FileDrop += new EventHandler<FileDropEventArgs>(Scintilla_FileDrop);
            }

            FormClosing += new FormClosingEventHandler(ScintillaEditForm_FormClosing);
        }

        /// <summary>
        /// Reload the file being edited (overriden in derived classes).
        /// </summary>
        public virtual void Reload()
        {
        }

        protected override string GetPersistString()
        {
            if (FilePath == null)
                return String.Empty;
            else if (scintilla.IsReadOnly)
                return FilePath + "?";
            else
                return FilePath;
        }

        #region Base Class Overrides

        /// <summary>
        /// Get the text being edited.
        /// </summary>
        /// <returns>The text being edited.</returns>
        public override object GetContent()
        {
            return (object) scintilla.Text;
        }

        /// <summary>
        /// Set the text to be edited.
        /// </summary>
        /// <param name="content">The text to be edited.</param>
        public override void SetContent(object content)
        {
            scintilla.Text = content as string;
        }

        /// <summary>
        /// Set the editing position.
        /// </summary>
        /// <param name="x">The column position.</param>
        /// <param name="y">The line position.</param>
        public override void SetLocation(int x, int y)
        {
            y--; // Scintilla lines are Zero-indexed
            scintilla.GoTo.Line(y);
            scintilla.Lines[y].EnsureVisible();
            scintilla.Lines[y].Select();

            UpdateCursorPosition();
        }

        /// <summary>
        /// If true allow the file to be opened in multiple editors.
        /// </summary>
        /// <returns>True if multiple instances are allowed.</returns>
        public override bool AllowDuplicates()
        {
            return scintilla.IsReadOnly;
        }

        #endregion

        #region ActionState Handlers

        protected bool ActionEnabled()
        {
            return true;
        }

        protected bool HaveSelection()
        {
            return scintilla.Selection.Length > 0;
        }

        protected bool CanUndo()
        {
            return scintilla.UndoRedo.CanUndo;
        }

        protected bool CanRedo()
        {
            return scintilla.UndoRedo.CanRedo;
        }

        protected bool CanPaste()
        {
            return scintilla.Clipboard.CanPaste;
        }

        protected bool CanLineComment()
        {
            if (String.IsNullOrEmpty(scintilla.Lexing.LineCommentPrefix))
                return false;

            return true;
        }

        protected bool CanBlockComment()
        {
            if (String.IsNullOrEmpty(scintilla.Lexing.StreamCommentPrefix))
                return false;

            return true;
        }

        protected bool HaveFolding()
        {
            return scintilla.Folding.IsEnabled;
        }

        #endregion

        #region Action Handlers

        /// <summary>
        /// Save the current document.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool Save()
        {
            return SaveDocument();
        }

        /// <summary>
        /// Save the document with a new name.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool SaveAs()
        {
            return SaveDocumentAs();
        }

        /// <summary>
        /// Open the page setup dialog.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool PageSetup()
        {
            try
            {
                return (scintilla.Printing.ShowPageSetupDialog() == DialogResult.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    Resources.PrintErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        /// <summary>
        /// Display the print preview.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool PrintPreview()
        {
            try
            {
                return (scintilla.Printing.PrintPreview() == DialogResult.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    Resources.PrintErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        /// <summary>
        /// Open the print dialog.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool Print()
        {
            try
            {
                return scintilla.Printing.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    Resources.PrintErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        /// <summary>
        /// Editor undo.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool Undo()
        {
            scintilla.UndoRedo.Undo();
            return true;
        }

        /// <summary>
        /// Editor redo.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool Redo()
        {
            scintilla.UndoRedo.Redo();
            return true;
        }

        /// <summary>
        /// Clipboard cut.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool Cut()
        {
            scintilla.Clipboard.Cut();
            return true;
        }

        /// <summary>
        /// Clipboard copy.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool Copy()
        {
            scintilla.Clipboard.Copy();
            return true;
        }

        /// <summary>
        /// Clipboard paste.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool Paste()
        {
            scintilla.Clipboard.Paste();
            return true;
        }

        /// <summary>
        /// Select all text.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool SelectAll()
        {
            scintilla.Selection.SelectAll();
            return true;
        }

        /// <summary>
        /// Show the find dialog.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool Find()
        {
            SearchManager.GetInstance().ShowSearchForm(false);
            return true;
        }

        /// <summary>
        /// Show the replace dialog.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool Replace()
        {
            SearchManager.GetInstance().ShowSearchForm(true);
            return true;
        }

        /// <summary>
        /// Show the go to line dialog.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool GoTo()
        {
            scintilla.GoTo.ShowGoToDialog();
            return true;
        }

        /// <summary>
        /// Toggle keystroke recording.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool MacroRecord()
        {
            if (_macroManager.IsRecording)
            {
                _macroManager.StopRecording();
                if (_macroManager.HaveMacro)
                    _mainForm.SetStatusBarMessage(Resources.MacroRecordEnd, 5);
                else
                    _mainForm.SetStatusBarMessage(Resources.MacroRecordCancelled, 5);
            }
            else
            {
                _mainForm.SetStatusBarMessage(Resources.MacroRecordStart);
                _macroManager.StartRecording(scintilla);
            }

            return true;
        }

        /// <summary>
        /// Play the current macro.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool MacroPlay()
        {
            if (!_macroManager.IsRecording)
            {
                if (_macroManager.HaveMacro)
                    _macroManager.PlayMacro(scintilla);
                else
                    _mainForm.SetStatusBarMessage(Resources.MacroBufferEmpty, 5);
            }
            
            return true;
        }

        /// <summary>
        /// Load a macro from a file.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool MacroLoad()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = Resources.MacroLoadTitle;
            ofd.Filter = Resources.MacroDocumentsFilter;
            ofd.DefaultExt = Resources.MacroFileExtension;
            ofd.InitialDirectory = Directory.GetCurrentDirectory();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _macroManager.LoadMacro(ofd.FileName);
                    _mainForm.SetStatusBarMessage(Resources.MacroLoadOK, 5);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format(
                            Resources.MacroLoadError, ex.Message),
                        Resources.MacroLoadTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            return true;
        }

        /// <summary>
        /// Save the current macro to a file.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool MacroSave()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = Resources.MacroSaveTitle;
            sfd.Filter = Resources.MacroDocumentsFilter;
            sfd.SupportMultiDottedExtensions = true;
            sfd.DefaultExt = Resources.MacroFileExtension;
            sfd.InitialDirectory = Directory.GetCurrentDirectory();

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _macroManager.SaveMacro(sfd.FileName);
                    _mainForm.SetStatusBarMessage(Resources.MacroSaveOK, 5);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format(
                            Resources.MacroSaveError, ex.Message),
                        Resources.MacroSaveTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            return true;
        }

        /// <summary>
        /// Make selection uppercase.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool MakeUppercase()
        {
            return scintilla.Commands.Execute(BindableCommand.UpperCase);
        }

        /// <summary>
        /// Make selection lowercase.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool MakeLowercase()
        {
            return scintilla.Commands.Execute(BindableCommand.LowerCase);
        }

        /// <summary>
        /// Toggle the selected or current line as a line comment.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool LineComment()
        {
            return scintilla.Commands.Execute(
                BindableCommand.ToggleLineComment);
        }

        /// <summary>
        /// Apply a block comment to the selected or current.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool BlockComment()
        {
            return scintilla.Commands.Execute(BindableCommand.StreamComment);
        }

        /// <summary>
        /// Toggle the display of whitespace characters.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool ViewWhitespace()
        {
            ToolStripMenuItem menuItem = _mainForm.GetMenuItemByName(
                Constants.UI_EDIT_MENU_VIEW_WHITESPACE);

            if (menuItem == null) return false;

            menuItem.Checked = !menuItem.Checked;

            if (menuItem.Checked)
            {
                scintilla.WhiteSpace.Mode = WhiteSpaceMode.VisibleAlways;
                scintilla.EndOfLine.IsVisible = true;
            }
            else
            {
                scintilla.WhiteSpace.Mode = WhiteSpaceMode.Invisible;
                scintilla.EndOfLine.IsVisible = false;
            }

            return true;
        }

        /// <summary>
        /// Trim whitespace characters from the end of each line in the document.
        /// </summary>
        /// <returns>Allways returns true.</returns>
        public bool TrimWhitespace()
        {
            foreach (Line line in scintilla.Lines)
                line.Text = line.Text.TrimEnd();

            scintilla.UndoRedo.EmptyUndoBuffer();

            return true;
        }

        /// <summary>
        /// Toggle the wrapping of long lines.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool WordWrap()
        {
            ToolStripMenuItem menuItem = _mainForm.GetMenuItemByName(
                Constants.UI_EDIT_MENU_WORD_WRAP);

            if (menuItem == null) return false;

            menuItem.Checked = !menuItem.Checked;

            if (menuItem.Checked)
                scintilla.LineWrap.Mode = WrapMode.Word;
            else
                scintilla.LineWrap.Mode = WrapMode.None;

            return true;
        }

        /// <summary>
        /// Toggle the current document as read-only.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool SetReadOnly()
        {
            ToolStripMenuItem menuItem = _mainForm.GetMenuItemByName(
                Constants.UI_EDIT_MENU_SET_READ_ONLY);

            if (menuItem == null) return false;

            if (scintilla.IsReadOnly)
            {
                /*
                 * Check not already open as read/write.
                 */

                foreach (IDockContent dockContent in 
                    _mainForm.ClientWindow.Documents)
                {
                    ScintillaEditForm f = dockContent as ScintillaEditForm;
                    if (f == null) continue;
                    if (f.FilePath == null) continue;
                    if (documentFilePath == null) continue;
                    if (FileTools.MatchPaths(f.FilePath, documentFilePath) &&
                        !f.Editor.IsReadOnly)
                    {
                        MessageBox.Show(
                            Resources.ReadWriteMessage,
                            Resources.ReadWriteTitle,
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        UpdateTab();
                        f.Activate();
                        return false;
                    }
                }

                menuItem.Checked = false;
                scintilla.IsReadOnly = false;
            }
            else
            {
                menuItem.Checked = true;
                scintilla.IsReadOnly = true;
            }

            UpdateTab();

            return true;
        }

        /// <summary>
        /// Set the document encoding to ANSI.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool SetEncodingANSI()
        {
            return SetFileEncoding(Encoding.ASCII);
        }

        /// <summary>
        /// Set the document encoding to UTF-8.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool SetEncodingUTF8()
        {
            return SetFileEncoding(Encoding.UTF8);
        }

        /// <summary>
        /// Set the document encoding to UTF-16BE.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool SetEncodingUTF16BE()
        {
            return SetFileEncoding(Encoding.BigEndianUnicode);
        }

        /// <summary>
        /// Set the document encoding to UTF-16LE.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool SetEncodingUTF16LE()
        {
            return SetFileEncoding(Encoding.Unicode);
        }

        /// <summary>
        /// Set the document line ending to Windows CRLF.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool SetLineEndingCRLF()
        {
            return SetLineEnding(EndOfLineMode.Crlf);
        }

        /// <summary>
        /// Set the document line ending to Unix LF.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool SetLineEndingLF()
        {
            return SetLineEnding(EndOfLineMode.LF);
        }

        /// <summary>
        /// Set the document line ending to Mac CR.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool SetLineEndingCR()
        {
            return SetLineEnding(EndOfLineMode.CR);
        }

        /// <summary>
        /// Toggle the current line bookmark.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool BookMarkToggle()
        {
            // Marker number 1 is mask 01 (0x01).
            if ((scintilla.Lines.Current.GetMarkerMask() &
                Constants.BOOKMARK_MASK) == Constants.BOOKMARK_MASK)
                scintilla.Lines.Current.DeleteMarker(Constants.BOOKMARK_MARKER);
            else
                scintilla.Lines.Current.AddMarker(Constants.BOOKMARK_MARKER);

            return true;
        }

        /// <summary>
        /// Clear all bookmarks.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool BookMarkClear()
        {
            scintilla.Markers.DeleteAll(Constants.BOOKMARK_MARKER);
            return true;
        }

        /// <summary>
        /// Move to the next bookmark.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool BookmarkNext()
        {
            Line line = scintilla.Markers.FindNextMarker(
                scintilla.Lines.Current.Number + 1, Constants.BOOKMARK_MASK);

            if (line != null)
            {
                scintilla.CurrentPos = line.StartPosition;
                line.EnsureVisible();
            }

            return true;
        }

        /// <summary>
        /// Move to the previous bookmark.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool BookmarkPrevious()
        {
            Line line = scintilla.Markers.FindPreviousMarker(
                scintilla.Lines.Current.Number - 1, Constants.BOOKMARK_MASK);

            if (line != null)
            {
                scintilla.CurrentPos = line.StartPosition;
                line.EnsureVisible();
            }
            
            return true;
        }

        /// <summary>
        /// Toggle folding on the current line.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool FoldToggle()
        {
            Line l = scintilla.Lines.Current;
            if (l.IsFoldPoint)
                l.ToggleFoldExpanded();

            return true;
        }

        /// <summary>
        /// Collapse all folds in the current document.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool FoldCollapseAll()
        {
            scintilla.Lexing.Colorize();

            foreach (Line l in scintilla.Lines)
                if (l.IsFoldPoint && l.FoldExpanded)
                    l.ToggleFoldExpanded();

            return true;
        }

        /// <summary>
        /// Expand all folds in the current document.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool FoldExpandAll()
        {
            scintilla.Lexing.Colorize();

            foreach (Line l in scintilla.Lines)
                if (l.IsFoldPoint && !l.FoldExpanded)
                    l.ToggleFoldExpanded();

            return true;
        }

        #endregion

        #region File Operations

        /// <summary>
        /// Save the document: saving proceeds directly if the file has a file path or
        /// via a Save As dialog if not. Save requests for unmodified files that exist on
        /// disk are ignored.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool SaveDocument()
        {
            if (!scintilla.Modified && File.Exists(documentFilePath))
                return true;

            if (String.IsNullOrEmpty(documentFilePath))
                return SaveDocumentAs();
            else
                return SaveFile(documentFilePath);
        }

        /// <summary>
        /// Request a filename and save the document with that name.
        /// </summary>
        /// <returns>True on success.True on success.</returns>
        public bool SaveDocumentAs()
        {
            FileInfo fi = new FileInfo(documentFileName);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = applicationManager.AllDocumentsFilter;
            sfd.SupportMultiDottedExtensions = true;
            sfd.AddExtension = false;
            sfd.InitialDirectory = Directory.GetCurrentDirectory();
            sfd.FileName = documentFileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                /*
                 * sfd.DefaultExtension doesn't work the way we want
                 * so we do it manually here.
                 */

                /*
                 * Win7 Note: originally we applied the default documents
                 * filter to the save as dialog but file types not included
                 * in the filter were ignored in Win7 so that (for example)
                 * .aspx files would be saved as .cs if no extension was
                 * supplied.
                 */

                string filename = sfd.FileName;

                if (Path.GetExtension(filename) == String.Empty)
                    filename = 
                        Path.GetFileNameWithoutExtension(filename) +
                        fi.Extension;

                return SaveFile(filename);
            }
            else
                return false;
        }

        /// <summary>
        /// Write the document to the path specified.
        /// </summary>
        /// <param name="path">The path to save to.</param>
        /// <returns>True on success.</returns>
        public bool SaveFile(string path)
        {
            try
            {
                /*
                 * Check to see if the on-disk file is more recent
                 * than the file we are about to save.
                 */

                if (!applicationManager.ClientProfile.HaveFlag(
                        ClientFlags.EditorDisableFileTimestampCheck) &&
                    FileTools.FileIsNewerOnDisk(path, FileTimeStamp))
                {
                    if (MessageBox.Show(
                            String.Format("{0}\r\n\r\n{1}\r\n{2}",
                                FilePath,
                                Resources.FileChangedMessage,
                                Resources.FileChangedOverwriteMessage),
                            Resources.FileChangedTitle,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        return false;
                    }
                }

                /*
                 * Save the file.
                 */

                FileTools.WriteFile(path, scintilla.Text, FileEncoding);
                scintilla.Modified = false;

                FileInfo fi = new FileInfo(path);
                Text = fi.Name;
                documentFileName = fi.Name;
                documentFilePath = fi.FullName;
                documentTimeStamp = DateTime.Now;
                scintilla.Printing.PrintDocument.DocumentName = fi.Name;

                string fileDirectory = Path.GetDirectoryName(documentFilePath);

                if (fileDirectory != Directory.GetCurrentDirectory() &&
                    applicationManager.ClientProfile.HaveFlag(
                        ClientFlags.EditorChangeDirectoryOnSave))
                    Directory.SetCurrentDirectory(fileDirectory);

                /*
                 * If the file extension has changed we will need to
                 * update the document type and editor language.
                 * This will only switch lexers within the editor - if
                 * the new type uses a different editor it will remain
                 * within the current editor.
                 */

                DocumentType documentType =
                    new DocumentType(fi.Extension);

                string language = documentManager.
                    GetDocumentLanguage(documentType);

                if (scintilla.ConfigurationManager.Language != language)
                    scintilla.ConfigurationManager.Language = language;

                /*
                 * Update any filesystem views.
                 */

                applicationManager.NotifyFileSystemChange();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}\r\n{1}",
                        Resources.SaveErrorMessage,
                        ex.Message),
                    Resources.SaveErrorTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
        }

        /// <summary>
        /// Set the current document encoding.
        /// </summary>
        /// <param name="encoding">A file encoding.</param>
        /// <returns>Always returns true.</returns>
        public bool SetFileEncoding(Encoding encoding)
        {
            FileEncoding = encoding;
            scintilla.Modified = true;
            UpdateCursorPosition();

            return true;
        }

        /// <summary>
        /// Set the document line-ending style.
        /// </summary>
        /// <param name="ending">A line-ending style.</param>
        /// <returns>Always returns true.</returns>
        public bool SetLineEnding(EndOfLineMode ending)
        {
            scintilla.NativeInterface.ConvertEols((int)ending);
            scintilla.EndOfLine.Mode = ending;
            LineEnding = ending;
            UpdateCursorPosition();

            return true;
        }

        #endregion

        #region Snippets

        /// <summary>
        /// Insert a text snippet into the current document. Text is inserted at the
        /// current position or in place of the current selection.
        /// </summary>
        /// <param name="snippet">The text to insert.</param>
        public void InsertSnippet(string snippet)
        {
            string eol = LineEndingText;
            snippet = ConvertLineEndings(snippet, eol);

            /*
             * Add any leading text at the insertion point to the snippet.
             */

            Line line = scintilla.Lines.Current;
            int curPos = line.SelectionStartPosition - line.StartPosition;

            if (curPos >= 0 && curPos <= line.Text.Length)
            {
                string text = line.Text.Substring(0, curPos);

                if (!String.IsNullOrEmpty(text))
                {
                    text = ConvertToWhiteSpace(text);
                    snippet = snippet.Replace(eol, eol + text);
                }
            }

            /*
             * Insert the snippet.
             */

            scintilla.Selection.Text = snippet;
        }

        protected string ConvertToWhiteSpace(string text)
        {
            char[] buffer = new char[text.Length];

            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsWhiteSpace(text[i]))
                    buffer[i] = text[i];
                else
                    buffer[i] = ' ';
            }

            return new String(buffer);
        }

        protected string ConvertLineEndings(string text, string newEol)
        {
            text = text.Replace("\r\n", "\n");  // normalize Windows
            text = text.Replace("\r", "\n");    // normalize Mac
            return text.Replace("\n", newEol);  // use requested
        }

        protected string ConvertLineEndingsToUnix(string text)
        {
            text = text.Replace("\r\n", "\n");  // normalize Windows
            return text.Replace("\r", "\n");    // normalize Mac
        }

        #endregion

        #region Form Events

        protected void ScintillaEditForm_FormClosing(
            object sender, FormClosingEventArgs e)
        {
            SearchManager.GetInstance().HideSearchForm();

            if (scintilla.Modified)
            {
                DialogResult res = MessageBox.Show(String.Format(
                        Resources.SaveChangesMessage,
                        documentFileName),
                    Resources.SaveChangesTitle,
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);

                if (res == DialogResult.Cancel)
                    e.Cancel = true;
                else if (res == DialogResult.Yes)
                    e.Cancel = !SaveDocument();
            }
        }

        #endregion

        #region Scintilla Events

        protected void Scintilla_ModifiedChanged(object sender, EventArgs e)
        {
            UpdateTab();
        }

        protected void Scintilla_Leave(object sender, EventArgs e)
        {
            _mainForm.StatusBar.Items[
                Constants.UI_STATUSBAR_CURSOR_POSITION_INDICATOR
                ].Text = String.Empty;
        }

        protected void Scintilla_Enter(object sender, EventArgs e)
        {
            UpdateEditor();
            UpdateCursorPosition();
        }

        protected void Scintilla_MouseClick(object sender, MouseEventArgs e)
        {
            UpdateCursorPosition();
        }

        protected void Scintilla_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateCursorPosition();
        }

        protected void Scintilla_MacroRecord(object sender, MacroRecordEventArgs e)
        {
            if (_macroManager.IsRecording)
                _macroManager.RecordEvent(e);
        }

        protected void Scintilla_FileDrop(object sender, FileDropEventArgs e)
        {
            foreach (string file in e.FileNames)
                _mainForm.LoadDocumentIntoWindow(file, true);
        }

        #endregion

        #region UI Updates

        private void UpdateEditor()
        {
            /*
             * Update the Show Whitespace, Word Wrap and Read Only menu settings.
             */

            ToolStripMenuItem whitespaceMenuItem =
                _mainForm.GetMenuItemByName(
                    Constants.UI_EDIT_MENU_VIEW_WHITESPACE);

            if (whitespaceMenuItem != null)
                whitespaceMenuItem.Checked =
                    (scintilla.WhiteSpace.Mode == WhiteSpaceMode.VisibleAlways);

            ToolStripMenuItem wordwrapMenuItem =
                _mainForm.GetMenuItemByName(
                    Constants.UI_EDIT_MENU_WORD_WRAP);

            if (wordwrapMenuItem != null)
                wordwrapMenuItem.Checked =
                    (scintilla.LineWrap.Mode == WrapMode.Word);

            ToolStripMenuItem readonlyMenuItem =
                _mainForm.GetMenuItemByName(
                    Constants.UI_EDIT_MENU_SET_READ_ONLY);

            if (readonlyMenuItem != null)
                readonlyMenuItem.Checked = scintilla.IsReadOnly;

            /*
             * Check the timestamp and prompt for reload if out of date.
             */

            if (!applicationManager.ClientProfile.HaveFlag(
                    ClientFlags.EditorDisableFileTimestampCheck))
            {
                /*
                 * An advanced timestamp will play havoc with the change detection.
                 * Annoy the user until they save the file with a current timestamp!
                 */

                if (FileTools.FileIsNewerOnDisk(FilePath, DateTime.Now))
                {
                    MessageBox.Show(String.Format("{0}:\r\n{1}",
                            Resources.FileFutureDateMessage1,
                            Resources.FileFutureDateMessage2),
                        Resources.FileFutureDateTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else if (FileTools.FileIsNewerOnDisk(FilePath, FileTimeStamp))
                {
                    if (MessageBox.Show(
                            String.Format("{0}\r\n\r\n{1}\r\n{2}", FilePath,
                                Resources.FileChangedMessage,
                                Resources.FileChangedReloadMessage),
                            Resources.FileChangedTitle,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning) == DialogResult.Yes)
                    {

                        /* 
                         * Bug: clicking a button on the dialog causes the editor to begin
                         * highlighting text. The mouse click on the dialog button seems to
                         * get passed through to the active editor and begins the highlighting
                         * operation.
                         */

                        Reload();
                    }
                }
            }
        }

        /// <summary>
        /// Update the editor display tab.
        /// </summary>
        public void UpdateTab()
        {
            if (scintilla.Modified)
                Text = documentFileName + "*";
            else
                Text = documentFileName;

            if (scintilla.IsReadOnly)
                Text += String.Format(" ({0})",
                    Resources.TabReadOnly);
        }

        /// <summary>
        /// Update the editing details in the status bar.
        /// </summary>
        public void UpdateCursorPosition()
        {
            _mainForm.StatusBar.Items[
                Constants.UI_STATUSBAR_CURSOR_POSITION_INDICATOR].Text =
                    String.Format("Ln {0}    Col {1}    {2} Lines    {3}    {4}    {5}",
                        scintilla.Lines.Current.VisibleLineNumber + 1,
                        scintilla.Lines.Current.SelectionStartPosition -
                        scintilla.Lines.Current.StartPosition + 1,
                        scintilla.Lines.Count,
                        LineEndingName,
                        FileEncodingName,
                        scintilla.OverType ? "OVR": "INS");
        }

        #endregion

        #region Editor Configuration

        /// <summary>
        /// Load editor settings from a configuration file.
        /// </summary>
        /// <param name="documentType">The document type of the file being edited.</param>
        public void LoadSettings(DocumentType documentType)
        {
            string scintillaConfig = Path.Combine(
                applicationManager.QuickSharpHome,
                Constants.CONFIG_DIR_NAME);

            /*
             * Look for theme specific sub-folder.
             */

            string themePath = applicationManager.
                GetSelectedThemeProviderKey();

            if (!String.IsNullOrEmpty(themePath))
            {
                string themeConfig = Path.Combine(
                    scintillaConfig, themePath);

                if (Directory.Exists(themeConfig))
                    scintillaConfig = themeConfig;
            }

            /*
             * Update scintilla if the location exists.
             */
            
            if (Directory.Exists(scintillaConfig))
                scintilla.ConfigurationManager.CustomLocation =
                    scintillaConfig;

            /*
             * Apply any lexer aliases.
             */

            Dictionary<String, String> lexerAliasMap =
                documentManager.LexerAliasMap;

            foreach (string alias in lexerAliasMap.Keys)
                if (!scintilla.Lexing.LexerLanguageMap.ContainsKey(alias))
                    scintilla.Lexing.LexerLanguageMap.Add(
                        alias, lexerAliasMap[alias]);

            /*
             * Language will be requested language or default.
             */

            string language = documentManager.GetDocumentLanguage(documentType);
            scintilla.ConfigurationManager.Language = language;

            /*
             * Set config from external files and internal settings.
             */

            scintilla.ConfigurationManager.IsUserEnabled = true;
            scintilla.UseFont = false;

            if (_settingsManager.OverrideConfigFiles)
            { 
                // Line numbers
                scintilla.Margins.Margin0.Type = MarginType.Number;
                scintilla.Margins.Margin0.Width = _settingsManager.LineNumberMarginSize;
                
                // Bookmarks
                scintilla.NativeInterface.SendMessageDirect(
                    ScintillaNet.Constants.SCI_MARKERDEFINE,
                        Constants.BOOKMARK_MARKER,
                        ScintillaNet.Constants.SC_MARK_SMALLRECT);
                scintilla.NativeInterface.SendMessageDirect(
                    ScintillaNet.Constants.SCI_MARKERSETFORE,
                        Constants.BOOKMARK_MARKER, 0);
                scintilla.NativeInterface.SendMessageDirect(
                    ScintillaNet.Constants.SCI_MARKERSETBACK,
                    Constants.BOOKMARK_MARKER, GetColor(Color.LightGreen));

                scintilla.Margins.Margin1.Type = MarginType.Symbol;
                scintilla.Margins.Margin1.Width = 8;

                // Folding
                scintilla.Margins.Margin2.Type = MarginType.Symbol;
                scintilla.Margins.Margin2.Width = 16;

                // Not used
                scintilla.Margins.Margin3.Width = 0;
                scintilla.Margins.Margin3.Type = MarginType.Back;

                scintilla.Indentation.ShowGuides =
                    _settingsManager.ShowIndentationGuides;
                scintilla.Indentation.TabWidth = _settingsManager.TabSize;
                scintilla.Indentation.UseTabs = _settingsManager.UseTabs;
                scintilla.Indentation.BackspaceUnindents =
                    _settingsManager.BackspaceUnindents;

                scintilla.Folding.IsEnabled = _settingsManager.ShowFolding;
                scintilla.Folding.Flags = _settingsManager.Flags;
                scintilla.Folding.MarkerScheme = _settingsManager.MarkerScheme;

                string fontName = Fonts.ValidateFontName(_settingsManager.FontName);
                float fontSize = Fonts.ValidateFontSize(_settingsManager.FontSize);
                scintilla.Font = new Font(fontName, fontSize);
                scintilla.UseFont = true;
            }

            /*
             * Global settings (not covered by config files)
             */

            scintilla.IsBraceMatching = _settingsManager.MatchBraces;
            scintilla.LineWrap.Mode = _settingsManager.WordWrap
                ? WrapMode.Word : WrapMode.None;

            scintilla.Printing.PageSettings.ColorMode = 
                _settingsManager.PrintingColorMode;
        }

        private int GetColor(Color color)
        {
            return (color.R | (color.G << 8) | (color.B << 16));
        }

        #endregion
    }
}
