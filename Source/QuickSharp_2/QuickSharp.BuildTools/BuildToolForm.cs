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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using QuickSharp.Core;

namespace QuickSharp.BuildTools
{
    /// <summary>
    /// The build tool configuration form.
    /// </summary>
    public partial class BuildToolForm : Form
    {
        private BuildToolManager _buildToolManager;
        private BuildToolCollection _buildTools;
        private string _toolId;

        /// <summary>
        /// Create a new instance of the form. The form is responsible for retrieving and updating
        /// the tool definition in the build tool collection.
        /// </summary>
        /// <param name="buildTools">The collection of build tools registered with the build system.</param>
        /// <param name="toolId">The build tool to be configured.</param>
        public BuildToolForm(BuildToolCollection buildTools, string toolId)
        {
            InitializeComponent();

            _buildToolManager = BuildToolManager.GetInstance();
            _buildTools = buildTools;
            _toolId = toolId;

            List<String> documentTypes = 
                _buildToolManager.GetAvailableDocumentTypes();

            if (documentTypes.Count == 0)
            {
                DisableForm();
            }
            else
            {
                foreach (string documentType in documentTypes)
                    _documentTypeComboBox.Items.Add(documentType);

                LoadTool();
            }

            /*
             * Allow client applications to modify the form.
             */

            BuildToolFormProxy.GetInstance().
                UpdateFormControls(Controls);
        }

        #region Load/Save

        /// <summary>
        /// Load the build tool to be configured or create a new configuration. 
        /// </summary>
        private void LoadTool()
        {
            if (_toolId == null)
            {
                _documentTypeComboBox.Enabled = true;
                _actionComboBox.Enabled = true;
                _parserComboBox.Enabled = true;

                _documentTypeComboBox.SelectedIndex = 0;
            }
            else
            {
                BuildTool tool = _buildTools.Tools[_toolId];
                if (tool == null) 
                    throw new Exception("Error retrieving build tool");

                _documentTypeComboBox.Enabled = false;
                _actionComboBox.Enabled = false;
                _parserComboBox.Enabled = true;

                int i = _documentTypeComboBox.Items.IndexOf(
                    tool.DocumentType.ToString());
                
                if (i != -1)
                    _documentTypeComboBox.SelectedIndex = i;

                UpdateActionComboBox();

                i = _actionComboBox.Items.IndexOf(tool.Action);
                
                if (i != -1)
                    _actionComboBox.SelectedIndex = i;

                UpdateParserComboBox();

                if (String.IsNullOrEmpty(tool.LineParserName))
                    _parserComboBox.SelectedIndex = 0;
                else
                {
                    i = _parserComboBox.Items.IndexOf(tool.LineParserName);
                    
                    if (i != -1)
                        _parserComboBox.SelectedIndex = i;
                }

                _displayNameTextBox.Text = tool.DisplayName;
                _toolPathTextBox.Text = tool.Path;
                _toolArgsTextBox.Text = tool.Args;
                _userArgsTextBox.Text = tool.UserArgs;
            }
        }

