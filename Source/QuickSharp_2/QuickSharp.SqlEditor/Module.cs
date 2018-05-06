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
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using QuickSharp.Core;
using QuickSharp.Editor;

namespace QuickSharp.SqlEditor
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "79F10E28-3ACF-440F-811D-C8E1FFAD9127";
        }

        public string GetName()
        {
            return "QuickSharp SQL Editor";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            string txt =
                "Provides SQL text editing services using " +
                "Scintilla by Neil Hodgson and ScintillaNET " +
                "by Garrett Serack and the ScintillaNET team. " +
                "For more information visit the project websites at " +
                "http://www.scintilla.org " +
                "and http://www.codeplex.com/ScintillaNET.";

            return txt;
        }

        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            deps.Add(new Plugin(QuickSharpPlugins.Editor, "QuickSharp.Editor", 1));
            return deps;
        }

        public void Activate(MainForm mainForm)
        {
            _mainForm = mainForm;
            ActivatePlugin();
        }

        #endregion

        private MainForm _mainForm;
        private ApplicationManager _applicationManager;
        private DocumentManager _documentManager;

        private void ActivatePlugin()
        {
            UserContent.DeployContent(Constants.PLUGIN_NAME);

            _applicationManager = ApplicationManager.GetInstance();

            _documentManager = DocumentManager.GetInstance();

            /*
             * Register to handle .sql documents.
             */

            DocumentType documentType =
                new DocumentType(Constants.DOCUMENT_TYPE_SQL);

            _applicationManager.RegisterOpenDocumentHandler(
                documentType, OpenDocument);

            _documentManager.RegisterDocumentLanguage(
                documentType,
                Constants.SCINTILLA_LANGUAGE_SQL,
                Encoding.ASCII);
        }

        #region Document Creation

        public bool NewDocument()
        {
            try
            {
                _mainForm.Refresh();
                _mainForm.Cursor = Cursors.WaitCursor;

                DocumentType documentType = _applicationManager.NewDocumentType;

                SqlEditForm ef = new SqlEditForm();
                ef.Text = _documentManager.GetNextUntitledFileName(documentType);
                ef.FileName = ef.Text;
                ef.FilePath = null;
                ef.FileEncoding = _documentManager.GetDocumentEncoding(documentType);
                ef.LineEnding = ScintillaNet.EndOfLineMode.Crlf;
                ef.Editor.EndOfLine.Mode = ef.LineEnding;
                ef.LoadSettings(documentType);
                ef.FileTimeStamp = DateTime.Now;
                ef.Show(_mainForm.ClientWindow, DockState.Document);
            }
            finally
            {
                _mainForm.Cursor = Cursors.Default;
            }

            return true;
        }

        public IDockContent OpenDocument(string path, bool readOnly)
        {
            /*
             * Activate the file if already in an editor.
             */

            foreach (Document d in _mainForm.ClientWindow.Documents)
            {
                if (d.FilePath != null &&
                    FileTools.MatchPaths(d.FilePath, path) &&
                    !d.AllowDuplicates())
                {
                    d.Activate();
                    return null;
                }
            }

            /*
             * Create a new editor and load the file.
             */

            try
            {
                _mainForm.Refresh();
                _mainForm.Cursor = Cursors.WaitCursor;

                string fileText = null;
                Encoding encoding;

                try
                {
                    fileText = FileTools.ReadFile(path, out encoding);
                    if (fileText == null) return null;
                }
                catch (Exception ex)
                {
                    _mainForm.Cursor = Cursors.Default;

                    MessageBox.Show(String.Format("{0}\r\n{1}",
                        Resources.OpenErrorMessage,
                        ex.Message),
                        Resources.OpenErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                    return null;
                }

                /*
                 * Populate the form.
                 */

                SqlEditForm ef = new SqlEditForm();
                FileInfo fi = new FileInfo(path);
                ef.Text = fi.Name;
                ef.FileName = fi.Name;
                ef.FilePath = fi.FullName;
                ef.FileEncoding = encoding;
                ef.LineEnding = _documentManager.DetectLineEnding(fileText);
                ef.Editor.EndOfLine.Mode = ef.LineEnding;
                ef.Editor.Text = fileText;
                ef.Editor.Modified = false;
                ef.Editor.UndoRedo.EmptyUndoBuffer();
                ef.Editor.IsReadOnly = readOnly;
                ef.LoadSettings(new DocumentType(fi.Extension));
                ef.FileTimeStamp = DateTime.Now;

                ef.UpdateTab();

                return ef;
            }
            finally
            {
                _mainForm.Cursor = Cursors.Default;
            }
        }

        #endregion
    }
}
