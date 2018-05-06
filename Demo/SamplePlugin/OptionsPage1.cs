using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuickSharp.Core;
 
namespace SamplePlugin
{
    public class OptionsPage1 : OptionsPage
    {
        public OptionsPage1()
        {
            /*
             * Option pages are organised by group text. If two
             * pages have the same group text they will be presented
             * in the options editor tree as two sub-pages of a node
             * named after the group text. The sub-pages will each
             * have their page text as the title. If a page is the
             * only member of a group it will appear in the tree as
             * a single node with the group text as its title.
             * 
             * Note in the tree the parent nodes are presented in
             * alphabetical order but sub-nodes are presented in the
             * order they are registered. More than one plugin can
             * contribute to a group; in these cases the order of the
             * sub-items will depend on plugin load order as well as
             * registration order.
             */

            Name = Constants.UI_OPTIONS_PAGE1;
            PageText = "Options Page 1";

            /*
             * Page1 and Page2 share the same group text and will
             * appear as sub-pages of the same tree node.
             */

            GroupText = "Options Group 1";

            /*
             * Create the form layout.
             * 
             * Unfortunately the Visual Studio form designer doesn't
             * work with isolated controls so the form controls have
             * to be added manually.
             */

            GroupBox groupBox = new GroupBox();
            groupBox.Size = new Size(430, 250);
            groupBox.Text = PageText;

            Controls.Add(groupBox);
        }

        public override bool Validate()
        {
            /*
             * Add validation code here; return false to cancel
             * saving of the settings.
             */

            return true;
        }

        public override void Save()
        {
        }
    }
}
