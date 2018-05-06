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

namespace QuickSharp.CodeAssist
{
    /// <summary>
    /// Manages the code assist providers.
    /// </summary>
    public class CodeAssistManager
    {
        #region Singleton

        private static CodeAssistManager _singleton;

        /// <summary>
        /// Get a reference to the CodeAssistManager singleton.
        /// </summary>
        /// <returns>A reference to the CodeAssistMananger.</returns>
        public static CodeAssistManager GetInstance()
        {
            if (_singleton == null)
                _singleton = new CodeAssistManager();

            return _singleton;
        }

        #endregion

        private CodeAssistManager()
        {
            _codeAssistProviders = 
                new Dictionary<String, ICodeAssistProvider>();
        }

        /// <summary>
        /// Update the code assist provider status.
        /// </summary>
        public void UpdateCodeAssistStatus()
        {
            ModuleProxy moduleProxy = ModuleProxy.GetInstance();

            if (moduleProxy.Module != null)
                moduleProxy.Module.UpdateCodeAssistStatus();
        }

        /// <summary>
        /// Get the list of lookup items for the current document.
        /// </summary>
        /// <param name="document">The current document.</param>
        /// <returns>A list of lookup items.</returns>
        public LookupList GetLookupList(ScintillaEditForm document)
        {
            if (document == null) return null;
            
            DocumentType documentType = new DocumentType(document.FileName);
            if (documentType == null ||
                String.IsNullOrEmpty(documentType.ToString()))
                return null;

            ICodeAssistProvider provider = GetProvider(documentType);
            if (provider == null) return null;

            return provider.GetLookupList(document);
        }

        #region Code Assist Providers

        private Dictionary<String, ICodeAssistProvider>
            _codeAssistProviders;

        /// <summary>
        /// Register a code assist provider.
        /// </summary>
        /// <param name="provider">A provider instance.</param>
        public void RegisterProvider(ICodeAssistProvider provider)
        {
            _codeAssistProviders[provider.DocumentType.ToString()] = provider;
        }

        /// <summary>
        /// Determine if code assist is available for a document type.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <returns>True if code assist is available.</returns>
        public bool CodeAssistAvailable(DocumentType documentType)
        {
            if (!_codeAssistProviders.ContainsKey(documentType.ToString()))
                return false;

            ICodeAssistProvider provider =
                _codeAssistProviders[documentType.ToString()];

            return provider.IsAvailable;
        }

        /// <summary>
        /// Get the code assist provider for a document type.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <returns>A reference to the registered provider instance.</returns>
        public ICodeAssistProvider GetProvider(DocumentType documentType)
        { 
            if (_codeAssistProviders.ContainsKey(documentType.ToString()))
                return _codeAssistProviders[documentType.ToString()];
            else
                return null;
        }

        #endregion
    }
}
