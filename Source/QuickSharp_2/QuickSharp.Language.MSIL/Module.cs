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
using QuickSharp.BuildTools;
using QuickSharp.Core;
using QuickSharp.Editor;
using QuickSharp.TextEditor;

namespace QuickSharp.Language.MSIL
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "0174B6A3-339B-4629-8F78-4181E1888231";
        }

        public string GetName()
        {
            return "QuickSharp MSIL Assembly Language Support";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return 
                "Provides support for Microsoft IL Assembly Language " +
                "and the .NET Framework IL Assembler (ilasm.exe).";
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

            _documentType = new DocumentType(Constants.DOCUMENT_TYPE_MSIL);

            DocumentManager.GetInstance().RegisterDocumentLanguage(
                _documentType, Constants.SCINTILLA_LANGUAGE_MSIL);

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
                Resources.MicrosoftMSILLineParser,
                _documentType,
                QuickSharp.BuildTools.Constants.ACTION_COMPILE,
                new MSILOutputLineParser());

            if (_buildToolManager.BuildTools.GetToolCount(
                _documentType) == 0)
                CreateDefaultTools();

            _buildToolManager.UpdateTools(_documentType);
            _buildToolManager.BuildTools.SortTools();
        }

        #region Default Tools

        private void CreateDefaultTools()
        {
            BuildTool ilasmD = CreateCompileTool(
                Resources.MicrosoftMSILAssemblerD, true);
            BuildTool ilasmR = CreateCompileTool(
                Resources.MicrosoftMSILAssemblerR, false);

            BuildTool windowsExe = new BuildTool(
                Guid.NewGuid().ToString(),
                _documentType, Resources.WindowsExe);

            windowsExe.Action = QuickSharp.BuildTools.Constants.ACTION_RUN;
            windowsExe.Path = "${OUT_PATH}";
            windowsExe.Args = "${RUNTIME_OPT}";
            windowsExe.UserArgs = String.Empty;
            windowsExe.LineParserName = String.Empty;

            _buildToolManager.BuildTools.AddTool(ilasmR);
            _buildToolManager.BuildTools.AddTool(ilasmD);
            _buildToolManager.BuildTools.AddTool(windowsExe);
            _buildToolManager.BuildTools.SelectTool(ilasmR);
            _buildToolManager.BuildTools.SelectTool(windowsExe);
        }

        private BuildTool CreateCompileTool(string name, bool debug)
        {
            BuildTool tool = new BuildTool(
                Guid.NewGuid().ToString(), _documentType, name);

            tool.Action = QuickSharp.BuildTools.Constants.ACTION_COMPILE;
            tool.Path = @"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\ilasm.exe";
            tool.Args = "${DOTNET_TARGET} ${COMMON_OPT} ${EMBEDDED_OPT} \"${SRC_FILE}\"";
            tool.UserArgs = debug ?
                "/nologo /quiet /debug":
                "/nologo /quiet";
            tool.LineParserName = Resources.MicrosoftMSILLineParser;

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

            buildCommand.StartText = String.Empty;
            buildCommand.FinishText = String.Empty;

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

            path = ExpandSpecificMacros(path, buildCommand.TargetType);

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
            Regex re = new Regex(@"(?m)^\s*\.entrypoint\s*$");
            Match m = re.Match(srcText);
            return m.Success;
        }

        private string ExpandSpecificMacros(string text, string targetType)
        {
            return text.Replace("${DOTNET_TARGET}",
               (targetType == ".dll") ? "/dll" : "/exe");
        }

        #endregion
    }
}
