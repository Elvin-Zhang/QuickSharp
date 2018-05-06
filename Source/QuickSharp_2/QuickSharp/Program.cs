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
using System.Threading;
using QuickSharp.Core;

namespace QuickSharp
{
    public static class Program
    {
        [STAThread]
        public static void Main(string [] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException +=
                new ThreadExceptionEventHandler(ThreadException);

            ClientProfile profile = new ClientProfile();
            profile.SetUICultureFromCommandLine(args);
            Thread.CurrentThread.CurrentUICulture = profile.CurrentUICulture;

            profile.ClientName = Resources.ClientName;
            profile.ClientTitle = Resources.ClientTitle;
            profile.PersistenceKey = "QuickSharp\\QuickSharp";
            profile.ClientIcon = Resources.MainFormIcon;
            profile.CopyrightText = String.Format("{0} {1}\r\n{2}\r\n{3}",
                Resources.ClientCopyright1,
                Application.ProductVersion,
                Resources.ClientCopyright2,
                Resources.ClientCopyright3);

            profile.AboutBoxImage = Resources.AboutBackgroundImage;
            profile.AboutBoxTextColor = Color.Black;
            profile.PersistenceProvider = Constants.REGISTRY_PERSISTENCE_PROVIDER;
            profile.CommandLineArgs = args;

            // Update check
            profile.UpdateCheckURL = Resources.UpdateCheckURL;
            profile.UpdateHomeURL = Resources.UpdateHomeURL;
            profile.UpdateLinkText = Resources.UpdateLinkText;

            // Set the ClientFlags
            profile.AddFlag(ClientFlags.ExplorerHideByDefault);
            profile.AddFlag(ClientFlags.ExplorerStartFromCurrentDirectory);
            profile.AddFlag(ClientFlags.EditorChangeDirectoryOnSave);
            profile.AddFlag(ClientFlags.TextEditorClaimUnknownDocument);
            profile.AddFlag(ClientFlags.CodeAssistObjectBrowserIncludeWorkspace);
            profile.AddFlag(ClientFlags.CodeAssistObjectBrowserUseMainToolbar);
            profile.AddFlag(ClientFlags.CodeAssistDotNetAutoPopulateDatabase);
            profile.AddFlag(ClientFlags.WorkspaceEnableTitleBarUpdate);
            profile.AddFlag(ClientFlags.WorkspaceEnableShowSource);
            profile.AddFlag(ClientFlags.SqlEditorEnableXsdExport);
            profile.AddFlag(ClientFlags.SqlManagerEnableDbmlExtract);

            ApplicationManager applicationManager =
                ApplicationManager.GetInstance();

            applicationManager.ClientProfile = profile;

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

        private static void ThreadException(
            object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ErrorForm ef = new ErrorForm(e.Exception, true);
            if (ef.ShowDialog() == DialogResult.OK)
                Application.Exit();
        }
    }
}
