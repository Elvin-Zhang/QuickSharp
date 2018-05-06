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

namespace QuickSharp.CodeAssist.CSharp
{
    public class LocalProperties
    {
        #region Class PropertyDefinition

        public class PropertyDefinition
        {
            public string Name { get; set; }
            public string ReturnType { get; set; }
            public string Visibility { get; set; }
            public bool IsProtectedInternal { get; set; }
            public bool IsStatic { get; set; }
            public int StartPos { get; set; }
            public int EndPos { get; set; }
        }

        #endregion

        private List<PropertyDefinition> _properties;

        public List<PropertyDefinition> Items
        {
            get { return _properties; }
        }

        #region Constructor

        public LocalProperties(string source)
        {
            _properties = new List<PropertyDefinition>();

            /*
             * Get all the property declarations in the source.
             */

            Regex re = new Regex(@"(public\s+|private\s+|protected\s+|internal\s+)(internal\s+)?(static\s+)?(?!class)(.+)\s+([\w\d_]+)\s*{");

            MatchCollection mc = re.Matches(source);

            foreach (Match m in mc)
            {
                PropertyDefinition property = new PropertyDefinition();

                property.Name = m.Groups[5].Value.Trim();
                property.ReturnType = m.Groups[4].Value.Trim();
                property.Visibility = m.Groups[1].Value.Trim();
                property.IsProtectedInternal = (m.Groups[2].Value.Trim() == "internal");
                property.IsStatic = (m.Groups[3].Value.Trim() == "static");
                property.StartPos = m.Index;

                _properties.Add(property);
            }

            /*
             * Now we have the properties and their start positions we need
             * to find their end positions to mark out the property 'zones'.
             */

            foreach (PropertyDefinition property in _properties)
                property.EndPos = FindPropertyEnd(property.StartPos, source);
        }

        #endregion

        #region Helpers

        public Variable GetProperty(string name)
        {
            string[] split = name.Split('.');
            name = split[0];

            foreach (PropertyDefinition property in _properties)
                if (property.Name == name)
                    return new Variable(
                        property.Name,
                        property.StartPos,
                        property.ReturnType);

            return null;
        }

        private int FindPropertyEnd(int start, string text)
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
            List<LookupListItem> list = new List<LookupListItem>();

            foreach (PropertyDefinition property in _properties)
            {
                /*
                 * Can only see statics in a static context.
                 */

                if (declarationContext == DeclarationContext.Static
                    && !property.IsStatic)
                    continue;

                /*
                 * Add the property to the list.
                 */

                LookupListItem listItem = new LookupListItem();
                listItem.DisplayText = property.Name;

                if (property.Visibility == "internal" || property.IsProtectedInternal)
                    listItem.Category = QuickSharp.CodeAssist.Constants.PROPERTIES_FRIEND;
                else if (property.Visibility == "protected")
                    listItem.Category = QuickSharp.CodeAssist.Constants.PROPERTIES_PROTECTED;
                else if (property.Visibility == "private")
                    listItem.Category = QuickSharp.CodeAssist.Constants.PROPERTIES_PRIVATE;
                else
                    listItem.Category = QuickSharp.CodeAssist.Constants.PROPERTIES;

                listItem.ToolTipText = String.Format("{0} {1}",
                    property.ReturnType, property.Name);

                listItem.InsertText = property.Name;

                list.Add(listItem);
            }

            return list;
        }

        #endregion
    }
}
