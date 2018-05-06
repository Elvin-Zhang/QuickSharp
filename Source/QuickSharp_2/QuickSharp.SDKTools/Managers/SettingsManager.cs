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
using Microsoft.Win32;
using QuickSharp.Core;

namespace QuickSharp.SDKTools
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

        public string IldasmPath { get; set; }
        public string DexplorePath { get; set; }
        public string DexploreArgs { get; set; }
        public string ClrDbgPath { get; set; }

        private SettingsManager()
        {
            _applicationManager = ApplicationManager.GetInstance();

            _persistenceManager = 
                _applicationManager.GetPersistenceManager(Constants.PLUGIN_NAME);

            Update();
        }

        public void Save()
        {
            _persistenceManager.WriteString(Constants.KEY_ILDASM_PATH, IldasmPath);
            _persistenceManager.WriteString(Constants.KEY_DEXPLORE_PATH, DexplorePath);
            _persistenceManager.WriteString(Constants.KEY_DEXPLORE_ARGS, DexploreArgs);
            _persistenceManager.WriteString(Constants.KEY_CLRDBG_PATH, ClrDbgPath);
        }

        public void Update()
        {
            IldasmPath = _persistenceManager.ReadString(
                Constants.KEY_ILDASM_PATH,
                @"C:\Program Files\Microsoft.NET\SDK\v2.0\bin\ildasm.exe");

            DexplorePath = _persistenceManager.ReadString(
                Constants.KEY_DEXPLORE_PATH,
                @"C:\Program Files\Common Files\Microsoft Shared\Help 8\dexplore.exe");

            DexploreArgs = _persistenceManager.ReadString(
                Constants.KEY_DEXPLORE_ARGS,
                @"/helpcol ms-help://MS.NETFramework.v20.en /usehelpsettings NETFrameworkSDK.20 ");

            ClrDbgPath = _persistenceManager.ReadString(
                Constants.KEY_CLRDBG_PATH,
                @"C:\Program Files\Microsoft.NET\SDK\v2.0\GuiDebug\DbgCLR.exe");
        }
    }
}
