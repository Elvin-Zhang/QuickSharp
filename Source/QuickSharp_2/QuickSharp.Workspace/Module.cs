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
using System.IO;
using System.Windows.Forms;
using QuickSharp.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.Workspace
{
    /// <summary>
    /// The workspace plugin module.
    /// </summary>
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        /// <summary>
        /// Get the ID of the plugin.
        /// </summary>
        /// <returns>The plugin ID.</returns>
        public string GetID()
        {
            return "5A27405B-42ED-4A7A-BFC8-35DE25F96CB1";
        }

        /// <summary>
        /// Get the name of the plugin.
        /// </summary>
        /// <returns>The plugin name.</returns>
        public string GetName()
        {
            return "QuickSharp Workspace";
        }

        /// <summary>
        /// Get the version of the plugin.
        /// </summary>
        /// <returns>The plugin version.</returns>
        public int GetVersion()
        {
            return 1;
        }

        /// <summary>
        /// Get a description of the plugin.
        /// </summary>
        /// <returns>The plugin description.</returns>
        public string GetDescription()
        {
            return
                "Provides the Workspace window and file management services.\r\n" +
                "Workspace backup uses #ZipLib by Mike Krueger; for " +
                "more information visit the web site at " +
                "http://www.icsharpcode.net/OpenSource/SharpZipLib/.";
        }

        /// <summary>
        /// Get the plugin's dependencies. This provides a list of the
        /// plugins required by the current plugin.
        /// </summary>
        /// <returns>The plugin dependencies,</returns>
        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            return deps;
        }

        /// <summary>
        /// The plugin entry point. This is called by the PluginManager to
        /// activate the plugin.
        /// </summary>
        /// <param name="mainForm">The application main form.</param>
        public void Activate(MainForm mainForm)
        {
            _mainForm = mainForm;
            ActivatePlugin();
        }

        #endregion

        private ApplicationManager _applicationManager;
        private SettingsManager _settingsManager;
        private MainForm _mainForm;
        private WorkspaceForm _workspaceForm;
        private ToolStripMenuItem _workspaceMenuItem;

        private void ActivatePlugin()
        {
            _applicationManager = ApplicationManager.GetInstance();
            _settingsManager = SettingsManager.GetInstance();

            /*
             * Create the menu item.
             */

            _workspaceMenuItem = MenuTools.CreateMenuItem(
                Constants.UI_VIEW_MENU_WORKSPACE, 
                Resources.MainViewMenuWorkspace,
                null,
                Keys.Control | Keys.Alt | Keys.P, null,
                UI_VIEW_MENU_WORKSPACE_Click);

            ToolStripMenuItem viewMenu =
                _mainForm.GetMenuItemByName(
                    QuickSharp.Core.Constants.UI_VIEW_MENU);

            if (viewMenu != null)
            {
                viewMenu.DropDownItems.Add(_workspaceMenuItem);
                viewMenu.DropDownOpening +=
                    new EventHandler(UI_VIEW_MENU_DropDownOpening);
            }

            /*
             * Restore the previous workspace directory.
             */

            if (Directory.Exists(_settingsManager.CurrentWorkspace))
                Directory.SetCurrentDirectory(_settingsManager.CurrentWorkspace);

            /*
             * Create and register the form.
             */

            _workspaceForm = new WorkspaceForm(Constants.DOCKED_FORM_KEY);

            _applicationManager.RegisterDockedForm(_workspaceForm);

            /*
             * Register the options page.
             */

            _applicationManager.RegisterOptionsPageFactory(
                delegate { return new WorkspaceOptionsPage(); });
        }

        #region Event Handlers

        private void UI_VIEW_MENU_WORKSPACE_Click(object sender, EventArgs e)
        {
            if (_workspaceForm.Visible)
                _workspaceForm.Hide();
            else
                _workspaceForm.Show();
        }

        private void UI_VIEW_MENU_DropDownOpening(object sender, EventArgs e)
        {
            _workspaceMenuItem.Checked = _workspaceForm.Visible;
        }

        #endregion
    }
}
