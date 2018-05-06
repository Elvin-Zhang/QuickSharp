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

using System.Collections.Generic;
using System.IO;

namespace QuickSharp.CodeAssist
{
    public class EmbeddedOptionHelper
    {
        public static LookupList GetEmbbededOptionFileList(string text)
        {
            /*
             * Not valid option if less than 3 chars.
             */

            if (text.Length < 3) return null;

            /*
             * Get the option.
             */

            List<LookupListItem> list;

            bool quickSharpOption = (text[2] == '&');

            text = text.Substring(3);
            string[] split = text.Split();
            string lookahead = split[split.Length - 1];

            if (quickSharpOption)
                list = GetQuickSharpOptionsList();
            else
            {
                /*
                 * Adjust lookahead for compiler option prefix.
                 */

                string[] split2 = lookahead.Split(':');
                lookahead = split2[split2.Length - 1];

                /*
                 * List DLLs only if compiler reference otherwise
                 * list all files.
                 */

                if (split2[0] == "/r" || split2[0] == "/reference")
                    list = GetFileList("*.dll");
                else
                    list = GetFileList("*.*");
            }

            return new LookupList(lookahead, list);
        }

        #region QuickSharp Options

        private static List<LookupListItem> GetQuickSharpOptionsList()
        {
            string[] options =
            {
                "RunInOwnWindow",
                "DoPreCompile",
                "DoPostCompile",
                "DoPreRun",
                "DoPostRun"
            };

            List<LookupListItem> list = new List<LookupListItem>();

            foreach (string option in options)
            {
                LookupListItem item = new LookupListItem();
                item.DisplayText =
                    item.InsertText =
                        item.ToolTipText = option;

                item.Category = Constants.PROPERTIES;
                list.Add(item);
            }

            return list;
        }

        #endregion

        #region File List

        private static List<LookupListItem> GetFileList(string pattern)
        {
            List<LookupListItem> list = new List<LookupListItem>();

            string workspace = Directory.GetCurrentDirectory();

            foreach (string file in Directory.GetFiles(workspace, pattern))
            {
                string filename = Path.GetFileName(file);

                LookupListItem item = new LookupListItem();
                item.DisplayText = filename;
                item.InsertText = filename;
                item.Category = Constants.FILE_REF;

                list.Add(item);
            }

            return list;
        }

        #endregion
    }
}
