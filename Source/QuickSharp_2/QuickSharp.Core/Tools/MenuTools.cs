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
using System.Windows.Forms;
using System.Collections.Generic;

namespace QuickSharp.Core
{
    /// <summary>
    /// Provides utility methods for creating menu and toolbar items.
    /// </summary>
    public static class MenuTools
    {
        /// <summary>
        /// Create a separator with the specified name.
        /// </summary>
        /// <param name="name">The name of the separator.</param>
        /// <returns>The separator.</returns>
        public static ToolStripSeparator CreateSeparator(string name)
        {
            ToolStripSeparator separator = new ToolStripSeparator();
            separator.Name = name;
            return separator;
        }

        /// <summary>
        /// Create a menu item.
        /// </summary>
        /// <param name="itemName">The menu item name.</param>
        /// <param name="itemText">The menu item text.</param>
        /// <param name="itemImage">The menu item image.</param>
        /// <param name="keys">The menu item's shortcut keys.</param>
        /// <param name="keyText">A text representation of the menu item shortcut
        /// keys if the keys are defined elsewhere.</param>
        /// <param name="onClick">The menu item click event handler.</param>
        /// <returns>The menu item.</returns>
        public static ToolStripMenuItem CreateMenuItem(
            string itemName,
            string itemText,
            Image itemImage,
            Keys keys,
            string keyText,
            EventHandler onClick)
        {
            return CreateMenuItem(
                itemName, itemText, itemImage, keys, keyText, onClick, false);
        }

        /// <summary>
        /// Create a menu item and a trailing separator. The separator is only created
        /// as a flag on the menu item, the actual separator is created once all the menu
        /// items have been loaded. A separator should be included after each group of
        /// related menu items. If a separator is inserted at the last position in a menu
        /// it will automatically be deleted if no menu items follow.
        /// </summary>
        /// <param name="itemName">The menu item name.</param>
        /// <param name="itemText">The menu item text.</param>
        /// <param name="itemImage">The menu item image.</param>
        /// <param name="keys">The menu item's shortcut keys.</param>
        /// <param name="keyText">A text representation of the menu item shortcut
        /// keys if the keys are defined elsewhere.</param>
        /// <param name="onClick">The menu item's click event handler.</param>
        /// <param name="includeSeparator">If true insert a separator after the menu item.</param>
        /// <returns>The menu item.</returns>
        public static ToolStripMenuItem CreateMenuItem(
            string itemName,
            string itemText,
            Image itemImage,
            Keys keys,
            string keyText,
            EventHandler onClick,
            bool includeSeparator)
        {
            ToolStripItemTag itemTag = new ToolStripItemTag();
            itemTag.IncludeSeparator = includeSeparator;
 
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Name = itemName;
            item.Text = itemText;
            item.Tag = itemTag;
            item.ShortcutKeys = keys;
            if (keyText != null) item.ShortcutKeyDisplayString = keyText;
            if (itemImage != null)
            {
                item.ImageTransparentColor = System.Drawing.Color.Fuchsia;
                item.Image = itemImage;
            }
            if (onClick != null)
                item.Click += new EventHandler(onClick);

            return item;
        }

        /// <summary>
        /// Create a toolbar button.
        /// </summary>
        /// <param name="itemName">The name of the button.</param>
        /// <param name="itemText">The tooltip text for the button.</param>
        /// <param name="itemImage">The button's image.</param>
        /// <param name="onClick">The button's click event handler.</param>
        /// <returns>The button.</returns>
        public static ToolStripButton CreateToolbarButton(
            string itemName,
            string itemText,
            Image itemImage,
            EventHandler onClick)
        {
            return CreateToolbarButton(
                itemName, itemText, itemImage, onClick, false);
        }

