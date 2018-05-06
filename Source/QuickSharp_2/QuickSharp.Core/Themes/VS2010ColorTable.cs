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

namespace QuickSharp.Core
{
    /// <summary>
    /// The Visual Studio 2010 theme color table.
    /// </summary>
    public class VS2010ColorTable : VSColorTable
    {
        /// <summary>
        /// Create an instance of the color scheme.
        /// </summary>
        protected override void InitColors(Dictionary<KnownColors, Color> rgbTable)
        {
            // Checked toolbar button
            rgbTable[KnownColors.ButtonCheckedGradientBegin] = Color.FromArgb(0xFF, 0xF3, 0xCD);
            rgbTable[KnownColors.ButtonCheckedGradientEnd] = Color.FromArgb(0xFF, 0xF3, 0xCD);
            rgbTable[KnownColors.ButtonCheckedGradientMiddle] = Color.FromArgb(0xFF, 0xF3, 0xCD);   //?
            rgbTable[KnownColors.ButtonCheckedHighlight] = Color.FromArgb(0xD5, 0xDC, 0xE8);        //?
            rgbTable[KnownColors.ButtonCheckedHighlightBorder] = Color.FromArgb(0xE5, 0xC3, 0x65);  //?

            // Pressed toolbar button (and mouse over)
            rgbTable[KnownColors.ButtonPressedBorder] = Color.FromArgb(0xE5, 0xC3, 0x65);
            rgbTable[KnownColors.ButtonPressedGradientBegin] = Color.FromArgb(0xFF, 0xF3, 0xCD);
            rgbTable[KnownColors.ButtonPressedGradientEnd] = Color.FromArgb(0xFF, 0xF3, 0xCD);

            // Toolbar overflow button
            rgbTable[KnownColors.ButtonPressedGradientMiddle] = Color.FromArgb(0xD5, 0xDC, 0xE8);
            rgbTable[KnownColors.ButtonPressedHighlight] = Color.FromArgb(0xD5, 0xDC, 0xE8);
            rgbTable[KnownColors.ButtonPressedHighlightBorder] = Color.FromArgb(0xD5, 0xDC, 0xE8);

            // Unchecked toolbar button
            rgbTable[KnownColors.ButtonSelectedBorder] = Color.FromArgb(0xE5, 0xC3, 0x65);
            rgbTable[KnownColors.ButtonSelectedGradientBegin] = Color.FromArgb(0xFF, 0xF3, 0xCD);
            rgbTable[KnownColors.ButtonSelectedGradientEnd] = Color.FromArgb(0xFF, 0xF3, 0xCD);

            // Toolbar overflow button mouseover
            rgbTable[KnownColors.ButtonSelectedGradientMiddle] = Color.FromArgb(0xD5, 0xDC, 0xE8);

            // ??
            rgbTable[KnownColors.ButtonSelectedHighlight] = Color.FromArgb(0xD5, 0xDC, 0xE8);
            rgbTable[KnownColors.ButtonSelectedHighlightBorder] = Color.FromArgb(0xE5, 0xC3, 0x65);    

            // Checked menu item
            rgbTable[KnownColors.CheckBackground] = Color.FromArgb(0xFF, 0xF3, 0xCD);

            // ??
            rgbTable[KnownColors.CheckPressedBackground] = Color.FromArgb(0xFF, 0xF3, 0xCD);        

            // Selected checked menu item
            rgbTable[KnownColors.CheckSelectedBackground] = Color.FromArgb(0xFF, 0xF3, 0xCD);

            // Toolbar grip (dots = dark, border = light)
            rgbTable[KnownColors.GripDark] = Color.FromArgb(0x60, 0x72, 0x8C);
            rgbTable[KnownColors.GripLight] = Color.FromArgb(0xBC, 0xC7, 0xD8); ;
            
            // Menu icon strip
            rgbTable[KnownColors.ImageMarginGradientBegin] = Color.FromArgb(0xE9, 0xEC, 0xEE);
            rgbTable[KnownColors.ImageMarginGradientEnd] = Color.FromArgb(0xE9, 0xEC, 0xEE);
            rgbTable[KnownColors.ImageMarginGradientMiddle] = Color.FromArgb(0xE9, 0xEC, 0xEE);

            // ??
            rgbTable[KnownColors.ImageMarginRevealedGradientBegin] = Color.FromArgb(0xD5, 0xDC, 0xE8);
            rgbTable[KnownColors.ImageMarginRevealedGradientEnd] = Color.FromArgb(0xD5, 0xDC, 0xE8);
            rgbTable[KnownColors.ImageMarginRevealedGradientMiddle] = Color.FromArgb(0xD5, 0xDC, 0xE8);

            // Menu border
            rgbTable[KnownColors.MenuBorder] = Color.FromArgb(0x9B, 0xA7, 0xB7);

            // Highlighted menu item border
            rgbTable[KnownColors.MenuItemBorder] = Color.FromArgb(0xE5, 0xC3, 0x65);

            // Menu strip highlighted item (with dropdown)
            rgbTable[KnownColors.MenuItemPressedGradientBegin] = Color.FromArgb(0xE9, 0xEC, 0xEE);
            rgbTable[KnownColors.MenuItemPressedGradientEnd] = Color.FromArgb(0xE9, 0xEC, 0xEE);
            rgbTable[KnownColors.MenuItemPressedGradientMiddle] = Color.FromArgb(0xE9, 0xEC, 0xEE);  //??

            // Highlighted menu item
            rgbTable[KnownColors.MenuItemSelected] = Color.FromArgb(0xFF, 0xF3, 0xCD);

            // Menu strip highlighted item (no dropdown)
            rgbTable[KnownColors.MenuItemSelectedGradientBegin] = Color.FromArgb(0xFF, 0xF3, 0xCD);
            rgbTable[KnownColors.MenuItemSelectedGradientEnd] = Color.FromArgb(0xFF, 0xF3, 0xCD);

            // Menu strip
            rgbTable[KnownColors.MenuStripGradientBegin] = Color.FromArgb(0xBB, 0xC4, 0xD5);
            rgbTable[KnownColors.MenuStripGradientEnd] = Color.FromArgb(0xBB, 0xC4, 0xD5);

            // Toolbar overflow button
            rgbTable[KnownColors.OverflowButtonGradientBegin] = Color.FromArgb(0xBC, 0xC7, 0xD8);
            rgbTable[KnownColors.OverflowButtonGradientEnd] = Color.FromArgb(0xBC, 0xC7, 0xD8);
            rgbTable[KnownColors.OverflowButtonGradientMiddle] = Color.FromArgb(0xBC, 0xC7, 0xD8);

            // ??
            rgbTable[KnownColors.RaftingContainerGradientBegin] = Color.FromArgb(0xD5, 0xDC, 0xE8);
            rgbTable[KnownColors.RaftingContainerGradientEnd] = Color.FromArgb(0xD5, 0xDC, 0xE8);

            // Menu and toolbar separator (dark for menu)
            rgbTable[KnownColors.SeparatorDark] = Color.FromArgb(0xBE, 0xC3, 0xCB);
            rgbTable[KnownColors.SeparatorLight] = Color.FromArgb(0x85, 0x91, 0xA2);

            // ??
            rgbTable[KnownColors.StatusStripGradientBegin] = Color.FromArgb(0xD5, 0xDC, 0xE8);
            rgbTable[KnownColors.StatusStripGradientEnd] = Color.FromArgb(0xD5, 0xDC, 0xE8);

            // ToolStrip border
            rgbTable[KnownColors.ToolStripBorder] = Color.FromArgb(0x9C, 0xAA, 0xC1);
            
            // ??
            rgbTable[KnownColors.ToolStripContentPanelGradientBegin] = Color.FromArgb(0xD5, 0xDC, 0xE8);
            rgbTable[KnownColors.ToolStripContentPanelGradientEnd] = Color.FromArgb(0xD5, 0xDC, 0xE8);
            
            // Menu background
            rgbTable[KnownColors.ToolStripDropDownBackground] = Color.FromArgb(0xE9, 0xEC, 0xEE);
            
            // Toolbar
            rgbTable[KnownColors.ToolStripGradientBegin] = Color.FromArgb(0xBC, 0xC7, 0xD8);
            rgbTable[KnownColors.ToolStripGradientEnd] = Color.FromArgb(0xBC, 0xC7, 0xD8);
            rgbTable[KnownColors.ToolStripGradientMiddle] = Color.FromArgb(0xBC, 0xC7, 0xD8);

            // ToolStripPanel
            rgbTable[KnownColors.ToolStripPanelGradientBegin] = Color.FromArgb(0x9C, 0xAA, 0xC1);
            rgbTable[KnownColors.ToolStripPanelGradientEnd] = Color.FromArgb(0x9C, 0xAA, 0xC1);
        }
    }
}