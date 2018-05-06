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
using System.IO;
using System.Windows.Forms;
using System.Threading;
using QuickSharp.Core;

namespace QuickDriver
{
    public static class Program
    {
        [STAThread]
        public static void Main(string [] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException +=
                new System.Threading.ThreadExceptionEventHandler(ThreadException);

            string driverName = GetDriverName();

            ClientProfile profile = new ClientProfile();
            profile.SetUICultureFromCommandLine(args);
            Thread.CurrentThread.CurrentUICulture = profile.CurrentUICulture;

            profile.ClientName = driverName;
            profile.ClientTitle = driverName;
            profile.ClientIcon = Resources.MainFormIcon;
            profile.CopyrightText = "QuickDriver Version " +
                Application.ProductVersion +
                "\r\nCopyright © 2008-2011 Steve Walker" +
                "\r\nhttp://quicksharp.sourceforge.net";

            profile.AboutBoxImage = Resources.AboutBackgroundImage;
            profile.AboutBoxTextColor = Color.Black;
            profile.UpdateCheckURL = null;
            profile.PersistenceProvider = Constants.REGISTRY_PERSISTENCE_PROVIDER;
            profile.PersistenceKey = driverName;
            profile.CommandLineArgs = args;

            ApplicationManager applicationManager =
                ApplicationManager.GetInstance();

            applicationManager.ClientProfile = profile;

            ReadClientFlags(profile, applicationManager.QuickSharpHome);

            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                ErrorForm ef = new ErrorForm(ex, false);
                ef.ShowDialog();
            }
        }

        private static string GetDriverName()
        {
            string driverPath = Application.ExecutablePath;
            string driverName = Path.GetFileNameWithoutExtension(driverPath);
            if (!String.IsNullOrEmpty(driverName))
                return driverName;
            else
                return "QuickDriver";
        }

        private static void ReadClientFlags(ClientProfile profile, string home)
        {
            string path = Path.Combine(home, "ClientFlags.txt");
            if (!File.Exists(path)) return;

            StreamReader sr = null;

            try
            {
                sr = new StreamReader(path);

                string line = null;

                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line == String.Empty) continue;
                    if (line.StartsWith("//")) continue;

                    profile.AddFlag(line);
                }
            }
            finally
            {
                if (sr != null) sr.Close();
            }
        }

        private static void ThreadException(
            object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ErrorForm ef = new ErrorForm(e.Exception, true);
            if (ef.ShowDialog() == DialogResult.OK)
                Application.Exit();
        }
    }
}
