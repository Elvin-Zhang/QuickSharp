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

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace QuickSharp.Core
{
    /// <summary>
    /// Base class for theme color tables.
    /// </summary>
    public abstract class BaseColorTable : ProfessionalColorTable
    {
        #region Colors

        protected Color buttonCheckedGradientBegin;
        protected Color buttonCheckedGradientEnd;
        protected Color buttonCheckedGradientMiddle;
        protected Color buttonCheckedHighlight;
        protected Color buttonCheckedHighlightBorder;
        protected Color buttonPressedBorder;
        protected Color buttonPressedGradientBegin;
        protected Color buttonPressedGradientEnd;
        protected Color buttonPressedGradientMiddle;
        protected Color buttonPressedHighlight;
        protected Color buttonPressedHighlightBorder;
        protected Color buttonSelectedBorder;
        protected Color buttonSelectedGradientBegin;
        protected Color buttonSelectedGradientEnd;
        protected Color buttonSelectedGradientMiddle;
        protected Color buttonSelectedHighlight;
        protected Color buttonSelectedHighlightBorder;
        protected Color checkBackground;
        protected Color checkPressedBackground;
        protected Color checkSelectedBackground;
        protected Color gripDark;
        protected Color gripLight;
        protected Color imageMarginGradientBegin;
        protected Color imageMarginGradientEnd;
        protected Color imageMarginGradientMiddle;
        protected Color imageMarginRevealedGradientBegin;
        protected Color imageMarginRevealedGradientEnd;
        protected Color imageMarginRevealedGradientMiddle;
        protected Color menuBorder;
        protected Color menuItemBorder;
        protected Color menuItemPressedGradientBegin;
        protected Color menuItemPressedGradientEnd;
        protected Color menuItemPressedGradientMiddle;
        protected Color menuItemSelected;
        protected Color menuItemSelectedGradientBegin;
        protected Color menuItemSelectedGradientEnd;
        protected Color menuStripGradientBegin;
        protected Color menuStripGradientEnd;
        protected Color overflowButtonGradientBegin;
        protected Color overflowButtonGradientEnd;
        protected Color overflowButtonGradientMiddle;
        protected Color raftingContainerGradientBegin;
        protected Color raftingContainerGradientEnd;
        protected Color separatorDark;
        protected Color separatorLight;
        protected Color statusStripGradientBegin;
        protected Color statusStripGradientEnd;
        protected Color toolStripBorder;
        protected Color toolStripContentPanelGradientBegin;
        protected Color toolStripContentPanelGradientEnd;
        protected Color toolStripDropDownBackground;
        protected Color toolStripGradientBegin;
        protected Color toolStripGradientEnd;
        protected Color toolStripGradientMiddle;
        protected Color toolStripPanelGradientBegin;
        protected Color toolStripPanelGradientEnd;

        #endregion

        #region Properties

        public override Color ButtonCheckedGradientBegin
        {
            get
            {
                if (buttonCheckedGradientBegin != Color.Empty)
                    return buttonCheckedGradientBegin;
                else
                    return base.ButtonCheckedGradientBegin;
            }
        }
        public override Color ButtonCheckedGradientEnd
        {
            get
            {
                if (buttonCheckedGradientEnd != Color.Empty)
                    return buttonCheckedGradientEnd;
                else
                    return base.ButtonCheckedGradientEnd;
            }
        }
        public override Color ButtonCheckedGradientMiddle
        {
            get
            {
                if (buttonCheckedGradientMiddle != Color.Empty)
                    return buttonCheckedGradientMiddle;
                else
                    return base.ButtonCheckedGradientMiddle;
            }
        }
        public override Color ButtonCheckedHighlight
        {
            get
            {
                if (buttonCheckedHighlight != Color.Empty)
                    return buttonCheckedHighlight;
                else
                    return base.ButtonCheckedHighlight;
            }
        }
        public override Color ButtonCheckedHighlightBorder
        {
            get
            {
                if (buttonCheckedHighlightBorder != Color.Empty)
                    return buttonCheckedHighlightBorder;
                else
                    return base.ButtonCheckedHighlightBorder;
            }
        }
        public override Color ButtonPressedBorder
        {
            get
            {
                if (buttonPressedBorder != Color.Empty)
                    return buttonPressedBorder;
                else
                    return base.ButtonPressedBorder;
            }
        }
        public override Color ButtonPressedGradientBegin
        {
            get
            {
                if (buttonPressedGradientBegin != Color.Empty)
                    return buttonPressedGradientBegin;
                else
                    return base.ButtonPressedGradientBegin;
            }
        }
        public override Color ButtonPressedGradientEnd
        {
            get
            {
                if (buttonPressedGradientEnd != Color.Empty)
                    return buttonPressedGradientEnd;
                else
                    return base.ButtonPressedGradientEnd;
            }
        }
        public override Color ButtonPressedGradientMiddle
        {
            get
            {
                if (buttonPressedGradientMiddle != Color.Empty)
                    return buttonPressedGradientMiddle;
                else
                    return base.ButtonPressedGradientMiddle;
            }
        }
        public override Color ButtonPressedHighlight
        {
            get
            {
                if (buttonPressedHighlight != Color.Empty)
                    return buttonPressedHighlight;
                else
                    return base.ButtonPressedHighlight;
            }
        }
        public override Color ButtonPressedHighlightBorder
        {
            get
            {
                if (buttonPressedHighlightBorder != Color.Empty)
                    return buttonPressedHighlightBorder;
                else
                    return base.ButtonPressedHighlightBorder;
            }
        }
        public override Color ButtonSelectedBorder
        {
            get
            {
                if (buttonSelectedBorder != Color.Empty)
                    return buttonSelectedBorder;
                else
                    return base.ButtonSelectedBorder;
            }
        }
        public override Color ButtonSelectedGradientBegin
        {
            get
            {
                if (buttonSelectedGradientBegin != Color.Empty)
                    return buttonSelectedGradientBegin;
                else
                    return base.ButtonSelectedGradientBegin;
            }
        }
        public override Color ButtonSelectedGradientEnd
        {
            get
            {
                if (buttonSelectedGradientEnd != Color.Empty)
                    return buttonSelectedGradientEnd;
                else
                    return base.ButtonSelectedGradientEnd;
            }
        }
        public override Color ButtonSelectedGradientMiddle
        {
            get
            {
                if (buttonSelectedGradientMiddle != Color.Empty)
                    return buttonSelectedGradientMiddle;
                else
                    return base.ButtonSelectedGradientMiddle;
            }
        }
        public override Color ButtonSelectedHighlight
        {
            get
            {
                if (buttonSelectedHighlight != Color.Empty)
                    return buttonSelectedHighlight;
                else
                    return base.ButtonSelectedHighlight;
            }
        }
        public override Color ButtonSelectedHighlightBorder
        {
            get
            {
                if (buttonSelectedHighlightBorder != Color.Empty)
                    return buttonSelectedHighlightBorder;
                else
                    return base.ButtonSelectedHighlightBorder;
            }
        }
        public override Color CheckBackground
        {
            get
            {
                if (checkBackground != Color.Empty)
                    return checkBackground;
                else
                    return base.CheckBackground;
            }
        }
        public override Color CheckPressedBackground
        {
            get
            {
                if (checkPressedBackground != Color.Empty)
                    return checkPressedBackground;
                else
                    return base.CheckPressedBackground;
            }
        }
        public override Color CheckSelectedBackground
        {
            get
            {
                if (checkSelectedBackground != Color.Empty)
                    return checkSelectedBackground;
                else
                    return base.CheckSelectedBackground;
            }
        }
        public override Color GripDark
        {
            get
            {
                if (gripDark != Color.Empty)
                    return gripDark;
                else
                    return base.GripDark;
            }
        }
        public override Color GripLight
        {
            get
            {
                if (gripLight != Color.Empty)
                    return gripLight;
                else
                    return base.GripLight;
            }
        }
        public override Color ImageMarginGradientBegin
        {
            get
            {
                if (imageMarginGradientBegin != Color.Empty)
                    return imageMarginGradientBegin;
                else
                    return base.ImageMarginGradientBegin;
            }
        }
        public override Color ImageMarginGradientEnd
        {
            get
            {
                if (imageMarginGradientEnd != Color.Empty)
                    return imageMarginGradientEnd;
                else
                    return base.ImageMarginGradientEnd;
            }
        }
        public override Color ImageMarginGradientMiddle
        {
            get
            {
                if (imageMarginGradientMiddle != Color.Empty)
                    return imageMarginGradientMiddle;
                else
                    return base.ImageMarginGradientMiddle;
            }
        }
        public override Color ImageMarginRevealedGradientBegin
        {
            get
            {
                if (imageMarginRevealedGradientBegin != Color.Empty)
                    return imageMarginRevealedGradientBegin;
                else
                    return base.ImageMarginRevealedGradientBegin;
            }
        }
        public override Color ImageMarginRevealedGradientEnd
        {
            get
            {
                if (imageMarginRevealedGradientEnd != Color.Empty)
                    return imageMarginRevealedGradientEnd;
                else
                    return base.ImageMarginRevealedGradientEnd;
            }
        }
        public override Color ImageMarginRevealedGradientMiddle
        {
            get
            {
                if (imageMarginRevealedGradientMiddle != Color.Empty)
                    return imageMarginRevealedGradientMiddle;
                else
                    return base.ImageMarginRevealedGradientMiddle;
            }
        }
        public override Color MenuBorder
        {
            get
            {
                if (menuBorder != Color.Empty)
                    return menuBorder;
                else
                    return base.MenuBorder;
            }
        }
        public override Color MenuItemBorder
        {
            get
            {
                if (menuItemBorder != Color.Empty)
                    return menuItemBorder;
                else
                    return base.MenuItemBorder;
            }
        }
        public override Color MenuItemPressedGradientBegin
        {
            get
            {
                if (menuItemPressedGradientBegin != Color.Empty)
                    return menuItemPressedGradientBegin;
                else
                    return base.MenuItemPressedGradientBegin;
            }
        }
        public override Color MenuItemPressedGradientEnd
        {
            get
            {
                if (menuItemPressedGradientEnd != Color.Empty)
                    return menuItemPressedGradientEnd;
                else
                    return base.MenuItemPressedGradientEnd;
            }
        }
        public override Color MenuItemPressedGradientMiddle
        {
            get
            {
                if (menuItemPressedGradientMiddle != Color.Empty)
                    return menuItemPressedGradientMiddle;
                else
                    return base.MenuItemPressedGradientMiddle;
            }
        }
        public override Color MenuItemSelected
        {
            get
            {
                if (menuItemSelected != Color.Empty)
                    return menuItemSelected;
                else
                    return base.MenuItemSelected;
            }
        }
        public override Color MenuItemSelectedGradientBegin
        {
            get
            {
                if (menuItemSelectedGradientBegin != Color.Empty)
                    return menuItemSelectedGradientBegin;
                else
                    return base.MenuItemSelectedGradientBegin;
            }
        }
        public override Color MenuItemSelectedGradientEnd
        {
            get
            {
                if (menuItemSelectedGradientEnd != Color.Empty)
                    return menuItemSelectedGradientEnd;
                else
                    return base.MenuItemSelectedGradientEnd;
            }
        }
        public override Color MenuStripGradientBegin
        {
            get
            {
                if (menuStripGradientBegin != Color.Empty)
                    return menuStripGradientBegin;
                else
                    return base.MenuStripGradientBegin;
            }
        }
        public override Color MenuStripGradientEnd
        {
            get
            {
                if (menuStripGradientEnd != Color.Empty)
                    return menuStripGradientEnd;
                else
                    return base.MenuStripGradientEnd;
            }
        }
        public override Color OverflowButtonGradientBegin
        {
            get
            {
                if (overflowButtonGradientBegin != Color.Empty)
                    return overflowButtonGradientBegin;
                else
                    return base.OverflowButtonGradientBegin;
            }
        }
        public override Color OverflowButtonGradientEnd
        {
            get
            {
                if (overflowButtonGradientEnd != Color.Empty)
                    return overflowButtonGradientEnd;
                else
                    return base.OverflowButtonGradientEnd;
            }
        }
        public override Color OverflowButtonGradientMiddle
        {
            get
            {
                if (overflowButtonGradientMiddle != Color.Empty)
                    return overflowButtonGradientMiddle;
                else
                    return base.OverflowButtonGradientMiddle;
            }
        }
        public override Color RaftingContainerGradientBegin
        {
            get
            {
                if (raftingContainerGradientBegin != Color.Empty)
                    return raftingContainerGradientBegin;
                else
                    return base.RaftingContainerGradientBegin;
            }
        }
        public override Color RaftingContainerGradientEnd
        {
            get
            {
                if (raftingContainerGradientEnd != Color.Empty)
                    return raftingContainerGradientEnd;
                else
                    return base.RaftingContainerGradientEnd;
            }
        }
        public override Color SeparatorDark
        {
            get
            {
                if (separatorDark != Color.Empty)
                    return separatorDark;
                else
                    return base.SeparatorDark;
            }
        }
        public override Color SeparatorLight
        {
            get
            {
                if (separatorLight != Color.Empty)
                    return separatorLight;
                else
                    return base.SeparatorLight;
            }
        }
        public override Color StatusStripGradientBegin
        {
            get
            {
                if (statusStripGradientBegin != Color.Empty)
                    return statusStripGradientBegin;
                else
                    return base.StatusStripGradientBegin;
            }
        }
        public override Color StatusStripGradientEnd
        {
            get
            {
                if (statusStripGradientEnd != Color.Empty)
                    return statusStripGradientEnd;
                else
                    return base.StatusStripGradientEnd;
            }
        }
        public override Color ToolStripBorder
        {
            get
            {
                if (toolStripBorder != Color.Empty)
                    return toolStripBorder;
                else
                    return base.ToolStripBorder;
            }
        }
        public override Color ToolStripContentPanelGradientBegin
        {
            get
            {
                if (toolStripContentPanelGradientBegin != Color.Empty)
                    return toolStripContentPanelGradientBegin;
                else
                    return base.ToolStripContentPanelGradientBegin;
            }
        }
        public override Color ToolStripContentPanelGradientEnd
        {
            get
            {
                if (toolStripContentPanelGradientEnd != Color.Empty)
                    return toolStripContentPanelGradientEnd;
                else
                    return base.ToolStripContentPanelGradientEnd;
            }
        }
        public override Color ToolStripDropDownBackground
        {
            get
            {
                if (toolStripDropDownBackground != Color.Empty)
                    return toolStripDropDownBackground;
                else
                    return base.ToolStripDropDownBackground;
            }
        }
        public override Color ToolStripGradientBegin
        {
            get
            {
                if (toolStripGradientBegin != Color.Empty)
                    return toolStripGradientBegin;
                else
                    return base.ToolStripGradientBegin;
            }
        }
        public override Color ToolStripGradientEnd
        {
            get
            {
                if (toolStripGradientEnd != Color.Empty)
                    return toolStripGradientEnd;
                else
                    return base.ToolStripGradientEnd;
            }
        }
        public override Color ToolStripGradientMiddle
        {
            get
            {
                if (toolStripGradientMiddle != Color.Empty)
                    return toolStripGradientMiddle;
                else
                    return base.ToolStripGradientMiddle;
            }
        }
        public override Color ToolStripPanelGradientBegin
        {
            get
            {
                if (toolStripPanelGradientBegin != Color.Empty)
                    return toolStripPanelGradientBegin;
                else
                    return base.ToolStripPanelGradientBegin;
            }
        }
        public override Color ToolStripPanelGradientEnd
        {
            get
            {
                if (toolStripPanelGradientEnd != Color.Empty)
                    return toolStripPanelGradientEnd;
                else
                    return base.ToolStripPanelGradientEnd;
            }
        }

        #endregion
    }
}
