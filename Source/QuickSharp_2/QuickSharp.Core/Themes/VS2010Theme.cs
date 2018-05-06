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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using QuickSharp.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.Core
{
    /// <summary>
    /// The Visual Studio 2010 abstract base theme provider.
    /// </summary>
    public abstract class VS2010Theme 
    {
        /// <summary>
        /// Applies the theme to the relevant UI elements.
        /// </summary>
        /// <param name="mainForm">The application main form.</param>
        public void ApplyTheme()
        {
            ApplicationManager applicationManager =
                ApplicationManager.GetInstance();

            ToolStripManager.Renderer =
                new ToolStripProfessionalRenderer(
                    new VS2010ColorTable()) { RoundedEdges = false };

            MainForm mainForm = applicationManager.MainForm;
            mainForm.StatusBar.BackColor = Color.FromArgb(0x29, 0x39, 0x55);
            mainForm.StatusBar.ForeColor = Color.White;

            #region DockPanel

            DockingHelper.ThemeDocumentTabActiveBorder = Color.FromArgb(0x29, 0x39, 0x55);
            DockingHelper.ThemeDocumentTabInactiveBorder = Color.FromArgb(0x29, 0x39, 0x55);
            DockingHelper.ThemeToolWindowTabBorder = Color.FromArgb(0x29, 0x39, 0x55);
            DockingHelper.HideDocumentWindowBorder = true;
            DockingHelper.UseLightDockStripImages = true;

            mainForm.ClientWindow.BackColor = Color.FromArgb(0x29, 0x39, 0x55);
            
            // Don't do this!
            //mainForm.ClientWindow.ForeColor = Color.White;

            /*
             * This caused a really bizarre bug in that the list views in the app would
             * become corrupted whenever Reload Code Assist was run. But only for release
             * builds. The corruption would persist until the next session at which point
             * all would be normal until the code assist load was run again. This affected
             * the Workspace and Output windows.
             */

            mainForm.ClientWindow.DockBackColor = Color.FromArgb(0x29, 0x39, 0x55);
            mainForm.ClientWindow.BackgroundImage = Resources.VS2010ThemeBackground;
            mainForm.ClientWindow.BackgroundImageLayout = ImageLayout.Tile;

            DockPaneStripGradient docGradient = mainForm.ClientWindow.
                Skin.DockPaneStripSkin.DocumentGradient;

            docGradient.DockStripGradient.StartColor = Color.FromArgb(0x29, 0x39, 0x55);
            docGradient.DockStripGradient.EndColor = Color.FromArgb(0x29, 0x39, 0x55);
            docGradient.ActiveTabGradient.StartColor = Color.White;
            docGradient.ActiveTabGradient.EndColor = Color.FromArgb(0xFF, 0xE7, 0xA5);
            docGradient.ActiveTabGradient.LinearGradientMode = LinearGradientMode.Vertical;
            docGradient.InactiveTabGradient.StartColor = Color.FromArgb(0x29, 0x39, 0x55);
            docGradient.InactiveTabGradient.EndColor = Color.FromArgb(0x29, 0x39, 0x55);
            docGradient.InactiveTabGradient.TextColor = Color.White;

            DockPaneStripToolWindowGradient toolGradient = mainForm.ClientWindow.
                Skin.DockPaneStripSkin.ToolWindowGradient;

            toolGradient.ActiveCaptionGradient.StartColor = Color.White;
            toolGradient.ActiveCaptionGradient.EndColor = Color.FromArgb(0xFF, 0xE7, 0xA5);
            toolGradient.ActiveCaptionGradient.TextColor = Color.Black;
            toolGradient.ActiveCaptionGradient.LinearGradientMode = LinearGradientMode.Vertical;
            toolGradient.InactiveCaptionGradient.StartColor = Color.FromArgb(0x4B, 0x5E, 0x81);
            toolGradient.InactiveCaptionGradient.EndColor = Color.FromArgb(0x3E, 0x53, 0x78);
            toolGradient.InactiveCaptionGradient.TextColor = Color.White;
            toolGradient.InactiveCaptionGradient.LinearGradientMode = LinearGradientMode.Vertical;
            toolGradient.DockStripGradient.StartColor = Color.FromArgb(0x29, 0x39, 0x55);
            toolGradient.DockStripGradient.EndColor = Color.FromArgb(0x29, 0x39, 0x55);
            toolGradient.InactiveTabGradient.TextColor = Color.White;

            DockPanelGradient hideStrip = mainForm.ClientWindow.
                Skin.AutoHideStripSkin.DockStripGradient;

            hideStrip.StartColor = Color.FromArgb(0x29, 0x39, 0x55);
            hideStrip.EndColor = Color.FromArgb(0x29, 0x39, 0x55);

            TabGradient hideTab = mainForm.ClientWindow.
                Skin.AutoHideStripSkin.TabGradient;

            hideTab.StartColor = Color.FromArgb(0x3D, 0x52, 0x77);
            hideTab.EndColor = Color.FromArgb(0x3D, 0x52, 0x77);
            hideTab.TextColor = Color.White;

            #endregion

            applicationManager.ClientProfile.ThemeFlags = new ThemeFlags()
            {
                MainBackColor = Color.FromArgb(0x29, 0x39, 0x55),
                ViewShowBorder = false
            };

            UpdateTheme();
        }

        protected virtual void UpdateTheme()
        {
        }
    }
}
