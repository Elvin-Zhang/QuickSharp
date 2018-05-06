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
using System.Windows.Forms;

namespace QuickSharp.Core
{
    /// <summary>
    /// Provides an application-wide error reporting form.
    /// </summary>
    public partial class ErrorForm : Form
    {
        /// <summary>
        /// Create an instance of the form to display the details of an exception.
        /// </summary>
        /// <param name="ex">The exception to display in the form.</param>
        /// <param name="allowContinue">Allow the application to continue once
        /// the error form has been closed. Set to false to force the application
        /// to exit.</param>
        public ErrorForm(Exception ex, bool allowContinue)
        {
            InitializeComponent();

            if (!allowContinue)
                _btnContinue.Enabled = false;

            _errorMessageLabel.Text = ex.Message;
            _stackTraceTextBox.Text = ex.StackTrace;

            // Allow client applications to modify the form.
            ErrorFormProxy.GetInstance().UpdateFormControls(Controls);
        }
    }
}
