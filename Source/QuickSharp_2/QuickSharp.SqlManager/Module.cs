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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;
using QuickSharp.Core;
using QuickSharp.Output;
using QuickSharp.SqlEditor;

namespace QuickSharp.SqlManager
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "159DBA3B-6C62-42CF-882C-BE6D5C3CDA41";
        }

        public string GetName()
        {
            return "QuickSharp SQL Manager";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return
                "Provides support for ADO.NET connections to supported data " +
                "providers and SQL query management.";
        }

        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            deps.Add(new Plugin(QuickSharpPlugins.Output, "QuickSharp.Output", 1));
            deps.Add(new Plugin(QuickSharpPlugins.Editor, "QuickSharp.Editor", 1));
            deps.Add(new Plugin(QuickSharpPlugins.SqlEditor, "QuickSharp.SqlEditor", 1));
            return deps;
        }

        public void Activate(MainForm mainForm)
        {
            _mainForm = mainForm;
            ActivatePlugin();
        }

        #endregion

        private ApplicationManager _applicationManager;
        private PluginManager _pluginManager; 
        private SqlConnectionManager _sqlConnectionManager;
        private MainForm _mainForm;
        private OutputForm _output;
        private ToolStripMenuItem _toolsMenuRunQuery;
        private ToolStripButton _toolbarSqlConnection;
        private ToolStripDropDownButton _toolbarSqlConnectionSelect;
        private ToolStripButton _toolbarSqlRunQuery;
        private ToolStripButton _toolbarSqlExtractDbml;

        private void ActivatePlugin()
        {
            ModuleProxy.GetInstance().Module = this;

            _applicationManager = ApplicationManager.GetInstance();
            _pluginManager = PluginManager.GetInstance();
            _sqlConnectionManager = SqlConnectionManager.GetInstance();
            _sqlConnectionManager.Load();

            /*
             * Enable SqlMetal/DMBL extract features if client flag set.
             */

            bool enableSqlMetal = 
                _applicationManager.ClientProfile.HaveFlag(
                    ClientFlags.SqlManagerEnableDbmlExtract);

            _output = _applicationManager.GetDockedForm(
                QuickSharp.Output.Constants.DOCKED_FORM_KEY) as
                OutputForm;

            _toolsMenuRunQuery = MenuTools.CreateMenuItem(
                Constants.UI_TOOLS_MENU_RUN_QUERY,
                Resources.MainToolsMenuRunQuery,
                Resources.RunQuery,
                Keys.F5, null, UI_TOOLBAR_RUN_SQL_QUERY_Click,
                true);

            ToolStripMenuItem toolsMenu = _mainForm.GetMenuItemByName(
                QuickSharp.Core.Constants.UI_TOOLS_MENU);

            int index = toolsMenu.DropDownItems.IndexOfKey(
                QuickSharp.Core.Constants.UI_TOOLS_MENU_OPTIONS);

            /*
             * If menu not found insert at the top.
             */

            if (index == -1) index = 0;

            toolsMenu.DropDownItems.Insert(index, _toolsMenuRunQuery);

            /*
             * Create toolbar buttons.
             */

            _toolbarSqlConnection = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_SQL_CONNECTION,
                Resources.ToolbarSqlConnection,
                Resources.SqlConnection,
                UI_TOOLBAR_SQL_CONNECTION_Click,
                true);

            _toolbarSqlConnection.ToolTipText =
                Resources.ToolbarActivateConnection;

            _toolbarSqlConnectionSelect = 
                MenuTools.CreateToolbarDropDownButton(
                    Constants.UI_TOOLBAR_SQL_CONNECTION_SELECT, null);

            _toolbarSqlConnectionSelect.DropDownOpening += 
                new EventHandler(
                    ToolbarSqlConnectionSelect_DropDownOpening);

            _toolbarSqlRunQuery = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_SQL_RUN_QUERY,
                Resources.ToolbarSqlRunQuery,
                Resources.RunQuery,
                UI_TOOLBAR_RUN_SQL_QUERY_Click);

            _toolbarSqlExtractDbml = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_SQL_EXTRACT_DBML,
                Resources.ToolbarSqlExtractDbml,
                Resources.ExtractDbml,
                UI_TOOLBAR_SQL_EXTRACT_DBML_Click);

            _mainForm.MainToolbar.Items.Add(_toolbarSqlConnection);
            _mainForm.MainToolbar.Items.Add(_toolbarSqlConnectionSelect);
            _mainForm.MainToolbar.Items.Add(_toolbarSqlRunQuery);
            
            if (enableSqlMetal)
                _mainForm.MainToolbar.Items.Add(_toolbarSqlExtractDbml);

            _mainForm.FormClosing +=
                new FormClosingEventHandler(MainForm_FormClosing);
            
            _mainForm.ClientWindow.ActiveDocumentChanged +=
                new EventHandler(ClientWindow_ActiveDocumentChanged);

            _applicationManager.RegisterOptionsPageFactory(
                delegate { return new SqlConnectionsOptionsPage(); });

            if (enableSqlMetal)
                _applicationManager.RegisterOptionsPageFactory(
                    delegate { return new SqlMetalOptionsPage(); });

            _applicationManager.FileSystemChange +=
                new MessageHandler(UpdateUI);

            /*
             * We don't have any build tools but we want SQL files
             * recognised as source files. Register with the
             * non-tool source files list in the application store.
             */

            ApplicationStorage appStore = ApplicationStorage.GetInstance();

            /*
             * We have no dependency on the BuildTools plugin so we
             * can't assume the non-tool source list has been created.
             * Use the Get method to create the list if it doesn't exist.
             */

            List<String> nonToolSourceTypes = appStore.Get(
                Constants.APP_STORE_KEY_NON_TOOL_SOURCE_TYPES,
                typeof(List<String>)) as List<String>;

            if (nonToolSourceTypes != null)
                nonToolSourceTypes.Add(Constants.DOCUMENT_TYPE_SQL);

            UpdateUI();
        }
 
        #region Event Handlers

        private void MainForm_FormClosing(
            object sender, FormClosingEventArgs e)
        {
            _sqlConnectionManager.Save();
        }

        private void ClientWindow_ActiveDocumentChanged(
            object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void ToolbarSqlConnectionSelect_DropDownOpening(
            object sender, EventArgs e)
        {
            _toolbarSqlConnectionSelect.Enabled = false;
            _toolbarSqlConnectionSelect.DropDownItems.Clear();

            ThemeFlags flags = _applicationManager.
                ClientProfile.ThemeFlags;
            
            bool applyTheme = flags != null && 
                flags.MenuForeColor != Color.Empty;

            foreach (SqlConnection cnn in
                _sqlConnectionManager.SqlConnections.Values)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = cnn.Name;
                item.Tag = cnn.ID;
                item.Click += new EventHandler(ConnectionMenu_Click);

                // Apply theme color if available.
                if (applyTheme) item.ForeColor = flags.MenuForeColor;

                if (_sqlConnectionManager.SelectedConnectionId == cnn.ID)
                    item.Checked = true;

                _toolbarSqlConnectionSelect.DropDownItems.Add(item);
            }

            if (_toolbarSqlConnectionSelect.DropDownItems.Count > 0)
                _toolbarSqlConnectionSelect.Enabled = true;
        }

        private void ConnectionMenu_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null) return;

            string id = item.Tag as string;
            if (id == null) return;

            if (id != _sqlConnectionManager.SelectedConnectionId)
            {
                _sqlConnectionManager.SelectedConnectionId = id;
                _toolbarSqlConnection.Checked = false;
                _sqlConnectionManager.ConnectionIsActive = false;
            }

            NotifyConnectionChange();

            UpdateUI();
        }

        private void UI_TOOLBAR_SQL_CONNECTION_Click(
            object sender, EventArgs e)
        {
            if (_toolbarSqlConnection.Checked)
                DisableConnection();
            else
                EnableConnection();
        }

        private void UI_TOOLBAR_RUN_SQL_QUERY_Click(
            object sender, EventArgs e)
        {
            try
            {
                _mainForm.Cursor = Cursors.WaitCursor;
                RunQuery();
            }
            finally
            {
                _mainForm.Cursor = Cursors.Default;
            }
        }

        private void UI_TOOLBAR_SQL_EXTRACT_DBML_Click(
            object sender, EventArgs e)
        {
            SqlMetalForm smf = new SqlMetalForm(
                _sqlConnectionManager.SelectedConnection);
            
            smf.ShowDialog();
        }

        #endregion

        #region Update Methods

        public void EnableConnection()
        {
            bool isActive = ActivateConnection();

            _toolbarSqlConnection.Checked = isActive;
            _sqlConnectionManager.ConnectionIsActive = isActive;

            NotifyConnectionChange();
            UpdateUI();
        }

        public void DisableConnection()
        {
            _toolbarSqlConnection.Checked = false;
            _sqlConnectionManager.ConnectionIsActive = false;
            NotifyConnectionChange();
            UpdateUI();
        }

        public void UpdateUI()
        {
            _toolbarSqlConnection.Enabled = true;
            _toolbarSqlConnectionSelect.Enabled = true;

            if (_sqlConnectionManager.SelectedConnectionId == String.Empty ||
                _sqlConnectionManager.SelectedConnection == null)
            {
                _toolbarSqlConnection.Enabled = false;
                _toolbarSqlConnection.Checked = false;
            }

            if (_sqlConnectionManager.SqlConnections.Values.Count == 0)
                _toolbarSqlConnectionSelect.Enabled = false;

            if (_toolbarSqlConnection.Checked)
            {
                _toolbarSqlConnection.ToolTipText =
                    String.Format("{0}: {1}",
                        Resources.ToolbarActiveConnection,
                        _sqlConnectionManager.SelectedConnection.Name);
            }
            else
            {
                if (_sqlConnectionManager.SelectedConnection == null)
                    _toolbarSqlConnection.ToolTipText =
                        Resources.ToolbarActivateConnection;
                else
                    _toolbarSqlConnection.ToolTipText =
                        String.Format("{0}: {1}",
                            Resources.ToolbarActivateConnection,
                            _sqlConnectionManager.SelectedConnection.Name);
            }
            
            /*
             * Reset toolbar buttons.
             */

            _toolsMenuRunQuery.Enabled = false;
            _toolbarSqlRunQuery.Enabled = false;
            _toolbarSqlExtractDbml.Enabled = false;

            /*
             * Set accessibility of query button - must have a
             * SqlEditForm and a .sql document.
             */

            DocumentType documentType = GetActiveDocumentType();

            if (ActiveEditorIsSql() && documentType != null)
            {
                if (documentType.Matches(
                    QuickSharp.SqlEditor.Constants.DOCUMENT_TYPE_SQL) &&
                    _toolbarSqlConnection.Checked)
                {
                    _toolsMenuRunQuery.Enabled = true;
                    _toolbarSqlRunQuery.Enabled = true;
                }
            }

            /*
             * Set accessibility of DBML extract - must have active
             * connection and SQL Server data provider.
             * 
             * SQL Server Compact edition is allowed but as the invariant
             * name contains the version number support is fixed to version
             * 3.5. Speculative support is provided for SSCE 4.0 but the
             * invariant name is an educated guess; if it's wrong it will
             * do no harm. 
             */

            bool enableSqlMetal = _toolbarSqlConnection.Checked;

            if (enableSqlMetal)
            {
                SqlConnection conn = _sqlConnectionManager.SelectedConnection;

                if (!_sqlConnectionManager.ConnectionIsSqlServer(conn) &&
                    !_sqlConnectionManager.ConnectionIsSqlServerCe(conn))
                    enableSqlMetal = false;
            }

            _toolbarSqlExtractDbml.Enabled = enableSqlMetal;
        }

        private void NotifyConnectionChange()
        {
            try
            {
                _mainForm.Cursor = Cursors.WaitCursor;

                _sqlConnectionManager.NotifyConnectionChange();
            }
            finally
            {
                _mainForm.Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Helpers

        private SqlEditForm GetActiveDocument()
        {
            return _mainForm.ActiveDocument as SqlEditForm;
        }

        private DocumentType GetActiveDocumentType()
        {
            if (_mainForm.ActiveDocument == null) return null;
            FileInfo fi = new FileInfo(_mainForm.ActiveDocument.FileName);
            return new DocumentType(fi.Extension);
        }

        private string GetActiveDocumentContent()
        {
            if (_mainForm.ActiveDocument == null) return null;

            SqlEditForm document = _mainForm.ActiveDocument as SqlEditForm;
            if (document == null) return null;

            if (document.Editor.Selection.Length > 0)
                return document.Editor.Selection.Text;
            else
                return document.Editor.Text;
        }

        private bool ActiveEditorIsSql()
        {
            if (_mainForm.ActiveDocument == null) return false;
            return (_mainForm.ActiveDocument is SqlEditForm);
        }

        private string RemoveComments(string text)
        {
            text = text.Replace("\r\n", "\n");

            StringBuilder sb = new StringBuilder();
            int max = text.Length - 1;
            int i = 0;

            while (i < text.Length)
            {
                if (text[i] == '-' && i < max && text[i + 1] == '-')
                {
                    // Goto line end
                    while (i <= max && text[i] != '\n') i++;
                }
                else if (text[i] == '/' && i < max && text[i + 1] == '*')
                {
                    // Goto next '*/'
                    while (i < max)
                    {
                        if (text[i] == '*' && text[i + 1] == '/') break;
                        i++;
                    }
                    i += 2;
                }

                if (i <= max) sb.Append(text[i]);
                i++;
            }

            return sb.ToString();
        }

        private void WriteLog(string text)
        {
            if (_output == null) return;

            _output.AddLineToOutputView(text);
        }

        private void WriteError(string text)
        {
            if (_output == null) return;

            _output.AddLineToOutputView(text, Color.Red);
        }

        #endregion

        #region Activate Connection

        private bool ActivateConnection()
        {
            /*
             * Test the connection.
             */

            SqlConnection cnn = _sqlConnectionManager.SelectedConnection;
            if (cnn == null) return false;

            try
            {
                _mainForm.Cursor = Cursors.WaitCursor;

                _sqlConnectionManager.TestConnection(cnn);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}:\r\n{1}",
                        Resources.ConnectionTestFailure, ex.Message),
                    Resources.ConnectionTestTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                _sqlConnectionManager.SelectedConnectionId = String.Empty;
                _sqlConnectionManager.ConnectionIsActive = false;
            }
            finally
            {
                _mainForm.Cursor = Cursors.Default;
            }

            return true;
        }

        #endregion

        #region Run Query

        private List<string> GetCommands()
        {
            string queryText = GetActiveDocumentContent();
            if (queryText == null) return null;

            queryText = RemoveComments(queryText);
            queryText = queryText.Replace("\n", " ");

            List<string> commands = new List<string>();

            foreach (string s in queryText.Split(';'))
            {
                string command = s.Trim();
                if (command != String.Empty)
                    commands.Add(command);
            }

            if (commands.Count == 0)
                return null;
            else
                return commands;
        }

        private string GetCommandName(string command)
        {
            if (String.IsNullOrEmpty(command))
                return Resources.QueryUnknownCommand;

            string[] commandNames =
            {
                "select", "insert", "update", "delete", "create",
                "alter", "drop", "grant", "revoke", "exec"
            };

            string[] split = command.Split();
            string commandName = split[0].ToLower();

            if (Array.IndexOf(commandNames, commandName) != -1)
                return commandName;
            else
                return Resources.QueryUnknownCommand;
        }

        private void RunQuery()
        {
            SqlEditForm activeDocument = GetActiveDocument();
            if (activeDocument == null) return;

            string dataSetName = 
                Path.GetFileNameWithoutExtension(activeDocument.FileName);

            if (String.IsNullOrEmpty(dataSetName))
                dataSetName = Resources.QueryDataSetName;

            activeDocument.ClearData();
            _output.ClearOutputViews();

            _output.Text = Resources.OutputWindowRunQuery;
            _mainForm.SetStatusBarMessage(String.Empty);

            List<string> commands = GetCommands();
            if (commands == null) return;

            SqlConnection activeConnection = 
                _sqlConnectionManager.SelectedConnection;
            if (activeConnection == null) return;

            _output.Text = String.Format("{0}: {1}",
                Resources.OutputWindowRunQuery,
                activeConnection.Name);

            DbConnection cnn = null;
            DbTransaction trx = null;
            DbCommand cmd = null;
            DbDataAdapter dataAdapter = null;
            DataSet dataSet = new DataSet(dataSetName);

            WriteLog(String.Format("------ {0}: {1}",
                Resources.QueryStarted,
                activeDocument.FileName));

            _mainForm.SetStatusBarMessage(Resources.StatusBarQueryStarted);

            _mainForm.StatusBar.Refresh();

            string logMessage = Resources.QueryUnknownCommand;
            bool useTransaction = (commands.Count > 1);

            try
            {
                DbProviderFactory factory = 
                    DbProviderFactories.GetFactory(
                        activeConnection.Provider.InvariantName);

                cnn = factory.CreateConnection();
                cnn.ConnectionString = activeConnection.ConnectionString;
                cnn.Open();

                cmd = factory.CreateCommand();
                cmd.Connection = cnn;
                cmd.CommandText = String.Empty;

                if (useTransaction)
                {
                    trx = cnn.BeginTransaction();
                    cmd.Transaction = trx;

                    WriteLog(Resources.QueryBeginTransaction);
                }

                dataAdapter = factory.CreateDataAdapter();
                dataAdapter.SelectCommand = cmd;

                int commandCount = 1;

                foreach (string command in commands)
                {
                    cmd.CommandText = command;

                    string commandType = GetCommandName(command);

                    logMessage = commandType;

                    if (commandType == Resources.QuerySelectCommand)
                    {
                        int res = dataAdapter.Fill(dataSet,
                            String.Format(
                                Resources.QueryDataRowName,
                                commandCount++));

                        if (res < 0) res = 0;

                        WriteLog(String.Format(
                            Resources.QuerySelectSuccess,
                            logMessage, res, res == 1 ?
                                Resources.QueryRowSingular :
                                Resources.QueryRowPlural));
                    }
                    else
                    {
                        int res = cmd.ExecuteNonQuery();

                        if (res < 0) res = 0;

                        WriteLog(String.Format(
                            Resources.QueryNonSelectSuccess,
                            logMessage, res, res == 1 ? 
                                Resources.QueryRowSingular : 
                                Resources.QueryRowPlural));
                    }
                }

                if (useTransaction)
                {
                    trx.Commit();
                    WriteLog(Resources.QueryCommitTransaction);
                }

                _mainForm.SetStatusBarMessage(
                    Resources.StatusBarQuerySuccess); 

                activeDocument.DataSet = dataSet;
                activeDocument.ShowTable();
            }
            catch (Exception ex)
            {
                string errorMessage = 
                    ex.Message.Replace("\r\n", " ");
                
                WriteError(String.Format(
                    Resources.QueryErrors,
                    logMessage, errorMessage));

                _mainForm.SetStatusBarMessage(Resources.StatusBarQueryError); 

                if (useTransaction && trx != null)
                {
                    trx.Rollback();
                    WriteLog(Resources.QueryRollbackTransaction);
                }
            }
            finally
            {
                if (cnn != null) cnn.Dispose();
            }

            WriteLog(String.Format("------ {0}",
                Resources.QueryComplete));
        }

        #endregion
    }
}
