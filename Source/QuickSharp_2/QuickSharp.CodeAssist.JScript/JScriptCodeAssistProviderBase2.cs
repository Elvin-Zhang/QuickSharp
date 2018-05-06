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
using System.Reflection;
using System.Linq;
using QuickSharp.CodeAssist.DotNet;

namespace QuickSharp.CodeAssist.JScript
{
    public partial class JScriptCodeAssistProviderBase
    {
        #region Type Lookup

        protected List<LookupListItem> GetVisibleTypes(
            string source,
            DeclaredVariables variables,
            bool includeNamespaces,
            bool includeVariables)
        {
            List<LookupListItem> list = new List<LookupListItem>();

            if (includeNamespaces)
            {
                /*
                 * Add all members of declared namespaces.
                 */

                List<LookupListItem> itemList =
                    FindNamespaceTypeLookupItems(
                        GetNamespaceList(source));

                if (itemList != null) list.AddRange(itemList);
            }

            if (includeVariables)
            {
                /*
                 * Add visible declared variables.
                 */

                foreach (Variable v in variables.Items)
                {
                    LookupListItem item = new LookupListItem();
                    item.DisplayText = v.Name;
                    item.InsertText = v.Name;
                    item.Category = QuickSharp.CodeAssist.Constants.FIELD;
                    item.ToolTipText = String.Format("{0} {1}",
                            v.GetVariableType(), v.Name);

                    list.Add(item);
                }
            }

            return list;
        }

        #endregion

        #region Type Member Lookup

        protected List<LookupListItem> GetTypeMembers(
            string source,
            LookupTarget target,
            bool staticLookup,
            bool allowNonPublic)
        {
            /*
             * First we need to fully qualify the target type if it's
             * not already fully qualified. We assume it isn't and
             * test each possible namespace prefix until we find a 
             * valid type. We add a blank namespace to the list to allow
             * for types that are already fully qualified.
             */

            List<String> namespaceList = GetNamespaceList(source);
            namespaceList.Insert(0, String.Empty);

            GetFullyQualifiedTarget(target, namespaceList);

            if (target.Type == null) return null;

            /*
             * We now need to separate the type from its sub-members
             * and work down the chain getting the type of
             * each sub-member until we either get a void return type or
             * reach the end of the list. The last type we get that's
             * not void is the type we want.
             */

            string members = target.FullEntity.Substring(
                target.Type.FullName.Length).TrimStart('.');

            Type type = target.Type;

            if (members != String.Empty)
            {
                string[] split = members.Split('.');

                if (split.Length == 0) return null;

                foreach (string member in split)
                {
                    MemberInfo[] mi = type.GetMember(member);

                    if (mi.Length == 0) return null;

                    staticLookup = false;

                    string typeName = GetMemberTypeName(mi[0]);

                    if (typeName == null) return null;
                    if (typeName == "System.Void") return null;

                    type = SearchAssemblyList(typeName);

                    // If not found try a nested type
                    if (type == null)
                    {
                        typeName = target.Type.FullName + "+" + typeName;
                        type = SearchAssemblyList(typeName);
                    }

                    if (type == null) return null;
                }
            }

            return GetTypeMemberList(
                type, staticLookup, allowNonPublic, true);
        }

        protected List<LookupListItem> GetTypeMemberList(
            Type type,
            bool staticLookup,
            bool allowNonPublic,
            bool allowInherited)
        {
            /*
             * Now we can get the members of the type we've found.
             */

            Dictionary<String, LookupListItem> foundItems =
                new Dictionary<String, LookupListItem>();

            BindingFlags bindingAttr = BindingFlags.Public;

            if (staticLookup)
                bindingAttr |= BindingFlags.Static;
            else
                bindingAttr |= BindingFlags.Instance;

            if (allowNonPublic)
                bindingAttr |= BindingFlags.NonPublic;

            if (allowInherited)
                bindingAttr |= BindingFlags.FlattenHierarchy;

            MemberInfo[] items = type.GetMembers(bindingAttr);

            foreach (MemberInfo item in items)
            {
                if (item.MemberType == MemberTypes.Property)
                {
                    AddProperty(foundItems, item);
                }
                else if (item.MemberType == MemberTypes.Method)
                {
                    // Hide inherited methods for Enums
                    if (type.IsEnum) continue;

                    AddMethod(foundItems, item);
                }
                else if (item.MemberType == MemberTypes.Field)
                {
                    AddField(foundItems, item);
                }
                else if (item.MemberType == MemberTypes.Event)
                {
                    AddEvent(foundItems, item);
                }
            }

            /*
             * Create the lookup item list and add overload
             * count to method tooltips.
             */

            List<LookupListItem> lookupList =
                foundItems.Values.ToList<LookupListItem>();

            foreach (LookupListItem li in lookupList)
            {
                string overloads = String.Empty;

                if (li.MenuItems.Count > 1)
                {
                    overloads = String.Format(" (+{0} {1})",
                        li.MenuItems.Count - 1,
                        li.MenuItems.Count > 2 ? "overloads" : "overload");
                }

                li.ToolTipText =
                    li.ToolTipText.Replace("_OVR_", overloads);
            }

            return lookupList;
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

                BindingFlags bindingAttr =
                    BindingFlags.Public |
                    BindingFlags.FlattenHierarchy |
                    BindingFlags.Instance;

                ConstructorInfo[] cia = type.GetConstructors(bindingAttr);

                foreach (ConstructorInfo ci in cia)
                {
                    LookupMenuItem lmi = new LookupMenuItem();
                    lmi.DisplayText = JScriptFormattingTools.
                            GetConstructorSignature(ci, type);
                    lmi.InsertText = JScriptFormattingTools.
                        GetMinimalConstructorSignature(ci, type);

                    item.MenuItems.Add(lmi);
                }
            }

            return item;
        }

        #endregion
    }
}
