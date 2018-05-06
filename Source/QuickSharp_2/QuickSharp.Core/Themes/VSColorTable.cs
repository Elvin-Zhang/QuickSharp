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
    /// Base class for Visual Studio themes color tables.
    /// </summary>
    public abstract class VSColorTable : ProfessionalColorTable
    {
        public Color FromKnownColor(KnownColors color)
        {
            return (Color)this.ColorTable[color];
        }

        protected static Color RGB(int r, int g, int b)
        {
            return Color.FromArgb(r, g, b);
        }

        private bool UseBaseColorTable
        {
            get { return false; }
        }

        private Dictionary<KnownColors, Color> themeRGB;

        private string ColorScheme
        {
            get
            {
                return DisplayInformation.ColorScheme;
            }
        }

        private Dictionary<KnownColors, Color> ColorTable
        {
            get
            {
                if (this.themeRGB == null)
                {
                    this.themeRGB = new Dictionary<KnownColors, Color>((int)KnownColors.LastKnownColor);
                    InitColors(this.themeRGB);
                }
                return this.themeRGB;
            }
        }

        protected abstract void InitColors(Dictionary<KnownColors, Color> rgbTable);

        #region Color properties

        public override Color ButtonCheckedGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ButtonCheckedGradientBegin);

                return base.ButtonCheckedGradientBegin;
            }
        }

        public override Color ButtonCheckedGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ButtonCheckedGradientEnd);

                return base.ButtonCheckedGradientEnd;
            }
        }

        public override Color ButtonCheckedGradientMiddle
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ButtonCheckedGradientMiddle);

                return base.ButtonCheckedGradientMiddle;
            }
        }

        public override Color ButtonCheckedHighlight
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ButtonCheckedHighlight);

                return base.ButtonCheckedHighlight;
            }
        }

        public override Color ButtonCheckedHighlightBorder
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ButtonCheckedHighlightBorder);

                return base.ButtonCheckedHighlightBorder;
            }
        }

        public override Color ButtonPressedBorder
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ButtonPressedBorder);

                return base.ButtonPressedBorder;
            }
        }

        public override Color ButtonPressedGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ButtonPressedGradientBegin);

                return base.ButtonPressedGradientBegin;
            }
        }

        public override Color ButtonPressedGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ButtonPressedGradientEnd);

                return base.ButtonPressedGradientEnd;
            }
        }

        public override Color ButtonPressedGradientMiddle
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ButtonPressedGradientMiddle);

                return base.ButtonPressedGradientMiddle;
            }
        }

        public override Color ButtonPressedHighlight
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ButtonPressedHighlight);

                return base.ButtonPressedHighlight;
            }
        }

        public override Color ButtonPressedHighlightBorder
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ButtonPressedHighlightBorder);

                return base.ButtonPressedHighlightBorder;
            }
        }

        public override Color ButtonSelectedBorder
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ButtonSelectedBorder);

                return base.ButtonSelectedBorder;
            }
        }

        public override Color ButtonSelectedGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ButtonSelectedGradientBegin);

                return base.ButtonSelectedGradientBegin;
            }
        }

        public override Color ButtonSelectedGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ButtonSelectedGradientEnd);

                return base.ButtonSelectedGradientEnd;
            }
        }

        public override Color ButtonSelectedGradientMiddle
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ButtonSelectedGradientMiddle);

                return base.ButtonSelectedGradientMiddle;
            }
        }

        public override Color ButtonSelectedHighlight
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ButtonSelectedHighlight);

                return base.ButtonSelectedHighlight;
            }
        }

        public override Color ButtonSelectedHighlightBorder
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ButtonSelectedHighlightBorder);

                return base.ButtonSelectedHighlightBorder;
            }
        }

        public override Color CheckBackground
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.CheckBackground);

                return base.CheckBackground;
            }
        }

        public override Color CheckPressedBackground
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.CheckPressedBackground);

                return base.CheckPressedBackground;
            }
        }

        public override Color CheckSelectedBackground
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.CheckSelectedBackground);

                return base.CheckSelectedBackground;
            }
        }

        public override Color GripDark
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.GripDark);

                return base.GripDark;
            }
        }

        public override Color GripLight
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.GripLight);

                return base.GripLight;
            }
        }

        public override Color ImageMarginGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ImageMarginGradientBegin);

                return base.ImageMarginGradientBegin;
            }
        }

        public override Color ImageMarginGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ImageMarginGradientEnd);

                return base.ImageMarginGradientEnd;
            }
        }

        public override Color ImageMarginGradientMiddle
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ImageMarginGradientMiddle);

                return base.ImageMarginGradientMiddle;
            }
        }

        public override Color ImageMarginRevealedGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ImageMarginRevealedGradientBegin);

                return base.ImageMarginRevealedGradientBegin;
            }
        }

        public override Color ImageMarginRevealedGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ImageMarginRevealedGradientEnd);

                return base.ImageMarginRevealedGradientEnd;
            }
        }

        public override Color ImageMarginRevealedGradientMiddle
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ImageMarginRevealedGradientMiddle);

                return base.ImageMarginRevealedGradientMiddle;
            }
        }

        public override Color MenuBorder
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.MenuBorder);

                return base.MenuBorder;
            }
        }

        public override Color MenuItemBorder
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.MenuItemBorder);

                return base.MenuItemBorder;
            }
        }

        public override Color MenuItemPressedGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.MenuItemPressedGradientBegin);

                return base.MenuItemPressedGradientBegin;
            }
        }

        public override Color MenuItemPressedGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.MenuItemPressedGradientEnd);

                return base.MenuItemPressedGradientEnd;
            }
        }

        public override Color MenuItemPressedGradientMiddle
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.MenuItemPressedGradientMiddle);

                return base.MenuItemPressedGradientMiddle;
            }
        }

        public override Color MenuItemSelected
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.MenuItemSelected);

                return base.MenuItemSelected;
            }
        }

        public override Color MenuItemSelectedGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.MenuItemSelectedGradientBegin);

                return base.MenuItemSelectedGradientBegin;
            }
        }

        public override Color MenuItemSelectedGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.MenuItemSelectedGradientEnd);

                return base.MenuItemSelectedGradientEnd;
            }
        }

        public override Color MenuStripGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.MenuStripGradientBegin);

                return base.MenuStripGradientBegin;
            }
        }

        public override Color MenuStripGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.MenuStripGradientEnd);

                return base.MenuStripGradientEnd;
            }
        }

        public override Color OverflowButtonGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.OverflowButtonGradientBegin);

                return base.OverflowButtonGradientBegin;
            }
        }

        public override Color OverflowButtonGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.OverflowButtonGradientEnd);

                return base.OverflowButtonGradientEnd;
            }
        }

        public override Color OverflowButtonGradientMiddle
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.OverflowButtonGradientMiddle);

                return base.OverflowButtonGradientMiddle;
            }
        }

        public override Color RaftingContainerGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.RaftingContainerGradientBegin);

                return base.RaftingContainerGradientBegin;
            }
        }

        public override Color RaftingContainerGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.RaftingContainerGradientEnd);

                return base.RaftingContainerGradientEnd;
            }
        }

        public override Color SeparatorDark
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.SeparatorDark);

                return base.SeparatorDark;
            }
        }

        public override Color SeparatorLight
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.SeparatorLight);

                return base.SeparatorLight;
            }
        }

        public override Color StatusStripGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.StatusStripGradientBegin);

                return base.StatusStripGradientBegin;
            }
        }

        public override Color StatusStripGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.StatusStripGradientEnd);

                return base.StatusStripGradientEnd;
            }
        }

        public override Color ToolStripBorder
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ToolStripBorder);

                return base.ToolStripBorder;
            }
        }

        public override Color ToolStripContentPanelGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ToolStripContentPanelGradientBegin);

                return base.ToolStripContentPanelGradientBegin;
            }
        }

        public override Color ToolStripContentPanelGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ToolStripContentPanelGradientEnd);

                return base.ToolStripContentPanelGradientEnd;
            }
        }

        public override Color ToolStripDropDownBackground
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ToolStripDropDownBackground);

                return base.ToolStripDropDownBackground;
            }
        }

        public override Color ToolStripGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ToolStripGradientBegin);

                return base.ToolStripGradientBegin;
            }
        }

        public override Color ToolStripGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ToolStripGradientEnd);

                return base.ToolStripGradientEnd;
            }
        }

        public override Color ToolStripGradientMiddle
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ToolStripGradientMiddle);

                return base.ToolStripGradientMiddle;
            }
        }

        public override Color ToolStripPanelGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ToolStripPanelGradientBegin);

                return base.ToolStripPanelGradientBegin;
            }
        }

        public override Color ToolStripPanelGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                    return this.FromKnownColor(KnownColors.ToolStripPanelGradientEnd);

                return base.ToolStripPanelGradientEnd;
            }
        }

        #endregion
    }
}
