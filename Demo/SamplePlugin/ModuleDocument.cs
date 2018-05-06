using System;
using System.IO;
using System.Windows.Forms;
using QuickSharp.Core;
using WeifenLuo.WinFormsUI.Docking; 

namespace SamplePlugin
{
    public partial class Module
    {
        private DocumentType documentType;
        private int documentCount = 1;

        private ToolStripMenuItem imageMenu;
        private ToolStripMenuItem imageMenuStretch;
        private ToolStripButton toolbarButtonStretch;

        private void CreateDocument()
        {
            /*
             * Create a document type, set it as the default document and
             * register the new and open document handlers.
             * 
             * For this demo we are creating a document that presents
             * bitmap images and allows them to be shown normally or
             * stretched. Open will open an existing .bmp file; New will
             * create blank documents. As this is a demo there is no
             * functionality to create or modify images but enough to
             * illustrate the document creation process.
             */

            documentType = new DocumentType(".bmp");
            applicationManager.NewDocumentType = documentType;
            applicationManager.NewDocumentHandler = NewDocument;
            applicationManager.RegisterOpenDocumentHandler(
                documentType, OpenDocument);

            /*
             * Add a filter provider for the open file dialog.
             */

            applicationManager.DocumentFilterHandlers.Add(
                delegate { return "Bitmap Files (*.bmp)|*.bmp|"; });

            /*
             * Add an image menu before the Tools menu and add
             * a menu item to toggle the current image sizing mode.
             */

            imageMenu = MenuTools.CreateMenuItem(
                Constants.UI_IMAGE_MENU,
                "&Image", null, Keys.None, null, null);

            int index = mainForm.GetMainMenuItemIndexByName(
                QuickSharp.Core.Constants.UI_TOOLS_MENU);

            mainForm.MainMenu.Items.Insert(index, imageMenu);

            imageMenuStretch = MenuTools.CreateMenuItem(
                Constants.UI_IMAGE_MENU_STRETCH,
                "&Stretch Image",
                Resources.StetchImage,
                Keys.None, null, MenuItem_Click);

            /*
             * Document related menu items should start disabled.
             */

            imageMenuStretch.Enabled = false;

            /*
             * Add the item to the menu.
             */

            imageMenu.DropDownItems.Add(imageMenuStretch);

            /*
             * Add a button to the main toolbar.
             */

            toolbarButtonStretch = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_STRETCH,
                "Stretch image",
                Resources.StetchImage,
                ToolbarItem_Click,
                true); // Add a separator before the button

            toolbarButtonStretch.Enabled = false; // Start disabled

            mainForm.MainToolbar.Items.Add(toolbarButtonStretch);

            /*
             * Assign event handlers for the menu being opened and the
             * active document changing. Both of these are required to
             * maintain the current state of the UI items.
             */

            imageMenu.DropDownOpening += 
                delegate { UpdateImageMenu(); };

            mainForm.ClientWindow.ActiveDocumentChanged +=
                delegate { ActiveDocumentChanged(); };
        }

        /*
         * The following methods are responsible for creating new
         * documents and opening existing documents from file paths
         * supplied by the document management system.
         */

        public IDockContent NewDocument()
        {
            /*
             * This doesn't mean much for this demo as we'll just
             * end up with an empty document.
             */

            DocumentForm form = new DocumentForm();
            form.Text = String.Format("untitled{0}{1}",
                documentCount++, documentType.ToString());
            form.FileName = form.Text;
            form.FilePath = null;

            return form;
        }

        public IDockContent OpenDocument(string path, bool readOnly)
        {
            /*
             * Activate the file if it's already been loaded.
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
             * Create a new document and load the image.
             */

            FileInfo fileInfo = new FileInfo(path);

            DocumentForm form = new DocumentForm();
            form.Text = fileInfo.Name;
            form.FileName = form.Text;
            form.FilePath = fileInfo.FullName;

            /*
             * Load the data.
             */

            try
            {
                form.Image.Load(fileInfo.FullName);
            }
            catch
            {
                // Return null if open fails.
                return null;
            }
            
            return form;
        }

        /*
         * The following methods maintain the state of the UI items.
         */

        private void ActiveDocumentChanged()
        {
            /*
             * Add an entry for each menu and toolbar item.
             */

            imageMenuStretch.Enabled =
                IsMenuItemEnabled(imageMenuStretch);

            toolbarButtonStretch.Enabled =
                IsToolbarItemEnabled(toolbarButtonStretch);

            /*
             * Additonal state updates - we don't want a disabled item to
             * be checked.
             */

            if (!imageMenuStretch.Enabled)
                imageMenuStretch.Checked = false;

            UpdateToolbar();
        }

        private void UpdateImageMenu()
        {
            /*
             * When the menu is opened check if it is the correct
             * document type and then set the menu state based on the
             * document properties. Again there could be multiple
             * menu items here but we only have one for this demo.
             */

            DocumentForm form = mainForm.ActiveDocument as DocumentForm;
            if (form == null) return;

            imageMenuStretch.Checked = form.Stretch;
        }

        private void UpdateToolbar()
        {
            /*
             * The toolbar is always visible so we have to make
             * some effort to keep it up to date. This method
             * is called wherever an action could change the state
             * of the toolbar.
             */

            if (toolbarButtonStretch.Enabled)
            {
                DocumentForm form = 
                    mainForm.ActiveDocument as DocumentForm;

                if (form != null)
                    toolbarButtonStretch.Checked = form.Stretch;
            }
            else
            {
                toolbarButtonStretch.Checked = false;
            }
        }

        /*
         * The following methods are generic and apply to all the
         * UI items. All menu items should use MenuItem_Click for
         * their event hander and toolbars should use ToolbarItem_Click.
         * These and the corresponding IsMenuEnabled and IsToolbarEnabled
         * methods connect with the Action and ActionState handlers
         * registered in the form. The action methods call the appropriate
         * methods to initiate each action and the action state methods
         * determine if the action is available.
         */

        private bool IsMenuItemEnabled(ToolStripMenuItem menuItem)
        {
            if (mainForm.ActiveDocument == null)
                return false;
            else
                return mainForm.ActiveDocument.IsMenuItemEnabled(menuItem);
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            if (mainForm.ActiveDocument == null) return;
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem == null) return;

            mainForm.ActiveDocument.PerformMenuAction(menuItem);

            UpdateToolbar();
        }

        private bool IsToolbarItemEnabled(ToolStripButton button)
        {
            if (mainForm.ActiveDocument == null)
                return false;
            else
                return mainForm.ActiveDocument.IsToolbarButtonEnabled(button);
        }

        private void ToolbarItem_Click(object sender, EventArgs e)
        {
            if (mainForm.ActiveDocument == null) return;
            ToolStripButton button = sender as ToolStripButton;
            if (button == null) return;

            mainForm.ActiveDocument.PerformToolbarAction(button);

            UpdateToolbar();
        }
    }
}
