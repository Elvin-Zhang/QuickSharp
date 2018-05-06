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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using QuickSharp.Core;
using QuickSharp.Output;

namespace QuickSharp.BuildTools
{
    /// <summary>
    /// Provides global access to the registered build tools.
    /// Uses a singleton; use the GetInstance() method to access.
    /// </summary>
    public class BuildToolManager
    {
        #region Singleton

        private static BuildToolManager _singleton;

        /// <summary>
        /// Provides access to the BuildToolManager singleton.
        /// </summary>
        /// <returns>A reference to the BuildToolManager.</returns>
        public static BuildToolManager GetInstance()
        {
            if (_singleton == null)
                _singleton = new BuildToolManager();

            return _singleton;
        }

        #endregion

        private BuildToolCollection _buildTools;
        private PinnedFilesCollection _pinnedFiles;
        private ApplicationManager _applicationManager;
        private IPersistenceManager _persistenceManager;
        private List<BuildCommandDelegateListItem> _buildCommandHandlers;
        private Dictionary<String, OutputLineParserListItem> _lineParsers;

        public bool AllwaysBuildOnCompile { get; set; }
        public bool AllwaysBuildOnRun { get; set; }

        private BuildToolManager()
        {
            _buildTools = new BuildToolCollection();

            _pinnedFiles = new PinnedFilesCollection();

            _applicationManager = ApplicationManager.GetInstance();
            
            _persistenceManager = _applicationManager.
                GetPersistenceManager(Constants.PLUGIN_NAME);

            _buildCommandHandlers =
                new List<BuildCommandDelegateListItem>();
            
            _lineParsers =
                new Dictionary<String, OutputLineParserListItem>();
        }

        /// <summary>
        /// The build tools registered with the build tool system.
        /// </summary>
        public BuildToolCollection BuildTools
        {
            get { return _buildTools; }
            set { _buildTools = value; }
        }

        /// <summary>
        /// The pinned files currently recognised by the build tool system.
        /// </summary>
        public PinnedFilesCollection PinnedFiles
        {
            get { return _pinnedFiles; }
        }

        /// <summary>
        /// Update the UI representations of the build tools status.
        /// </summary>
        public void UpdateBuildToolStatus()
        {
            ModuleProxy moduleProxy = ModuleProxy.GetInstance();

            if (moduleProxy.Module != null)
                moduleProxy.Module.UpdateBuildToolStatus();
        }

        #region Build Commands

        /// <summary>
        /// Register a build command provider with the build tool system. A
        /// language support plugin defines a provider method to create a
        /// build command; the build command contains a concrete invocation
        /// of a build tool for execution by the output window. The provider
        /// is represented by a BuildCommandDelegate which is associated with
        /// a document type and build action.
        /// </summary>
        /// <param name="documentType">A document type.</param>
        /// <param name="action">A build action (e.g. compile or run).</param>
        /// <param name="commandDelegate">A build command provider.</param>
        public void RegisterBuildCommand(
            DocumentType documentType,
            string action,
            BuildCommandDelegate commandDelegate)
        {
            BuildCommandDelegateListItem listItem =
                new BuildCommandDelegateListItem();
            
            listItem.DocumentType = documentType;
            listItem.Action = action;
            listItem.BuildCommand = commandDelegate;

            _buildCommandHandlers.Add(listItem);
        }

        /// <summary>
        /// Get the currently active build command provider for a
        /// document type and build action combination. Returns
        /// null if nothing is available.
        /// </summary>
        /// <param name="documentType">A document type.</param>
        /// <param name="action">A build action (e.g. compile or run).</param>
        /// <returns>A build command provider.</returns>
        public BuildCommandDelegate GetBuildCommand(
            DocumentType documentType, string action)
        {
            foreach (BuildCommandDelegateListItem i in _buildCommandHandlers)
                if (i.DocumentType.Matches(documentType) &&
                    i.Action == action)
                    return i.BuildCommand;

            return null;
        }

        /// <summary>
        /// Finalize the build tools registered by a plugin. This must be called
        /// whenever additional tools are added to the build tool system and is required
        /// to populate the tool definitions with additional data not stored in the
        /// saved definitions (e.g. line parser instances).
        /// </summary>
        /// <param name="documentType">The document type of the tools being registered.</param>
        public void UpdateTools(DocumentType documentType)
        {
            Dictionary<String, BuildTool> tools =
                _buildTools.GetTools(documentType);

            foreach (string id in tools.Keys)
            {
                BuildTool tool = tools[id];
                
                tool.BuildCommand = GetBuildCommand(
                    tool.DocumentType, tool.Action);
                
                if (!String.IsNullOrEmpty(tool.LineParserName))
                    tool.LineParser = GetLineParser(tool.LineParserName);
            }
        }

