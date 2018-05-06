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

namespace QuickSharp.SQLiteManager
{
    public class SQLiteManagerOptionsPage : OptionsPage
    {
        private IPersistenceManager _persistenceManager;
        private Label _sqliteManagerPathLabel;
        private TextBox _sqliteMangerPathTextBox;
        private Label _sqliteManagerPathWarningLabel;

        #region From Control Names

        public const string m_sqliteManagerPathLabel = "sqliteManagerPathLabel";
        public const string m_sqliteMangerPathTextBox = "sqliteMangerPathTextBox";
        public const string m_sqliteManagerPathWarningLabel = "sqliteManagerPathWarningLabel";

        #endregion

        public SQLiteManagerOptionsPage()
        {
            Name = Constants.UI_OPTIONS_PAGE_SQLITE_MANAGER;
            PageText = Resources.OptionsPageText;
            GroupText = Resources.OptionsGroupText;

            _sqliteManagerPathLabel = new Label();
            _sqliteMangerPathTextBox = new TextBox();
            _sqliteManagerPathWarningLabel = new Label();

            Controls.Add(_sqliteManagerPathLabel);
            Controls.Add(_sqliteMangerPathTextBox);
            Controls.Add(_sqliteManagerPathWarningLabel);

            _sqliteManagerPathLabel.AutoSize = true;
            _sqliteManagerPathLabel.Location = new System.Drawing.Point(0, 0);
            _sqliteManagerPathLabel.Name = m_sqliteManagerPathLabel;
            _sqliteManagerPathLabel.TabIndex = 0;
            _sqliteManagerPathLabel.Text = Resources.SQLiteManagerPathLabel;

            _sqliteMangerPathTextBox.Location = new System.Drawing.Point(3, 16);
            _sqliteMangerPathTextBox.Name = m_sqliteMangerPathTextBox;
            _sqliteMangerPathTextBox.Size = new System.Drawing.Size(424, 20);
            _sqliteMangerPathTextBox.TabIndex = 1;

            _sqliteManagerPathWarningLabel.AutoSize = true;
            _sqliteManagerPathWarningLabel.Location = new System.Drawing.Point(0, 46);
            _sqliteManagerPathWarningLabel.Name = m_sqliteManagerPathWarningLabel;
            _sqliteManagerPathWarningLabel.TabIndex = 2;
            _sqliteManagerPathWarningLabel.ForeColor = Color.Red;
            _sqliteManagerPathWarningLabel.Text = Resources.SQLiteManagerPathWarningLabel;
            _sqliteManagerPathWarningLabel.Visible = false;

            _persistenceManager = ApplicationManager.GetInstance().
                GetPersistenceManager(Constants.PLUGIN_NAME);

            _sqliteMangerPathTextBox.Text = _persistenceManager.ReadString(
                Constants.KEY_SQLITE_MANAGER_PATH, String.Empty);

            if (!File.Exists(_sqliteMangerPathTextBox.Text))
                _sqliteManagerPathWarningLabel.Visible = true;
        }

        public override void Save()
        {
            _persistenceManager.WriteString(
                Constants.KEY_SQLITE_MANAGER_PATH,
                _sqliteMangerPathTextBox.Text.Trim());
        }
    }
}
