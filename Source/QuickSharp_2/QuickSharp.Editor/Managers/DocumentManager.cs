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
using System.Text;
using System.Collections.Generic;
using QuickSharp.Core;
using ScintillaNet;

namespace QuickSharp.Editor
{
    /// <summary>
    /// Maintains the association between registered document
    /// types and the corresponding Scintilla lexer. A language represents
    /// a specific document type recognised by the Scintilla editor and is mapped on to
    /// an internal Scintilla lexer. The language is represented by a configuration file
    /// with the same name as the language; each different configuration file maps to
    /// a lexer either directly or via an alias. A lexer may be referenced by multiple
    /// languages each with their own specific configuration. The lexer determines how
    /// the editor interprets the structure if the document, the language determines
    /// how the structure is presented (syntax coloring, fold points, etc.).
    /// </summary>
    public class DocumentManager
    {
        #region Singleton

        private static DocumentManager _singleton;

        /// <summary>
        /// Get a reference to the DocumentManager singleton.
        /// </summary>
        /// <returns>A reference to the DocumentManager.</returns>
        public static DocumentManager GetInstance()
        {
            if (_singleton == null)
                _singleton = new DocumentManager();
            
            return _singleton;
        }

        #endregion

        private Dictionary<String, String> _lexerAliasMap;
        private Dictionary<String, String> _documentLanguageMap;
        private Dictionary<String, Encoding> _documentEncodingMap;
        private int _documentNumber;

        /// <summary>
        /// A mapping of lexer aliases to actual Scintilla lexer names.
        /// </summary>
        public Dictionary<String, String> LexerAliasMap
        {
            get { return _lexerAliasMap; }
        }

        private DocumentManager()
        {
            _lexerAliasMap = new Dictionary<String, String>();
            _documentLanguageMap = new Dictionary<String, String>();
            _documentEncodingMap = new Dictionary<String, Encoding>();
            _documentNumber = 1;
        }

        /// <summary>
        /// Register a lexer alias; adds the alias to the LexerAliasMap. Any
        /// existing mapping is overridden.
        /// </summary>
        /// <param name="alias">The lexer alias.</param>
        /// <param name="lexer">A Scintilla lexer name.</param>
        public void RegisterLexerAlias(string alias, string lexer)
        {
            _lexerAliasMap[alias] = lexer;
        }

        /// <summary>
        /// Register a document language. A document type is associated with a
        /// language represented by a Scintilla lexer (or alias). The language
        /// encoding is UTF-8 by default.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <param name="language">A Scintilla language.</param>
        public void RegisterDocumentLanguage(
            DocumentType documentType, string language)
        {
            RegisterDocumentLanguage(documentType, language, Encoding.UTF8);
        }

        /// <summary>
        /// Register a document language. A document type is associated with a
        /// language represented by a Scintilla lexer (or alias). The language
        /// encoding is UTF-8 by default.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <param name="language">A Scintilla language.</param>
        public void RegisterDocumentLanguage(
            string documentType, string language)
        {
            RegisterDocumentLanguage(documentType, language, Encoding.UTF8);
        }

        /// <summary>
        /// Register a document language. A document type is associated with a
        /// language represented by a Scintilla lexer (or alias).
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <param name="language">A Scintilla language.</param>
        /// <param name="encoding">The default encoding for the document type.</param>
        public void RegisterDocumentLanguage(
            DocumentType documentType, string language, Encoding encoding)
        {
            RegisterDocumentLanguage(
                documentType.ToString(), language, encoding);
        }

        /// <summary>
        /// Register a document language. A document type is associated with a
        /// language represented by a Scintilla lexer (or alias).
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <param name="language">A Scintilla language.</param>
        /// <param name="encoding">The default encoding for the document type.</param>
        public void RegisterDocumentLanguage(
            string documentType, string language, Encoding encoding)
        {
            if (String.IsNullOrEmpty(documentType)) return;
            _documentLanguageMap[documentType] = language;
            _documentEncodingMap[documentType] = encoding;
        }

        /// <summary>
        /// Get the language registered for a document type.
        /// </summary>
        /// <param name="documentType">A document type.</param>
        /// <returns>The registered language name or the default if not found.</returns>
        public string GetDocumentLanguage(DocumentType documentType)
        {
            if (documentType == null)
                return Constants.DEFAULT_SCINTILLA_LEXER;

            if (_documentLanguageMap.ContainsKey(
                documentType.ToString()))
                return _documentLanguageMap[documentType.ToString()];
            else
                return Constants.DEFAULT_SCINTILLA_LEXER;
        }

        /// <summary>
        /// Get the encoding of a registered document type.
        /// </summary>
        /// <param name="documentType">A document type.</param>
        /// <returns>The registered encoding or UTF-8 if not found.</returns>
        public Encoding GetDocumentEncoding(DocumentType documentType)
        {
            if (documentType == null)
                return Encoding.UTF8;

            if (_documentEncodingMap.ContainsKey(
                documentType.ToString()))
                return _documentEncodingMap[documentType.ToString()];
            else
                return Encoding.UTF8;
        }

        /// <summary>
        /// Get the base name for the next untitled document (e.g. 'untitled1').
        /// </summary>
        /// <returns>The untitled document name.</returns>
        public string GetNextUntitledFileBasename()
        {
            return String.Format("{0}{1}",
                Constants.UNTITLED_FILENAME,
                _documentNumber++);
        }

        /// <summary>
        /// Get the full file name for the next untitled document (e.g. 'untitled1.txt').
        /// </summary>
        /// <param name="documentType">The document type of the file.</param>
        /// <returns>The untitled document filename.</returns>
        public string GetNextUntitledFileName(DocumentType documentType)
        {
            if (documentType == null)
                throw new Exception("Null document type");

            return String.Format("{0}{1}{2}",
                Constants.UNTITLED_FILENAME,
                _documentNumber++,
                documentType.ToString());
        }

        /*
         * Don't move this to FileTools - has a dependency
         * on ScintillaNet that we don't want in the core.
         */

        /// <summary>
        /// Determine the line ending type used by a text string.
        /// </summary>
        /// <param name="text">The text string.</param>
        /// <returns>The line ending type (e.g. CRLF).</returns>
        public EndOfLineMode DetectLineEnding(String text)
        {
            /*
             * This assumes a uniformly formatted document and is
             * pretty crude as one rogue CRLF in a Unix or Mac
             * formatted doc will override the whole file. However,
             * since this is a Windows app we will give priority
             * to the native EOL format.
             */

            int i = text.IndexOf("\r\n");
            if (i != -1) return EndOfLineMode.Crlf;

            i = text.IndexOf("\n");
            if (i != -1) return EndOfLineMode.LF;

            i = text.IndexOf("\r");
            if (i != -1) return EndOfLineMode.CR;

            return EndOfLineMode.Crlf;
        }
    }
}


