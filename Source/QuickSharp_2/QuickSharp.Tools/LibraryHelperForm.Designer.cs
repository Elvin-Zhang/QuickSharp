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

namespace QuickSharp.Tools
{
    partial class LibraryHelperForm
    {
        private IContainer _components = null;
        private Button _okButton;
        private Button _closeButton;
        private Label _libraryHelperLabel;
        private CheckedListBox _libraryCheckedListBox;

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
            _closeButton = new Button();
            _libraryHelperLabel = new Label();
            _libraryCheckedListBox = new CheckedListBox();
            SuspendLayout();

            _okButton.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            _okButton.DialogResult = DialogResult.OK;
            _okButton.Location = new Point(216, 366);
            _okButton.Name = "okButton";
            _okButton.Size = new Size(75, 23);
            _okButton.TabIndex = 2;
            _okButton.Text = "ok";
            _okButton.UseVisualStyleBackColor = true;
            _okButton.Click += new EventHandler(OkButton_Click);

            _closeButton.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            _closeButton.DialogResult = DialogResult.Cancel;
            _closeButton.Location = new Point(297, 366);
            _closeButton.Name = "closeButton";
            _closeButton.Size = new Size(75, 23);
            _closeButton.TabIndex = 3;
            _closeButton.Text = "close";
            _closeButton.UseVisualStyleBackColor = true;

            _libraryHelperLabel.AutoSize = true;
            _libraryHelperLabel.Location = new Point(13, 13);
            _libraryHelperLabel.Name = "libraryHelperLabel";
            _libraryHelperLabel.TabIndex = 0;
            _libraryHelperLabel.Text = "copy quicksharp libraries to the workspace";

            _libraryCheckedListBox.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
                        | AnchorStyles.Left)
                        | AnchorStyles.Right)));
            _libraryCheckedListBox.CheckOnClick = true;
            _libraryCheckedListBox.FormattingEnabled = true;
            _libraryCheckedListBox.IntegralHeight = false;
            _libraryCheckedListBox.Location = new Point(16, 42);
            _libraryCheckedListBox.Name = "libraryCheckedListBox";
            _libraryCheckedListBox.Size = new Size(356, 304);
            _libraryCheckedListBox.TabIndex = 1;

            AcceptButton = _okButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _closeButton;
            ClientSize = new Size(388, 401);
            Controls.Add(_libraryCheckedListBox);
            Controls.Add(_libraryHelperLabel);
            Controls.Add(_closeButton);
            Controls.Add(_okButton);
            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            MinimizeBox = false;
            MinimumSize = new Size(300, 200);
            Name = "LibraryHelperForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "LibraryHelperForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}