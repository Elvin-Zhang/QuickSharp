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
using QuickSharp.Core;

namespace QuickSharp.Core
{
    /*
     * 
     * 
     */

    /// <summary>
    /// Deploys user data items to the user's application data folder on first-time startup.
    /// The items are stored within a subfolder of the common user data area within the
    /// application installation directory. The common area is named 'User' and contains
    /// subfolders containing files and folders that are to be copied to the user's local
    /// data folder the first time they run the application. The name of the subfolder is
    /// passed as a key to the DeployContent method.
    /// </summary>
    public class UserContent
    {
        /// <summary>
        /// Copy the files in the named content folder to the user data folder.
        /// </summary>
        /// <param name="contentName">The name of the content folder to deploy.
        /// </param>
        public static void DeployContent(string contentName)
        {
            ApplicationManager applicationManager =
                ApplicationManager.GetInstance();

            /*
             * Check content hasn't been deployed already.
             */

            IPersistenceManager persistenceManager =
                applicationManager.GetPersistenceManager(contentName);

            bool contentDeployed = persistenceManager.ReadBoolean(
                Constants.KEY_USER_CONTENT_DEPLOYED, false);

            if (contentDeployed) return;

            /*
             * Check we have content to deploy.
             */

            string contentRoot = Path.Combine(
                Constants.USER_CONTENT_FOLDER, contentName);

            contentRoot = Path.Combine(
                applicationManager.QuickSharpHome, contentRoot);

            if (!Directory.Exists(contentRoot)) return;

            int contentRootLength = contentRoot.Length;

            /*
             * Copy the content to the user home directory.
             * Just ignore any errors - there's nothing the end user
             * can do and any message is likely to be confusing.
             */

            string targetRoot = applicationManager.QuickSharpUserHome;

            string[] folders = Directory.GetDirectories(
                contentRoot, "*.*", SearchOption.AllDirectories);

            foreach (string folder in folders)
            {
                try
                {
                    string targetPath = Path.Combine(targetRoot,
                        folder.Substring(contentRootLength + 1));

                    Directory.CreateDirectory(targetPath);
                }
                catch
                {
                }
            }

            string[] files = Directory.GetFiles(
                contentRoot, "*.*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                try
                {
                    string targetPath = Path.Combine(targetRoot,
                        file.Substring(contentRootLength + 1));

                    File.Copy(file, targetPath);
                }
                catch
                {
                }
            }

            /*
             * Register that we've done it.
             */

            persistenceManager.WriteBoolean(
                Constants.KEY_USER_CONTENT_DEPLOYED, true);
        }
    }
}
