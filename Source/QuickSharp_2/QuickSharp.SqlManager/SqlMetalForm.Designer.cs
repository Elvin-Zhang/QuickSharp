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
    partial class SqlMetalForm
    {
        private IContainer _components = null;
        private Button _okButton;
        private Button _cancelButton;
        private Label _introLabel;
        private Label _filenameLabel;
        private TextBox _filenameTextBox;
        private GroupBox _includeGroupBox;
        private CheckBox _sprocsCheckBox;
        private CheckBox _functionsCheckBox;
        private CheckBox _viewsCheckBox;

        #region Form Control Names

        public const string m_okButton = "okButton";
        public const string m_cancelButton = "cancelButton";
        public const string m_introLabel = "introLabel";
        public const string m_filenameLabel = "filenameLabel";
        public const string m_filenameTextBox = "filenameTextBox";
        public const string m_includeGroupBox = "includeGroupBox";
        public const string m_sprocsCheckBox = "sprocsCheckBox";
        public const string m_functionsCheckBox = "functionsCheckBox";
        public const string m_viewsCheckBox = "viewsCheckBox";

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
            _introLabel = new Label();
            _filenameLabel = new Label();
            _filenameTextBox = new TextBox();
            _includeGroupBox = new GroupBox();
            _sprocsCheckBox = new CheckBox();
            _functionsCheckBox = new CheckBox();
            _viewsCheckBox = new CheckBox();
            _includeGroupBox.SuspendLayout();
            SuspendLayout();

            _okButton.Location = new Point(179, 226);
            _okButton.Name = m_okButton;
            _okButton.Size = new Size(75, 23);
            _okButton.TabIndex = 4;
            _okButton.Text = "OK";
            _okButton.UseVisualStyleBackColor = true;
            _okButton.Click += new System.EventHandler(OkButton_Click);

            _cancelButton.DialogResult = DialogResult.Cancel;
            _cancelButton.Location = new Point(260, 226);
            _cancelButton.Name = m_cancelButton;
            _cancelButton.Size = new Size(75, 23);
            _cancelButton.TabIndex = 5;
            _cancelButton.Text = "Cancel";
            _cancelButton.UseVisualStyleBackColor = true;

            _introLabel.AutoSize = true;
            _introLabel.Location = new Point(9, 18);
            _introLabel.Name = m_introLabel;
            _introLabel.TabIndex = 0;
            _introLabel.Text = "Extract Database Markup Language file from the database";

            _filenameLabel.AutoSize = true;
            _filenameLabel.Location = new Point(9, 52);
            _filenameLabel.Name = m_filenameLabel;
            _filenameLabel.TabIndex = 1;
            _filenameLabel.Text = "Output filename:";

            _filenameTextBox.Location = new Point(12, 68);
            _filenameTextBox.Name = m_filenameTextBox;
            _filenameTextBox.Size = new Size(323, 21);
            _filenameTextBox.TabIndex = 2;

            _includeGroupBox.Controls.Add(_sprocsCheckBox);
            _includeGroupBox.Controls.Add(_functionsCheckBox);
            _includeGroupBox.Controls.Add(_viewsCheckBox);
            _includeGroupBox.Location = new Point(12, 95);
            _includeGroupBox.Name = m_includeGroupBox;
            _includeGroupBox.Size = new Size(323, 100);
            _includeGroupBox.TabIndex = 3;
            _includeGroupBox.TabStop = false;
            _includeGroupBox.Text = "Include";

            _sprocsCheckBox.AutoSize = true;
            _sprocsCheckBox.Location = new Point(21, 66);
            _sprocsCheckBox.Name = m_sprocsCheckBox;
            _sprocsCheckBox.Size = new Size(57, 17);
            _sprocsCheckBox.TabIndex = 2;
            _sprocsCheckBox.Text = "sprocs";
            _sprocsCheckBox.UseVisualStyleBackColor = true;

            _functionsCheckBox.AutoSize = true;
            _functionsCheckBox.Location = new Point(21, 43);
            _functionsCheckBox.Name = m_functionsCheckBox;
            _functionsCheckBox.Size = new Size(70, 17);
            _functionsCheckBox.TabIndex = 1;
            _functionsCheckBox.Text = "functions";
            _functionsCheckBox.UseVisualStyleBackColor = true;

            _viewsCheckBox.AutoSize = true;
            _viewsCheckBox.Location = new Point(21, 20);
            _viewsCheckBox.Name = m_viewsCheckBox;
            _viewsCheckBox.Size = new Size(53, 17);
            _viewsCheckBox.TabIndex = 0;
            _viewsCheckBox.Text = "views";
            _viewsCheckBox.UseVisualStyleBackColor = true;

            AcceptButton = _okButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _cancelButton;
            ClientSize = new Size(347, 261);
            Controls.Add(_includeGroupBox);
            Controls.Add(_filenameTextBox);
            Controls.Add(_filenameLabel);
            Controls.Add(_introLabel);
            Controls.Add(_cancelButton);
            Controls.Add(_okButton);
            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SqlMetalForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SqlMetalForm";
            _includeGroupBox.ResumeLayout(false);
            _includeGroupBox.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}