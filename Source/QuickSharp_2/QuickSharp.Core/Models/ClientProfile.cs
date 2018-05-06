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
using System.Windows.Forms;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace QuickSharp.Core
{
    /// <summary>
    /// Provides properties and methods used to define customizable
    /// behaviour for a QuickSharp.Core application.
    /// </summary>
    public class ClientProfile
    {
        /// <summary>
        /// The name of the client application. Provides a simple name
        /// available for naming application resources. Used to define the
        /// name of the local user data directory.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// The title of the client application. Provides a display name for
        /// the application. Used in the main form and about box title bars.
        /// </summary>
        public string ClientTitle { get; set; }

        /// <summary>
        /// The icon used in the application main form.
        /// </summary>
        public Icon ClientIcon { get; set; }

        /// <summary>
        /// Application-specific copyright information presented in the about box.
        /// </summary>
        public string CopyrightText { get; set; }

        /// <summary>
        /// The background image used for the about box.
        /// </summary>
        public Image AboutBoxImage { get; set; }

        /// <summary>
        /// The color of the text used in the about box.
        /// </summary>
        public Color AboutBoxTextColor { get; set; }

        /// <summary>
        /// The URL for the update check. The update check feature is disabled
        /// if this is null. The URL is the file retrieved when 'Check for updates'
        /// is clicked and should consist of a text file containing the latest
        /// application version number on a single line.
        /// </summary>
        public string UpdateCheckURL { get; set; }

        /// <summary>
        /// The URL for the link presented in the update check form. The link is not
        /// presented if this is null.
        /// </summary>
        public string UpdateHomeURL { get; set; }

        /// <summary>
        /// The text for the link presented in the update check form. The link is not
        /// presented if this is null.
        /// </summary>
        public string UpdateLinkText { get; set; }

        /// <summary>
        /// The maximum number of files listed in the MRU documents list.
        /// Defaults to 9. Set to 0 to disable the MRU list.
        /// </summary>
        public int MRUDocumentListMax { get; set; }

        /// <summary>
        /// The application help file. This is presented on the main form Help
        /// menu and is opened using the Windows shell when the help menu item
        /// is clicked. The file name should be a resource that can be opened directly
        /// such as a text, chm or html file. The help menu item will be hidden if this
        /// value is null or empty.
        /// </summary>
        public string HelpFileName { get; set; }

        /// <summary>
        /// The text of the menu item presented in the Help menu if the help file is
        /// available. The help menu item will be hidden if this is null or empty.
        /// </summary>
        public string HelpFileTitle { get; set; }

        /// <summary>
        /// Optional shortcut keys used to access the help menu item.
        /// </summary>
        public Keys HelpShortcutKeys { get; set; }

        /// <summary>
        /// Optional image to be used for the help menu item.
        /// </summary>
        public Image HelpMenuImage { get; set; }

        /// <summary>
        /// The persistence provider used for the application.
        /// </summary>
        public string PersistenceProvider { get; set; }

        /// <summary>
        /// The text key to be associated with the persistence manager.
        /// </summary>
        public string PersistenceKey { get; set; }

        /// <summary>
        /// The command line arguments passed to the application at startup.
        /// </summary>
        public string[] CommandLineArgs { get; set; }

        /// <summary>
        /// The UI culture presented by the application. Defaults to the 
        /// CurrentUICulture of the current thread.
        /// </summary>
        public CultureInfo CurrentUICulture { get; set; }

        /// <summary>
        /// Set to false to disable the use of local user data folders. All user-specific
        /// content will be located in the application home directory.
        /// </summary>
        public bool DisableUserHome { get; set; }

        private List<String> _flags;

        /// <summary>
        /// Factory handler used to provide the UpdateCheck form. By default this uses the
        /// built-in update check form. Provide a custom factory handler to use an
        /// alternative update check form.
        /// </summary>
        public FormFactoryHandler UpdateCheckFormFactory { get; set; }

        /// <summary>
        /// Factory handler used to provide the About form. By default this uses the
        /// built-in about form. Provide a custom factory handler to use an
        /// alternative about box.
        /// </summary>
        public FormFactoryHandler AboutBoxFactory { get; set; }

        /// <summary>
        /// Provides theme flag values for plugins.
        /// </summary>
        public ThemeFlags ThemeFlags { get; set; }

        /// <summary>
        /// ClientProfile constructor.
        /// </summary>
        public ClientProfile()
        {
            ClientName = "QuickSharpApplication";
            ClientTitle = "QuickSharp Application";
            CopyrightText = String.Empty;
            AboutBoxTextColor = Color.Black;
            HelpShortcutKeys = Keys.None;
            MRUDocumentListMax = 9;
            PersistenceProvider = Constants.REGISTRY_PERSISTENCE_PROVIDER;
            PersistenceKey = "QuickSharpApplicaton";
            CurrentUICulture = Thread.CurrentThread.CurrentUICulture;

            _flags = new List<String>();
        }

        #region Client Flags

        /// <summary>
        /// Add a client flag to the application. Client flags allow arbitrary text
        /// values to be made available for reading anywhere in the application.
        /// Typically they are used to provide customised behaviour in plugins.
        /// Flags should be specified as simple text values not constants; it is
        /// important that no dependencies be created between the application driver
        /// and any plugin reading the flags.
        /// </summary>
        /// <param name="name">The flag value</param>
        public void AddFlag(String name)
        {
            name = name.ToLower();

            if (!_flags.Contains(name))
                _flags.Add(name);
        }

        /// <summary>
        /// Determine if a client flag is present.  
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HaveFlag(String name)
        {
            name = name.ToLower();

            return _flags.Contains(name);
        }

        #endregion

        #region UI Culture

        /// <summary>
        /// Set the current UI culture from a command line switch. The
        /// culture should be specified using this example format: "/uiculture:en-AU".
        /// </summary>
        /// <param name="args"></param>
        public void SetUICultureFromCommandLine(string[] args)
        {
            foreach (string arg in args)
            {
                // Is it a switch?
                if (arg[0] != '/' && arg[0] != '-') continue;

                // Is it the right switch?
                string s = arg.Substring(1).ToLower();
                if (!s.StartsWith("uiculture:")) continue;

                // Do we have a value?
                string[] split = s.Split(':');
                if (split.Length < 2) continue;

                string cultureName = split[1];

                try
                {
                    CurrentUICulture = new CultureInfo(cultureName);
                    break;
                }
                catch // ArgumentException or ArgumentNullException
                {
                    // Just ignore a bad switch.
                }
            }
        }

        #endregion
    }
}