        #endregion

        #region Line Parsers

        /// <summary>
        /// Register an output line parser with the build tool system. Output line
        /// parsers are used to extract information from the output of build tools
        /// such as compilers. They provide file and line number information for
        /// displaying error messages and locating error lines in the text editor.
        /// </summary>
        /// <param name="displayName">The parser's display name.</param>
        /// <param name="documentType">A document type.</param>
        /// <param name="action">A build action.</param>
        /// <param name="parser">the parser.</param>
        public void RegisterLineParser(
            string displayName, DocumentType documentType, 
            string action, IOutputLineParser parser)
        {
            OutputLineParserListItem p = new OutputLineParserListItem();
            p.DisplayName = displayName;
            p.DocumentType = documentType;
            p.Action = action;
            p.Parser = parser;

            _lineParsers[p.DisplayName] = p;
        }

        /// <summary>
        /// Get a line parser from its display name.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <returns>The parser instance.</returns>
        public IOutputLineParser GetLineParser(string displayName)
        {
            if (displayName == null) return null;

            if (_lineParsers.ContainsKey(displayName))
                return _lineParsers[displayName].Parser;
            else
                return null;
        }

        /// <summary>
        /// Get the display names of the line parsers registered for a document
        /// type/build action combination. Used to present the parsers for
        /// selection in the build tool configuration form.
        /// </summary>
        /// <param name="documentType">A document type.</param>
        /// <param name="action">A build action.</param>
        /// <returns>A list of parser display names.</returns>
        public List<String> GetLineParsers(
            DocumentType documentType, string action)
        {
            List<String> names = new List<String>();

            foreach (string key in _lineParsers.Keys)
                if (_lineParsers[key].DocumentType.Matches(documentType) &&
                    _lineParsers[key].Action == action)
                    names.Add(_lineParsers[key].DisplayName);

            names.Sort();

            return names;
        }

        #endregion

        #region Actions and DocumentTypes

        /// <summary>
        /// Get a list of the document types for which build command
        /// providers have been registered.
        /// </summary>
        /// <returns>A list of document types.</returns>
        public List<String> GetAvailableDocumentTypes()
        {
            List<String> list = new List<String>();

            foreach (BuildCommandDelegateListItem i in _buildCommandHandlers)
                if (!list.Contains(i.DocumentType.ToString()))
                    list.Add(i.DocumentType.ToString());

            list.Sort();

            return list;
        }

        /// <summary>
        /// Get a list of the build actions for which build command providers
        /// have been registered for a particular document type.
        /// </summary>
        /// <param name="documentType">A document type.</param>
        /// <returns>A list of build actions.</returns>
        public List<String> GetAvailableActions(DocumentType documentType)
        {
            List<String> list = new List<String>();

            foreach (BuildCommandDelegateListItem i in _buildCommandHandlers)
                if (i.DocumentType.Matches(documentType) && !list.Contains(i.Action))
                    list.Add(i.Action);

            list.Sort();

            return list;
        }

        #endregion

        #region Persistence

        /// <summary>
        /// Load general settings from the session persistence store.
        /// </summary>
        public void LoadSettings()
        {
            AllwaysBuildOnCompile = _persistenceManager.ReadBoolean(
                Constants.KEY_ALWAYS_BUILD_ON_COMPILE, false);
            AllwaysBuildOnRun = _persistenceManager.ReadBoolean(
                Constants.KEY_ALWAYS_BUILD_ON_RUN, false);
        }

        /// <summary>
        /// Save general settings to the session persistence store.
        /// </summary>
        public void SaveSettings()
        {
            _persistenceManager.WriteBoolean(
                Constants.KEY_ALWAYS_BUILD_ON_COMPILE, AllwaysBuildOnCompile);
            _persistenceManager.WriteBoolean(
                Constants.KEY_ALWAYS_BUILD_ON_RUN, AllwaysBuildOnRun);
        }

