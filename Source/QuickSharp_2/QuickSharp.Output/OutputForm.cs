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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using QuickSharp.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.Output
{
    /// <summary>
    /// The output window form.
    /// </summary>
    public partial class OutputForm : BaseDockedForm
    {
        /// <summary>
        /// Create the output window form.
        /// </summary>
        /// <param name="formKey">The GUID key used to identify the form internally.</param>
        public OutputForm(string formKey) : base(formKey)
        {
            InitializeComponent();

            _listViewListView.Dock = DockStyle.Fill;
            _listViewListView.Visible = true;
            _listViewToolStripButton.Checked = true;

            _textViewTextBox.Dock = DockStyle.Fill;
            _textViewTextBox.Visible = false;
            _textViewToolStripButton.Checked = false;

            CheckForIllegalCrossThreadCalls = false;

            mainForm.ThemeActivated +=
                new MessageHandler(MainForm_ThemeActivated);

            /*
             * Clear the views when first shown (but not if we
             * already have some output).
             */

            Load += delegate
            {
                if (_listViewListView.Items.Count == 0)
                    ClearOutputViews();
            };
        }

        private void MainForm_ThemeActivated()
        {
            ApplicationManager applicationManager = 
                ApplicationManager.GetInstance();

            ThemeFlags flags = applicationManager.
                ClientProfile.ThemeFlags;

            if (flags != null)
            {
                if (flags.MainBackColor != Color.Empty)
                    BackColor = flags.MainBackColor;

                if (flags.ViewBackColor != Color.Empty)
                {
                    ListView.BackColor = flags.ViewBackColor;
                    TextView.BackColor = flags.ViewBackColor;
                }

                if (flags.ViewForeColor != Color.Empty)
                {
                    ListView.ForeColor = flags.ViewForeColor;
                    TextView.ForeColor = flags.ViewForeColor;
                }

                if (flags.ViewShowBorder == false)
                {
                    _listViewListView.BorderStyle = BorderStyle.None;
                    _textViewTextBox.BorderStyle = BorderStyle.None;
                }
            }

            SetBackgroundImage();
        }

        /// <summary>
        /// Set the form's initial state when there is no saved configuration
        /// to be restored from the previous session.
        /// </summary>
        protected override void SetFormDefaultState()
        {
            DockState = DockState.DockBottom;
            DockPanel.UpdateDockWindowZOrder(DockStyle.Bottom, false);

            ClientProfile profile = ApplicationManager.
                GetInstance().ClientProfile;

            if (profile.HaveFlag(ClientFlags.OutputHideByDefault))
                Hide();
            else
                Show();
        }

        #region Public UI Elements

        /// <summary>
        /// The output window toolbar.
        /// </summary>
        public ToolStrip Toolbar
        {
            get { return _mainToolStrip; }
        }

        /// <summary>
        /// The list view.
        /// </summary>
        public ListView ListView
        {
            get { return _listViewListView; }
        }

        /// <summary>
        /// The image list used to provide images to the list view.
        /// </summary>
        public ImageList ListViewImageList
        {
            get { return _listViewImageList; }
        }

        /// <summary>
        /// The text view.
        /// </summary>
        public TextBox TextView
        {
            get { return _textViewTextBox; }
        }

        #endregion

        #region Toolbar

        /// <summary>
        /// Disable the output views.
        /// </summary>
        public void DisableOutputViews()
        {
            foreach (ToolStripItem item in _mainToolStrip.Items)
                if (item is ToolStripButton)
                    ((ToolStripButton)item).Checked = false;

            foreach (Control item in Controls)
                if (item != _mainToolStrip)
                    item.Visible = false;
        }

        /// <summary>
        /// Make the list view the active view.
        /// </summary>
        public void SelectListView()
        {
            DisableOutputViews();

            _listViewListView.Visible = true;
            _listViewToolStripButton.Checked = true;
        }

        /// <summary>
        /// Make the text view the active view.
        /// </summary>
        public void SelectTextView()
        {
            DisableOutputViews();

            _textViewTextBox.Visible = true;
            _textViewToolStripButton.Checked = true;
        }

        private void listViewToolStripButton_Click(object sender, EventArgs e)
        {
            SelectListView();
        }

        private void textViewToolStripButton_Click(object sender, EventArgs e)
        {
            SelectTextView();
        }

        private void clearViewToolStripButton_Click(object sender, EventArgs e)
        {
            ClearOutputViews();
            Text = Resources.OutputWindowTitle;
        }

        #endregion

        #region Run Process

        private ProcessStartInfo CreateProcessStartInfo(
            RunProcessContext context, bool runInternal)
        {
            ProcessStartInfo pi = new ProcessStartInfo();
            pi.FileName = String.Format("\"{0}\"", context.ExePath);
            pi.Arguments = context.ProcessArgs;
            pi.UseShellExecute = false;

            if (runInternal)
            {
                pi.CreateNoWindow = true;
                pi.RedirectStandardOutput = true;
                pi.RedirectStandardError = true;
            }
            else
            {
                pi.CreateNoWindow = false;
                pi.RedirectStandardOutput = false;
                pi.RedirectStandardError = false;
            }

            return pi;
        }

        /// <summary>
        /// Run a new process in a separate window.
        /// </summary>
        /// <param name="context">The process context information.</param>
        /// <returns>True if the process exits without error.</returns>
        public bool RunProcessExternal(RunProcessContext context)
        {
            ProcessStartInfo pi = CreateProcessStartInfo(context, false);
            bool result = false;

            _listViewListView.Columns[0].Width =
                _listViewListView.Width - 25;

            // Header
            string header = String.Format("------ {0}{1} {2}",
                context.HeaderText,
                context.ExePath,
                context.ProcessArgs);

            AddLineToOutputView(header);

            // Run process
            try
            {
                Process p = Process.Start(pi);
                while (!p.HasExited)
                {
                    Thread.Sleep(10);
                    mainForm.Update();
                }

                context.ExitCode = p.ExitCode;
                result = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}\r\n{1}",
                        Resources.RunProcessErrorMessage,
                        ex.Message),
                    Resources.RunProcessErrorTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Footer
            string footer = String.Format("------ {0}{1} {2}",
                context.FooterText,
                Resources.RunProcessReturn,
                context.ExitCode);

            AddLineToOutputView(footer);

            AdjustOutputWidth();
            
            MoveToEndOfOutput();

            return result;
        }

        /// <summary>
        /// Run the process and present the standard and error output
        /// streams in the output window views.
        /// </summary>
        /// <param name="context">The process context information.</param>
        /// <returns>True if the process exits without error.</returns>
        public bool RunProcessInternal(RunProcessContext context)
        {
            bool result = true;
            context.Output = this;

            try
            {
                ProcessStartInfo pi = CreateProcessStartInfo(context, true);

                mainForm.Cursor = Cursors.WaitCursor;

                _listViewListView.Columns[0].Width =
                    _listViewListView.Width - 25;

                // Header
                string header = String.Format("------ {0}{1} {2}",
                    context.HeaderText,
                    context.ExePath,
                    context.ProcessArgs);

                AddLineToOutputView(header);

                // Run process
                try
                {
                    Process p = Process.Start(pi);
                    p.OutputDataReceived += 
                        new DataReceivedEventHandler(
                            context.OutputDataReceived);
                    p.ErrorDataReceived +=
                        new DataReceivedEventHandler(
                            context.ErrorDataReceived);

                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();

                    while (!p.HasExited)
                    {
                        Thread.Sleep(10);
                        mainForm.Update();
                    }
                    
                    p.WaitForExit();

                    context.ExitCode = p.ExitCode;
                }
                catch (Exception ex)
                {
                    AddLineToOutputView(ex.Message);
                    result = false;
                }

                // Footer
                string footer = String.Format("------ {0}{1} {2}",
                    context.FooterText,
                    Resources.RunProcessReturn,
                    context.ExitCode);

                AddLineToOutputView(footer);

                MoveToEndOfOutput();
                
                AdjustOutputWidth();
            }
            finally
            {
                mainForm.Cursor = Cursors.Default;
            }

            return result;
        }

        /// <summary>
        /// Run a program in the output window.
        /// </summary>
        /// <param name="exe">The path of the program to run.</param>
        /// <param name="args">Arguments to be passed to the program.</param>
        /// <returns>True if the program returns 0 on exit.</returns>
        public bool RunProgram(string exe, string args)
        {
            return RunProgram(exe, args, true);
        }

        /// <summary>
        /// Run a program in the output window.
        /// </summary>
        /// <param name="exe">The path of the program to run.</param>
        /// <param name="args">Arguments to be passed to the program.</param>
        /// <param name="clearTitle">Clear the output window title bar if true.</param>
        /// <returns>True if the program returns 0 on exit.</returns>
        public bool RunProgram(string exe, string args, bool clearTitle)
        {
            /*
             * Reset the window title bar.
             */

            if (clearTitle) Text = Resources.OutputWindowTitle;

            /*
             * Run the process.
             */

            RunProcessContext context = new RunProcessContext();
            context.ExePath = exe;
            context.ProcessArgs = args;
            context.HeaderText = 
                String.Format("{0}: ", Resources.RunProgramCaption);
            
            RunProcessInternal(context);
            return (context.ExitCode == 0);
        }

        /// <summary>
        /// Run a shell command and redirect standard output and error
        /// streams to the output views.
        /// </summary>
        /// <param name="caption">Text to display before the output of the command.</param>
        /// <param name="cmd">The command to run.</param>
        /// <returns>True if the command returns 0 on exit.</returns>
        public bool RunShellCommandInternal(string caption, string cmd)
        {
            return RunShellCommandInternal(caption, cmd, true);
        }

        /// <summary>
        /// Run a shell command and redirect standard output and error
        /// streams to the output views.
        /// </summary>
        /// <param name="caption">Text to display before the output of the command.</param>
        /// <param name="cmd">The command to run.</param>
        /// <param name="clearTitle">Clear the output window title bar if true.</param>
        /// <returns>True if the command returns 0 on exit.</returns>
        public bool RunShellCommandInternal(
            string caption, string cmd, bool clearTitle)
        {
            /*
             * Reset the window title bar.
             */

            if (clearTitle) Text = Resources.OutputWindowTitle;
            
            /*
             * Do some simple macro substitutions.
             */
                
            ApplicationManager applicationManager =
                ApplicationManager.GetInstance();
            
            string qshome = applicationManager.QuickSharpHome;
            string qsuser = applicationManager.QuickSharpUserHome;
            string system = Environment.SystemDirectory;
            string pfiles = Environment.GetFolderPath(
                Environment.SpecialFolder.ProgramFiles);
            string mydocs = Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments);

            cmd = cmd.Replace("${IDE_HOME}", qshome);
            cmd = cmd.Replace("${USR_HOME}", qsuser);
            cmd = cmd.Replace("${USR_DOCS}", mydocs);
            cmd = cmd.Replace("${SYSTEM}", system);
            cmd = cmd.Replace("${PFILES}", pfiles);
            
            /*
             * Run the command.
             */

            RunProcessContext context = new RunProcessContext();

            String comspec = Environment.GetEnvironmentVariable(
                Constants.WINDOWS_COMSPEC);

            if (String.IsNullOrEmpty(comspec))
            {
                comspec = Path.Combine(
                    Environment.SystemDirectory,
                    Constants.WINDOWS_SHELL_PROCESSOR);
            }

            context.ExePath = comspec;
            context.ProcessArgs = "/D /C " + cmd;
            context.HeaderText = caption;
            RunProcessInternal(context);
            return (context.ExitCode == 0);
        }

        #endregion

        #region Output Views

        /// <summary>
        /// Clear the output window views.
        /// </summary>
        public void ClearOutputViews()
        {
            _listViewListView.Items.Clear();
            _listViewListView.Refresh();
            _textViewTextBox.Text = String.Empty;
            _textViewTextBox.Refresh();

            _listViewListView.Columns[0].Width =
                _listViewListView.Width - 25;
        }

        /// <summary>
        /// Add a line to the output window views.
        /// </summary>
        /// <param name="line">The text to add.</param>
        public void AddLineToOutputView(string line)
        {
            AddLineToListView(line);
            AddLineToTextView(line);
        }

        /// <summary>
        /// Add a line to the output window views.
        /// </summary>
        /// <param name="line">The text to add.</param>
        /// <param name="foreColor">The colour used to display the text.</param>
        public void AddLineToOutputView(string line, Color foreColor)
        {
            AddLineToListView(line, null, foreColor);
            AddLineToTextView(line);
        }

        /// <summary>
        /// Add a line to the output window views and parse it with the
        /// output line parser provided.
        /// </summary>
        /// <param name="line">The text to add.</param>
        /// <param name="parser">A line parser.</param>
        public void AddLineToOutputView(string line, IOutputLineParser parser)
        {
            AddLineToListView(line, parser);
            AddLineToTextView(line);
        }

        /// <summary>
        /// Add a line to the text view of the output windows.
        /// </summary>
        /// <param name="line">The text to add.</param>
        public void AddLineToTextView(string line)
        {
            string[] split = line.Split('\n');

            foreach (string s in split)
                _textViewTextBox.AppendText(CleanUpLine(s) + "\r\n");
        }

        /// <summary>
        /// Add a line to the list view of the output window.
        /// </summary>
        /// <param name="line">The text to add.</param>
        public void AddLineToListView(string line)
        {
            AddLineToListView(line, null);
        }

        /// <summary>
        /// Add a line to the list view of the output window and parse it with the
        /// output line parser provided.
        /// </summary>
        /// <param name="line">The text to add.</param>
        /// <param name="parser">A line parser.</param>
        public void AddLineToListView(
            string line, IOutputLineParser parser)
        {
            AddLineToListView(line, parser, _listViewListView.ForeColor);
        }

        /// <summary>
        /// Add a line to the list view of the output window and parse it with the
        /// output line parser provided.
        /// </summary>
        /// <param name="line">The text to add.</param>
        /// <param name="parser">A line parser.</param>
        /// <param name="foreColor">The colour used to display the text.</param>
        public void AddLineToListView(
            string line, IOutputLineParser parser, Color foreColor)
        {
            line = CleanUpLine(line);
            if (String.IsNullOrEmpty(line)) return;

            ListViewItem lvi = new ListViewItem(line);
            lvi.ForeColor = foreColor;
            ParsedLine parsedLine = null;

            if (parser == null)
                lvi.Tag = null;
            else
            {
                parsedLine = parser.Parse(line);
                lvi.Tag = parsedLine;
            }

            if (parsedLine != null && parsedLine.IsError)
                lvi.ImageKey = Constants.ERROR_IMAGE;

            _listViewListView.Items.Add(lvi);
        }

        /// <summary>
        /// Expand the width of the list view to fit the maximum length of text.
        /// </summary>
        public void AdjustOutputWidth()
        {
            // Set the new window width
            int width1 = _listViewListView.Columns[0].Width;
            _listViewListView.Columns[0].Width = -1;
            int width2 = _listViewListView.Columns[0].Width;
            if (width1 > width2)
                _listViewListView.Columns[0].Width = width1;

            // Make sure the last line is visible
            int lineCount = _listViewListView.Items.Count;
            if (lineCount > 0)
                _listViewListView.Items[lineCount - 1].EnsureVisible();
        }

        /*
         * We want to see the end of the output visible in the output
         * window when the processes are complete.
         */

        private void MoveToEndOfOutput()
        {
            // Make sure last list item is visible
            ListViewItem item = _listViewListView.Items[_listViewListView.Items.Count - 1];
            item.Selected = true;
            item.Focused = true;

            // Move to end of text view (only works if text box is visible)
            _textViewTextBox.SelectionStart = _textViewTextBox.Text.Length - 1;
            _textViewTextBox.ScrollToCaret();
        }

        private string CleanUpLine(string line)
        {
            if (String.IsNullOrEmpty(line)) return line;
            string s = line.Replace("\n", String.Empty);
            s = s.Replace("\r", String.Empty);
            s = s.Replace("\t", "    ");
            return s;
        }

        #endregion

        #region List View Events

        /*
         * If the user clicks on a parsed line in the output list
         * view, open the relevant document at the indicated line.
         */

        private void ListViewListView_MouseDoubleClick(
            object sender, MouseEventArgs e)
        {
            if (_listViewListView.SelectedItems.Count == 0) return;

            ListViewItem lvi = _listViewListView.SelectedItems[0];
            if (lvi == null) return;
            if (lvi.Tag == null) return;

            ParsedLine line = lvi.Tag as ParsedLine;
            if (line != null)
            {
                try
                {
                    mainForm.LoadDocumentIntoWindow(line.FilePath, true);

                    if (mainForm.ActiveDocument != null)
                        mainForm.ActiveDocument.SetLocation(0, line.Line);
                }
                catch
                {
                    // Fail silently if file fails to open.
                    // Lack of response in UI sufficient feedback.
                }
            }
        }

        #endregion

        #region Text View Events

        private void TextViewTextBox_KeyDown(
            object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                _textViewTextBox.SelectAll();
        }

        #endregion

        #region Background Image

        private void SetBackgroundImage()
        {
            ApplicationManager applicationManager = 
                ApplicationManager.GetInstance();

            string imagePath = Path.Combine(
                applicationManager.QuickSharpUserHome,
                Constants.BACKGROUND_IMAGE_FOLDER);

            if (!Directory.Exists(imagePath)) return;

            /*
             * Try to find a theme specific folder, if not
             * use the default.
             */

            string themeName = applicationManager.
                GetSelectedThemeProviderKey();

            if (!String.IsNullOrEmpty(themeName))
            {
                string themePath =
                    Path.Combine(imagePath, themeName);

                if (Directory.Exists(themePath))
                    imagePath = themePath;
            }

            /*
             * Look for files matching the filename pattern.
             */

            string[] files = Directory.GetFiles(
                imagePath, Constants.BACKGROUND_IMAGE_PATTERN);

            if (files.Length == 0) return;

            /*
             * Use the first matching filename.
             */

            string filepath = files[0].ToLower();

            /*
             * If name contains 'dark' or 'tile' set the appropriate flags.
             */

            string filename = Path.GetFileName(filepath);

            bool darkBackground =
                (filename.IndexOf(Constants.BACKGROUND_IMAGE_DARK) != -1);

            bool tiledBackground =
                (filename.IndexOf(Constants.BACKGROUND_IMAGE_TILE) != -1);

            /*
             * Try to show the image.
             */

            try
            {
                Bitmap img = new Bitmap(filepath);
                _listViewListView.BackgroundImage = img;
                _listViewListView.BackgroundImageTiled = tiledBackground;

                // Only do this if the image is set successfully.
                if (darkBackground)
                {
                    _listViewListView.ForeColor = Color.White;
                    _listViewListView.BorderStyle = BorderStyle.None;
                }
            }
            catch
            {
                // Do nothing
            }            
        }

        #endregion
    }
}
