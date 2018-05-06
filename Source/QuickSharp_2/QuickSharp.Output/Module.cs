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
using QuickSharp.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.Output
{
    /// <summary>
    /// The output window plugin module.
    /// </summary>
    public class Module: IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        /// <summary>
        /// Get the ID of the plugin.
        /// </summary>
        /// <returns>The plugin ID.</returns>
        public string GetID()
        {
            return "542A7DA1-C7ED-488D-845B-AB6EBDBBEDA0";
        }

        /// <summary>
        /// Get the name of the plugin.
        /// </summary>
        /// <returns>The plugin name.</returns>
        public string GetName()
        {
            return "QuickSharp Output";
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
                "Provides the Output window " +
                "and services for running external programs.";
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
        private IPersistenceManager _persistenceManager;
        private MainForm _mainForm;
        private OutputForm _outputForm;
        private ToolStripMenuItem _outputMenuItem;

        private void ActivatePlugin()
        {
            _applicationManager = 
                ApplicationManager.GetInstance();
            
            _persistenceManager = _applicationManager.
                GetPersistenceManager(Constants.PLUGIN_NAME);

            /*
             * Create the UI elements.
             */

            _outputMenuItem = MenuTools.CreateMenuItem(
                Constants.UI_VIEW_MENU_OUTPUT,
                Resources.MainViewMenuOutput,
                null, 
                Keys.Control | Keys.Alt | Keys.O, null,
                UI_VIEW_MENU_OUTPUT_Click);

            ToolStripMenuItem viewMenu = _mainForm.GetMenuItemByName(
                QuickSharp.Core.Constants.UI_VIEW_MENU);
            
            if (viewMenu != null)
            {
                viewMenu.DropDownItems.Add(_outputMenuItem);
                viewMenu.DropDownOpening += 
                    new EventHandler(UI_VIEW_MENU_DropDownOpening);
            }

            /*
             * Create and register the form.
             */

            _outputForm = new OutputForm(Constants.DOCKED_FORM_KEY);

            _applicationManager.RegisterDockedForm(_outputForm);
        }

        #region Event Handlers

        private void UI_VIEW_MENU_OUTPUT_Click(object sender, EventArgs e)
        {
            if (_outputForm.Visible)
                _outputForm.Hide();
            else
                _outputForm.Show();
        }

        private void UI_VIEW_MENU_DropDownOpening(object sender, EventArgs e)
        {
            _outputMenuItem.Checked = _outputForm.Visible;
        }

        #endregion
    }
}
