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
using QuickSharp.Core;
using QuickSharp.Editor;

namespace QuickSharp.Language.Html
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "56837D0F-6E05-4401-AE1F-1FB20A97650D";
        }

        public string GetName()
        {
            return "QuickSharp Web Language Support";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return "Provides support for HTML and related web file types.";
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
            ActivatePlugin();
        }

        #endregion

        private void ActivatePlugin()
        {
            DocumentType documentTypeHtml =
                new DocumentType(Constants.DOCUMENT_TYPE_HTML);

            DocumentType documentTypeHtm =
                new DocumentType(Constants.DOCUMENT_TYPE_HTM);

            DocumentType documentTypeCss =
                new DocumentType(Constants.DOCUMENT_TYPE_CSS);

            DocumentManager documentManager = 
                DocumentManager.GetInstance();
            
            documentManager.RegisterDocumentLanguage(
                documentTypeHtml, Constants.SCINTILLA_LANGUAGE_HTML);

            documentManager.RegisterDocumentLanguage(
                documentTypeHtm, Constants.SCINTILLA_LANGUAGE_HTML);

            documentManager.RegisterDocumentLanguage(
                documentTypeCss, Constants.SCINTILLA_LANGUAGE_CSS);

            ApplicationManager applicationManager =
                ApplicationManager.GetInstance();

            OpenDocumentHandler openHandler =
                applicationManager.GetOpenDocumentHandler(
                    QuickSharp.TextEditor.Constants.DOCUMENT_TYPE_TXT);

            if (openHandler != null)
            {
                applicationManager.RegisterOpenDocumentHandler(
                    documentTypeHtml, openHandler);

                applicationManager.RegisterOpenDocumentHandler(
                    documentTypeHtm, openHandler);

                applicationManager.RegisterOpenDocumentHandler(
                    documentTypeCss, openHandler);
            }

            /*
             * Add to the non-source tools list (create if necessary).
             */

            ApplicationStorage appStore = ApplicationStorage.GetInstance();

            List<String> nonToolSourceTypes = appStore.Get(
                Constants.APP_STORE_KEY_NON_TOOL_SOURCE_TYPES,
                typeof(List<String>)) as List<String>;

            if (nonToolSourceTypes != null)
            {
                nonToolSourceTypes.Add(Constants.DOCUMENT_TYPE_HTML);
                nonToolSourceTypes.Add(Constants.DOCUMENT_TYPE_HTM);
                nonToolSourceTypes.Add(Constants.DOCUMENT_TYPE_CSS);
            }
        }
    }
}
