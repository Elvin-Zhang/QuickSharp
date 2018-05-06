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

using System.Collections.Generic;

namespace QuickSharp.Core
{
    /// <summary>
    /// Contains extended metadata for describing plugins. This class is used to represent
    /// actual plugins inside the plugin management system, it is not the base class for
    /// plugins which are simply required to implement IQuickSharpPlugin.
    /// </summary>
    public class PluginModule : Plugin
    {
        private IQuickSharpPlugin _plugin;
        private string _description;
        private List<Plugin> _dependencies;

        /// <summary>
        /// Create a PluginModule object.
        /// </summary>
        /// <param name="plugin">A reference to an actual plugin module instance.</param>
        /// <param name="pluginId">The ID of the plugin.</param>
        /// <param name="pluginName">The name of the plugin.</param>
        /// <param name="pluginVersion">The versionof the plugin.</param>
        /// <param name="pluginDescription">A description of the plugin.</param>
        public PluginModule(
            IQuickSharpPlugin plugin,
            string pluginId,
            string pluginName,
            int pluginVersion,
            string pluginDescription)
            : base(pluginId, pluginName, pluginVersion)
        {
            _plugin = plugin;
            _description = pluginDescription;
            _dependencies = new List<Plugin>();
        }

        /// <summary>
        /// A reference to the actual plugin module loaded by the PluginManager.
        /// </summary>
        public IQuickSharpPlugin Item
        {
            get { return _plugin; }
        }

        /// <summary>
        /// The plugin description.
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// A list of plugin metadata objects representing the plugins on which
        /// this plugin depends.
        /// </summary>
        public List<Plugin> Dependencies
        {
            get { return _dependencies; }
        }

        /// <summary>
        /// Add a plugin to the list of dependencies as a Plugin metadata object.
        /// </summary>
        /// <param name="plugin"></param>
        public void AddDependency(Plugin plugin)
        {
            _dependencies.Add(plugin);
        }

        /// <summary>
        /// Add a plugin to the list of dependencies.
        /// </summary>
        /// <param name="pluginId">The ID of the required plugin.</param>
        /// <param name="pluginName">The name of the required plugin. This is necessary to
        /// identify the plugin where the dependency fails. Missing plugins are hard to
        /// trace from IDs alone.</param>
        /// <param name="pluginVersion">The required version of the plugin.</param>
        public void AddDependency(
            string pluginId, string pluginName, int pluginVersion)
        {
            _dependencies.Add(
                new Plugin(pluginId, pluginName, pluginVersion));
        }
    }
}
