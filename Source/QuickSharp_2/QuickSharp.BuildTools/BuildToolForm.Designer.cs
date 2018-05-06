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

namespace QuickSharp.BuildTools
{
    partial class BuildToolForm
    {
        private IContainer _components = null;
        private TextBox _displayNameTextBox;
        private TextBox _toolPathTextBox;
        private TextBox _toolArgsTextBox;
        private TextBox _userArgsTextBox;
        private Label _displayNameLabel;
        private Label _toolPathLabel;
        private Label _toolArgsLabel;
        private Label _userArgsLabel;
        private ComboBox _actionComboBox;
        private Label _fileTypeLabel;
        private Label _actionLabel;
        private Button _okButton;
        private Button _cancelButton;
        private Button _toolPathHelperButton;
        private ComboBox _parserComboBox;
        private Label _parserLabel;
        private ComboBox _documentTypeComboBox;

        #region Form Control Names

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_displayNameTextBox = "displayNameTextBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_toolPathTextBox = "toolPathTextBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_toolArgsTextBox = "toolArgsTextBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_userArgsTextBox = "userArgsTextBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_displayNameLabel = "displayNameLabel";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_toolPathLabel = "toolPathLabel";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_toolArgsLabel = "toolArgsLabel";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_userArgsLabel = "userArgsLabel";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_actionComboBox = "actionComboBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_fileTypeLabel = "fileTypeLabel";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_actionLabel = "actionLabel";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_okButton = "okButton";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_cancelButton = "cancelButton";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_toolPathHelperButton = "toolPathHelperButton";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_parserComboBox = "parserComboBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_parserLabel = "parserLabel";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_documentTypeComboBox = "documentTypeComboBox";

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
            _displayNameTextBox = new TextBox();
            _toolPathTextBox = new TextBox();
            _toolArgsTextBox = new TextBox();
            _userArgsTextBox = new TextBox();
            _displayNameLabel = new Label();
            _toolPathLabel = new Label();
            _toolArgsLabel = new Label();
            _userArgsLabel = new Label();
            _actionComboBox = new ComboBox();
            _fileTypeLabel = new Label();
            _actionLabel = new Label();
            _okButton = new Button();
            _cancelButton = new Button();
            _toolPathHelperButton = new Button();
            _parserComboBox = new ComboBox();
            _parserLabel = new Label();
            _documentTypeComboBox = new ComboBox();
            SuspendLayout();

