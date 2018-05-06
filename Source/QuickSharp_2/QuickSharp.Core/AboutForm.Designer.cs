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

using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace QuickSharp.Core
{
    partial class AboutForm
    {
        private IContainer _components = null;
        private Button _okButton;
        private ListBox _pluginListBox;
        private Label _installedPluginsLabel;
        private TextBox _pluginDescriptionTextBox;
        private Label _pluginDetailsLabel;
        private Label _coreCopyrightLabel;
        private Label _clientCopyrightLabel;
        private Label _dockpanelCopyrightLabel;

        #region Form Control Names

        /// <summary>
        /// Name used to access the form control collection member.
        /// </summary>
        public const string m_okButton = "okButton";

        /// <summary>
        /// Name used to access the form control collection member.
        /// </summary>
        public const string m_pluginListBox = "pluginListBox";

        /// <summary>
        /// Name used to access the form control collection member.
        /// </summary>
        public const string m_installedPluginsLabel = "installedPluginsLabel";

        /// <summary>
        /// Name used to access the form control collection member.
        /// </summary>
        public const string m_pluginDescriptionTextBox = "pluginDescriptionTextBox";

        /// <summary>
        /// Name used to access the form control collection member.
        /// </summary>
        public const string m_pluginDetailsLabel = "pluginDetailsLabel";

        /// <summary>
        /// Name used to access the form control collection member.
        /// </summary>
        public const string m_coreCopyrightLabel = "coreCopyrightLabel";

        /// <summary>
        /// Name used to access the form control collection member.
        /// </summary>
        public const string m_clientCopyrightLabel = "clientCopyrightLabel";

        /// <summary>
        /// Name used to access the form control collection member.
        /// </summary>
        public const string m_dockpanelCopyrightLabel = "dockpanelCopyrightLabel";

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
                _components.Dispose();

            base.Dispose(disposing);
        }

        #region Form Layout

        private void InitializeComponent()
        {
            _okButton = new Button();
            _pluginListBox = new ListBox();
            _installedPluginsLabel = new Label();
            _pluginDescriptionTextBox = new TextBox();
            _pluginDetailsLabel = new Label();
            _coreCopyrightLabel = new Label();
            _clientCopyrightLabel = new Label();
            _dockpanelCopyrightLabel = new Label();
            
            SuspendLayout();

            _okButton.DialogResult = DialogResult.Cancel;
            _okButton.Location = new Point(457, 363);
            _okButton.Name = m_okButton;
            _okButton.Size = new Size(75, 23);
            _okButton.TabIndex = 0;
            _okButton.Text = Resources.AboutButtonOK;
            _okButton.UseVisualStyleBackColor = true;

            _pluginListBox.DisplayMember = "Name";
            _pluginListBox.FormattingEnabled = true;
            _pluginListBox.Location = new Point(12, 225);
            _pluginListBox.Name = m_pluginListBox;
            _pluginListBox.Size = new Size(428, 69);
            _pluginListBox.TabIndex = 2;
            _pluginListBox.SelectedIndexChanged += new System.EventHandler(PluginListBox_SelectedIndexChanged);

            _installedPluginsLabel.AutoSize = true;
            _installedPluginsLabel.ForeColor = _aboutBoxTextColor;
            _installedPluginsLabel.BackColor = Color.Transparent;
            _installedPluginsLabel.Location = new Point(12, 209);
            _installedPluginsLabel.Name = m_installedPluginsLabel;
            _installedPluginsLabel.TabIndex = 3;
            _installedPluginsLabel.Text = Resources.AboutInstalledPlugins;

            _pluginDescriptionTextBox.BackColor = SystemColors.Window;
            _pluginDescriptionTextBox.Location = new Point(12, 313);
            _pluginDescriptionTextBox.Multiline = true;
            _pluginDescriptionTextBox.Name = m_pluginDescriptionTextBox;
            _pluginDescriptionTextBox.ReadOnly = true;
            _pluginDescriptionTextBox.ScrollBars = ScrollBars.Vertical;
            _pluginDescriptionTextBox.Size = new Size(428, 73);
            _pluginDescriptionTextBox.TabIndex = 4;

            _pluginDetailsLabel.AutoSize = true;
            _pluginDetailsLabel.ForeColor = _aboutBoxTextColor;
            _pluginDetailsLabel.BackColor = Color.Transparent;
            _pluginDetailsLabel.Location = new Point(12, 297);
            _pluginDetailsLabel.Name = m_pluginDetailsLabel;
            _pluginDetailsLabel.TabIndex = 5;
            _pluginDetailsLabel.Text = Resources.AboutPluginDetails;

            _coreCopyrightLabel.AutoSize = true;
            _coreCopyrightLabel.ForeColor = _aboutBoxTextColor;
            _coreCopyrightLabel.BackColor = Color.Transparent;
            _coreCopyrightLabel.Location = new Point(12, 113);
            _coreCopyrightLabel.Name = m_coreCopyrightLabel;
            _coreCopyrightLabel.TabIndex = 6;
            _coreCopyrightLabel.Text = "coreCopyrightLabel";

            _clientCopyrightLabel.AutoSize = true;
            _clientCopyrightLabel.ForeColor = _aboutBoxTextColor;
            _clientCopyrightLabel.BackColor = Color.Transparent;
            _clientCopyrightLabel.Location = new Point(286, 113);
            _clientCopyrightLabel.Name = m_clientCopyrightLabel;
            _clientCopyrightLabel.TabIndex = 7;
            _clientCopyrightLabel.Text = "clientCopyrightLabel";

            _dockpanelCopyrightLabel.AutoSize = true;
            _dockpanelCopyrightLabel.ForeColor = _aboutBoxTextColor;
            _dockpanelCopyrightLabel.BackColor = Color.Transparent;
            _dockpanelCopyrightLabel.Location = new Point(12, 164);
            _dockpanelCopyrightLabel.Name = m_dockpanelCopyrightLabel;
            _dockpanelCopyrightLabel.TabIndex = 8;
            _dockpanelCopyrightLabel.Text = "dockpanelCopyrightLabel";

            Controls.Add(_dockpanelCopyrightLabel);
            Controls.Add(_clientCopyrightLabel);
            Controls.Add(_coreCopyrightLabel);
            Controls.Add(_pluginDetailsLabel);
            Controls.Add(_pluginDescriptionTextBox);
            Controls.Add(_installedPluginsLabel);
            Controls.Add(_pluginListBox);
            Controls.Add(_okButton);

            AcceptButton = _okButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _okButton;
            ClientSize = new Size(550, 400);
            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "About";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}