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
using System.Text.RegularExpressions;
using QuickSharp.CodeAssist.DotNet;
using System.Text;

namespace QuickSharp.CodeAssist.CSharp
{
    public class DeclaredVariables
    {
        private List<Variable> _declaredVariables;
        private DeclarationContext _declarationContext;

        #region Constructor

        public DeclaredVariables(string source,
            List<string> namespaceList,
            bool visibleScopesOnly,
            DeclarationContext declarationContext)
        {
            _declaredVariables = new List<Variable>();
            _declarationContext = declarationContext;

            source = CSharpFormattingTools.
                RemoveNamespaceDeclarations(source);

            if (visibleScopesOnly)
                source = CSharpFormattingTools.
                    RemoveInaccessibleScopes(source);

            FindVariableDeclaratons(source);

            foreach (Variable v in _declaredVariables)
            {
                /*
                 * To allow for nested types we need to replace any '.'
                 * that's not part of a namespace with '+'.
                 */

                if (v.Type.IndexOf('.') != -1)
                {
                    int pos = 0;

                    /*
                     * Need to replace all '.' occurring
                     * after the longest possible namespace
                     * match.
                     */

                    foreach (string ns in namespaceList)
                        if (v.Type.StartsWith(ns) &&
                            pos <= ns.Length)
                            pos = ns.Length + 1;

                    v.Type = v.Type.Substring(0, pos) +
                        v.Type.Substring(pos).Replace(".", "+"); 
                }
            }
        }

        #endregion

        public Variable GetVariable(string name)
        {
            string[] split = name.Split('.');
            name = split[0];

            foreach (Variable v in _declaredVariables)
                if (v.Name == name) return v;

            return null;
        }

        public List<Variable> Items
        {
            get { return _declaredVariables; }
        }

        #region Find Variables

        private void FindVariableDeclaratons(string text)
        {
            /*
             * Grab everything that looks like variable declaration code
             * and sort into reverse order of location (doesn't matter if we get
             * duplicates as long as the scoping order is preserved).
             */

            string prefix =
                @"(public\s+|private\s+|protected\s+|internal\s+)?" +
                @"(internal\s+)?(static\s+|readonly\s+|const\s+)?" + 
                @"(static\s+|readonly\s+|const\s+)?";

            // Simple declarations (e.g. int x = 0; )
            AddMatches(
                prefix + @"([\w\.]+)\s+([\w\d]+)\s*[,;=\)]",
                text, false);

            // Implicit declarations (e.g. var sb = new StringBuilder())
            AddVarMatches(
                @"var\s+([\w\d]+)\s*=\s*new\s+([\w\.]+)\s*\(",
                text);
            AddVarSpecificMatches(
                "var\\s+([\\w\\d]+)\\s*=\\s*\\\"",
                text, "string");
            AddVarSpecificMatches(
                @"var\s+([\w\d]+)\s*=\s*\'",
                text, "char");
            AddVarSpecificMatches(
                @"var\s+([\w\d]+)\s*=\s*(true|false);",
                text, "bool");
            AddVarSpecificMatches(
                @"var\s+([\w\d]+)\s*=\s*\d+\s*;",
                text, "int");
            AddVarSpecificMatches(
                @"var\s+([\w\d]+)\s*=\s*\d+L\s*;",
                text, "long");
            AddVarSpecificMatches(
                @"var\s+([\w\d]+)\s*=\s*(\+|-)?\d+\.\d+((e|E)?(\+|-)?\d*)?(d|D)?\s*;",
                text, "double");
            AddVarSpecificMatches(
                @"var\s+([\w\d]+)\s*=\s*(\+|-)?\d+\.\d+((e|E)?(\+|-)?\d*)?(f|F)\s*;",
                text, "float");
            AddVarSpecificMatches(
                @"var\s+([\w\d]+)\s*=\s*(\+|-)?\d+\.\d+((e|E)?(\+|-)?\d*)?(m|M)\s*;",
                text, "decimal");

            AddVarGeneric1Matches(
                @"var\s+([\w\d]+)\s*=\s*new\s+([\w\.]+)\s*<\s*([\w\d]+)\s*>\s*\(\)", text);

            AddVarGeneric2Matches(
                @"var\s+([\w\d]+)\s*=\s*new\s+([\w\.]+)\s*<\s*([\w\d]+)\s*,\s*([\w\d]+)\s*>\s*\(\)", text);

            /*
             * Multi declarations (e.g. int i = 0, y, z = 10; ) are not supported.
             */

            // foreach (e.g. foreach(String foo in bar) )
            AddForeachMatches(
                @"foreach\s*\(\s*([\w\.]+)\s+([\w\d]+)\s+in",
                text);

            // arrays (e.g.  String [] foo = blah... )
            AddMatches(
                prefix + @"([\w\.]+)\s*\[\s*\]\s+([\w\d]+)\s*[,;=\)]",
                text, true);

            // Generics (`1) (e.g. List<String> foo = blah... )
            AddGeneric1Matches(
                prefix + @"([\w\.]+)\s*<\s*([\w\d]+)\s*>\s+([\w\d]+)\s*[,;=\)]", text);

            // Generics (`2) (e.g. Dictionary<String, String> foo = blah... )
            AddGeneric2Matches(
                prefix + @"([\w\.]+)\s*<\s*([\w\d]+)\s*,\s*([\w\d]+)\s*>\s+([\w\d]+)\s*[,;=\)]", text);

            /*
             * Sort into reverse location order so that variables
             * in inner scopes mask any similarly named variables
             * in outer scopes.
             */

            _declaredVariables.Sort(CompareVariables);
        }

