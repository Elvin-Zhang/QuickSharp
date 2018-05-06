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
using System.Windows.Forms;
using QuickSharp.Core;
using QuickSharp.Editor;
using QuickSharp.BuildTools;
using ScintillaNet;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.SDKTools
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "CE2FF3B1-9F9F-4087-8645-C43B497A4111";
        }

        public string GetName()
        {
            return "QuickSharp .NET Framework SDK Tools Support";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return
                "Provides access to tools and documentation from the Microsoft " +
                ".NET Framework SDK. By default this plugin uses tools from " +
                "the .NET 2.0 SDK; update the paths if you wish to use a later " +
                "version.";
        }

        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            deps.Add(new Plugin("72F1F89B-F7D7-425B-B617-9E0B70597475", "QuickSharp.Editor", 1));
            deps.Add(new Plugin("708F1B1A-7A3C-4A2A-9E33-46D2CAB4AEFC", "QuickSharp.BuildTools", 1));
            return deps;
        }

        public void Activate(MainForm mainForm)
        {
            _mainForm = mainForm;
            ActivatePlugin();
        }

        #endregion

        private MainForm _mainForm;
        private BuildToolManager _buildToolManager;
        private SettingsManager _settingsManager;
        private ToolStripMenuItem _toolMenuIldasm;
        private ToolStripMenuItem _toolMenuClrDbg;
        private ToolStripMenuItem _helpMenuHelpContents;
        private ToolStripMenuItem _helpMenuContextHelp;

        private void ActivatePlugin()
        {
            _buildToolManager = BuildToolManager.GetInstance();
            _settingsManager = SettingsManager.GetInstance();

            ApplicationManager _applicationManager = 
                ApplicationManager.GetInstance();

            _applicationManager.RegisterOpenDocumentHandler(
                Constants.DOCUMENT_TYPE_EXE, OpenIldasmFile);

            _applicationManager.RegisterOpenDocumentHandler(
                Constants.DOCUMENT_TYPE_DLL, OpenIldasmFile);

            _applicationManager.RegisterOptionsPageFactory(
                delegate { return new SDKToolsOptionsPage(); });

            _mainForm.ClientWindow.ActiveDocumentChanged += 
                new EventHandler(ClientWindow_ActiveDocumentChanged);

            #region Menu Items

            ToolStripMenuItem toolMenu = _mainForm.GetMenuItemByName(
                QuickSharp.Core.Constants.UI_TOOLS_MENU);

            int index = toolMenu.DropDownItems.IndexOfKey(
                QuickSharp.Core.Constants.UI_TOOLS_MENU_OPTIONS);

            /*
             * If the options menu isn't found insert at the top.
             */

            if (index == -1) index = 0;

            _toolMenuIldasm = MenuTools.CreateMenuItem(
                Constants.UI_TOOLS_MENU_ILDASM,
                Resources.MainToolsMenuIldasm,
                null, Keys.None, null, UI_TOOLS_MENU_ILDASM_Click);

            _toolMenuClrDbg = MenuTools.CreateMenuItem(
                Constants.UI_TOOLS_MENU_CLRDBG,
                Resources.MainToolsMenuClrDbg,
                Resources.Debug, Keys.Control | Keys.F11,
                null, UI_TOOLS_MENU_CLRDBG_Click,
                true);

            toolMenu.DropDownItems.Insert(index, _toolMenuClrDbg);
            toolMenu.DropDownItems.Insert(index, _toolMenuIldasm);
            toolMenu.DropDownOpening +=
                new EventHandler(toolMenu_DropDownOpening);

            ToolStripMenuItem helpMenu = _mainForm.GetMenuItemByName(
                QuickSharp.Core.Constants.UI_HELP_MENU);

            _helpMenuHelpContents = MenuTools.CreateMenuItem(
                Constants.UI_HELP_MENU_HELP_CONTENTS,
                Resources.MainHelpMenuHelpContents,
                Resources.Help,
                Keys.Control | Keys.F1,
                null, UI_HELP_MENU_HELP_CONTENTS_Click);

            _helpMenuContextHelp = MenuTools.CreateMenuItem(
                Constants.UI_HELP_MENU_CONTEXT_HELP,
                Resources.MainHelpMenuContextHelp,
                null, Keys.F1,
                null, UI_HELP_MENU_CONTEXT_HELP_Click, true);

            helpMenu.DropDownItems.Insert(0, _helpMenuContextHelp);
            helpMenu.DropDownItems.Insert(0, _helpMenuHelpContents);
            helpMenu.DropDownOpening +=
                new EventHandler(helpMenu_DropDownOpening);

            #endregion

            UpdateToolMenuStatus();
            UpdateHelpMenuStatus();
        }

        #region Open Document Handlers

        private IDockContent OpenIldasmFile(string path, bool readOnly)
        {
            /*
             * This doesn't create documents for the DockPanel
             * so the return will always be null.
             */

            if (File.Exists(_settingsManager.IldasmPath))
                FileTools.LaunchApplication(false,
                    _settingsManager.IldasmPath,
                    String.Format("\"{0}\"", path));
            
            return null;
        }

        #endregion

        #region Menu Event Handlers

        private void UI_TOOLS_MENU_ILDASM_Click(
            object sender, EventArgs e)
        {
            if (!File.Exists(_settingsManager.IldasmPath))
                return;

            FileTools.LaunchApplication(false,
                _settingsManager.IldasmPath,
                GetDocumentTarget(true));
        }

        private void UI_TOOLS_MENU_CLRDBG_Click(
            object sender, EventArgs e)
        {
            if (!File.Exists(_settingsManager.ClrDbgPath))
                return;

            FileTools.LaunchApplication(false,
                _settingsManager.ClrDbgPath,
                GetDocumentTarget(false));
        }

        private void UI_HELP_MENU_HELP_CONTENTS_Click(object sender, EventArgs e)
        {
            if (!File.Exists(_settingsManager.DexplorePath))
                return;

            FileTools.LaunchApplication(false,
                _settingsManager.DexplorePath,
                _settingsManager.DexploreArgs +
                    " /LaunchNamedUrlTopic DefaultPage");
        }

        private void UI_HELP_MENU_CONTEXT_HELP_Click(object sender, EventArgs e)
        {
            if (!File.Exists(_settingsManager.DexplorePath))
                return;

            string keyword = String.Empty;

            if (_mainForm.ActiveDocument != null)
            {
                ScintillaEditForm ef =
                    _mainForm.ActiveDocument as ScintillaEditForm;

                if (ef != null)
                {
                    string currentword =
                        ef.Editor.GetWordFromPosition(ef.Editor.CurrentPos);

                    if (!String.IsNullOrEmpty(currentword))
                        keyword = " /LaunchFKeywordTopic " + currentword;
                }
            }

            FileTools.LaunchApplication(false,
                _settingsManager.DexplorePath,
                _settingsManager.DexploreArgs + keyword);
        }

        private void ClientWindow_ActiveDocumentChanged(
            object sender, EventArgs e)
        {
            UpdateToolMenuStatus();
            UpdateHelpMenuStatus();
        }

        private void toolMenu_DropDownOpening(
            object sender, EventArgs e)
        {
            UpdateToolMenuStatus();
        }

        private void helpMenu_DropDownOpening(
            object sender, EventArgs e)
        {
            UpdateHelpMenuStatus();
        }

        private void UpdateToolMenuStatus()
        {
            _toolMenuIldasm.Enabled = 
                File.Exists(_settingsManager.IldasmPath);

            _toolMenuClrDbg.Enabled =
                File.Exists(_settingsManager.ClrDbgPath);
        }
        
        private void UpdateHelpMenuStatus()
        {
            _helpMenuHelpContents.Enabled =
                File.Exists(_settingsManager.DexplorePath);

            _helpMenuContextHelp.Enabled = false;
            if (_mainForm.ActiveDocument == null) return;

            ScintillaEditForm ef = _mainForm.ActiveDocument as ScintillaEditForm;
            if (ef == null) return;

            _helpMenuContextHelp.Enabled =
                File.Exists(_settingsManager.DexplorePath);
        }

        #endregion

        #region Tool Helpers

        private string GetDocumentTarget(bool allowLib)
        {
            string targetPath = null;

            FileInfo srcInfo = GetDocumentInfo();

            if (srcInfo != null)
            {
                BuildCommand buildCommand = GetCompileCommand(srcInfo);

                if (buildCommand != null &&
                    buildCommand.TargetInfo != null)
                {
                    FileInfo targetInfo = buildCommand.TargetInfo;

                    if (targetInfo.Extension.ToLower() == ".exe" ||
                        (targetInfo.Extension.ToLower() == ".dll" && allowLib))
                    {
                        targetPath = buildCommand.TargetInfo.FullName;

                        if (!File.Exists(targetPath))
                            targetPath = null;

                        if (!String.IsNullOrEmpty(targetPath))
                            targetPath = String.Format("\"{0}\"", targetPath);
                    }
                }
            }

            return targetPath;
        }

        private FileInfo GetDocumentInfo()
        {
            string filePath = null;

            PinnedFile pinnedFile =
                _buildToolManager.PinnedFiles.GetPinnedFile(
                    Directory.GetCurrentDirectory());

            if (pinnedFile != null && pinnedFile.Selected)
            {
                filePath = pinnedFile.Path;
            }
            else
            {
                if (_mainForm.ActiveDocument != null &&
                    _mainForm.ActiveDocument.FilePath != null)
                    filePath = _mainForm.ActiveDocument.FilePath;
            }

            if (filePath == null)
                return null;
            else
                return new FileInfo(filePath);
        }

        private BuildCommand GetCompileCommand(FileInfo srcInfo)
        {
            if (srcInfo == null) return null;

            BuildTool buildTool =
                _buildToolManager.BuildTools.GetSelectedTool(
                    srcInfo.Extension,
                    QuickSharp.BuildTools.Constants.ACTION_COMPILE);

            if (buildTool == null || buildTool.BuildCommand == null)
                return null;

            BuildCommand buildCommand =
                buildTool.BuildCommand(buildTool, srcInfo);

            return buildCommand;
        }

        #endregion
    }
}
