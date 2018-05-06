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
using System.Drawing;
using System.Windows.Forms;
using QuickSharp.Core;

namespace QuickSharp.Core
{
    /// <summary>
    /// Provides an options page to allow some general application settings to be set.
    /// </summary>
    public class GeneralOptionsPage : OptionsPage
    {
        private ApplicationManager _applicationManager;
        private IPersistenceManager _persistenceManager;
        private GroupBox _documentsGroupBox;
        private CheckBox _restoreLastSessionCheckBox;
        private CheckBox _showNoHandlerCheckBox;
        private CheckBox _allowShellOpenCheckBox;
        private CheckBox _showDocumentPathCheckBox;
        private GroupBox _themeGroupBox;
        private ComboBox _themeComboBox;
        private Label _restartRequiredLabel;

        #region Form Control Names

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_documentGroupBox = "documentsGroupBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_restoreLastSessionCheckBox = "restoreLastSessionCheckBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_showNoHandlerCheckBox = "showNoHandlerCheckBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_allowShellOpenCheckBox = "allowShellOpenCheckBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_showDocumentPathCheckBox = "showDocumentPathCheckBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_themeGroupBox = "themeGroupBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_themeComboBox = "themeComboBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_restartRequiredLabel = "restartRequiredLabel";

        #endregion

