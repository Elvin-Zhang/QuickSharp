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
using System.Net;
using System.Windows.Forms;

namespace QuickSharp.Core
{
    /// <summary>
    /// Provides the application 'Check for Updates' form.
    /// </summary>
    public partial class UpdateCheckForm : Form
    {
        private Uri _updateCheckUrl;
        private WebClient _webClient;
        ClientProfile _profile;

        /// <summary>
        /// Create an instance of the default 'Check for Updates' form.
        /// </summary>
        public UpdateCheckForm()
        {
            InitializeComponent();

            _profile = ApplicationManager.GetInstance().ClientProfile;
            _updateCheckUrl = new Uri(_profile.UpdateCheckURL);

            _webClient = new WebClient();

            Text = Resources.UpdateTitle;
            _checkButton.Text = Resources.UpdateButtonCheck;
            _cancelButton.Text = Resources.UpdateButtonCancel;
            _closeButton.Text = Resources.UpdateButtonClose;

            _latestVersionLabel.Text = Resources.UpdateMessageCheckNow;
            _currentVersionLabel.Text = String.Empty;
            _checkButton.Enabled = true;
            _cancelButton.Enabled = false;

            _linkLabel.Visible = false;

            if (_profile.UpdateHomeURL != null &&
                _profile.UpdateLinkText != null)
            {
                _linkLabel.Visible = true;
                _linkLabel.Text = _profile.UpdateLinkText;
                _linkLabel.LinkClicked += new
                    LinkLabelLinkClickedEventHandler(LinkLabel_LinkClicked);
            }

            // Allow client applications to modify the form.
            UpdateCheckFormProxy.GetInstance().UpdateFormControls(Controls);

            Refresh();
        }

        private void LinkLabel_LinkClicked(
            object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                FileTools.ShellOpenFile(_profile.UpdateHomeURL, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}:\r\n{1}",
                        Resources.UpdateCheckLinkErrorMessage,
                        ex.Message),
                    Resources.UpdateCheckLinkErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        #region Event Handlers

        private void CheckButton_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                _latestVersionLabel.Text = Resources.UpdateMessageChecking;
                _currentVersionLabel.Text = String.Empty;
                _checkButton.Enabled = false;
                _cancelButton.Enabled = true;

                Refresh();

                _webClient.DownloadStringCompleted +=
                    new DownloadStringCompletedEventHandler(Client_DownloadStringCompleted);

                _webClient.DownloadStringAsync(_updateCheckUrl);
            }
            catch
            {
                CheckFail(false);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            _webClient.CancelAsync();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            if (_cancelButton.Enabled)
                _webClient.CancelAsync();

            Close();
        }

        private void UpdateCheckForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (_cancelButton.Enabled)
                _webClient.CancelAsync();
        }

        private void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                    CheckFail(true);
                else
                    CheckSuccess(e.Result);
            }
            catch
            {
                CheckFail(false);
            }
        }

        #endregion

        private void CheckSuccess(string result)
        {
            _latestVersionLabel.Text = String.Format(
                Resources.UpdateMessageLatestVersion, result);

            _currentVersionLabel.Text = String.Format(
                Resources.UpdateMessageCurrentVersion, Application.ProductVersion);

            Reset();
        }

        private void CheckFail(bool cancelled)
        {
            if (cancelled)
                _latestVersionLabel.Text = Resources.UpdateMessageCheckCancelled;
            else
                _latestVersionLabel.Text = Resources.UpdateMessageCheckFailed;

            _currentVersionLabel.Text = String.Empty;

            Reset();
        }

        private void Reset()
        {
            _checkButton.Enabled = true;
            _cancelButton.Enabled = false;

            Refresh();
        }
    }
}
