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
using System.Windows.Forms;

namespace QuickSharp.Core
{
    /// <summary>
    /// Provides the options editor to allow plugins to present individual
    /// settings control pages. The options form contains each page and provides
    /// a single 'Save' button to initiate saving of each option pages settings.
    /// </summary>
    public partial class OptionsForm : Form
    {
        private class PageNode : TreeNode { }

        private Dictionary<String, OptionsPage> _pages;
        private Dictionary<String, List<String>> _groups;

        /// <summary>
        /// The name of the currently selected options page. Used to
        /// enable the form's caller to save the active page for 
        /// restoration when the form is next shown.
        /// </summary>
        public string SelectedPageName { get; private set; }

        /// <summary>
        /// Create an instance of the application options editor.
        /// </summary>
        public OptionsForm()
        {
            ApplicationManager _applicationManager =
                ApplicationManager.GetInstance();

            InitializeComponent();

            /*
             * Get the form proxy so we can call any update events.
             */

            OptionsFormProxy formProxy = OptionsFormProxy.GetInstance();

            /*
             * Call each registered page factory to get the pages.
             */

            _pages = new Dictionary<String, OptionsPage>();

            foreach (OptionsPageFactoryHandler d in
                _applicationManager.OptionsPageFactoryHandlers)
            {
                OptionsPage page = d();

                if (page != null)
                    _pages[page.Name] = page;
            }

            formProxy.UpdateOptionsFormPages(_pages);

            /*
             * Create the groups and assign the child pages.
             */

            _groups = new Dictionary<String, List<String>>();

            foreach (OptionsPage page in _pages.Values)
            {
                if (!_groups.ContainsKey(page.GroupText))
                    _groups[page.GroupText] = new List<String>();

                _groups[page.GroupText].Add(page.Name);
            }

            /*
             * Populate the tree.
             */

            optionsTreeView.SuspendLayout();

            foreach (string groupText in _groups.Keys)
            {
                /*
                 * Groups will be empty, have one page or several pages.
                 * If group has only one page, the group will access the
                 * page directly, otherwise it will be the parent of the
                 * sub pages.
                 */

                if (_groups[groupText].Count < 1) continue;

                if (_groups[groupText].Count == 1)
                {
                    PageNode groupNode = new PageNode();
                    groupNode.Text = groupText;
                    groupNode.Name = _groups[groupText][0];

                    optionsTreeView.Nodes.Add(groupNode);
                }
                else if (_groups[groupText].Count > 1)
                {
                    TreeNode groupNode = new TreeNode();
                    groupNode.Text = groupText;

                    foreach (string pageName in _groups[groupText])
                    {
                        OptionsPage page = _pages[pageName];

                        PageNode pageNode = new PageNode();
                        pageNode.Text = page.PageText;
                        pageNode.Name = page.Name;

                        groupNode.Nodes.Add(pageNode);
                    }

                    optionsTreeView.Nodes.Add(groupNode);
                }

                optionsTreeView.ResumeLayout(true);
            }

            /*
             * Call the control update proxy.
             */

            formProxy.UpdateOptionsFormControls(Controls);

            /*
             * Sort the top level nodes.
             */

            if (optionsTreeView.Nodes.Count > 0)
            {
                optionsTreeView.TreeViewNodeSorter = new TreeNodeSorter();
                optionsTreeView.Sort();
            }
        }

        private class TreeNodeSorter : System.Collections.IComparer
        {
            public int Compare(object o1, object o2)
            {
                TreeNode n1 = o1 as TreeNode;
                TreeNode n2 = o2 as TreeNode;

                /*
                 * Just sort the top level so that the sub nodes
                 * can be presented in the order they were registered.
                 */

                if (n1.Parent != null) return 0;

                return String.Compare(n1.Text, n2.Text);
            }
        }

        #region Page Selection

        /// <summary>
        /// Select the options page to be visible when the options form is shown.
        /// (Internal use only).
        /// </summary>
        /// <param name="name">The name of the page.</param>
        public void SelectPage(string name)
        {
            SelectPage(name, true);
        }

        /// <summary>
        /// Select the options page to be visible when the options form is shown.
        /// (Internal use only).
        /// </summary>
        /// <param name="name">The name of the page.</param>
        /// <param name="selectNode">Select the corresponding tree node.</param>
        public void SelectPage(string name, bool selectNode)
        {
            /*
             * First time this is called (from the MainForm) the
             * name will be null. Select the first node in the tree
             * by default (if there are any).
             */

            if (String.IsNullOrEmpty(name))
            {
                if (optionsTreeView.Nodes.Count > 0) // Have nodes to select
                {
                    TreeNode groupNode = optionsTreeView.Nodes[0];

                    if (groupNode is PageNode)
                    {
                        name = groupNode.Name;
                    }
                    else
                    {
                        PageNode pageNode = groupNode.Nodes[0] as PageNode;
                        name = pageNode.Name;
                    }
                }
            }

            if (!String.IsNullOrEmpty(name) &&
                _pages.ContainsKey(name))       // Page is valid
            {
                if (name == SelectedPageName)   // Already selected
                    return;

                /*
                 * Select the new page into the form.
                 */

                optionsPanel.Controls.Clear();
                optionsPanel.Controls.Add(_pages[name]);

                if (selectNode)
                {
                    TreeNode[] nodes =
                        optionsTreeView.Nodes.Find(name, true);

                    if (nodes.Length > 0)
                        optionsTreeView.SelectedNode = nodes[0];
                }

                /*
                 * Keep track of the selected page for passing
                 * back to the caller.
                 */

                SelectedPageName = name;
            }
        }

        private void OptionsTreeView_AfterSelect(
            object sender, TreeViewEventArgs e)
        {
            if (e.Node is PageNode)
            {
                SelectPage(e.Node.Name, false);
            }
            else
            {
                if (e.Node.Nodes.Count > 0)
                    SelectPage(e.Node.Nodes[0].Name, false);
            }
        }

        #endregion

        #region Save

        private void OkButton_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                /*
                 * Allow each page to validate its contents.
                 * Closing the options form is cancelled as soon
                 * as a validate method returns false.
                 */

                foreach (OptionsPage page in _pages.Values)
                {
                    if (!page.Validate())
                    {
                        /*
                         * Select the offending page and 
                         * cancel the close request.
                         */

                        SelectPage(page.Name);
                        return;
                    }
                }

                /*
                 * Instruct each page to save its contents.
                 */

                foreach (OptionsPage page in _pages.Values)
                    page.Save();
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            Close();
        }

        #endregion
    }
}
