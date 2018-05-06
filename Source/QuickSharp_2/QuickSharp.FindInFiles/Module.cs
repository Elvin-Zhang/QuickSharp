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
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using QuickSharp.Core;
using QuickSharp.Editor;
using QuickSharp.Output;

namespace QuickSharp.FindInFiles
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "D230E177-305A-4354-8456-8046B8027ECB";
        }

        public string GetName()
        {
            return "QuickSharp Find In Files";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return
                "Provides search and replace across multiple files.";
        }

        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            deps.Add(new Plugin(QuickSharpPlugins.Editor, "QuickSharp.Editor", 1)); 
            deps.Add(new Plugin(QuickSharpPlugins.Output, "QuickSharp.Output", 1));
            return deps;
        }

        public void Activate(MainForm mainForm)
        {
            _mainForm = mainForm;
            ActivatePlugin();
        }

        #endregion

        private MainForm _mainForm;
        private OutputForm _output;
        private List<String> _findTextHistory;
        private List<String> _replaceTextHistory;
        private List<String> _fileSpecHistory;
        private bool _useRegex;
        private bool _matchCase;
        private bool _searchSubFolders;

        private void ActivatePlugin()
        {
            _findTextHistory = new List<String>();
            _replaceTextHistory = new List<String>();
            _fileSpecHistory = new List<String>();

            /*
             * Access the output window.
             */

            _output = ApplicationManager.GetInstance().GetDockedForm(
                QuickSharp.Output.Constants.DOCKED_FORM_KEY) as OutputForm;

            if (_output == null) return;

            _output.ClearOutputViews();

            /*
             * Create the menu items.
             */

            ToolStripMenuItem editMenu = _mainForm.GetMenuItemByName(
                QuickSharp.Editor.Constants.UI_EDIT_MENU);

            if (editMenu == null) return;

            int index = editMenu.DropDownItems.IndexOfKey(
                QuickSharp.Editor.Constants.UI_EDIT_MENU_REPLACE);

            if (index == -1) return;

            ToolStripMenuItem findMenu = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_FIND_IN_FILES,
                Resources.MainEditMenuFindInFiles,
                null, Keys.Control | Keys.Shift | Keys.F,
                null, delegate { FindInFiles(true); });

            ToolStripMenuItem replaceMenu = MenuTools.CreateMenuItem(
                Constants.UI_EDIT_MENU_REPLACE_IN_FILES,
                Resources.MainEditMenuReplaceInFiles,
                null, Keys.Control | Keys.Shift | Keys.H,
                null, delegate { FindInFiles(false); }, true);

            editMenu.DropDownItems.Insert(index + 1, replaceMenu);
            editMenu.DropDownItems.Insert(index + 1, findMenu);
        }

        private void FindInFiles(bool findOnly)
        {
            FindInFilesForm fiff = new FindInFilesForm(findOnly);

            /*
             * Restore search history to search form.
             */

            fiff.SetFindTextHistory(_findTextHistory);
            if (!findOnly)
                fiff.SetReplaceTextHistory(_replaceTextHistory);
            fiff.SetFileSpecHistory(_fileSpecHistory);
            fiff.UseRegex = _useRegex;
            fiff.MatchCase = _matchCase;
            fiff.SearchSubFolders = _searchSubFolders;

            /*
             * Show the form.
             */

            if (fiff.ShowDialog() == DialogResult.OK)
            {
                ApplicationManager applicationManager =
                    ApplicationManager.GetInstance();

                /*
                 * Optionally ensure the output window is visible.
                 */

                if (applicationManager.ClientProfile.HaveFlag(
                    ClientFlags.FindInFilesShowOutputOnFind))
                    _output.Show();

                /*
                 * Find the affected files.
                 */

                Finder finder = new Finder(_output);

                try
                {
                    _mainForm.Cursor = Cursors.WaitCursor;

                    finder.Find(fiff.FindText, fiff.ReplaceText, fiff.FileSpec,
                        fiff.SearchSubFolders, fiff.UseRegex, fiff.MatchCase, findOnly);
                }
                finally
                {
                    _mainForm.Cursor = Cursors.Default;
                }

                /*
                 * Update the search history from the form.
                 */

                AddHistoryItem(_findTextHistory, fiff.FindText);
                if (!findOnly)
                    AddHistoryItem(_replaceTextHistory, fiff.ReplaceText);
                AddHistoryItem(_fileSpecHistory, fiff.FileSpec);
                _useRegex = fiff.UseRegex;
                _matchCase = fiff.MatchCase;
                _searchSubFolders = fiff.SearchSubFolders;
            }
        }

        #region Helpers

        private void AddHistoryItem(List<String> list, string item)
        {
            int index = list.IndexOf(item);

            if (index != -1)
                list.RemoveAt(index);

            list.Insert(0, item);
        }

        #endregion
    }
}
