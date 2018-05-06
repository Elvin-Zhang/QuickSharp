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

using System.Collections.Generic;
using QuickSharp.CodeAssist.Sql;
using QuickSharp.Core;

namespace QuickSharp.CodeAssist.MSSql
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "DBC95EEA-A668-4428-A84D-21E681F9EA71";
        }

        public string GetName()
        {
            return "QuickSharp Code Assist SQL Server Support";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return 
                "Provides Microsoft SQL Server support for the SQL Code Assist plugin.";
        }

        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            deps.Add(new Plugin(QuickSharpPlugins.Editor, "QuickSharp.Editor", 1));
            deps.Add(new Plugin(QuickSharpPlugins.CodeAssist, "QuickSharp.CodeAssist", 1));
            deps.Add(new Plugin(QuickSharpPlugins.CodeAssist_Sql, "QuickSharp.CodeAssist.Sql", 1));
            deps.Add(new Plugin(QuickSharpPlugins.SqlManager, "QuickSharp.SqlManager", 1));
            return deps;
        }

        public void Activate(MainForm mainForm)
        {
            _mainForm = mainForm;
            ActivatePlugin();
        }

        #endregion

        private MainForm _mainForm;
        
        private void ActivatePlugin()
        {
            IPersistenceManager persistenceManager =
                ApplicationManager.GetInstance().GetPersistenceManager(
                    Constants.PLUGIN_NAME);

            string dataProviderInvariantName = persistenceManager.ReadString(
                Constants.KEY_DATA_PROVIDER_INVARIANT_NAME,
                Constants.DATA_PROVIDER_INVARIANT_NAME);

            SqlCodeAssistManager.GetInstance().
                RegisterProviderFactoryHandlers(
                    dataProviderInvariantName,
                    GetMSSqlCodeAssistProvider);
        }

        public ISqlCodeAssistProvider GetMSSqlCodeAssistProvider()
        {
            return new MSSqlCodeAssistProvider();
        }
    }
}
