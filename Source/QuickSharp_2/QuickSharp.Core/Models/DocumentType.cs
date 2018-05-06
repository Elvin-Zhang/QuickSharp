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
using System.IO;

namespace QuickSharp.Core
{
    /// <summary>
    /// Represents a document in the document management system. A
    /// document type is defined by a file extension and determines the
    /// actions available for a document within an application.
    /// </summary>
    public class DocumentType
    {
        private string _documentType;

        /// <summary>
        /// Create a new DocumentType from a file path.
        /// </summary>
        /// <param name="filePath">File path containing a file extension.</param>
        public DocumentType(string filePath)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                _documentType = Constants.EMPTY_DOCUMENT_TYPE;
                return;
            }

            FileInfo fileInfo = new FileInfo(filePath);
            _documentType = Canonicalize(fileInfo.Extension);
        }

        /// <summary>
        /// Create a new DocumentType from a FileInfo object.
        /// </summary>
        /// <param name="fileInfo">FileInfo containing a file extension.</param>
        public DocumentType(FileInfo fileInfo)
        {
            if (fileInfo == null)
            {
                _documentType = Constants.EMPTY_DOCUMENT_TYPE;
                return;
            }

            _documentType = Canonicalize(fileInfo.Extension);
        }

        /// <summary>
        /// Get a string representation of the DocumentType. This returns
        /// the file extension on which the DocumentType is based.
        /// </summary>
        /// <returns>File extension for the document type.</returns>
        public override string ToString()
        {
            return _documentType;
        }

        /// <summary>
        /// Performs a boolean match between the DocumentType and a string. Returns true
        /// if the string matches the file extension on which the DocumentType
        /// is based. The match is not case sensitive.
        /// </summary>
        /// <param name="text">A file extension.</param>
        /// <returns>True if the extension matches the DocumentType.</returns>
        public bool Matches(string text)
        {
            // Empty string can never match
            if (String.IsNullOrEmpty(text)) return false;
                
            text = text.Trim();
            text = text.ToLower();

            return (_documentType == text);
        }

        /// <summary>
        /// Performs a boolean match between two DocumentTypes.
        /// Returns true if they match.
        /// </summary>
        /// <param name="documentType">A DocumentType.</param>
        /// <returns>True if the DocumentTypes are the same.</returns>
        public bool Matches(DocumentType documentType)
        {
            if (documentType == null) return false;
            return (_documentType == documentType.ToString());
        }

        /// <summary>
        /// Returns true if the DocumentType is empty, i.e. has no
        /// extension. An empty DocumentType is represented as ".".
        /// </summary>
        /// <returns>True if the DocumentType has no extension.</returns>
        public bool IsEmpty()
        {
            return _documentType == Constants.EMPTY_DOCUMENT_TYPE;
        }

        /// <summary>
        /// Performs a string comparison between two DocumentTypes.
        /// Returns 0 if they match.
        /// </summary>
        /// <param name="documentType">A DocumentType.</param>
        /// <returns>Returns 0 if the document types are the same.</returns>
        public int CompareTo(DocumentType documentType)
        {
            return _documentType.CompareTo(documentType.ToString());
        }

        private string Canonicalize(string ext)
        {
            string s = ext.Trim().ToLower();
            if (!s.StartsWith(".")) s = "." + s;
            return s;
        }
    }
}
