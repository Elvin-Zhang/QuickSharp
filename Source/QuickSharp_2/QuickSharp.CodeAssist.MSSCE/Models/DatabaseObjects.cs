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
using System.Data.Common;
using QuickSharp.CodeAssist.Sql;
using QuickSharp.SqlManager;

namespace QuickSharp.CodeAssist.MSSCE
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
            DbCommand cmd = null;
            DbDataReader rdr = null;

            /*
             * This is basically the same code as the MSSql provider
             * with minor changes for the single database nature of SCE.
             */

            try
            {
                DbProviderFactory factory =
                    DbProviderFactories.GetFactory(
                        connection.Provider.InvariantName);

                cnn = factory.CreateConnection();
                cnn.ConnectionString = connection.ConnectionString;

                cnn.Open();

                cmd = factory.CreateCommand();
                cmd.Connection = cnn;
                cmd.CommandText =
                    "SELECT '" + Constants.DEFAULT_SCHEMA +
                    "' as TABLE_SCHEMA, C.TABLE_NAME, T.TABLE_TYPE, " +
                    "C.COLUMN_NAME, C.DATA_TYPE, C.CHARACTER_MAXIMUM_LENGTH " +
                    "FROM INFORMATION_SCHEMA.COLUMNS C " +
                    "INNER JOIN INFORMATION_SCHEMA.TABLES T ON " +
                    "T.TABLE_NAME = C.TABLE_NAME";

                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    /*
                     * Need to make keys lowercase for case-insensitive
                     * matching but retain case for object properties.
                     */

                    string schemaName = rdr.GetString(0);
                    string schemaNameNC = schemaName.ToLower();
                    schemaNameNC = schemaNameNC.Replace(' ', '~');

                    string tableName = rdr.GetString(1);
                    string tableNameNC = tableName.ToLower();
                    tableNameNC = tableNameNC.Replace(' ', '~');

                    // No view support in Compact Edition
                    bool isView = false; // (rdr.GetString(2) == "VIEW");
                    
                    string columnName = rdr.GetString(3);
                    string columnType = rdr.GetString(4);
                    int columnSize = (rdr.IsDBNull(5) ? -1 : rdr.GetInt32(5));
                    string columnTypeName = GetColumnTypeName(
                        columnType, columnSize);

                    if (!schemata.ContainsKey(schemaNameNC))
                        schemata[schemaNameNC] = new Schema(schemaName);

                    if (!schemata[schemaNameNC].Tables.ContainsKey(tableNameNC))
                        schemata[schemaNameNC].Tables[tableNameNC] =
                            new Table(tableName, schemaName, isView);

                    schemata[schemaNameNC].Tables[tableNameNC].Columns.Add(
                        new TableColumn(columnName, columnTypeName));
                }

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (rdr != null) rdr.Dispose();
                if (cnn != null) cnn.Dispose();
            }
        }

        #endregion

        #region Helpers

        protected override string GetColumnTypeName(
            string columnType, long columnSize)
        {
            if (columnSize == -1 ||
                columnType == "ntext" ||
                columnType == "text" ||
                columnType == "image")
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
                "add all alter and any as asc authorization backup begin between break browse bulk by cascade case check checkpoint " +
                "close clustered coalesce collate column commit compute constraint contains containstable continue convert create " +
                "cross current current_date current_time current_timestamp current_user cursor database dbcc deallocate declare " +
                "default delete deny desc disk distinct distributed double drop dummy dump else end errlvl escape except exec " +
                "execute exists exit external fetch file fillfactor for foreign freetext freetexttable from full function goto " +
                "grant group having holdlock identity identity_insert identitycol if in index inner insert intersect into is join " +
                "key kill left like lineno load national nocheck nonclustered not null nullif of off offsets on open opendatasource " +
                "openquery openrowset openxml option or order outer over percent pivot plan precision primary print proc procedure " +
                "public raiserror read readtext reconfigure references replication restore restrict return revert revoke right " +
                "rollback rowcount rowguidcol rule save schema select session_user set setuser shutdown some statistics system_user " +
                "table tablesample textsize then to top tran transaction trigger truncate tsequal union unique unpivot update " +
                "updatetext use user values varying view waitfor when where while with writetext";
        }

        public override string GetDataTypes()
        {
            return
                "bigint decimal int numeric smallint money tinyint smallmoney bit float real datetime smalldatetime char text " +
                "varchar nchar ntext nvarchar binary image varbinary cursor timestamp sql_variant uniqueidentifier table xml";
        }

        public override string GetFunctions()
        {
            return
                "abs acos app_name ascii asin atan atn2 avg binary_checksum case cast " +
                "ceiling char charindex checksum checksum_agg coalesce collationproperty " +
                "col_length col_name columns_updated columnproperty convert cos cot count " +
                "count_big current_timestamp current_user cursor_status databaseproperty " +
                "databasepropertyex datalength dateadd datediff datename datepart day " +
                "db_id db_name degrees difference exp file_id file_name filegroup_id " +
                "filegroup_name filegroupproperty fileproperty floor fn_helpcollations " +
                "fn_listextendedproperty fn_servershareddrives fn_trace_geteventinfo " +
                "fn_trace_getfilterinfo fn_trace_getinfo fn_trace_gettable " +
                "fn_virtualfilestats formatmessage fulltextcatalogproperty " +
                "fulltextserviceproperty getansinull getdate getutcdate grouping " +
                "has_dbaccess host_id host_name ident_current ident_incr ident_seed " +
                "index_col indexkey_property indexproperty is_member is_srvrolemember " +
                "isdate isnull isnumeric left len log log10 lower ltrim max min month " +
                "nchar newid nullif object_id object_name objectproperty parsename " +
                "patindex permissions pi power quotename radians rand replace replicate " +
                "reverse right round rowcount_big rtrim scope_identity serverproperty " +
                "sessionproperty session_user sign sin soundex space sqare " +
                "sql_variant_property sqrt stats_date stdev stdevp str stuff substring " +
                "sum suser_sid suser_sname system_user tan typeproperty unicode upper " +
                "user_id user_name var varp year";
        }

        #endregion
    }
}
