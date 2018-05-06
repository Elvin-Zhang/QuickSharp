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

namespace QuickSharp.Explorer
{
    /// <summary>
    /// The explorer window plugin module.
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
            return "3C92A3C7-D536-481C-B556-E66211F27C73";
        }

        /// <summary>
        /// Get the name of the plugin.
        /// </summary>
        /// <returns>The plugin name.</returns>
        public string GetName()
        {
            return "QuickSharp Explorer";
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
            return "Provides the Explorer window and file management services.";
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
        private MainForm _mainForm;
        private ExplorerForm _explorerForm;
        private ToolStripMenuItem _explorerMenuItem;

        private void ActivatePlugin()
        {
            _applicationManager = ApplicationManager.GetInstance();

            /*
             * Create the UI elements.
             */

            _explorerMenuItem = MenuTools.CreateMenuItem(
                Constants.UI_VIEW_MENU_EXPLORER,
                Resources.MainViewMenuExplorer,
                null,
                Keys.Control | Keys.Alt | Keys.E, null,
                UI_VIEW_MENU_EXPLORER_Click);

            ToolStripMenuItem viewMenu =
                _mainForm.GetMenuItemByName(
                    QuickSharp.Core.Constants.UI_VIEW_MENU);

            if (viewMenu != null)
            {
                viewMenu.DropDownItems.Add(_explorerMenuItem);
                viewMenu.DropDownOpening +=
                    new EventHandler(UI_VIEW_MENU_DropDownOpening);
            }

            /*
             * Create and register the form.
             */

            _explorerForm = new ExplorerForm(Constants.DOCKED_FORM_KEY);

            _applicationManager.RegisterDockedForm(_explorerForm);

            /*
             * Register the options pages. Pages are saved in the order they
             * are registered. We have two here so we put the UpdateUI call
             * in the second so it is only called once when all the settings
             * have been updated.
             */

            _applicationManager.RegisterOptionsPageFactory(
                delegate { return new ExplorerOptionsPage(); });

            _applicationManager.RegisterOptionsPageFactory(
                delegate { return new FileFiltersOptionsPage(); });
        }

        #region Event Handlers

        private void UI_VIEW_MENU_EXPLORER_Click(object sender, EventArgs e)
        {
            if (_explorerForm.Visible)
                _explorerForm.Hide();
            else
                _explorerForm.Show();
        }

        private void UI_VIEW_MENU_DropDownOpening(object sender, EventArgs e)
        {
            _explorerMenuItem.Checked = _explorerForm.Visible;
        }

        #endregion
    }
}
