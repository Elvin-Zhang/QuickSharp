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
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using System.IO;

namespace QuickSharp.Core
{
    /// <summary>
    /// The Visual Studio 2008 theme color table.
    /// </summary>
    public class VS2008ColorTable : ProfessionalColorTable
    {
        /// <summary>
        /// Create an instance of the color scheme.
        /// </summary>
        public VS2008ColorTable()
        {
        }

        internal Color FromKnownColor(VS2008ColorTable.KnownColors color)
        {
            return (Color)this.ColorTable[color];
        }

        internal static void InitTanLunaColors(ref Dictionary<VS2008ColorTable.KnownColors, Color> rgbTable)
        {
            rgbTable[VS2008ColorTable.KnownColors.GripDark] = Color.FromArgb(0xc1, 190, 0xb3);
            rgbTable[VS2008ColorTable.KnownColors.SeparatorDark] = Color.FromArgb(0xc5, 0xc2, 0xb8);
            rgbTable[VS2008ColorTable.KnownColors.MenuItemSelected] = Color.FromArgb(0xc1, 210, 0xee);
            rgbTable[VS2008ColorTable.KnownColors.ButtonPressedBorder] = Color.FromArgb(0x31, 0x6a, 0xc5);
            rgbTable[VS2008ColorTable.KnownColors.CheckBackground] = Color.FromArgb(0xe1, 230, 0xe8);
            rgbTable[VS2008ColorTable.KnownColors.MenuItemBorder] = Color.FromArgb(0x31, 0x6a, 0xc5);
            rgbTable[VS2008ColorTable.KnownColors.CheckBackgroundMouseOver] = Color.FromArgb(0x31, 0x6a, 0xc5);
            rgbTable[VS2008ColorTable.KnownColors.MenuItemBorderMouseOver] = Color.FromArgb(0x4b, 0x4b, 0x6f);
            rgbTable[VS2008ColorTable.KnownColors.ToolStripDropDownBackground] = Color.FromArgb(0xfc, 0xfc, 0xf9);
            rgbTable[VS2008ColorTable.KnownColors.MenuBorder] = Color.FromArgb(0x8a, 0x86, 0x7a);
            rgbTable[VS2008ColorTable.KnownColors.SeparatorLight] = Color.FromArgb(0xff, 0xff, 0xff);
            rgbTable[VS2008ColorTable.KnownColors.ToolStripBorder] = Color.FromArgb(0xa3, 0xa3, 0x7c);
            rgbTable[VS2008ColorTable.KnownColors.MenuStripGradientBegin] = Color.FromArgb(0xeb, 0xeb, 0xdd);
            rgbTable[VS2008ColorTable.KnownColors.MenuStripGradientEnd] = Color.FromArgb(0xeb, 0xeb, 0xdd);
            rgbTable[VS2008ColorTable.KnownColors.ImageMarginGradientBegin] = Color.FromArgb(0xfe, 0xfe, 0xfb);
            rgbTable[VS2008ColorTable.KnownColors.ImageMarginGradientMiddle] = Color.FromArgb(0xec, 0xe7, 0xe0);
            rgbTable[VS2008ColorTable.KnownColors.ImageMarginGradientEnd] = Color.FromArgb(0xbd, 0xbd, 0xa3);
            rgbTable[VS2008ColorTable.KnownColors.OverflowButtonGradientBegin] = Color.FromArgb(0xf3, 0xf2, 240);
            rgbTable[VS2008ColorTable.KnownColors.OverflowButtonGradientMiddle] = Color.FromArgb(0xe2, 0xe1, 0xdb);
            rgbTable[VS2008ColorTable.KnownColors.OverflowButtonGradientEnd] = Color.FromArgb(0x92, 0x92, 0x76);
            rgbTable[VS2008ColorTable.KnownColors.MenuItemPressedGradientBegin] = Color.FromArgb(0xfc, 0xfc, 0xf9);
            rgbTable[VS2008ColorTable.KnownColors.MenuItemPressedGradientEnd] = Color.FromArgb(0xf6, 0xf4, 0xec);
            rgbTable[VS2008ColorTable.KnownColors.ImageMarginRevealedGradientBegin] = Color.FromArgb(0xf7, 0xf6, 0xef);
            rgbTable[VS2008ColorTable.KnownColors.ImageMarginRevealedGradientMiddle] = Color.FromArgb(0xf2, 240, 0xe4);
            rgbTable[VS2008ColorTable.KnownColors.ImageMarginRevealedGradientEnd] = Color.FromArgb(230, 0xe3, 210);
            rgbTable[VS2008ColorTable.KnownColors.ButtonCheckedGradientBegin] = Color.FromArgb(0xe1, 230, 0xe8);
            rgbTable[VS2008ColorTable.KnownColors.ButtonCheckedGradientMiddle] = Color.FromArgb(0xe1, 230, 0xe8);
            rgbTable[VS2008ColorTable.KnownColors.ButtonCheckedGradientEnd] = Color.FromArgb(0xe1, 230, 0xe8);
            rgbTable[VS2008ColorTable.KnownColors.ButtonSelectedGradientBegin] = Color.FromArgb(0xc1, 210, 0xee);
            rgbTable[VS2008ColorTable.KnownColors.ButtonSelectedGradientMiddle] = Color.FromArgb(0xc1, 210, 0xee);
            rgbTable[VS2008ColorTable.KnownColors.ButtonSelectedGradientEnd] = Color.FromArgb(0xc1, 210, 0xee);
            rgbTable[VS2008ColorTable.KnownColors.ButtonPressedGradientBegin] = Color.FromArgb(0x98, 0xb5, 0xe2);
            rgbTable[VS2008ColorTable.KnownColors.ButtonPressedGradientMiddle] = Color.FromArgb(0x98, 0xb5, 0xe2);
            rgbTable[VS2008ColorTable.KnownColors.ButtonPressedGradientEnd] = Color.FromArgb(0x98, 0xb5, 0xe2);
            rgbTable[VS2008ColorTable.KnownColors.GripLight] = Color.FromArgb(0xff, 0xff, 0xff);
        }

