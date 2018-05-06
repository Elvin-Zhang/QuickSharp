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
using System.Reflection;

namespace QuickSharp.Core
{
    /// <summary>
    /// Manages the loading of plugins.
    /// </summary>
    public class PluginManager
    {
        #region Singleton

        private static PluginManager _singleton;

        /// <summary>
        /// Retrieves a reference to the PluginManager singleton.
        /// </summary>
        /// <returns>A reference to the PluginManager.</returns>
        public static PluginManager GetInstance()
        {
            if (_singleton == null)
                _singleton = new PluginManager();

            return _singleton;
        }

        #endregion

        private ApplicationManager _applicationManager;
        private List<PluginModule> _loadedPlugins;
        private List<String> _loadedPluginIDs;
        private List<PluginModule> _registeredPlugins;

        #region Public events

        /// <summary>
        /// Raises the PluginPreActivate event. PreActivation handlers can be
        /// registered to perform tasks before any plugins are activated.
        /// As plugins aren't active until the activation phase PreActivation
        /// tasks can only be registered by non-plugin application code, typically
        /// the application driver.
        /// </summary>
        public event MessageHandler PluginPreActivate;

        /// <summary>
        /// Raises the PluginPostActivate event. PostActivation handlers can be
        /// registered to perform post activation tasks. Plugins can register
        /// handlers during their activation phase. Typically this is used where
        /// a plugin needs to wait until all the plugins have loaded
        /// before performing some task.
        /// </summary>
        public event MessageHandler PluginPostActivate;

        #endregion

        private PluginManager()
        {
            _applicationManager = ApplicationManager.GetInstance();
            _loadedPlugins = new List<PluginModule>();
            _loadedPluginIDs = new List<String>();
            _registeredPlugins = new List<PluginModule>();
        }

        #region Plugin Loading

        /**********************************************************************
         * PLUGIN MANAGEMENT
         * 
         * Plugin loading is a three stage process:
         * 
         * 1. LoadPlugins finds the plugin DLLs. The must be located in the
         *    application directory and must implement IQuickSharpPlugin.
         *    The details of each plugin are added to the list _loadedPlugins.
         *    
         * 2. RegisterPlugins checks the dependencies of each plugin making
         *    sure each one exists and adding each to a second list
         *    _registeredPlugins which sorts them into the correct loading
         *    order. Any unsatisfied dependency (either the dependency doesn't
         *    exist or is an insufficient version number) will cause a fatal
         *    error and the application will aborted.
         *    
         * 3. ActivatePlugins works through _registedPlugins and calls the
         *    Activate method on each DLL passing a reference to the application
         *    main form.
         *    
         **********************************************************************/

        /// <summary>
        /// Loads the plugins available to the application.
        /// These must be located in the application directory and must
        /// implement IQuickSharpPlugin.
        /// </summary>
        public void LoadPlugins()
        {
            string[] dlls = Directory.GetFiles(
                _applicationManager.QuickSharpHome, "*.dll");

            foreach (string dll in dlls)
            {
                List<IQuickSharpPlugin> plugins =
                    GetPluginsFromAssembly(dll);

                foreach (IQuickSharpPlugin plugin in plugins)
                {
                    string pluginID = plugin.GetID();

                    if (String.IsNullOrEmpty(pluginID))
                        throw new Exception(String.Format(
                            "Plugin \"{0}\" has an invalid ID.", plugin.GetName()));

                    pluginID = pluginID.ToUpper();
                    
                    if (_loadedPluginIDs.Contains(pluginID))
                        throw new Exception(String.Format(
                            "Plugin \"{0}\" has a duplicate ID.", plugin.GetName()));

                    PluginModule p = new PluginModule(plugin,
                        plugin.GetID(),
                        plugin.GetName(),
                        plugin.GetVersion(),
                        plugin.GetDescription());

                    foreach (Plugin dependency in plugin.GetDependencies())
                        p.AddDependency(dependency);

                    _loadedPlugins.Add(p);
                    _loadedPluginIDs.Add(p.ID);
                }
            }
        }

