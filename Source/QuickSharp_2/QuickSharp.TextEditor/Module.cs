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

namespace QuickSharp.TextEditor
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "6D5F1958-A1E6-4312-A5EA-EA4F58135678";
        }

        public string GetName()
        {
            return "QuickSharp Text Editor";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            string txt =
                "Provides text editing services using " +
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
            _applicationManager = ApplicationManager.GetInstance();

            UserContent.DeployContent(Constants.PLUGIN_NAME);

            _documentManager = DocumentManager.GetInstance();

            /*
             * Register to handle .txt documents.
             */

            DocumentType txtDocumentType =
                new DocumentType(Constants.DOCUMENT_TYPE_TXT);

            _applicationManager.RegisterOpenDocumentHandler(
                txtDocumentType, OpenDocument);

            // Register .txt files with the default lexer/config
            _documentManager.RegisterDocumentLanguage(
                txtDocumentType,
                QuickSharp.Editor.Constants.DEFAULT_SCINTILLA_LEXER,
                Encoding.ASCII);

            /*
             * Become the default document handler.
             */

            _applicationManager.NewDocumentType = txtDocumentType;
            _applicationManager.NewDocumentHandler = NewDocument;

            /*
             * Claim the unknown document handler if enabled.
             */

            if (_applicationManager.ClientProfile.HaveFlag(
                ClientFlags.TextEditorClaimUnknownDocument))
                _applicationManager.UnknownDocumentType = txtDocumentType;

            /*
             * Claim the empty document type if enabled. Empty
             * document types have no file extension and have a
             * document type '.'. Claiming the empty document allows
             * files without extensions to be loaded by the text editor
             * without having to claim the unknown document handler.
             * Claiming the unknown document will have the same effect
             * but will also load any document type not recognised.
             */

            if (_applicationManager.ClientProfile.HaveFlag(
                ClientFlags.TextEditorClaimEmptyDocument))
            {
                _applicationManager.RegisterOpenDocumentHandler(
                    Constants.DOCUMENT_TYPE_EMPTY, OpenDocument);

                _documentManager.RegisterDocumentLanguage(
                    Constants.DOCUMENT_TYPE_EMPTY,
                    QuickSharp.Editor.Constants.DEFAULT_SCINTILLA_LEXER,
                    Encoding.ASCII);
            }

            /*
             * Add extra document types from the config file.
             * Check the user's directory first, then the 
             * installation directory.
             */

            if (!_applicationManager.ClientProfile.HaveFlag(
                    ClientFlags.TextEditorDisableDocumentTypeConfigFile))
            {
                String configPath = Path.Combine(
                    _applicationManager.QuickSharpUserHome,
                    QuickSharp.Editor.Constants.CONFIG_DIR_NAME);

                if (!Directory.Exists(configPath))
                    configPath = Path.Combine(
                        _applicationManager.QuickSharpHome,
                        QuickSharp.Editor.Constants.CONFIG_DIR_NAME);

                String configFile = Path.Combine(configPath,
                    Constants.DOCUMENT_TYPES_CONFIG_FILE);

                if (File.Exists(configFile))
                    ReadConfig(configFile);
            }
        }

        #region Document Creation

        public IDockContent NewDocument()
        {
            try
            {
                _mainForm.Refresh();
                _mainForm.Cursor = Cursors.WaitCursor;

                DocumentType documentType = _applicationManager.NewDocumentType;

                EditForm ef = new EditForm();
                ef.Text = _documentManager.GetNextUntitledFileName(documentType);
                ef.FileName = ef.Text;
                ef.FilePath = null;
                ef.FileEncoding = _documentManager.GetDocumentEncoding(documentType);
                ef.LineEnding = ScintillaNet.EndOfLineMode.Crlf;
                ef.Editor.EndOfLine.Mode = ef.LineEnding;
                ef.LoadSettings(documentType);
                ef.FileTimeStamp = DateTime.Now;

                return ef;
            }
            finally
            {
                _mainForm.Cursor = Cursors.Default;
            }
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

                EditForm ef = new EditForm();
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

        #region Document Types Config File

        private void ReadConfig(String configFile)
        {
            using (StreamReader sr = new StreamReader(configFile))
            {
                String line = null;

                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line == String.Empty) continue;
                    if (line.StartsWith("//")) continue;

                    String[] split = line.Split('=');
                    if (split.Length != 2) continue;

                    DocumentType documentType = new DocumentType(split[0]);

                    String language = split[1].Trim().ToLower();
                    if (language == String.Empty) continue;
                    
                    _documentManager.RegisterDocumentLanguage(
                        documentType, language);

                    _applicationManager.RegisterOpenDocumentHandler(
                        documentType, OpenDocument);
                }
            }
        }

        #endregion
    }
}
