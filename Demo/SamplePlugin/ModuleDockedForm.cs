using System.Windows.Forms;
using QuickSharp.Core;

namespace SamplePlugin
{
    public partial class Module
    {
        private void CreateDockedForm()
        {
            /*
             * Create and register the form. Each docked form requires
             * a unique identifier (normally a GUID).
             */

            DockedForm dockedForm = new DockedForm(Constants.DOCKED_FORM_KEY);

            applicationManager.RegisterDockedForm(dockedForm);

            /*
             * Create the menu to allow the form to be shown and hidden
             * and add it to the main View menu.
             */

            ToolStripMenuItem dockedFormMenuItem = MenuTools.CreateMenuItem(
                Constants.UI_DOCKED_FORM_MENU_ITEM,
                "Sample &Docked Form", null, Keys.None, null,
                delegate // OnClick event handler
                {
                    if (dockedForm.Visible)
                        dockedForm.Hide();
                    else
                        dockedForm.Show();
                });

            ToolStripMenuItem viewMenu = mainForm.GetMenuItemByName(
                QuickSharp.Core.Constants.UI_VIEW_MENU);

            viewMenu.DropDownItems.Add(dockedFormMenuItem);

            /*
             * Add an event handler to allow us to set the state of the
             * menu item according to the visibility of the form.
             */

            viewMenu.DropDownOpening += delegate
                { dockedFormMenuItem.Checked = dockedForm.Visible; };
        }
    }
}
