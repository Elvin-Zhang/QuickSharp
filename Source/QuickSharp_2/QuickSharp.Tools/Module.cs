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

namespace QuickSharp.Tools
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "11A72A6E-F0F0-428A-92BE-6B715807F333";
        }

        public string GetName()
        {
            return "QuickSharp Tools";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return
                "Provides Create GUID and Regular Expression Helper tools.";
        }

        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            deps.Add(new Plugin(QuickSharpPlugins.Editor, "QuickSharp.Editor", 1));
            return deps;
        }

        public void Activate(MainForm mainForm)
        {
            _mainForm = mainForm;
            ActivatePlugin();
        }

        #endregion

        private MainForm _mainForm;
        private ToolStripMenuItem _toolMenuRegexHelper;
        private ToolStripMenuItem _toolMenuLibraryHelper;
        private ToolStripMenuItem _toolMenuCreateGUID;
        private List<String> _regexHistory;

        private void ActivatePlugin()
        {
            ToolStripMenuItem toolMenu = _mainForm.GetMenuItemByName(
                QuickSharp.Core.Constants.UI_TOOLS_MENU);

            int index = toolMenu.DropDownItems.IndexOfKey(
                QuickSharp.Core.Constants.UI_TOOLS_MENU_OPTIONS);

            /*
             * If we can't find the options item insert the
             * new items at the top of the menu.
             */

            if (index == -1) index = 0;

            _toolMenuRegexHelper = MenuTools.CreateMenuItem(
                Constants.UI_TOOLS_MENU_REGEX_HELPER,
                Resources.MainToolsMenuRegexHelper,
                null, Keys.None, null,
                UI_TOOLS_MENU_REGEX_HELPER_Click, true);

            toolMenu.DropDownOpening +=
                new EventHandler(toolMenu_DropDownOpening);

            _regexHistory = new List<String>();

            _toolMenuCreateGUID = MenuTools.CreateMenuItem(
                Constants.UI_TOOLS_MENU_CREATE_GUID,
                Resources.MainToolsMenuCreateGuid,
                null, Keys.None, null,
                UI_TOOLS_MENU_CREATE_GUID_Click);

            _toolMenuLibraryHelper = MenuTools.CreateMenuItem(
                Constants.UI_TOOLS_MENU_LIBRARY_HELPER,
                Resources.MainToolsMenuLibraryHelper,
                null, Keys.None, null,
                UI_TOOLS_MENU_LIBRARY_HELPER_Click, true);

            toolMenu.DropDownItems.Insert(index, _toolMenuLibraryHelper);
            toolMenu.DropDownItems.Insert(index, _toolMenuCreateGUID);
            toolMenu.DropDownItems.Insert(index, _toolMenuRegexHelper);
        }

        #region Regex Helper

        private void UI_TOOLS_MENU_REGEX_HELPER_Click(
            object sender, EventArgs e)
        {
            RegexHelperForm rhf = new RegexHelperForm(_regexHistory);
            rhf.ShowDialog();
        }

        private void toolMenu_DropDownOpening(
            object sender, EventArgs e)
        {
            _toolMenuRegexHelper.Enabled = false;
            if (_mainForm.ActiveDocument == null) return;

            _toolMenuRegexHelper.Enabled =
                (_mainForm.ActiveDocument is ScintillaEditForm);
        }

        #endregion

        #region Create GUID

        private void UI_TOOLS_MENU_CREATE_GUID_Click(object sender, EventArgs e)
        {
            String guid = Guid.NewGuid().ToString();

            Clipboard.SetText(guid.ToUpper());

            MessageBox.Show(
                Resources.CreateGuidDialogMessage,
                Resources.CreateGuidDialgTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        #endregion

        #region Library Helper
        
        private void UI_TOOLS_MENU_LIBRARY_HELPER_Click(
            object sender, EventArgs e)
        {
            LibraryHelperForm lhf = new LibraryHelperForm();
            lhf.ShowDialog();
        }

        #endregion
    }
}
