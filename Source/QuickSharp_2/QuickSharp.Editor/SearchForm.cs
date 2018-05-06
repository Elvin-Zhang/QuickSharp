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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ScintillaNet;
using QuickSharp.Core;
using System.Text.RegularExpressions;

/*
 * Search/Replace may be performed with simple text matching or
 * regular expressions. Simple text matching may be case-sensitive
 * or case-insensitive for 'find next'/'find all' but case-sensitive
 * only for 'replace next'/'replace all'. Simple text matching will
 * only work with single line text items and generates bookmarks for
 * 'find all' and 'replace all' operations. Regular expresion
 * search/replace can search for multi-line items given the right
 * regex pattern. Bookmarks are only generated for 'find all'
 * operations.
 */

namespace QuickSharp.Editor
{
    /// <summary>
    /// The editor find/replace form.
    /// </summary>
    public partial class SearchForm : Form
    {
        private bool _isReplace;
        private bool _isSearchAll;

        /// <summary>
        /// Create the form.
        /// </summary>
        public SearchForm()
        {
            InitializeComponent();

            // Allow client applications to modify the form.
            SearchFormProxy.GetInstance().UpdateFormControls(Controls);
        }

        /// <summary>
        /// Show the form as a find dialog.
        /// </summary>
        public void ShowFind()
        {
            _findTextComboBox.BackColor = Color.Empty;
            _replaceWithLabel.Visible = false;
            _replaceTextComboBox.Visible = false;
            _matchCaseCheckBox.Visible = true;
            _searchMessageLabel.Text = String.Empty;
            _findButton.Text = Resources.FindDialogButtonFind;
            _findAllButton.Text = Resources.FindDialogButtonFindAllFind;
            _isReplace = false;

            UpdateCheckBoxes();
            Text = Resources.FindDialogTitleFind;
            Visible = true;

            _findTextComboBox.Focus();
        }

        /// <summary>
        /// Show the form as a replace dialog.
        /// </summary>
        public void ShowReplace()
        {
            _findTextComboBox.BackColor = Color.Empty;
            _replaceWithLabel.Visible = true;
            _replaceTextComboBox.Visible = true;
            _matchCaseCheckBox.Visible = false;
            _searchMessageLabel.Text = String.Empty;
            _findButton.Text = Resources.FindDialogButtonReplace;
            _findAllButton.Text = Resources.FindDialogButtonFindAllReplace;
            _isReplace = true;

            UpdateCheckBoxes();
            Text = Resources.FindDialogTitleReplace;
            Visible = true;

            _findTextComboBox.Focus();
        }

        #region Event Handlers

        private void SearchForm_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible) return;

            /*
             * If we have an editor see if we have any selected text.
             */

