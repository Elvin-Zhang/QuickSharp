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
    /// Abstract base theme provider.
    /// </summary>
    public abstract class BaseTheme 
    {
        protected ApplicationManager applicationManager;
        protected MainForm mainForm;

        // ColorTable
        protected ProfessionalColorTable ColorTable;

        // StatusBar
        protected Color StatusBarBackColor;
        protected Color StatusBarForeColor;

        // DockPanel
        protected Color ClientWindowBackColor;
        protected Color ClientWindowDockBackColor;
        protected Image ClientWindowBackgroundImage;
        protected ImageLayout ClientWindowBackgroundImageLayout;
        protected Color DocumentDockStripGradientStartColor;
        protected Color DocumentDockStripGradientEndColor;
        protected LinearGradientMode DocumentDockStripGradientMode;
        protected Color DocumentActiveTabGradientStartColor;
        protected Color DocumentActiveTabGradientEndColor;
        protected Color DocumentActiveTabGradientTextColor;
        protected LinearGradientMode DocumentActiveTabGradientMode;
        protected Color DocumentInactiveTabGradientStartColor;
        protected Color DocumentInactiveTabGradientEndColor;
        protected Color DocumentInactiveTabGradientTextColor;
        protected LinearGradientMode DocumentInactiveTabGradientMode;
        protected Color ToolWindowActiveCaptionGradientStartColor;
        protected Color ToolWindowActiveCaptionGradientEndColor;
        protected Color ToolWindowActiveCaptionGradientTextColor;
        protected LinearGradientMode ToolWindowActiveCaptionGradientMode;
        protected Color ToolWindowInactiveCaptionGradientStartColor;
        protected Color ToolWindowInactiveCaptionGradientEndColor;
        protected Color ToolWindowInactiveCaptionGradientTextColor;
        protected LinearGradientMode ToolWindowInactiveCaptionGradientMode;
        protected Color ToolWindowDockStripGradientStartColor;
        protected Color ToolWindowDockStripGradientEndColor;
        protected LinearGradientMode ToolWindowDockStripGradientMode;
        protected Color ToolWindowActiveTabGradientStartColor;
        protected Color ToolWindowActiveTabGradientEndColor;
        protected Color ToolWindowActiveTabGradientTextColor;
        protected LinearGradientMode ToolWindowActiveTabGradientMode;
        protected Color ToolWindowInactiveTabGradientStartColor;
        protected Color ToolWindowInactiveTabGradientEndColor;
        protected Color ToolWindowInactiveTabGradientTextColor;
        protected LinearGradientMode ToolWindowInactiveTabGradientMode;
        protected Color HideStripGradientStartColor;
        protected Color HideStripGradientEndColor;
        protected LinearGradientMode HideStripGradientMode;
        protected Color HideTabGradientStartColor;
        protected Color HideTabGradientEndColor;
        protected Color HideTabGradientTextColor;
        protected LinearGradientMode HideTabGradientMode;

        // DockingHelper
        protected Color DocumentActiveTabBorderColor;
        protected Color DocumentInactiveTabBorderColor;
        protected bool DocumentWindowHideBorder;
        protected Color ToolWindowTabBorderColor;
        protected Color DockStripMenuForeColor;
        protected bool DockStripUseLightImages;

        // ThemeFlags
        protected Color MainBackColor;
        protected Color ViewBackColor;
        protected Color ViewForeColor;
        protected Color ViewAltBackColor;
        protected Color ViewAltForeColor;
        protected bool ViewShowBorder = true;
        protected Color MenuForeColor;
        protected bool MenuHideImages;

        /// <summary>
        /// Initialize the theme base.
        /// </summary>
        public BaseTheme()
        {
            applicationManager = ApplicationManager.GetInstance();
            mainForm = applicationManager.MainForm;
        }

        /// <summary>
        /// Override the defaults to create a new theme.
        /// </summary>
        public virtual void UpdateTheme()
        {
        }

        /// <summary>
        /// Applies the theme to the relevant UI elements.
        /// </summary>
        public void ApplyTheme()
        {
            UpdateTheme();

            // ColorTable
            if (ColorTable != null)
                ToolStripManager.Renderer = new ToolStripProfessionalRenderer(ColorTable);

            // StatusBar
            if (StatusBarBackColor != Color.Empty)
                mainForm.StatusBar.BackColor = StatusBarBackColor;
            if (StatusBarForeColor != Color.Empty)
                mainForm.StatusBar.ForeColor = StatusBarForeColor;

            // DockPanel
            if (ClientWindowBackColor != Color.Empty)
                mainForm.ClientWindow.BackColor = ClientWindowBackColor;
            if (ClientWindowDockBackColor != Color.Empty)
                mainForm.ClientWindow.DockBackColor = ClientWindowDockBackColor;
            mainForm.ClientWindow.BackgroundImage = ClientWindowBackgroundImage;
            mainForm.ClientWindow.BackgroundImageLayout = ClientWindowBackgroundImageLayout;
            //mainForm.ClientWindow.ForeColor = don't assign this.

            DockPaneStripGradient docGradient = mainForm.ClientWindow.
                Skin.DockPaneStripSkin.DocumentGradient;

            if (DocumentDockStripGradientStartColor != Color.Empty)
                docGradient.DockStripGradient.StartColor = DocumentDockStripGradientStartColor;
            if (DocumentDockStripGradientEndColor != Color.Empty)
                docGradient.DockStripGradient.EndColor = DocumentDockStripGradientEndColor;
            docGradient.DockStripGradient.LinearGradientMode = DocumentDockStripGradientMode;

            if (DocumentActiveTabGradientStartColor != Color.Empty)
                docGradient.ActiveTabGradient.StartColor = DocumentActiveTabGradientStartColor;
            if (DocumentDockStripGradientEndColor != Color.Empty)
                docGradient.ActiveTabGradient.EndColor = DocumentActiveTabGradientEndColor;
            if (DocumentActiveTabGradientTextColor != Color.Empty)
                docGradient.ActiveTabGradient.TextColor = DocumentActiveTabGradientTextColor;
            docGradient.ActiveTabGradient.LinearGradientMode = DocumentActiveTabGradientMode;

            if (DocumentInactiveTabGradientStartColor != Color.Empty)
                docGradient.InactiveTabGradient.StartColor = DocumentInactiveTabGradientStartColor;
            if (DocumentInactiveTabGradientEndColor != Color.Empty)
                docGradient.InactiveTabGradient.EndColor = DocumentInactiveTabGradientEndColor;
            if (DocumentInactiveTabGradientTextColor != Color.Empty)
                docGradient.InactiveTabGradient.TextColor = DocumentInactiveTabGradientTextColor;
            docGradient.InactiveTabGradient.LinearGradientMode = DocumentInactiveTabGradientMode;

            DockPaneStripToolWindowGradient toolGradient = mainForm.ClientWindow.
                Skin.DockPaneStripSkin.ToolWindowGradient;

            if (ToolWindowActiveCaptionGradientStartColor != Color.Empty)
                toolGradient.ActiveCaptionGradient.StartColor = ToolWindowActiveCaptionGradientStartColor;
            if (ToolWindowActiveCaptionGradientEndColor != Color.Empty)
                toolGradient.ActiveCaptionGradient.EndColor = ToolWindowActiveCaptionGradientEndColor;
            if (ToolWindowActiveCaptionGradientTextColor != Color.Empty)
                toolGradient.ActiveCaptionGradient.TextColor = ToolWindowActiveCaptionGradientTextColor;
            toolGradient.ActiveCaptionGradient.LinearGradientMode = ToolWindowActiveCaptionGradientMode;

            if (ToolWindowInactiveCaptionGradientStartColor != Color.Empty)
                toolGradient.InactiveCaptionGradient.StartColor = ToolWindowInactiveCaptionGradientStartColor;
            if (ToolWindowInactiveCaptionGradientEndColor != Color.Empty)
                toolGradient.InactiveCaptionGradient.EndColor = ToolWindowInactiveCaptionGradientEndColor;
            if (ToolWindowInactiveCaptionGradientTextColor != Color.Empty)
                toolGradient.InactiveCaptionGradient.TextColor = ToolWindowInactiveCaptionGradientTextColor;
            toolGradient.InactiveCaptionGradient.LinearGradientMode = ToolWindowInactiveCaptionGradientMode;

            if (ToolWindowActiveTabGradientStartColor != Color.Empty)
                toolGradient.ActiveTabGradient.StartColor = ToolWindowActiveTabGradientStartColor;
            if (ToolWindowActiveTabGradientEndColor != Color.Empty)
                toolGradient.ActiveTabGradient.EndColor = ToolWindowActiveTabGradientEndColor;
            if (ToolWindowActiveTabGradientTextColor != Color.Empty)
                toolGradient.ActiveTabGradient.TextColor = ToolWindowActiveTabGradientTextColor;
            toolGradient.ActiveTabGradient.LinearGradientMode = ToolWindowActiveTabGradientMode;

            if (ToolWindowInactiveTabGradientStartColor != Color.Empty)
                toolGradient.InactiveTabGradient.StartColor = ToolWindowInactiveTabGradientStartColor;
            if (ToolWindowInactiveTabGradientEndColor != Color.Empty)
                toolGradient.InactiveTabGradient.EndColor = ToolWindowInactiveTabGradientEndColor;
            if (ToolWindowInactiveTabGradientTextColor != Color.Empty)
                toolGradient.InactiveTabGradient.TextColor = ToolWindowInactiveTabGradientTextColor;
            toolGradient.InactiveTabGradient.LinearGradientMode = ToolWindowInactiveTabGradientMode;

            if (ToolWindowDockStripGradientStartColor != Color.Empty)
                toolGradient.DockStripGradient.StartColor = ToolWindowDockStripGradientStartColor;
            if (ToolWindowDockStripGradientEndColor != Color.Empty)
                toolGradient.DockStripGradient.EndColor = ToolWindowDockStripGradientEndColor;
            toolGradient.DockStripGradient.LinearGradientMode = ToolWindowDockStripGradientMode;

            DockPanelGradient hideStrip = mainForm.ClientWindow.
                Skin.AutoHideStripSkin.DockStripGradient;

            if (HideStripGradientStartColor != Color.Empty)
                hideStrip.StartColor = HideStripGradientStartColor;
            if (HideStripGradientEndColor != Color.Empty)
                hideStrip.EndColor = HideStripGradientEndColor;
            hideStrip.LinearGradientMode = HideStripGradientMode;

            TabGradient hideTab = mainForm.ClientWindow.
                Skin.AutoHideStripSkin.TabGradient;

            if (HideTabGradientStartColor != Color.Empty)
                hideTab.StartColor = HideTabGradientStartColor;
            if (HideTabGradientEndColor != Color.Empty)
                hideTab.EndColor = HideTabGradientEndColor;
            if (HideTabGradientTextColor != Color.Empty)
                hideTab.TextColor = HideTabGradientTextColor;
            hideTab.LinearGradientMode = HideTabGradientMode;

            // DockingHelper
            if (DocumentActiveTabBorderColor != Color.Empty)
                DockingHelper.ThemeDocumentTabActiveBorder = DocumentActiveTabBorderColor;
            if (DocumentInactiveTabBorderColor != Color.Empty)
                DockingHelper.ThemeDocumentTabInactiveBorder = DocumentInactiveTabBorderColor;
            if (ToolWindowTabBorderColor != Color.Empty)
                DockingHelper.ThemeToolWindowTabBorder = ToolWindowTabBorderColor;
            DockingHelper.HideDocumentWindowBorder = DocumentWindowHideBorder;
            DockingHelper.UseLightDockStripImages = DockStripUseLightImages;

            // ThemeFlags
            ThemeFlags flags = new ThemeFlags();
            if (MainBackColor != Color.Empty)
                flags.MainBackColor = MainBackColor;
            if (MenuForeColor != Color.Empty)
                flags.MenuForeColor = MenuForeColor;
            if (ViewBackColor != Color.Empty)
                flags.ViewBackColor = ViewBackColor;
            if (ViewForeColor != Color.Empty)
                flags.ViewForeColor = ViewForeColor;
            if (ViewAltBackColor != Color.Empty)
                flags.ViewAltBackColor = ViewAltBackColor;
            if (ViewAltForeColor != Color.Empty)
                flags.ViewAltForeColor = ViewAltForeColor;            
            flags.ViewShowBorder = ViewShowBorder;
            flags.MenuHideImages = MenuHideImages;
            applicationManager.ClientProfile.ThemeFlags = flags;
        }
    }
}