        /// <summary>
        /// Registers the loaded plugins. RegisterPlugins checks the dependencies
        /// of each plugin making sure each one exists and adds each to list of
        /// registered plugins sorted into the correct loading order. Any unsatisfied
        /// dependency (either the dependency doesn't exist or is an insufficient
        /// version number) will cause a fatal error and the application will aborted.
        /// </summary>
        public void RegisterPlugins()
        {
            foreach (PluginModule p in _loadedPlugins)
                LoadDependencies(p);
        }

        /// <summary>
        /// Activates the registered plugins. ActivatePlugins works through the list
        /// of registed plugins and calls the Activate method on each one passing a
        /// reference to the application main form. 
        /// </summary>
        /// <param name="mainForm">The application main form.</param>
        public void ActivatePlugins(MainForm mainForm)
        {
            if (PluginPreActivate != null)
                PluginPreActivate();

            foreach (PluginModule p in _registeredPlugins)
                p.Item.Activate(mainForm);

            if (PluginPostActivate != null)
                PluginPostActivate();
        }

        /// <summary>
        /// Stores a list of the registered plugins.
        /// </summary>
        public List<PluginModule> RegisteredPlugins
        {
            get { return _registeredPlugins; }
        }
        
        /// <summary>
        /// Determine if a plugin has been registered.
        /// </summary>
        /// <param name="pluginID">The plugin ID.</param>
        /// <returns>True if the plugin has been registered.</returns>
        public bool HavePlugin(string pluginID)
        {
            PluginModule pm = FindPluginByID(_registeredPlugins, pluginID);
            return (pm != null);
        }
        
        #endregion

        #region Helpers

        private List<IQuickSharpPlugin>
            GetPluginsFromAssembly(string pluginPath)
        {
            List<IQuickSharpPlugin> plugins =
                new List<IQuickSharpPlugin>();

            Assembly asm = null;

            try
            {
                asm = Assembly.LoadFile(pluginPath);
            }
            catch
            {
                // Ignore if not loadable.
            }

            if (asm != null)
            {
                try
                {
                    Type[] types = asm.GetTypes();

                    foreach (Type t in types)
                    {
                        /*
                         * QuickSharp.Core is not a plugin but will
                         * appear to be one as it contains the plugin
                         * interface type.
                         */

                        if (t == typeof(IQuickSharpPlugin))
                            continue;

                        /*
                         * Anything that implements the plugin interface
                         * is treated as a plugin.
                         */

                        if (typeof(IQuickSharpPlugin).IsAssignableFrom(t))
                        {
                            IQuickSharpPlugin plugin = 
                                Activator.CreateInstance(t) as IQuickSharpPlugin;
                        
                            if (plugin != null)
                                plugins.Add(plugin);
                        }
                    }
                }
                catch
                {
                    throw new Exception(String.Format(
                        Resources.PluginDependencyError0,
                        Path.GetFileName(pluginPath)));
                }
            }

            return plugins;
        }

        private void LoadDependencies(PluginModule p1)
        {
            foreach (Plugin dependency in p1.Dependencies)
            {
                PluginModule p2 = FindPluginByID(
                    _loadedPlugins, dependency.ID);

                if (p2 == null)
                {
                    throw new Exception(String.Format(
                        Resources.PluginDependencyError1,
                        p1.Name, dependency.Name)
                    );
                }

                if (p2.Version < dependency.Version)
                {
                    throw new Exception(String.Format(
                        Resources.PluginDependencyError2,
                        p1.Name,
                        dependency.Name,
                        dependency.Version,
                        p2.Version)
                    );
                }

                LoadDependencies(p2);
            }

            if (FindPluginByID(_registeredPlugins, p1.ID) == null)
                _registeredPlugins.Add(p1);
        }

        private PluginModule FindPluginByID(
            List<PluginModule> pluginList, string pluginID)
        {
            pluginID = pluginID.ToUpper();

            foreach (PluginModule plugin in pluginList)
                if (plugin.ID.ToUpper() == pluginID)
                    return plugin;

            return null;
        }

        private PluginModule FindPluginByName(
            List<PluginModule> pluginList, string pluginName)
        {
            foreach (PluginModule plugin in pluginList)
                if (plugin.Name == pluginName)
                    return plugin;

            return null;
        }

        #endregion
    }
}
