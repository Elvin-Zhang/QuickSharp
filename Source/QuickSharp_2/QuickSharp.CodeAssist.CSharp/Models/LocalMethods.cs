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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using QuickSharp.CodeAssist.DotNet;

namespace QuickSharp.CodeAssist.CSharp
{
    public class LocalMethods
    {
        #region Class MethodDefinition

        public class MethodDefinition
        {
            public string Name { get; set; }
            public string ReturnType { get; set; }
            public string Parameters { get; set; }
            public string Visibility { get; set; }
            public bool IsProtectedInternal { get; set; }
            public bool IsStatic { get; set; }
            public int StartPos { get; set; }
            public int EndPos { get; set; }
        }

        #endregion

        private List<MethodDefinition> _methods;

        public List<MethodDefinition> Items
        {
            get { return _methods; }
        }

        #region Constructor

        public LocalMethods(string source)
        {
            _methods = new List<MethodDefinition>();

            /*
             * Get all the method declarations in the source.
             */

            Regex methodRegex = new Regex(@"(public\s+|private\s+|protected\s+|internal\s+)(internal\s+)?(abstract\s+|virtual\s+|override\s+|static\s+)?(.+)\s+([\w\d_]+)\s*\(([\d\.\w\s,<>\[\]_]*)\)\s+{");

            foreach (Match m in methodRegex.Matches(source))
            {
                MethodDefinition method = new MethodDefinition();

                method.Name = m.Groups[5].Value.Trim();
                method.ReturnType = m.Groups[4].Value.Trim();
                method.Parameters = m.Groups[6].Value.Trim();
                method.Visibility = m.Groups[1].Value.Trim();
                method.IsProtectedInternal = (m.Groups[2].Value.Trim() == "internal");
                method.IsStatic = (m.Groups[3].Value.Trim() == "static");
                method.StartPos = m.Index;

                method.ReturnType = 
                    CSharpFormattingTools.ToCTSType(method.ReturnType); 

                /*
                 * The parameter types should ideally be formatted to
                 * show the CLR type names to make them consistent with
                 * the display of other types but we'll leave that for
                 * now and make do with the formatted typed by the user.
                 */

                _methods.Add(method);
            }

            /*
             * Now find the constructors.
             */

            Regex constructorRegex = new Regex(@"(public\s+|private\s+|protected\s+|internal\s+|static\s+)(internal\s+)?([\w\d_]+)\s*\(([\d\.\w\s,<>\[\]_]*)\)\s+({|:)");

            foreach (Match m in constructorRegex.Matches(source))
            {
                MethodDefinition method = new MethodDefinition();

                method.Name = m.Groups[3].Value.Trim();
                method.ReturnType = null;
                method.Parameters = m.Groups[4].Value.Trim();
                method.Visibility = m.Groups[1].Value.Trim();
                method.IsProtectedInternal = (m.Groups[2].Value.Trim() == "internal");
                method.IsStatic = (m.Groups[1].Value.Trim() == "static");
                method.StartPos = m.Index;

                _methods.Add(method);
            }

            /*
             * Now we have the methods and their start positions we need
             * to find their end positions to mark out the method 'zones'.
             */

            foreach (MethodDefinition method in _methods)
                method.EndPos = FindMethodEnd(method.StartPos, source);
        }

        #endregion

        #region Helpers

        public MethodDefinition GetMethod(string name)
        {
            foreach (MethodDefinition method in _methods)
                if (method.Name == name)
                    return method;

            return null;
        }

        private string GetParameterNames(MethodDefinition method)
        {
            string parameters = method.Parameters.Trim();
            if (parameters == String.Empty) return String.Empty;

            parameters = CSharpFormattingTools.
                RemoveUnwantedBracketText(parameters);

            parameters = CSharpFormattingTools.
                RemoveUnwantedAngleBracketText(parameters);

            StringBuilder sb = new StringBuilder();

            foreach (string s in parameters.Split(','))
            {
                string p = s.Trim();
                string[] split = p.Split();

                sb.Append(split[split.Length - 1]);
                sb.Append(", ");
            }

            return sb.ToString().Trim().TrimEnd(',');
        }

        private int FindMethodEnd(int start, string text)
        {
            int max = text.Length;
            int i = start;
            int braceLevel = 0;

            // Move to the opening brace
            while (i < max && text[i] != '{') i++;

            while (i < max)
            {
                if (text[i] == '{')
                    braceLevel++;

                if (text[i] == '}')
                    braceLevel--;

                if (braceLevel == 0)
                    break;

                i++;
            }

            return i;

            /* On exhaustion i will be the last index + 1.
             * This is OK as it's only going to be used
             * for location detection, not text indexing.
             */
        }

