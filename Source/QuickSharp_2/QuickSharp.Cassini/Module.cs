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

namespace QuickSharp.Cassini
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "A10B404B-DF5B-4CDA-A940-0455EAA793A0";
        }

        public string GetName()
        {
            return "QuickSharp Cassini Web Server Support";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return
                "Provides support for hosting web sites and ASP.NET " +
                "applications using the Cassini web server.";
        }

        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            return deps;
        }

        public void Activate(MainForm mainForm)
        {
            _mainForm = mainForm;
            ActivatePlugin();
        }

        #endregion

        private MainForm _mainForm;
        private SettingsManager _settingsManager;
        private ToolStripMenuItem _toolMenuWebServer;

        private void ActivatePlugin()
        {
            /*
             * Web server tools menu option.
             */

            ToolStripMenuItem toolMenu = _mainForm.GetMenuItemByName(
                QuickSharp.Core.Constants.UI_TOOLS_MENU);

            int index = toolMenu.DropDownItems.IndexOfKey(
                QuickSharp.Core.Constants.UI_TOOLS_MENU_OPTIONS);

            /*
             * If menu item not found insert at the top.
             */

            if (index == -1) index = 0;

            _toolMenuWebServer = MenuTools.CreateMenuItem(
                Constants.UI_TOOLS_MENU_WEB_SERVER,
                Resources.MainToolsMenuWebServer,
                null, Keys.None, null,
                UI_TOOLS_MENU_WEB_SERVER_Click,
                true);

            _toolMenuWebServer.Enabled = false;

            toolMenu.DropDownItems.Insert(index, _toolMenuWebServer);

            toolMenu.DropDownOpening += 
                new EventHandler(ToolMenu_DropDownOpening);

            /*
             * Server options.
             */

            ApplicationManager applicationManager =
                ApplicationManager.GetInstance();
                
            applicationManager.RegisterOptionsPageFactory(
                delegate { return new WebServerOptionsPage(); });

            _settingsManager = SettingsManager.GetInstance();

            /*
             * One time only: initialize the web server path to the
             * QuickSharp home directory.
             */

            if (_settingsManager.ServerPath == String.Empty)
            {
                /*
                 * Determine the server path. 
                 */

                _settingsManager.ServerPath = Path.Combine(
                    applicationManager.QuickSharpHome, 
                    Constants.WEBSERVER_EXE);

                _settingsManager.Save();
            }
        }

        private void ToolMenu_DropDownOpening(object sender, EventArgs e)
        {
            _toolMenuWebServer.Enabled =
                File.Exists(_settingsManager.ServerPath);
        }

        private void UI_TOOLS_MENU_WEB_SERVER_Click(
            object sender, EventArgs e)
        {
            /*
             * Server document root is current directory. Store in
             * ApplicationStorage for plugins that need to know the web root.
             */

            string startupDirectory = Directory.GetCurrentDirectory();

            ApplicationStorage appStore = ApplicationStorage.GetInstance();

            appStore[Constants.APP_STORE_KEY_WEB_APP_ROOT] = startupDirectory;

            _mainForm.SetStatusBarMessage(String.Format("{0}: \"{1}\"",
                Resources.StatusBarStartupMessage, startupDirectory), 5);

            if (startupDirectory.EndsWith("\\"))
                startupDirectory += "\\";

            string args = String.Format("\"{0}\" {1} \"{2}\"",
                startupDirectory,
                _settingsManager.ServerPort,
                _settingsManager.ServerRoot);

            FileTools.LaunchApplication(
                false, _settingsManager.ServerPath, args);
        }
    }
}
