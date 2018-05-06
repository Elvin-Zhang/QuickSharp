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
using System.Windows.Forms;
using QuickSharp.Core;
using QuickSharp.Editor;
using ScintillaNet;

namespace QuickSharp.CodeAssist
{
    /// <summary>
    /// The code assist plugin module.
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
            return "DAA6FA3B-5E18-491E-9A07-FFF3E6B8CF83";
        }

        /// <summary>
        /// Get the name of the plugin.
        /// </summary>
        /// <returns>The plugin name.</returns>
        public string GetName()
        {
            return "QuickSharp Code Assist";
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
                "Provides support for programming language 'code assist' services. " +
                "Additional plugins are required to provide support for specific " +
                "languages; this plugin provides the code assist infrastructure only.";

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
            deps.Add(new Plugin(QuickSharpPlugins.Editor, "QuickSharp.Editor", 1));
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

        private MainForm _mainForm;
        private ApplicationManager _applicationManager;
        private CodeAssistManager _codeAssistManager;
        private ToolStripMenuItem _codeAssist;

        private void ActivatePlugin()
        {
            ModuleProxy.GetInstance().Module = this;
 
            _applicationManager = ApplicationManager.GetInstance();
            _codeAssistManager = CodeAssistManager.GetInstance();

            ToolStripMenuItem editMenu = _mainForm.GetMenuItemByName(
                QuickSharp.Editor.Constants.UI_EDIT_MENU);

            if (editMenu == null) return;

            _codeAssist = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_CODE_ASSIST,
                Resources.MainEditMenuCodeAssist,
                null,
                Keys.Control | Keys.Space, null,
                UI_EDIT_MENU_CODE_ASSIST_Click);

            _codeAssist.Enabled = false;

            editMenu.DropDownItems.Add(_codeAssist);

            _mainForm.ClientWindow.ActiveDocumentChanged +=
                new EventHandler(_mainForm_ActiveDocumentChanged);
        }

        #region Invoke Code Assist

        /// <summary>
        /// Invoke code assist on the current document.
        /// </summary>
        /// <param name="document">The current document.</param>
        public void Invoke(ScintillaEditForm document)
        {
            if (document == null) return;
            
            LookupList lookupList = null;

            try
            {
                _mainForm.Cursor = Cursors.WaitCursor;
                lookupList = _codeAssistManager.GetLookupList(document);
            }
            catch
            {
                /*
                 * We want to ignore any exceptions that get this far
                 * for usability reasons. If the lookup fails we might
                 * as well just ignore it as there's nothing the user
                 * can do. We keep the command line option to allow
                 * debugging and testing.
                 */

                if (_applicationManager.HaveCommandLineSwitch(
                        Resources.SwitchDiagnostic)) throw;
            }
            finally
            {
                _mainForm.Cursor = Cursors.Default;
            }

            if (lookupList == null) return;

            if (lookupList.Items != null && lookupList.Items.Count > 0)
            {
                Point cursorLocation = GetCursorLocation(document);
                if (cursorLocation.IsEmpty) return;

                /*
                 * Make sure the caret isn't too far out of bounds.
                 */

                if (cursorLocation.Y < _mainForm.Location.Y) return;
                if (cursorLocation.Y > _mainForm.Location.Y + _mainForm.Height) return;

                /*
                 * Move the lookup display point if the caret is
                 * close to the lower edge of the form.
                 */

                int threshold = GetWindowBottomEdge() -
                    Constants.LOOKUP_WINDOW_HEIGHT;
                if (cursorLocation.Y > threshold)
                    cursorLocation.Y = threshold;

                LookupForm lf;

                try
                {
                    _mainForm.Cursor = Cursors.WaitCursor;
                     lf = new LookupForm(cursorLocation, lookupList);
                }
                finally
                {
                    _mainForm.Cursor = Cursors.Default;
                }

                if (lf.ShowDialog() == DialogResult.OK)
                {
                    string selectedText = lf.SelectedText;
                    string lookAheadText = lf.LookAheadText;

                    if (!String.IsNullOrEmpty(lookAheadText))
                    {
                        document.Editor.Caret.Position -=
                            lookAheadText.Length;
                        document.Editor.Selection.Start =
                            document.Editor.Caret.Position;
                        document.Editor.Selection.End =
                            document.Editor.Caret.Position + lookAheadText.Length;
                        document.Editor.Selection.Text = String.Empty;
                    }

                    if (lf.InsertionTemplate == null)
                    {
                        document.Editor.InsertText(selectedText);
                    }
                    else
                    {
                        string template = lf.InsertionTemplate;
                        
                        string insertionText = template.Replace(
                            Constants.INSERTION_TEMPLATE_TEXT_PLACEHOLDER,
                            selectedText);

                        int cursorOffset = insertionText.IndexOf(
                            Constants.INSERTION_TEMPLATE_CPOS_PLACEHOLDER);

                        insertionText = insertionText.Replace(
                            Constants.INSERTION_TEMPLATE_CPOS_PLACEHOLDER,
                            String.Empty);
 
                        document.Editor.InsertText(insertionText);

                        /*
                         * If offset is -1 the caret stays at the
                         * end of the inserted text.
                         */

                        if (cursorOffset != -1)
                        {
                            document.Editor.Caret.Position -=
                                (insertionText.Length - cursorOffset);

                            document.Editor.Selection.Start =
                                document.Editor.Selection.End =
                                    document.Editor.Caret.Position;
                        }
                    }
                }

                lf.Dispose();
            }
        }

        #endregion

        #region Update Status

        /// <summary>
        /// Update the code assist UI status. Determines if code assist should
        /// be made available to the current document and calls the DocumentActivated
        /// method of the code assist provider.
        /// </summary>
        public void UpdateCodeAssistStatus()
        {
            _codeAssist.Enabled = false;

            QuickSharp.Core.Document document =
                _mainForm.ActiveDocument;

            if (document == null ||
                !(document is ScintillaEditForm))
                return;

            DocumentType documentType =
                new DocumentType(document.FileName);

            if (_codeAssistManager.CodeAssistAvailable(documentType))
            {
                _codeAssist.Enabled = true;
                ICodeAssistProvider provider =
                    _codeAssistManager.GetProvider(documentType);

                if (provider != null)
                    provider.DocumentActivated(document as ScintillaEditForm);
            }
        }

        #endregion

        #region Event Handlers

        private void UI_EDIT_MENU_CODE_ASSIST_Click(
            object sender, EventArgs e)
        {
            ScintillaEditForm document =
                _mainForm.ActiveDocument as ScintillaEditForm;

            Invoke(document);
        }

        private void _mainForm_ActiveDocumentChanged(
            object sender, EventArgs e)
        {
            UpdateCodeAssistStatus();
        }

        #endregion

        #region Helpers

        private Point GetCursorLocation(ScintillaEditForm document)
        {
            if (document == null) return Point.Empty;

            Line line = document.Editor.Lines.Current;
            
            int borderWidth =
                (_mainForm.Width - _mainForm.ClientSize.Width) / 2;

            int titlebarHeight = _mainForm.Height - 
                _mainForm.ClientSize.Height - (2 * borderWidth);

            Point p = new Point(
                _mainForm.Location.X +
                document.Location.X +
                document.Editor.Location.X +
                document.Editor.PointXFromPosition(line.SelectionStartPosition),
                _mainForm.Location.Y +
                titlebarHeight +
                _mainForm.MainMenu.Height +
                _mainForm.ClientWindow.Location.Y +
                document.Location.Y +
                document.Editor.Location.Y +
                document.Editor.PointYFromPosition(line.SelectionStartPosition));

            return p;
        }

        private int GetWindowBottomEdge()
        {
            int borderWidth =
                (_mainForm.Width - _mainForm.ClientSize.Width) / 2;
            
            int titlebarHeight = _mainForm.Height - 
                _mainForm.ClientSize.Height - (2 * borderWidth);

            int Y =
                _mainForm.Location.Y +
                titlebarHeight +
                _mainForm.MainMenu.Height +
                _mainForm.ClientWindow.Height;

            return Y;
        }

        #endregion
    }
}