        /// <summary>
        /// Load the registered build tools from the session persistence
        /// store.
        /// </summary>
        public void LoadTools()
        {
            _buildTools.ClearAll();

            List<String> toolStrings = 
                _persistenceManager.ReadStrings(
                    Constants.KEY_TOOLS_COLLECTION);

            foreach (string s in toolStrings)
            {
                BuildTool tool = BuildTool.Parse(s);
                if (tool != null)
                    _buildTools.AddTool(tool);
            }

            List<String> selectedToolStrings =
                _persistenceManager.ReadStrings(
                    Constants.KEY_SELECTED_TOOLS_COLLECTION);

            foreach (string s in selectedToolStrings)
                _buildTools.SelectTool(s);
        }

        /// <summary>
        /// Save the registered build tools to the session persistence
        /// store.
        /// </summary>
        public void SaveTools()
        {
            List<String> tools = new List<String>();

            foreach (string id in _buildTools.Tools.Keys)
                tools.Add(_buildTools.Tools[id].ToString());

            _persistenceManager.WriteStrings(
                Constants.KEY_TOOLS_COLLECTION, tools);

            List<String> selectedTools = 
                _buildTools.SelectedTools.Keys.ToList<String>();

            _persistenceManager.WriteStrings(
                Constants.KEY_SELECTED_TOOLS_COLLECTION, selectedTools);
        }

        /// <summary>
        /// Load the list of pinned files from the session persistence
        /// store.
        /// </summary>
        public void LoadPinnedFiles()
        {
            List<String> filenames = _persistenceManager.ReadStrings(
                Constants.KEY_PINNED_FILES);

            _pinnedFiles.Clear();

            foreach (string file in filenames)
            {
                if (!File.Exists(file)) continue;

                _pinnedFiles.Add(new PinnedFile(file));
            }
        }

        /// <summary>
        /// Save the list of pinned files to the session persistence
        /// store.
        /// </summary>
        public void SavePinnedFiles()
        {
            _persistenceManager.WriteStrings(
                Constants.KEY_PINNED_FILES,
                _pinnedFiles.GetPaths());
        }

        #endregion

        #region Macro Expansion

        /**********************************************************************
         * SRC_PATH - source file path
         * SRC_FILE - source file name
         * SRC_NAME - source file name without extension
         * SRC_EXT - source file extension
         * OUT_PATH - output file path
         * OUT_FILE - output file name
         * OUT_NAME - output file name without extension
         * OUT_EXT - output file extension
         * IDE_HOME - QuickSharp installation directory
         * USR_HOME - User's data directory
         * WORKSPACE - workspace folder name
         * COMMON_OPT - options provided by the build tool
         * EMBEDDED_OPT - options embedded in the source
         * RUNTIME_OPT - runtime arguments embedded in the source
         **********************************************************************/

        /// <summary>
        /// Expand a build tool template string by translating
        /// macro definitions into their actual values.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="buildTool">The build tool it belongs to.</param>
        /// <param name="srcText">The source code on which the build tool is being invoked.</param>
        /// <param name="srcFile">The name of the source file on which the build tool is being invoked.</param>
        /// <param name="outFile">The name of the output file of the build tool.</param>
        /// <returns>The expanded template.</returns>
        public string ExpandGenericMacros(
            string template, BuildTool buildTool,
            string srcText, FileInfo srcFile, FileInfo outFile)
        {
            string SRC_PATH = srcFile.FullName;
            string SRC_FILE = srcFile.Name;
            string SRC_NAME = Path.GetFileNameWithoutExtension(srcFile.Name);
            string SRC_EXT = srcFile.Extension;
            string OUT_PATH = outFile.FullName;
            string OUT_FILE = outFile.Name;
            string OUT_NAME = Path.GetFileNameWithoutExtension(outFile.Name);
            string OUT_EXT = outFile.Extension;
            string IDE_HOME = _applicationManager.QuickSharpHome;
            string USR_HOME = _applicationManager.QuickSharpUserHome;
            string USR_DOCS = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string SYSTEM = Environment.SystemDirectory;
            string PFILES = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string WORKSPACE = srcFile.Directory.Name;
            string COMMON_OPT = buildTool.UserArgs;
            string EMBEDDED_OPT = GetCompilerOptions(srcText);
            string RUNTIME_OPT = GetRuntimeOptions(srcText);

            /*
             * We allow carriage returns/newlines to be stored in the
             * build command string to improve ease of editing but we
             * need to take them out here.
             */

            COMMON_OPT = COMMON_OPT.Replace("\r\n", " ");

            template = template.Replace("${SRC_PATH}", SRC_PATH);
            template = template.Replace("${SRC_FILE}", SRC_FILE);
            template = template.Replace("${SRC_NAME}", SRC_NAME);
            template = template.Replace("${SRC_EXT}", SRC_EXT);
            template = template.Replace("${OUT_PATH}", OUT_PATH);
            template = template.Replace("${OUT_FILE}", OUT_FILE);
            template = template.Replace("${OUT_NAME}", OUT_NAME);
            template = template.Replace("${OUT_EXT}", OUT_EXT);
            template = template.Replace("${IDE_HOME}", IDE_HOME);
            template = template.Replace("${USR_HOME}", USR_HOME);
            template = template.Replace("${USR_DOCS}", USR_DOCS);
            template = template.Replace("${SYSTEM}", SYSTEM);
            template = template.Replace("${PFILES}", PFILES);
            template = template.Replace("${WORKSPACE}", WORKSPACE);
            template = template.Replace("${COMMON_OPT}", COMMON_OPT);
            template = template.Replace("${EMBEDDED_OPT}", EMBEDDED_OPT);
            template = template.Replace("${RUNTIME_OPT}", RUNTIME_OPT);

            return template;
        }