            ScintillaEditForm sci = GetActiveEditor();
            if (sci != null)
            {
                /*
                 * May be multiline so we split at first newline
                 * after normalizing for Windows/Mac/Unix differences.
                 */

                string text = sci.Editor.Selection.Text;
                text = text.Replace("\n", "\r");
                int i = text.IndexOf("\r");
                if (i != -1) text = text.Substring(0, i);

                _findTextComboBox.Text = text;
                _findTextComboBox.SelectAll();
            }
        }

        private void SearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Don't close just hide
            e.Cancel = true;
            Visible = false;
        }

        private void UpdateCheckBoxes()
        {
            _matchCaseCheckBox.Enabled = !_useRegexCheckBox.Checked;
        }

        private void Search(bool searchAll)
        {
            _isSearchAll = searchAll;

            ScintillaEditForm sci = GetActiveEditor();
            if (sci == null)
            {
                _searchMessageLabel.Text = Resources.FindDialogMessageCantSearch;
                return;
            }

            _findTextComboBox.BackColor = Color.Empty;
            _searchMessageLabel.Text = String.Empty;

            string searchText = _findTextComboBox.Text;
            if (searchText == String.Empty)
            {
                _findTextComboBox.BackColor = Color.Yellow;
                return;
            }

            AddHistoryItem(_findTextComboBox, searchText);

            string replaceText = String.Empty;
            if (_isReplace)
            {
                replaceText = _replaceTextComboBox.Text;
                AddHistoryItem(_replaceTextComboBox, replaceText);
            }

            bool isRegex = _useRegexCheckBox.Checked;
            bool matchCase = _matchCaseCheckBox.Checked;

            try
            {
                Cursor = Cursors.WaitCursor;

                if (_isSearchAll)
                {
                    if (isRegex)
                        FindAllRegex(searchText, replaceText, _isReplace);
                    else
                        FindAll(searchText, replaceText, _isReplace, matchCase);
                }
                else
                {
                    if (isRegex)
                        FindNextRegex(searchText, replaceText, _isReplace);
                    else
                        FindNext(searchText, replaceText, _isReplace, matchCase);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}:\n{1}",
                    Resources.FindDialogExceptionMessage, ex.Message),
                    _isReplace ?
                        Resources.FindDialogTitleReplace :
                        Resources.FindDialogTitleFind,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Search

        private void FindNext(string searchText, string replaceText,
            bool isReplace, bool matchCase)
        {
            ScintillaEditForm sci = GetActiveEditor();
            if (sci == null) return;

            // Replace is always case-sensitive
            if (isReplace)
                matchCase = true;

            if (!matchCase)
                searchText = searchText.ToLower();

            /*
             * Is there an instance anywhere in the document?
             */

            string text = matchCase ? 
                sci.Editor.Text : sci.Editor.Text.ToLower();
            
            int index = text.IndexOf(searchText);

            if (index == -1)
            {
                _searchMessageLabel.Text = Resources.FindDialogMessageNotFound;
                return;
            }

            /*
             * We have at least one instance, search from the cursor if not at document end.
             */

            if (sci.Editor.CurrentPos < sci.Editor.Text.Length)
                index = text.IndexOf(searchText, sci.Editor.CurrentPos);

            if (index == -1)    // Search from the start of the document
                index = text.IndexOf(searchText);

            if (index == -1)    // Shouldn't happen
                return;

            sci.Editor.Selection.Start = index;
            sci.Editor.Selection.Length = searchText.Length;

            if (_isReplace)
            {
                sci.Editor.Selection.Text = replaceText;
                sci.Editor.Selection.Start = index;
                sci.Editor.Selection.Length = replaceText.Length;
            }
        }

        private void FindNextRegex(string searchText, string replaceText, bool isReplace)
        {
            ScintillaEditForm sci = GetActiveEditor();
            if (sci == null) return;

            Regex searchRegex = new Regex(searchText);

            // Is there an instance anywhere in the document?
            string text = sci.Editor.Text;

            Match match = searchRegex.Match(text);

            if (!match.Success)
            {
                _searchMessageLabel.Text = Resources.FindDialogMessageNotFound;
                return;
            }

            // Have at least one instance, search from the cursor if not at document end.
            if (sci.Editor.CurrentPos < sci.Editor.Text.Length)
                match = searchRegex.Match(text, sci.Editor.CurrentPos);

            if (!match.Success)    // Search from the start of the document
                match = searchRegex.Match(text);

            if (!match.Success)    // Shouldn't happen
                return;

            sci.Editor.Selection.Start = match.Index;
            sci.Editor.Selection.Length = match.Length;

            if (_isReplace)
            {
                sci.Editor.Selection.Text = replaceText;
                sci.Editor.Selection.Start = match.Index;
                sci.Editor.Selection.Length = replaceText.Length;
            }
        }

        private void FindAll(string searchText, string replaceText,
            bool isReplace, bool matchCase)
        {
            ScintillaEditForm sci = GetActiveEditor();
            if (sci == null) return;

            // Clear all bookmarks
            sci.Editor.Markers.DeleteAll(Constants.BOOKMARK_MARKER);

            // Replace is always case-sensitive
            if (isReplace)
                matchCase = true;

            if (!matchCase)
                searchText = searchText.ToLower();

            int lineCount = 0;

            if (isReplace)
            {
                sci.Editor.UndoRedo.BeginUndoAction();

                foreach (Line line in sci.Editor.Lines)
                {
                    string text = line.Text;

                    if (text.IndexOf(searchText) != -1)
                    {
                        string s = line.Text.Replace(searchText, replaceText);

                        /*
                         * Line needs to be trimmed of any EOL characters to prevent 
                         * extra blank lines being inserted.
                         */

                        line.Text = s.TrimEnd(new Char[] { '\n', '\r' });
                        line.AddMarker(Constants.BOOKMARK_MARKER);
                        lineCount++;
                    }
                }

                sci.Editor.UndoRedo.EndUndoAction();
            }
            else
            {
                foreach (Line line in sci.Editor.Lines)
                {
                    string text = matchCase ? line.Text : line.Text.ToLower();

                    if (text.IndexOf(searchText) != -1)
                    {
                        line.AddMarker(Constants.BOOKMARK_MARKER);
                        lineCount++;
                    }
                }
            }

            if (lineCount == 1)
                _searchMessageLabel.Text = String.Format(isReplace ?
                    Resources.FindDialogMessageReplacedAllSingle :
                    Resources.FindDialogMessageFoundSingle, lineCount);
            else
                _searchMessageLabel.Text = String.Format(isReplace ?
                    Resources.FindDialogMessageReplacedAllMulti : 
                    Resources.FindDialogMessageFoundMulti, lineCount);
        }

        private void FindAllRegex(string searchText, string replaceText, bool isReplace)
        {
            ScintillaEditForm sci = GetActiveEditor();
            if (sci == null) return;

            // Clear all bookmarks
            sci.Editor.Markers.DeleteAll(Constants.BOOKMARK_MARKER);

            Regex searchRegex = new Regex(searchText);
            MatchCollection matches = searchRegex.Matches(sci.Editor.Text);
            
            int lineNumber = -1;
            int lineCount = 0;
            
            foreach (Match match in matches)
            {
                Line line = sci.Editor.Lines.FromPosition(match.Index);
                if (line == null) continue;
                
                // Only report each line once.
                if (lineNumber != line.Number)
                {
                    if (!isReplace)
                        line.AddMarker(Constants.BOOKMARK_MARKER);

                    lineNumber = line.Number;
                    lineCount++;
                }
            }

            if (isReplace)
            {
                /*
                 * Can't track the actual change locations using this method so
                 * we add a sentinel string to mark the lines on which the changes
                 * occur. Then we bookmark each line with a sentinel and remove it.
                 * This is a bit hacky and would break if the sentinel actually
                 * appears in the text but it's the only way to do multi-line
                 * replaces and still preserve the bookmarks in the right places.
                 */

                string sentinel = "{^¬%`^}";

                sci.Editor.UndoRedo.BeginUndoAction();

                sci.Editor.Text = searchRegex.Replace(
                    sci.Editor.Text, sentinel + replaceText);
                
                foreach (Line line in sci.Editor.Lines)
                {
                    if (line.Text.IndexOf(sentinel) != -1)
                    {
                        string s = line.Text.Replace(sentinel, String.Empty);

                        /*
                         * Line needs to be trimmed of any EOL characters to prevent 
                         * extra blank lines being inserted.
                         */

                        line.Text = s.TrimEnd(new Char[] { '\n', '\r' });
                        line.AddMarker(Constants.BOOKMARK_MARKER);
                    }
                }

                sci.Editor.UndoRedo.EndUndoAction();
            }

            if (lineCount == 1)
                _searchMessageLabel.Text = String.Format(isReplace ?
                    Resources.FindDialogMessageReplacedAllSingle :
                    Resources.FindDialogMessageFoundSingle, lineCount);
            else
                _searchMessageLabel.Text = String.Format(isReplace ?
                    Resources.FindDialogMessageReplacedAllMulti :
                    Resources.FindDialogMessageFoundMulti, lineCount);
        }

        #endregion

        #region Helpers

        private ScintillaEditForm GetActiveEditor()
        {
            MainForm mainForm = ApplicationManager.
                GetInstance().MainForm;
            
            if (mainForm.ClientWindow.DocumentsCount == 0)
                return null;

            return mainForm.ClientWindow.ActiveDocument
                as ScintillaEditForm;
        }

        private void AddHistoryItem(ComboBox cbo, string item)
        {
            int index = cbo.Items.IndexOf(item);
            if (index != -1) cbo.Items.RemoveAt(index);
            cbo.Items.Insert(0, item);
            cbo.SelectedIndex = 0;
        }

        #endregion
    }
}
