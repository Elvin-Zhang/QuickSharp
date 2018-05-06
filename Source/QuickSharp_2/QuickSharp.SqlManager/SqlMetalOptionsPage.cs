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
using System.Drawing;
using System.Windows.Forms;
using QuickSharp.Core;

namespace QuickSharp.SqlManager
{
    public class SqlMetalOptionsPage : OptionsPage
    {
        private IPersistenceManager _persistenceManager;
        private Label _sqlMetalPathLabel;
        private TextBox _sqlMetalPathTextBox;
        private Label _sqlMetalPathWarningLabel;

        #region Form Control Names

        public const string m_sqlMetalPathLabel = "sqlMetalPathLabel";
        public const string m_sqlMetalPathTextBox = "sqlMetalPathTextBox";
        public const string m_sqlMetalPathWarningLabel = "sqlMetalPathWarningLabel";

        #endregion

        public SqlMetalOptionsPage()
        {
            Name = Constants.UI_OPTIONS_PAGE_SQLMETAL;
            PageText = Resources.OptionsPageTextSqlMetal;
            GroupText = Resources.OptionsGroupText;

            _sqlMetalPathLabel = new Label();
            _sqlMetalPathTextBox = new TextBox();
            _sqlMetalPathWarningLabel = new Label();

            Controls.Add(_sqlMetalPathLabel);
            Controls.Add(_sqlMetalPathTextBox);
            Controls.Add(_sqlMetalPathWarningLabel);

            #region Form Layout

            _sqlMetalPathLabel.AutoSize = true;
            _sqlMetalPathLabel.Location = new System.Drawing.Point(0, 0);
            _sqlMetalPathLabel.Name = m_sqlMetalPathLabel;
            _sqlMetalPathLabel.TabIndex = 0;
            _sqlMetalPathLabel.Text = Resources.SqlMetalToolPath;

            _sqlMetalPathTextBox.Location = new System.Drawing.Point(3, 16);
            _sqlMetalPathTextBox.Name = m_sqlMetalPathTextBox;
            _sqlMetalPathTextBox.Size = new System.Drawing.Size(424, 20);
            _sqlMetalPathTextBox.TabIndex = 1;

            _sqlMetalPathWarningLabel.AutoSize = true;
            _sqlMetalPathWarningLabel.Location = new System.Drawing.Point(0, 46);
            _sqlMetalPathWarningLabel.Name = m_sqlMetalPathWarningLabel;
            _sqlMetalPathWarningLabel.TabIndex = 2;
            _sqlMetalPathWarningLabel.ForeColor = Color.Red;
            _sqlMetalPathWarningLabel.Text = Resources.SqlMetalToolPathWarning;
            _sqlMetalPathWarningLabel.Visible = false;

            #endregion
            
            _persistenceManager = ApplicationManager.GetInstance().
                GetPersistenceManager(Constants.PLUGIN_NAME);

            // Update the form
            _sqlMetalPathTextBox.Text = _persistenceManager.ReadString(
                Constants.KEY_SQL_METAL_PATH,
                Constants.SQL_METAL_DEFAULT_PATH);

            if (!File.Exists(_sqlMetalPathTextBox.Text))
                _sqlMetalPathWarningLabel.Visible = true;
        }

        public override void Save()
        {
            _persistenceManager.WriteString(
                Constants.KEY_SQL_METAL_PATH,
                _sqlMetalPathTextBox.Text.Trim());
        }
    }
}
