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
using QuickSharp.Core;

namespace QuickSharp.BuildTools
{
    /// <summary>
    /// Represents a pinned file.
    /// </summary>
    public class PinnedFile
    {
        /// <summary>
        /// The file name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The file path.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The directory portion of the file path.
        /// </summary>
        public string Directory { get; private set; }

        /// <summary>
        /// Is the file selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// The pinned file's document type.
        /// </summary>
        public DocumentType DocumentType
        {
            get { return new DocumentType(Path); }
        }

        /// <summary>
        /// A FileInfo object representing the file.
        /// </summary>
        public System.IO.FileInfo FileInfo
        {
            get { return new System.IO.FileInfo(Path); }
        }

        /// <summary>
        /// Create a PinnedFile instance from a path.
        /// </summary>
        /// <param name="path"></param>
        public PinnedFile(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(Path);
            Directory = System.IO.Path.GetDirectoryName(Path);
        }

        /// <summary>
        /// Determine if the file exists.
        /// </summary>
        /// <returns></returns>
        public bool Exists()
        {
            return (System.IO.File.Exists(Path));
        }

        /// <summary>
        /// Match the pinned file with a name.
        /// </summary>
        /// <param name="name">A name.</param>
        /// <returns>True if the name matches the pinned file name.</returns>
        public bool Matches(string name)
        {
            if (String.IsNullOrEmpty(name)) return false;

            return (name == Path);
        }
    }
}
