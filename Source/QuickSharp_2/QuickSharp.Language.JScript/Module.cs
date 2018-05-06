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

namespace QuickSharp.Language.JScript
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin members

        public string GetID()
        {
            return "9ED467F4-7B46-468E-8F57-F22FC8BBEF24";
        }

        public string GetName()
        {
            return "QuickSharp JScript.NET Language Support";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return "Provides support for the JScript.NET programming language.";
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

            _documentType = new DocumentType(Constants.DOCUMENT_TYPE_JS);

            DocumentManager.GetInstance().RegisterDocumentLanguage(
                _documentType, Constants.SCINTILLA_LANGUAGE_JS);

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
                Resources.MicrosoftJSLineParser,
                _documentType,
                QuickSharp.BuildTools.Constants.ACTION_COMPILE,
                new JSOutputLineParser());

            if (_buildToolManager.BuildTools.GetToolCount(
                _documentType) == 0)
                CreateDefaultTools();

            _buildToolManager.UpdateTools(_documentType);
            _buildToolManager.BuildTools.SortTools();
        }

        #region Default Tools

        private void CreateDefaultTools()
        {
            BuildTool jscR = CreateCompileTool(
                Resources.MicrosoftJS8CompilerR, false);
            BuildTool jscD = CreateCompileTool(
                Resources.MicrosoftJS8CompilerD, true);
            BuildTool exe = new BuildTool(
                Guid.NewGuid().ToString(),
                _documentType, Resources.WindowsExe);

            exe.Action = QuickSharp.BuildTools.Constants.ACTION_RUN;
            exe.Path = "${OUT_PATH}";
            exe.Args = "${RUNTIME_OPT}";
            exe.UserArgs = String.Empty;
            exe.LineParserName = null;

            _buildToolManager.BuildTools.AddTool(jscR);
            _buildToolManager.BuildTools.AddTool(jscD);
            _buildToolManager.BuildTools.AddTool(exe);
            _buildToolManager.BuildTools.SelectTool(jscR);
            _buildToolManager.BuildTools.SelectTool(exe);
        }

        private BuildTool CreateCompileTool(string name, bool debug)
        {
            BuildTool tool = new BuildTool(
                Guid.NewGuid().ToString(), _documentType, name);

            tool.Action = QuickSharp.BuildTools.Constants.ACTION_COMPILE;
            tool.Path = @"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\jsc.exe";
            tool.Args = "${COMMON_OPT} ${EMBEDDED_OPT} \"${SRC_FILE}\"";
            tool.UserArgs = debug ?
                "/nologo /autoref /debug /define:DEBUG;TRACE":
                "/nologo /autoref";

            tool.LineParserName = Resources.MicrosoftJSLineParser;

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

            buildCommand.TargetType = TargetIsLibrary(srcText) ? ".dll" : ".exe";

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

            buildCommand.Path = path;
            buildCommand.Args = args;
            buildCommand.Cancel = false;
            buildCommand.SuccessCode = 0;

            return buildCommand;
        }

        #endregion

        #region Helpers

        private bool TargetIsLibrary(string source)
        {
            Regex re = new Regex(@"//\$\s*/t:library");
            Match m = re.Match(source.ToLower());
            return m.Success;
        }

        #endregion
    }
}