        /// <summary>
        /// Create a toolbar button with a leading separator. The separator is only created
        /// as a flag on the button, the actual separator is inserted after the entire
        /// toolbar has been assembled. The first item in a group of buttons should
        /// include a separator.
        /// </summary>
        /// <param name="itemName">The name of the button.</param>
        /// <param name="itemText">The tooltip text for the button.</param>
        /// <param name="itemImage">The button's image.</param>
        /// <param name="onClick">The button's click event handler.</param>
        /// <param name="includeSeparator">If true insert a separator before the button.</param>
        /// <returns>The button.</returns>
        public static ToolStripButton CreateToolbarButton(
            string itemName,
            string itemText,
            Image itemImage,
            EventHandler onClick,
            bool includeSeparator)
        {
            ToolStripItemTag itemTag = new ToolStripItemTag();
            itemTag.IncludeSeparator = includeSeparator;

            ToolStripButton button = new ToolStripButton();
            button.Name = itemName;
            button.Tag = itemTag;
            button.ToolTipText = itemText;
            button.DisplayStyle = ToolStripItemDisplayStyle.Image;
            if (itemImage != null)
            {
                button.ImageTransparentColor = Color.Fuchsia;
                button.Image = itemImage;
            }
            if (onClick != null)
                button.Click += new EventHandler(onClick);

            return button;
        }

        /// <summary>
        /// Create a dropdown toolbar button.
        /// </summary>
        /// <param name="itemName">The button name.</param>
        /// <param name="onClick">The button's click event handler.</param>
        /// <returns>The button.</returns>
        public static ToolStripDropDownButton CreateToolbarDropDownButton(
            string itemName,
            ToolStripItemClickedEventHandler onClick)
        {
            ToolStripDropDownButton button = new ToolStripDropDownButton();
            button.Name = itemName;
            if (onClick != null)
                button.DropDownItemClicked += onClick;

            return button;
        }

        /// <summary>
        /// Set the foreground color for all enabled items in a menu strip.
        /// </summary>
        /// <param name="menuStrip">The menu strip.</param>
        /// <param name="color">The foreground color.</param>
        /// <param name="hideImages">Hide menu item images.</param>
        public static void MenuStripItemsSetForeColor(
            MenuStrip menuStrip, Color color, bool hideImages)
        {
            menuStrip.ForeColor = color;

            foreach (ToolStripItem item in menuStrip.Items)
            {
                ToolStripMenuItem menu = item as ToolStripMenuItem;
                
                if (menu != null)
                    MenuDropDownItemsSetForeColor(menu, color, hideImages);
            }
        }

        /// <summary>
        /// Set the foreground color for all enabled items in a
        /// context menu strip.
        /// </summary>
        /// <param name="menuStrip">The menu strip.</param>
        /// <param name="color">The foreground color.</param>
        /// <param name="hideImages">Hide menu item images.</param>
        public static void ContextMenuStripItemsSetForeColor(
            ContextMenuStrip menuStrip, Color color, bool hideImages)
        {
            menuStrip.ForeColor = color;

            foreach (ToolStripItem item in menuStrip.Items)
            {
                ToolStripMenuItem menu = item as ToolStripMenuItem;
                if (menu == null) continue;

                if (hideImages)
                    menu.DisplayStyle = ToolStripItemDisplayStyle.Text;

                MenuDropDownItemsSetForeColor(menu, color, hideImages);
            }
        }

        /// <summary>
        /// Set the foreground color for all the enabled dropdown items
        /// in a menu and its child menus.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="color">The foreground color.</param>
        /// <param name="hideImages">Hide menu item images.</param>
        public static void MenuDropDownItemsSetForeColor(
            ToolStripMenuItem menu, Color color, bool hideImages)
        {
            foreach (ToolStripItem menuItem in menu.DropDownItems)
            {
                ToolStripMenuItem item = menuItem as ToolStripMenuItem;
                if (item == null) continue;

                item.ForeColor = color;

                if (hideImages)
                    item.DisplayStyle = ToolStripItemDisplayStyle.Text; 

                MenuDropDownItemsSetForeColor(item, color, hideImages);
            }
        }
    }
}
