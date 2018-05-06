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
using QuickSharp.Core;

namespace QuickSharp.Core
{
    /// <summary>
    /// Provides a means to access the controls of a form for customization.
    /// </summary>
    public abstract class BaseFormProxy
    {
        /*
         * Plugins can create singleton versions of this class to provide an
         * access point to forms managed by the plugin. It provides a means
         * by which client applications can modify the form's controls when
         * it is created.
         */

        /// <summary>
        /// Attach handlers to this event to gain access to the form's control
        /// collection each time it is invoked. The handlers can modify the
        /// controls to create customized forms.
        /// </summary>
        public event FormControlUpdateHandler FormControlUpdate;

        /// <summary>
        /// Raise the FormControlUpdate event. Called by the form to allow its
        /// controls to be customized.
        /// </summary>
        /// <param name="controls">The form's control collection.</param>
        public void UpdateFormControls(
            Control.ControlCollection controls)
        {
            if (FormControlUpdate != null)
                FormControlUpdate(controls);
        }
    }
}
