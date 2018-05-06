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
using System.ComponentModel;

namespace QuickSharp.CodeAssist
{
    partial class LookupForm
    {
        private IContainer _components = null;
        private ToolStripMenuItem _dummyMenuItem;
        private ListView _listView;
        private ColumnHeader _itemColumnName;
        private ContextMenuStrip _lookupContextMenuStrip;
        private ImageList _itemImageList;

        #region Form Control Names

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_listView = "listView";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_itemColumnName = "itemColumnName";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_lookupContextMenuStrip = "lookupContextMenuStrip";

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
                _components.Dispose();

            base.Dispose(disposing);
        }

        #region Form Layout

        private void InitializeComponent()
        {
            _components = new Container();
            _listView = new ListView();
            _itemColumnName = new ColumnHeader();
            _lookupContextMenuStrip = new ContextMenuStrip(_components);
            _dummyMenuItem = new ToolStripMenuItem();
            _itemImageList = new ImageList(_components);
            _lookupContextMenuStrip.SuspendLayout();
            SuspendLayout();

            _itemColumnName.Name = m_itemColumnName;
            
            _listView.Columns.AddRange(new ColumnHeader[] {
            _itemColumnName});
            _listView.ContextMenuStrip = _lookupContextMenuStrip;
            _listView.Dock = DockStyle.Fill;
            _listView.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            _listView.FullRowSelect = false;
            _listView.HeaderStyle = ColumnHeaderStyle.None;
            _listView.Location = new Point(0, 0);
            _listView.Name = m_listView;
            _listView.ShowItemToolTips = true;
            _listView.Size = new Size(162, Constants.LOOKUP_WINDOW_HEIGHT);
            _listView.SmallImageList = _itemImageList;
            _listView.Sorting = SortOrder.Ascending;
            _listView.TabIndex = 0;
            _listView.UseCompatibleStateImageBehavior = false;
            _listView.View = View.Details;
            _listView.MouseDoubleClick += new MouseEventHandler(ListView_MouseDoubleClick);

            _lookupContextMenuStrip.Items.AddRange(new ToolStripItem[] {
            _dummyMenuItem});
            _lookupContextMenuStrip.Name = m_lookupContextMenuStrip;
            _lookupContextMenuStrip.Size = new Size(162, 26);
            _lookupContextMenuStrip.Opening += new CancelEventHandler(LookupContextMenuStrip_Opening);

            _dummyMenuItem.Name = "UI_LOOKUP_MENU_DUMMY_ITEM";
            _dummyMenuItem.Size = new Size(161, 22);
            _dummyMenuItem.Text = @"Don't Delete Me";

            _itemImageList.TransparentColor = Color.Fuchsia;
            _itemImageList.ColorDepth = ColorDepth.Depth24Bit; 
            _itemImageList.Images.Add("ASSEMBLY", Resources.ASSEMBLY);
            _itemImageList.Images.Add("CLASS", Resources.CLASS);
            _itemImageList.Images.Add("CLASS_FRIEND", Resources.CLASS_FRIEND);
            _itemImageList.Images.Add("CLASS_PRIVATE", Resources.CLASS_PRIVATE);
            _itemImageList.Images.Add("CLASS_PROTECTED", Resources.CLASS_PROTECTED);
            _itemImageList.Images.Add("CLASS_SEALED", Resources.CLASS_SEALED);
            _itemImageList.Images.Add("CONSTANT", Resources.CONSTANT);
            _itemImageList.Images.Add("CONSTANT_FRIEND", Resources.CONSTANT_FRIEND);
            _itemImageList.Images.Add("CONSTANT_PRIVATE", Resources.CONSTANT_PRIVATE);
            _itemImageList.Images.Add("CONSTANT_PROTECTED", Resources.CONSTANT_PROTECTED);
            _itemImageList.Images.Add("CONSTANT_SEALED", Resources.CONSTANT_SEALED);
            _itemImageList.Images.Add("DELEGATE", Resources.DELEGATE);
            _itemImageList.Images.Add("DELEGATE_FRIEND", Resources.DELEGATE_FRIEND);
            _itemImageList.Images.Add("DELEGATE_PRIVATE", Resources.DELEGATE_PRIVATE);
            _itemImageList.Images.Add("DELEGATE_PROTECTED", Resources.DELEGATE_PROTECTED);
            _itemImageList.Images.Add("DELEGATE_SEALED", Resources.DELEGATE_SEALED);
            _itemImageList.Images.Add("ENUM", Resources.ENUM);
            _itemImageList.Images.Add("ENUM_FRIEND", Resources.ENUM_FRIEND);
            _itemImageList.Images.Add("ENUM_PROTECTED", Resources.ENUM_PROTECTED);
            _itemImageList.Images.Add("ENUM_SEALED", Resources.ENUM_SEALED);
            _itemImageList.Images.Add("EVENT", Resources.EVENT);
            _itemImageList.Images.Add("EVENT_FRIEND", Resources.EVENT_FRIEND);
            _itemImageList.Images.Add("EVENT_PRIVATE", Resources.EVENT_PRIVATE);
            _itemImageList.Images.Add("EVENT_PROTECTED", Resources.EVENT_PROTECTED);
            _itemImageList.Images.Add("EVENT_SEALED", Resources.EVENT_SEALED);
            _itemImageList.Images.Add("EXCEPTION", Resources.EXCEPTION);
            _itemImageList.Images.Add("EXCEPTION_FRIEND", Resources.EXCEPTION_FRIEND);
            _itemImageList.Images.Add("EXCEPTION_PRIVATE", Resources.EXCEPTION_PRIVATE);
            _itemImageList.Images.Add("EXCEPTION_PROTECTED", Resources.EXCEPTION_PROTECTED);
            _itemImageList.Images.Add("EXCEPTION_SEALED", Resources.EXCEPTION_SEALED);
            _itemImageList.Images.Add("FIELD", Resources.FIELD);
            _itemImageList.Images.Add("FIELD_FRIEND", Resources.FIELD_FRIEND);
            _itemImageList.Images.Add("FIELD_PRIVATE", Resources.FIELD_PRIVATE);
            _itemImageList.Images.Add("FIELD_PROTECTED", Resources.FIELD_PROTECTED);
            _itemImageList.Images.Add("FIELD_SEALED", Resources.FIELD_SEALED);
            _itemImageList.Images.Add("FILE_REF", Resources.FILE_REF);
            _itemImageList.Images.Add("INTERFACE", Resources.INTERFACE);
            _itemImageList.Images.Add("INTERFACE_FRIEND", Resources.INTERFACE_FRIEND);
            _itemImageList.Images.Add("INTERFACE_PRIVATE", Resources.INTERFACE_PRIVATE);
            _itemImageList.Images.Add("INTERFACE_PROTECTED", Resources.INTERFACE_PROTECTED);
            _itemImageList.Images.Add("INTERFACE_SEALED", Resources.INTERFACE_SEALED);
            _itemImageList.Images.Add("METHOD", Resources.METHOD);
            _itemImageList.Images.Add("METHOD_FRIEND", Resources.METHOD_FRIEND);
            _itemImageList.Images.Add("METHOD_PRIVATE", Resources.METHOD_PRIVATE);
            _itemImageList.Images.Add("METHOD_PROTECTED", Resources.METHOD_PROTECTED);
            _itemImageList.Images.Add("METHOD_SEALED", Resources.METHOD_SEALED);
            _itemImageList.Images.Add("METHOD_EXTENSION", Resources.METHOD_EXTENSION);
            _itemImageList.Images.Add("METHODOVERLOAD", Resources.METHODOVERLOAD);
            _itemImageList.Images.Add("METHODOVERLOAD_FRIEND", Resources.METHODOVERLOAD_FRIEND);
            _itemImageList.Images.Add("METHODOVERLOAD_PRIVATE", Resources.METHODOVERLOAD_PRIVATE);
            _itemImageList.Images.Add("METHODOVERLOAD_PROTECTED", Resources.METHODOVERLOAD_PROTECTED);
            _itemImageList.Images.Add("METHODOVERLOAD_SEALED", Resources.METHODOVERLOAD_SEALED);
            _itemImageList.Images.Add("NAMESPACE", Resources.NAMESPACE);
            _itemImageList.Images.Add("NAMESPACE_FRIEND", Resources.NAMESPACE_FRIEND);
            _itemImageList.Images.Add("NAMESPACE_PRIVATE", Resources.NAMESPACE_PRIVATE);
            _itemImageList.Images.Add("NAMESPACE_PROTECTED", Resources.NAMESPACE_PROTECTED);
            _itemImageList.Images.Add("NAMESPACE_SEALED", Resources.NAMESPACE_SEALED);
            _itemImageList.Images.Add("OPERATOR", Resources.OPERATOR);
            _itemImageList.Images.Add("OPERATOR_FRIEND", Resources.OPERATOR_FRIEND);
            _itemImageList.Images.Add("OPERATOR_PRIVATE", Resources.OPERATOR_PRIVATE);
            _itemImageList.Images.Add("OPERATOR_PROTECTED", Resources.OPERATOR_PROTECTED);
            _itemImageList.Images.Add("OPERATOR_SEALED", Resources.OPERATOR_SEALED);
            _itemImageList.Images.Add("PROPERTIES", Resources.PROPERTIES);
            _itemImageList.Images.Add("PROPERTIES_FRIEND", Resources.PROPERTIES_FRIEND);
            _itemImageList.Images.Add("PROPERTIES_PRIVATE", Resources.PROPERTIES_PRIVATE);
            _itemImageList.Images.Add("PROPERTIES_PROTECTED", Resources.PROPERTIES_PROTECTED);
            _itemImageList.Images.Add("PROPERTIES_SEALED", Resources.PROPERTIES_SEALED);
            _itemImageList.Images.Add("STRUCTURE", Resources.STRUCTURE);
            _itemImageList.Images.Add("STRUCTURE_FRIEND", Resources.STRUCTURE_FRIEND);
            _itemImageList.Images.Add("STRUCTURE_PRIVATE", Resources.STRUCTURE_PRIVATE);
            _itemImageList.Images.Add("STRUCTURE_PROTECTED", Resources.STRUCTURE_PROTECTED);
            _itemImageList.Images.Add("STRUCTURE_SEALED", Resources.STRUCTURE_SEALED);
            _itemImageList.Images.Add("TABLE", Resources.TABLE);
            _itemImageList.Images.Add("TEMPLATE", Resources.TEMPLATE);
            _itemImageList.Images.Add("TEMPLATE_FRIEND", Resources.TEMPLATE_FRIEND);
            _itemImageList.Images.Add("TEMPLATE_PRIVATE", Resources.TEMPLATE_PRIVATE);
            _itemImageList.Images.Add("TEMPLATE_PROTECTED", Resources.TEMPLATE_PROTECTED);
            _itemImageList.Images.Add("TEMPLATE_SEALED", Resources.TEMPLATE_SEALED);
            _itemImageList.Images.Add("TYPE", Resources.TYPE);
            _itemImageList.Images.Add("TYPE_FRIEND", Resources.TYPE_FRIEND);
            _itemImageList.Images.Add("TYPE_PRIVATE", Resources.TYPE_PRIVATE);
            _itemImageList.Images.Add("TYPE_PROTECTED", Resources.TYPE_PROTECTED);
            _itemImageList.Images.Add("TYPE_SEALED", Resources.TYPE_SEALED);
            _itemImageList.Images.Add("TYPEDEF", Resources.TYPEDEF);
            _itemImageList.Images.Add("TYPEDEF_FRIEND", Resources.TYPEDEF_FRIEND);
            _itemImageList.Images.Add("TYPEDEF_PRIVATE", Resources.TYPEDEF_PRIVATE);
            _itemImageList.Images.Add("TYPEDEF_PROTECTED", Resources.TYPEDEF_PROTECTED);
            _itemImageList.Images.Add("TYPEDEF_SEALED", Resources.TYPEDEF_SEALED);
            _itemImageList.Images.Add("UNION", Resources.UNION);
            _itemImageList.Images.Add("UNION_FRIEND", Resources.UNION_FRIEND);
            _itemImageList.Images.Add("UNION_PRIVATE", Resources.UNION_PRIVATE);
            _itemImageList.Images.Add("UNION_PROTECTED", Resources.UNION_PROTECTED);
            _itemImageList.Images.Add("UNION_SEALED", Resources.UNION_SEALED);
            _itemImageList.Images.Add("VALUETYPE", Resources.VALUETYPE);
            _itemImageList.Images.Add("VALUETYPE_FRIEND", Resources.VALUETYPE_FRIEND);
            _itemImageList.Images.Add("VALUETYPE_PRIVATE", Resources.VALUETYPE_PRIVATE);
            _itemImageList.Images.Add("VALUETYPE_PROTECTED", Resources.VALUETYPE_PROTECTED);
            _itemImageList.Images.Add("VALUETYPE_SEALED", Resources.VALUETYPE_SEALED);
            _itemImageList.Images.Add("VIEW", Resources.VIEW);
            _itemImageList.Images.Add("WEBCONTROL", Resources.WEBCONTROL);
            _itemImageList.Images.Add("FIELD_ALIAS", Resources.FIELD_ALIAS);
            _itemImageList.Images.Add("TABLE_ALIAS", Resources.TABLE_ALIAS);
            _itemImageList.Images.Add("VIEW_ALIAS", Resources.VIEW_ALIAS);

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(162, Constants.LOOKUP_WINDOW_HEIGHT);
            Controls.Add(_listView);
            KeyPreview = true;
            Name = "LookupForm";
            ShowInTaskbar = false;
            Text = "LookupForm";
            KeyDown += new KeyEventHandler(LookupForm_KeyDown);
            _lookupContextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}