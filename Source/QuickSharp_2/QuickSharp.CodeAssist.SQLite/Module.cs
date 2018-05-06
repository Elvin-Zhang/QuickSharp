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
using System.Windows.Forms;
using System.Collections.Generic;
using QuickSharp.Core;
using QuickSharp.CodeAssist.Sql;

namespace QuickSharp.CodeAssist.SQLite
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin

        public string GetID()
        {
            return "28E44EA1-C521-4B62-9F3D-9A9FB302F70A";
        }

        public string GetName()
        {
            return "QuickSharp Code Assist SQLite Support";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return "Provides SQLite support for the SQL Code Assist plugin.";
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
                    GetSQLiteCodeAssistProvider);
        }

        public ISqlCodeAssistProvider GetSQLiteCodeAssistProvider()
        {
            return new SQLiteCodeAssistProvider();
        }
    }
}
