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
using System.ComponentModel;

namespace QuickSharp.SqlManager
{
    partial class SqlConnectionForm
    {
        private IContainer _components = null;
        private Button _okButton;
        private Button _cancelButton;
        private Label _nameLabel;
        private Label _connectionStringLabel;
        private TextBox _nameTextBox;
        private TextBox _connectionStringTextBox;
        private Label _providerLabel;
        private ComboBox _providerComboBox;
        private Button _testButton;

        #region Form Control Names

        public const string m_okButton = "okButton";
        public const string m_cancelButton = "cancelButton";
        public const string m_nameLabel = "nameLabel";
        public const string m_connectionStringLabel = "connectionStringLabel";
        public const string m_nameTextBox = "nameTextBox";
        public const string m_connectionStringTextBox = "connectionStringTextBox";
        public const string m_providerLabel = "providerLabel";
        public const string m_providerComboBox = "providerComboBox";
        public const string m_testButton = "testButton";

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
            _cancelButton = new Button();
            _nameLabel = new Label();
            _connectionStringLabel = new Label();
            _nameTextBox = new TextBox();
            _connectionStringTextBox = new TextBox();
            _providerLabel = new Label();
            _providerComboBox = new ComboBox();
            _testButton = new Button();
            SuspendLayout();

            _okButton.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            _okButton.Location = new Point(259, 230);
            _okButton.Name = m_okButton;
            _okButton.Size = new Size(75, 23);
            _okButton.TabIndex = 7;
            _okButton.Text = Resources.FormButtonOK;
            _okButton.UseVisualStyleBackColor = true;
            _okButton.Click += new System.EventHandler(OkButton_Click);

            _cancelButton.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            _cancelButton.DialogResult = DialogResult.Cancel;
            _cancelButton.Location = new Point(340, 230);
            _cancelButton.Name = m_cancelButton;
            _cancelButton.Size = new Size(75, 23);
            _cancelButton.TabIndex = 8;
            _cancelButton.Text = Resources.FormButtonCancel;
            _cancelButton.DialogResult = DialogResult.Cancel;
            _cancelButton.UseVisualStyleBackColor = true;

            _nameLabel.AutoSize = true;
            _nameLabel.Location = new Point(9, 9);
            _nameLabel.Name = m_nameLabel;
            _nameLabel.TabIndex = 0;
            _nameLabel.Text = Resources.FormName;

            _connectionStringLabel.AutoSize = true;
            _connectionStringLabel.Location = new Point(9, 88);
            _connectionStringLabel.Name = m_connectionStringLabel;
            _connectionStringLabel.TabIndex = 4;
            _connectionStringLabel.Text = Resources.FormConnectionString;

            _nameTextBox.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right)));
            _nameTextBox.Location = new Point(12, 25);
            _nameTextBox.Name = m_nameTextBox;
            _nameTextBox.Size = new Size(403, 21);
            _nameTextBox.TabIndex = 1;

            _connectionStringTextBox.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
                | AnchorStyles.Left) | AnchorStyles.Right)));
            _connectionStringTextBox.Location = new Point(12, 104);
            _connectionStringTextBox.Multiline = true;
            _connectionStringTextBox.Name = m_connectionStringTextBox;
            _connectionStringTextBox.Size = new Size(403, 104);
            _connectionStringTextBox.TabIndex = 5;
            _connectionStringTextBox.AcceptsReturn = true;
            _connectionStringTextBox.ScrollBars = ScrollBars.Vertical;

            _providerLabel.AutoSize = true;
            _providerLabel.Location = new Point(9, 49);
            _providerLabel.Name = m_providerLabel;
            _providerLabel.TabIndex = 2;
            _providerLabel.Text = Resources.FormProvider;

            _providerComboBox.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right)));
            _providerComboBox.DisplayMember = "DisplayName";
            _providerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _providerComboBox.FormattingEnabled = true;
            _providerComboBox.Location = new Point(12, 64);
            _providerComboBox.Name = m_providerComboBox;
            _providerComboBox.Size = new Size(403, 21);
            _providerComboBox.TabIndex = 3;

            _testButton.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
            _testButton.Location = new Point(12, 230);
            _testButton.Name = m_testButton;
            _testButton.Size = new Size(106, 23);
            _testButton.TabIndex = 6;
            _testButton.Text = Resources.FormButtonTest;
            _testButton.UseVisualStyleBackColor = true;
            _testButton.Click += new System.EventHandler(TestButton_Click);

            AcceptButton = _okButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _cancelButton;
            ClientSize = new Size(427, 265);
            Controls.Add(_testButton);
            Controls.Add(_providerComboBox);
            Controls.Add(_providerLabel);
            Controls.Add(_connectionStringTextBox);
            Controls.Add(_nameTextBox);
            Controls.Add(_connectionStringLabel);
            Controls.Add(_nameLabel);
            Controls.Add(_cancelButton);
            Controls.Add(_okButton);
            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            MaximizeBox = true;
            MinimizeBox = false;
            Name = "SqlConnectionForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(400, 300);
            Text = Resources.FormTitle;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}