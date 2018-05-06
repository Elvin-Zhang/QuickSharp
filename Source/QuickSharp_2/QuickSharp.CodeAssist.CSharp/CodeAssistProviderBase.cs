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
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using QuickSharp.Core;
using QuickSharp.CodeAssist.DotNet;

namespace QuickSharp.CodeAssist.CSharp
{
    public abstract partial class CodeAssistProviderBase : CodeAssistBase
    {
        #region Type Lookup

        protected List<LookupListItem> GetVisibleTypes(
            LookupContext context,
            bool includeLocals,
            bool includeNamespaces,
            bool includeVariables)
        {
            /*
             * Get the names of the base types of the current class.
             * This includes and ancestor class and any interfaces
             * implemented by the class.
             */

            List<String> baseTypes = GetBaseTypes(context.PreSource);

            List<String> namespaceList = GetNamespaceList(context.PreSource);
            namespaceList.Insert(0, String.Empty);

            /*
             * We want to find the members inherited from the base
             * class not the interfaces. If there is no base type
             * System.Object is assumed.
             */

            List<LookupListItem> list = new List<LookupListItem>();

            foreach (string baseType in baseTypes)
            {
                LookupTarget target =
                    new LookupTarget(baseType, String.Empty);

                List<LookupListItem> typeList = GetTypeMembers(
                    context, target, context.DeclarationContext,
                    namespaceList, true, true);

                if (typeList != null && !target.Type.IsInterface)
                    list.AddRange(typeList);
            }

            if (list.Count == 0)
            {
                LookupTarget target =
                    new LookupTarget("System.Object", String.Empty);

                List<LookupListItem> typeList = GetTypeMembers(
                    context, target, context.DeclarationContext,
                    namespaceList, true, true);

                if (typeList != null)
                    list.AddRange(typeList);
            }

            if (includeNamespaces)
            {
                /*
                 * Add all members of declared namespaces.
                 */

                List<LookupListItem> itemList =
                    FindNamespaceTypeLookupItems(namespaceList);

                if (itemList != null) list.AddRange(itemList);
            }

            /*
             * Add declared (local) variables.
             */

            if (includeVariables)
                list.AddRange(context.DeclaredVariables.
                    GetList(context.LineStartPos));

            /*
             * Add local methods and properties.
             */

            if (includeLocals)
            {
                list.AddRange(context.Methods.GetList(
                    context.DeclarationContext));

                list.AddRange(context.Properties.GetList(
                    context.DeclarationContext));
            }

            return list;
        }

        #endregion

        #region Namespace Types

        /*
         * We're searching for namespaces so we have to go through
         * each assembly looking at the namespace of each type we're
         * interested in. For efficiency we can look for the members
         * of more than one namespace at a time.
         */

        /*
         * Find the types belonging to a namespace and
         * return as LookupListItems.
         */

        protected List<LookupListItem> FindNamespaceTypeLookupItems(
            string parentNamespace)
        {
            List<string> parentNamespaceList = new List<String>();
            parentNamespaceList.Add(parentNamespace);

            return FindNamespaceTypeLookupItems(parentNamespaceList);
        }

        /*
         * Find the types belonging to a list of namespaces
         * and return as LookupListItems.
         */

        protected List<LookupListItem> FindNamespaceTypeLookupItems(
            List<String> parentNamespaceList)
        {
            List<LookupListItem> itemList = new List<LookupListItem>();

            List<Type> typeList =
                FindNamespaceTypes(parentNamespaceList);

            foreach (Type type in typeList)
                itemList.Add(GetItem(type));

            if (itemList.Count > 0)
                return itemList;
            else
                return null;
        }

        /*
         * Find the types belonging to a namespace.
         */

        protected List<Type> FindNamespaceTypes(
            string parentNamespace)
        {
            List<string> parentNamespaceList = new List<string>();
            parentNamespaceList.Add(parentNamespace);

            return FindNamespaceTypes(parentNamespaceList);
        }

        /*
         * Find the types belonging to a list of namespaces.
         */