        #endregion

        #region Embedded Options

        /// <summary>
        /// Extract a source file's dependencies from the embedded options
        /// in the source text.
        /// </summary>
        /// <param name="src">The source text.</param>
        /// <returns>A list of dependent files.</returns>
        public List<String> GetDependencies(string src)
        {
            List<String> deps = new List<String>();

            Regex re = new Regex(@"(?m)//\?(.+)$");
            MatchCollection mc = re.Matches(src);

            foreach (Match m in mc)
            {
                string s = m.Groups[1].Value.Trim();

                if (!String.IsNullOrEmpty(s))
                    deps.Add(s.Trim());
            }

            return deps;
        }

        /// <summary>
        /// Extract the runtime arguments from the embedded options
        /// in the source text.
        /// </summary>
        /// <param name="src">The source text.</param>
        /// <returns>The runtime arguments.</returns>
        public string GetRuntimeOptions(string src)
        {
            string args = String.Empty;

            Regex re = new Regex("(?m)//@(.+)$");
            Match m = re.Match(src);
            
            if (m.Success) 
                args = m.Groups[1].Value.Trim();
            
            return args;
        }

        /// <summary>
        /// Extract the compiler options from the embedded options
        /// in the source text.
        /// </summary>
        /// <param name="src">The source text.</param>
        /// <returns>The compiler options.</returns>
        public string GetCompilerOptions(string src)
        {
            StringBuilder sb = new StringBuilder();

            Regex re = new Regex(@"(?m)//\$(.+)$");
            MatchCollection mc = re.Matches(src);

            foreach (Match m in mc)
            {
                string s = m.Groups[1].Value.Trim();
                
                if (s != String.Empty)
                {
                    sb.Append(s);
                    sb.Append(" ");
                }
            }

            return sb.ToString().Trim();
        }

        /// <summary>
        /// Get the build commands from the embedded options in
        /// the source text.
        /// </summary>
        /// <param name="src">The source text.</param>
        /// <returns>A dictionary of command types and instances of each (e.g.
        /// a set of DoPreCompile commands, a set of DoPostCompile commands and so on).</returns>
        public Dictionary<String, List<String>> GetActionCommands(string src)
        {
            Dictionary<String, List<String>> dict =
                new Dictionary<String, List<String>>();

            AddCommands(dict, Constants.ACTION_CMD_RUN_IN_OWN_WINDOW, src);
            AddCommands(dict, Constants.ACTION_CMD_DO_PRE_COMPILE, src);
            AddCommands(dict, Constants.ACTION_CMD_DO_POST_COMPILE, src);
            AddCommands(dict, Constants.ACTION_CMD_DO_PRE_RUN, src);
            AddCommands(dict, Constants.ACTION_CMD_DO_POST_RUN, src);

            return dict;
        }

        private void AddCommands(
            Dictionary<String, List<String>> dict, string name, string src)
        {
            Regex re = new Regex(@"(?i)//&\s*" + name + @"(.+)");

            List<String> list = new List<String>();
            
            MatchCollection mc = re.Matches(src);

            foreach (Match m in mc)
                list.Add(m.Groups[1].Value.Trim());

            dict.Add(name, list);
        }

        #endregion
    }
}
