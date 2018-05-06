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

namespace QuickSharp.CodeAssist.CSharp
{
    public abstract class InheritedVariablesBase : CodeAssistBase
    {
        protected List<Variable> _inheritedVariables;
        private DeclarationContext _declarationContext;

        #region Constructor

        public InheritedVariablesBase(
            string source,
            DeclarationContext declarationContext,
            List<string> workspaceAssemblies,
            List<string> fullNamespaces,
            List<string> rootNamespaces,
            List<string> webConfigNamespaces)
        {
            /*
             * Populate the lists from the exisiting values
             * passed by the constructor.
             */

            CopyLists(
                workspaceAssemblies,
                fullNamespaces,
                rootNamespaces,
                webConfigNamespaces);

            _inheritedVariables = new List<Variable>();
            _declarationContext = declarationContext;

            List<String> baseTypeNames = GetBaseTypes(source);
            List<String> namespaceList = GetNamespaceList(source);
            
            namespaceList.Insert(0, String.Empty);

            foreach (string typeName in baseTypeNames)
            {
                foreach (string ns in namespaceList)
                {
                    string target = ns;
                    if (ns != String.Empty) target += ".";
                    target += typeName;

                    Type type = SearchAssemblyList(target);

                    if (type == null) continue;

                    BindingFlags bindingAttr = 
                        BindingFlags.Public |
                        BindingFlags.FlattenHierarchy |
                        BindingFlags.NonPublic |
                        BindingFlags.Instance |
                        BindingFlags.Static;

                    MemberInfo [] items = type.GetMembers(bindingAttr);

                    foreach (MemberInfo item in items)
                    {
                        if (item.MemberType == MemberTypes.Property)
                        {
                            PropertyInfo pi = (PropertyInfo)item;

                            Variable v = new Variable(
                                pi.Name,
                                pi.PropertyType.FullName,
                                PropertyIsStatic(pi));

                            AddVariable(v);
                        }
                        if (item.MemberType == MemberTypes.Field)
                        {
                            FieldInfo fi = (FieldInfo)item;
                            if (fi.IsPrivate) continue;

                            Variable v = new Variable(
                                fi.Name,
                                fi.FieldType.FullName,
                                fi.IsStatic);

                            AddVariable(v);
                        }
                    }

                    break;
                }
            }
        }

        private bool PropertyIsStatic(PropertyInfo pi)
        {
            MethodInfo getMethod = pi.GetGetMethod();
            if (getMethod != null)
                return getMethod.IsStatic;

            MethodInfo setMethod = pi.GetSetMethod();
            if (setMethod != null)
                return setMethod.IsStatic;

            return false;
        }

        private void AddVariable(Variable v)
        {
            if (_declarationContext == DeclarationContext.Static
                && !v.IsStatic)
                return;

            _inheritedVariables.Add(v);
        }

        #endregion

        public Variable GetVariable(string name)
        {
            string[] split = name.Split('.');
            name = split[0];

            foreach (Variable v in _inheritedVariables)
                if (v.Name == name) return v;

            return null;
        }

        public List<Variable> Items
        {
            get { return _inheritedVariables; }
        }
    }
}