            _displayNameTextBox.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
                        | AnchorStyles.Right)));
            _displayNameTextBox.Location = new Point(15, 25);
            _displayNameTextBox.Name = m_displayNameTextBox;
            _displayNameTextBox.Size = new Size(467, 21);
            _displayNameTextBox.TabIndex = 1;

            _toolPathTextBox.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
                        | AnchorStyles.Right)));
            _toolPathTextBox.Location = new Point(15, 105);
            _toolPathTextBox.Name = m_toolPathTextBox;
            _toolPathTextBox.Size = new Size(436, 21);
            _toolPathTextBox.TabIndex = 9;

            _toolArgsTextBox.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
                        | AnchorStyles.Right)));
            _toolArgsTextBox.Location = new Point(15, 145);
            _toolArgsTextBox.Name = m_toolArgsTextBox;
            _toolArgsTextBox.Size = new Size(467, 21);
            _toolArgsTextBox.TabIndex = 12;

            _userArgsTextBox.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
                        | AnchorStyles.Left)
                        | AnchorStyles.Right)));
            _userArgsTextBox.Location = new Point(15, 185);
            _userArgsTextBox.Multiline = true;
            _userArgsTextBox.Name = m_userArgsTextBox;
            _userArgsTextBox.ScrollBars = ScrollBars.Vertical;
            _userArgsTextBox.Size = new Size(467, 51);
            _userArgsTextBox.TabIndex = 14;
            _userArgsTextBox.AcceptsReturn = true;

            _displayNameLabel.AutoSize = true;
            _displayNameLabel.Location = new Point(12, 9);
            _displayNameLabel.Name = m_displayNameLabel;
            _displayNameLabel.TabIndex = 0;
            _displayNameLabel.Text = Resources.ToolDialogDisplayName;

            _toolPathLabel.AutoSize = true;
            _toolPathLabel.Location = new Point(12, 89);
            _toolPathLabel.Name = m_toolPathLabel;
            _toolPathLabel.TabIndex = 8;
            _toolPathLabel.Text = Resources.ToolDialogPath;

            _toolArgsLabel.AutoSize = true;
            _toolArgsLabel.Location = new Point(12, 129);
            _toolArgsLabel.Name = m_toolArgsLabel;
            _toolArgsLabel.TabIndex = 11;
            _toolArgsLabel.Text = Resources.ToolDialogArguments;

            _userArgsLabel.AutoSize = true;
            _userArgsLabel.Location = new Point(12, 169);
            _userArgsLabel.Name = m_userArgsLabel;
            _userArgsLabel.TabIndex = 13;
            _userArgsLabel.Text = Resources.ToolDialogCommonOptions;

            _actionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _actionComboBox.FormattingEnabled = true;
            _actionComboBox.Location = new Point(92, 65);
            _actionComboBox.Name = m_actionComboBox;
            _actionComboBox.Size = new Size(144, 21);
            _actionComboBox.TabIndex = 5;
            _actionComboBox.SelectedIndexChanged +=
                new System.EventHandler(ActionComboBox_SelectedIndexChanged);

            _fileTypeLabel.AutoSize = true;
            _fileTypeLabel.Location = new Point(12, 49);
            _fileTypeLabel.Name = m_fileTypeLabel;
            _fileTypeLabel.Size = new Size(52, 13);
            _fileTypeLabel.TabIndex = 2;
            _fileTypeLabel.Text = Resources.ToolDialogFileType;

            _actionLabel.AutoSize = true;
            _actionLabel.Location = new Point(89, 49);
            _actionLabel.Name = m_actionLabel;
            _actionLabel.Size = new Size(41, 13);
            _actionLabel.TabIndex = 4;
            _actionLabel.Text = Resources.ToolDialogAction;

            _okButton.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            _okButton.Location = new Point(326, 247);
            _okButton.Name = m_okButton;
            _okButton.Size = new Size(75, 23);
            _okButton.TabIndex = 15;
            _okButton.Text = Resources.ToolDialogOK;
            _okButton.UseVisualStyleBackColor = true;
            _okButton.Click += new System.EventHandler(OkButton_Click);

            _cancelButton.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            _cancelButton.DialogResult = DialogResult.Cancel;
            _cancelButton.Location = new Point(407, 247);
            _cancelButton.Name = m_cancelButton;
            _cancelButton.Size = new Size(75, 23);
            _cancelButton.TabIndex = 16;
            _cancelButton.Text = Resources.ToolDialogCancel;
            _cancelButton.UseVisualStyleBackColor = true;

            _toolPathHelperButton.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            _toolPathHelperButton.Location = new Point(456, 104);
            _toolPathHelperButton.Name = m_toolPathHelperButton;
            _toolPathHelperButton.Size = new Size(27, 23);
            _toolPathHelperButton.TabIndex = 10;
            _toolPathHelperButton.Text = "...";
            _toolPathHelperButton.UseVisualStyleBackColor = true;
            _toolPathHelperButton.Click += new System.EventHandler(ToolPathHelperButton_Click);

            _parserComboBox.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
                        | AnchorStyles.Right)));
            _parserComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _parserComboBox.FormattingEnabled = true;
            _parserComboBox.Location = new Point(242, 65);
            _parserComboBox.Name = m_parserComboBox;
            _parserComboBox.Size = new Size(240, 21);
            _parserComboBox.TabIndex = 7;

            _parserLabel.AutoSize = true;
            _parserLabel.Location = new Point(239, 49);
            _parserLabel.Name = m_parserLabel;
            _parserLabel.Size = new Size(79, 13);
            _parserLabel.TabIndex = 6;
            _parserLabel.Text = Resources.ToolDialogOutputParser;

            _documentTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _documentTypeComboBox.FormattingEnabled = true;
            _documentTypeComboBox.Location = new Point(15, 65);
            _documentTypeComboBox.Name = m_documentTypeComboBox;
            _documentTypeComboBox.Size = new Size(71, 21);
            _documentTypeComboBox.TabIndex = 3;
            _documentTypeComboBox.SelectedIndexChanged +=
                new System.EventHandler(DocumentTypeComboBox_SelectedIndexChanged);

            AcceptButton = _okButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _cancelButton;
            ClientSize = new Size(497, 282);
            Controls.Add(_documentTypeComboBox);
            Controls.Add(_parserLabel);
            Controls.Add(_parserComboBox);
            Controls.Add(_toolPathHelperButton);
            Controls.Add(_cancelButton);
            Controls.Add(_okButton);
            Controls.Add(_actionLabel);
            Controls.Add(_fileTypeLabel);
            Controls.Add(_actionComboBox);
            Controls.Add(_userArgsLabel);
            Controls.Add(_toolArgsLabel);
            Controls.Add(_toolPathLabel);
            Controls.Add(_displayNameLabel);
            Controls.Add(_userArgsTextBox);
            Controls.Add(_toolArgsTextBox);
            Controls.Add(_toolPathTextBox);
            Controls.Add(_displayNameTextBox);
            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            MaximizeBox = true;
            MinimizeBox = false;
            MinimumSize = new Size(400, 300);
            Name = "BuildToolForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = Resources.ToolDialogTitle;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}