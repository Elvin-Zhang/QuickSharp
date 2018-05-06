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
using System.Data.Common;
using QuickSharp.SqlManager;
using QuickSharp.CodeAssist.Sql;

using System.Text;

namespace QuickSharp.CodeAssist.Sql
{
    public class DatabaseObjectsBase
    {
        protected SqlConnection connection;
        protected Dictionary<String, Schema> schemata;
        protected string database;
        protected bool isValid;

        public DatabaseObjectsBase(SqlConnection cnn)
        {
            if (cnn == null) return;

            connection = cnn;
            schemata = new Dictionary<String, Schema>();
            database = String.Empty;
        }

        #region Properties

        public bool IsValid
        {
            get { return isValid; }
        }

        public Dictionary<String, Schema> Schemata
        {
            get { return schemata; }
        }

        public string Database
        {
            get { return database; }
        }

        public bool HaveDatabase
        {
            get { return !String.IsNullOrEmpty(database); }
        }

        #endregion

        #region Load

        protected virtual bool LoadObjects()
        {
            return false;
        }

        #endregion

        #region Helpers

        protected virtual string GetColumnTypeName(
            string columnType, long columnSize)
        {
            return String.Format(
                "{0}({1})", columnType, columnSize);
        }

        #endregion

        #region Colorization

        public string GetEntityNames()
        {
            if (!isValid) return String.Empty;

            List<String> entities = new List<String>();

            foreach (Schema schema in schemata.Values)
            {
                if (!entities.Contains(schema.Name))
                    entities.Add(schema.Name);

                foreach (Table table in schema.Tables.Values)
                {
                    if (!entities.Contains(table.Name))
                        entities.Add(table.Name);

                    foreach (TableColumn tableColumn in table.Columns)
                    {
                        if (!entities.Contains(tableColumn.Name))
                            entities.Add(tableColumn.Name);
                    }
                }
            }

            StringBuilder sb = new StringBuilder();

            foreach (string name in entities)
            {
                sb.Append(name.ToLower());
                sb.Append(" ");
            }

            return sb.ToString();
        }

        public virtual string GetKeywords()
        {
            return String.Empty;
        }

        public virtual string GetDataTypes()
        {
            return String.Empty;
        }

        public virtual string GetFunctions()
        {
            return String.Empty;
        }

        #endregion
    }
}
