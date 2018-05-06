using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuickSharp.Core;

namespace SamplePlugin
{
    public partial class Module
    {
        private void CreateToolbar()
        {
            /*
             * By default QuickSharp uses a single toolbar which you can
             * access via the MainForm.MainToolbar property. If additional
             * toolbars are registered it will automatically switch to
             * multi-toolbar mode.
             */

            ToolStrip demoToolbar = new ToolStrip();

            /*
             * These are required, the name is used to register the toolbar
             * in the toolbar collection and the text is used for the
             * toolbar menu entry.
             */

            demoToolbar.Name = Constants.UI_TOOLBAR;
            demoToolbar.Text = "&Demo Toolbar";

            /*
             * Create the buttons and add to the toolbar.
             */

            ToolStripButton button1 = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_BUTTON1,
                "Button 1",
                Resources.ToolbarButton,
                DemoToolbarButton_Click);

            ToolStripButton button2 = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_BUTTON2,
                "Button 2",
                Resources.ToolbarButton,
                DemoToolbarButton_Click);

            ToolStripButton button3 = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_BUTTON3,
                "Button 3",
                Resources.ToolbarButton,
                DemoToolbarButton_Click,
                true); // Add a separator before the button

            demoToolbar.Items.Add(button1);
            demoToolbar.Items.Add(button2);
            demoToolbar.Items.Add(button3);

            /*
             * Register the toolbar with the main form. The numbers
             * are the row and column hints. For multi-row toolbars
             * the row hint indicates the row the toolbar should appear
             * on when first created. The column hint indicates a bias
             * for the horizontal positioning of the tool; the higher
             * the value the further to the right it will appear. Neither
             * of these are absolute and depend on the other toolbars
             * in the application. For example, a row hint of 3 will not
             * put a toolbar on row 3 if there is only one row.
             */

            mainForm.AddDockedToolStrip(demoToolbar, 0, 10);
        }

        private void DemoToolbarButton_Click(object sender, EventArgs e)
        {
        }
    }
}