        /// <summary>
        /// Initialize a new instance of the GeneralOptionsPage.
        /// </summary>
        public GeneralOptionsPage()
        {
            _applicationManager = ApplicationManager.GetInstance();
            _persistenceManager = _applicationManager.
                GetPersistenceManager(Constants.MODULE_NAME);

            Name = Constants.UI_OPTIONS_PAGE_GENERAL;
            PageText = Resources.OptionsPageTextGeneral;
            GroupText = Resources.OptionsGroupText;

            _documentsGroupBox = new GroupBox();
            _restoreLastSessionCheckBox = new CheckBox();
            _showNoHandlerCheckBox = new CheckBox();
            _allowShellOpenCheckBox = new CheckBox();
            _showDocumentPathCheckBox = new CheckBox();
            _themeGroupBox = new GroupBox();
            _themeComboBox = new ComboBox();
            _restartRequiredLabel = new Label();

            #region Form Layout

            _documentsGroupBox.Controls.Add(_restoreLastSessionCheckBox);
            _documentsGroupBox.Controls.Add(_showNoHandlerCheckBox);
            _documentsGroupBox.Controls.Add(_allowShellOpenCheckBox);
            _documentsGroupBox.Controls.Add(_showDocumentPathCheckBox);
            _documentsGroupBox.Location = new Point(0, 0);
            _documentsGroupBox.Name = m_documentGroupBox;
            _documentsGroupBox.TabIndex = 0;
            _documentsGroupBox.TabStop = false;
            _documentsGroupBox.Text = Resources.OptionsDocumentsGroupBox;
            _documentsGroupBox.Size = new Size(430, 137);

            _restoreLastSessionCheckBox.AutoSize = true;
            _restoreLastSessionCheckBox.Location = new Point(19, 22);
            _restoreLastSessionCheckBox.Name = m_restoreLastSessionCheckBox;
            _restoreLastSessionCheckBox.TabIndex = 1;
            _restoreLastSessionCheckBox.Text = Resources.OptionsRestoreLastSession;

            _showNoHandlerCheckBox.AutoSize = true;
            _showNoHandlerCheckBox.Location = new Point(19, 45);
            _showNoHandlerCheckBox.Name = m_showNoHandlerCheckBox;
            _showNoHandlerCheckBox.TabIndex = 2;
            _showNoHandlerCheckBox.Text = Resources.OptionsShowNoHandler;

            _allowShellOpenCheckBox.AutoSize = true;
            _allowShellOpenCheckBox.Location = new Point(19, 68);
            _allowShellOpenCheckBox.Name = m_allowShellOpenCheckBox;
            _allowShellOpenCheckBox.TabIndex = 3;
            _allowShellOpenCheckBox.Text = Resources.OptionsAllowShellOpen;

            _showDocumentPathCheckBox.AutoSize = true;
            _showDocumentPathCheckBox.Location = new Point(19, 91);
            _showDocumentPathCheckBox.Name = m_showDocumentPathCheckBox;
            _showDocumentPathCheckBox.TabIndex = 4;
            _showDocumentPathCheckBox.Text = Resources.OptionsShowDocumentPath;

            _themeGroupBox.Location = new Point(0, 144);
            _themeGroupBox.Name = m_themeGroupBox;
            _themeGroupBox.TabIndex = 5;
            _themeGroupBox.TabStop = false;
            _themeGroupBox.Text = Resources.OptionsThemeGroupBox;
            _themeGroupBox.Size = new Size(430, 78);
            _themeGroupBox.Controls.Add(_themeComboBox);

            _themeComboBox.Location = new Point(19, 31);
            _themeComboBox.Name = m_themeComboBox;
            _themeComboBox.TabIndex = 6;
            _themeComboBox.Size = new Size(392, 21);
            _themeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _themeComboBox.DisplayMember = "Name";

            _restartRequiredLabel.AutoSize = true;
            _restartRequiredLabel.Location = new System.Drawing.Point(0, 230);
            _restartRequiredLabel.Name = m_restartRequiredLabel;
            _restartRequiredLabel.Text = Resources.OptionsRestartRequired;
            _restartRequiredLabel.TabIndex = 7;

            Controls.Add(_documentsGroupBox);
            Controls.Add(_themeGroupBox);
            Controls.Add(_restartRequiredLabel);

            #endregion

            _restoreLastSessionCheckBox.Checked =
                _persistenceManager.ReadBoolean(
                    Constants.KEY_DOCUMENT_RESTORE_LAST_SESSION, false);

            _showNoHandlerCheckBox.Checked =
                _persistenceManager.ReadBoolean(
                    Constants.KEY_DOCUMENT_SHOW_NO_HANDLER_MESSAGE, true);

            _allowShellOpenCheckBox.Checked =
                _persistenceManager.ReadBoolean(
                    Constants.KEY_DOCUMENT_ALLOW_SHELL_OPEN, true);

            _showDocumentPathCheckBox.Checked =
                _persistenceManager.ReadBoolean(
                    Constants.KEY_SHOW_DOCUMENT_PATH, false);

            string savedTheme = _persistenceManager.ReadString(
                Constants.KEY_SELECTED_THEME,
                _applicationManager.SelectedTheme);

            // Is the theme still available?
            if (_applicationManager.GetThemeProvider(savedTheme) == null)
                savedTheme = Constants.DEFAULT_THEME_ID;

            List<StringMap> themes = 
                _applicationManager.ThemeProviderMap;

            foreach (StringMap map in themes)
            {
                _themeComboBox.Items.Add(map);

                if (map.Value == savedTheme)
                    _themeComboBox.SelectedItem = map;
            }
        }

        /// <summary>
        /// Writes the setting to the persistence manager.
        /// </summary>
        public override void Save()
        {
            _persistenceManager.WriteBoolean(
                Constants.KEY_DOCUMENT_RESTORE_LAST_SESSION,
                _restoreLastSessionCheckBox.Checked);

            _persistenceManager.WriteBoolean(
                Constants.KEY_DOCUMENT_SHOW_NO_HANDLER_MESSAGE,
                _showNoHandlerCheckBox.Checked);

            _persistenceManager.WriteBoolean(
                Constants.KEY_DOCUMENT_ALLOW_SHELL_OPEN,
                _allowShellOpenCheckBox.Checked);

            _persistenceManager.WriteBoolean(
                Constants.KEY_SHOW_DOCUMENT_PATH,
                _showDocumentPathCheckBox.Checked);

            StringMap selectedTheme =
                _themeComboBox.SelectedItem as StringMap;

            if (selectedTheme != null)
                _persistenceManager.WriteString(
                    Constants.KEY_SELECTED_THEME, selectedTheme.Value);
        }
    }
}
