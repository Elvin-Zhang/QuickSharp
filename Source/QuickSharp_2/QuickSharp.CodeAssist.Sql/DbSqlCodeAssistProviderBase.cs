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
using System.Windows.Forms;
using QuickSharp.Editor;
using QuickSharp.SqlManager;

namespace QuickSharp.CodeAssist.Sql
{
    public abstract class DbSqlCodeAssistProviderBase
    {
        protected SqlConnectionManager sqlConnectionManager;
        protected Dictionary<String, TableAlias> tableAliases;
        protected DatabaseObjectsBase activeDatabase;
        protected char[] stmtDelimiters = { ' ', '(', ')', ',' };

        public DbSqlCodeAssistProviderBase()
        {
            sqlConnectionManager = SqlConnectionManager.GetInstance();
        }

        #region Load Database Objects

        protected virtual DatabaseObjectsBase GetDatabaseObjects(
            SqlConnection connection)
        {
            return new DatabaseObjectsBase(connection);
        }

        protected void LoadDatabaseObjects()
        {
            DatabaseObjectsBase dbo = GetDatabaseObjects(
                sqlConnectionManager.SelectedConnection);

            if (dbo.IsValid)
            {
                activeDatabase = dbo;
            }
            else
            {
                activeDatabase = null;

                MessageBox.Show(
                    Resources.MetadataErrorMessage,
                    Resources.MetadataErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        #endregion

        #region SQL Keywords

        protected string GetFirstToken(string text)
        {
            string[] split = text.Trim().Split();
            return split[0];
        }

        protected string GetSecondToken(string text)
        {
            string[] split = text.Trim().Split();
            if (split.Length > 1)
                return split[1];
            else
                return null;
        }

        protected string GetCurrentToken(string src1, string src2)
        {
            string[] split1 = src1.Split(stmtDelimiters);
            string[] split2 = src2.Split(stmtDelimiters);

            return split1[split1.Length - 1] + split2[0];
        }

        protected string[] sqlKeywords =
        {
            "select", "from", "join", "on", "where", "order", "group",
            "by", "having", "update", "set", "insert", "into", "values",
            "delete", "create", "alter", "drop", "table", "view"
        };

        protected bool IsSQLKeyword(string text)
        {
            return (Array.IndexOf(sqlKeywords, text.ToLower()) != -1);
        }

        protected bool IsSQLStatement(string text)
        {
            string[] keywords =
            {
                "select", "insert", "update", "delete",
                "create", "alter", "drop", "use"
            };

            text = text.ToLower();

            return (Array.IndexOf(keywords, text) != -1);
        }

        protected string GetActiveKeyword(string text)
        {
            text = text.ToLower();

            string keyword = null;

            string[] split = text.Split();

            Array.Reverse(split);

            for (int i = 0; i < split.Length; i++)
            {
                int j = Array.IndexOf(sqlKeywords, split[i]);
                if (j != -1)
                {
                    keyword = sqlKeywords[j];

                    if (keyword == "by" && i < split.Length - 1)
                    {
                        if (split[i + 1] == "order")
                            keyword = "order_by";
                        else if (split[i + 1] == "group")
                            keyword = "group_by";
                    }

                    break;
                }
            }

            return keyword;
        }

        #endregion

        #region Helpers

        protected string RemoveMultiSpaces(string text)
        {
            StringBuilder sb = new StringBuilder();

            bool lastCharWasSpace = false;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != ' ')
                {
                    sb.Append(text[i]);
                    lastCharWasSpace = false;
                    continue;
                }

                if (!lastCharWasSpace)
                {
                    sb.Append(text[i]);
                    lastCharWasSpace = true;
                }
            }

            return sb.ToString();
        }

        protected virtual string GetEscapedName(string name)
        {
            return GetGenericEscapedName(
                name, String.Empty, String.Empty);
        }

        protected string GetGenericEscapedName(
            string name, string start, string end)
        {
            if (name.IndexOf(" ") != -1)
                return String.Format("{0}{1}{2}",
                    start, name, end);
            else
                return name;
        }

        protected virtual string GetEscapedFullName(string name)
        {
            return GetGenericEscapedFullName(
                name, String.Empty, String.Empty);
        }

        protected string GetGenericEscapedFullName(
            string name, string start, string end )
        {
            string[] split = name.Split('.');

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < split.Length; i++)
            {
                if (split[i].IndexOf(" ") != -1)
                    sb.Append(String.Format("{0}{1}{2}",
                        start, split[i], end));
                else
                    sb.Append(split[i]);

                if (i < split.Length - 1)
                    sb.Append(".");
            }

            return sb.ToString();
        }

        protected string GetSchemaFromName(string name)
        {
            if (name.IndexOf(".") != -1)
            {
                string[] split = name.Split('.');
                return split[0];
            }

            return null;
        }

