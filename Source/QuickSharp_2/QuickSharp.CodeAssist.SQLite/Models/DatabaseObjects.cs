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
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using QuickSharp.SqlManager;
using QuickSharp.CodeAssist.Sql;

namespace QuickSharp.CodeAssist.SQLite
{
    public class DatabaseObjects : DatabaseObjectsBase
    {
        public DatabaseObjects(SqlConnection connection)
            : base(connection)
        {
            isValid = LoadObjects();
        }

        #region Load

        protected override bool LoadObjects()
        {
            DbConnection cnn = null;

            try
            {
                DbProviderFactory factory =
                    DbProviderFactories.GetFactory(
                        connection.Provider.InvariantName);

                cnn = factory.CreateConnection();
                cnn.ConnectionString = connection.ConnectionString;

                cnn.Open();

                database = cnn.Database;

                DataTable schema = cnn.GetSchema("COLUMNS");

                foreach (DataRow row in schema.Rows)
                {
                    /*
                     * Need to make keys lowercase for case-insensitive
                     * matching but retain case for object properties.
                     */

                    string schemaName = row[0] as string;
                    string schemaNameNC = schemaName.ToLower();
                    schemaNameNC = schemaNameNC.Replace(' ', '~');

                    string tableName = row[2] as string;
                    string tableNameNC = tableName.ToLower();
                    tableNameNC = tableNameNC.Replace(' ', '~');

                    bool isView = false;

                    string columnName = row[3] as string;
                    string columnType = row[11] as string;

                    long columnSize = (row.IsNull(15) ? -1 :(int) row[15]);

                    string columnTypeName = columnType;

                    if (!schemata.ContainsKey(schemaNameNC))
                        schemata[schemaNameNC] = new Schema(schemaName);

                    if (!schemata[schemaNameNC].Tables.ContainsKey(tableNameNC))
                        schemata[schemaNameNC].Tables[tableNameNC] =
                            new Table(tableName, schemaName, isView);

                    schemata[schemaNameNC].Tables[tableNameNC].Columns.Add(
                        new TableColumn(columnName, columnTypeName));
                }

                cnn.Close();

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (cnn != null) cnn.Dispose();
            }
        }

        #endregion

        #region Helpers

        protected override string GetColumnTypeName(
            string columnType, long columnSize)
        {
            if (columnSize == -1 ||
                columnType.EndsWith("text") ||
                columnType.EndsWith("blob"))
                return columnType;
            else
                return String.Format("{0}({1})",
                    columnType, columnSize);
        }

        #endregion

        #region Colorization

        public override string GetKeywords()
        {
            return
                "abort action add after all alter analyze and any as asc attach autoincr before begin between blob by " +
                "cascade case cast check collate column commit concat conflict constraint create database default deferrable " +
                "deferred delete desc detach distinct drop each else end escape except exclusive exists explain fail float for " +
                "foreign from function group having id if ignore immediate in index indexed initially insert instead integer " +
                "intersect into is isnull join key like limit match no not notnull null of offset on or order plan pragma " +
                "primary query raise references register reindex release rename replace restrict rollback row savepoint select " +
                "set string table temp then to transaction trigger union unique update using vacuum values variable view " +
                "virtual when where abs changes coalesce date datetime glob ifnull hex julianday last_insert_rowid length " +
                "like load_extension lower ltrim match max min nullif quote random randomblob regex replace round rtrim " +
                "sqlite_compileoption_get sqlite_compileoption_used sqlite_source_id sqlite_version strftime substr time " +
                "total_changes trim typeof upper zeroblob avg count group_concat sum total";
        }

        #endregion
    }
}
