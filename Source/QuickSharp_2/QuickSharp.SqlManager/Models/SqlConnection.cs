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

namespace QuickSharp.SqlManager
{
    public class SqlConnection
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public SqlDataProvider Provider { get; set; }

        private string _connectionString;

        /*
         * We maintain carriage returns/newlines in the string
         * to ease editing in the config dialog. They're stripped
         * out when required for DB connections but not when edited.
         */

        public string ConnectionString
        {
            get { return _connectionString.Replace("\r\n", " "); }
            set { _connectionString = value; }
        }

        public string ConnectionStringForEditing
        {
            get { return _connectionString; }
        }

        public SqlConnection(
            string id,
            string name,
            string connectionString,
            SqlDataProvider provider)
        {
            ID = id;
            Name = name;
            _connectionString = connectionString;
            Provider = provider;
        }

        public override string ToString()
        {
            return String.Format("{0}¬{1}¬{2}¬{3}",
                ID, Name, 
                ConnectionStringForEditing,
                Provider.InvariantName);
        }

        public static SqlConnection Parse(string s)
        {
            string[] split = s.Split('¬');

            if (split.Length != 4)
                return null;

            string id = split[0];
            string name = split[1];
            string connectionString = split[2];
            string providerName = split[3];

            SqlConnectionManager sqlConnectionManager =
                SqlConnectionManager.GetInstance();

            SqlDataProvider provider = 
                sqlConnectionManager.GetProviderByInvariantName(providerName);

            if (provider == null)
                return null;

            return new SqlConnection(
                id, name, connectionString, provider);
        }
    }
}