        #endregion

        #region Selected Tables

        protected List<string> GetSelectedTables(
            string src, bool getAliases)
        {
            List<string> tables = new List<string>();

            /*
             * Create new dictionary (GC the old one).
             */

            tableAliases = new Dictionary<String, TableAlias>();

            /*
             * Get all tables named after from or join keywords.
             */

            string[] split = src.Trim().Split();

            for (int i = 0; i < split.Length - 1; i++)
            {
                if (split[i] == "from" ||
                    split[i] == "join" ||
                    split[i] == "update")
                {
                    i++;

                    string tableName = split[i];

                    tables.Add(tableName.ToLower());

                    if (getAliases && i < split.Length - 1)
                    {
                        string alias = null;

                        if (split[i + 1] != "as")
                        {
                            if (!IsSQLKeyword(split[i + 1]))
                            {
                                alias = split[i + 1];
                            }
                        }
                        else
                        {
                            if (i < split.Length - 2 &&
                                !IsSQLKeyword(split[i + 2]))
                            {
                                alias = split[i + 2];
                            }
                        }

                        if (alias != null)
                        {
                            string aliasNC = alias.ToLower();
                            tableAliases[aliasNC] =
                                new TableAlias(alias, tableName);
                        }
                    }
                }
            }

            return tables;
        }

        #endregion

        #region Build Entity Lists

        protected List<LookupListItem> BuildSchemaList()
        {
            List<LookupListItem> list = new List<LookupListItem>();

            foreach (Schema schema in activeDatabase.Schemata.Values)
            {
                LookupListItem item = new LookupListItem();
                item.DisplayText = schema.Name;
                item.InsertText = GetEscapedName(schema.Name);
                item.ToolTipText = String.Format("{0} {1}",
                    Resources.TooltipSchema,
                    GetEscapedName(schema.Name));
                item.Category = QuickSharp.CodeAssist.Constants.NAMESPACE;

                list.Add(item);
            }

            return list;
        }

        protected List<LookupListItem> BuildSchemaTableList(Schema schema)
        {
            List<LookupListItem> list = new List<LookupListItem>();

            foreach (Table table in schema.Tables.Values)
            {
                LookupListItem item = new LookupListItem();
                item.DisplayText = table.Name;
                item.InsertText = GetEscapedName(table.Name);

                if (table.IsView)
                {
                    item.ToolTipText = String.Format("{0} {1}",
                        Resources.TooltipView,
                        GetEscapedFullName(table.FullName));
                    item.Category = QuickSharp.CodeAssist.Constants.VIEW;
                }
                else
                {
                    item.ToolTipText = String.Format("{0} {1}",
                        Resources.TooltipTable,
                        GetEscapedFullName(table.FullName));
                    item.Category = QuickSharp.CodeAssist.Constants.TABLE;
                }

                list.Add(item);
            }

            return list;
        }

        protected List<LookupListItem> BuildTableColumnList(Table table)
        {
            List<LookupListItem> list = new List<LookupListItem>();

            foreach (TableColumn col in table.Columns)
            {
                LookupListItem item = new LookupListItem();
                item.DisplayText = col.Name;
                item.InsertText = GetEscapedName(col.Name);
                item.ToolTipText = String.Format("{0} {1}.{2}",
                    col.Type,
                    GetEscapedFullName(table.FullName),
                    GetEscapedName(col.Name));

                item.Category = QuickSharp.CodeAssist.Constants.FIELD;

                list.Add(item);
            }

            return list;
        }

        protected List<LookupListItem> BuildTableAliasList()
        {
            List<LookupListItem> list = new List<LookupListItem>();

            foreach (TableAlias alias in tableAliases.Values)
            {
                LookupListItem item = new LookupListItem();
                item.DisplayText = alias.DisplayName;
                item.InsertText = GetEscapedFullName(alias.DisplayName);
                item.ToolTipText = String.Format("{0} {1} = {2}",
                    Resources.TooltipAlias,
                    GetEscapedFullName(alias.DisplayName),
                    GetEscapedFullName(alias.TableDisplayName));

                item.Category = QuickSharp.CodeAssist.Constants.TABLE_ALIAS;

                list.Add(item);
            }

            return list;
        }

        #endregion

        #region Colorization

        protected void Colorize(ScintillaEditForm document)
        {
            if (activeDatabase != null)
            {
                document.Editor.Lexing.Keywords[0] =
                    activeDatabase.GetKeywords();
                document.Editor.Lexing.Keywords[1] =
                    activeDatabase.GetDataTypes();
                document.Editor.Lexing.Keywords[5] =
                    activeDatabase.GetFunctions();

                document.Editor.Lexing.Keywords[4] =
                    activeDatabase.GetEntityNames();

                document.Editor.Lexing.Colorize();
            }
        }

        #endregion
    }
}
