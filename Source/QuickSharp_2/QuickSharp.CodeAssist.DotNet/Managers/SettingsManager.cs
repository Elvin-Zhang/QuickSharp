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
using QuickSharp.Core;

namespace QuickSharp.CodeAssist.DotNet
{
    public class SettingsManager
    {
        #region Singleton

        private static SettingsManager _singleton;

        public static SettingsManager GetInstance()
        {
            if (_singleton == null)
                _singleton = new SettingsManager();

            return _singleton;
        }

        #endregion

        private ApplicationManager _applicationManager;
        private IPersistenceManager _persistenceManager;

        public bool ColorizeTypes { get; set; }
        public bool ColorizeVariables { get; set; }
        public bool ColorizeOnActivate { get; set; }
        public bool ColorizeOnLookup { get; set; }

        private SettingsManager()
        {
            _applicationManager = 
                ApplicationManager.GetInstance();

            _persistenceManager = 
                _applicationManager.GetPersistenceManager(
                    Constants.PLUGIN_NAME);

            Update();
        }

        public void Update()
        {
            ColorizeTypes = _persistenceManager.ReadBoolean(
                Constants.KEY_COLORIZE_TYPES, false);
            ColorizeVariables = _persistenceManager.ReadBoolean(
                Constants.KEY_COLORIZE_VARIABLES, false);
            ColorizeOnActivate = _persistenceManager.ReadBoolean(
                Constants.KEY_COLORIZE_ON_ACTIVATE, false);
            ColorizeOnLookup = _persistenceManager.ReadBoolean(
                Constants.KEY_COLORIZE_ON_LOOKUP, false);
        }

        public void Save()
        {
            _persistenceManager.WriteBoolean(
                Constants.KEY_COLORIZE_TYPES, ColorizeTypes);
            _persistenceManager.WriteBoolean(
                Constants.KEY_COLORIZE_VARIABLES, ColorizeVariables);
            _persistenceManager.WriteBoolean(
                Constants.KEY_COLORIZE_ON_ACTIVATE, ColorizeOnActivate);
            _persistenceManager.WriteBoolean(
                Constants.KEY_COLORIZE_ON_LOOKUP, ColorizeOnLookup);
        }
    }
}
