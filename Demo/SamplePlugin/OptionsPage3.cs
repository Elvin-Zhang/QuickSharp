using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuickSharp.Core;
 
namespace SamplePlugin
{
    public class OptionsPage3 : OptionsPage
    {
        IPersistenceManager persistenceManager;
        TextBox textMessage;

        /*
         * In this sample options page a text value is presented for editing
         * and will be saved to the registry. A validate method prevents
         * an empty message from being saved.
         */

        public OptionsPage3()
        {
            Name = Constants.UI_OPTIONS_PAGE3;
            PageText = "Options Page 3";
            GroupText = "Options Group 2";

            /*
             * Create the form layout.
             */

            textMessage = new TextBox();
            textMessage.Location = new Point(15, 30);
            textMessage.Size = new Size(400, 20);

            GroupBox groupBox = new GroupBox();
            groupBox.Size = new Size(430, 250);
            groupBox.Text = PageText;
            groupBox.Controls.Add(textMessage);

            Controls.Add(groupBox);

            /*
             * Create a persistence manager instance and load the
             * stored settings.
             */

            persistenceManager = ApplicationManager.GetInstance().
                GetPersistenceManager(Constants.PLUGIN_NAME);

            textMessage.Text = persistenceManager.ReadString(
                Constants.KEY_OPTION_TEXT_MESSAGE,
                "Enter a message here");
        }

        public override bool Validate()
        {
            /*
             * Check the message text is not blank. If so update the
             * UI to give a warning and cancel the save.
             */

            textMessage.Text = textMessage.Text.Trim();

            if (textMessage.Text == String.Empty)
            {
                textMessage.BackColor = Color.Yellow;
                return false;
            }

            return true;
        }

        public override void Save()
        {
            /*
             * Save the new value.
             */

            persistenceManager.WriteString(
                Constants.KEY_OPTION_TEXT_MESSAGE,
                textMessage.Text);
        }
    }
}