        #endregion

        #region LookupListItem List

        public List<LookupListItem> GetList(
            DeclarationContext declarationContext)
        {
            Dictionary<String, LookupListItem> dict = 
                new Dictionary<String, LookupListItem>();

            foreach (MethodDefinition method in _methods)
            {
                /*
                 * Hide any dummy methods used intenally.
                 * The convention is that such methods begin
                 * with a '0' which is illegal in C#.
                 */
                 
                if (method.Name[0] == '0') continue;

                /*
                 * Check visibility of methods.
                 */

                bool showMethod = false;

                if (method.IsStatic &&
                    declarationContext != DeclarationContext.Instance)
                    showMethod = true;

                if (!method.IsStatic &&
                    declarationContext != DeclarationContext.Static)
                    showMethod = true;

                if (!showMethod) continue;

                /*
                 * Add the method to the list.
                 */

                if (!dict.ContainsKey(method.Name))
                {
                    LookupListItem listItem = new LookupListItem();
                    listItem.DisplayText = method.Name;

                    if (method.Visibility == "internal" || method.IsProtectedInternal)
                        listItem.Category = QuickSharp.CodeAssist.Constants.METHOD_FRIEND;
                    else if (method.Visibility == "protected")
                        listItem.Category = QuickSharp.CodeAssist.Constants.METHOD_PROTECTED;
                    else if (method.Visibility == "private")
                        listItem.Category = QuickSharp.CodeAssist.Constants.METHOD_PRIVATE;
                    else
                        listItem.Category = QuickSharp.CodeAssist.Constants.METHOD;

                    if (method.ReturnType != null)
                    {
                        listItem.ToolTipText = String.Format("{0} {1}({2})",
                            method.ReturnType, method.Name, method.Parameters);
                    }
                    else
                    {
                        listItem.ToolTipText = String.Format("{0}({1})",
                            method.Name, method.Parameters);
                    }

                    listItem.InsertText = method.Name;

                    LookupMenuItem menuItem = new LookupMenuItem();
                    menuItem.DisplayText = listItem.ToolTipText;

                    menuItem.InsertText = String.Format("{0}({1})",
                        method.Name, GetParameterNames(method));

                    listItem.MenuItems.Add(menuItem);

                    dict[method.Name] = listItem;
                }
                else
                {
                    LookupListItem listItem = dict[method.Name];

                    LookupMenuItem menuItem = new LookupMenuItem();

                    if (method.ReturnType != null)
                    {
                        menuItem.DisplayText = String.Format("{0} {1}({2})",
                            method.ReturnType, method.Name, method.Parameters);
                    }
                    else
                    {
                        menuItem.DisplayText = String.Format("{0}({1})",
                            method.Name, method.Parameters);
                    }

                    menuItem.InsertText = String.Format("{0}({1})",
                        method.Name, method.Parameters);

                    listItem.MenuItems.Add(menuItem);
                }
            }

            /*
             * Convert the dictionary to a list and fixup the
             * overloaded items to show the correct icon and
             * tooltip text.
             */
            
            List<LookupListItem> list = new List<LookupListItem>();

            foreach (LookupListItem listItem in dict.Values)
            {
                if (listItem.MenuItems.Count > 1)
                {
                    switch (listItem.Category)
                    {
                        case QuickSharp.CodeAssist.Constants.METHOD_FRIEND:
                            listItem.Category = QuickSharp.CodeAssist.Constants.METHODOVERLOAD_FRIEND;
                            break;
                        case QuickSharp.CodeAssist.Constants.METHOD_PROTECTED:
                            listItem.Category = QuickSharp.CodeAssist.Constants.METHODOVERLOAD_PROTECTED;
                            break;
                        case QuickSharp.CodeAssist.Constants.METHOD_PRIVATE:
                            listItem.Category = QuickSharp.CodeAssist.Constants.METHODOVERLOAD_PRIVATE;
                            break;
                        default:
                            listItem.Category = QuickSharp.CodeAssist.Constants.METHODOVERLOAD;
                            break;
                    }

                    listItem.ToolTipText = String.Format("{0} (+{1} {2})",
                        listItem.ToolTipText, listItem.MenuItems.Count - 1,
                        listItem.MenuItems.Count == 2 ? "overload" : "overloads");
                }

                list.Add(listItem);
            }

            return list;
        }

        #endregion
    }
}
