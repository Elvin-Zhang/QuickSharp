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
using System.Text;

namespace QuickSharp.Explorer
{
    /// <summary>
    /// A file type filter.
    /// </summary>
    public class FileFilter
    {
        /// <summary>
        /// The filter ID.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The filter name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The file type filter.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Create a copy of the current filter object.
        /// </summary>
        /// <returns></returns>
        public FileFilter Clone()
        {
            FileFilter f = new FileFilter();
            f.ID = ID;
            f.Name = Name;
            f.Filter = Filter;

            return f;
        }

        /// <summary>
        /// Get a string representation of the filter.
        /// </summary>
        /// <returns>An string.</returns>
        public override string ToString()
        {
            string s1 = ID.Replace("|", String.Empty);
            string s2 = Name.Replace("|", String.Empty);
            string s3 = Filter.Replace("|", String.Empty);

            return String.Format("{0}|{1}|{2}", s1, s2, s3);            
        }

        /// <summary>
        /// Get a dictionary of filters from a list of encoded strings.
        /// </summary>
        /// <param name="list">The list of strings.</param>
        /// <returns>A dictionary of FileFilter objects keyed on ID.</returns>
        public static Dictionary<string, FileFilter> GetFileFilters(List<string> list)
        {
            Dictionary<string, FileFilter> dict = new Dictionary<string, FileFilter>();

            foreach (string s in list)
            {
                string[] split = s.Split('|');
                if (split.Length == 3)
                {
                    FileFilter f = new FileFilter();
                    f.ID = split[0];
                    f.Name = split[1];
                    f.Filter = split[2];

                    dict[f.ID] = f;
                }
            }

            return dict;
        }

        /// <summary>
        /// Get a list of encoded strings from a FileFilter dictionary.
        /// </summary>
        /// <param name="filters">The filter dictionary.</param>
        /// <returns>A list of encoded strings.</returns>
        public static List<string> GetFilterList(Dictionary<string, FileFilter> filters)
        {
            List<string> list = new List<string>();

            foreach (string key in filters.Keys)
                list.Add(filters[key].ToString());

            return list;
        }

        /// <summary>
        /// Get a filter from a list of FileFilter objects by its ID.
        /// </summary>
        /// <param name="list">A list of DileFilter objects</param>
        /// <param name="id">The filter ID.</param>
        /// <returns></returns>
        public static FileFilter GetFromList(List<FileFilter> list, string id)
        {
            foreach (FileFilter f in list)
                if (f.ID == id) return f;

            return null;
        }
    }

    /// <summary>
    /// Compare two FileFilter objects by name.
    /// </summary>
    public class FileFilterComparer : IComparer<FileFilter>
    {
        /// <summary>
        /// Compare two FileFilter objects by name.
        /// </summary>
        /// <param name="f1">A FileFilter.</param>
        /// <param name="f2">A FileFilter.</param>
        /// <returns>The result of the name comparison.</returns>
        public int Compare(FileFilter f1, FileFilter f2)
        {
            return f1.Name.CompareTo(f2.Name);
        }
    }
}
