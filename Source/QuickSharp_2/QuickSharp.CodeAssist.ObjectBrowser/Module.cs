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
using System.Windows.Forms;
using QuickSharp.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.CodeAssist.ObjectBrowser
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "F16CFA9F-636C-4639-8A51-D5E4BD0486D4";
        }

        public string GetName()
        {
            return "QuickSharp Code Assist Object Browser";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return "Provides the .NET Framework Object Browser.";
        }

        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            deps.Add(new Plugin(QuickSharpPlugins.CodeAssist_DotNet, "QuickSharp.CodeAssist.DotNet", 1));
            return deps;
        }

        public void Activate(MainForm mainForm)
        {
            _mainForm = mainForm;
            ActivatePlugin();
        }

        #endregion

        private MainForm _mainForm;
        private ApplicationManager _applicationManager;
        private ToolStripMenuItem _toolMenuObjectBrowser;
        private ToolStrip _toolbar;
        private bool _useMainToolbar;

        /*
         * Toolbar notes: The original design called for the ObjectBrowser buttons to be
         * added to the main toolbar when the form is created and removed when it is closed.
         * A later enhancement required the option of the buttons being added to a separate
         * toolbar. This presented a problem in that the buttons, belonging as they originally
         * did to the form, are created, managed and destroyed within the form and are
         * inaccessible to the module where they need to be created in order to provide
         * a separate toolbar. The solution is to provide a 'permanent' toolbar with dummy
         * buttons to start with which are then replaced by the form when it is created.
         * The form is passed the 'permanent' toolbar on creation and takes over its
         * management until the form is closed. The toolbar is disabled and re-enabled
         * the next time the form is opened. The original behaviour is preserved
         * by passing a null toolbar to the form at which it uses the main application
         * as before.
         */

        private void ActivatePlugin()
        {
            _applicationManager = ApplicationManager.GetInstance();

            /*
             * Menu setup.
             */

            ToolStripMenuItem toolMenu = _mainForm.GetMenuItemByName(
                QuickSharp.Core.Constants.UI_TOOLS_MENU);

            _toolMenuObjectBrowser = MenuTools.CreateMenuItem(
                Constants.UI_TOOLS_MENU_OBJECT_BROWSER,
                Resources.MainToolsMenuObjectBrowser,
                Resources.ObjectBrowser,
                Keys.Control | Keys.Alt | Keys.J, null,
                UI_TOOLS_MENU_OBJECT_BROWSER_Click,
                true);

            int index = toolMenu.DropDownItems.IndexOfKey(
                QuickSharp.Core.Constants.UI_TOOLS_MENU_OPTIONS);

            /*
             * If menu not found insert at top.
             */

            if (index == -1) index = 0;

            toolMenu.DropDownItems.Insert(index, _toolMenuObjectBrowser);

            /*
             * Separate toolbar setup.
             */

            _useMainToolbar = _applicationManager.ClientProfile.HaveFlag(
                ClientFlags.CodeAssistObjectBrowserUseMainToolbar);

            if (!_useMainToolbar)
            {
                _toolbar = new ToolStrip();
                _toolbar.Name = Constants.UI_OBJECT_BROWSER_TOOLBAR;
                _toolbar.Text = Resources.ToolbarText;

                #region Dummy Buttons

                _toolbar.SuspendLayout();

                _toolbar.Items.Add(MenuTools.CreateToolbarButton(
                    Constants.UI_TOOLBAR_VIEW_MODULES,
                    Resources.OBViewByContainer,
                    Resources.ViewByContainer, null));

                _toolbar.Items.Add(MenuTools.CreateToolbarButton(
                    Constants.UI_TOOLBAR_VIEW_NAMESPACES,
                    Resources.OBViewByNamespace,
                    Resources.ViewByNamespace, null));

                if (_applicationManager.ClientProfile.HaveFlag(
                    ClientFlags.CodeAssistObjectBrowserIncludeWorkspace))
                {
                    _toolbar.Items.Add(MenuTools.CreateToolbarButton(
                        Constants.UI_TOOLBAR_SHOW_WORKSPACE_ONLY,
                        Resources.OBShowWorkspaceOnly,
                        Resources.ShowWorkspaceOnly, null));
                }

                _toolbar.Items.Add(MenuTools.CreateToolbarButton(
                    Constants.UI_TOOLBAR_SHOW_NONPUBLIC,
                    Resources.OBShowNonPublic,
                    Resources.ShowNonPublic, null));

                _toolbar.Items.Add(MenuTools.CreateToolbarButton(
                    Constants.UI_TOOLBAR_SHOW_HIDDEN,
                    Resources.OBShowHidden,
                    Resources.ShowHidden, null));

                _toolbar.Items.Add(MenuTools.CreateToolbarButton(
                    Constants.UI_TOOLBAR_SHOW_INHERITED,
                    Resources.OBShowInherited,
                    Resources.ShowInherited, null));

                _toolbar.Items.Add(MenuTools.CreateToolbarButton(
                    Constants.UI_TOOLBAR_REFRESH_VIEW,
                    Resources.OBRefresh,
                    Resources.RefreshView, null));

                _toolbar.Items.Add(MenuTools.CreateToolbarButton(
                    Constants.UI_TOOLBAR_SHOW_PROPERTIES,
                    Resources.OBShowProperties,
                    Resources.PROPERTIES, null));

                foreach (ToolStripItem item in _toolbar.Items)
                    item.Enabled = false;

                _toolbar.ResumeLayout(true);

                #endregion

                _mainForm.AddDockedToolStrip(_toolbar, 0, 50);
            }

            /*
             * OpenDocument handler registration.
             */

            _applicationManager.RegisterOpenDocumentHandler(
                new DocumentType(Constants.OBJECT_BROWSER_DOCUMENT_TYPE),
                    OpenObjectBrowser);
        }

        private void UI_TOOLS_MENU_OBJECT_BROWSER_Click(
            object sender, EventArgs e)
        {
            _mainForm.LoadDocumentIntoWindow(
                Constants.OBJECT_BROWSER_DOCUMENT_NAME, false);
        }

        #region OpenDocument Handler

        private IDockContent OpenObjectBrowser(string path, bool readOnly)
        {
            /*
             * Arguments are required by the document management system but
             * are ignored here. The ObjectBrowser uses a 'hack' on the 
             * document system to achieve a singleton document type
             * by adopting an internal file name which is always
             * used when trying to open the ObjectBrowser. As the filename
             * is always the same the document is only ever opened once;
             * subsequent requests simply activate the existing document
             * window.
             */

            foreach (Document d in _mainForm.ClientWindow.Documents)
            {
                if (FileTools.MatchPaths(d.FilePath, Constants.OBJECT_BROWSER_DOCUMENT_NAME))
                {
                    d.Activate();
                    return null;
                }
            }

            try
            {
                _mainForm.Refresh();
                _mainForm.Cursor = Cursors.WaitCursor;

                ObjectBrowserForm obf = new ObjectBrowserForm(_toolbar);
                obf.FileName = Constants.OBJECT_BROWSER_DOCUMENT_NAME;
                obf.FilePath = Constants.OBJECT_BROWSER_DOCUMENT_NAME;

                return obf;
            }
            finally
            {
                _mainForm.Cursor = Cursors.Default;
            }
        }

        #endregion
    }
}
