using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuickSharp.Core;
 
namespace SamplePlugin
{
    public class OptionsPage2 : OptionsPage
    {
        public OptionsPage2()
        {
            Name = Constants.UI_OPTIONS_PAGE2;
            PageText = "Options Page 2";
            GroupText = "Options Group 1";

            /*
             * Create the form layout.
             */

            GroupBox groupBox = new GroupBox();
            groupBox.Size = new Size(430, 250);
            groupBox.Text = PageText;

            Controls.Add(groupBox);
        }

        public override bool Validate()
        {
            return true;
        }

        public override void Save()
        {
        }
    }
}
