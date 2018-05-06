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
using System.Windows.Forms;
using QuickSharp.Core;

namespace QuickSharp.Core
{
    /// <summary>
    /// Provides a means to access the controls of the Options form.
    /// </summary>
    public class OptionsFormProxy
    {
        #region Singleton

        private static OptionsFormProxy _singleton;

        /// <summary>
        /// Get a reference to the form proxy singleton.
        /// </summary>
        /// <returns>A reference to the form proxy.</returns>
        public static OptionsFormProxy GetInstance()
        {
            if (_singleton == null)
                _singleton = new OptionsFormProxy();

            return _singleton;
        }

        #endregion

        private OptionsFormProxy()
        {
        }

        /// <summary>
        /// Attach handlers to this event to gain access to the form's page
        /// collection each time it is invoked. The handlers can modify the
        /// pages to create a customized form.
        /// </summary>
        public event OptionsFormUpdateHandler OptionsFormUpdatePages;

        /// <summary>
        /// Attach handlers to this event to gain access to the form's control
        /// collection each time it is invoked. The handlers can modify the
        /// controls to create a customized form.
        /// </summary>
        public event FormControlUpdateHandler OptionsFormUpdateControls;

        /// <summary>
        /// Raise the OptionsFormUpdatePages event. Called by the form to allow its
        /// pages to be customized.
        /// </summary>
        /// <param name="pages">The option pages included in the form.</param>
        public void UpdateOptionsFormPages(Dictionary<String, OptionsPage> pages)
        {
            if (OptionsFormUpdatePages != null)
                OptionsFormUpdatePages(pages);
        }

        /// <summary>
        /// Raise the OptionsFormUpdateControls event. Called by the form to allow its
        /// controls to be customized.
        /// </summary>
        /// <param name="controls">The form's control collection.</param>
        public void UpdateOptionsFormControls(Control.ControlCollection controls)
        {
            if (OptionsFormUpdateControls != null)
                OptionsFormUpdateControls(controls);
        }
    }
}
