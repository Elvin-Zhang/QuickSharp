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
using System.IO;
using System.Text;
using System.Windows.Forms;
using QuickSharp.Core;
using QuickSharp.Editor;
using ICSharpCode.SharpZipLib.Zip;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.DocumentTemplates
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "3CBCE04D-B494-4379-AC9F-4FFC4B8E3CD4";
        }

        public string GetName()
        {
            return "QuickSharp Document Templates";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return
                "Provides support for default documents and user-defined " +
                "document templates.";
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
        private string _templateFolderPath;
        private string _defaultDocumentPath;
        private ToolStripMenuItem _newFromTemplate;

        private void ActivatePlugin()
        {
            ModuleProxy.GetInstance().Module = this;

            UserContent.DeployContent(Constants.PLUGIN_NAME);

            _applicationManager = ApplicationManager.GetInstance();

            /*
             * Determine status once all plugins have been loaded.
             */

            PluginManager.GetInstance().PluginPostActivate += 
                new MessageHandler(Module_PluginPostActivate);
        }

        private void Module_PluginPostActivate()
        {
            /*
             * Find the templates and create the menus from the items found.
             * Need to do this here in case other plugins contribute templates.
             */

            _documentManager = DocumentManager.GetInstance();

            _templateFolderPath = Path.Combine(
                _applicationManager.QuickSharpUserHome,
                Constants.TEMPLATE_FOLDER);

            if (!Directory.Exists(_templateFolderPath)) return;

            ToolStripMenuItem fileMenu = _mainForm.GetMenuItemByName(
                QuickSharp.Core.Constants.UI_FILE_MENU);

            if (fileMenu == null) return;

            int newMenuIndex = fileMenu.DropDownItems.IndexOfKey(
                QuickSharp.Core.Constants.UI_FILE_MENU_NEW);

            if (newMenuIndex == -1) return;

            _newFromTemplate = MenuTools.CreateMenuItem(
                Constants.UI_FILE_MENU_NEW_FROM_TEMPLATE,
                Resources.MainFileMenuNewFromTemplate,
                null, Keys.None, null, null);

            fileMenu.DropDownItems.Insert(
                newMenuIndex + 1, _newFromTemplate);

            CreateTemplateMenu(_templateFolderPath, _newFromTemplate);

            /*
             * Find the default document template.
             */

            foreach (string file in Directory.GetFiles(_templateFolderPath))
            {
                string name = Path.GetFileNameWithoutExtension(
                    Path.GetFileName(file));

                if (name == Constants.DEFAULT_DOCUMENT_NAME)
                {
                    _defaultDocumentPath = file;
                    break;
                }
            }

            /*
             * Creating files from templates relies on the open document
             * handlers for the template document types so we need to
             * disable templates if there are no handlers available.
             */

            if (_applicationManager.OpenDocumentHandlers.Count == 0)
            {
                _newFromTemplate.Enabled = false;
                return;
            }

            /*
             * If we have a default document claim the new document
             * type and handler.
             */

            if (_defaultDocumentPath != null)
            {
                _applicationManager.NewDocumentType =
                    new DocumentType(_defaultDocumentPath);
                
                _applicationManager.NewDocumentHandler =
                    TemplateNewDocumentHandler;
            }
        }

        #region Template Menu

        private void CreateTemplateMenu(
            string rootFolder, ToolStripMenuItem rootMenuItem)
        {
            foreach (string folder in Directory.GetDirectories(rootFolder))
            {
                try
                {
                    ToolStripMenuItem tmi = new ToolStripMenuItem();
                    tmi.Text = tmi.Name = Path.GetFileName(folder);
                    tmi.Image = Resources.Folder;
                    tmi.ImageTransparentColor = Color.Fuchsia;
                    rootMenuItem.DropDownItems.Add(tmi);

                    CreateTemplateMenu(folder, tmi);
                }
                catch
                {
                    // Ignore menu entry on error
                }
            }

            foreach (string file in Directory.GetFiles(rootFolder))
            {
                if (Path.GetFileName(file).StartsWith("_")) continue;

                try
                {
                    ToolStripMenuItem tmi = new ToolStripMenuItem();
                    tmi.Text = Path.GetFileNameWithoutExtension(file);
                    tmi.Name = file;
                    tmi.Click += UI_FILE_MENU_NEW_FROM_TEMPLATE_Click;
                    rootMenuItem.DropDownItems.Add(tmi);

                }
                catch
                {
                    // Ignore menu entry on error
                }
            }

            rootMenuItem.Enabled = (rootMenuItem.DropDownItems.Count > 0);
        }

        private void UI_FILE_MENU_NEW_FROM_TEMPLATE_Click(
            object sender, EventArgs e)
        {
            ToolStripMenuItem tmi = sender as ToolStripMenuItem;
            if (tmi == null) return;

            string fileType = 
                Path.GetExtension(tmi.Name).ToLower();

            if (fileType == Constants.ZIP_FILE_EXTENSION)
                ExtractFilesToWorkspace(tmi.Name);
            else
            {
                IDockContent doc = LoadFileFromTemplate(tmi.Name);

                if (doc != null)
                    doc.DockHandler.Show(
                        _mainForm.ClientWindow, DockState.Document);
            }
        }

        #endregion

        #region File Templates

        private IDockContent TemplateNewDocumentHandler()
        {
            return LoadFileFromTemplate(_defaultDocumentPath);
        }

        public IDockContent LoadFileFromTemplate(string templatePath)
        {
            IDockContent doc = _mainForm.LoadDocument(templatePath);

            if (doc != null)
            {
                Document document = doc as Document;

                if (document != null)
                {
                    document.FileName = 
                        _documentManager.GetNextUntitledFileName(
                            new DocumentType(templatePath));

                    // Force 'Save As'
                    document.FilePath = null;
                }
            }

            return doc;
        }

        public void ExtractFilesToWorkspace(string templatePath)
        {
            if (!File.Exists(templatePath))
            {
                MessageBox.Show(String.Format("{0}\r\n{1}",
                        Resources.TemplateNotFound, templatePath),
                    Resources.TemplateCreationError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            string workspace = Directory.GetCurrentDirectory();

            /*
             * We don't want to overwrite existing files when 
             * extracting zip archives. We warn the user if there
             * are files present in the workspace and if the template
             * zip doesn't start with the word "New ".
             * The convention here is that a template archive containing
             * files to be renamed will start with 'New'. If not
             * the files will be deployed as they are.
             */

            int fileCount = Directory.GetFiles(workspace, "*.*").Length;
            string fileName = Path.GetFileName(templatePath);

            bool projectHasUntitledFiles = fileName.StartsWith("New ");

            /*
             * If template doesn't have untitled files, template contents
             * will not be given uniques names and could therefore
             * overwrite files already present in the folder. Give a
             * warning if this could happen.
             */

            if (fileCount > 0 && !projectHasUntitledFiles)
            {
                if (MessageBox.Show(String.Format("{0}\r\n{1}\r\n{2}",
                        Resources.TemplateOverwriteWarningMessage1,
                        Resources.TemplateOverwriteWarningMessage2,
                        Resources.TemplateOverwriteWarningMessage3),
                    Resources.TemplateOverwriteWarningTitle,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.No)

                return;
            }

            string basename = _documentManager.GetNextUntitledFileBasename();

            /*
             * If we're going to be renaming untitled files get the
             * basename for the new file(s) from the user. We need to do
             * this before unzipping the file in case the user cancels.
             */

            if (projectHasUntitledFiles)
            {
                NewFilenameForm nff = new NewFilenameForm(
                    basename,
                    Path.GetFileNameWithoutExtension(fileName));

                if (nff.ShowDialog() != DialogResult.OK) return;

                basename = nff.Filename;

                /*
                 * basename may already exist but checks later on will prevent
                 * existing files from being overwritten. File will get next
                 * default name instead of name provided. Can't check to see if
                 * it exists at this stage as we don't know what the actual file
                 * names will be until we unzip the template.
                 */
            }

            /*
             * Extract the files.
             */

            FastZip fz = new FastZip();
            fz.CreateEmptyDirectories = true;
            fz.ExtractZip(templatePath, workspace, ".");
            
            if (projectHasUntitledFiles)
            {
                /*
                 * Rename any "__untitled__" files.
                 */

                string[] files = Directory.GetFiles(
                    workspace, Constants.UNTITLED_FILENAME_TEMPLATE + ".*");

                foreach (string file in files)
                {
                    string newname = file.Replace(
                        Constants.UNTITLED_FILENAME_TEMPLATE,
                        basename);

                    /*
                     * Make sure we don't already have this name.
                     */

                    while (File.Exists(newname))
                    {
                        basename =
                            _documentManager.GetNextUntitledFileBasename();

                        newname = file.Replace(
                            Constants.UNTITLED_FILENAME_TEMPLATE,
                            basename);
                    }

                    /*
                     * Update the filename.
                     */

                    try
                    {
                        FileTools.ChangeFileName(file, newname);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("{0}\r\n{1}",
                                Resources.TemplateFileRenameError,
                                ex.Message),
                            Resources.TemplateCreationError,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                    /*
                     * Update the content to include the new filename.
                     */

                    try
                    {
                        // Detect encoding from BOM, assume ANSI if none found
                        StreamReader sr = new StreamReader(newname, Encoding.ASCII, true);
                        String content = sr.ReadToEnd();
                        Encoding encoding = sr.CurrentEncoding;
                        sr.Close();

                        content = content.Replace(
                            Constants.UNTITLED_FILENAME_TEMPLATE, basename);

                        // Rewrite the file using the detected encoding
                        StreamWriter sw = new StreamWriter(newname, false, encoding);
                        sw.Write(content);
                        sw.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("{0}\r\n{1}",
                                Resources.TemplateContentUpdateError,
                                ex.Message),
                            Resources.TemplateCreationError,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }

            /*
             * Refresh any file system views.
             */

            _applicationManager.NotifyFileSystemChange();
        }

        #endregion
    }
}
