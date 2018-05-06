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
using System.Text.RegularExpressions;
using QuickSharp.Core;
using QuickSharp.Editor;
using QuickSharp.TextEditor;
using QuickSharp.BuildTools;

namespace QuickSharp.Language.VBNet
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin members

        public string GetID()
        {
            return "DAE48B20-8DB1-4EFC-A1F2-2BA5895F4740";
        }

        public string GetName()
        {
            return "QuickSharp VB.NET Language Support";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return "Provides support for the VB.NET programming language.";
        }

        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            deps.Add(new Plugin(QuickSharpPlugins.Output, "QuickSharp.Output", 1));
            deps.Add(new Plugin(QuickSharpPlugins.BuildTools, "QuickSharp.BuildTools", 1));
            deps.Add(new Plugin(QuickSharpPlugins.Editor, "QuickSharp.Editor", 1));
            deps.Add(new Plugin(QuickSharpPlugins.TextEditor, "QuickSharp.TextEditor", 1));
            return deps;
        }

        public void Activate(MainForm mainForm)
        {
            _mainForm = mainForm;
            ActivatePlugin();
        }

        #endregion

        private BuildToolManager _buildToolManager;
        private MainForm _mainForm;
        private DocumentType _documentType;

        private void ActivatePlugin()
        {
            /*
             * Define the document handling.
             */

            _documentType = new DocumentType(Constants.DOCUMENT_TYPE_VB);

            DocumentManager.GetInstance().RegisterDocumentLanguage(
                _documentType, Constants.SCINTILLA_LANGUAGE_VB);

            ApplicationManager applicationManager =
                ApplicationManager.GetInstance();

            OpenDocumentHandler openHandler =
                applicationManager.GetOpenDocumentHandler(
                    QuickSharp.TextEditor.Constants.DOCUMENT_TYPE_TXT);

            if (openHandler != null)
                applicationManager.RegisterOpenDocumentHandler(
                    _documentType, openHandler);

            /*
             * Define the build tools.
             */

            _buildToolManager = BuildToolManager.GetInstance();

            _buildToolManager.RegisterBuildCommand(
                _documentType,
                QuickSharp.BuildTools.Constants.ACTION_COMPILE,
                CompileBuildCommand);

            _buildToolManager.RegisterBuildCommand(
                _documentType,
                QuickSharp.BuildTools.Constants.ACTION_RUN,
                RunBuildCommand);

            _buildToolManager.RegisterLineParser(
                Resources.MicrosoftVBLineParser,
                _documentType,
                QuickSharp.BuildTools.Constants.ACTION_COMPILE,
                new VBNetOutputLineParser());

            if (_buildToolManager.BuildTools.GetToolCount(
                _documentType) == 0)
                CreateDefaultTools();

            _buildToolManager.UpdateTools(_documentType);
            _buildToolManager.BuildTools.SortTools();
        }

        #region Default Tools

        private void CreateDefaultTools()
        {
            BuildTool vbcD = CreateCompileTool(
                Resources.MicrosoftVB9CompilerD, true);
            BuildTool vbcR = CreateCompileTool(
                Resources.MicrosoftVB9CompilerR, false);
            BuildTool exe = new BuildTool(
                Guid.NewGuid().ToString(), 
                _documentType, Resources.WindowsExe);

            exe.Action = QuickSharp.BuildTools.Constants.ACTION_RUN;
            exe.Path = "${OUT_PATH}";
            exe.Args = "${RUNTIME_OPT}";
            exe.UserArgs = String.Empty;
            exe.LineParserName = null;

            _buildToolManager.BuildTools.AddTool(vbcR);
            _buildToolManager.BuildTools.AddTool(vbcD);
            _buildToolManager.BuildTools.AddTool(exe);
            _buildToolManager.BuildTools.SelectTool(vbcR);
            _buildToolManager.BuildTools.SelectTool(exe);
        }

        private BuildTool CreateCompileTool(string name, bool debug)
        {
            BuildTool tool = new BuildTool(
                Guid.NewGuid().ToString(), _documentType, name);

            tool.Action = QuickSharp.BuildTools.Constants.ACTION_COMPILE;
            tool.Path = @"C:\WINDOWS\Microsoft.NET\Framework\v3.5\vbc.exe";
            tool.Args = "${DOTNET_TARGET} ${COMMON_OPT} ${EMBEDDED_OPT} \"${SRC_FILE}\"";
            tool.UserArgs = debug ?
                "/nologo /quiet /debug:pdbonly":
                "/nologo /quiet /optimize";
            tool.LineParserName = Resources.MicrosoftVBLineParser;

            return tool;
        }

        #endregion

        #region Build Commands

        private BuildCommand CompileBuildCommand(
            BuildTool buildTool, FileInfo srcInfo)
        {
            BuildCommand buildCommand = GetBuildCommand(
                buildTool, srcInfo, false);

            buildCommand.StartText =
                String.Format("{0}: ", Resources.CompileStarted);
            buildCommand.FinishText =
                String.Format("{0}: ", Resources.CompileComplete);

            return buildCommand;
        }

        private BuildCommand RunBuildCommand(
            BuildTool buildTool, FileInfo srcInfo)
        {
            BuildCommand buildCommand = GetBuildCommand(
                buildTool, srcInfo, true);

            buildCommand.StartText =
                String.Format("{0}: ", Resources.RunStarted);
            buildCommand.FinishText =
                String.Format("{0}: ", Resources.RunComplete);

            return buildCommand;
        }

        private BuildCommand GetBuildCommand(
            BuildTool buildTool, FileInfo srcInfo, bool exeRequired)
        {
            if (srcInfo == null) return null;

            string srcText = FileTools.ReadFile(srcInfo.FullName);
            if (srcText == null) return null;

            BuildCommand buildCommand = new BuildCommand();
            buildCommand.BuildTool = buildTool;
            buildCommand.SourceInfo = srcInfo;
            buildCommand.SourceText = srcText;

            buildCommand.TargetType = SourceHasMain(srcText) ? ".exe" : ".dll";

            string targetName = Path.ChangeExtension(
                buildCommand.SourceInfo.FullName,
                buildCommand.TargetType);

            buildCommand.TargetInfo = new FileInfo(targetName);

            string path = buildTool.Path;
            path = _buildToolManager.ExpandGenericMacros(
                path, buildTool,
                buildCommand.SourceText,
                buildCommand.SourceInfo,
                buildCommand.TargetInfo);

            if (exeRequired && buildCommand.TargetType != ".exe")
            {
                buildCommand.Cancel = true;
                buildCommand.CancelResult = false;
                return buildCommand;
            }

            string args = buildTool.Args;
            args = _buildToolManager.ExpandGenericMacros(
                args, buildTool,
                buildCommand.SourceText,
                buildCommand.SourceInfo,
                buildCommand.TargetInfo);

            args = ExpandSpecificMacros(args, buildCommand.TargetType);

            buildCommand.Path = path;
            buildCommand.Args = args;
            buildCommand.Cancel = false;
            buildCommand.SuccessCode = 0;

            return buildCommand;
        }

        #endregion

        #region Helpers

        private bool SourceHasMain(string srcText)
        {
            Regex re = new Regex(@"(?:Sub|Function)\s+Main\s*\(");
            Match m = re.Match(srcText);
            return m.Success;
        }

        private string ExpandSpecificMacros(string text, string targetType)
        {
            return text.Replace("${DOTNET_TARGET}",
               (targetType == ".dll") ? "/t:library" : "/t:exe");
        }

        #endregion
    }
}
