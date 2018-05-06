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
using System.Drawing;
using System.Windows.Forms;
using QuickSharp.Core;

namespace QuickSharp.Editor
{
    /// <summary>
    /// The editor settings option page.
    /// </summary>
    public class EditorOptionsPage : OptionsPage
    {
        private SettingsManager _settingsManager;
        private GroupBox _editorGroupBox;
        private CheckBox _useTabsCheckBox;
        private Label _tabSizeLabel;
        private NumericUpDown _tabSizeSelector;
        private CheckBox _backspaceUnindentsCheckBox;
        private Label _lnMarginSizeLabel;
        private NumericUpDown _lnMarginSizeSelector;
        private CheckBox _showGuidesCheckBox;
        private CheckBox _showFoldingCheckBox;
        private Label _foldStyleLabel;
        private ComboBox _foldStyleCombo;
        private Label _foldMarkerStyleLabel;
        private ComboBox _foldMarkerStyleCombo;
        private Label _fontNameLabel;
        private ComboBox _fontNameCombo;
        private NumericUpDown _fontSizeSelector;
        private CheckBox _overrideConfigFilesCheckBox;

        #region Form Control Names

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_editorGroupBox = "editorGroupBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_useTabsCheckbox = "useTabsCheckBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_tabSizeLabel = "tabSizeLabel";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_tabSizeSelector = "tabSizeSelector";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_backspaceUnindentsCheckBox = "backspaceUnindentsCheckBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_lnMarginSizeLabel = "lnMarginSizeLabel";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_lnMarginSizeSelector = "lnMarginSizeSelector";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_showGuidesCheckBox = "showGuidesCheckBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_showFoldingCheckbox = "showFoldingCheckBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_foldStyleLabel = "foldStyleLabel";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_foldStyleCombo = "foldStyleCombo";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_foldMarkerStyleLabel = "foldMarkerStyleLabel";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_foldMarkerStyleCombo = "foldMarkerStyleCombo";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_fontNameLabel = "fontNameLabel";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_fontNameCombo = "fontNameCombo";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_fontSizeSelector = "fontSizeSelector";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_overrideConfigFilesCheckbox = "overrideConfigFilesCheckBox";

        #endregion

        /// <summary>
        /// Create the editor options page.
        /// </summary>
        public EditorOptionsPage()
        {
            Name = Constants.UI_OPTIONS_PAGE_EDITOR;
            PageText = Resources.OptionsPageTextEditor;
            GroupText = Resources.OptionsGroupText;

            _editorGroupBox = new GroupBox();
            _useTabsCheckBox = new CheckBox();
            _tabSizeLabel = new Label();
            _tabSizeSelector = new NumericUpDown();
            _backspaceUnindentsCheckBox = new CheckBox();
            _lnMarginSizeLabel = new Label();
            _lnMarginSizeSelector = new NumericUpDown();
            _showGuidesCheckBox = new CheckBox();
            _showFoldingCheckBox = new CheckBox();
            _foldStyleLabel = new Label();
            _foldStyleCombo = new ComboBox();
            _foldMarkerStyleLabel = new Label();
            _foldMarkerStyleCombo = new ComboBox();
            _fontNameLabel = new Label();
            _fontNameCombo = new ComboBox();
            _fontSizeSelector = new NumericUpDown();
            _overrideConfigFilesCheckBox = new CheckBox();

            Controls.Add(_editorGroupBox);
            _editorGroupBox.Controls.Add(_useTabsCheckBox);
            _editorGroupBox.Controls.Add(_tabSizeLabel);
            _editorGroupBox.Controls.Add(_tabSizeSelector);
            _editorGroupBox.Controls.Add(_backspaceUnindentsCheckBox);
            _editorGroupBox.Controls.Add(_lnMarginSizeLabel);
            _editorGroupBox.Controls.Add(_lnMarginSizeSelector);
            _editorGroupBox.Controls.Add(_showGuidesCheckBox);
            _editorGroupBox.Controls.Add(_showFoldingCheckBox);
            _editorGroupBox.Controls.Add(_foldStyleLabel);
            _editorGroupBox.Controls.Add(_foldStyleCombo);
            _editorGroupBox.Controls.Add(_foldMarkerStyleLabel);
            _editorGroupBox.Controls.Add(_foldMarkerStyleCombo);
            _editorGroupBox.Controls.Add(_fontNameLabel);
            _editorGroupBox.Controls.Add(_fontNameCombo);
            _editorGroupBox.Controls.Add(_fontSizeSelector);
            Controls.Add(_overrideConfigFilesCheckBox);

            #region Form Layout

            _editorGroupBox.Text = Resources.OptionsEditorSettings;
            _editorGroupBox.Name = m_editorGroupBox;
            _editorGroupBox.Location = new Point(0, 0);
            _editorGroupBox.Size = new Size(430, 222);
 
            _useTabsCheckBox.AutoSize = true;
            _useTabsCheckBox.Location = new Point(19, 22);
            _useTabsCheckBox.Name = m_useTabsCheckbox;
            _useTabsCheckBox.TabIndex = 2;
            _useTabsCheckBox.Text = Resources.OptionsUseTabs;
            _useTabsCheckBox.UseVisualStyleBackColor = true;

            _tabSizeLabel.AutoSize = true;
            _tabSizeLabel.Location = new Point(230, 23);
            _tabSizeLabel.Name = m_tabSizeLabel;
            _tabSizeLabel.TabIndex = 3;
            _tabSizeLabel.Text = Resources.OptionsTabSize;
 
            _tabSizeSelector.Location = new Point(364, 21);
            _tabSizeSelector.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            _tabSizeSelector.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            _tabSizeSelector.Name = m_tabSizeSelector;
            _tabSizeSelector.Size = new Size(44, 20);
            _tabSizeSelector.TabIndex = 4;
            _tabSizeSelector.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});

