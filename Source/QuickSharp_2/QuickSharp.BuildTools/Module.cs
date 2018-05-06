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
using System.Drawing;
using System.Windows.Forms;
using QuickSharp.Core;
using QuickSharp.Output;

namespace QuickSharp.BuildTools
{
    /// <summary>
    /// The plugin main module.
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
            return "708F1B1A-7A3C-4A2A-9E33-46D2CAB4AEFC";
        }

        /// <summary>
        /// Get the name of the plugin.
        /// </summary>
        /// <returns>The plugin name.</returns>
        public string GetName()
        {
            return "QuickSharp Build Tools Support";
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
            return
                "Provides support for programming language build tools such " +
                "as compilers and interpreters. Additional plugins are required " +
                "to provide support for specific languages and tools; this plugin " +
                "provides the build tool infrastructure only.";
        }

        /// <summary>
        /// Get the plugin's dependencies. This provides a list of the
        /// plugins required by the current plugin.
        /// </summary>
        /// <returns>The plugin dependencies,</returns>
        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            deps.Add(new Plugin(QuickSharpPlugins.Output, "QuickSharp.Output", 1));
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

        /**********************************************************************
         * BUILD TOOLS
         * 
         * Provides the default Compile and Run tool menu and toolbar entries
         * but also provides an extensible tools framework via the 
         * BuildToolManager. This plugin provides the infrastructure for
         * languages that support compile and run actions (such as compilers
         * and scripting languages) but doesn't include support for specific
         * tools and languages. Additional plugins are required to provide
         * details of actual tools.
         **********************************************************************/

        private ApplicationManager _applicationManager;
        private ApplicationStorage _applicationStorage;
        private MainForm _mainForm;
        private OutputForm _output;
        private ToolStripMenuItem _toolsMenuCompile;
        private ToolStripMenuItem _toolsMenuCompileAll;
        private ToolStripMenuItem _toolsMenuRun;
        private ToolStripButton _toolbarCompile;
        private ToolStripButton _toolbarRun;
        private ToolStripDropDownButton _toolbarCompileSelect;
        private ToolStripDropDownButton _toolbarRunSelect;
        private ToolStripButton _toolbarPin;
        private ToolStripDropDownButton _toolbarPinSelect;
        private BuildToolManager _buildToolManager;

        private void ActivatePlugin()
        {
            ModuleProxy.GetInstance().Module = this;

            _applicationManager = ApplicationManager.GetInstance();
            _applicationStorage = ApplicationStorage.GetInstance();

            _applicationManager.RegisterOptionsPageFactory(
                delegate { return new BuildToolOptionsPage(); });

            if (!_applicationManager.ClientProfile.HaveFlag(
                ClientFlags.BuildToolsDisableBuildSettingsPage))
            {
                _applicationManager.RegisterOptionsPageFactory(
                    delegate { return new BuildSettingsOptionsPage(); });
            }

            _applicationManager.DocumentFilterHandlers.Add(
                GetSourceFilesFilter);

            #region UI Elements

            bool disableCompilerUI = _applicationManager.ClientProfile.
                HaveFlag(ClientFlags.BuildToolsDisableCompilerUI);

            _output = _applicationManager.GetDockedForm(
                QuickSharp.Output.Constants.DOCKED_FORM_KEY)
                as OutputForm;

            /* Menu items */

            _toolsMenuCompile = MenuTools.CreateMenuItem(
                Constants.UI_TOOLS_MENU_COMPILE,
                Resources.MainToolsMenuCompile,
                Resources.Compile,
                Keys.Shift | Keys.F5, null, UI_TOOLS_MENU_COMPILE_Click);

            _toolsMenuCompileAll = MenuTools.CreateMenuItem(
                Constants.UI_TOOLS_MENU_COMPILE_ALL,
                Resources.MainToolsMenuCompileAll,
                null,
                Keys.F6, null, UI_TOOLS_MENU_COMPILE_ALL_Click,
                true);

            _toolsMenuRun = MenuTools.CreateMenuItem(
                Constants.UI_TOOLS_MENU_RUN,
                Resources.MainToolsMenuRun,
                Resources.Run,
                Keys.Control | Keys.F5, null, UI_TOOLS_MENU_RUN_Click);

            if (disableCompilerUI)
                _toolsMenuRun.Tag = new ToolStripItemTag { IncludeSeparator = true };

            ToolStripMenuItem toolsMenu = _mainForm.GetMenuItemByName(
                QuickSharp.Core.Constants.UI_TOOLS_MENU);

            if (!disableCompilerUI)
            {
                toolsMenu.DropDownItems.Insert(0, _toolsMenuCompileAll);
                toolsMenu.DropDownItems.Insert(0, _toolsMenuCompile);
            }

            toolsMenu.DropDownItems.Insert(0, _toolsMenuRun);
            toolsMenu.DropDownOpening +=
                new EventHandler(ToolsMenu_DropDownOpening);

            /* Toolbar */

            _toolbarCompile = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_COMPILE,
                Resources.ToolbarCompile,
                Resources.Compile, UI_TOOLBAR_COMPILE_Click,
                true);
            _toolbarCompile.Enabled = false;
            _toolbarCompileSelect = MenuTools.CreateToolbarDropDownButton(
                Constants.UI_TOOLBAR_COMPILE_SELECT,
                UI_TOOLBAR_COMPILE_SELECT_Click);
            _toolbarCompileSelect.Enabled = false;
            _toolbarCompileSelect.DropDownOpening +=
                new EventHandler(ToolbarCompileSelect_DropDownOpening);

            _toolbarRun = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_RUN,
                Resources.ToolbarRun,
                Resources.Run, UI_TOOLBAR_RUN_Click);

            if (disableCompilerUI)
                _toolbarRun.Tag = new ToolStripItemTag { IncludeSeparator = true };

            _toolbarRun.Enabled = false;
            _toolbarRunSelect = MenuTools.CreateToolbarDropDownButton(
                Constants.UI_TOOLBAR_RUN_SELECT,
                UI_TOOLBAR_RUN_SELECT_Click);
            _toolbarRunSelect.Enabled = false;
            _toolbarRunSelect.DropDownOpening +=
                new EventHandler(ToolbarRunSelect_DropDownOpening);

            _toolbarPin = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_PIN,
                Resources.ToolbarPinFile,
                Resources.Pin, UI_TOOLBAR_PIN_Click,
                true);
            _toolbarPin.Enabled = false;
            _toolbarPinSelect = MenuTools.CreateToolbarDropDownButton(
                Constants.UI_TOOLBAR_PIN_SELECT,
                UI_TOOLBAR_PIN_SELECT_Click);
            _toolbarPinSelect.Enabled = false;

            if (!disableCompilerUI)
            {
                _mainForm.MainToolbar.Items.Add(_toolbarCompile);
                _mainForm.MainToolbar.Items.Add(_toolbarCompileSelect);
            }

            _mainForm.MainToolbar.Items.Add(_toolbarRun);
            _mainForm.MainToolbar.Items.Add(_toolbarRunSelect);
            _mainForm.MainToolbar.Items.Add(_toolbarPin);
            _mainForm.MainToolbar.Items.Add(_toolbarPinSelect);

            #endregion

            _mainForm.ClientWindow.ActiveDocumentChanged += 
                new EventHandler(MainForm_ActiveDocumentChanged);

            _buildToolManager = BuildToolManager.GetInstance();
            _buildToolManager.LoadSettings();
            _buildToolManager.LoadTools();
            _buildToolManager.BuildTools.SortTools();
            _buildToolManager.LoadPinnedFiles();

            _mainForm.FormClosed +=
                new FormClosedEventHandler(MainForm_FormClosed);

            _applicationManager.FileSystemChange +=
                new MessageHandler(UpdatePinFileStatus);

            PluginManager.GetInstance().PluginPostActivate +=
                new MessageHandler(PostActivationHandler);
        }

        private void PostActivationHandler()
        {
            /*
             * Create a list of the registered build tool source
             * types and store it in the application store. This
             * allows other plugins, particularly those with no
             * direct dependency on the BuildTools plugin, to access
             * a list of registered tool source types.
             */

            List<String> toolSourceTypes = new List<String>();

            toolSourceTypes.AddRange(
                _buildToolManager.BuildTools.GetDocumentTypes());

            _applicationStorage[
                Constants.APP_STORE_KEY_TOOL_SOURCE_TYPES]
                = toolSourceTypes;

            /*
             * Update the file list once we have a full list of
             * source types (wait until theme has been applied in
             * case we need to use a theme color).
             */

            _mainForm.ThemeActivated += delegate { UpdatePinFileStatus(); };
        }

        private void MainForm_FormClosed(object sender, EventArgs e)
        {
            try
            {
                _mainForm.Cursor = Cursors.WaitCursor;
                _buildToolManager.SaveSettings();
                _buildToolManager.SaveTools();
                _buildToolManager.SavePinnedFiles();
            }
            finally
            {
                _mainForm.Cursor = Cursors.Default;
            }
        }

        #region Actions

        private void UI_TOOLS_MENU_COMPILE_Click(object sender, EventArgs e)
        {
            _mainForm.Refresh();
            CompileActiveDocument();
        }

        private void UI_TOOLS_MENU_COMPILE_ALL_Click(object sender, EventArgs e)
        {
            _mainForm.Refresh();
            CompileActiveDocument(true);
        }

        private void UI_TOOLS_MENU_RUN_Click(object sender, EventArgs e)
        {
            _mainForm.Refresh();
            RunActiveDocument();
        }

        private void UI_TOOLBAR_COMPILE_Click(object sender, EventArgs e)
        {
            CompileActiveDocument();
        }

        private void UI_TOOLBAR_RUN_Click(object sender, EventArgs e)
        {
            RunActiveDocument();
        }

        private void UI_TOOLBAR_PIN_Click(object sender, EventArgs e)
        {
            PinnedFile pinnedFile =
                _buildToolManager.PinnedFiles.GetPinnedFile(
                    Directory.GetCurrentDirectory());

            if (pinnedFile != null)
                pinnedFile.Selected = !pinnedFile.Selected;

            UpdatePinFileStatus();
        }

        private void UI_TOOLBAR_COMPILE_SELECT_Click(
            object sender, ToolStripItemClickedEventArgs e)
        {
            SelectToolFromDropDownList(e);
        }

        private void UI_TOOLBAR_RUN_SELECT_Click(
            object sender, ToolStripItemClickedEventArgs e)
        {
            SelectToolFromDropDownList(e);
        }
        
        private void UI_TOOLBAR_PIN_SELECT_Click(
            object sender, ToolStripItemClickedEventArgs e)
        {
            SelectFileFromDropDownList(e);
        }

        #endregion

        #region Action State

        private void MainForm_ActiveDocumentChanged(
            object sender, EventArgs e)
        {
            UpdateBuildToolStatus();
        }

        private void ToolsMenu_DropDownOpening(
            object sender, EventArgs e)
        {
            UpdateBuildToolStatus();
        }
        
        private void ToolbarCompileSelect_DropDownOpening(
            object sender, EventArgs e)
        {
            UpdateToolDropDownList(
                _toolbarCompileSelect, Constants.ACTION_COMPILE);
        }

        private void ToolbarRunSelect_DropDownOpening(
            object sender, EventArgs e)
        {
            UpdateToolDropDownList(
                _toolbarRunSelect, Constants.ACTION_RUN);
        }

        #endregion

        #region Tool Selection

        /// <summary>
        /// Get the type of the pinned or active document.
        /// </summary>
        /// <returns>A document type.</returns>
        public DocumentType GetPinnedOrActiveDocumentType()
        {
            PinnedFile pinnedFile =
                _buildToolManager.PinnedFiles.GetPinnedFile(
                    Directory.GetCurrentDirectory());
            
            if (pinnedFile != null && pinnedFile.Selected)
                return pinnedFile.DocumentType;
            else
                return GetActiveDocumentType();
        }

        private void UpdateToolDropDownList(
            ToolStripDropDownButton button, string action)
        {
            button.DropDownItems.Clear();

            DocumentType documentType = GetPinnedOrActiveDocumentType();
            if (documentType == null) return;

            Dictionary<String, BuildTool> tools = 
                _buildToolManager.BuildTools.GetTools(
                    documentType, action);

            foreach (string id in tools.Keys)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = tools[id].DisplayName;
                item.Tag = id;
                item.Checked = 
                    _buildToolManager.BuildTools.ToolIsSelected(tools[id]);
                item.Enabled = tools[id].BuildCommand != null;

                // Apply theme color if available.
                ThemeFlags flags = _applicationManager.ClientProfile.ThemeFlags;
                if (flags != null && flags.MenuForeColor != Color.Empty)
                    item.ForeColor = flags.MenuForeColor;

                button.DropDownItems.Add(item);
            }
        }

        private void SelectToolFromDropDownList(
            ToolStripItemClickedEventArgs e)
        {
            ToolStripMenuItem item = e.ClickedItem as ToolStripMenuItem;
            if (item == null) return;

            string id = item.Tag as string;
            if (id == null) return;

            _buildToolManager.BuildTools.SelectTool(id);
            
            UpdateBuildToolStatus();
        }

        /// <summary>
        /// Update the UI representaion of the build tool status.
        /// </summary>
        public void UpdateBuildToolStatus()
        {
            _toolsMenuRun.Enabled = false;
            _toolsMenuCompile.Enabled = false;
            _toolsMenuCompileAll.Enabled = false;
            _toolbarCompile.Enabled = false;
            _toolbarCompileSelect.Enabled = false;
            _toolbarRun.Enabled = false;
            _toolbarRunSelect.Enabled = false;

            DocumentType documentType = GetPinnedOrActiveDocumentType(); 
            if (documentType == null) return;

            if (_buildToolManager.BuildTools.ToolsAreAvailable(
                documentType, Constants.ACTION_COMPILE))
                _toolbarCompileSelect.Enabled = true;

            if (_buildToolManager.BuildTools.SelectedToolIsAvailable(
                documentType, Constants.ACTION_COMPILE))
            {
                _toolsMenuCompile.Enabled = true;
                _toolsMenuCompileAll.Enabled = true;
                _toolbarCompile.Enabled = true;
            }

            if (_buildToolManager.BuildTools.ToolsAreAvailable(
                documentType, Constants.ACTION_RUN))
                _toolbarRunSelect.Enabled = true;

            if (_buildToolManager.BuildTools.SelectedToolIsAvailable(
                documentType, Constants.ACTION_RUN))
            {
                _toolsMenuRun.Enabled = true;
                _toolbarRun.Enabled = true;
            }
        }

        #endregion

        #region Pin-file Selection

        private void UpdatePinFileStatus()
        {
            string currentDirectory = 
                Directory.GetCurrentDirectory();
            
            PinnedFile pinnedFile = 
                _buildToolManager.PinnedFiles.
                    GetPinnedFile(currentDirectory);

            /*
             * Is it still valid?
             */

            if (pinnedFile != null && !pinnedFile.Exists())
            {
                _buildToolManager.PinnedFiles.Remove(pinnedFile);
                pinnedFile = null;
            }

            /*
             * PinnedFile is now null or a valid file path.
             * Reset the UI to base state.
             */

            _toolbarPin.Enabled = false;
            _toolbarPin.Checked = false;
            _toolbarPinSelect.Enabled = false;
            _toolbarPinSelect.DropDownItems.Clear();

            /*
             * Now refresh the file list.
             */

            List<String> documentTypes =
                _buildToolManager.BuildTools.GetDocumentTypes();

            string[] files = Directory.GetFiles(currentDirectory);

            ThemeFlags flags = _applicationManager.
                ClientProfile.ThemeFlags;

            bool applyTheme = flags != null &&
                flags.MenuForeColor != Color.Empty;

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);

                if (!documentTypes.Contains(fi.Extension))
                    continue;

                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = Path.GetFileName(file);
                item.Tag = file;

                // Apply theme color if available.
                if (applyTheme) item.ForeColor = flags.MenuForeColor;

                if (pinnedFile != null)
                    item.Checked = pinnedFile.Matches(file);

                _toolbarPinSelect.DropDownItems.Add(item);
            }

            /*
             * Update the UI based on what we found.
             */

            if (_toolbarPinSelect.DropDownItems.Count > 0)
                _toolbarPinSelect.Enabled = true;

            if (pinnedFile != null)
            {
                _toolbarPin.Enabled = true;
                _toolbarPin.Checked = pinnedFile.Selected;

                _toolbarPin.ToolTipText = String.Format("{0}: {1}",
                    Resources.ToolbarPinFile,
                    pinnedFile.Name);
            }
            else
                _toolbarPin.ToolTipText = Resources.ToolbarPinFile;

            UpdateBuildToolStatus();
        }

        private void SelectFileFromDropDownList(
            ToolStripItemClickedEventArgs e)
        {
            ToolStripMenuItem item = e.ClickedItem as ToolStripMenuItem;
            if (item == null) return;

            string file = item.Tag as string;
            if (file == null) return;

            _buildToolManager.PinnedFiles.Add(file);

            UpdatePinFileStatus();
        }

        #endregion

        #region Compile

        /// <summary>
        /// Compile the active document and any out of date dependencies.
        /// </summary>
        /// <returns>True if the action completed successfully.</returns>
        public bool CompileActiveDocument()
        {
            return CompileActiveDocument(_buildToolManager.AllwaysBuildOnCompile);
        }

        /// <summary>
        /// Compile the active document and its dependencies. Dependencies may be compiled
        /// conditionally if out of date or unconditionally.
        /// </summary>
        /// <param name="compileAll">Compile dependencies unconditionally if true.</param>
        /// <returns>True if the action completed successfully.</returns>
        public bool CompileActiveDocument(bool compileAll)
        {
            if (!_mainForm.SaveAllDocuments())
            {
                _mainForm.SetStatusBarMessage(Resources.BuildHalted);
                return false;
            }

            PinnedFile pinnedFile =
                _buildToolManager.PinnedFiles.GetPinnedFile(
                    Directory.GetCurrentDirectory());

            if (pinnedFile != null && pinnedFile.Selected)
                return Compile(
                    pinnedFile.FileInfo,
                    FileTools.ReadFile(pinnedFile.Path),
                    compileAll);
            else
                return Compile(
                    GetActiveDocumentInfo(),
                    GetActiveDocumentContent(),
                    compileAll);
        }

        /// <summary>
        /// Compile the file provided and its dependencies. Dependencies may be compiled
        /// conditionally if out of date or unconditionally.
        /// </summary>
        /// <param name="srcInfo">The source file info.</param>
        /// <param name="srcText">The source file text.</param>
        /// <param name="compileAll">Compile dependencies unconditionally if true.</param>
        /// <returns>True if the action completed successfully.</returns>
        public bool Compile(FileInfo srcInfo, string srcText, bool compileAll)
        {
            if (_output == null) return false;
            _output.ClearOutputViews();
            _mainForm.SetStatusBarMessage(Resources.BuildStarted);
            _mainForm.StatusBar.Refresh();

            if (srcInfo == null || String.IsNullOrEmpty(srcText))
            {
                _output.AddLineToOutputView(String.Format(
                    "------ {0}: {1}",
                    Resources.BuildErrors,
                    Resources.ErrorSourceInvalid));

                _output.AdjustOutputWidth();
                _mainForm.SetStatusBarMessage(Resources.BuildErrors);
                return false;
            }

            if (!CompileDependencies(srcText, compileAll)) return false;

            BuildCommand compileCommand =
                GetBuildCommand(srcInfo, Constants.ACTION_COMPILE);

            return CompileFile(compileCommand);
        }

        private bool CompileDependencies(string srcText, bool compileAll)
        {
            List<String> deps = _buildToolManager.GetDependencies(srcText);

            foreach (string dep in deps)
            {
                /*
                 * Check the filename is valid and actually exists.
                 */

                FileInfo depInfo = null;

                try
                {
                    depInfo = new FileInfo(dep);
                }
                catch
                {
                    _output.AddLineToOutputView(String.Format(
                        "------ {0}: {1}: {2}",
                        Resources.BuildErrors,
                        Resources.ErrorFilenameInvalid, dep));

                    _output.AdjustOutputWidth();
                    _mainForm.SetStatusBarMessage(Resources.BuildErrors);
                    return false;
                }

                if (!File.Exists(depInfo.FullName))
                {
                    _output.AddLineToOutputView(String.Format(
                        "------ {0}: {1}: {2}",
                        Resources.BuildErrors,
                        Resources.ErrorFileNotFound,
                        depInfo.FullName));

                    _output.AdjustOutputWidth();
                    _mainForm.SetStatusBarMessage(Resources.BuildErrors);
                    return false;
                }

                /*
                 * Compile the file.
                 */

                BuildCommand compileCommand = 
                    GetBuildCommand(depInfo, Constants.ACTION_COMPILE);

                if (compileCommand == null)
                {
                    _output.AddLineToOutputView(String.Format(
                        "------ {0}: {1} {2}",
                        Resources.BuildErrors,
                        Resources.ErrorBuildToolNotFound,
                        depInfo.Name));

                    _output.AdjustOutputWidth();
                    _mainForm.SetStatusBarMessage(Resources.BuildErrors);
                    return false;
                }

                if (compileCommand.TargetBuildRequired() || compileAll)
                    if (!CompileFile(compileCommand))
                        return false;
            }

            return true;
        }

        private bool CompileFile(BuildCommand compileCommand)
        {
            if (_output == null) return false;

            if (compileCommand == null)
            {
                _output.AddLineToOutputView(String.Format(
                    "------ {0}: {1}",
                    Resources.BuildErrors,
                    Resources.ErrorBuildToolInvalid));

                _output.AdjustOutputWidth();
                _mainForm.SetStatusBarMessage(Resources.BuildErrors);
                return false;
            }

            if (Directory.GetCurrentDirectory() != 
                compileCommand.SourceInfo.DirectoryName)
            {
                Directory.SetCurrentDirectory(
                    compileCommand.SourceInfo.DirectoryName);

                _applicationManager.NotifyFileSystemChange();
            }

            RunProcessContext context = new RunProcessContext();
            context.ExePath = compileCommand.Path;
            context.ProcessArgs = compileCommand.Args;
            context.HeaderText = compileCommand.StartText;
            context.FooterText = compileCommand.FinishText;
            context.LineParser = compileCommand.BuildTool.LineParser;
            context.ExitCode = 0;

            Dictionary<String, List<String>> actionCommands =
                _buildToolManager.GetActionCommands(compileCommand.SourceText);

            _output.Text = String.Format("{0} {1}",
                Resources.OutputWindowCompile,
                compileCommand.BuildTool.DisplayName);

            bool res = RunShellCommands(
                String.Format("{0} ", Resources.PreCompileTask),
                actionCommands[Constants.ACTION_CMD_DO_PRE_COMPILE]);

            _applicationManager.NotifyFileSystemChange();

            if (res) res = _output.RunProcessInternal(context);

            if (context.ExitCode != compileCommand.SuccessCode) res = false;

            if (res) res = RunShellCommands(
                String.Format("{0} ", Resources.PostCompileTask),
                actionCommands[Constants.ACTION_CMD_DO_POST_COMPILE]);

            _applicationManager.NotifyFileSystemChange();

            if (res)
                _mainForm.SetStatusBarMessage(Resources.BuildSuccess);
            else
                _mainForm.SetStatusBarMessage(Resources.BuildErrors);

            return res;
        }

        #endregion

        #region Run

        /// <summary>
        /// Run the active document's target (output) file. Any out of date
        /// source files and their dependencies will be recompiled.
        /// </summary>
        /// <returns>True if the action completed successfully.</returns>
        public bool RunActiveDocument()
        {
            return RunActiveDocument(_buildToolManager.AllwaysBuildOnRun);
        }

        /// <summary>
        /// Run the active document's target (output) file. Source files and their dependencies
        /// will be recompiled conditionally if out of date or unconditionally.
        /// </summary>
        /// <param name="compileAll">Compile dependencies unconditionally if true.</param>
        /// <returns>True if the action completed successfully.</returns>
        public bool RunActiveDocument(bool compileAll)
        {
            if (!_mainForm.SaveAllDocuments())
            {
                _mainForm.SetStatusBarMessage(Resources.BuildHalted);
                return false;
            }

            PinnedFile pinnedFile =
                _buildToolManager.PinnedFiles.GetPinnedFile(
                    Directory.GetCurrentDirectory());

            if (pinnedFile != null && pinnedFile.Selected)
                return Run(
                    pinnedFile.FileInfo,
                    FileTools.ReadFile(pinnedFile.Path),
                    compileAll);
            else
                return Run(
                    GetActiveDocumentInfo(),
                    GetActiveDocumentContent(),
                    compileAll);
        }

        /// <summary>
        /// Run a file. Source files and their dependencies
        /// will be recompiled if out of date.
        /// </summary>
        /// <param name="srcInfo">The source file info.</param>
        /// <param name="srcText">The source file text.</param>
        /// <returns>True if the action completed successfully.</returns>
        public bool Run(FileInfo srcInfo, string srcText)
        {
            return Run(srcInfo, srcText, false);
        }

        /// <summary>
        /// Run a file. Source files and their dependencies
        /// will be recompiled conditionally if out of date or unconditionally.
        /// </summary>
        /// <param name="srcInfo">The source file info.</param>
        /// <param name="srcText">The source file text.</param>
        /// <param name="compileAll">Compile dependencies unconditionally if true.</param>
        /// <returns>True if the action completed successfully.</returns>
        public bool Run(FileInfo srcInfo, string srcText, bool compileAll)
        {
            if (_output == null) return false;
            _output.ClearOutputViews();
            _mainForm.SetStatusBarMessage(Resources.BuildStarted);
            _mainForm.StatusBar.Refresh();

            if (srcInfo == null || String.IsNullOrEmpty(srcText))
            {
                _output.AddLineToOutputView(String.Format(
                    "------ {0}: {1}",
                    Resources.BuildErrors,
                    Resources.ErrorSourceInvalid));

                _output.AdjustOutputWidth();
                _mainForm.SetStatusBarMessage(Resources.BuildErrors);
                return false;
            }

            if (!CompileDependencies(srcText, compileAll)) return false;

            BuildCommand runCommand =
                GetBuildCommand(srcInfo, Constants.ACTION_RUN);

            if (runCommand.TargetBuildRequired())
            {
                BuildCommand compileCommand =
                    GetBuildCommand(srcInfo, Constants.ACTION_COMPILE);

                if (!CompileFile(compileCommand))
                    return false;
            }

            return RunFile(runCommand);
        }

        private bool RunFile(BuildCommand runCommand)
        {
            if (_output == null) return false;

            if (runCommand == null)
            {
                _output.AddLineToOutputView(String.Format(
                    "------ {0}: {1}",
                    Resources.BuildErrors,
                    Resources.ErrorBuildToolInvalid));

                _output.AdjustOutputWidth();
                _mainForm.SetStatusBarMessage(Resources.BuildErrors);
                return false;
            }

            if (Directory.GetCurrentDirectory() !=
                runCommand.SourceInfo.DirectoryName)
            {
                Directory.SetCurrentDirectory(
                    runCommand.SourceInfo.DirectoryName);

                _applicationManager.NotifyFileSystemChange();
            }

            if (runCommand.Cancel)
            {
                _output.AddLineToOutputView(
                    String.Format(
                        Resources.ProgramTarget,
                        runCommand.TargetInfo.FullName));

                _output.AddLineToOutputView(
                    String.Format(
                        Resources.ProgramNotExecutable,
                        runCommand.TargetInfo.Extension));

                _output.AdjustOutputWidth();
                _mainForm.SetStatusBarMessage(Resources.BuildHalted);
                return runCommand.CancelResult;
            }

            RunProcessContext context = new RunProcessContext();
            context.ExePath = runCommand.Path;
            context.ProcessArgs = runCommand.Args;
            context.HeaderText = runCommand.StartText;
            context.FooterText = runCommand.FinishText;
            context.LineParser = runCommand.BuildTool.LineParser;
            context.ExitCode = 0;

            Dictionary<String, List<String>> actionCommands =
                _buildToolManager.GetActionCommands(runCommand.SourceText);

            _output.Text = String.Format("{0} {1}",
                Resources.OutputWindowRun,
                runCommand.BuildTool.DisplayName);

            bool useWindow =
                (actionCommands[Constants.ACTION_CMD_RUN_IN_OWN_WINDOW].Count > 0);

            bool res = RunShellCommands(
                String.Format("{0} ", Resources.PreRunTask),
                actionCommands[Constants.ACTION_CMD_DO_PRE_RUN]);

            _applicationManager.NotifyFileSystemChange();

            if (res)
            {
                if (useWindow)
                    res = _output.RunProcessExternal(context);
                else
                    res = _output.RunProcessInternal(context);
            }

            if (res) res = RunShellCommands(
                String.Format("{0} ", Resources.PostRunTask),
                actionCommands[Constants.ACTION_CMD_DO_POST_RUN]);

            _applicationManager.NotifyFileSystemChange();

            _mainForm.SetStatusBarMessage(
                String.Format(Resources.ProgramReturn, context.ExitCode));

            return res;
        }

        #endregion

        #region Helpers

        private BuildCommand GetBuildCommand(
            FileInfo srcInfo, string action)
        {
            if (srcInfo == null) return null;

            BuildTool buildTool =
                _buildToolManager.BuildTools.GetSelectedTool(
                    srcInfo.Extension, action);

            if (buildTool == null || buildTool.BuildCommand == null)
                return null;

            BuildCommand buildCommand =
                buildTool.BuildCommand(buildTool, srcInfo);

            return buildCommand;
        }

        private bool RunShellCommands(
            string caption, List<String> commands)
        {
            bool res = true;

            foreach (string cmd in commands)
            {
                res = _output.RunShellCommandInternal(caption, cmd, false);
                if (!res) break;
            }

            return res;
        }

        private FileInfo GetDocumentInfo(Document document)
        {
            if (document == null) return null;
            if (document.FilePath == null) return null;
            return new FileInfo(document.FilePath);
        }

        private FileInfo GetActiveDocumentInfo()
        {
            if (_mainForm.ActiveDocument == null) return null;
            if (_mainForm.ActiveDocument.FilePath == null) return null;
            return new FileInfo(_mainForm.ActiveDocument.FilePath);
        }

        private DocumentType GetActiveDocumentType()
        {
            if (_mainForm.ActiveDocument == null) return null;
            FileInfo fi = new FileInfo(_mainForm.ActiveDocument.FileName);
            return new DocumentType(fi.Extension);
        }

        private string GetActiveDocumentContent()
        {
            if (_mainForm.ActiveDocument == null) return null;
            return _mainForm.ActiveDocument.GetContent() as String;
        }

        #endregion

        #region Source Files Filter

        /// <summary>
        /// Get a list of registered source file filters.
        /// </summary>
        /// <returns>A list of source file types formatted as a FileOpenDialog filter.</returns>
        public string GetSourceFilesFilter()
        {
            List<String> documentTypes =
                _buildToolManager.BuildTools.GetDocumentTypes();

            List<String> nonToolSourceTypes = _applicationStorage[
                Constants.APP_STORE_KEY_NON_TOOL_SOURCE_TYPES]
                as List<String>;

            if (nonToolSourceTypes != null)
                documentTypes.AddRange(nonToolSourceTypes);

            documentTypes.Sort();

            StringBuilder sb = new StringBuilder();
            
            for (int i = 0; i < documentTypes.Count; i++)
            {
                sb.Append("*");
                sb.Append(documentTypes[i]);
                if (i < documentTypes.Count - 1)
                    sb.Append(";");
            }

            return string.Format(
                "Source Files ({0})|{0}|",
                sb.ToString());
        }

        #endregion
    }
}