        /// <summary>
        /// Create or update the tool in the build tools collection.
        /// </summary>
        /// <returns></returns>
        private bool SaveTool()
        {
            _displayNameTextBox.BackColor = Color.Empty;
            _toolPathTextBox.BackColor = Color.Empty;

            string id =
                (_toolId != null) ? _toolId : Guid.NewGuid().ToString();

            string documentTypeString =
                _documentTypeComboBox.SelectedItem as string;

            string displayName = _displayNameTextBox.Text.Trim();
            string toolAction = _actionComboBox.SelectedItem as string;
            string toolPath = _toolPathTextBox.Text.Trim();
            string toolArgs = _toolArgsTextBox.Text.Trim();
            string userArgs = _userArgsTextBox.Text.Trim();
            string lineParserName = String.Empty;

            if (_parserComboBox.SelectedIndex > 0)
                lineParserName = _parserComboBox.SelectedItem as string;

            id = MakeSafeForSerialization(id);
            documentTypeString = MakeSafeForSerialization(documentTypeString);
            displayName = MakeSafeForSerialization(displayName);
            toolAction = MakeSafeForSerialization(toolAction);
            toolPath = MakeSafeForSerialization(toolPath);
            toolArgs = MakeSafeForSerialization(toolArgs);
            userArgs = MakeSafeForSerialization(userArgs);
            lineParserName = MakeSafeForSerialization(lineParserName);

            bool validated = true;

            if (displayName == String.Empty)
            {
                validated = false;
                _displayNameTextBox.BackColor = Color.Yellow;
            }

            if (toolPath == String.Empty)
            {
                validated = false;
                _toolPathTextBox.BackColor = Color.Yellow;
            }

            if (!validated) return false;

            DocumentType documentType = new DocumentType(documentTypeString);
            BuildTool tool = new BuildTool(id, documentType, displayName);

            tool.Action = toolAction;
            tool.Path = toolPath;
            tool.Args = toolArgs;
            tool.UserArgs = userArgs;
            tool.BuildCommand =
                _buildToolManager.GetBuildCommand(documentType, toolAction);
            tool.LineParserName = lineParserName;
            tool.LineParser =
                _buildToolManager.GetLineParser(tool.LineParserName);

            _buildTools.AddTool(tool);

            return true;
        }

        #endregion

        #region Combo Boxes

        private void UpdateActionComboBox()
        {
            _actionComboBox.Items.Clear();

            DocumentType documentType = new DocumentType(
                _documentTypeComboBox.SelectedItem as string);

            List<String> actions =
                _buildToolManager.GetAvailableActions(documentType);

            foreach (string action in actions)
                _actionComboBox.Items.Add(action);

            if (actions.Count == 0)
                _actionComboBox.Enabled = false;
            else
                _actionComboBox.SelectedIndex = 0;
        }

        private void UpdateParserComboBox()
        {
            _parserComboBox.Items.Clear();
            _parserComboBox.Items.Add(Resources.ParserListNone);
            _parserComboBox.SelectedIndex = 0;

            DocumentType documentType = new DocumentType(
                _documentTypeComboBox.SelectedItem as string);

            string action = _actionComboBox.SelectedItem as string;

            List<String> parsers =
                _buildToolManager.GetLineParsers(documentType, action);

            foreach (string s in parsers)
                _parserComboBox.Items.Add(s);
        }

        #endregion

        #region Event Handlers

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (SaveTool())
                DialogResult = DialogResult.OK;
        }

        private void ToolPathHelperButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.RestoreDirectory = true;

            string currentTool = _toolPathTextBox.Text.Trim();

            if (File.Exists(currentTool))
                ofd.InitialDirectory = Path.GetDirectoryName(currentTool);

            ofd.Title = Resources.SelectFileDialogTitle;
            ofd.Filter = Resources.SelectFileDialogFilter;
            if (ofd.ShowDialog() == DialogResult.OK)
                _toolPathTextBox.Text = ofd.FileName;
        }

        private void ActionComboBox_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            UpdateParserComboBox();
        }

        private void DocumentTypeComboBox_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            UpdateActionComboBox();
            UpdateParserComboBox();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Display the form as read-only.
        /// </summary>
        public void DisableForm()
        {
            _displayNameTextBox.Enabled = false;
            _documentTypeComboBox.Enabled = false;
            _actionComboBox.Enabled = false;
            _parserComboBox.Enabled = false;
            _toolPathTextBox.Enabled = false;
            _toolPathHelperButton.Enabled = false;
            _toolArgsTextBox.Enabled = false;
            _userArgsTextBox.Enabled = false;
            _okButton.Enabled = false;
        }

        private bool DocumentTypeIsValid(string documentType)
        {
            Regex re = new Regex(@"(?mi)^\.\w+$");
            Match m = re.Match(documentType);

            return m.Success;
        }

        private string MakeSafeForSerialization(string text)
        {
            return text.Replace(
                Constants.SERIALIZATION_DELIMITER, String.Empty);
        }

        #endregion
    }
}