            _backspaceUnindentsCheckBox.AutoSize = true;
            _backspaceUnindentsCheckBox.Location = new Point(19, 45);
            _backspaceUnindentsCheckBox.Name = m_backspaceUnindentsCheckBox;
            _backspaceUnindentsCheckBox.TabIndex = 5;
            _backspaceUnindentsCheckBox.Text = Resources.OptionsBackspaceUnindents;
            _backspaceUnindentsCheckBox.UseVisualStyleBackColor = true;


            _lnMarginSizeLabel.AutoSize = true;
            _lnMarginSizeLabel.Location = new Point(230, 56);
            _lnMarginSizeLabel.Name = m_lnMarginSizeLabel;
            _lnMarginSizeLabel.TabIndex = 6;
            _lnMarginSizeLabel.Text = Resources.OptionsLnMarginSize;

            _lnMarginSizeSelector.Location = new Point(364, 54);
            _lnMarginSizeSelector.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            _lnMarginSizeSelector.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            _lnMarginSizeSelector.Name = m_lnMarginSizeSelector;
            _lnMarginSizeSelector.Size = new Size(44, 20);
            _lnMarginSizeSelector.TabIndex = 7;
            _lnMarginSizeSelector.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});

            _showGuidesCheckBox.AutoSize = true;
            _showGuidesCheckBox.Location = new Point(19, 68);
            _showGuidesCheckBox.Name = m_showGuidesCheckBox;
            _showGuidesCheckBox.TabIndex = 8;
            _showGuidesCheckBox.Text = Resources.OptionsShowIndentationGuides;
            _showGuidesCheckBox.UseVisualStyleBackColor = true;

            _showFoldingCheckBox.AutoSize = true;
            _showFoldingCheckBox.Location = new Point(19, 91);
            _showFoldingCheckBox.Name = m_showFoldingCheckbox;
            _showFoldingCheckBox.TabIndex = 9;
            _showFoldingCheckBox.Text = Resources.OptionsShowFolding;
            _showFoldingCheckBox.UseVisualStyleBackColor = true;
            _showFoldingCheckBox.CheckedChanged += delegate { SetFoldingState(); };

            _foldStyleLabel.AutoSize = true;
            _foldStyleLabel.Location = new Point(160, 92);
            _foldStyleLabel.Name = m_foldStyleLabel;
            _foldStyleLabel.TabIndex = 10;
            _foldStyleLabel.Text = Resources.OptionsFoldStyle;

            _foldStyleCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _foldStyleCombo.FormattingEnabled = true;
            _foldStyleCombo.Items.AddRange(new object[] {
                Resources.OptionsNone,
                Resources.OptionsLineBeforeContracted,
                Resources.OptionsLineAfterContracted
            });
            _foldStyleCombo.Location = new Point(268, 88);
            _foldStyleCombo.Name = m_foldStyleCombo;
            _foldStyleCombo.Size = new Size(140, 21);
            _foldStyleCombo.TabIndex = 11;

            _foldMarkerStyleLabel.AutoSize = true;
            _foldMarkerStyleLabel.Location = new Point(160, 115);
            _foldMarkerStyleLabel.Name = m_foldMarkerStyleLabel;
            _foldMarkerStyleLabel.TabIndex = 12;
            _foldMarkerStyleLabel.Text = Resources.OptionsFoldMarkerStyle;

            _foldMarkerStyleCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _foldMarkerStyleCombo.FormattingEnabled = true;
            _foldMarkerStyleCombo.Items.AddRange(new object[] {
                Resources.OptionsBoxPlusMinus,
                Resources.OptionsCirclePlusMinus,
                Resources.OptionsPlusMinus,
                Resources.OptionsArrows
            });
            _foldMarkerStyleCombo.Location = new Point(268, 111);
            _foldMarkerStyleCombo.Name = m_foldMarkerStyleCombo;
            _foldMarkerStyleCombo.Size = new Size(140, 21);
            _foldMarkerStyleCombo.TabIndex = 13;

            _fontNameLabel.AutoSize = true;
            _fontNameLabel.Location = new Point(98, 149);
            _fontNameLabel.Name = m_fontNameLabel;
            _fontNameLabel.TabIndex = 14;
            _fontNameLabel.Text = Resources.OptionsFontName;

            _fontNameCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _fontNameCombo.FormattingEnabled = true;
            _fontNameCombo.Location = new Point(160, 145);
            _fontNameCombo.Name = m_fontNameCombo;
            _fontNameCombo.Size = new Size(200, 21);
            _fontNameCombo.TabIndex = 15;

            _fontSizeSelector.Location = new Point(364, 145);
            _fontSizeSelector.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            _fontSizeSelector.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            _fontSizeSelector.Name = m_fontSizeSelector;
            _fontSizeSelector.Size = new Size(44, 20);
            _fontSizeSelector.TabIndex = 16;
            _fontSizeSelector.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});

            _overrideConfigFilesCheckBox.AutoSize = true;
            _overrideConfigFilesCheckBox.Location = new Point(2, 229);
            _overrideConfigFilesCheckBox.Name = m_overrideConfigFilesCheckbox;
            _overrideConfigFilesCheckBox.TabIndex = 0;
            _overrideConfigFilesCheckBox.Text = Resources.OptionsOverrideConfigFiles;
            _overrideConfigFilesCheckBox.UseVisualStyleBackColor = true;
            _overrideConfigFilesCheckBox.CheckedChanged += delegate { SetOverrideState(); };

            #endregion

            _settingsManager = SettingsManager.GetInstance();

            _useTabsCheckBox.Checked = _settingsManager.UseTabs;
            _tabSizeSelector.Value = _settingsManager.TabSize;
            _backspaceUnindentsCheckBox.Checked = _settingsManager.BackspaceUnindents;
            _lnMarginSizeSelector.Value = _settingsManager.LineNumberMarginSize;
            _showGuidesCheckBox.Checked = _settingsManager.ShowIndentationGuides;
            _showFoldingCheckBox.Checked = _settingsManager.ShowFolding;
            _foldStyleCombo.SelectedIndex = _settingsManager.FoldStyle;
            _foldMarkerStyleCombo.SelectedIndex = _settingsManager.FoldMarkerStyle;
            _fontSizeSelector.Value = (decimal)Fonts.ValidateFontSize(_settingsManager.FontSize);
            _overrideConfigFilesCheckBox.Checked = _settingsManager.OverrideConfigFiles;

            InitFontNameCombo(_settingsManager.FontName);

            SetOverrideState();
            SetFoldingState();
        }

        private void InitFontNameCombo(string selectedName)
        {
            selectedName = Fonts.ValidateFontName(selectedName);
 
            List<string> fonts = Fonts.GetFontList();

            foreach (string name in fonts)
            {
                _fontNameCombo.Items.Add(name);
                
                if (name == selectedName)
                    _fontNameCombo.SelectedItem = name;
            }
        }

        private void SetOverrideState()
        {
            _editorGroupBox.Enabled = _overrideConfigFilesCheckBox.Checked;
        }

        private void SetFoldingState()
        {
            _foldStyleLabel.Enabled = _showFoldingCheckBox.Checked;
            _foldStyleCombo.Enabled = _showFoldingCheckBox.Checked;
            _foldMarkerStyleLabel.Enabled = _showFoldingCheckBox.Checked;
            _foldMarkerStyleCombo.Enabled = _showFoldingCheckBox.Checked;
        }

        /// <summary>
        /// Save the settings to the session persistence store (e.g. the registry).
        /// </summary>
        public override void Save()
        {
            _settingsManager.UseTabs = _useTabsCheckBox.Checked;
            _settingsManager.TabSize = (int)_tabSizeSelector.Value;
            _settingsManager.BackspaceUnindents = _backspaceUnindentsCheckBox.Checked;
            _settingsManager.LineNumberMarginSize = (int)_lnMarginSizeSelector.Value;
            _settingsManager.ShowIndentationGuides = _showGuidesCheckBox.Checked;
            _settingsManager.ShowFolding = _showFoldingCheckBox.Checked;
            _settingsManager.FoldStyle = _foldStyleCombo.SelectedIndex;
            _settingsManager.FoldMarkerStyle = _foldMarkerStyleCombo.SelectedIndex;
            _settingsManager.FontName = (string)_fontNameCombo.SelectedItem;
            _settingsManager.FontSize = (float)_fontSizeSelector.Value;
            _settingsManager.OverrideConfigFiles = _overrideConfigFilesCheckBox.Checked;
            _settingsManager.SaveEditorSettings();
        }
    }
}
