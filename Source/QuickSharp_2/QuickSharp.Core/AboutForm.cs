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
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace QuickSharp.Core
{
    /// <summary>
    /// Provides the default About box form.
    /// </summary>
    public partial class AboutForm : Form
    {
        private Color _aboutBoxTextColor;
        private PluginManager _pluginManager;
        
        /// <summary>
        /// Create an instance of the default about box form.
        /// </summary>
        public AboutForm()
        {
            ApplicationManager applicationManager = 
                ApplicationManager.GetInstance();

            ClientProfile profile = applicationManager.ClientProfile;
            
            _aboutBoxTextColor = profile.AboutBoxTextColor;

            InitializeComponent();

            BackgroundImage = profile.AboutBoxImage;

            Text = String.Format("{0} {1}",
                Resources.AboutTitle,
                profile.ClientTitle);

            _clientCopyrightLabel.Text = profile.CopyrightText;

            _coreCopyrightLabel.Text = String.Format("{0} {1}\r\n{2}\r\n{3}",
                Resources.AboutCopyright1,
                GetVersionNumber(),
                Resources.AboutCopyright2,
                Resources.AboutCopyright3);

            _dockpanelCopyrightLabel.Text = String.Format("{0}\r\n{1}",
                Resources.AboutCopyrightDPS1,
                Resources.AboutCopyrightDPS2);

            _pluginManager = PluginManager.GetInstance();

            foreach (PluginModule plugin in _pluginManager.RegisteredPlugins)
                _pluginListBox.Items.Add(plugin);

            // Allow client applications to modify the form.
            AboutFormProxy.GetInstance().UpdateFormControls(Controls);
        }

        private void PluginListBox_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            PluginModule plugin =
                _pluginListBox.SelectedItem as PluginModule;
            
            if (plugin != null)
                _pluginDescriptionTextBox.Text = GetPluginDetails(plugin);
            else
                _pluginDescriptionTextBox.Text = "";
        }

        #region Plugin Details

        private string GetPluginDetails(PluginModule plugin)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(plugin.Name);
            sb.Append("\r\n[");
            sb.Append(plugin.ID);
            sb.Append(":");
            sb.Append(plugin.Version);
            sb.Append("]\r\n\r\n");
            sb.Append(plugin.Description);
            sb.Append("\r\n\r\n");
            sb.Append(Resources.AboutRequires);
            sb.Append(":\r\n");

            if (plugin.Dependencies.Count == 0)
            {
                sb.Append(Resources.AboutDependenciesNone);
                sb.Append("\r\n");
            }
            else
            {
                foreach (Plugin dep in plugin.Dependencies)
                {
                    sb.Append(dep.Name);
                    sb.Append("  [");
                    sb.Append(dep.ID);
                    sb.Append(":");
                    sb.Append(dep.Version);
                    sb.Append("]\r\n");
                }
            }

            sb.Append("\r\n");
            sb.Append(Resources.AboutRequiredBy);
            sb.Append(":\r\n");

            List<String> dependents = GetDependents(plugin);
            if (dependents.Count == 0)
            {
                sb.Append(Resources.AboutDependenciesNone);
                sb.Append("\r\n");
            }
            else
            {
                foreach (String dep in dependents)
                    sb.Append(dep);
            }

            return sb.ToString();
        }

        private List<String> GetDependents(PluginModule plugin)
        {
            List<String> idList = new List<String>();
            List<String> depList = new List<String>();

            foreach (PluginModule pm in _pluginManager.RegisteredPlugins)
            {
                foreach (Plugin p in pm.Dependencies)
                    if (plugin.ID == p.ID && !idList.Contains(pm.ID))
                    {
                        idList.Add(pm.ID);
                        depList.Add(String.Format("{0}\r\n", pm.Name));
                        break;
                    }
            }

            return depList;
        }

        private string GetVersionNumber()
        {
            Version v = GetType().Assembly.GetName().Version;

            return String.Format("{0}.{1}.{2}.{3}",
                v.Major, v.Minor, v.Build,
                v.Revision.ToString().PadLeft(5, '0'));
        }

        #endregion
    }
}
