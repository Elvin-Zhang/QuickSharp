/*
 * QuickSharp Copyright (C) 2008-2011 Steve Walker.
 *
 * This file is part of QuickSharp.
 *
 * QuickSharp is free software: you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation, either version 3 of the License, or (at your option)
 * any later version.
 *
 * QuickSharp is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
 * for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with QuickSharp. If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using QuickSharp.Core;

namespace QuickSharp.CodeAssist.DotNet
{
    public class CodeAssistOptionsPage : OptionsPage
    {
        private SettingsManager _settingsManager;
        private ReferenceManager _referenceManager;
        private bool _disableColorization;

        private GroupBox _referenceGroupBox;
        private Button _manageNamespacesButton;
        private Button _reloadNamespacesButton;
        private GroupBox _colorizeGroupBox;
        private CheckBox _colorizeVariablesCheckBox;
        private CheckBox _colorizeTypesCheckBox;
        private CheckBox _colorizeOnActivateCheckBox;
        private CheckBox _colorizeOnLookupCheckBox;

        #region Form Control Names

        public const string m_referenceGroupBox = "referenceGroupBox";
        public const string m_manageNamespacesButton = "manageNamespacesButton";
        public const string m_reloadNamespacesButton = "reloadNamespacesButton";
        public const string m_colorizeGroupBox = "colorizeGroupBox";
        public const string m_colorizeVariablesCheckBox = "colorizeVariablesCheckBox";
        public const string m_colorizeTypesCheckBox = "colorizeTypesCheckBox";
        public const string m_colorizeOnActivateCheckBox = "colorizeOnActivateCheckBox";
        public const string m_colorizeOnLookupCheckBox = "colorizeOnLookupCheckBox";

        #endregion

        private ReferenceDatabase _activeNamespaces;
        private ReferenceDatabase _inactiveNamespaces;

        public CodeAssistOptionsPage()
        {
            Name = Constants.UI_OPTIONS_CODE_ASSIST_PAGE;
            PageText = Resources.OptionsPageText;
            GroupText = Resources.OptionsGroupText;

            _settingsManager = SettingsManager.GetInstance();
            _referenceManager = ReferenceManager.GetInstance();

            _referenceGroupBox = new GroupBox();
            _manageNamespacesButton = new Button();
            _reloadNamespacesButton = new Button();

            Controls.Add(_referenceGroupBox);

            ApplicationManager applicationManager = 
                ApplicationManager.GetInstance();
            
            _disableColorization = applicationManager.ClientProfile.HaveFlag(
                ClientFlags.CodeAssistDotNetDisableColorization);

            bool enableReload = !applicationManager.ClientProfile.HaveFlag(
                ClientFlags.CodeAssistDotNetDisableReloadDatabase);

            if (!_disableColorization)
            {
                _colorizeGroupBox = new GroupBox();
                _colorizeTypesCheckBox = new CheckBox();
                _colorizeVariablesCheckBox = new CheckBox();
                _colorizeOnActivateCheckBox = new CheckBox();
                _colorizeOnLookupCheckBox = new CheckBox();

                Controls.Add(_colorizeGroupBox);
            }

            #region Form Layout

            _referenceGroupBox.Controls.Add(_manageNamespacesButton);

            if (enableReload)
                _referenceGroupBox.Controls.Add(_reloadNamespacesButton);

            _referenceGroupBox.Location = new Point(0, 0);
            _referenceGroupBox.Name = m_referenceGroupBox;
            _referenceGroupBox.TabIndex = 0;
            _referenceGroupBox.TabStop = false;
            _referenceGroupBox.Text = Resources.OptionsReferenceGroupBox;
            _referenceGroupBox.Size = new Size(430, 100);

            _manageNamespacesButton.Name = m_manageNamespacesButton;
            _manageNamespacesButton.Size = new Size(98, 23);
            _manageNamespacesButton.TabIndex = 1;
            _manageNamespacesButton.Text = Resources.OptionsManageNamespacesButton;
            _manageNamespacesButton.UseVisualStyleBackColor = true;

            if (enableReload)
                _manageNamespacesButton.Location = new Point(111, 40);
            else
                _manageNamespacesButton.Location = new Point(166, 40);

            if (enableReload)
            {
                _reloadNamespacesButton.Name = m_reloadNamespacesButton;
                _reloadNamespacesButton.Size = new Size(98, 23);
                _reloadNamespacesButton.TabIndex = 2;
                _reloadNamespacesButton.Text = Resources.OptionsReloadNamespacesButton;
                _reloadNamespacesButton.UseVisualStyleBackColor = true;
                _reloadNamespacesButton.Location = new Point(221, 40);
            }

            if (!_disableColorization)
            {
                _colorizeGroupBox.Controls.Add(_colorizeTypesCheckBox);
                _colorizeGroupBox.Controls.Add(_colorizeVariablesCheckBox);
                _colorizeGroupBox.Controls.Add(_colorizeOnActivateCheckBox);
                _colorizeGroupBox.Controls.Add(_colorizeOnLookupCheckBox);
                _colorizeGroupBox.Location = new Point(0, 110);
                _colorizeGroupBox.Name = m_colorizeGroupBox;
                _colorizeGroupBox.Size = new Size(430, 140);
                _colorizeGroupBox.TabIndex = 3;
                _colorizeGroupBox.TabStop = false;
                _colorizeGroupBox.Text = Resources.OptionsColorizeGroupBox;

                _colorizeTypesCheckBox.AutoSize = true;
                _colorizeTypesCheckBox.Location = new Point(19, 31);
                _colorizeTypesCheckBox.Name = m_colorizeTypesCheckBox;
                _colorizeTypesCheckBox.TabIndex = 4;
                _colorizeTypesCheckBox.Text = Resources.OptionsColorizeTypes;
                _colorizeTypesCheckBox.UseVisualStyleBackColor = true;

                _colorizeVariablesCheckBox.AutoSize = true;
                _colorizeVariablesCheckBox.Location = new Point(19, 54);
                _colorizeVariablesCheckBox.Name = m_colorizeVariablesCheckBox;
                _colorizeVariablesCheckBox.TabIndex = 5;
                _colorizeVariablesCheckBox.Text = Resources.OptionsColorizeVariables;
                _colorizeVariablesCheckBox.UseVisualStyleBackColor = true;

                _colorizeOnActivateCheckBox.AutoSize = true;
                _colorizeOnActivateCheckBox.Location = new Point(19, 77);
                _colorizeOnActivateCheckBox.Name = m_colorizeOnActivateCheckBox;
                _colorizeOnActivateCheckBox.TabIndex = 6;
                _colorizeOnActivateCheckBox.Text = Resources.OptionsColorizeOnActivate;
                _colorizeOnActivateCheckBox.UseVisualStyleBackColor = true;
 
                _colorizeOnLookupCheckBox.AutoSize = true;
                _colorizeOnLookupCheckBox.Location = new Point(19, 100);
                _colorizeOnLookupCheckBox.Name = m_colorizeOnLookupCheckBox;
                _colorizeOnLookupCheckBox.TabIndex = 7;
                _colorizeOnLookupCheckBox.Text = Resources.OptionsColorizeOnLookup;
                _colorizeOnLookupCheckBox.UseVisualStyleBackColor = true;
            }

            #endregion
             
            _manageNamespacesButton.Click +=
                new EventHandler(ManageNamespacesButton_Click);

            if (enableReload)
                _reloadNamespacesButton.Click +=
                    new EventHandler(reloadNamespacesButton_Click);

            if (!_disableColorization)
            {
                _colorizeTypesCheckBox.Checked = 
                    _settingsManager.ColorizeTypes;
                _colorizeVariablesCheckBox.Checked = 
                    _settingsManager.ColorizeVariables;
                _colorizeOnActivateCheckBox.Checked = 
                    _settingsManager.ColorizeOnActivate;
                _colorizeOnLookupCheckBox.Checked = 
                    _settingsManager.ColorizeOnLookup;
            }

            /*
             * Copy the reference database to allow chnages to be
             * abandoned on exit if required. Not a full copy as 
             * we're not copying the lists but as these won't change
             * we can just copy them as references.
             */

            _activeNamespaces = new ReferenceDatabase();
            _inactiveNamespaces = new ReferenceDatabase();

            foreach (ReferenceNamespace ns in
                _referenceManager.ActiveNamespaces.Values)
                _activeNamespaces[ns.Name] = ns;

            foreach (ReferenceNamespace ns in
                _referenceManager.InactiveNamespaces.Values)
                _inactiveNamespaces[ns.Name] = ns;
        }

        private void ManageNamespacesButton_Click(
            object sender, EventArgs e)
        {
            ManageReferenceDatabaseForm f =
                new ManageReferenceDatabaseForm(
                    _activeNamespaces, _inactiveNamespaces);

            f.ShowDialog();

            _activeNamespaces = f.ActiveNamespaces;
            _inactiveNamespaces = f.InactiveNamespaces;
        }

        private void reloadNamespacesButton_Click(
            object sender, EventArgs e)
        {
            if (MessageBox.Show(String.Format("{0}\r\n{1}",
                    Resources.OptionsReloadWarningMessage1,
                    Resources.OptionsReloadWarningMessage2),
                Resources.OptionsReloadWarningTitle,
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning) == DialogResult.OK)
            {
                CreateReferenceDatabaseForm form =
                    new CreateReferenceDatabaseForm();

                // Clear the MessageBox
                Refresh();

                form.ShowDialog();

                /*
                 * The form is now populated with the reloaded
                 * database. We need to update the local copies
                 * for editing.
                 */

                ReferenceDatabase database = form.ReferenceDatabase;

                _activeNamespaces.Clear();
                _inactiveNamespaces.Clear();

                foreach (String ns in database.Keys)
                {
                    if (database[ns].Name.StartsWith("System"))
                        _activeNamespaces[ns] = database[ns];
                    else
                        _inactiveNamespaces[ns] = database[ns];
                }
            }
        }

        #region Save

        public override void Save()
        {
            /*
             * Save the registry settings.
             */

            if (!_disableColorization)
            {
                _settingsManager.ColorizeTypes = 
                    _colorizeTypesCheckBox.Checked;
                _settingsManager.ColorizeVariables = 
                    _colorizeVariablesCheckBox.Checked;
                _settingsManager.ColorizeOnActivate =
                    _colorizeOnActivateCheckBox.Checked;
                _settingsManager.ColorizeOnLookup =
                    _colorizeOnLookupCheckBox.Checked;

                _settingsManager.Save();
            }

            /*
             * Write the reference database if it has been updated.
             */

            if (_activeNamespaces == null || _inactiveNamespaces == null)
                return;

            /*
             * Sanitize the lists first.
             */

            /*
             * Make sure we don't have any namespaces without assemblies.
             * (Not sure if that could happen but better safe than sorry.)
             */

            Dictionary<string, ReferenceNamespace> cleanActiveNamespaces =
                 new Dictionary<string, ReferenceNamespace>();

            foreach (string key in _activeNamespaces.Keys)
            {
                if (_activeNamespaces[key].AssemblyList.Count > 0)
                    cleanActiveNamespaces[key] = _activeNamespaces[key];
            }

            /*
             * Check for any empty namespaces and also dump any '@' 
             * referenced assemblies so they don't build up.
             */

            ReferenceDatabase cleanInactiveNamespaces =
                new ReferenceDatabase();

            foreach (string key in _inactiveNamespaces.Keys)
            {
                ReferenceNamespace ns = new ReferenceNamespace(key);
                
                foreach (string s in _inactiveNamespaces[key].AssemblyList)
                    if (!s.StartsWith("@")) ns.AddAssembly(s);

                if (ns.AssemblyList.Count > 0)
                    cleanInactiveNamespaces[ns.Name] = ns;
            }

            /*
             * Write the data to the database file.
             */

            string path = _referenceManager.ReferenceDatabasePath;

            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(path);

                foreach (string key in cleanActiveNamespaces.Keys)
                {
                    sw.WriteLine("+" + key);

                    foreach (string assmembly in
                        cleanActiveNamespaces[key].AssemblyList)
                        sw.WriteLine(assmembly);
                }

                foreach (string key in cleanInactiveNamespaces.Keys)
                {
                    sw.WriteLine("-" + key);

                    foreach (string assmembly in
                        cleanInactiveNamespaces[key].AssemblyList)
                        sw.WriteLine(assmembly);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}:\r\n{1}",
                        Resources.CreateDbErrorMessage,
                        ex.Message),
                    Resources.CreateDbErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                if (sw != null) sw.Close();
            }

            /*
             * Reread the database.
             */

            ReferenceManager.GetInstance().LoadReferenceDatabaseFromFile();
        }

        #endregion
    }
}