        private void AddVariable(Variable v)
        {
            if (v.IsLocal)
            {
                _declaredVariables.Add(v);
                return;
            }

            if (_declarationContext == DeclarationContext.Static
                && !v.IsStatic)
                return;

            _declaredVariables.Add(v);
        }

        private void AddMatches(string regex, string text, bool isArray)
        {
            Regex re = new Regex(regex);
            MatchCollection mc = re.Matches(text);

            foreach (Match m in mc)
            {
                bool isLocal = m.Groups[1].Value.Trim() == String.Empty;
                bool isStatic =
                    m.Groups[3].Value.Trim() == "static" ||
                    m.Groups[4].Value.Trim() == "static";
                
                string type = m.Groups[5].Value;
                string name = m.Groups[6].Value;

                // Fix regex leaks...
                if (type == "var") continue;
                if (type == "as") continue;
                if (type == "in") continue;
                if (type == "try") continue;
                if (type == "return") continue;
                if (type == "select") continue;
                if (type == "ref") continue;
                if (type == "out") continue;

                // Create the variable
                Variable v = new Variable(name, m.Index, isLocal, isStatic);
                v.Setup(isArray, type);

                AddVariable(v);
            }
        }

        private void AddVarMatches(string regex, string text)
        {
            Regex re = new Regex(regex);
            MatchCollection mc = re.Matches(text);

            foreach (Match m in mc)
            {
                Variable v = new Variable(
                    m.Groups[1].Value, m.Index, true, false);
                
                v.Setup(false, m.Groups[2].Value);

                AddVariable(v);
            }
        }

        private void AddVarSpecificMatches(
            string regex, string text, string type)
        {
            Regex re = new Regex(regex);
            MatchCollection mc = re.Matches(text);

            foreach (Match m in mc)
            {
                Variable v = new Variable(
                    m.Groups[1].Value, m.Index, true, false);
                
                v.Setup(false, type);

                AddVariable(v);
            }
        }

        private void AddVarGeneric1Matches(string regex, string text)
        {
            Regex re = new Regex(regex);
            MatchCollection mc = re.Matches(text);

            foreach (Match m in mc)
            {
                string type1 = m.Groups[2].Value;
                string type2 = m.Groups[3].Value;
                string name = m.Groups[1].Value;

                Variable v = new Variable(name, m.Index, true, false);
                v.SetupGeneric1(type1, type2);

                AddVariable(v);
            }
        }

        private void AddVarGeneric2Matches(string regex, string text)
        {
            Regex re = new Regex(regex);
            MatchCollection mc = re.Matches(text);

            foreach (Match m in mc)
            {
                string type1 = m.Groups[2].Value;
                string type2 = m.Groups[3].Value;
                string type3 = m.Groups[4].Value;
                string name = m.Groups[1].Value;

                Variable v = new Variable(name, m.Index, true, false);
                v.SetupGeneric2(type1, type2, type3);

                AddVariable(v);
            }
        }

