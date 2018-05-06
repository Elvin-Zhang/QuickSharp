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

namespace QuickSharp.BuildTools
{
    /// <summary>
    /// The build settings configuration page for the main options editor.
    /// </summary>
    public class BuildSettingsOptionsPage : OptionsPage
    {
        private BuildToolManager _buildToolManager;
        private GroupBox _settingsGroupBox;
        private CheckBox _alwaysBuildOnCompileCheckBox;
        private CheckBox _alwaysBuildOnRunCheckBox;

        #region Form Control Names

        /// <summary>
        /// Form control name.
        /// </summary>
        private const string m_settingsGroupBox = "settingsGroupBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        private const string m_alwaysBuildOnCompileCheckBox = "alwaysBuildOnCompileCheckBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        private const string m_alwaysBuildOnRunCheckBox = "alwaysBuildOnRunCheckBox";

        #endregion

        /// <summary>
        /// Create the options page.
        /// </summary>
        public BuildSettingsOptionsPage()
        {
            _buildToolManager = BuildToolManager.GetInstance();

            Name = Constants.UI_OPTIONS_BUILD_SETTINGS_PAGE;
            PageText = Resources.OptionsPageTextSettings;
            GroupText = Resources.OptionsGroupText;

            #region Form Layout

            _settingsGroupBox = new GroupBox();
            _settingsGroupBox.Location = new Point(0, 0);
            _settingsGroupBox.Name = m_settingsGroupBox;
            _settingsGroupBox.Size = new Size(430, 250);
            _settingsGroupBox.TabIndex = 0;
            _settingsGroupBox.TabStop = false;
            _settingsGroupBox.Text = Resources.OptionsPageTextSettings;

            _alwaysBuildOnCompileCheckBox = new CheckBox();
            _alwaysBuildOnCompileCheckBox.AutoSize = true;
            _alwaysBuildOnCompileCheckBox.Location = new Point(19, 22);
            _alwaysBuildOnCompileCheckBox.Name = m_alwaysBuildOnCompileCheckBox;
            _alwaysBuildOnCompileCheckBox.TabIndex = 0;
            _alwaysBuildOnCompileCheckBox.Text = Resources.OptionsAlwaysBuildOnCompile;

            _alwaysBuildOnRunCheckBox = new CheckBox();
            _alwaysBuildOnRunCheckBox.AutoSize = true;
            _alwaysBuildOnRunCheckBox.Location = new Point(19, 45);
            _alwaysBuildOnRunCheckBox.Name = m_alwaysBuildOnRunCheckBox;
            _alwaysBuildOnRunCheckBox.TabIndex = 1;
            _alwaysBuildOnRunCheckBox.Text = Resources.OptionsAlwaysBuildOnRun;

            _settingsGroupBox.Controls.Add(_alwaysBuildOnCompileCheckBox);
            _settingsGroupBox.Controls.Add(_alwaysBuildOnRunCheckBox);
            Controls.Add(_settingsGroupBox);

            #endregion

            _alwaysBuildOnCompileCheckBox.Checked =
                _buildToolManager.AllwaysBuildOnCompile;
            _alwaysBuildOnRunCheckBox.Checked =
                _buildToolManager.AllwaysBuildOnRun;
        }

        /// <summary>
        /// Save the settings when requested by the options editor.
        /// </summary>
        public override void Save()
        {
            _buildToolManager.AllwaysBuildOnCompile =
                _alwaysBuildOnCompileCheckBox.Checked;
            _buildToolManager.AllwaysBuildOnRun =
                _alwaysBuildOnRunCheckBox.Checked;
        }
    }
}
