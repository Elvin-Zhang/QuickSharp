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

namespace QuickSharp.Cassini
{
    public class WebServerOptionsPage : OptionsPage
    {
        private SettingsManager _settingsManager;
        private Label _serverPathLabel;
        private TextBox _serverPathTextBox;
        private Label _serverPortLabel;
        private TextBox _serverPortTextBox;
        private Label _serverRootLabel;
        private TextBox _serverRootTextBox;
        private Label _serverPathWarningLabel;

        #region Form Control Names

        public const string m_serverPathLabel = "serverPathLabel";
        public const string m_serverPathTextBox = "serverPathTextBox";
        public const string m_serverPortLabel = "serverPortLabel";
        public const string m_serverPortTextBox = "serverPortTextBox";
        public const string m_serverRootLabel = "serverRootLabel";
        public const string m_serverRootTextBox = "serverRootTextBox";
        public const string m_serverPathWarningLabel = "serverPathWarningLabel";

        #endregion

        public WebServerOptionsPage()
        {
            Name = Constants.UI_OPTIONS_WEB_SERVER_PAGE;
            PageText = Resources.OptionsPageText;
            GroupText = Resources.OptionsGroupText;

            _serverPathLabel = new Label();
            _serverPathTextBox = new TextBox();
            _serverPortLabel = new Label();
            _serverPortTextBox = new TextBox();
            _serverRootLabel = new Label();
            _serverRootTextBox = new TextBox();
            _serverPathWarningLabel = new Label();

            Controls.Add(_serverPathLabel);
            Controls.Add(_serverPathTextBox);
            Controls.Add(_serverPortLabel);
            Controls.Add(_serverPortTextBox); 
            Controls.Add(_serverRootLabel);
            Controls.Add(_serverRootTextBox);
            Controls.Add(_serverPathWarningLabel);

            #region Form Layout

            _serverPathLabel.AutoSize = true;
            _serverPathLabel.Location = new Point(0, 0);
            _serverPathLabel.Name = m_serverPathLabel;
            _serverPathLabel.TabIndex = 0;
            _serverPathLabel.Text = Resources.OptionsServerPathLabel;

            _serverPathTextBox.Location = new Point(3, 16);
            _serverPathTextBox.Name = m_serverPathTextBox;
            _serverPathTextBox.Size = new Size(424, 20);
            _serverPathTextBox.TabIndex = 1;
 
            _serverPortLabel.AutoSize = true;
            _serverPortLabel.Location = new Point(0, 46);
            _serverPortLabel.Name = m_serverPortLabel;
            _serverPortLabel.TabIndex = 2;
            _serverPortLabel.Text = Resources.OptionsServerPortLabel;

            _serverPortTextBox.Location = new Point(3, 62);
            _serverPortTextBox.Name = m_serverPortTextBox;
            _serverPortTextBox.Size = new Size(424, 20);
            _serverPortTextBox.TabIndex = 3;

            _serverRootLabel.AutoSize = true;
            _serverRootLabel.Location = new Point(0, 92);
            _serverRootLabel.Name = m_serverRootLabel;
            _serverRootLabel.TabIndex = 4;
            _serverRootLabel.Text = Resources.OptionsServerRootLabel;

            _serverRootTextBox.Location = new Point(3, 108);
            _serverRootTextBox.Name = m_serverRootTextBox;
            _serverRootTextBox.Size = new Size(424, 20);
            _serverRootTextBox.TabIndex = 5;

            _serverPathWarningLabel.AutoSize = true;
            _serverPathWarningLabel.Location = new Point(0, 138);
            _serverPathWarningLabel.Name = m_serverPathWarningLabel;
            _serverPathWarningLabel.TabIndex = 6;
            _serverPathWarningLabel.ForeColor = Color.Red;
            _serverPathWarningLabel.Text = Resources.OptionsServerPathWarningLabel;
            _serverPathWarningLabel.Visible = false;

            #endregion

            // Update the form
            _settingsManager = SettingsManager.GetInstance();
            _serverPathTextBox.Text = _settingsManager.ServerPath;
            _serverPortTextBox.Text = _settingsManager.ServerPort;
            _serverRootTextBox.Text = _settingsManager.ServerRoot;

            if (!File.Exists(_serverPathTextBox.Text))
                _serverPathWarningLabel.Visible = true;
        }

        public override void Save()
        {
            _settingsManager.ServerPath = _serverPathTextBox.Text.Trim();
            _settingsManager.ServerPort = _serverPortTextBox.Text.Trim();
            _settingsManager.ServerRoot = _serverRootTextBox.Text.Trim();
            _settingsManager.Save();
        }
    }
}
