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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using QuickSharp.Core;
using QuickSharp.Output;

namespace QuickSharp.SqlManager
{
    public partial class SqlMetalForm : Form
    {
        private ApplicationManager _applicationManager;
        private IPersistenceManager _persistenceManager;
        private SqlConnectionManager _sqlConnectionManager;
        private OutputForm _output;
        private SqlDataProvider _provider;
        private string _connectionString;
        private string _sqlMetalPath;
        private bool _useCompactEdition;

        public SqlMetalForm(SqlConnection selectedConnection)
        {
            InitializeComponent();

            _applicationManager = 
                QuickSharp.Core.ApplicationManager.GetInstance();
            
            _persistenceManager = _applicationManager.
                GetPersistenceManager(Constants.PLUGIN_NAME);
            
            _sqlConnectionManager = 
                SqlConnectionManager.GetInstance();

            _provider = selectedConnection.Provider;
            _connectionString = selectedConnection.ConnectionString;

            _useCompactEdition = _sqlConnectionManager.
                ConnectionIsSqlServerCe(selectedConnection);

            _output = _applicationManager.GetDockedForm(
                QuickSharp.Output.Constants.DOCKED_FORM_KEY)
                as OutputForm;

            /*
             * Get the tool path.
             */

            _sqlMetalPath = _persistenceManager.ReadString(
                Constants.KEY_SQL_METAL_PATH,
                Constants.SQL_METAL_DEFAULT_PATH);

            /*
             * Populate the filename with the database name.
             */

            _filenameTextBox.Text = GetOutputFilename();

            /*
             * Update the UI
             */

            Text = Resources.SqlMetalFormTitle;
            _filenameLabel.Text = Resources.SqlMetalFormFilename;
            _includeGroupBox.Text = Resources.SqlMetalFormIncludeGroup;
            _viewsCheckBox.Text = Resources.SqlMetalFormViews;
            _functionsCheckBox.Text = Resources.SqlMetalFormFunctions;
            _sprocsCheckBox.Text = Resources.SqlMetalFormSprocs;
            _okButton.Text = Resources.SqlMetalFormOk;
            _cancelButton.Text = Resources.SqlMetalFormCancel;

            CheckToolAvailable();

            /*
             * Allow client applications to modify form.
             */

            SqlMetalFormProxy.GetInstance().
                UpdateFormControls(Controls);
        }

        #region Form Event Handlers

        private void OkButton_Click(object sender, EventArgs e)
        {
            string outputFilename = _filenameTextBox.Text.Trim();

            /*
             * Check the filename is valid.
             */

            if (FileTools.FilenameIsInvalid(outputFilename))
            {
                _filenameTextBox.BackColor = Color.Yellow;
                return;
            }

            /*
             * Confirm overwrite if exists.
             */

            if (File.Exists(outputFilename) &&
                MessageBox.Show(String.Format(
                        Resources.SqlMetalOverwriteMessage,
                        outputFilename),
                    Resources.SqlMetalOverwriteTitle,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.No)
                return;

            /*
             * Run SqlMetal.
             */

            MainForm mainForm = _applicationManager.MainForm;

            try
            {
                Enabled = false;

                mainForm.SetStatusBarMessage(
                    Resources.StatusBarExtractStarted);

                RunSqlMetal(outputFilename);
            }
            finally
            {
                Enabled = true;

                mainForm.SetStatusBarMessage(
                    Resources.StatusBarExtractCompleted);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        #endregion

        #region Run SqlMetal

        private void RunSqlMetal(string filename)
        {
            string sqlMetalArgs;

            if (_useCompactEdition)
            {
                string dbname = _sqlConnectionManager.
                    GetSqlServrCeDbName(_connectionString, true);

                dbname = Path.ChangeExtension(dbname, ".sdf");

                if (!File.Exists(dbname))
                {
                    MessageBox.Show(String.Format("{0}: {1}",
                            Resources.DbFileNotFoundMessage,
                            dbname),
                        Resources.DbFileNotFoundTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                sqlMetalArgs = String.Format("/dbml:\"{0}\" \"{1}\"",
                    filename, dbname);
            }
            else
            {
                sqlMetalArgs = String.Format("/conn:\"{0}\" /dbml:\"{1}\"",
                    _connectionString, filename);
            }

            if (_viewsCheckBox.Checked) sqlMetalArgs += " /views";
            if (_functionsCheckBox.Checked) sqlMetalArgs += " /functions";
            if (_sprocsCheckBox.Checked) sqlMetalArgs += " /sprocs";

            RunProcessContext context = new RunProcessContext();
            context.ExePath = _sqlMetalPath;
            context.ProcessArgs = sqlMetalArgs;
            context.HeaderText =
                String.Format("{0}: ", Resources.SqlMetalStarted);
            context.FooterText =
                String.Format("{0}: ", Resources.SqlMetalComplete);

            _output.ClearOutputViews();
            _output.Text = Resources.OutputWindowRunSqlMetal; 
            _output.RunProcessInternal(context);

            _applicationManager.NotifyFileSystemChange();
        }

        #endregion

        #region Helpers

        private void CheckToolAvailable()
        {
            bool enabled = File.Exists(_sqlMetalPath);

            if (enabled)
                _introLabel.Text = Resources.SqlMetalFormIntro;
            else
                _introLabel.Text = Resources.SqlMetalFormNoTool;

            _filenameLabel.Enabled = enabled;
            _filenameTextBox.Enabled = enabled;
            _includeGroupBox.Enabled = enabled;
            _okButton.Enabled = enabled;
        }

        private string GetOutputFilename()
        {
            string dbname = String.Empty;

            if (_useCompactEdition)
                dbname = _sqlConnectionManager.
                    GetSqlServrCeDbName(_connectionString, false);
            else
                dbname = _sqlConnectionManager.
                    GetSqlServerDbName(_connectionString);

            if (dbname != String.Empty)
                return dbname + ".dbml";
            else
                return "untitled.dbml";
        }

        #endregion
    }
}
