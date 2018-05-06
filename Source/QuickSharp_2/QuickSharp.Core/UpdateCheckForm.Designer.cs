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
    partial class UpdateCheckForm
    {
        private IContainer _components = null;
        private Button _closeButton;
        private Label _currentVersionLabel;
        private Label _latestVersionLabel;
        private Button _checkButton;
        private Button _cancelButton;
        private LinkLabel _linkLabel;

        #region Form Control Names

        /// <summary>
        /// Name used to access the form control collection member.
        /// </summary>
        public const string m_closeButton = "closeButton";

        /// <summary>
        /// Name used to access the form control collection member.
        /// </summary>
        public const string m_currentVersionLabel = "currentVersionLabel";

        /// <summary>
        /// Name used to access the form control collection member.
        /// </summary>
        public const string m_latestVersionLabel = "latestVersionLabel";

        /// <summary>
        /// Name used to access the form control collection member.
        /// </summary>
        public const string m_checkButton = "checkButton";

        /// <summary>
        /// Name used to access the form control collection member.
        /// </summary>
        public const string m_cancelButton = "cancelButton";

        /// <summary>
        /// Name used to access the form control collection member.
        /// </summary>
        public const string m_linkLabel = "linkLabel";

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
            _closeButton = new Button();
            _currentVersionLabel = new Label();
            _latestVersionLabel = new Label();
            _checkButton = new Button();
            _cancelButton = new Button();
            _linkLabel = new LinkLabel();

            SuspendLayout();

            _closeButton.DialogResult = DialogResult.Cancel;
            _closeButton.Location = new Point(257, 141);
            _closeButton.Name = m_closeButton;
            _closeButton.Size = new Size(75, 23);
            _closeButton.TabIndex = 5;
            _closeButton.Text = "close";
            _closeButton.UseVisualStyleBackColor = true;
            _closeButton.Click += new System.EventHandler(closeButton_Click);

            _currentVersionLabel.AutoSize = true;
            _currentVersionLabel.Location = new Point(31, 55);
            _currentVersionLabel.Name = m_currentVersionLabel;
            _currentVersionLabel.TabIndex = 1;
            _currentVersionLabel.Text = "Current Version";

            _latestVersionLabel.AutoSize = true;
            _latestVersionLabel.Location = new Point(31, 25);
            _latestVersionLabel.Name = m_latestVersionLabel;
            _latestVersionLabel.TabIndex = 0;
            _latestVersionLabel.Text = "Latest Version";

            _checkButton.Location = new Point(31, 141);
            _checkButton.Name = m_checkButton;
            _checkButton.Size = new Size(75, 23);
            _checkButton.TabIndex = 3;
            _checkButton.Text = "check";
            _checkButton.UseVisualStyleBackColor = true;
            _checkButton.Click += new System.EventHandler(CheckButton_Click);

            _cancelButton.Location = new Point(112, 141);
            _cancelButton.Name = m_cancelButton;
            _cancelButton.Size = new Size(75, 23);
            _cancelButton.TabIndex = 4;
            _cancelButton.Text = "cancel";
            _cancelButton.UseVisualStyleBackColor = true;
            _cancelButton.Click += new System.EventHandler(CancelButton_Click);

            _linkLabel.AutoSize = true;
            _linkLabel.LinkBehavior = LinkBehavior.HoverUnderline;
            _linkLabel.Location = new Point(31, 85);
            _linkLabel.Name = m_linkLabel;
            _linkLabel.TabIndex = 2;
            _linkLabel.TabStop = true;
            _linkLabel.Text = "linkLabel";

            Controls.Add(_linkLabel);
            Controls.Add(_cancelButton);
            Controls.Add(_checkButton);
            Controls.Add(_latestVersionLabel);
            Controls.Add(_currentVersionLabel);
            Controls.Add(_closeButton);

            AcceptButton = _closeButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _closeButton;
            ClientSize = new Size(344, 176);
            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "UpdateCheckForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            FormClosing += new FormClosingEventHandler(UpdateCheckForm_FormClosing);
            
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
   }
}