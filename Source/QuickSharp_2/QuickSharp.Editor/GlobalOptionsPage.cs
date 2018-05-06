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
using System.Windows.Forms;
using QuickSharp.Core;

namespace QuickSharp.Editor
{
    /// <summary>
    /// The global settings option page.
    /// </summary>
    public class GlobalOptionsPage : OptionsPage
    {
        private SettingsManager _settingsManager;
        private GroupBox _globalGroupBox;
        private CheckBox _matchBracesCheckBox;
        private CheckBox _wordWrapCheckBox;
        private Label _colorModeLabel;
        private ComboBox _colorModeComboBox;

        #region Form Control Names

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_globalGroupBox = "globalGroupBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_matchBracesCheckbox = "matchBracesCheckBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_wordWrapCheckBox = "wordWrapCheckBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_colorModeLabel = "colorModeLabel";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_colorModeComboBox = "colorModeComboBox";

        #endregion

        /// <summary>
        /// Create the options page.
        /// </summary>
        public GlobalOptionsPage()
        {
            Name = Constants.UI_OPTIONS_PAGE_GLOBAL;
            PageText = Resources.OptionsPageTextGlobal;
            GroupText = Resources.OptionsGroupText;

            #region Form layout

            _globalGroupBox = new GroupBox();
            _globalGroupBox.Location = new Point(0, 0);
            _globalGroupBox.Name = m_globalGroupBox;
            _globalGroupBox.Size = new Size(430, 250);
            _globalGroupBox.TabIndex = 0;
            _globalGroupBox.TabStop = false;
            _globalGroupBox.Text = Resources.OptionsGlobalSettings;

            _matchBracesCheckBox = new CheckBox();
            _matchBracesCheckBox.AutoSize = true;
            _matchBracesCheckBox.Location = new Point(19, 22);
            _matchBracesCheckBox.Name = m_matchBracesCheckbox;
            _matchBracesCheckBox.TabIndex = 1;
            _matchBracesCheckBox.Text = Resources.OptionsMatchBraces;
            _matchBracesCheckBox.UseVisualStyleBackColor = true;

            _colorModeLabel = new Label();
            _colorModeLabel.AutoSize = true;
            _colorModeLabel.Location = new Point(220, 23);
            _colorModeLabel.Name = m_colorModeLabel;
            _colorModeLabel.TabIndex = 2;
            _colorModeLabel.Text = Resources.OptionsPrintingColorMode;

            _colorModeComboBox = new ComboBox();
            _colorModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _colorModeComboBox.FormattingEnabled = true;
            _colorModeComboBox.Items.AddRange(new object[] {
                Resources.OptionsColorModeNormal,
                Resources.OptionsColorModeBlackOnWhite,
                Resources.OptionsColorModeColorOnWhite
            });
            _colorModeComboBox.Location = new Point(280, 21);
            _colorModeComboBox.Name = m_colorModeComboBox;
            _colorModeComboBox.Size = new Size(128, 21);
            _colorModeComboBox.TabIndex = 3;

            _wordWrapCheckBox = new CheckBox();
            _wordWrapCheckBox.AutoSize = true;
            _wordWrapCheckBox.Location = new Point(19, 45);
            _wordWrapCheckBox.Name = m_wordWrapCheckBox;
            _wordWrapCheckBox.TabIndex = 4;
            _wordWrapCheckBox.Text = Resources.OptionsWordWrap;
            _wordWrapCheckBox.UseVisualStyleBackColor = true;

            _globalGroupBox.Controls.Add(_matchBracesCheckBox);
            _globalGroupBox.Controls.Add(_wordWrapCheckBox);
            _globalGroupBox.Controls.Add(_colorModeLabel);
            _globalGroupBox.Controls.Add(_colorModeComboBox);
            Controls.Add(_globalGroupBox);

            #endregion

            _settingsManager = SettingsManager.GetInstance();
            _matchBracesCheckBox.Checked = _settingsManager.MatchBraces;
            _wordWrapCheckBox.Checked = _settingsManager.WordWrap;
            _colorModeComboBox.SelectedIndex =
                _settingsManager.GetPrintingColorModeCode(
                    _settingsManager.PrintingColorMode);
        }

        /// <summary>
        /// Save the settings to the session persistence store (e.g. the registry).
        /// </summary>
        public override void Save()
        {
            _settingsManager.MatchBraces = _matchBracesCheckBox.Checked;
            _settingsManager.WordWrap = _wordWrapCheckBox.Checked;

            _settingsManager.PrintingColorMode =
                _settingsManager.GetPrintingColorMode(
                    _colorModeComboBox.SelectedIndex);

            _settingsManager.SaveGlobalSettings();
        }
    }
}
