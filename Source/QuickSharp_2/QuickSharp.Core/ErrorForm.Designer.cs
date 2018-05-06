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
    partial class ErrorForm
    {
        private IContainer _components = null;
        private Button _btnQuit;
        private Button _btnContinue;
        private Label _errorMessageCaptionLabel;
        private Label _errorMessageLabel;
        private TextBox _stackTraceTextBox;
        private Label _stackTraceCaptionLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
                _components.Dispose();

            base.Dispose(disposing);
        }

        #region Form Layout

        private void InitializeComponent()
        {
            _btnQuit = new Button();
            _btnContinue = new Button();
            _errorMessageCaptionLabel = new Label();
            _errorMessageLabel = new Label();
            _stackTraceTextBox = new TextBox();
            _stackTraceCaptionLabel = new Label();
            
            SuspendLayout();

            _btnQuit.DialogResult = DialogResult.OK;
            _btnQuit.Location = new Point(356, 293);
            _btnQuit.Name = "btnQuit";
            _btnQuit.Size = new Size(75, 23);
            _btnQuit.TabIndex = 0;
            _btnQuit.Text = Resources.ErrorButtonQuit;
            _btnQuit.UseVisualStyleBackColor = true;
            _btnQuit.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;

            _btnContinue.DialogResult = DialogResult.Cancel;
            _btnContinue.Location = new Point(437, 293);
            _btnContinue.Name = "btnContinue";
            _btnContinue.Size = new Size(75, 23);
            _btnContinue.TabIndex = 1;
            _btnContinue.Text = Resources.ErrorButtonContinue;
            _btnContinue.UseVisualStyleBackColor = true;
            _btnContinue.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;

            _errorMessageCaptionLabel.AutoSize = true;
            _errorMessageCaptionLabel.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            _errorMessageCaptionLabel.Location = new Point(12, 9);
            _errorMessageCaptionLabel.Name = "errorMessageCaptionLabel";
            _errorMessageCaptionLabel.Size = new Size(133, 13);
            _errorMessageCaptionLabel.TabIndex = 2;
            _errorMessageCaptionLabel.Text = Resources.ErrorMessage;

            _errorMessageLabel.Location = new Point(12, 33);
            _errorMessageLabel.Name = "errorMessageLabel";
            _errorMessageLabel.Size = new Size(497, 64);
            _errorMessageLabel.TabIndex = 3;
            _errorMessageLabel.Text = "errorMessageLabel";

            _stackTraceTextBox.BackColor = SystemColors.Window;
            _stackTraceTextBox.Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            _stackTraceTextBox.Location = new Point(15, 113);
            _stackTraceTextBox.Multiline = true;
            _stackTraceTextBox.Name = "stackTraceTextBox";
            _stackTraceTextBox.ReadOnly = true;
            _stackTraceTextBox.ScrollBars = ScrollBars.Both;
            _stackTraceTextBox.Size = new Size(497, 164);
            _stackTraceTextBox.TabIndex = 4;
            _stackTraceTextBox.WordWrap = false;
            _stackTraceTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Bottom |
                AnchorStyles.Left | AnchorStyles.Top;

            _stackTraceCaptionLabel.AutoSize = true;
            _stackTraceCaptionLabel.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            _stackTraceCaptionLabel.Location = new Point(12, 97);
            _stackTraceCaptionLabel.Name = "stackTraceCaptionLabel";
            _stackTraceCaptionLabel.Size = new Size(75, 13);
            _stackTraceCaptionLabel.TabIndex = 5;
            _stackTraceCaptionLabel.Text = Resources.ErrorStackTrace;
            
            Controls.Add(_stackTraceCaptionLabel);
            Controls.Add(_stackTraceTextBox);
            Controls.Add(_errorMessageLabel);
            Controls.Add(_errorMessageCaptionLabel);
            Controls.Add(_btnContinue);
            Controls.Add(_btnQuit);

            AcceptButton = _btnQuit;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _btnContinue;
            ClientSize = new Size(524, 328);
            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            FormBorderStyle = FormBorderStyle.Sizable;
            Icon = Resources.ErrorFormIcon;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ErrorForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = Resources.ErrorTitle;
            MinimumSize = new Size(400, 300);

            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}