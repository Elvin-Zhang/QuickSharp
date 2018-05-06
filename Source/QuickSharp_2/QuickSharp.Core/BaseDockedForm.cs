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
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.Core
{
    /// <summary>
    /// Provides a base class for docked forms.
    /// </summary>
    public abstract class BaseDockedForm : DockContent
    {
        /// <summary>
        /// The form's unique ID. Docked forms must each have a unique key to allow
        /// them to be registered with the application manager and handled by the
        /// docking window manager.
        /// </summary>
        public string FormKey { get; private set; }

        protected MainForm mainForm;

        /// <summary>
        /// Registers the form with the application manager and
        /// provides default settings for the form.
        /// </summary>
        /// <param name="formKey">The docked form ID.</param>
        public BaseDockedForm(string formKey)
        {
            FormKey = formKey;

            mainForm = ApplicationManager.GetInstance().MainForm;

            /*
             * When the form is serialized to disk the
             * key is used to associate the saved settings
             * with this form.
             */

            DockHandler.GetPersistStringCallback =
                delegate { return FormKey; };

            /*
             * These settings are required for correct behaviour
             * of the docking windows. Do not override.
             */

            HideOnClose = true;
            ShowInTaskbar = false;

            /*
             * The default dock positions of the form.
             * Can be overridden to limit the positions.
             */

            DockAreas = (DockAreas)(
                DockAreas.Float |
                DockAreas.DockLeft |
                DockAreas.DockRight |
                DockAreas.DockTop |
                DockAreas.DockBottom);

            /*
             * If a form is not restored from the saved config
             * Initialize allows the default values to be set.
             * Override SetFormDefaultState to customize the
             * form's initial state.
             */

            mainForm.DockPanelPostLoad +=
                new MessageHandler(Initialize);
        }

        private void Initialize()
        {
            if (mainForm.HaveDockedFormContent(FormKey))
                return;

            DockPanel = mainForm.ClientWindow;

            SetFormDefaultState();
        }

        /// <summary>
        /// Sets the default state of the form for the first time it is created.
        /// Subsequent instances will retrieve the settings from the saved docking
        /// window configuration managed by the application. This should be
        /// overridden in derived classes to customize the default state.
        /// </summary>
        protected virtual void SetFormDefaultState()
        {
            DockState = DockState.DockLeft;
            Show();
        }
    }
}
