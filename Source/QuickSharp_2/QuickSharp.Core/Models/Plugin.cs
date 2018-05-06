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

namespace QuickSharp.Core
{
    /// <summary>
    /// Contains metadata for describing plugins. This class is used to represent
    /// plugins inside the plugin management system, it is not the base class for
    /// plugins which are simply required to implement IQuickSharpPlugin. The Plugin class
    /// is used to represent plugin dependencies and is the base class for PluginModule
    /// which represents actual plugins.
    /// </summary>
    public class Plugin
    {
        private string _id;
        private string _name;
        private int _version;

        /// <summary>
        /// Create a Plugin metatdata object.
        /// </summary>
        /// <param name="pluginId">The plugin ID.</param>
        /// <param name="pluginName">The plugin name.</param>
        /// <param name="pluginVersion">The plugin version.</param>
        public Plugin(string pluginId, string pluginName, int pluginVersion)
        {
            _id = pluginId;
            _name = pluginName;
            _version = pluginVersion;
        }

        /// <summary>
        /// The plugin ID. This must be a unique identifier and is usually a GUID.
        /// </summary>
        public string ID
        {
            get { return _id; }
        }

        /// <summary>
        /// The plugin name. This is a short title and is used to provide a human readable
        /// identity for the plugin. The name is for information purposes only.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The version of the plugin. The version identifies a particular revision of a plugin
        /// and is only required if the functional interace of a plugin changes. The version is
        /// mainly intended to allow downstream plugins to indicate a minimum version of the plugin
        /// they require.
        /// </summary>
        public int Version
        {
            get { return _version; }
        }
    }
}
