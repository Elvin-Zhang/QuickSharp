using System;
using System.Windows.Forms;

namespace SamplePlugin
{
    public partial class DocumentForm : QuickSharp.Core.Document
    {
        public DocumentForm()
        {
            InitializeComponent();

            /*
             * Register action and action state handlers. These are used
             * to link the menu and toolbar items added by the module to
             * the corresponding event handlers in this form.
             * 
             * The first set of handlers apply to the menu and toolbar
             * items we have added in this plugin.
             */

            RegisterActionHandler(
                Constants.UI_IMAGE_MENU_STRETCH, ToggleSizingMode);

            RegisterActionStateHandler(
                Constants.UI_IMAGE_MENU_STRETCH, CanToggleSizingMode);

            RegisterActionHandler(
                Constants.UI_TOOLBAR_STRETCH, ToggleSizingMode);

            RegisterActionStateHandler(
                Constants.UI_TOOLBAR_STRETCH, CanToggleSizingMode);

            /*
             * We also need to link up with the exisiting menu and toolbar
             * items for the save operations provided by the main form.
             */

            RegisterActionHandler(
                QuickSharp.Core.Constants.UI_FILE_MENU_SAVE, Save);

            RegisterActionHandler(
                QuickSharp.Core.Constants.UI_FILE_MENU_SAVE_AS, Save);

            RegisterActionStateHandler(
                QuickSharp.Core.Constants.UI_FILE_MENU_SAVE, CanSave);

            RegisterActionStateHandler(
                QuickSharp.Core.Constants.UI_FILE_MENU_SAVE_AS, CanSave);

            RegisterActionHandler(
                QuickSharp.Core.Constants.UI_TOOLBAR_SAVE, Save);

            RegisterActionStateHandler(
                QuickSharp.Core.Constants.UI_TOOLBAR_SAVE, CanSave);

            /*
             * This level of indirection is necessary because even though
             * a plugin might provide menu items and their action methods
             * these are in fact two separate parts of the application.
             * Once an item has been added to a menu it is available 
             * throughout the application and there needs to be a means to
             * enable/disable the menu items as the active document changes.
             * Also, a menu item might apply to more than one plugin (as
             * with the save actions here) so it is necessary to provide
             * a means to link up with existing menu items.
             *
             * Essentially the menu items 'publish' actions and the
             * action and action state handlers provide a means to
             * 'subscribe' to them by providing an implementation
             * of an action and the means for the application to determine
             * if the implementation is available.
             * 
             * Note a plugin only needs to provide action state handlers if
             * it is 'interested' in an action; if the application finds
             * no handlers for a particular action in a plugin it assumes
             * the plugin has no interest and the menu item becomes inactive
             * when the plugin's documents are active.
             * 
             * ActionState handlers can also return false to disable a menu
             * when it is not needed. For example a clipboard 'paste' command
             * could be maintained by a plugin but would need to be inactive
             * if there was no clipboard content to paste.
             */
        }

        public PictureBox Image
        {
            get { return pictureBox; }
        }

        public bool Stretch
        {
            get
            {
                return pictureBox.SizeMode ==
                    PictureBoxSizeMode.StretchImage;
            }
            set
            {
                if (value)
                    pictureBox.SizeMode =
                        PictureBoxSizeMode.StretchImage;
                else
                    pictureBox.SizeMode =
                        PictureBoxSizeMode.CenterImage;
            }
        }

        /*
         * Action handlers.
         */
        
        public bool ToggleSizingMode()
        {
            Stretch = !Stretch;

            return true;
        }

        public bool Save()
        {
            /*
             * For this demo we do nothing but normally
             * this method would save the data and we would
             * also have a Save As method.
             */

            MessageBox.Show(
                String.Format("Document {0} has been saved", FileName),
                "Save",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return true;
        }

        /*
         * Action State Handlers.
         */

        public bool CanToggleSizingMode()
        {
            return true;
        }

        public bool CanSave()
        {
            /*
             * Always allow the document to be saved.
             */

            return true;
        }
    }
}
