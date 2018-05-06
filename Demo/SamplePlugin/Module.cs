using System.Collections.Generic;
using QuickSharp.Core;

namespace SamplePlugin
{
    /*
     * Every QuickSharp plugin must implement IQuickSharpPlugin.
     * 
     * See the build events in the project configuration to see
     * how the plugin is copied to the SampleApp to make it available
     * at runtime. We don't want to create a dependency between the
     * application driver and it's plugins so we can't use the 
     * Visual Studio deployment process.
     */

    public partial class Module : IQuickSharpPlugin
    {
        /*
         * Every plugin requires a unique ID.
         */

        public string GetID()
        {
            return "585296EA-A9C1-4ECB-BF7D-A8C3FC4A6229";
        }

        /*
         * The name of the plugin for identification purposes.
         */

        public string GetName()
        {
            return "QuickSharp Sample Plugin";
        }

        /*
         * The version of the plugin.
         */

        public int GetVersion()
        {
            return 1;
        }

        /*
         * A short description of the plugin. This will appear in the
         * application about box and should be used to give information
         * about the plugin. Treat this as the plugin's about box and
         * use it to provide any copyright or extra information required.
         */

        public string GetDescription()
        {
            return "Provides the QuickSharp Sample Application with its core functionality.";
        }

        /*
         * List the plugins this plugin requires.
         */

        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            return deps;
        }

        /*
         * This is the plugin's entry point. It will receive a reference
         * to the application main form providing access to the main
         * user interface.
         */

        private MainForm mainForm;
        private ApplicationManager applicationManager;

        public void Activate(MainForm mainForm)
        {
            /*
             * Plugin activation usually requires the creation
             * of the user interface elements and the registration
             * of the functionality provided by the plugin. First
             * we save the main form reference as an instance variable
             * and get a reference to the application manager.
             */

            this.mainForm = mainForm;

            applicationManager = ApplicationManager.GetInstance();

            /*
             * Customise the layout of the About box. Use the
             * AboutFormProxy class to add an event handler that
             * will be called whenever the About box is invoked. This
             * handler receives access to the form's controls and gives
             * us the opportunity to update the form layout.
             */

            AboutFormProxy.GetInstance().FormControlUpdate +=
                new FormControlUpdateHandler(UpdateAboutBox); 
           
            /*
             * Add a docked form window.
             */

            CreateDockedForm();

            /*
             * Register the Options pages.
             */

            applicationManager.RegisterOptionsPageFactory(
                delegate { return new OptionsPage1(); });

            applicationManager.RegisterOptionsPageFactory(
                delegate { return new OptionsPage2(); });

            applicationManager.RegisterOptionsPageFactory(
                delegate { return new OptionsPage3(); });

            /*
             * Register a document type (ModuleDocument.cs).
             */

            CreateDocument();

            /*
             * Register a singleton document type. This is a
             * document type that only ever has one instance.
             */

            CreateSingletonDocument();

            /*
             * Add another toolbar. Enable this call to demonstrate
             * multiple toolbar handling.
             */

            // CreateToolbar();
        }
    }
}
