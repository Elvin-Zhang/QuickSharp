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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using QuickSharp.Core;
using QuickSharp.CodeAssist;
using QuickSharp.CodeAssist.DotNet;

namespace QuickSharp.CodeAssist.JScript
{
    public abstract partial class JScriptCodeAssistProviderBase
    {
        protected ApplicationManager applicationManager;
        protected SettingsManager settingsManager;
        protected ReferenceManager referenceManager;
        protected CacheManager cacheManager;

        protected List<string> workspaceAssemblyList;
        protected List<string> fullNamespaceList;
        protected List<string> rootNamespaceList;

        public JScriptCodeAssistProviderBase()
        {
            applicationManager = ApplicationManager.GetInstance();
            settingsManager = SettingsManager.GetInstance();
            referenceManager = ReferenceManager.GetInstance();
            cacheManager = CacheManager.GetInstance();

            workspaceAssemblyList = new List<string>();
            fullNamespaceList = new List<string>();
            rootNamespaceList = new List<string>();
        }

        #region Lists

        protected void UpdateLists()
        {
            /*
             * Update the workspace cache and get the
             * assemblies as a list.
             */

            workspaceAssemblyList.Clear();

            workspaceAssemblyList.AddRange(
                cacheManager.UpdateAssemblyCache());

            /*
             * Build the namespace lists.
             */

            fullNamespaceList.Clear();
            fullNamespaceList.AddRange(referenceManager.FullNamespaceList);
            rootNamespaceList.Clear();
            rootNamespaceList.AddRange(referenceManager.RootNamespaceList);

            foreach (string name in workspaceAssemblyList)
            {
                Assembly assembly = CodeAssistTools.LoadAssembly(name);
                if (assembly == null) continue;

                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        string ns = type.Namespace;
                        if (String.IsNullOrEmpty(ns)) continue;

                        if (!fullNamespaceList.Contains(ns))
                            fullNamespaceList.Add(ns);

                        string[] split = ns.Split('.');

                        if (String.IsNullOrEmpty(split[0])) continue;

                        if (!rootNamespaceList.Contains(split[0]))
                            rootNamespaceList.Add(split[0]);
                    }
                }
                catch
                {
                    // Ignore unloadable assemblies
                }
            }
        }

        /*
         * To prevent the overhead of recomputing the lists
         * allow them to be populated from alreading existing
         * versions.
         */

        protected void CopyLists(
            List<string> workspaceAssemblies,
            List<string> fullNamespaces,
            List<string> rootNamespaces)
        {
            workspaceAssemblyList.Clear();
            workspaceAssemblyList.AddRange(workspaceAssemblies);
            fullNamespaceList.Clear();
            fullNamespaceList.AddRange(fullNamespaces);
            rootNamespaceList.Clear();
            rootNamespaceList.AddRange(rootNamespaces);
        }

        #endregion

        #region Namespace Searches

        protected List<LookupListItem> GetChildNamespaces(
            LookupTarget target, string text)
        {
            string[] split = text.Split(';');
            text = split[split.Length - 1];

            Regex re = new Regex(@"import\s+");
            MatchCollection mc = re.Matches(text);
            if (mc.Count == 0) return null;

            return GetImmediateChildNamespaces(target.Entity);
        }

        protected List<LookupListItem> GetImmediateChildNamespaces(
            string target)
        {
            List<String> foundItems = new List<String>();

            if (target == String.Empty)
            {
                foundItems.AddRange(rootNamespaceList);
            }
            else
            {
                foreach (string name in fullNamespaceList)
                {
                    if (name == target) continue;
                    if (!name.StartsWith(target)) continue;

                    string child = name.Substring(target.Length);
                    if (child[0] != '.') continue;
                    string[] split = child.Split('.');

                    if (!foundItems.Contains(split[1]))
                        foundItems.Add(split[1]);
                }
            }

            List<LookupListItem> namespaces = new List<LookupListItem>();

            foreach (string name in foundItems)
            {
                LookupListItem li = new LookupListItem();
                li.DisplayText = name;
                li.InsertText = name;
                li.ToolTipText = String.Format("namespace {0}", name);
                li.Category = QuickSharp.CodeAssist.Constants.NAMESPACE;
                namespaces.Add(li);
            }

            return namespaces;
        }

        protected List<String> GetNamespaceList(string source)
        {
            List<String> namespaces = new List<String>();

            Regex re = new Regex(@"import\s+([\w\.]*);");
            MatchCollection mc = re.Matches(source);

            foreach (Match m in mc)
                namespaces.Add(m.Groups[1].Value);

            return namespaces;
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

        #region Assembly Search

        protected Type SearchAssemblyList(string targetType)
        {
            if (String.IsNullOrEmpty(targetType)) return null;

            int i = targetType.LastIndexOf('.');
            if (i == -1) return null;

            string ns = targetType.Substring(0, i);

            /*
             * Get all the assemblies containing the target
             * type's namespace.
             */

            List<string> assemblySearchList = new List<string>();

            assemblySearchList.AddRange(
                referenceManager.GetNamespaceAssemblies(ns));

            /*
             * Add all the workspace assemblies (we just search them
             * all as there shouldn't be too many). The list will
             * be empty if we've not called UpdateLists().
             */

            assemblySearchList.AddRange(workspaceAssemblyList);

            foreach (string name in assemblySearchList)
            {
                try
                {
                    Assembly assembly = CodeAssistTools.LoadAssembly(name);
                    if (assembly == null) continue;

                    Type type = assembly.GetType(targetType, false);

                    if (type != null && !type.IsNotPublic)
                        return type;
                }
                catch
                {
                    // Ignore unloadable assemblies
                }
            }

            return null;
        }

        /*
         * Update the target to include the fully
         * qualified type name of the target entity.
         */

        protected void GetFullyQualifiedTarget(
            LookupTarget target, List<string> namespaceList)
        {
            foreach (string ns in namespaceList)
            {
                string fullEntity = ns;
                if (ns != String.Empty) fullEntity += ".";
                fullEntity += target.Entity;

                string[] split = fullEntity.Split('.');

                string type = String.Empty;

                for (int i = 0; i < split.Length; i++)
                {
                    if (i > 0) type += ".";
                    type += split[i];

                    /*
                     * Try a lookahead for nested types. This is a hack
                     * and only works for one level (i.e. just one '+').
                     */

                    if (i < split.Length - 1)
                    {
                        string nestedName = type + "+" + split[i + 1];

                        target.Type = SearchAssemblyList(nestedName);

                        if (target.Type != null)
                        {
                            target.FullEntity = fullEntity;
                            return;
                        }
                    }

                    /*
                     * Get back to looking for 'normal' types.
                     */

                    target.Type = SearchAssemblyList(type);

                    if (target.Type != null)
                    {
                        target.FullEntity = fullEntity;
                        return;
                    }
                }
            }
        }

        /*
         * Get the fullname or name of a member type.
         */

        protected string GetMemberTypeName(MemberInfo member)
        {
            string name = null;

            if (member.MemberType == MemberTypes.Property)
            {
                name = ((PropertyInfo)member).PropertyType.FullName;

                if (name == null)
                    name = ((PropertyInfo)member).PropertyType.Name;

                return name;
            }

            if (member.MemberType == MemberTypes.Method)
            {
                name = ((MethodInfo)member).ReturnType.FullName;

                if (name == null)
                    name = ((MethodInfo)member).ReturnType.Name;

                return name;
            }

            if (member.MemberType == MemberTypes.Field)
            {
                name = ((FieldInfo)member).FieldType.FullName;

                if (name == null)
                    name = ((FieldInfo)member).FieldType.Name;

                return name;
            }

            return null;
        }

        #endregion

        #region Private Helpers

        private void AddProperty(
            Dictionary<String, LookupListItem> foundItems,
            MemberInfo item)
        {
            PropertyInfo pi = (PropertyInfo)item;

            if (!foundItems.ContainsKey(pi.Name))
            {
                LookupListItem li = new LookupListItem();
                li.DisplayText = pi.Name;
                li.InsertText = pi.Name;
                li.Category = QuickSharp.CodeAssist.Constants.PROPERTIES;

                li.ToolTipText = String.Format("{0} {1}",
                    JScriptFormattingTools.GetTypeSignature(pi.PropertyType),
                    pi.Name);

                foundItems.Add(pi.Name, li);
            }
            else
            {
                foundItems[pi.Name].ToolTipText = String.Format("{0} {1}",
                    JScriptFormattingTools.GetTypeSignature(pi.PropertyType),
                    pi.Name);
            }
        }

        private void AddMethod(
            Dictionary<String, LookupListItem> foundItems,
            MemberInfo item)
        {
            MethodInfo mi = (MethodInfo)item;

            if (mi.IsSpecialName) return;
            if (mi.IsPrivate) return;
            if (mi.Name == "Finalize") return;

            if (!foundItems.ContainsKey(mi.Name))
            {
                bool isExtensionMethod = mi.GetCustomAttributes(
                    typeof(ExtensionAttribute), false).Length > 0;

                LookupListItem lli = new LookupListItem();
                lli.DisplayText = mi.Name;
                lli.InsertText = mi.Name;
                lli.ToolTipText = String.Format("{0} {1}_OVR_",
                    JScriptFormattingTools.GetTypeSignature(mi.ReturnType),
                    JScriptFormattingTools.GetMethodSignature(mi));

                if (isExtensionMethod)
                    lli.Category =
                        QuickSharp.CodeAssist.Constants.METHOD_EXTENSION;
                else if (mi.IsPublic)
                    lli.Category =
                        QuickSharp.CodeAssist.Constants.METHOD;
                else if (mi.IsFamily)
                    lli.Category =
                        QuickSharp.CodeAssist.Constants.METHOD_PROTECTED;
                else
                    lli.Category =
                        QuickSharp.CodeAssist.Constants.METHOD_FRIEND;

                LookupMenuItem lmi = new LookupMenuItem();

                lmi.DisplayText = String.Format("{0} {1}",
                    JScriptFormattingTools.GetTypeSignature(mi.ReturnType),
                    JScriptFormattingTools.GetMethodSignature(mi));

                lmi.InsertText =
                    JScriptFormattingTools.GetMinimalMethodSignature(mi);

                lli.MenuItems.Add(lmi);

                foundItems.Add(mi.Name, lli);
            }
            else
            {
                // Overloaded method
                LookupListItem lli = foundItems[mi.Name];
                LookupMenuItem lmi = new LookupMenuItem();

                lmi.DisplayText = String.Format("{0} {1}",
                    JScriptFormattingTools.GetTypeSignature(mi.ReturnType),
                    JScriptFormattingTools.GetMethodSignature(mi));

                lmi.InsertText =
                    JScriptFormattingTools.GetMinimalMethodSignature(mi);

                lli.MenuItems.Add(lmi);
            }
        }

        private void AddField(
            Dictionary<String, LookupListItem> foundItems,
            MemberInfo item)
        {
            FieldInfo fi = (FieldInfo)item;
            
            if (fi.IsPrivate) return;

            if (!foundItems.ContainsKey(fi.Name))
            {
                LookupListItem li = new LookupListItem();
                li.DisplayText = fi.Name;
                li.InsertText = fi.Name;
                li.ToolTipText = String.Format("{0} {1}",
                        JScriptFormattingTools.GetTypeSignature(fi.FieldType),
                        fi.Name);

                if (fi.IsLiteral)
                {
                    if (fi.IsPublic)
                        li.Category =
                            QuickSharp.CodeAssist.Constants.CONSTANT;
                    else
                        li.Category =
                            QuickSharp.CodeAssist.Constants.CONSTANT_FRIEND;
                }
                else
                {
                    if (fi.IsPublic)
                        li.Category =
                            QuickSharp.CodeAssist.Constants.FIELD;
                    else if (fi.IsFamily)
                        li.Category =
                            QuickSharp.CodeAssist.Constants.EXCEPTION_PROTECTED;
                    else if (fi.IsPrivate)
                        li.Category =
                            QuickSharp.CodeAssist.Constants.FIELD_PRIVATE;
                    else
                        li.Category =
                            QuickSharp.CodeAssist.Constants.FIELD_FRIEND;
                }

                foundItems.Add(li.DisplayText, li);
            }
        }

        private void AddEvent(
            Dictionary<String, LookupListItem> foundItems,
            MemberInfo item)
        {
            EventInfo ei = (EventInfo)item;

            if (!foundItems.ContainsKey(ei.Name))
            {
                LookupListItem li = new LookupListItem();
                li.DisplayText = ei.Name;
                li.InsertText = ei.Name;
                li.ToolTipText = ei.EventHandlerType.Name;

                li.Category = QuickSharp.CodeAssist.Constants.EVENT;

                foundItems.Add(li.DisplayText, li);
            }
        }

        #endregion
    }
}
