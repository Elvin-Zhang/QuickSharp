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

namespace QuickSharp.Core
{
    partial class OptionsForm
    {
        private IContainer components = null;
        private Button okButton;
        private Button cancelButton;
        private TreeView optionsTreeView;
        private Panel optionsPanel;

        #region Form Control Names

        /// <summary>
        /// Name used to access the form control collection member.
        /// </summary>
        public const string m_okButton = "okButton";

        /// <summary>
        /// Name used to access the form control collection member.
        /// </summary>
        public const string m_cancelButton = "cancelButton";

        /// <summary>
        /// Name used to access the form control collection member.
        /// </summary>
        public const string m_optionsTreeView = "optionsTreeView";

        /// <summary>
        /// Name used to access the form control collection member.
        /// </summary>
        public const string m_optionsPanel = "optionsPanel";

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Form Layout

        private void InitializeComponent()
        {
            okButton = new Button();
            cancelButton = new Button();
            optionsTreeView = new TreeView();
            optionsPanel = new Panel();

            SuspendLayout();

            optionsTreeView.Location = new Point(12, 12);
            optionsTreeView.Name = m_optionsTreeView;
            optionsTreeView.Size = new Size(200, 250);
            optionsTreeView.TabIndex = 0;
            optionsTreeView.HideSelection = false;
            optionsTreeView.AfterSelect +=
                new TreeViewEventHandler(OptionsTreeView_AfterSelect);
 
            optionsPanel.Location = new Point(222, 12);
            optionsPanel.Name = m_optionsPanel;
            optionsPanel.Size = new Size(430, 250);
            optionsPanel.TabIndex = 1;
            optionsPanel.AutoScroll = true;

            okButton.Location = new Point(497, 283);
            okButton.Name = m_okButton;
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 2;
            okButton.Text = Resources.OptionsButtonOK;
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += new EventHandler(OkButton_Click);

            cancelButton.Location = new Point(578, 283);
            cancelButton.Name = m_cancelButton;
            cancelButton.Size = new Size(75, 23);
            cancelButton.TabIndex = 3;
            cancelButton.Text = Resources.OptionsButtonCancel;
            cancelButton.UseVisualStyleBackColor = true;

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            AcceptButton = okButton;
            CancelButton = cancelButton;
            ClientSize = new Size(664, 318);

            Controls.Add(cancelButton);
            Controls.Add(optionsTreeView);
            Controls.Add(optionsPanel);
            Controls.Add(okButton);

            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "OptionsForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = Resources.OptionsTitle;
            ResumeLayout(false);
        }

        #endregion
    }
}