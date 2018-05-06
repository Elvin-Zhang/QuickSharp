using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using QuickSharp.Core;

namespace SampleApp
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException +=
                new ThreadExceptionEventHandler(ThreadException);

            /*
             * Every application needs a ClientProfile. This defines
             * the basic customisation parameters of the application
             * and needs to be defined before the core platform is
             * loaded.
             */

            ClientProfile profile = new ClientProfile();

            /*
             * Define the internal name of the application. Used for
             * the user data folder name.
             */

            profile.ClientName = "SampleApp";

            /*
             * Define the text that appears the main form and about
             * box title bars.
             */

            profile.ClientTitle = "Sample Application";

            /*
             * Define the type of settings storage used. At present only
             * the registry is supported and will use the specified 
             * key to save settings under "HKEY_CURRENT_USER\Software".
             */

            profile.PersistenceKey = "QuickSharpSample";

            /*
             * Make the command line arguments accessible to the rest
             * of the application.
             */

            profile.CommandLineArgs = args;
            
            /*
             * Define the application's main form icon.
             */

            profile.ClientIcon = Resources.MainFormIcon;

            /*
             * Define the copyright text for the application. It will
             * appear in the application's About box.
             */

            profile.CopyrightText = String.Format("{0} {1}\r\n{2}\r\n{3}",
                "Sample Application Version",
                Application.ProductVersion,
                "Copyright © 2010 by Someone",
                "http://mywebsite.com");

            /*
             * Define the background image and text color for the
             * application About box.
             */

            profile.AboutBoxImage = Resources.AboutBackgroundImage;
            profile.AboutBoxTextColor = Color.Black;

            /*
             * Create the ApplicationManager and assign the client
             * profile. The application manager provides a centralized
             * location for various application properties and functions.
             */

            ApplicationManager applicationManager =
                ApplicationManager.GetInstance();

            applicationManager.ClientProfile = profile;

            /*
             * Run the application.
             */

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