        protected List<Type> FindNamespaceTypes(
            List<string> parentNamespaceList)
        {
            List<Type> types = new List<Type>();
            List<string> assemblySearchList = new List<string>();

            /*
             * Get the assemblies containing the namespaces.
             */

            foreach (string ns in parentNamespaceList)
            {
                List<string> assemblyList = new List<string>();

                assemblyList.AddRange(
                    referenceManager.GetNamespaceAssemblies(ns));

                foreach (string assemblyRef in assemblyList)
                    if (!assemblySearchList.Contains(assemblyRef))
                        assemblySearchList.Add(assemblyRef);
            }

            assemblySearchList.AddRange(workspaceAssemblyList);

            /*
             * Get the types.
             */

            foreach (string assemblyRef in assemblySearchList)
            {
                Assembly assembly =
                    CodeAssistTools.LoadAssembly(assemblyRef);

                if (assembly == null) continue;

                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.IsNotPublic) continue;
                        if (!type.IsVisible) continue;
                        if (type.Namespace == null) continue;

                        if (parentNamespaceList.Contains(type.Namespace))
                            types.Add(type);
                    }
                }
                catch
                {
                    // Just skip any problematic assemblies
                }
            }

            return types;
        }

        #endregion

        #region Namespace Namespaces

        /*
         * Find the immediate child namespaces belonging to a namespace.
         */

        protected List<LookupListItem> FindNamespaceChildLookupItems(
            string parentNamespace)
        {
            List<String> foundNamespaces = new List<String>();

            foreach (string ns in fullNamespaceList)
            {
                if (ns == parentNamespace) continue;

                if (ns.StartsWith(parentNamespace))
                {
                    string childNamespace =
                        ns.Substring(parentNamespace.Length);

                    if (childNamespace[0] != '.') continue;

                    string[] split = childNamespace.Split('.');

                    if (String.IsNullOrEmpty(split[1])) continue;

                    if (!foundNamespaces.Contains(split[1]))
                        foundNamespaces.Add(split[1]);
                }
            }

            List<LookupListItem> listItems = new List<LookupListItem>();

            foreach (string ns in foundNamespaces)
            {
                LookupListItem lli = new LookupListItem();
                lli.DisplayText = ns;
                lli.InsertText = ns;
                lli.ToolTipText = String.Format("namespace {0}", ns);
                lli.Category = QuickSharp.CodeAssist.Constants.NAMESPACE;

                listItems.Add(lli);
            }

            if (listItems.Count > 0)
                return listItems;
            else
                return null;
        }

        #endregion

        #region Create LookupListItem

        /*
         * Create a LookupListItem from a type.
         */

        protected LookupListItem GetItem(Type type)
        {
            string typeName = CSharpFormattingTools.GetTypeSignature(type);
            string fullName = CSharpFormattingTools.GetTypeSignature(type, true);

            LookupListItem item = new LookupListItem();
            item.DisplayText = typeName;
            item.InsertText = type.ContainsGenericParameters ?
                CSharpFormattingTools.RemoveGenericSignature(typeName) :
                typeName;

            // Classify the type - order is important here
            if (type.IsClass && type.IsSubclassOf(typeof(System.Delegate)))
            {
                item.Category = QuickSharp.CodeAssist.Constants.DELEGATE;
                item.ToolTipText = String.Format("delegate {0}", fullName);
            }
            else if (type.IsEnum)
            {
                item.Category = QuickSharp.CodeAssist.Constants.ENUM;
                item.ToolTipText = String.Format("enum {0}", fullName);
            }
            else if (type.IsValueType)
            {
                item.Category = QuickSharp.CodeAssist.Constants.VALUETYPE;
                item.ToolTipText = String.Format("struct {0}", fullName);

                AddConstructors(type, item);
            }
            else if (type.IsInterface)
            {
                item.Category = QuickSharp.CodeAssist.Constants.INTERFACE;
                item.ToolTipText = String.Format("interface {0}", fullName);
            }
            else
            {
                if (type.IsSealed)
                    item.Category = QuickSharp.CodeAssist.Constants.CLASS_SEALED;
                else
                    item.Category = QuickSharp.CodeAssist.Constants.CLASS;

                item.ToolTipText = String.Format("class {0}", fullName);

                AddConstructors(type, item);
            }

            return item;
        }

        protected void AddConstructors(Type type, LookupListItem item)
        {
            BindingFlags bindingAttr =
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy |
                BindingFlags.Instance;

            ConstructorInfo[] cia = type.GetConstructors(bindingAttr);

            foreach (ConstructorInfo ci in cia)
            {
                LookupMenuItem lmi = new LookupMenuItem();
                lmi.DisplayText = CSharpFormattingTools.
                    GetConstructorSignature(ci, type);
                lmi.InsertText = CSharpFormattingTools.
                    GetMinimalConstructorSignature(ci, type);

                item.MenuItems.Add(lmi);
            }
        }

        #endregion

        #region Helpers

        /*
         * Get the number of leading spaces in a line.
         */

        protected int GetLineStartPosition(string line)
        {
            int i;

            for (i = 0; i < line.Length; i++)
                if (line[i] != ' ') return i;

            return i;
        }

        #endregion
    }
}
