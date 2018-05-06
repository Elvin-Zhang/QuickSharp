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

namespace QuickSharp.CodeAssist.DotNet
{
    partial class CreateReferenceDatabaseForm
    {
        private IContainer _components = null;
        private Label _messageLabel;
        private ProgressBar _progressBar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
                _components.Dispose();

            base.Dispose(disposing);
        }

        #region Form Layout

        private void InitializeComponent()
        {
            _messageLabel = new Label();
            _progressBar = new ProgressBar();
            SuspendLayout();

            _messageLabel.Location = new Point(28, 19);
            _messageLabel.Name = "messageLabel";
            _messageLabel.Size = new Size(304, 40);
            _messageLabel.TabIndex = 0;
            _messageLabel.Text = Resources.CreateDbDialogMessage;
            _messageLabel.TextAlign = ContentAlignment.MiddleCenter;

            _progressBar.Location = new Point(28, 77);
            _progressBar.Name = "progressBar";
            _progressBar.Size = new Size(305, 23);
            _progressBar.TabIndex = 1;

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(360, 134);
            ControlBox = false;
            Controls.Add(_progressBar);
            Controls.Add(_messageLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CreateReferenceDatabaseForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Code Assist";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}