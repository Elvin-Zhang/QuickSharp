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

namespace QuickSharp.BuildTools
{
    /// <summary>
    /// A collection of pinned files.
    /// </summary>
    public class PinnedFilesCollection
    {
        private Dictionary<String, PinnedFile> _pinnedFiles;

        /// <summary>
        /// Create a new pinned file collection.
        /// </summary>
        public PinnedFilesCollection()
        {
            _pinnedFiles = new Dictionary<String, PinnedFile>();
        }

        /// <summary>
        /// Add a file to the collection.
        /// </summary>
        /// <param name="pinnedFile">A PinnedFile instance.</param>
        public void Add(PinnedFile pinnedFile)
        {
            _pinnedFiles[pinnedFile.Directory] = pinnedFile;
        }

        /// <summary>
        /// Add a file to the collection by path.
        /// </summary>
        /// <param name="path">The file path.</param>
        public void Add(string path)
        {
            PinnedFile pinnedFile = new PinnedFile(path);
            Add(pinnedFile);
        }

        /// <summary>
        /// Remove a file from the collection.
        /// </summary>
        /// <param name="pinnedFile">A PinnedFile instance.</param>
        public void Remove(PinnedFile pinnedFile)
        {
            Remove(pinnedFile.Directory);
        }

        /// <summary>
        /// Remove files from the collection by directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        public void Remove(string directory)
        {
            if (_pinnedFiles.ContainsKey(directory))
                _pinnedFiles.Remove(directory);
        }

        /// <summary>
        /// Remove all files from the collection.
        /// </summary>
        public void Clear()
        {
            _pinnedFiles.Clear();
        }

        /// <summary>
        /// Get a pinned file by directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns>A PinnedFile instance or null in not found.</returns>
        public PinnedFile GetPinnedFile(string directory)
        {
            if (!_pinnedFiles.ContainsKey(directory))
                return null;
            else
                return _pinnedFiles[directory];
        }

        /// <summary>
        /// Get all the pinned files in the collection.
        /// </summary>
        /// <returns>A list of file paths.</returns>
        public List<String> GetPaths()
        {
            List<String> paths = new List<String>();

            foreach (PinnedFile pf in _pinnedFiles.Values)
                paths.Add(pf.Path);

            return paths;
        }
    }
}
