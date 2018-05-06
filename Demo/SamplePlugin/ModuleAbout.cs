using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuickSharp.Core;

namespace SamplePlugin
{
    public partial class Module
    {
        private void UpdateAboutBox(Control.ControlCollection controls)
        {
            /*
             * This illustrates how to manipulate the controls of
             * the existing About box to provide a cutomised appearance. An
             * alternative approach is to provide a completely new form
             * and a factory method to create and return an instance. Use the
             * ClientProfile AboutBoxFactory property to replace the existing
             * About form factory with the new form factory.
             */
            
            /*
             * Get references to the form and its controls.
             */

            AboutForm aboutForm = controls.Owner as AboutForm;

            Label clientCopyrightLabel = 
                controls[AboutForm.m_clientCopyrightLabel] as Label;
            Label coreCopyrightLabel = 
                controls[AboutForm.m_coreCopyrightLabel] as Label;
            Label dockpanelCopyrightLabel = 
                controls[AboutForm.m_dockpanelCopyrightLabel] as Label;
            ListBox pluginListBox = 
                controls[AboutForm.m_pluginListBox] as ListBox;
            Label installedPluginsLabel = 
                controls[AboutForm.m_installedPluginsLabel] as Label;
            TextBox pluginDescriptionTextBox = 
                controls[AboutForm.m_pluginDescriptionTextBox] as TextBox;
            Label pluginDetailsLabel = 
                controls[AboutForm.m_pluginDetailsLabel] as Label;
            Button okButton = 
                controls[AboutForm.m_okButton] as Button;
            
            /*
             * Resize the form.
             */

            aboutForm.ClientSize = new Size(453, 215);

            /*
             * Move the OK button.
             */

            okButton.Location = new Point(363, 177);

            /*
             * Move the copyright labels and make the application
             * copyright text bold.
             */

            clientCopyrightLabel.Location = new Point(12, 20);
            clientCopyrightLabel.Font = new Font("Tahoma", 8.25F,
                FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));

            coreCopyrightLabel.Location = new Point(12, 79);
            dockpanelCopyrightLabel.Location = new Point(12, 128);

            /*
             * Hide the plugin information controls.
             */

            installedPluginsLabel.Visible = false;
            pluginListBox.Visible = false;
            pluginDetailsLabel.Visible = false;
            pluginDescriptionTextBox.Visible = false;

            /*
             * Create the 'Show Plugin Details' button.
             */

            Button showButton = new Button();
            showButton.Location = new Point(12, 177);
            showButton.Name = "showButton";
            showButton.Size = new Size(120, 23);
            showButton.TabIndex = 10;
            showButton.Text = "Show Plugin Details";
            showButton.UseVisualStyleBackColor = true;

            /*
             * When clicked the button resizes the form and
             * shows the plugin information.
             */

            showButton.Click += delegate
            {
                aboutForm.ClientSize = new Size(453, 400);
                installedPluginsLabel.Visible = true;
                pluginListBox.Visible = true;
                pluginDetailsLabel.Visible = true;
                pluginDescriptionTextBox.Visible = true;
                showButton.Visible = false;
            };

            controls.Add(showButton);
        }
    }
}
