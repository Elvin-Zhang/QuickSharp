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
using QuickSharp.Core;
using System.ComponentModel;
 
namespace QuickSharp.CodeAssist.DotNet
{
    partial class ManageReferenceDatabaseForm
    {
        private IContainer _components = null;
        private Button _cancelButton;
        private CheckedListBox _checkedListBox;
        private Button _okButton;
        private Button _addFileButton;
        private Button _addFolderButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
                _components.Dispose();

            base.Dispose(disposing);
        }

        #region Form Layout

        private void InitializeComponent()
        {
            _cancelButton = new Button();
            _checkedListBox = new CheckedListBox();
            _okButton = new Button();
            _addFileButton = new Button();
            _addFolderButton = new Button();
            SuspendLayout();

            _cancelButton.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            _cancelButton.DialogResult = DialogResult.Cancel;
            _cancelButton.Location = new Point(344, 330);
            _cancelButton.Name = "cancelButton";
            _cancelButton.Size = new Size(75, 23);
            _cancelButton.TabIndex = 4;
            _cancelButton.Text = global::QuickSharp.CodeAssist.DotNet.Resources.ReferencesFormCancel;
            _cancelButton.UseVisualStyleBackColor = true;

            _checkedListBox.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
                        | AnchorStyles.Left)
                        | AnchorStyles.Right)));
            _checkedListBox.CheckOnClick = true;
            _checkedListBox.HorizontalScrollbar = true;
            _checkedListBox.IntegralHeight = false;
            _checkedListBox.Location = new Point(12, 12);
            _checkedListBox.Name = "checkedListBox";
            _checkedListBox.Size = new Size(407, 308);
            _checkedListBox.Sorted = true;
            _checkedListBox.TabIndex = 0;

            _okButton.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            _okButton.DialogResult = DialogResult.OK;
            _okButton.Location = new Point(263, 330);
            _okButton.Name = "okButton";
            _okButton.Size = new Size(75, 23);
            _okButton.TabIndex = 3;
            _okButton.Text = QuickSharp.CodeAssist.DotNet.Resources.ReferencesFormOK;
            _okButton.UseVisualStyleBackColor = true;
            _okButton.Click += new EventHandler(OkButton_Click);

            _addFileButton.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
            _addFileButton.Location = new Point(12, 330);
            _addFileButton.Name = "addFileButton";
            _addFileButton.Size = new Size(75, 23);
            _addFileButton.TabIndex = 1;
            _addFileButton.Text = QuickSharp.CodeAssist.DotNet.Resources.ReferencesFormAddFile;
            _addFileButton.UseVisualStyleBackColor = true;
            _addFileButton.Click += new EventHandler(AddFileButton_Click);

            _addFolderButton.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
            _addFolderButton.Location = new Point(93, 330);
            _addFolderButton.Name = "addFolderButton";
            _addFolderButton.Size = new Size(92, 23);
            _addFolderButton.TabIndex = 2;
            _addFolderButton.Text = QuickSharp.CodeAssist.DotNet.Resources.ReferencesFormAddFolder;
            _addFolderButton.UseVisualStyleBackColor = true;
            _addFolderButton.Click += new EventHandler(AddFolderButton_Click);

            AcceptButton = _okButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _cancelButton;
            ClientSize = new Size(431, 365);
            Controls.Add(_addFolderButton);
            Controls.Add(_addFileButton);
            Controls.Add(_okButton);
            Controls.Add(_checkedListBox);
            Controls.Add(_cancelButton);
            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            MinimizeBox = false;
            MinimumSize = new Size(400, 200);
            Name = "ManageReferenceDatabaseForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = Resources.ReferencesFormTitle;
            ResumeLayout(false);
        }

        #endregion
    }
}