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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.TextEditor
{
    partial class EditForm
    {
        private IContainer _components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
                _components.Dispose();

            base.Dispose(disposing);
        }

        #region Form Layout

        private void InitializeComponent()
        {
            SuspendLayout();

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(443, 302);
            Controls.Add(scintilla);
            DockAreas = DockAreas.Document;
            Font = new Font("Microsoft Sans Serif", 8.25F,
                FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            Icon = Resources.DocumentIcon;
            Name = "EditForm";
            ShowInTaskbar = false;
            Text = "EditForm";

            ResumeLayout(false);
        }

        #endregion
    }
}