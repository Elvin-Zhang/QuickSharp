using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SamplePlugin
{
    public partial class SingletonDocumentForm : QuickSharp.Core.Document
    {
        public SingletonDocumentForm()
        {
            InitializeComponent();
        }

        /*
         * For this demo we aren't allowing the singleton document
         * to be reloaded if session documents are restored on startup.
         * This might be the right choice if the singleton requires
         * a lot of setup and it's restoration might create an excessive
         * startup time for the application. To make it reloadable return
         * the document name from this method as in the commented out
         * code below.
         */

        protected override string GetPersistString()
        {
            return String.Empty;
            //return Constants.SINGLETON_DOCUMENT_NAME; 
        }
    }
}
