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
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.IO;
using QuickSharp.Core;
using System.Text.RegularExpressions;

namespace QuickSharp.SqlManager
{
    public class SqlConnectionManager
    {
        #region Singleton

        private static SqlConnectionManager _singleton;

        public static SqlConnectionManager GetInstance()
        {
            if (_singleton == null)
                _singleton = new SqlConnectionManager();

            return _singleton;
        }

        #endregion

        private ApplicationManager _applicationManager;
        private IPersistenceManager _persistenceManager;
        private Dictionary<String, SqlConnection> _sqlConnections;
        private Dictionary<String, SqlDataProvider> _sqlDataProviders;

        public event MessageHandler ConnectionChange;

        private SqlConnectionManager()
        {
            _applicationManager =
                ApplicationManager.GetInstance();

            _persistenceManager = 
                _applicationManager.GetPersistenceManager(
                    Constants.PLUGIN_NAME);

            _sqlConnections = new Dictionary<String, SqlConnection>();
            _sqlDataProviders = new Dictionary<String, SqlDataProvider>();

            LoadDataProviders();
        }

        public void NotifyConnectionChange()
        {
            if (ConnectionChange != null)
                ConnectionChange();
        }

        #region Properties

        public Dictionary<String, SqlConnection> SqlConnections
        {
            get { return _sqlConnections; }
            set { _sqlConnections = value; }
        }

        public Dictionary<String, SqlDataProvider> SqlDataProviders
        {
            get { return _sqlDataProviders; }
            set { _sqlDataProviders = value; }
        }

        public string SelectedConnectionId { get; set; }
        public bool ConnectionIsActive { get; set; }

        public SqlConnection SelectedConnection
        {
            get
            {
                if (SelectedConnectionId == String.Empty)
                    return null;
                else
                {
                    if (_sqlConnections.ContainsKey(SelectedConnectionId))
                        return _sqlConnections[SelectedConnectionId];
                    else
                        return null;
                }
            }
        }

        #endregion

        #region Persistence

        public void Load()
        {
            /*
             * Don't call this from the constructor -
             * loading the SqlConnections invokes an instance
             * of this class and if it's called from the
             * constructor we get an infinite loop.
             */

            List<string> list = 
                _persistenceManager.ReadStrings(
                    Constants.KEY_SQL_CONNECTIONS);

            foreach (string s in list)
            {
                SqlConnection cnn = SqlConnection.Parse(s);

                if (cnn != null)
                    _sqlConnections[cnn.ID] = cnn;
            }

            SelectedConnectionId = _persistenceManager.ReadString(
                Constants.KEY_SQL_CONNECTION_SELECTED,
                String.Empty);
        }

        public void Save()
        {
            List<string> list = new List<string>();

            foreach (SqlConnection cnn in _sqlConnections.Values)
                list.Add(cnn.ToString());

            _persistenceManager.WriteStrings(
                Constants.KEY_SQL_CONNECTIONS, list);

            _persistenceManager.WriteString(
                Constants.KEY_SQL_CONNECTION_SELECTED,
                SelectedConnectionId);
        }

        #endregion

        #region Data Providers

        private void LoadDataProviders()
        {
            DataTable table = DbProviderFactories.GetFactoryClasses();

            foreach (DataRow row in table.Rows)
            {
                SqlDataProvider provider = new SqlDataProvider(
                    row[0].ToString(),
                    row[1].ToString(),
                    row[2].ToString(),
                    row[3].ToString());

                _sqlDataProviders[provider.InvariantName] = provider;
            }
        }

        public SqlDataProvider GetProviderByInvariantName(string name)
        {
            if (!_sqlDataProviders.ContainsKey(name))
                return null;
            else
                return _sqlDataProviders[name];
        }

        #endregion

        #region Connection Test

        public void TestConnection(SqlConnection cnn)
        {
            TestConnection(cnn.Provider, cnn.ConnectionString);
        }

        public void TestConnection(
            SqlDataProvider provider, string connectionString)
        {
            DbConnection cnn = null;

            try
            {
                DbProviderFactory factory =
                    DbProviderFactories.GetFactory(provider.InvariantName);

                cnn = factory.CreateConnection();
                cnn.ConnectionString = connectionString;

                cnn.Open();
            }
            finally
            {
                if (cnn != null) cnn.Dispose();
            }
        }

        #endregion

        #region SQL Server Helpers

        public bool ConnectionIsSqlServer(SqlConnection connection)
        {
            string providerName = connection.Provider.InvariantName;
            return (providerName == Constants.MSSQL_PROVIDER_INVARIANT_NAME);
        }

        public bool ConnectionIsSqlServerCe(SqlConnection connection)
        {
            string providerName = connection.Provider.InvariantName;

            /*
             * Don't want to be too specific about the provider names
             * so that we can have some degree of future proofing
             * but not too loose that versions that don't work with
             * Linq to Sql are allowed. Compromise on 3.x and 4.x and
             * hope that Microsoft don't change the naming conventions.
             */

            if (providerName.StartsWith(Constants.SSCE3X_PROVIDER_INVARIANT_NAME) ||
                providerName.StartsWith(Constants.SSCE4X_PROVIDER_INVARIANT_NAME))
                return true;

            return false;
        }

        public string GetSqlServerDbName(string connectionString)
        {
            /*
             * Get the name of the database; returns either
             * the name or an empty string.
             */

            string database = null;

            /*
             * Find the segement containing the data source.
             */

            foreach (string segment in connectionString.Split(';'))
            {
                string s = segment.Trim();
                string s2 = s.ToLower();

                if (s2.StartsWith("database") || s2.StartsWith("initial"))
                {
                    database = s;
                    break;
                }
            }

            if (database == null) return String.Empty;

            /*
             * Extract the name.
             */

            // Try 'database'
            Regex re1 = new Regex(@"(?i)database\s*=\s*([\w\s@#_]+)");
            Match m1 = re1.Match(database);

            if (m1.Success) return m1.Groups[1].Value;

            // Try 'initial catalog'
            Regex re2 = new Regex(@"(?i)initial\s+catalog\s*=\s*([\w\s@#_]+)");
            Match m2 = re2.Match(database);

            if (m2.Success)
                return m2.Groups[1].Value;
            else
                return String.Empty;
        }

        public string GetSqlServrCeDbName(
            string connectionString, bool keepPath)
        {
            /*
             * Get the base name of the database; returns either
             * the name or an empty string.
             */

            string dataSource = null;

            /*
             * Find the segement containing the data source.
             */

            foreach (string segment in connectionString.Split(';'))
            {
                string s = segment.Trim();

                if (s.ToLower().StartsWith("data"))
                {
                    dataSource = s;
                    break;
                }
            }

            if (dataSource == null) return String.Empty;

            /*
             * Extract the file basename.
             */

            Regex re = new Regex(@"(?i)data\s+source\s*=\s*\'?([\w\s_\.\\:]+)\.sdf\'?");

            Match m = re.Match(dataSource);

            if (m.Success)
            {
                string name = m.Groups[1].Value;

                if (keepPath) return name;

                /*
                 * Filename may include a path: return last '\' delimited
                 * segment or whole string if no path.
                 */

                string [] split = name.Split('\\');
                return split[split.Length - 1];
            }

            return String.Empty;
        }

        #endregion
    }
}
