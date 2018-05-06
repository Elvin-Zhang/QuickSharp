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
using Microsoft.Win32;
using QuickSharp.Core;
using QuickSharp.BuildTools;

namespace QuickSharp.Language.Mono
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "5A4550C2-A0DC-48BB-8E1F-952351492A7B";
        }

        public string GetName()
        {
            return "QuickSharp Mono C# Support";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return "Provides support for Mono C# compilers.";
        }

        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            deps.Add(new Plugin(QuickSharpPlugins.BuildTools, "QuickSharp.BuildTools", 1));
            deps.Add(new Plugin(QuickSharpPlugins.Language_CSharp, "QuickSharp.Language.CSharp", 1));
            return deps;
        }

        public void Activate(MainForm mainForm)
        {
            ActivatePlugin();
        }

        #endregion

        private ApplicationManager _applicationManager;
        private DocumentType _documentType;

        private void ActivatePlugin()
        {
            _applicationManager = ApplicationManager.GetInstance();

            IPersistenceManager persistenceManager = _applicationManager.
                GetPersistenceManager(Constants.PLUGIN_NAME);

            bool haveTools = persistenceManager.ReadBoolean(
                Constants.KEY_HAVE_MONO_TOOLS, false);

            if (!haveTools && CreateDefaultTools())
                persistenceManager.WriteBoolean(
                    Constants.KEY_HAVE_MONO_TOOLS, true);
        }

        private bool CreateDefaultTools()
        {
            /*
             * Define the document type.
             */

            _documentType = new DocumentType(Constants.DOCUMENT_TYPE_CS);

            /*
             * Get the build tool locations.
             */

            string monoRootKey = Registry.LocalMachine +
                "\\Software\\Novell\\Mono";

            string monoVersion = Registry.GetValue(
                monoRootKey, "DefaultCLR", null) as string;

            if (monoVersion == null) return false;

            string monoPathKey = String.Format("{0}\\{1}",
                monoRootKey, monoVersion);

            string monoSDKRootPath = Registry.GetValue(
                monoPathKey, "SdkInstallRoot", null) as string;

            if (String.IsNullOrEmpty(monoSDKRootPath)) return false;

            string monoSDKBinPath = Path.Combine(monoSDKRootPath, "bin");

            if (!Directory.Exists(monoSDKBinPath)) return false;

            /*
             * Create the default build tool instances for the Mono compilers.
             */

            BuildTool mcscompiler = CreateMonoCompiler(
                Resources.MonoMCSCompiler, monoSDKBinPath, "mcs.bat");

            BuildTool gmcscompiler = CreateMonoCompiler(
                Resources.MonoGMCSCompiler, monoSDKBinPath, "gmcs.bat");

            BuildTool dmcscompiler = CreateMonoCompiler(
                Resources.MonoDMCSCompiler, monoSDKBinPath, "dmcs.bat");

            BuildTool monoexe = new BuildTool(Guid.NewGuid().ToString(),
                _documentType, Resources.MonoRuntime);

            monoexe.Action = QuickSharp.BuildTools.Constants.ACTION_RUN;
            monoexe.Path = Path.Combine(monoSDKBinPath, "mono.exe");
            monoexe.Args = "\"${OUT_PATH}\" ${RUNTIME_OPT}";
            monoexe.UserArgs = String.Empty;
            monoexe.LineParserName = String.Empty;

            BuildToolManager buildToolManager = 
                BuildToolManager.GetInstance();

            buildToolManager.BuildTools.AddTool(mcscompiler);
            buildToolManager.BuildTools.AddTool(gmcscompiler);
            buildToolManager.BuildTools.AddTool(dmcscompiler);
            buildToolManager.BuildTools.AddTool(monoexe);

            /*
             * Update the tools collection.
             */

            buildToolManager.UpdateTools(_documentType);
            buildToolManager.BuildTools.SortTools();

            return true;
        }

        private BuildTool CreateMonoCompiler(
            string name, string path, string filename)
        {
            BuildTool compiler = new BuildTool(
                Guid.NewGuid().ToString(), _documentType, name);

            compiler.Action = QuickSharp.BuildTools.Constants.ACTION_COMPILE;
            compiler.Path = Path.Combine(path, filename);
            compiler.Args = "${DOTNET_TARGET} ${COMMON_OPT} ${EMBEDDED_OPT} \"${SRC_FILE}\"";
            compiler.UserArgs = String.Empty;

            /*
             * The line parser name must be identical to the name defined in the
             * CSharp plugin. This is not ideal; the parser should really be identified
             * by a key or ID we can access via a constant rather than a text string
             * included as a resource. This is a design flaw in the build tools system.
             */

            compiler.LineParserName = Resources.MicrosoftCSLineParser;

            return compiler;
        }
    }
}
