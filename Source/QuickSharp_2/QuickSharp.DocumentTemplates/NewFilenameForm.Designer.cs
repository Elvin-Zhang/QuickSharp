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

namespace QuickSharp.DocumentTemplates
{
    partial class NewFilenameForm
    {
        private IContainer _components = null;
        private Button _okButton;
        private Button _cancelButton;
        private Label _filenameLabel;
        private TextBox _filenameTextBox;
        private Label _templateLabel;

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
            _filenameLabel = new Label();
            _filenameTextBox = new TextBox();
            _templateLabel = new Label();
            SuspendLayout();

            _okButton.Location = new Point(207, 121);
            _okButton.Name = "okButton";
            _okButton.Size = new Size(75, 23);
            _okButton.TabIndex = 2;
            _okButton.Text = global::QuickSharp.DocumentTemplates.Resources.NewFileFormOK;
            _okButton.UseVisualStyleBackColor = true;
            _okButton.Click += new System.EventHandler(OkButton_Click);

            _cancelButton.DialogResult = DialogResult.Cancel;
            _cancelButton.Location = new Point(288, 121);
            _cancelButton.Name = "cancelButton";
            _cancelButton.Size = new Size(75, 23);
            _cancelButton.TabIndex = 3;
            _cancelButton.Text = global::QuickSharp.DocumentTemplates.Resources.NewFileFormCancel;
            _cancelButton.UseVisualStyleBackColor = true;

            _filenameLabel.AutoSize = true;
            _filenameLabel.Location = new Point(9, 52);
            _filenameLabel.Name = "filenameLabel";
            _filenameLabel.TabIndex = 0;
            _filenameLabel.Text = "&Enter a name for the new item:";

            _filenameTextBox.Location = new Point(12, 68);
            _filenameTextBox.Name = "filenameTextBox";
            _filenameTextBox.Size = new Size(351, 21);
            _filenameTextBox.TabIndex = 1;

            _templateLabel.AutoSize = true;
            _templateLabel.Location = new Point(9, 18);
            _templateLabel.Name = "templateLabel";
            _templateLabel.TabIndex = 4;
            _templateLabel.Text = "New From Template";

            AcceptButton = _okButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _cancelButton;
            ClientSize = new Size(375, 156);
            Controls.Add(_templateLabel);
            Controls.Add(_filenameTextBox);
            Controls.Add(_filenameLabel);
            Controls.Add(_cancelButton);
            Controls.Add(_okButton);
            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NewFilenameForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "New From Template";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}