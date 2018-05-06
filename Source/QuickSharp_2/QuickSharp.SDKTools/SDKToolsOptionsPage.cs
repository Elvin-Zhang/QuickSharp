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

namespace QuickSharp.SDKTools
{
    public class SDKToolsOptionsPage : OptionsPage
    {
        private SettingsManager _settingsManager;
        private Label _ildasmPathLabel;
        private TextBox _ildasmPathTextBox;
        private Label _clrdbgPathLabel;
        private TextBox _clrdbgPathTextBox;
        private Label _dexplorePathLabel;
        private TextBox _dexplorePathTextBox;
        private Label _dexploreArgsLabel;
        private TextBox _dexploreArgsTextBox;
        private Label _pathWarningLabel;

        #region From Control Names

        public const string m_ildasmPathLabel = "ildasmPathLabel";
        public const string m_ildasmPathTextBox = "ildasmPathTextBox";
        public const string m_clrdbgPathLabel = "clrdbgPathLabel";
        public const string m_clrdbgPathTextBox = "clrdbgPathTextBox";
        public const string m_dexplorePathLabel = "dexplorePathLabel";
        public const string m_dexplorePathTextBox = "dexplorePathTextBox";
        public const string m_dexploreArgsLabel = "dexploreArgsLabel";
        public const string m_dexploreArgsTextBox = "dexploreArgsTextBox";
        public const string m_pathWarningLabel = "pathWarningLabel";

        #endregion

        public SDKToolsOptionsPage()
        {
            Name = Constants.UI_OPTIONS_SDK_TOOLS_PAGE;
            PageText = Resources.OptionsPageText;
            GroupText = Resources.OptionsGroupText;

            _ildasmPathLabel = new Label();
            _ildasmPathTextBox = new TextBox();
            _clrdbgPathLabel = new Label();
            _clrdbgPathTextBox = new TextBox();
            _dexplorePathLabel = new Label();
            _dexplorePathTextBox = new TextBox();
            _dexploreArgsLabel = new Label();
            _dexploreArgsTextBox = new TextBox();
            _pathWarningLabel = new Label();

            Controls.Add(_ildasmPathLabel);
            Controls.Add(_ildasmPathTextBox);
            Controls.Add(_clrdbgPathLabel);
            Controls.Add(_clrdbgPathTextBox);
            Controls.Add(_dexplorePathLabel);
            Controls.Add(_dexplorePathTextBox);
            Controls.Add(_dexploreArgsLabel);
            Controls.Add(_dexploreArgsTextBox);
            Controls.Add(_pathWarningLabel);

            #region Form Layout

            _ildasmPathLabel.AutoSize = true;
            _ildasmPathLabel.Location = new System.Drawing.Point(0, 0);
            _ildasmPathLabel.Name = m_ildasmPathLabel;
            _ildasmPathLabel.TabIndex = 0;
            _ildasmPathLabel.Text = Resources.OptionsIldasmPathLabel;

            _ildasmPathTextBox.Location = new System.Drawing.Point(3, 16);
            _ildasmPathTextBox.Name = m_ildasmPathTextBox;
            _ildasmPathTextBox.Size = new System.Drawing.Size(424, 20);
            _ildasmPathTextBox.TabIndex = 1;
 
            _clrdbgPathLabel.AutoSize = true;
            _clrdbgPathLabel.Location = new System.Drawing.Point(0, 46);
            _clrdbgPathLabel.Name = m_clrdbgPathLabel;
            _clrdbgPathLabel.Size = new System.Drawing.Size(105, 13);
            _clrdbgPathLabel.TabIndex = 2;
            _clrdbgPathLabel.Text = Resources.OptionsClrDbgPathLabel;

            _clrdbgPathTextBox.Location = new System.Drawing.Point(3, 62);
            _clrdbgPathTextBox.Name = m_clrdbgPathTextBox;
            _clrdbgPathTextBox.Size = new System.Drawing.Size(424, 20);
            _clrdbgPathTextBox.TabIndex = 3;

            _dexplorePathLabel.AutoSize = true;
            _dexplorePathLabel.Location = new System.Drawing.Point(0, 92);
            _dexplorePathLabel.Name = m_dexplorePathLabel;
            _dexplorePathLabel.TabIndex = 4;
            _dexplorePathLabel.Text = Resources.OptionsDexplorePathLabel;

            _dexplorePathTextBox.Location = new System.Drawing.Point(3, 108);
            _dexplorePathTextBox.Name = m_dexplorePathTextBox;
            _dexplorePathTextBox.Size = new System.Drawing.Size(424, 20);
            _dexplorePathTextBox.TabIndex = 5;

            _dexploreArgsLabel.AutoSize = true;
            _dexploreArgsLabel.Location = new System.Drawing.Point(0, 138);
            _dexploreArgsLabel.Name = m_dexploreArgsLabel;
            _dexploreArgsLabel.TabIndex = 6;
            _dexploreArgsLabel.Text = Resources.OptionsDexploreArgsLabel;

            _dexploreArgsTextBox.Location = new System.Drawing.Point(3, 154);
            _dexploreArgsTextBox.Name = m_dexploreArgsTextBox;
            _dexploreArgsTextBox.Size = new System.Drawing.Size(424, 20);
            _dexploreArgsTextBox.TabIndex = 7;

            _pathWarningLabel.AutoSize = true;
            _pathWarningLabel.Location = new System.Drawing.Point(0, 184);
            _pathWarningLabel.Name = m_pathWarningLabel;
            _pathWarningLabel.TabIndex = 8;
            _pathWarningLabel.ForeColor = Color.Red;
            _pathWarningLabel.Text = String.Empty;
            _pathWarningLabel.Visible = false;

            #endregion

            // Update the form
            _settingsManager = SettingsManager.GetInstance();
            _ildasmPathTextBox.Text = _settingsManager.IldasmPath;
            _clrdbgPathTextBox.Text = _settingsManager.ClrDbgPath;
            _dexplorePathTextBox.Text = _settingsManager.DexplorePath;
            _dexploreArgsTextBox.Text = _settingsManager.DexploreArgs;

            /*
             * Check the paths exists.
             */

            string warning = String.Empty;

            if (!File.Exists(_ildasmPathTextBox.Text))
                warning = Resources.OptionsIldasmPathWarning + "\r\n";

            if (!File.Exists(_clrdbgPathTextBox.Text))
                warning += Resources.OptionsClrDbgPathWarning + "\r\n";

            if (!File.Exists(_dexplorePathTextBox.Text))
                warning += Resources.OptionsDexplorePathWarning;

            if (warning != String.Empty)
            {
                _pathWarningLabel.Text = warning;
                _pathWarningLabel.Visible = true;
            }
        }

        public override void Save()
        {
            _settingsManager.IldasmPath = _ildasmPathTextBox.Text.Trim();
            _settingsManager.ClrDbgPath = _clrdbgPathTextBox.Text.Trim();
            _settingsManager.DexplorePath = _dexplorePathTextBox.Text.Trim();
            _settingsManager.DexploreArgs = _dexploreArgsTextBox.Text.Trim();
            _settingsManager.Save();
        }
    }
}
