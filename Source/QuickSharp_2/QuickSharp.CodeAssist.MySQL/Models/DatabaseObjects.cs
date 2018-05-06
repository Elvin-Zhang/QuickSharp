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

namespace QuickSharp.CodeAssist.MySQL
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

            try
            {
                DbProviderFactory factory =
                    DbProviderFactories.GetFactory(
                        connection.Provider.InvariantName);

                cnn = factory.CreateConnection();
                cnn.ConnectionString = connection.ConnectionString;

                cnn.Open();

                database = cnn.Database;

                cmd = factory.CreateCommand();
                cmd.Connection = cnn;
                cmd.CommandText =
                    "SELECT C.TABLE_SCHEMA, C.TABLE_NAME, T.TABLE_TYPE, " +
                    "C.COLUMN_NAME, C.DATA_TYPE, C.CHARACTER_MAXIMUM_LENGTH " +
                    "FROM INFORMATION_SCHEMA.COLUMNS C INNER JOIN " +
                    "INFORMATION_SCHEMA.TABLES T ON " +
                    "T.TABLE_SCHEMA = C.TABLE_SCHEMA AND " +
                    "T.TABLE_NAME = C.TABLE_NAME ";

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

                    bool isView = (rdr.GetString(2).EndsWith("VIEW"));
                    
                    string columnName = rdr.GetString(3);
                    string columnType = rdr.GetString(4);
                    long columnSize = (rdr.IsDBNull(5) ? -1 : rdr.GetInt64(5));
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
                "accessible add all alter analyze and as asc asensitive before between bigint binary blob both " +
                "by call cascade case change char character check collate column condition constraint continue convert create " +
                "cross current_date current_time current_timestamp current_user cursor database databases day_hour " +
                "day_microsecond day_minute day_second dec decimal declare default delayed delete desc describe deterministic " +
                "distinct distinctrow div double drop dual each else elseif enclosed escaped exists exit explain false " +
                "fetch float float4 float8 for force foreign from fulltext grant group having high_priority hour_microsecond hour_minute " +
                "hour_second if ignore in index infile inner inout insensitive insert int int1 int2 int3 int4 " +
                "int8 integer interval into is iterate join key keys kill leading leave left like limit " +
                "linear lines load localtime localtimestamp lock long longblob longtext loop low_priority master_ssl_verify_server_cert " +
                "match mediumblob mediumint mediumtext middleint minute_microsecond minute_second mod modifies " +
                "natural not no_write_to_binlog null numeric on optimize option optionally or order out " +
                "outer outfile precision primary procedure purge range read reads read_write real references " +
                "regexp release rename repeat replace require restrict return revoke right rlike schema " +
                "schemas second_microsecond select sensitive separator set show smallint spatial specific sql sqlexception " +
                "sqlstate sqlwarning sql_big_result sql_calc_found_rows sql_small_result ssl starting straight_join table " +
                "terminated then tinyblob tinyint tinytext to trailing trigger true undo union unique " +
                "unlock unsigned update usage use using utc_date utc_time utc_timestamp values varbinary varchar " +
                "varcharacter varying view when where while with write xor year_month zerofill " +
                "accessible linear master_ssl_verify_server_cert range read_only read_write ";
        }

        #endregion
    }
}
