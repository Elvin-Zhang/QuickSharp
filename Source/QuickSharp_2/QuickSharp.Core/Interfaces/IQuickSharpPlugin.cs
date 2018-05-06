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
    /// Defines the interface to QuickSharp plugin modules.
    /// </summary>
    public interface IQuickSharpPlugin
    {
        /// <summary>
        /// Get the ID of the plugin.
        /// </summary>
        /// <returns>The plugin ID.</returns>
        string GetID();

        /// <summary>
        /// Get the name of the plugin.
        /// </summary>
        /// <returns>The plugin name.</returns>
        string GetName();

        /// <summary>
        /// Get the version of the plugin.
        /// </summary>
        /// <returns>The plugin version.</returns>
        int GetVersion();

        /// <summary>
        /// Get a description of the plugin.
        /// </summary>
        /// <returns>The plugin description.</returns>
        string GetDescription();

        /// <summary>
        /// Get the plugin's dependencies. This provides a list of the
        /// plugins required by the current plugin.
        /// </summary>
        /// <returns>The plugin dependencies,</returns>
        List<Plugin> GetDependencies();

        /// <summary>
        /// The plugin entry point. This is called by the PluginManager to
        /// activate the plugin.
        /// </summary>
        /// <param name="mainForm">The application main form.</param>
        void Activate(MainForm mainForm);
    }
}
