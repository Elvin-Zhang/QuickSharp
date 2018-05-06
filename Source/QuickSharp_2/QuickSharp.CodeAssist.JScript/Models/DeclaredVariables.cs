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
using System.Reflection;
using System.Text.RegularExpressions;
using QuickSharp.CodeAssist.DotNet;

namespace QuickSharp.CodeAssist.JScript
{
    public class DeclaredVariables
    {
        private List<Variable> _declaredVariables;

        public DeclaredVariables(string source,
            List<string> fullNamespaceList, bool visibleScopesOnly)
        {
            _declaredVariables = new List<Variable>();

            /*
             * Get the local variables declared in the source.
             */

            source = JScriptFormattingTools.RemoveNamespaceDeclarations(source);

            if (visibleScopesOnly)
                source = JScriptFormattingTools.RemoveInaccessibleScopes(source);

            FindVariableDeclaratons(source);

            /*
             * Convert the JScript typenames to their framework classes.
             */

            foreach (Variable v in _declaredVariables)
            {
                // Fixup any language type names
                v.Type = JScriptFormattingTools.ToCTSType(v.Type);
            }
        }

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

        private void FindVariableDeclaratons(string text)
        {
            // Explicity typed declarations (e.g. "x : String;")
            AddTypedMatches(
                @"([\w\d]+)\s*:\s*([\w\.]+)\s*[;=,\)]",
                text, false);

            // Implicitly typed declarations (e.g. "var x = new String()")
            AddTypedMatches(
                @"var\s+([\w\d]+)\s*=\s*new\s+([\w\.]+)\(",
                text, false);

            // Arrays (e.g. "var a : String [] ")
            AddTypedMatches(
                @"([\w\d]+)\s*:\s*([\w\.]+)\s*\[\s*\]",
                text, true);

            // Double-quoted string (e.g. "var s = ""; ");
            AddMatches(@"var\s+([\w\d]+)\s*=\s*\"".*""\s*;", text, "String");

            // Single-quoted string (e.g. "var s = ''; ");
            AddMatches(@"var\s+([\w\d]+)\s*=\s*\'.*\'\s*;", text, "String");

            // Boolean (e.g. "var b = true; ")
            AddMatches(@"var\s+([\w\d]+)\s*=\s*(true|false)\s*;", text, "Boolean");

            // Decimal number
            AddMatches(@"var\s+([\w\d]+)\s*=\s*[\-\+\d\.e]+\s*;", text, "Double");

            _declaredVariables.Sort(CompareVariables);
        }

        private void AddTypedMatches(string regex, string text, bool isArray)
        {
            Regex re = new Regex(regex);
            MatchCollection mc = re.Matches(text);

            foreach (Match m in mc)
            {
                string name = m.Groups[1].Value;
                string type = m.Groups[2].Value;

                // Create the variable
                Variable v = new Variable();
                v.Index = m.Index;
                v.Name = name;
                v.IsArray = isArray;

                if (isArray)
                {
                    v.Type = "System.Array";
                    v.ArrayType = type;
                }
                else
                {
                    v.Type = type;
                }

                _declaredVariables.Add(v);
            }
        }

        private void AddMatches(string regex, string text, string type)
        {
            Regex re = new Regex(regex);
            MatchCollection mc = re.Matches(text);

            foreach (Match m in mc)
            {
                string name = m.Groups[1].Value;

                // Create the variable
                Variable v = new Variable();
                v.Index = m.Index;
                v.Name = name;
                v.IsArray = false;
                v.Type = type;

                _declaredVariables.Add(v);
            }
        }
        
        private int CompareVariables(Variable v1, Variable v2)
        {
            return v2.Index.CompareTo(v1.Index);
        }
    }
}
