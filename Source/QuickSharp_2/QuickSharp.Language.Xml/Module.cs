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

namespace QuickSharp.Language.Xml
{
    public class Module : IQuickSharpPlugin
    {
        #region IQuickSharpPlugin Members

        public string GetID()
        {
            return "6E202484-6906-4A9C-92BA-10932CAA5BB3";
        }

        public string GetName()
        {
            return "QuickSharp XML Language Support";
        }

        public int GetVersion()
        {
            return 1;
        }

        public string GetDescription()
        {
            return "Provides support for XML and XML-based files.";
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
            DocumentType documentTypeXML =
                new DocumentType(Constants.DOCUMENT_TYPE_XML);

            DocumentType documentTypeXAML =
                new DocumentType(Constants.DOCUMENT_TYPE_XAML);

            DocumentManager documentManager = 
                DocumentManager.GetInstance();
            
            documentManager.RegisterDocumentLanguage(
                documentTypeXML, Constants.SCINTILLA_LANGUAGE_XML);

            documentManager.RegisterDocumentLanguage(
                documentTypeXAML, Constants.SCINTILLA_LANGUAGE_XML);
            
            ApplicationManager applicationManager =
                ApplicationManager.GetInstance();

            OpenDocumentHandler openHandler =
                applicationManager.GetOpenDocumentHandler(
                    QuickSharp.TextEditor.Constants.DOCUMENT_TYPE_TXT);

            if (openHandler != null)
            {
                applicationManager.RegisterOpenDocumentHandler(
                    documentTypeXML, openHandler);

                applicationManager.RegisterOpenDocumentHandler(
                    documentTypeXAML, openHandler);
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
                nonToolSourceTypes.Add(Constants.DOCUMENT_TYPE_XML);
                nonToolSourceTypes.Add(Constants.DOCUMENT_TYPE_XAML);
            }
        }
    }
}
