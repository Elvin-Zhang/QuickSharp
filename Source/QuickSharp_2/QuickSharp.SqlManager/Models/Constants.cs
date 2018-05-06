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

namespace QuickSharp.SqlManager
{
    public class Constants
    {
        public const string PLUGIN_NAME = "QuickSharp.SqlManager";
        public const string DOCUMENT_TYPE_SQL = ".sql";
        public const string SQL_METAL_DEFAULT_PATH = @"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\SqlMetal.exe";
        public const string MSSQL_PROVIDER_INVARIANT_NAME = "System.Data.SqlClient";
        public const string SSCE3X_PROVIDER_INVARIANT_NAME = "System.Data.SqlServerCe.3.";
        public const string SSCE4X_PROVIDER_INVARIANT_NAME = "System.Data.SqlServerCe.4.";

        // Registry Keys
        public const string KEY_SQL_CONNECTIONS = "SqlConnections";
        public const string KEY_SQL_CONNECTION_SELECTED = "SqlConnectionSelected";
        public const string KEY_SQL_METAL_PATH = "SqlMetalPath";

        // AppStore Keys
        public const string APP_STORE_KEY_NON_TOOL_SOURCE_TYPES = "APP_STORE_KEY_NON_TOOL_SOURCE_TYPES";

        // UI Elements
        public const string UI_TOOLS_MENU_RUN_QUERY = "UI_TOOLS_MENU_RUN_QUERY";
        public const string UI_CONTEXT_MENU_NEW = "UI_CONTEXT_MENU_NEW";
        public const string UI_CONTEXT_MENU_EDIT = "UI_CONTEXT_MENU_EDIT";
        public const string UI_CONTEXT_MENU_CLONE = "UI_CONTEXT_MENU_CLONE";
        public const string UI_CONTEXT_MENU_DELETE = "UI_CONTEXT_MENU_DELETE";
        public const string UI_OPTIONS_PAGE_CONNECTIONS = "UI_OPTIONS_PAGE_CONNECTIONS";
        public const string UI_OPTIONS_PAGE_SQLMETAL = "UI_OPTIONS_PAGE_SQLMETAL";
        public const string UI_TOOLBAR_SQL_CONNECTION = "UI_TOOLBAR_SQL_CONNECTION";
        public const string UI_TOOLBAR_SQL_CONNECTION_SELECT = "UI_TOOLBAR_SQL_CONNECTION_SELECT";
        public const string UI_TOOLBAR_SQL_RUN_QUERY = "UI_TOOLBAR_SQL_RUN_QUERY";
        public const string UI_TOOLBAR_SQL_EXTRACT_DBML = "UI_TOOLBAR_SQL_EXTRACT_DBML";
    }
}
 