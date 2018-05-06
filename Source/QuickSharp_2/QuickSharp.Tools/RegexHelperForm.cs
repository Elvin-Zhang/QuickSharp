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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using QuickSharp.Core;
using QuickSharp.Editor;
using ScintillaNet;

namespace QuickSharp.Tools
{
    public partial class RegexHelperForm : Form
    {
        private List<String> _regexHistory;

        public RegexHelperForm(List<String> regexHistory)
        {
            InitializeComponent();
            
            _regexHistory = regexHistory;

            UpdateComboBoxList();

            // Allow client applications to modify the form.
            RegexHelperFormProxy.GetInstance().
                UpdateFormControls(Controls);
        }

        #region Event Handlers

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MatchButton_Click(object sender, EventArgs e)
        {
            _regexComboBox.BackColor = Color.Empty;

            string regex = _regexComboBox.Text.Trim();

            if (String.IsNullOrEmpty(regex))
            {
                _regexComboBox.BackColor = Color.Yellow;
                return;
            }
            
            MainForm mainForm = ApplicationManager.
                GetInstance().MainForm;

            ScintillaEditForm document = 
                mainForm.ActiveDocument as ScintillaEditForm;
            
            if (document == null) return;

            string target = document.GetContent() as string;
            if (String.IsNullOrEmpty(target)) return;

            _treeView.Nodes.Clear();

            Regex re = null;
            
            try
            {
                re = new Regex(regex);
                MatchCollection mc = re.Matches(target);

                string rootText = String.Format("{0} {1}",
                    mc.Count,
                    (mc.Count == 1) ?
                        Resources.ResultMatch :
                        Resources.ResultMatches
                    );
                
                if (mc.Count > 0)
                    rootText += String.Format(" ({0} {1})",
                        mc[0].Groups.Count,
                        (mc[0].Groups.Count == 1) ?
                            Resources.ResultGroup :
                            Resources.ResultGroups
                        );

                TreeNode rootNode = new TreeNode(rootText);
                rootNode.ImageKey = Constants.IMAGE_KEY_SEARCH;
                rootNode.SelectedImageKey = Constants.IMAGE_KEY_SEARCH;
                _treeView.Nodes.Add(rootNode);

                foreach (Match m in mc)
                {
                    string matchText = String.Format("{0} {1}: [{2}]",
                        Resources.ResultTextLine, 
                        LineFromPosition(document.Lines, m.Index),
                        m.Value);
                    
                    TreeNode matchNode = new TreeNode(matchText);
                    matchNode.ImageKey = Constants.IMAGE_KEY_MATCH;
                    matchNode.SelectedImageKey = Constants.IMAGE_KEY_MATCH;

                    int groupCount = 0;
                    foreach (Group g in m.Groups)
                    {
                        string s = String.Format("{0,2:00} ({1}): [{2}]",
                            groupCount, re.GroupNameFromNumber(groupCount), g.Value);

                        TreeNode n = new TreeNode(s);
                        n.ImageKey = Constants.IMAGE_KEY_GROUP;
                        n.SelectedImageKey = Constants.IMAGE_KEY_GROUP;
                        matchNode.Nodes.Add(n);

                        groupCount++;
                    }

                    rootNode.Nodes.Add(matchNode);
                }

                rootNode.Expand();

                AddItemToHistory(regex);
                UpdateComboBoxList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    Resources.RegexErrorDialogTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Helpers

        private int LineFromPosition(LinesCollection lines, int pos)
        {
            int line = 1;

            foreach (Line l in lines)
            {
                if ((pos >= l.StartPosition) && (pos <= l.EndPosition)) return line;
                line++;
            }

            return line;
        }

        private void UpdateComboBoxList()
        {
            _regexComboBox.Items.Clear();

            if (_regexHistory.Count == 0) return;

            foreach (string s in _regexHistory)
                _regexComboBox.Items.Add(s);
        }

        private void AddItemToHistory(string item)
        {
            int index = _regexHistory.IndexOf(item);

            if (index != -1)
                _regexHistory.RemoveAt(index);

            _regexHistory.Insert(0, item);
        }

        #endregion
    }
}
