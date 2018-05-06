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

namespace QuickSharp.Editor
{
    partial class SearchForm
    {
        private IContainer _components = null;
        private Label _findWhatLabel;
        private ComboBox _findTextComboBox;
        private Label _replaceWithLabel;
        private ComboBox _replaceTextComboBox;
        private Label _searchMessageLabel;
        private CheckBox _useRegexCheckBox;
        private CheckBox _matchCaseCheckBox;
        private Button _findButton;
        private Button _findAllButton;
        private Button _closeButton;

        #region Form Control Names

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_findWhatLabel = "findWhatLabel";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_findTextComboBox = "findTextComboBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_replaceWithLabel = "replaceWithLabel";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_replaceTextComboBox = "replaceTextComboBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_searchMessageLabel = "searchMessageLabel";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_useRegexCheckBox = "useRegexCheckBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_matchCaseCheckBox = "matchCaseCheckBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_findButton = "findButton";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_findAllButton = "findAllButton";

        #endregion

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_closeButton = "closeButton";

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
                _components.Dispose();

            base.Dispose(disposing);
        }

        #region Form Layout

        private void InitializeComponent()
        {
            SuspendLayout();

            #region Controls

            _findWhatLabel = new Label();
            _findTextComboBox = new ComboBox();
            _replaceWithLabel = new Label();
            _replaceTextComboBox = new ComboBox();
            _searchMessageLabel = new Label();
            _useRegexCheckBox = new CheckBox();
            _matchCaseCheckBox = new CheckBox();
            _findButton = new Button();
            _findAllButton = new Button();
            _closeButton = new Button();

            _findWhatLabel.AutoSize = true;
            _findWhatLabel.Location = new Point(12, 9);
            _findWhatLabel.Name = m_findWhatLabel;
            _findWhatLabel.TabIndex = 0;
            _findWhatLabel.Text = Resources.FindDialogFindWhat;

            _findTextComboBox.FormattingEnabled = true;
            _findTextComboBox.Location = new Point(15, 25);
            _findTextComboBox.Name = m_findTextComboBox;
            _findTextComboBox.Size = new Size(369, 21);
            _findTextComboBox.TabIndex = 1;

            _replaceWithLabel.AutoSize = true;
            _replaceWithLabel.Location = new Point(12, 49);
            _replaceWithLabel.Name = m_replaceWithLabel;
            _replaceWithLabel.TabIndex = 2;
            _replaceWithLabel.Text = Resources.FindDialogReplaceWith;

            _replaceTextComboBox.FormattingEnabled = true;
            _replaceTextComboBox.Location = new Point(15, 65);
            _replaceTextComboBox.Name = m_replaceTextComboBox;
            _replaceTextComboBox.Size = new Size(369, 21);
            _replaceTextComboBox.TabIndex = 3;

            _searchMessageLabel.AutoSize = true;
            _searchMessageLabel.Location = new Point(12, 100);
            _searchMessageLabel.Name = m_searchMessageLabel;
            _searchMessageLabel.TabIndex = 4;
            _searchMessageLabel.Text = String.Empty;

            _useRegexCheckBox.AutoSize = true;
            _useRegexCheckBox.Location = new Point(15, 138);
            _useRegexCheckBox.Name = m_useRegexCheckBox;
            _useRegexCheckBox.TabIndex = 6;
            _useRegexCheckBox.Text = Resources.FindDialogUseRegularExpression;
            _useRegexCheckBox.UseVisualStyleBackColor = true;
            _useRegexCheckBox.CheckedChanged += delegate { UpdateCheckBoxes(); };

            _matchCaseCheckBox.AutoSize = true;
            _matchCaseCheckBox.Location = new Point(165, 138);
            _matchCaseCheckBox.Name = m_matchCaseCheckBox;
            _matchCaseCheckBox.TabIndex = 7;
            _matchCaseCheckBox.Text = Resources.FindDialogMatchCase;
            _matchCaseCheckBox.UseVisualStyleBackColor = true;

            _findButton.Location = new Point(14, 178);
            _findButton.Name = m_findButton;
            _findButton.Size = new Size(90, 23);
            _findButton.TabIndex = 8;
            _findButton.Text = Resources.FindDialogButtonFind;
            _findButton.UseVisualStyleBackColor = true;
            _findButton.Click += delegate { Search(false); };

            _findAllButton.Location = new Point(120, 178);
            _findAllButton.Name = m_findAllButton;
            _findAllButton.Size = new Size(90, 23);
            _findAllButton.TabIndex = 9;
            _findAllButton.Text = Resources.FindDialogButtonFindAllReplace;
            _findAllButton.UseVisualStyleBackColor = true;
            _findAllButton.Click += delegate { Search(true); };

            _closeButton.Location = new Point(309, 178);
            _closeButton.Name = m_closeButton;
            _closeButton.Size = new Size(75, 23);
            _closeButton.TabIndex = 10;
            _closeButton.Text = Resources.FindDialogButtonClose;
            _closeButton.UseVisualStyleBackColor = true;
            _closeButton.Click += delegate { Close(); };

            Controls.Add(_findWhatLabel);
            Controls.Add(_findTextComboBox);
            Controls.Add(_replaceWithLabel);
            Controls.Add(_replaceTextComboBox);
            Controls.Add(_searchMessageLabel);
            Controls.Add(_useRegexCheckBox);
            Controls.Add(_matchCaseCheckBox);
            Controls.Add(_findButton);
            Controls.Add(_findAllButton);
            Controls.Add(_closeButton);

            #endregion

            AcceptButton = _findButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _closeButton;
            ClientSize = new Size(396, 213);
            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            Name = "FindInFilesForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            TopMost = true;
            Visible = false;
            Text = Resources.FindDialogTitleFind;
            VisibleChanged += new EventHandler(SearchForm_VisibleChanged);
            FormClosing += new FormClosingEventHandler(SearchForm_FormClosing);

            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}