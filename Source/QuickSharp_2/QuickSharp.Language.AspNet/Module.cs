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
using System.IO;
using System.Windows.Forms;
using QuickSharp.Core;
using QuickSharp.Editor;
using QuickSharp.TextEditor;

namespace QuickSharp.Language.AspNet
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "46E231EA-EE38-4208-8893-55377914709A";
        }

        public string GetName()
        {
            return "QuickSharp ASP.NET Language Support";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return "Provides support for ASP.NET document types.";
        }

        public List<Plugin> GetDependencies()
        {
            List<Plugin> deps = new List<Plugin>();
            deps.Add(new Plugin(QuickSharpPlugins.Editor, "QuickSharp.Editor", 1));
            deps.Add(new Plugin(QuickSharpPlugins.TextEditor, "QuickSharp.TextEditor", 1));
            return deps;
        }

        public void Activate(MainForm mainForm)
        {
            _mainForm = mainForm;
            ActivatePlugin();
        }

        #endregion

        private MainForm _mainForm;

        private void ActivatePlugin()
        {
            /*
             * Register the document types.
             */

            DocumentManager documentManager = 
                DocumentManager.GetInstance();

            documentManager.RegisterDocumentLanguage(
                Constants.DOCUMENT_TYPE_ASAX,
                Constants.SCINTILLA_LANGUAGE_ASPX);

            documentManager.RegisterDocumentLanguage(
                Constants.DOCUMENT_TYPE_ASCX,
                Constants.SCINTILLA_LANGUAGE_ASPX);

            documentManager.RegisterDocumentLanguage(
                Constants.DOCUMENT_TYPE_ASHX,
                Constants.SCINTILLA_LANGUAGE_CS);

            documentManager.RegisterDocumentLanguage(
                Constants.DOCUMENT_TYPE_ASMX,
                Constants.SCINTILLA_LANGUAGE_CS);

            documentManager.RegisterDocumentLanguage(
                Constants.DOCUMENT_TYPE_ASPX,
                Constants.SCINTILLA_LANGUAGE_ASPX);

            documentManager.RegisterDocumentLanguage(
                Constants.DOCUMENT_TYPE_MASTER,
                Constants.SCINTILLA_LANGUAGE_ASPX);

            documentManager.RegisterDocumentLanguage(
                Constants.DOCUMENT_TYPE_CONFIG,
                Constants.SCINTILLA_LANGUAGE_XML);

            documentManager.RegisterDocumentLanguage(
                Constants.DOCUMENT_TYPE_SITEMAP,
                Constants.SCINTILLA_LANGUAGE_XML);

            /*
             * Define the document handlers.
             */

            ApplicationManager applicationManager =
                ApplicationManager.GetInstance();

            OpenDocumentHandler openHandler =
                applicationManager.GetOpenDocumentHandler(
                    QuickSharp.TextEditor.Constants.DOCUMENT_TYPE_TXT);

            if (openHandler != null)
            {
                applicationManager.RegisterOpenDocumentHandler(
                    Constants.DOCUMENT_TYPE_ASAX, openHandler);

                applicationManager.RegisterOpenDocumentHandler(
                    Constants.DOCUMENT_TYPE_ASCX, openHandler);

                applicationManager.RegisterOpenDocumentHandler(
                    Constants.DOCUMENT_TYPE_ASHX, openHandler);

                applicationManager.RegisterOpenDocumentHandler(
                    Constants.DOCUMENT_TYPE_ASMX, openHandler);

                applicationManager.RegisterOpenDocumentHandler(
                    Constants.DOCUMENT_TYPE_ASPX, openHandler);

                applicationManager.RegisterOpenDocumentHandler(
                    Constants.DOCUMENT_TYPE_MASTER, openHandler);

                applicationManager.RegisterOpenDocumentHandler(
                    Constants.DOCUMENT_TYPE_CONFIG, openHandler);

                applicationManager.RegisterOpenDocumentHandler(
                    Constants.DOCUMENT_TYPE_SITEMAP, openHandler);
            }

             /*
             * We don't have any build tools but we want ASP.NET files
             * recognised as source files. Register with the
             * non-tool source files list in the application store.
             */

            ApplicationStorage appStore = ApplicationStorage.GetInstance();

            /*
             * We have no dependency on the BuildTools plugin so we
             * can't assume the non-tool source list has been created.
             * Use the Get method to create the list if it doesn't exist.
             */

            List<String> nonToolSourceTypes = appStore.Get(
                Constants.APP_STORE_KEY_NON_TOOL_SOURCE_TYPES,
                typeof(List<String>)) as List<String>;

            if (nonToolSourceTypes != null)
            {
                nonToolSourceTypes.Add(Constants.DOCUMENT_TYPE_ASAX);
                nonToolSourceTypes.Add(Constants.DOCUMENT_TYPE_ASCX);
                nonToolSourceTypes.Add(Constants.DOCUMENT_TYPE_ASHX);
                nonToolSourceTypes.Add(Constants.DOCUMENT_TYPE_ASMX);
                nonToolSourceTypes.Add(Constants.DOCUMENT_TYPE_ASPX);
                nonToolSourceTypes.Add(Constants.DOCUMENT_TYPE_CONFIG);
                nonToolSourceTypes.Add(Constants.DOCUMENT_TYPE_MASTER);
                nonToolSourceTypes.Add(Constants.DOCUMENT_TYPE_SITEMAP);
            }
        }
    }
}