        private void AddForeachMatches(string regex, string text)
        {
            Regex re = new Regex(regex);
            MatchCollection mc = re.Matches(text);

            foreach (Match m in mc)
            {
                string type = m.Groups[1].Value;
                string name = m.Groups[2].Value;

                // Fix regex leaks...
                if (type == "var") continue;
                if (type == "as") continue;
                if (type == "in") continue;
                if (type == "try") continue;
                if (type == "return") continue;
                if (type == "select") continue;

                // Create the variable
                Variable v = new Variable(name, m.Index, true, false);
                v.Setup(false, type);

                AddVariable(v);
            }
        }
        
        private void AddGeneric1Matches(string regex, string text)
        {
            Regex re = new Regex(regex);
            MatchCollection mc = re.Matches(text);

            foreach (Match m in mc)
            {
                bool isLocal = m.Groups[1].Value.Trim() == String.Empty;
                bool isStatic =
                    m.Groups[3].Value.Trim() == "static" ||
                    m.Groups[4].Value.Trim() == "static";

                string type1 = m.Groups[5].Value;
                string type2 = m.Groups[6].Value;
                string name = m.Groups[7].Value;

                Variable v = new Variable(name, m.Index, isLocal, isStatic);
                v.SetupGeneric1(type1, type2);

                AddVariable(v);
            }
        }

        private void AddGeneric2Matches(string regex, string text)
        {
            Regex re = new Regex(regex);
            MatchCollection mc = re.Matches(text);

            foreach (Match m in mc)
            {
                bool isLocal = m.Groups[1].Value.Trim() == String.Empty;
                bool isStatic =
                    m.Groups[3].Value.Trim() == "static" ||
                    m.Groups[4].Value.Trim() == "static";

                string type1 = m.Groups[5].Value;
                string type2 = m.Groups[6].Value;
                string type3 = m.Groups[7].Value;
                string name = m.Groups[8].Value;

                Variable v = new Variable(name, m.Index, isLocal, isStatic);
                v.SetupGeneric2(type1, type2, type3);

                AddVariable(v);
            }
        }

        private int CompareVariables(Variable v1, Variable v2)
        {
            return v2.Index.CompareTo(v1.Index);
        }

        #endregion

        #region LookupListItem List

        private string FormatNestedType(string name)
        {
            return name.Replace("+", ".");
        }

        public List<LookupListItem> GetList(int lineStartPos)
        {
            List<LookupListItem> list =  new List<LookupListItem>();

            foreach (Variable v in _declaredVariables)
            {
                LookupListItem item = new LookupListItem();
                item.DisplayText = v.Name;
                item.InsertText = v.Name;
                item.Category = QuickSharp.CodeAssist.Constants.FIELD;
                item.ToolTipText =
                    String.Format("{0} {1}",
                        FormatNestedType(v.GetVariableType()),
                        v.Name);

                item.MenuItems.Add(
                    GetPropertyMenuItem(v, lineStartPos));

                list.Add(item);
            }

            return list;
        }

        #endregion

        #region Property Insertion

        protected LookupMenuItem GetPropertyMenuItem(
            Variable v, int offset)
        {
            LookupMenuItem lmi1 = new LookupMenuItem();

            string padding = new String(' ', offset);

            string typeName;

            if (v.IsArray || v.IsGeneric)
                typeName = v.GetVariableType();
            else
                typeName = v.DeclaredType;

            string propertyName = v.Name.TrimStart('_');

            if (propertyName.Length > 1)
                propertyName =
                    propertyName.Substring(0, 1).ToUpper() +
                    propertyName.Substring(1);
            else
                propertyName = propertyName.ToUpper();

            string displayText1 = Resources.InsertProperty;

            string insertText1 = String.Format(
                @"public {0} {1}
{3}{{
{3}    get {{ return {2}; }}
{3}    set {{ {2} = value; }}
{3}}}",
                typeName,
                propertyName,
                v.Name,
                padding);

            lmi1.DisplayText = displayText1;
            lmi1.InsertText = insertText1;

            return lmi1;
        }

        #endregion
    }
}
