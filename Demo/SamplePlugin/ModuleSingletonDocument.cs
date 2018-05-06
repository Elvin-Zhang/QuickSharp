using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuickSharp.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace SamplePlugin
{
    public partial class Module
    {
        private DocumentType singletonDocumentType;

        private void CreateSingletonDocument()
        {
            singletonDocumentType = new DocumentType(
                Constants.SINGLETON_DOCUMENT_TYPE);

            /*
             * Create the document type and register the open
             * document handler.
             */

            applicationManager.RegisterOpenDocumentHandler(
                singletonDocumentType, OpenSingletonDocument);

            /*
             * Create a menu item to open the document.
             */

            ToolStripMenuItem singletonMenuItem = MenuTools.CreateMenuItem(
                Constants.UI_TOOLS_MENU_SINGLETON,
                "&Singleton Document",
                null, Keys.None, null,
                SingletonMenuItem_Click,
                true); // Add a separator after this item

            ToolStripMenuItem toolsMenu = mainForm.GetMenuItemByName(
                QuickSharp.Core.Constants.UI_TOOLS_MENU);

            toolsMenu.DropDownItems.Insert(0, singletonMenuItem);
        }

        public IDockContent OpenSingletonDocument(string path, bool readOnly)
        {
            /*
             * Activate the document if it's already been loaded.
             */

            foreach (Document d in mainForm.ClientWindow.Documents)
            {
                if (d.FilePath != null &&
                    d.FilePath.ToLower() == path.ToLower() &&
                    !d.AllowDuplicates())
                {
                    d.Activate();
                    return null;
                }
            }

            /*
             * Create the document.
             */

            SingletonDocumentForm form = new SingletonDocumentForm();
            form.Text = "Singleton Document";
            form.FileName = Constants.SINGLETON_DOCUMENT_NAME;
            form.FilePath = Constants.SINGLETON_DOCUMENT_NAME;

            return form;
        }

        /*
         * Singleton document handling is a bit of a hack - we use the
         * document management system to open the document as normal
         * but bypass the usual file opening dialog to make sure
         * we always pass the same filename to the load document method.
         * Because we are bypassing the standard open method we need to
         * handle the document content ourselves. Note each singleton
         * document in the application must have a unique name; choose
         * a filename that is unlikely to occur in normal use.
         */

        private void SingletonMenuItem_Click(object sender, EventArgs e)
        {
            /*
             * Don't use the MRU list for singletons.
             */

            mainForm.LoadDocumentIntoWindow(
                Constants.SINGLETON_DOCUMENT_NAME, false);
        }
    }
}
