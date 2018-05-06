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

using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace QuickSharp.CodeAssist.ObjectBrowser
{
    partial class ObjectBrowserForm
    {
        private IContainer _components = null;
        private Splitter _splitter;
        private SplitContainer _splitContainer;
        private ColumnHeader _columnHeader;

        public TreeView treeView;
        public ImageList nodeImageList;
        public ListView listView;
        public PropertyGrid propertyGrid;

        public const string m_treeView = "treeView";
        public const string m_splitter = "splitter";
        public const string m_nodeImageList = "nodeImageList";
        public const string m_splitContainer = "splitContainer";
        public const string m_listView = "listView";
        public const string m_columnHeader = "columnHeader";
        public const string m_propertyGrid = "propertyGrid";

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
            _splitter = new Splitter();
            _splitContainer = new SplitContainer();
            _columnHeader = new ColumnHeader();
            
            treeView = new TreeView();
            nodeImageList = new ImageList(_components);
            listView = new ListView();
            propertyGrid = new PropertyGrid();

            _splitContainer.Panel1.SuspendLayout();
            _splitContainer.Panel2.SuspendLayout();
            _splitContainer.SuspendLayout();

            SuspendLayout();

            treeView.Dock = DockStyle.Left;
            treeView.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            treeView.ImageIndex = 0;
            treeView.ImageList = nodeImageList;
            treeView.HideSelection = false;
            treeView.Location = new Point(0, 0);
            treeView.Name = m_treeView;
            treeView.SelectedImageIndex = 0;
            treeView.Size = new Size(275, 366);
            treeView.TabIndex = 0;
            treeView.AfterSelect += delegate { SelectTreeNode(); };
            treeView.Click += delegate { SelectTreeNode(); };
            treeView.BeforeExpand += new TreeViewCancelEventHandler(TreeView_BeforeExpand);

            nodeImageList.TransparentColor = Color.Fuchsia;
            nodeImageList.ColorDepth = ColorDepth.Depth24Bit; 
            nodeImageList.Images.Add("ASSEMBLY", Resources.ASSEMBLY);
            nodeImageList.Images.Add("CLASS", Resources.CLASS);
            nodeImageList.Images.Add("CLASS_FRIEND", Resources.CLASS_FRIEND);
            nodeImageList.Images.Add("CLASS_PRIVATE", Resources.CLASS_PRIVATE);
            nodeImageList.Images.Add("CLASS_PROTECTED", Resources.CLASS_PROTECTED);
            nodeImageList.Images.Add("CLASS_SEALED", Resources.CLASS_SEALED);
            nodeImageList.Images.Add("CONSTANT", Resources.CONSTANT);
            nodeImageList.Images.Add("CONSTANT_FRIEND", Resources.CONSTANT_FRIEND);
            nodeImageList.Images.Add("CONSTANT_PRIVATE", Resources.CONSTANT_PRIVATE);
            nodeImageList.Images.Add("CONSTANT_PROTECTED", Resources.CONSTANT_PROTECTED);
            nodeImageList.Images.Add("CONSTANT_SEALED", Resources.CONSTANT_SEALED);
            nodeImageList.Images.Add("DELEGATE", Resources.DELEGATE);
            nodeImageList.Images.Add("DELEGATE_FRIEND", Resources.DELEGATE_FRIEND);
            nodeImageList.Images.Add("DELEGATE_PRIVATE", Resources.DELEGATE_PRIVATE);
            nodeImageList.Images.Add("DELEGATE_PROTECTED", Resources.DELEGATE_PROTECTED);
            nodeImageList.Images.Add("DELEGATE_SEALED", Resources.DELEGATE_SEALED);
            nodeImageList.Images.Add("ENUM", Resources.ENUM);
            nodeImageList.Images.Add("ENUM_FRIEND", Resources.ENUM_FRIEND);
            nodeImageList.Images.Add("ENUM_PROTECTED", Resources.ENUM_PROTECTED);
            nodeImageList.Images.Add("ENUM_SEALED", Resources.ENUM_SEALED);
            nodeImageList.Images.Add("EVENT", Resources.EVENT);
            nodeImageList.Images.Add("EVENT_FRIEND", Resources.EVENT_FRIEND);
            nodeImageList.Images.Add("EVENT_PRIVATE", Resources.EVENT_PRIVATE);
            nodeImageList.Images.Add("EVENT_PROTECTED", Resources.EVENT_PROTECTED);
            nodeImageList.Images.Add("EVENT_SEALED", Resources.EVENT_SEALED);
            nodeImageList.Images.Add("FIELD", Resources.FIELD);
            nodeImageList.Images.Add("FIELD_FRIEND", Resources.FIELD_FRIEND);
            nodeImageList.Images.Add("FIELD_PRIVATE", Resources.FIELD_PRIVATE);
            nodeImageList.Images.Add("FIELD_PROTECTED", Resources.FIELD_PROTECTED);
            nodeImageList.Images.Add("FIELD_SEALED", Resources.FIELD_SEALED);
            nodeImageList.Images.Add("INTERFACE", Resources.INTERFACE);
            nodeImageList.Images.Add("INTERFACE_FRIEND", Resources.INTERFACE_FRIEND);
            nodeImageList.Images.Add("INTERFACE_PRIVATE", Resources.INTERFACE_PRIVATE);
            nodeImageList.Images.Add("INTERFACE_PROTECTED", Resources.INTERFACE_PROTECTED);
            nodeImageList.Images.Add("INTERFACE_SEALED", Resources.INTERFACE_SEALED);
            nodeImageList.Images.Add("METHOD", Resources.METHOD);
            nodeImageList.Images.Add("METHOD_FRIEND", Resources.METHOD_FRIEND);
            nodeImageList.Images.Add("METHOD_PRIVATE", Resources.METHOD_PRIVATE);
            nodeImageList.Images.Add("METHOD_PROTECTED", Resources.METHOD_PROTECTED);
            nodeImageList.Images.Add("METHOD_SEALED", Resources.METHOD_SEALED);
            nodeImageList.Images.Add("METHOD_EXTENSION", Resources.METHOD_EXTENSION);
            nodeImageList.Images.Add("METHODOVERLOAD", Resources.METHODOVERLOAD);
            nodeImageList.Images.Add("METHODOVERLOAD_FRIEND", Resources.METHODOVERLOAD_FRIEND);
            nodeImageList.Images.Add("METHODOVERLOAD_PRIVATE", Resources.METHODOVERLOAD_PRIVATE);
            nodeImageList.Images.Add("METHODOVERLOAD_PROTECTED", Resources.METHODOVERLOAD_PROTECTED);
            nodeImageList.Images.Add("METHODOVERLOAD_SEALED", Resources.METHODOVERLOAD_SEALED);
            nodeImageList.Images.Add("NAMESPACE", Resources.NAMESPACE);
            nodeImageList.Images.Add("NAMESPACE_FRIEND", Resources.NAMESPACE_FRIEND);
            nodeImageList.Images.Add("NAMESPACE_PRIVATE", Resources.NAMESPACE_PRIVATE);
            nodeImageList.Images.Add("NAMESPACE_PROTECTED", Resources.NAMESPACE_PROTECTED);
            nodeImageList.Images.Add("NAMESPACE_SEALED", Resources.NAMESPACE_SEALED);
            nodeImageList.Images.Add("PROPERTIES", Resources.PROPERTIES);
            nodeImageList.Images.Add("PROPERTIES_FRIEND", Resources.PROPERTIES_FRIEND);
            nodeImageList.Images.Add("PROPERTIES_PRIVATE", Resources.PROPERTIES_PRIVATE);
            nodeImageList.Images.Add("PROPERTIES_PROTECTED", Resources.PROPERTIES_PROTECTED);
            nodeImageList.Images.Add("PROPERTIES_SEALED", Resources.PROPERTIES_SEALED);
            nodeImageList.Images.Add("STRUCTURE", Resources.STRUCTURE);
            nodeImageList.Images.Add("STRUCTURE_FRIEND", Resources.STRUCTURE_FRIEND);
            nodeImageList.Images.Add("STRUCTURE_PRIVATE", Resources.STRUCTURE_PRIVATE);
            nodeImageList.Images.Add("STRUCTURE_PROTECTED", Resources.STRUCTURE_PROTECTED);
            nodeImageList.Images.Add("STRUCTURE_SEALED", Resources.STRUCTURE_SEALED);
            nodeImageList.Images.Add("VALUETYPE", Resources.VALUETYPE);
            nodeImageList.Images.Add("VALUETYPE_FRIEND", Resources.VALUETYPE_FRIEND);
            nodeImageList.Images.Add("VALUETYPE_PRIVATE", Resources.VALUETYPE_PRIVATE);
            nodeImageList.Images.Add("VALUETYPE_PROTECTED", Resources.VALUETYPE_PROTECTED);
            nodeImageList.Images.Add("VALUETYPE_SEALED", Resources.VALUETYPE_SEALED);

            _splitter.Location = new Point(275, 0);
            _splitter.Name = m_splitter;
            _splitter.Size = new Size(3, 366);
            _splitter.TabIndex = 1;
            _splitter.TabStop = false;

            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.Location = new Point(278, 0);
            _splitContainer.Name = m_splitContainer;
            _splitContainer.Panel1.Controls.Add(listView);
            _splitContainer.Panel2.Controls.Add(propertyGrid);
            _splitContainer.Size = new Size(339, 366);
            _splitContainer.SplitterDistance = 215;
            _splitContainer.TabIndex = 4;
            _splitContainer.Panel2Collapsed = true;

            listView.Columns.AddRange(new ColumnHeader[] {
                _columnHeader
            });
            listView.Dock = DockStyle.Fill;
            listView.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            listView.HeaderStyle = ColumnHeaderStyle.None;
            listView.Location = new Point(0, 0);
            listView.Name = m_listView;
            listView.Size = new Size(215, 366);
            listView.SmallImageList = nodeImageList;
            listView.Sorting = SortOrder.Ascending;
            listView.TabIndex = 3;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = View.Details;
            listView.SelectedIndexChanged += new System.EventHandler(ListView_SelectedIndexChanged);
            listView.ShowItemToolTips = true;

            propertyGrid.Dock = DockStyle.Fill;
            propertyGrid.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            propertyGrid.HelpVisible = false;
            propertyGrid.Location = new Point(0, 0);
            propertyGrid.Name = m_propertyGrid;
            propertyGrid.PropertySort = PropertySort.Alphabetical;
            propertyGrid.TabIndex = 4;
            propertyGrid.ToolbarVisible = false;

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(617, 366);
            Controls.Add(_splitContainer);
            Controls.Add(_splitter);
            Controls.Add(treeView);
            DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            Icon = Resources.ObjectBrowserIcon;
            Name = "ObjectBrowserForm";
            ShowInTaskbar = false;
            Text = "ObjectBrowserForm";

            _splitContainer.Panel1.ResumeLayout(false);
            _splitContainer.Panel2.ResumeLayout(false);
            _splitContainer.ResumeLayout(false);

            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}