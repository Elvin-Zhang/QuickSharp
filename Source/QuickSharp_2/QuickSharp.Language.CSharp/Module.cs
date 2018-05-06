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
using System.Text.RegularExpressions;
using Microsoft.Win32;
using QuickSharp.BuildTools;
using QuickSharp.Core;
using QuickSharp.Editor;
using QuickSharp.TextEditor;

namespace QuickSharp.Language.CSharp
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "63122714-75A7-48E8-92EF-518FF78B0CA6";
        }

        public string GetName()
        {
            return "QuickSharp C# Language Support";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return "Provides support for the C# programming language and Microsoft C# compilers.";
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

            _documentType = new DocumentType(Constants.DOCUMENT_TYPE_CS);

            DocumentManager.GetInstance().RegisterDocumentLanguage(
                _documentType, Constants.SCINTILLA_LANGUAGE_CS);

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
                Resources.MicrosoftCSLineParser,
                _documentType, 
                QuickSharp.BuildTools.Constants.ACTION_COMPILE,
                new CSharpOutputLineParser());

            /*
             * Create the default build tool instances if there are none.
             */

            if (_buildToolManager.BuildTools.GetToolCount(_documentType) == 0)
                CreateDefaultTools();

            /*
             * Update the tools collection.
             */

            _buildToolManager.UpdateTools(_documentType);
            _buildToolManager.BuildTools.SortTools();
        }

        #region Default tools

        private void CreateDefaultTools()
        {
            BuildTool cs2compilerR = CreateMicrosoftCompiler(
                Resources.MicrosoftCS2CompilerR, false, false);
            BuildTool cs2compilerD = CreateMicrosoftCompiler(
                Resources.MicrosoftCS2CompilerD, true, false);
            BuildTool cs3compilerR = CreateMicrosoftCompiler(
                Resources.MicrosoftCS3CompilerR, false, true);
            BuildTool cs3compilerD = CreateMicrosoftCompiler(
                Resources.MicrosoftCS3CompilerD, true, true);

            _buildToolManager.BuildTools.AddTool(cs2compilerR);
            _buildToolManager.BuildTools.AddTool(cs2compilerD);
            _buildToolManager.BuildTools.AddTool(cs3compilerR);
            _buildToolManager.BuildTools.AddTool(cs3compilerD);
            _buildToolManager.BuildTools.SelectTool(cs3compilerR);

            BuildTool windowsExe = new BuildTool(
                Guid.NewGuid().ToString(),
                _documentType, Resources.WindowsExe);

            windowsExe.Action = QuickSharp.BuildTools.Constants.ACTION_RUN;
            windowsExe.Path = "${OUT_PATH}";
            windowsExe.Args = "${RUNTIME_OPT}";
            windowsExe.UserArgs = String.Empty;
            windowsExe.LineParserName = String.Empty;

            _buildToolManager.BuildTools.AddTool(windowsExe);
            _buildToolManager.BuildTools.SelectTool(windowsExe);
        }

        private BuildTool CreateMicrosoftCompiler(string name, bool debug, bool cs3)
        {
            BuildTool compiler = new BuildTool(
                Guid.NewGuid().ToString(), _documentType, name);

            compiler.Action = QuickSharp.BuildTools.Constants.ACTION_COMPILE;
            compiler.Path = cs3 ?
                @"C:\WINDOWS\Microsoft.NET\Framework\v3.5\csc.exe":
                @"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\csc.exe";
            compiler.Args = "${DOTNET_TARGET} ${COMMON_OPT} ${EMBEDDED_OPT} \"${SRC_FILE}\"";
            compiler.UserArgs = debug ?
                "/nologo /debug:pdbonly /define:DEBUG;TRACE":
                "/nologo";
            compiler.LineParserName = Resources.MicrosoftCSLineParser;

            return compiler;
        }

        #endregion

        #region BuildCommands

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
            srcText = RemoveComments(srcText);

            Regex re = new Regex(@"static\s+(int|Int32|void)\s+Main\s*\(");
            Match m = re.Match(srcText);
            return m.Success;
        }

        private string ExpandSpecificMacros(string text, string targetType)
        {
            return text.Replace("${DOTNET_TARGET}",
               (targetType == ".dll") ? "/t:library" : "/t:exe");
        }

        private string RemoveComments(string text)
        {
            text = text.Replace("\r\n", "\n");
            text = text.Replace("\\\\", String.Empty);

            StringBuilder sb = new StringBuilder();
            int max = text.Length - 1;
            int i = 0;

            while (i < text.Length)
            {
                if (text[i] == '/' && i < max && text[i + 1] == '/')
                {
                    // Goto line end
                    while (i < max && text[i] != '\n') i++;
                }
                else if (text[i] == '/' && i < max && text[i + 1] == '*')
                {
                    // Goto next '*/'
                    while (i < max)
                    {
                        if (text[i] == '*' && text[i + 1] == '/') break;
                        i++;
                    }
                    i += 2;
                }

                if (i <= max) sb.Append(text[i]);
                i++;
            }

            return sb.ToString();
        }

        #endregion
    }
}
