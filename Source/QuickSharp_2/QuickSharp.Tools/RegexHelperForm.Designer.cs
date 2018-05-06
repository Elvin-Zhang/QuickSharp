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
    partial class RegexHelperForm
    {
        private IContainer _components = null;
        private Button _closeButton;
        private ComboBox _regexComboBox;
        private Button _matchButton;
        private TreeView _treeView;
        private ImageList _imageList;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
                _components.Dispose();

            base.Dispose(disposing);
        }

        #region Form Layout

        private void InitializeComponent()
        {
            _components = new Container();
            _closeButton = new Button();
            _regexComboBox = new ComboBox();
            _matchButton = new Button();
            _treeView = new TreeView();
            _imageList = new ImageList(_components);
            SuspendLayout();

            _closeButton.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Right)));
            _closeButton.DialogResult = DialogResult.OK;
            _closeButton.Location = new Point(409, 39);
            _closeButton.Name = "closeButton";
            _closeButton.Size = new Size(75, 23);
            _closeButton.TabIndex = 2;
            _closeButton.Text = Resources.RegexDialogButtonClose;
            _closeButton.UseVisualStyleBackColor = true;
            _closeButton.Click += new System.EventHandler(CloseButton_Click);

            _regexComboBox.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
                        | AnchorStyles.Right)));
            _regexComboBox.FormattingEnabled = true;
            _regexComboBox.Location = new Point(12, 12);
            _regexComboBox.Name = "regexComboBox";
            _regexComboBox.Size = new Size(472, 21);
            _regexComboBox.TabIndex = 0;

            _matchButton.Location = new Point(12, 39);
            _matchButton.Name = "matchButton";
            _matchButton.Size = new Size(75, 23);
            _matchButton.TabIndex = 1;
            _matchButton.Text = Resources.RegexDialogButtonMatch;
            _matchButton.UseVisualStyleBackColor = true;
            _matchButton.Click += new System.EventHandler(MatchButton_Click);

            _treeView.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
                        | AnchorStyles.Left)
                        | AnchorStyles.Right)));
            _treeView.ImageIndex = 0;
            _treeView.ImageList = _imageList;
            _treeView.Location = new Point(12, 68);
            _treeView.Name = "treeView";
            _treeView.SelectedImageIndex = 0;
            _treeView.Size = new Size(472, 244);
            _treeView.TabIndex = 3;

            _imageList.TransparentColor = Color.Fuchsia;
            _imageList.Images.Add("SEARCH", Resources.Search);
            _imageList.Images.Add("MATCH", Resources.Match);
            _imageList.Images.Add("GROUP", Resources.Group);

            AcceptButton = _matchButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _closeButton;
            ClientSize = new Size(496, 324);
            Controls.Add(_treeView);
            Controls.Add(_matchButton);
            Controls.Add(_regexComboBox);
            Controls.Add(_closeButton);
            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            MinimumSize = new Size(200, 200);
            Name = "RegexHelperForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = Resources.RegexDialogTitle;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}