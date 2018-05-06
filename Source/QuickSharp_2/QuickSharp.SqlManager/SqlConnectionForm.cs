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
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuickSharp.SqlManager
{
    public partial class SqlConnectionForm : Form
    {
        private SqlConnectionManager _sqlConnectionManager;
        private Dictionary<String, SqlConnection> _sqlConnections;
        private string _connectionId;

        public SqlConnectionForm(
            Dictionary<String, SqlConnection> sqlConnections,
            string id)
        {
            InitializeComponent();

            _sqlConnectionManager = SqlConnectionManager.GetInstance();
            _sqlConnections = sqlConnections;
            _connectionId = id;

            foreach (SqlDataProvider provider in
                _sqlConnectionManager.SqlDataProviders.Values)
                _providerComboBox.Items.Add(provider);

            if (_providerComboBox.Items.Count > 0)
                _providerComboBox.SelectedIndex = 0;

            if (_connectionId != null)
            {
                if (!_sqlConnections.ContainsKey(id))
                    throw new Exception("Invalid connection ID");

                SqlConnection connection = _sqlConnections[id];
                _nameTextBox.Text = connection.Name;
                _connectionStringTextBox.Text =
                    connection.ConnectionStringForEditing;

                SelectedProviderByInvariantName(
                    connection.Provider.InvariantName);
            }

            // Allow client applications to modify the form.
            SqlConnectionFormProxy.GetInstance().
                UpdateFormControls(Controls);
        }

        #region Helpers

        private void SelectedProviderByInvariantName(string name)
        {
            for (int i = 0; i < _providerComboBox.Items.Count; i++)
            {
                SqlDataProvider provider = 
                    _providerComboBox.Items[i] as SqlDataProvider;

                if (provider == null)
                    throw new Exception("Invalid data provider");

                if (provider.InvariantName == name)
                {
                    _providerComboBox.SelectedIndex = i;
                    return;
                }
            }
        }

        #endregion

        #region Event Handlers

        private void OkButton_Click(object sender, EventArgs e)
        {
            _nameTextBox.BackColor = Color.Empty;
            _connectionStringTextBox.BackColor = Color.Empty;

            string name = _nameTextBox.Text.Trim();
            string connectionString = _connectionStringTextBox.Text.Trim();

            SqlDataProvider provider = 
                _providerComboBox.SelectedItem as SqlDataProvider;

            if (provider == null)
                throw new Exception(
                    Resources.ConnectionInvalidProvider);
            
            bool validated = true;

            if (name == String.Empty)
            {
                validated = false;
                _nameTextBox.BackColor = Color.Yellow;
            }

            if (connectionString == String.Empty)
            {
                validated = false;
                _connectionStringTextBox.BackColor = Color.Yellow;
            }

            if (validated)
            {
                if (_connectionId == null)
                    _connectionId = Guid.NewGuid().ToString();

                SqlConnection cnn = new SqlConnection(
                    _connectionId,
                    name,
                    connectionString,
                    provider);

                _sqlConnections[_connectionId] = cnn;

                ModuleProxy.GetInstance().Module.DisableConnection();

                DialogResult = DialogResult.OK;
            }
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _testButton.Enabled = false;
                _okButton.Enabled = false;
                _cancelButton.Enabled = false;

                string connectionString =
                    _connectionStringTextBox.Text.Trim();

                if (connectionString == String.Empty)
                    throw new Exception(
                        Resources.ConnectionTestNoString);

                SqlDataProvider provider =
                    _providerComboBox.SelectedItem as SqlDataProvider;

                if (provider == null)
                    throw new Exception(
                        Resources.ConnectionInvalidProvider);

                _sqlConnectionManager.TestConnection(
                    provider, connectionString);

                MessageBox.Show(Resources.ConnectionTestSuccess,
                    Resources.ConnectionTestTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}:\r\n{1}",
                        Resources.ConnectionTestFailure, ex.Message),
                    Resources.ConnectionTestTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                _testButton.Enabled = true;
                _okButton.Enabled = true;
                _cancelButton.Enabled = true;
            }
        }

        #endregion
    }
}
