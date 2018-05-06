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
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using QuickSharp.Core;
using QuickSharp.CodeAssist.DotNet;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.CodeAssist.ObjectBrowser
{
    public partial class ObjectBrowserForm : Document
    {
        #region UI elements

        private MainForm _mainForm;
        private ToolStrip _toolbar;
        private ToolStripButton _viewModulesButton;
        private ToolStripButton _viewNamespacesButton;
        private ToolStripButton _showWorkspaceOnlyButton;
        private ToolStripButton _showNonPublicButton;
        private ToolStripButton _showHiddenButton;
        private ToolStripButton _showInheritedButton;
        private ToolStripButton _refreshViewButton;
        private ToolStripButton _showPropertiesButton;
        private ToolStripSeparator _toolbarSeparator;
        private bool _showModules;
        private bool _showWorkspaceOnly;
        private bool _hideNonPublicMembers;
        private bool _hideSpecialNames;
        private bool _hideInheritedMembers;
        private bool _hidePropertyGrid;
        private string _treeNodeId;
        private string _treeNodePath;

        #endregion

        private ReferenceManager _referenceManager;
        private Dictionary<string, ModuleData> _modules;
        private List<string> _workspaceAssemblies;
        private bool _includeWorkspace;
        private bool _useMainToolbar;

        #region Nested types

        private class ModuleData
        {
            internal string Name;
            internal string FullName;
            internal bool IsWorkspaceAssembly;
            internal System.Reflection.Module Module;
            internal Dictionary<String, List<TreeNode>> NamespaceNodes;

            internal ModuleData()
            {
                NamespaceNodes = new Dictionary<String, List<TreeNode>>();
            }
        }

        private class MemberTagData
        {
            internal string Name;
            internal string ImageKey;
            internal MemberInfo Info;
        }

        #endregion

        public ObjectBrowserForm(ToolStrip toolbar)
        {
            _includeWorkspace =
                ApplicationManager.GetInstance().ClientProfile.HaveFlag(
                    ClientFlags.CodeAssistObjectBrowserIncludeWorkspace);

            InitializeComponent();

            #region Initialize UI

            _mainForm = ApplicationManager.GetInstance().MainForm;

            if (toolbar == null)
            {
                _useMainToolbar = true;
                _toolbar = _mainForm.MainToolbar;
            }
            else
            {
                _toolbar = toolbar;
                _useMainToolbar = false;
            }
            
            _hideNonPublicMembers = true;
            _hideSpecialNames = true;
            _hideInheritedMembers = true;
            _hidePropertyGrid = true;

            // Make the readonly items nearly black (black will be ignored!)
            propertyGrid.ViewForeColor = Color.FromArgb(0,0,1);
            
            Text = Resources.OBDocumentTitle;
            listView.Columns[0].Width = listView.Width;

            PopulateToolbar();

            /*
             * Apply theme if available.
             */

            ThemeFlags flags = ApplicationManager.GetInstance().
                ClientProfile.ThemeFlags;

            if (flags != null)
            {
                if (flags.ViewAltBackColor != Color.Empty)
                {
                    treeView.BackColor = flags.ViewAltBackColor;
                    listView.BackColor = flags.ViewAltBackColor;
                    propertyGrid.ViewBackColor = flags.ViewAltBackColor;
                }

                if (flags.ViewAltForeColor != Color.Empty)
                {
                    treeView.ForeColor = flags.ViewAltForeColor;
                    listView.ForeColor = flags.ViewAltForeColor;

                    // Don't set to black (shows up as silver).
                    if (flags.ViewAltForeColor != Color.Black)
                        propertyGrid.ViewForeColor = flags.ViewAltForeColor;
                }

                if (flags.MainBackColor != Color.Empty)
                {
                    _splitter.BackColor = flags.MainBackColor;
                    _splitContainer.BackColor = flags.MainBackColor;
                }

                if (flags.ViewShowBorder == false)
                {
                    treeView.BorderStyle = BorderStyle.None;
                    listView.BorderStyle = BorderStyle.None;
                }
            }

            #endregion

            #region ActionState Handlers

            RegisterActionStateHandler(
                Constants.UI_TOOLBAR_VIEW_MODULES, ToolbarButtonState);
            RegisterActionStateHandler(
                Constants.UI_TOOLBAR_VIEW_NAMESPACES, ToolbarButtonState);

            if (_includeWorkspace)
                RegisterActionStateHandler(
                    Constants.UI_TOOLBAR_SHOW_WORKSPACE_ONLY, ToolbarButtonState);

            RegisterActionStateHandler(
                Constants.UI_TOOLBAR_SHOW_NONPUBLIC, ToolbarButtonState);
            RegisterActionStateHandler(
                Constants.UI_TOOLBAR_SHOW_HIDDEN, ToolbarButtonState);
            RegisterActionStateHandler(
                Constants.UI_TOOLBAR_SHOW_INHERITED, ToolbarButtonState);
            RegisterActionStateHandler(
                Constants.UI_TOOLBAR_REFRESH_VIEW, ToolbarButtonState);
            RegisterActionStateHandler(
                Constants.UI_TOOLBAR_SHOW_PROPERTIES, ToolbarButtonState);

            _mainForm.ClientWindow.ActiveDocumentChanged +=
                new EventHandler(_mainForm_ActiveDocumentChanged);

            FormClosed +=
                new FormClosedEventHandler(ObjectBrowserForm_FormClosed);

            #endregion

            _referenceManager = ReferenceManager.GetInstance();
            _modules = new Dictionary<string, ModuleData>();

            LoadModules();
            EnableModuleView(false);
            UpdateTreeView();

            /*
             * Allow client applications to modify the form.
             */

            ObjectBrowserFormProxy.GetInstance().
                UpdateFormControls(Controls);
        }

        private void PopulateToolbar()
        {
            _toolbar.SuspendLayout();

            _toolbarSeparator = MenuTools.CreateSeparator(
                Constants.UI_TOOLBAR_OBJECT_BROWSER_SEP);

            _viewModulesButton = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_VIEW_MODULES,
                Resources.OBViewByContainer,
                Resources.ViewByContainer,
                UI_TOOLBAR_VIEW_MODULES_Click);

            _viewNamespacesButton = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_VIEW_NAMESPACES,
                Resources.OBViewByNamespace,
                Resources.ViewByNamespace,
                UI_TOOLBAR_VIEW_NAMESPACES_Click);

            if (_includeWorkspace)
            {
                _showWorkspaceOnlyButton = MenuTools.CreateToolbarButton(
                    Constants.UI_TOOLBAR_SHOW_WORKSPACE_ONLY,
                    Resources.OBShowWorkspaceOnly,
                    Resources.ShowWorkspaceOnly,
                    UI_TOOLBAR_SHOW_WORKSPACE_ONLY_Click);
            }

            _showNonPublicButton = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_SHOW_NONPUBLIC,
                Resources.OBShowNonPublic,
                Resources.ShowNonPublic,
                UI_TOOLBAR_SHOW_NONPUBLIC_Click);

            _showHiddenButton = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_SHOW_HIDDEN,
                Resources.OBShowHidden,
                Resources.ShowHidden,
                UI_TOOLBAR_SHOW_HIDDEN_Click);

            _showInheritedButton = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_SHOW_INHERITED,
                Resources.OBShowInherited,
                Resources.ShowInherited,
                UI_TOOLBAR_SHOW_INHERITED_Click);

            _refreshViewButton = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_REFRESH_VIEW,
                Resources.OBRefresh,
                Resources.RefreshView,
                UI_TOOLBAR_REFRESH_VIEW_Click);

            _showPropertiesButton = MenuTools.CreateToolbarButton(
                Constants.UI_TOOLBAR_SHOW_PROPERTIES,
                Resources.OBShowProperties,
                Resources.PROPERTIES,
                UI_TOOLBAR_SHOW_PROPERTIES_Click);

            if (!_useMainToolbar)
                _toolbar.Items.Clear();

            if (_useMainToolbar)
                _toolbar.Items.Add(_toolbarSeparator);

            _toolbar.Items.Add(_viewModulesButton);
            _toolbar.Items.Add(_viewNamespacesButton);

            if (_includeWorkspace)
                _toolbar.Items.Add(_showWorkspaceOnlyButton);

            _toolbar.Items.Add(_showNonPublicButton);
            _toolbar.Items.Add(_showHiddenButton);
            _toolbar.Items.Add(_showInheritedButton);
            _toolbar.Items.Add(_refreshViewButton);
            _toolbar.Items.Add(_showPropertiesButton);

            _toolbar.ResumeLayout(true);
        }

        protected override string GetPersistString()
        {
            /*
             * For startup speed reasons don't allow the object browser
             * to be restored from the previous session.
             */

            return null;

            // Here's how to do it if you want to
            //return Constants.OBJECT_BROWSER_DOCUMENT_NAME;
        }

        #region Load modules

        private void LoadModules()
        {
            try
            {
                _mainForm.Cursor = Cursors.WaitCursor;

                _modules.Clear();

                _workspaceAssemblies =
                    CacheManager.GetInstance().UpdateAssemblyCache();

                List<String> assemblyList = new List<String>();

                if (!_showWorkspaceOnly)
                    assemblyList.AddRange(_referenceManager.GetAllAssemblies());
                
                if (_includeWorkspace)
                    assemblyList.AddRange(_workspaceAssemblies);

                foreach (string assemblyName in assemblyList)
                    LoadModuleMembers(assemblyName);
            }
            finally
            {
                _mainForm.Cursor = Cursors.Default;
            }
        }

        private void LoadModuleMembers(string assemblyName)
        {
            Assembly assembly = 
                CodeAssistTools.LoadAssembly(assemblyName);
            
            if (assembly == null) return;

            foreach (System.Reflection.Module module
                in assembly.GetModules())
            {
                string moduleName = module.Name;

                // Remove '.dll'
                if (moduleName.Length > 4)
                    moduleName = moduleName.Substring(
                        0, moduleName.Length - 4);

                bool isWorkspaceAssembly = false;

                if (assemblyName[0] == '@')
                {
                    isWorkspaceAssembly = true;
                }
                else if (assemblyName[0] == '?' && moduleName.Length > 37)
                {
                    // 'LoadFile' context, remove local cache signature.
                    moduleName = moduleName.Substring(37);
                    isWorkspaceAssembly = true;
                }

                ModuleData moduleData = new ModuleData();
                moduleData.Name = moduleName;
                moduleData.FullName = module.FullyQualifiedName;
                moduleData.IsWorkspaceAssembly = isWorkspaceAssembly;
                moduleData.Module = module;

                /*
                 * Gather all the namespaces in the module. As
                 * always we have to do this the hard way by
                 * looking at each type and adding each new
                 * namespace to the list.
                 */

                try
                {
                    foreach (Type type in module.GetTypes())
                    {
                        if (type.IsNested)
                            continue;
                        if (_hideNonPublicMembers && type.IsNotPublic)
                            continue;

                        string namespaceName = type.Namespace;
                        if (String.IsNullOrEmpty(namespaceName))
                            continue;

                        if (!isWorkspaceAssembly &&
                            !_referenceManager.FullNamespaceList.
                                Contains(namespaceName))
                            continue;

                        if (!moduleData.NamespaceNodes.
                            ContainsKey(namespaceName))
                            moduleData.NamespaceNodes[namespaceName] = 
                                new List<TreeNode>();
                    }
                }
                catch
                {
                    // Just skip problematic modules.
                }

                _modules[moduleName] = moduleData;
            }
        }

        #endregion

        #region TreeView Display

        private void EnableModuleView(bool enabled)
        {
            _showModules = enabled;
            _viewModulesButton.Checked = enabled;
            _viewNamespacesButton.Checked = !enabled;
        }

        private void UpdateTreeView()
        {
            try
            {
                _mainForm.Cursor = Cursors.WaitCursor;
                treeView.SuspendLayout();
                treeView.Nodes.Clear();
                listView.Items.Clear();

                if (_showModules)
                    ShowModules();
                else
                    ShowNamespaces();

                treeView.Sort();

                if (treeView.Nodes.Count == 0)
                    treeView.Nodes.Add(new TreeNode(
                        Resources.OBNoAssemblies));
            }
            finally
            {
                treeView.ResumeLayout(true);
                _mainForm.Cursor = Cursors.Default;
            }
        }

        private void ShowModules()
        {
            foreach (ModuleData moduleData in _modules.Values)
            {
                TreeNode moduleNode = new TreeNode(moduleData.Name);
                moduleNode.ImageKey = Constants.ASSEMBLY;
                moduleNode.SelectedImageKey = Constants.ASSEMBLY;
                moduleNode.Tag = moduleData;
                treeView.Nodes.Add(moduleNode);

                foreach (string namespaceName in
                    _modules[moduleData.Name].NamespaceNodes.Keys)
                {
                    TreeNode namespaceNode = new TreeNode(namespaceName);
                    namespaceNode.ImageKey = Constants.NAMESPACE;
                    namespaceNode.SelectedImageKey = Constants.NAMESPACE;
                    namespaceNode.Nodes.Add(new TreeNode());
                    moduleNode.Nodes.Add(namespaceNode);
                }
            }
        }

        private void ShowNamespaces()
        {
            Dictionary<string, TreeNode> namespaceNodes =
                new Dictionary<string, TreeNode>();

            foreach (string moduleName in _modules.Keys)
            {
                foreach (string namespaceName in
                    _modules[moduleName].NamespaceNodes.Keys)
                {
                    if (!namespaceNodes.ContainsKey(namespaceName))
                    {
                        TreeNode namespaceNode = new TreeNode(namespaceName);
                        namespaceNode.ImageKey = Constants.NAMESPACE;
                        namespaceNode.SelectedImageKey = Constants.NAMESPACE;
                        namespaceNodes[namespaceName] = namespaceNode;
                    }
                }
            }

            foreach (string key in namespaceNodes.Keys)
            {
                TreeNode node = namespaceNodes[key];
                node.Nodes.Add(new TreeNode());
                treeView.Nodes.Add(node);
            }
        }

        #endregion

        #region TreeView Events

        void TreeView_BeforeExpand(
            object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.ImageKey == Constants.NAMESPACE)
                GetNamespaceTypes(e.Node);
        }

        private void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGrid.SelectedObject = null;

            if (listView.SelectedItems.Count > 0)
            {
                ListViewItem lvi = listView.SelectedItems[0];
                if (lvi != null && !_hidePropertyGrid)
                    propertyGrid.SelectedObject = lvi.Tag as MemberInfo;
            }
        }

        private void SelectTreeNode()
        {
            TreeNode node = treeView.SelectedNode;
            if (node == null) return;

            _treeNodeId = null;
            listView.Items.Clear();
            propertyGrid.SelectedObject = null;

            // Node data is a type

            Type type = node.Tag as Type;

            if (type != null)
            {
                _treeNodeId = node.Name;
                _treeNodePath = node.FullPath;

                if (!_hidePropertyGrid)
                    propertyGrid.SelectedObject = type;

                try
                {
                    UpdateListView(GetTypeMembers(type));
                }
                catch
                {
                }

                listView.Columns[0].AutoResize(
                    ColumnHeaderAutoResizeStyle.ColumnContent);

                return;
            }

            // Node data is a container

            ModuleData moduleData = node.Tag as ModuleData;

            if (moduleData != null)
            {
                if (!_hidePropertyGrid)
                    propertyGrid.SelectedObject = moduleData.Module;
            }
        }

        private void UpdateListView(List<MemberTagData> memberList)
        {
            listView.SuspendLayout();
            listView.Items.Clear();

            foreach (MemberTagData mtd in memberList)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.SubItems[0].Text = mtd.Name;
                lvi.ImageKey = mtd.ImageKey;
                lvi.Tag = mtd.Info;

                switch (mtd.Info.MemberType)
                {
                    case MemberTypes.Constructor:
                        lvi.ToolTipText = mtd.Name;
                        break;

                    case MemberTypes.Property:
                        PropertyInfo pi = (PropertyInfo)mtd.Info;
                        lvi.ToolTipText = String.Format("{0} {1}",
                            CSharpFormattingTools.GetTypeSignature(pi.PropertyType),
                            pi.Name);
                        break;

                    case MemberTypes.Method:
                        MethodInfo mi = (MethodInfo)mtd.Info;
                        lvi.ToolTipText = String.Format("{0} {1}",
                            CSharpFormattingTools.GetTypeSignature(mi.ReturnType),
                            CSharpFormattingTools.GetMethodSignature(mi));
                        break;

                    case MemberTypes.Field:
                        FieldInfo fi = (FieldInfo)mtd.Info;
                        lvi.ToolTipText = String.Format("{0} {1}",
                            CSharpFormattingTools.GetTypeSignature(fi.FieldType),
                            fi.Name);
                        break;

                    case MemberTypes.Event:
                        EventInfo ei = (EventInfo)mtd.Info;
                        lvi.ToolTipText = ei.EventHandlerType.Name;
                        break;
                }

                listView.Items.Add(lvi);
            }

            listView.ResumeLayout(true);
        }

        #endregion

        #region TreeView Members

        private void GetNamespaceTypes(TreeNode node)
        {
            node.Nodes.Clear();

            // Keep track of found types so we don't add dupes.
            List<string> typeNames = new List<string>();

            /*
             * Go through each assembly containing the namespace
             * and get the types contained within it.
             */

            List<string> assemblyNames = new List<string>();

            /*
             * If using namespace view add all assemblies containing the
             * namespace, otherwise just include the selected assembly.
             */

            if (_showModules)
            {
                if (node.Parent != null)
                {
                    /*
                     * Add the selected assembly.
                     */

                    ModuleData moduleData = 
                        node.Parent.Tag as ModuleData;

                    if (moduleData != null)
                    {
                        Assembly assembly = moduleData.Module.Assembly;

                        if (!moduleData.IsWorkspaceAssembly)
                            assemblyNames.Add(assembly.FullName);
                        else
                            assemblyNames.Add("?" + moduleData.FullName);
                    }
                }
            }
            else
            {
                /*
                 * Add the assemblies that are known to cantain the namespace
                 * and all the local ones (no way of telling if they do).
                 */

                assemblyNames.AddRange(
                    _referenceManager.GetNamespaceAssemblies(node.Text));

                if (_includeWorkspace)
                    assemblyNames.AddRange(_workspaceAssemblies);
            }

            foreach (string name in assemblyNames)
            {
                Assembly assembly = CodeAssistTools.LoadAssembly(name);
                if (assembly == null) continue;

                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (typeNames.Contains(type.FullName))
                            continue;
                        if (type.IsNested)
                            continue;
                        if (_hideNonPublicMembers && type.IsNotPublic)
                            continue;

                        // Reject types not in the namespace we're looking for.
                        if (String.IsNullOrEmpty(type.Namespace))
                            continue;
                        if (type.Namespace != node.Text)
                            continue;

                        string typeName =
                            CSharpFormattingTools.GetTypeSignature(type);

                        TreeNode typeNode = CreateTreeNode(type, typeName);
                        
                        node.Nodes.Add(typeNode);

                        typeNames.Add(type.FullName);

                        AddNestedTypes(type, typeName, node);
                    }
                }
                catch
                {
                    // Just suppress any errors from problematic assemblies.
                }
            }
        }

        private void AddNestedTypes(Type type, string name, TreeNode node)
        {
            Type[] nestedTypes = type.GetNestedTypes();

            if (nestedTypes.Length > 0)
            {
                foreach (Type nestedType in nestedTypes)
                {
                    string typeName = name + '.' + nestedType.Name;
                    
                    TreeNode nestedTypeNode =
                        CreateTreeNode(nestedType, typeName);
                    
                    node.Nodes.Add(nestedTypeNode);

                    AddNestedTypes(nestedType, typeName, node);
                }
            }
        }

        private TreeNode CreateTreeNode(Type type, string name)
        {
            TreeNode node = new TreeNode(name);
            node.Tag = type;
            node.Name = type.GUID.ToString();
            node.SelectedImageKey =
                node.ImageKey = 
                    GetTypeImageKey(type);

            AddBaseTypes(type, node);

            return node;
        }

        private void AddBaseTypes(Type baseType, TreeNode rootNode)
        {
            List<Type> types = new List<Type>();

            if (baseType.BaseType != null)
                types.Add(baseType.BaseType);

            foreach (Type iface in baseType.GetInterfaces())
                if (iface != null)
                    types.Add(iface);

            foreach (Type type in types)
            {
                TreeNode node = new TreeNode();
                node.Text = CSharpFormattingTools.GetTypeSignature(type);
                node.SelectedImageKey = node.ImageKey = GetTypeImageKey(type);
                node.Tag = type;
                node.Name = type.GUID.ToString();

                AddBaseTypes(type, node);

                rootNode.Nodes.Add(node);
            }
        }

        private List<MemberTagData> GetTypeMembers(Type type)
        {
            List<MemberTagData> memberList = new List<MemberTagData>();

            BindingFlags flags =
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.Static;

            if (!_hideNonPublicMembers)
                flags |= BindingFlags.NonPublic;

            if (_hideInheritedMembers)
                flags |= BindingFlags.DeclaredOnly;
            else
                flags |= BindingFlags.FlattenHierarchy;

            MemberInfo[] members = type.GetMembers(flags);

            bool hasExtensionMethods = false;

                hasExtensionMethods = type.GetCustomAttributes(
                    typeof(ExtensionAttribute), false).Length > 0;

            foreach (MemberInfo memberInfo in members)
            {
                MemberTagData mtd = new MemberTagData();
                mtd.Name = memberInfo.Name;
                mtd.Info = memberInfo;

                switch (memberInfo.MemberType)
                {
                    case MemberTypes.NestedType:
                        continue;

                    case MemberTypes.Constructor:
                        ConstructorInfo ci = (ConstructorInfo)memberInfo;
                        /*
                         * Even though constructors are special names
                         * for convenience sake we don't hide them.
                         */
                        //if (_hideSpecialNames && ci.IsSpecialName) continue;
                        mtd.Name = CSharpFormattingTools.GetConstructorSignature(ci, type);
                        mtd.ImageKey = GetMemberImageKey(Constants.METHOD,
                            ci.IsPrivate, ci.IsFamily, ci.IsFamilyAndAssembly);
                        break;

                    case MemberTypes.Method:
                        MethodInfo mi = (MethodInfo)memberInfo;
                        if (_hideSpecialNames && mi.IsSpecialName) continue;

                        bool isExtensionMethod = false;

                        if (hasExtensionMethods)
                        {
                            isExtensionMethod = mi.GetCustomAttributes(
                                typeof(ExtensionAttribute), false).Length > 0;
                        }

                        mtd.Name = CSharpFormattingTools.
                            GetMethodSignature(mi, isExtensionMethod);

                        if (isExtensionMethod)
                            mtd.ImageKey = Constants.METHOD_EXTENSION;
                        else
                            mtd.ImageKey = GetMemberImageKey(Constants.METHOD,
                                mi.IsPrivate, mi.IsFamily, mi.IsFamilyAndAssembly);

                        break;

                    case MemberTypes.Field:
                        FieldInfo fi = (FieldInfo)memberInfo;
                        if (_hideSpecialNames && fi.IsSpecialName) continue;
                        mtd.ImageKey = GetMemberImageKey(
                            (fi.IsLiteral) ? Constants.CONSTANT : Constants.FIELD,
                            fi.IsPrivate, fi.IsFamily, fi.IsFamilyAndAssembly);
                        break;

                    case MemberTypes.Property:
                        mtd.ImageKey = Constants.PROPERTIES;
                        break;

                    case MemberTypes.Event:
                        mtd.ImageKey = Constants.EVENT;
                        break;
                }

                memberList.Add(mtd);
            }

            return memberList;
        }

        private string GetTypeImageKey(Type type)
        {
            string key = String.Empty;

            if (type.IsClass && type.IsSubclassOf(typeof(System.Delegate)))
                key = Constants.DELEGATE;
            else if (type.IsClass)
                key = Constants.CLASS;
            else if (type.IsInterface)
                key = Constants.INTERFACE;
            else if (type.IsEnum)
                key = Constants.ENUM;
            else if (type.IsValueType && !type.IsEnum)
                key = Constants.STRUCTURE;
            else
                key = Constants.VALUETYPE;

            if (type.IsNotPublic)
                key += "_FRIEND";

            return key;
        }

        private string GetMemberImageKey(
            string key, bool isPrivate, bool isProtected, bool isInternal)
        {
            if (isPrivate) return key + "_PRIVATE";
            if (isProtected) return key + "_PROTECTED";
            if (isInternal) return key + "_FRIEND";
            return key;
        }

        #endregion

        #region Menu and Toolbar Event Handlers

        private bool ToolbarButtonState()
        {
            return (_mainForm.ActiveDocument != null &&
                _mainForm.ActiveDocument.FileName ==
                    Constants.OBJECT_BROWSER_DOCUMENT_NAME);
        }

        private void _mainForm_ActiveDocumentChanged(
            object sender, EventArgs e)
        {
            _viewModulesButton.Enabled = IsActionEnabled(
                Constants.UI_TOOLBAR_VIEW_MODULES);
            _viewNamespacesButton.Enabled = IsActionEnabled(
                Constants.UI_TOOLBAR_VIEW_NAMESPACES);

            if (_includeWorkspace)
                _showWorkspaceOnlyButton.Enabled = IsActionEnabled(
                    Constants.UI_TOOLBAR_SHOW_WORKSPACE_ONLY);

            _showNonPublicButton.Enabled = IsActionEnabled(
                Constants.UI_TOOLBAR_SHOW_NONPUBLIC);
            _showHiddenButton.Enabled = IsActionEnabled(
                Constants.UI_TOOLBAR_SHOW_HIDDEN);
            _showInheritedButton.Enabled = IsActionEnabled(
                Constants.UI_TOOLBAR_SHOW_INHERITED);
            _refreshViewButton.Enabled = IsActionEnabled(
                Constants.UI_TOOLBAR_REFRESH_VIEW);
            _showPropertiesButton.Enabled = IsActionEnabled(
                Constants.UI_TOOLBAR_SHOW_PROPERTIES);
        }

        private void ObjectBrowserForm_FormClosed(
            object sender, EventArgs e)
        {
            if (_useMainToolbar)
            {
                _toolbar.Items.Remove(_toolbarSeparator);
                _toolbar.Items.Remove(_viewModulesButton);
                _toolbar.Items.Remove(_viewNamespacesButton);

                if (_includeWorkspace)
                    _toolbar.Items.Remove(_showWorkspaceOnlyButton);

                _toolbar.Items.Remove(_showNonPublicButton);
                _toolbar.Items.Remove(_showHiddenButton);
                _toolbar.Items.Remove(_showInheritedButton);
                _toolbar.Items.Remove(_refreshViewButton);
                _toolbar.Items.Remove(_showPropertiesButton);
            }
            else
            {
                _viewModulesButton.Enabled = false;
                _viewModulesButton.Checked = false;
                _viewNamespacesButton.Enabled = false;
                _viewNamespacesButton.Checked = false;

                if (_includeWorkspace)
                {
                    _showWorkspaceOnlyButton.Enabled = false;
                    _showWorkspaceOnlyButton.Checked = false;
                }

                _showNonPublicButton.Enabled = false;
                _showNonPublicButton.Checked = false;
                _showHiddenButton.Enabled = false;
                _showHiddenButton.Checked = false;
                _showInheritedButton.Enabled = false;
                _showInheritedButton.Checked = false;
                _refreshViewButton.Enabled = false;
                _showPropertiesButton.Enabled = false;
                _showPropertiesButton.Checked = false;
            }
        }

        private void UI_TOOLBAR_VIEW_MODULES_Click(
            object sender, EventArgs e)
        {
            EnableModuleView(true);
            UpdateTreeView();
        }

        private void UI_TOOLBAR_VIEW_NAMESPACES_Click(
            object sender, EventArgs e)
        {
            EnableModuleView(false);
            UpdateTreeView();
        }

        private void UI_TOOLBAR_SHOW_WORKSPACE_ONLY_Click(
            object sender, EventArgs e)
        {
            _showWorkspaceOnlyButton.Checked = !_showWorkspaceOnlyButton.Checked;
            _showWorkspaceOnly = !_showWorkspaceOnly;
            Reload();
        }

        private void UI_TOOLBAR_SHOW_NONPUBLIC_Click(
            object sender, EventArgs e)
        { 
            _showNonPublicButton.Checked = !_showNonPublicButton.Checked;
            _hideNonPublicMembers = !_showNonPublicButton.Checked;
        }

        private void UI_TOOLBAR_SHOW_HIDDEN_Click(
            object sender, EventArgs e)
        {
            _showHiddenButton.Checked = !_showHiddenButton.Checked;
            _hideSpecialNames = !_showHiddenButton.Checked;
        }

        private void UI_TOOLBAR_SHOW_INHERITED_Click(
            object sender, EventArgs e)
        {
            _showInheritedButton.Checked = !_showInheritedButton.Checked;
            _hideInheritedMembers = !_showInheritedButton.Checked;
        }

        private void UI_TOOLBAR_REFRESH_VIEW_Click(
            object sender, EventArgs e)
        {
            Reload();
        }

        private void Reload()
        {
            LoadModules();
            EnableModuleView(_showModules);
            UpdateTreeView();
        }

        private void UI_TOOLBAR_SHOW_PROPERTIES_Click(
            object sender, EventArgs e)
        {
            TogglePropertyGrid();
        }

        #endregion

        #region Property Grid

        public void TogglePropertyGrid()
        {
            _showPropertiesButton.Checked = !_showPropertiesButton.Checked;
            _hidePropertyGrid = !_showPropertiesButton.Checked;

            if (_hidePropertyGrid)
            {
                propertyGrid.SelectedObject = null;
                _splitContainer.Panel2Collapsed = true;
            }
            else
            {
                _splitContainer.Panel2Collapsed = false;
            }
        }

        #endregion
    }
}
