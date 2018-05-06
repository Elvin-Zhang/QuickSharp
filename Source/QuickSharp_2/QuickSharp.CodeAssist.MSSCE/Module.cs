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

namespace QuickSharp.CodeAssist.MSSCE
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "5BB1B367-87E7-4DED-95C6-6796067B00A3";
        }

        public string GetName()
        {
            return "QuickSharp Code Assist SQL Server Compact Edition Support";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return 
                "Provides Microsoft SQL Server Compact Edition support " +
                "for the SQL Code Assist plugin. Supports versions 3.5 and 4.0.";
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

            /*
             * We support SCE 3.5 and 4.0. use the registry override keys to
             * use a different invariant name in place of the defaults.
             */

            string providerInvariantName35 = persistenceManager.ReadString(
                Constants.KEY_DATA_PROVIDER_INVARIANT_NAME_35,
                Constants.DATA_PROVIDER_INVARIANT_NAME_35);

            string providerInvariantName40 = persistenceManager.ReadString(
                Constants.KEY_DATA_PROVIDER_INVARIANT_NAME_40,
                Constants.DATA_PROVIDER_INVARIANT_NAME_40);

            SqlCodeAssistManager sqlCodeAssistManager =
                SqlCodeAssistManager.GetInstance();

            sqlCodeAssistManager.RegisterProviderFactoryHandlers(
                providerInvariantName35, GetMSSCECodeAssistProvider);

            sqlCodeAssistManager.RegisterProviderFactoryHandlers(
                providerInvariantName40, GetMSSCECodeAssistProvider);
        }

        public ISqlCodeAssistProvider GetMSSCECodeAssistProvider()
        {
            return new MSSCECodeAssistProvider();
        }
    }
}