        public override Color ButtonCheckedGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ButtonCheckedGradientBegin);
                }
                return base.ButtonCheckedGradientBegin;
            }
        }

        public override Color ButtonCheckedGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ButtonCheckedGradientEnd);
                }
                return base.ButtonCheckedGradientEnd;
            }
        }

        public override Color ButtonCheckedGradientMiddle
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ButtonCheckedGradientMiddle);
                }
                return base.ButtonCheckedGradientMiddle;
            }
        }

        public override Color ButtonPressedBorder
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ButtonPressedBorder);
                }
                return base.ButtonPressedBorder;
            }
        }

        public override Color ButtonPressedGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ButtonPressedGradientBegin);
                }
                return base.ButtonPressedGradientBegin;
            }
        }

        public override Color ButtonPressedGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ButtonPressedGradientEnd);
                }
                return base.ButtonPressedGradientEnd;
            }
        }

        public override Color ButtonPressedGradientMiddle
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ButtonPressedGradientMiddle);
                }
                return base.ButtonPressedGradientMiddle;
            }
        }

        public override Color ButtonSelectedBorder
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ButtonPressedBorder);
                }
                return base.ButtonSelectedBorder;
            }
        }

        public override Color ButtonSelectedGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ButtonSelectedGradientBegin);
                }
                return base.ButtonSelectedGradientBegin;
            }
        }

        public override Color ButtonSelectedGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ButtonSelectedGradientEnd);
                }
                return base.ButtonSelectedGradientEnd;
            }
        }

        public override Color ButtonSelectedGradientMiddle
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ButtonSelectedGradientMiddle);
                }
                return base.ButtonSelectedGradientMiddle;
            }
        }

        public override Color CheckBackground
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.CheckBackground);
                }
                return base.CheckBackground;
            }
        }

        public override Color CheckPressedBackground
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.CheckBackgroundMouseOver);
                }
                return base.CheckPressedBackground;
            }
        }

        public override Color CheckSelectedBackground
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.CheckBackgroundMouseOver);
                }
                return base.CheckSelectedBackground;
            }
        }

        internal static string ColorScheme
        {
            get
            {
                return VS2008ColorTable.DisplayInformation.ColorScheme;
            }
        }

        private Dictionary<KnownColors, Color> ColorTable
        {
            get
            {
                if (this.tanRGB == null)
                {
                    this.tanRGB = new Dictionary<KnownColors, Color>((int)KnownColors.LastKnownColor);
                    VS2008ColorTable.InitTanLunaColors(ref this.tanRGB);
                }
                return this.tanRGB;
            }
        }

        public override Color GripDark
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.GripDark);
                }
                return base.GripDark;
            }
        }

        public override Color GripLight
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.GripLight);
                }
                return base.GripLight;
            }
        }

        public override Color ImageMarginGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ImageMarginGradientBegin);
                }
                return base.ImageMarginGradientBegin;
            }
        }

        public override Color ImageMarginGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ImageMarginGradientEnd);
                }
                return base.ImageMarginGradientEnd;
            }
        }

        public override Color ImageMarginGradientMiddle
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ImageMarginGradientMiddle);
                }
                return base.ImageMarginGradientMiddle;
            }
        }

        public override Color ImageMarginRevealedGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ImageMarginRevealedGradientBegin);
                }
                return base.ImageMarginRevealedGradientBegin;
            }
        }

        public override Color ImageMarginRevealedGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ImageMarginRevealedGradientEnd);
                }
                return base.ImageMarginRevealedGradientEnd;
            }
        }

        public override Color ImageMarginRevealedGradientMiddle
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ImageMarginRevealedGradientMiddle);
                }
                return base.ImageMarginRevealedGradientMiddle;
            }
        }

        public override Color MenuBorder
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.MenuBorder);
                }
                return base.MenuItemBorder;
            }
        }

        public override Color MenuItemBorder
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.MenuItemBorder);
                }
                return base.MenuItemBorder;
            }
        }

        public override Color MenuItemPressedGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.MenuItemPressedGradientBegin);
                }
                return base.MenuItemPressedGradientBegin;
            }
        }

        public override Color MenuItemPressedGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.MenuItemPressedGradientEnd);
                }
                return base.MenuItemPressedGradientEnd;
            }
        }

        public override Color MenuItemPressedGradientMiddle
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ImageMarginRevealedGradientMiddle);
                }
                return base.MenuItemPressedGradientMiddle;
            }
        }

        public override Color MenuItemSelected
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.MenuItemSelected);
                }
                return base.MenuItemSelected;
            }
        }

        public override Color MenuItemSelectedGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ButtonSelectedGradientBegin);
                }
                return base.MenuItemSelectedGradientBegin;
            }
        }

        public override Color MenuItemSelectedGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ButtonSelectedGradientEnd);
                }
                return base.MenuItemSelectedGradientEnd;
            }
        }

        public override Color MenuStripGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.MenuStripGradientBegin);
                }
                return base.MenuStripGradientBegin;
            }
        }

        public override Color MenuStripGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.MenuStripGradientEnd);
                }
                return base.MenuStripGradientEnd;
            }
        }

        public override Color OverflowButtonGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.OverflowButtonGradientBegin);
                }
                return base.OverflowButtonGradientBegin;
            }
        }

        public override Color OverflowButtonGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.OverflowButtonGradientEnd);
                }
                return base.OverflowButtonGradientEnd;
            }
        }

        public override Color OverflowButtonGradientMiddle
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.OverflowButtonGradientMiddle);
                }
                return base.OverflowButtonGradientMiddle;
            }
        }

        public override Color RaftingContainerGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.MenuStripGradientBegin);
                }
                return base.RaftingContainerGradientBegin;
            }
        }

        public override Color RaftingContainerGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.MenuStripGradientEnd);
                }
                return base.RaftingContainerGradientEnd;
            }
        }

        public override Color SeparatorDark
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.SeparatorDark);
                }
                return base.SeparatorDark;
            }
        }

        public override Color SeparatorLight
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.SeparatorLight);
                }
                return base.SeparatorLight;
            }
        }

        public override Color ToolStripBorder
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ToolStripBorder);
                }
                return base.ToolStripBorder;
            }
        }

        public override Color ToolStripDropDownBackground
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ToolStripDropDownBackground);
                }
                return base.ToolStripDropDownBackground;
            }
        }

        public override Color ToolStripGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ImageMarginGradientBegin);
                }
                return base.ToolStripGradientBegin;
            }
        }

        public override Color ToolStripGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ImageMarginGradientEnd);
                }
                return base.ToolStripGradientEnd;
            }
        }

        public override Color ToolStripGradientMiddle
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.ImageMarginGradientMiddle);
                }
                return base.ToolStripGradientMiddle;
            }
        }

        public override Color ToolStripPanelGradientBegin
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.MenuStripGradientBegin);
                }
                return base.ToolStripPanelGradientBegin;
            }
        }

        public override Color ToolStripPanelGradientEnd
        {
            get
            {
                if (!this.UseBaseColorTable)
                {
                    return this.FromKnownColor(VS2008ColorTable.KnownColors.MenuStripGradientEnd);
                }
                return base.ToolStripPanelGradientEnd;
            }
        }

        private bool UseBaseColorTable
        {
            get
            {
                //bool flag1 = !ToolStripTanColorTable.DisplayInformation.IsLunaTheme;

                //if (flag1 && (this.tanRGB != null))
                //{
                //    this.tanRGB.Clear();
                //    this.tanRGB = null;
                //}
                //return flag1;

                return false;
            }
        }

        private Dictionary<VS2008ColorTable.KnownColors, Color> tanRGB;

        private static class DisplayInformation
        {
            static DisplayInformation()
            {
                SystemEvents.UserPreferenceChanged += 
                    new UserPreferenceChangedEventHandler(
                        VS2008ColorTable.DisplayInformation.OnUserPreferenceChanged);
                
                VS2008ColorTable.DisplayInformation.SetScheme();
            }

            private static void OnUserPreferenceChanged(
                object sender, UserPreferenceChangedEventArgs e)
            {
                VS2008ColorTable.DisplayInformation.SetScheme();
            }

            private static void SetScheme()
            {
                VS2008ColorTable.DisplayInformation.isLunaTheme = false;
                if (VisualStyleRenderer.IsSupported)
                {
                    DisplayInformation.colorScheme = 
                        VisualStyleInformation.ColorScheme;

                    if (!VisualStyleInformation.IsEnabledByUser)
                    {
                        return;
                    }
                    StringBuilder builder1 = new StringBuilder(0x200);
                    GetCurrentThemeName(
                        builder1, builder1.Capacity, null, 0, null, 0);

                    string text1 = builder1.ToString();
                    
                    VS2008ColorTable.DisplayInformation.isLunaTheme = 
                        string.Equals(
                            "luna.msstyles", 
                            Path.GetFileName(text1), 
                            StringComparison.InvariantCultureIgnoreCase);
                }
                else
                {
                    VS2008ColorTable.DisplayInformation.colorScheme = null;
                }
            }

            public static string ColorScheme
            {
                get
                {
                    return colorScheme;
                }
            }

            internal static bool IsLunaTheme
            {
                get
                {
                    return isLunaTheme;
                }
            }

            [ThreadStatic]
            private static string colorScheme;
            [ThreadStatic]
            private static bool isLunaTheme;
            private const string lunaFileName = "luna.msstyles";

            [DllImport("uxtheme.dll", CharSet = CharSet.Auto)]
            public static extern int GetCurrentThemeName(
                StringBuilder pszThemeFileName, 
                int dwMaxNameChars, 
                StringBuilder pszColorBuff, 
                int dwMaxColorChars,
                StringBuilder pszSizeBuff,
                int cchMaxSizeChars);

        }

        internal enum KnownColors
        {
            ButtonPressedBorder,
            MenuItemBorder,
            MenuItemBorderMouseOver,
            MenuItemSelected,
            CheckBackground,
            CheckBackgroundMouseOver,
            GripDark,
            GripLight,
            MenuStripGradientBegin,
            MenuStripGradientEnd,
            ImageMarginRevealedGradientBegin,
            ImageMarginRevealedGradientEnd,
            ImageMarginRevealedGradientMiddle,
            MenuItemPressedGradientBegin,
            MenuItemPressedGradientEnd,
            ButtonPressedGradientBegin,
            ButtonPressedGradientEnd,
            ButtonPressedGradientMiddle,
            ButtonSelectedGradientBegin,
            ButtonSelectedGradientEnd,
            ButtonSelectedGradientMiddle,
            OverflowButtonGradientBegin,
            OverflowButtonGradientEnd,
            OverflowButtonGradientMiddle,
            ButtonCheckedGradientBegin,
            ButtonCheckedGradientEnd,
            ButtonCheckedGradientMiddle,
            ImageMarginGradientBegin,
            ImageMarginGradientEnd,
            ImageMarginGradientMiddle,
            MenuBorder,
            ToolStripDropDownBackground,
            ToolStripBorder,
            SeparatorDark,
            SeparatorLight,
            LastKnownColor = SeparatorLight
        }
    }
}