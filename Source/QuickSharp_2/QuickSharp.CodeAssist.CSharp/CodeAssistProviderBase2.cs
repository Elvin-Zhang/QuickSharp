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
using System.Reflection;
using System.Runtime.CompilerServices;
using QuickSharp.CodeAssist.DotNet;

namespace QuickSharp.CodeAssist.CSharp
{
    public abstract partial class CodeAssistProviderBase
    {
        protected List<LookupListItem> GetTypeMembers(
            LookupContext context, LookupTarget target,
            DeclarationContext declarationContext,
            List<String> namespaceList,
            bool allowNonPublic, bool isInherited)
        {
            #region Part 1: Find the type

            /*
             * To avoid multiple calls to GetNamespaceList in any
             * single lookup we can preload this list or create it here.
             */

            if (namespaceList == null)
            {
                namespaceList = GetNamespaceList(context.PreSource);
                namespaceList.Insert(0, String.Empty);
            }

            /*
             * First we need to fully qualify the target type if it's
             * not already fully qualified. We assume it isn't and
             * test each possible namespace prefix until we find a 
             * valid type. We add a blank namespace to the list to allow
             * for types that are already fully qualified.
             */

            GetFullyQualifiedTarget(target, namespaceList);
            if (target.Type == null) return null;

            if (target.Type.FullName == "System.Void")
                return new List<LookupListItem>();

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

            List<Type> allTypes = null;

            if (members != String.Empty)
            {
                string[] split = members.Split('.');

                if (split.Length == 0) return null;

                foreach (string member in split)
                {
                    MemberInfo[] mi = type.GetMember(member);

                    if (mi.Length == 0)
                    {
                        /*
                         * Member not found; could be an
                         * extension method...
                         */

                        allTypes = FindNamespaceTypes(namespaceList);

                        bool foundMember = false;

                        foreach (Type t in allTypes)
                        {
                            if (!TypeHasExtensionMethods(t)) continue;

                            MethodInfo[] mia = t.GetMethods();

                            if (ExtensionMethodFoundInTypeMembers(
                                mia, member))
                            {
                                mi = mia;
                                foundMember = true;
                            }
                        }

                        if (!foundMember) return null;
                    }

                    /*
                     * We are past the first level members which can
                     * belong to a static or instance item; from here
                     * all sub-members will be in instance contexts.
                     */

                    declarationContext = DeclarationContext.Instance;

                    string typeName = GetMemberTypeName(mi[0]);

                    if (typeName == null) return null;
                    if (typeName == "System.Void") return null;

                    type = SearchAssemblyList(typeName);

                    /*
                     * If not found try a nested type.
                     */

                    if (type == null)
                    {
                        typeName = target.Type.FullName + "+" + typeName;
                        type = SearchAssemblyList(typeName);
                    }

                    /*
                     * Exit if no return type found.
                     */

                    if (type == null) return null;
                }
            }

            #endregion

            #region Part 2: Get the type members

            /*
             * Now we can get the members of the type we've found.
             */

            Dictionary<String, LookupListItem> foundItems =
                new Dictionary<String, LookupListItem>();

            BindingFlags bindingAttr =
                BindingFlags.Public | BindingFlags.FlattenHierarchy;

            /*
             * Use static or instance lookup?
             */

            if (declarationContext == DeclarationContext.Static || type.IsEnum)
                bindingAttr |= BindingFlags.Static;
            else
            {
                bindingAttr |= BindingFlags.Instance;
                if (isInherited) bindingAttr |= BindingFlags.Static;
            }

            /*
             * Include non-public?
             */

            if (allowNonPublic)
                bindingAttr |= BindingFlags.NonPublic;

            MemberInfo[] items = type.GetMembers(bindingAttr);

            foreach (MemberInfo item in items)
            {
                if (item.MemberType == MemberTypes.Property)
                {
                    AddProperty(foundItems, item);
                }
                else if (item.MemberType == MemberTypes.Method)
                {
                    /*
                     * Hide methods for Enums.
                     */

                    if (type.IsEnum) continue;

                    AddMethod(foundItems, item);
                }
                else if (item.MemberType == MemberTypes.Field)
                {
                    AddField(foundItems, item);
                }
                else if (item.MemberType == MemberTypes.Event)
                {
                    AddEvent(foundItems, item,
                        target.Name, context.LineStartPos);
                }
            }

            #endregion
            
            #region Part 3: Find any extension methods for the type
            
            if (allTypes == null)
                allTypes = FindNamespaceTypes(namespaceList);

            foreach (Type t in allTypes)
            {
                if (!TypeHasExtensionMethods(t)) continue;

                MethodInfo[] mia = t.GetMethods();

                foreach (MethodInfo mi in mia)
                {
                    if (!mi.IsStatic) continue;

                    if (mi.GetCustomAttributes(
                        typeof(ExtensionAttribute), false).Length > 0)
                    {
                        ParameterInfo[] pia = mi.GetParameters();
                        if (pia.Length == 0) break;

                        /*
                         * Is the extension applicable to the current type?
                         * We're assuming that the extension type is the
                         * type of the 0-index parameter.
                         */

                        /*
                         * This doesn't appear to be 100% accurate
                         * but it's OK for simple (i.e. non-generic)
                         * extensions. (Might have something to do with
                         * the above assumption?)
                         */

                        if (pia[0].ParameterType.IsAssignableFrom(type))
                            AddExtensionMethod(foundItems, mi);
                    }
                }
            }

            #endregion

            #region Part 4: Update overload information
            
            List<LookupListItem> lookupList =
                foundItems.Values.ToList<LookupListItem>();

            foreach (LookupListItem li in lookupList)
            {
                /*
                 * Update the tooltips to include overload count
                 * or remove overload marker.
                 */

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

            #endregion

            return lookupList;
        }

        #region Private Helpers
        
        private bool TypeHasExtensionMethods(Type t)
        {
            return t.GetCustomAttributes(
                typeof(ExtensionAttribute), false).Length > 0;
        }

        private bool ExtensionMethodFoundInTypeMembers(
            MethodInfo[] mia, string name)
        {
            foreach (MethodInfo mi in mia)
            {
                if (!mi.IsStatic) continue;
                if (mi.Name == name) return true;
            }

            return false;
        }        

        private void AddProperty(
            Dictionary<String, LookupListItem> foundItems,
            MemberInfo memberInfo)
        {
            PropertyInfo pi = (PropertyInfo)memberInfo;

            if (!foundItems.ContainsKey(pi.Name))
            {
                LookupListItem li = new LookupListItem();
                li.DisplayText = pi.Name;
                li.InsertText = pi.Name;
                li.Category = QuickSharp.CodeAssist.Constants.PROPERTIES;

                li.ToolTipText = String.Format("{0} {1}",
                    CSharpFormattingTools.GetTypeSignature(pi.PropertyType),
                    pi.Name);

                foundItems.Add(pi.Name, li);
            }
        }

        private void AddMethod(
            Dictionary<String, LookupListItem> foundItems,
            MemberInfo memberInfo)
        {
            MethodInfo mi = (MethodInfo)memberInfo;

            if (mi.IsSpecialName) return;
            if (mi.IsPrivate) return;
            if (mi.Name == "Finalize") return;

            if (!foundItems.ContainsKey(mi.Name))
            {
                bool isExtensionMethod = 
                    mi.GetCustomAttributes(
                        typeof(ExtensionAttribute),
                        false).Length > 0;
            
                LookupListItem lli = new LookupListItem();
                lli.DisplayText = mi.Name;
                lli.InsertText = mi.Name;
                lli.ToolTipText = String.Format("{0} {1}_OVR_",
                    CSharpFormattingTools.GetTypeSignature(mi.ReturnType),
                    CSharpFormattingTools.GetMethodSignature(mi));

                if (isExtensionMethod)
                    lli.Category = QuickSharp.CodeAssist.Constants.METHOD_EXTENSION;
                else if (mi.IsPublic)
                    lli.Category = QuickSharp.CodeAssist.Constants.METHOD;
                else if (mi.IsFamily)
                    lli.Category = QuickSharp.CodeAssist.Constants.METHOD_PROTECTED;
                else
                    return;

                LookupMenuItem lmi = new LookupMenuItem();

                lmi.DisplayText = String.Format("{0} {1}",
                    CSharpFormattingTools.GetTypeSignature(mi.ReturnType),
                    CSharpFormattingTools.GetMethodSignature(mi));

                lmi.InsertText = CSharpFormattingTools.GetMinimalMethodSignature(mi);

                lli.MenuItems.Add(lmi);

                foundItems.Add(mi.Name, lli);
            }
            else
            {
                // Overloaded method
                LookupListItem lli = foundItems[mi.Name];
                LookupMenuItem lmi = new LookupMenuItem();

                lmi.DisplayText = String.Format("{0} {1}",
                    CSharpFormattingTools.GetTypeSignature(mi.ReturnType),
                    CSharpFormattingTools.GetMethodSignature(mi));

                lmi.InsertText = CSharpFormattingTools.GetMinimalMethodSignature(mi);

                if (mi.IsPublic)
                    lli.Category = QuickSharp.CodeAssist.Constants.METHODOVERLOAD;
                else if (mi.IsFamily)
                    lli.Category = QuickSharp.CodeAssist.Constants.METHODOVERLOAD_PROTECTED;
                else
                    return;

                lli.MenuItems.Add(lmi);
            }
        }

        private void AddField(
            Dictionary<String, LookupListItem> foundItems,
            MemberInfo memberInfo)
        {
            FieldInfo fi = (FieldInfo)memberInfo;
            if (fi.IsSpecialName) return;
            if (fi.IsPrivate) return;

            if (!foundItems.ContainsKey(fi.Name))
            {
                LookupListItem li = new LookupListItem();
                li.DisplayText = fi.Name;
                li.InsertText = fi.Name;
                li.ToolTipText = String.Format("{0} {1}",
                        CSharpFormattingTools.GetTypeSignature(fi.FieldType),
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
                            QuickSharp.CodeAssist.Constants.FIELD_PROTECTED;
                    else
                        return;
                }

                foundItems.Add(li.DisplayText, li);
            }
        }

        private void AddEvent(
            Dictionary<String, LookupListItem> foundItems,
            MemberInfo memberInfo, string targetName, int lineStartPos)
        {
            EventInfo ei = (EventInfo)memberInfo;

            if (!foundItems.ContainsKey(ei.Name))
            {
                LookupListItem li = new LookupListItem();
                li.DisplayText = ei.Name;
                li.InsertText = ei.Name;
                li.ToolTipText = ei.EventHandlerType.Name;

                li.Category = QuickSharp.CodeAssist.Constants.EVENT;

                AddEventHandlerMenuItems(
                    targetName, ei.Name, ei.EventHandlerType,
                    lineStartPos, li);

                foundItems.Add(li.DisplayText, li);
            }
        }

        private void AddEventHandlerMenuItems(
            string targetName, string eventName,
            Type handlerType, int indent,
            LookupListItem item)
        {
            BindingFlags bindingAttr =
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy |
                BindingFlags.Instance;

            MethodInfo[] mia = handlerType.GetMethods(bindingAttr);

            foreach (MethodInfo mi in mia)
            {
                if (mi.Name != "Invoke") continue;

                string paddingFull = new String(' ', indent);
                string paddingShort = new String(' ',
                    (indent > 4 ? indent - 4 : indent));

                string methodParameters =
                    CSharpFormattingTools.GetMethodParameters(
                        mi.GetParameters());

                string methodParameterNames =
                    CSharpFormattingTools.GetMethodParameterNames(
                        mi.GetParameters());

                /*
                 * Full event handler.
                 */

                LookupMenuItem lmi1 = new LookupMenuItem();

                string displayText1 = Resources.InsertEventHandler;

                string insertText1 = String.Format(
                    @"{0} += new {1}({2}_{0});

{4}private void {2}_{0}{3}
{4}{{
{4}}}
",
                    eventName, handlerType.Name,
                    FormatEventHandlerName(targetName),
                    methodParameters, paddingShort);

                lmi1.DisplayText = displayText1;
                lmi1.InsertText = insertText1;
                item.MenuItems.Add(lmi1);

                /*
                 * Anonymous event handler.
                 */

                LookupMenuItem lmi2 = new LookupMenuItem();

                string displayText2 = Resources.InsertAnonEventHandler;

                string insertText2 = String.Format(
                    @"{0} += delegate {1}
{2}{{
{2}}};",
                    eventName, methodParameters, paddingFull);

                lmi2.DisplayText = displayText2;
                lmi2.InsertText = insertText2;
                item.MenuItems.Add(lmi2);

                /*
                 * Lambda event handler.
                 */

                LookupMenuItem lmi3 = new LookupMenuItem();

                string displayText3 = Resources.InsertLambdaEventHandler;

                string insertText3 = String.Format(
                    @"{0} += {1} =>
{2}{{
{2}}};",
                    eventName, methodParameterNames, paddingFull);

                lmi3.DisplayText = displayText3;
                lmi3.InsertText = insertText3;
                item.MenuItems.Add(lmi3);
            }
        }

        private string FormatEventHandlerName(string name)
        {
            if (name.Length < 2) return name;

            name = name.TrimStart('_');
            name = name.Replace('.', '_');
            name = name.Substring(0, 1).ToUpper() + name.Substring(1);

            return name;
        }
        
        private void AddExtensionMethod(
            Dictionary<String, LookupListItem> foundItems, 
            MethodInfo mi)
        {
            if (!foundItems.ContainsKey(mi.Name))
            {
                LookupListItem lli = new LookupListItem();
                lli.DisplayText = mi.Name;
                lli.InsertText = mi.Name;
                lli.ToolTipText = String.Format(
                    "{0} {1}_OVR_\r\nDeclared in: {2}",
                    CSharpFormattingTools.GetTypeSignature(mi.ReturnType),
                    CSharpFormattingTools.GetMethodSignature(mi, true, true),
                    mi.DeclaringType.FullName);

                lli.Category = QuickSharp.CodeAssist.Constants.METHOD_EXTENSION;

                LookupMenuItem lmi = new LookupMenuItem();

                lmi.DisplayText = String.Format("{0} {1}",
                    CSharpFormattingTools.GetTypeSignature(mi.ReturnType),
                    CSharpFormattingTools.GetMethodSignature(mi, true, true));

                lmi.InsertText =
                    CSharpFormattingTools.GetMethodSignature(mi, true, true);

                lli.MenuItems.Add(lmi);

                foundItems.Add(lli.DisplayText, lli);
            }
            else
            {
                // Overloaded methods
                LookupListItem lli = foundItems[mi.Name];
                LookupMenuItem lmi = new LookupMenuItem();

                lmi.DisplayText = String.Format("{0} {1}",
                    CSharpFormattingTools.GetTypeSignature(mi.ReturnType),
                    CSharpFormattingTools.GetMethodSignature(mi, true, true));

                lmi.InsertText =
                    CSharpFormattingTools.GetMethodSignature(mi, true, true);

                lli.MenuItems.Add(lmi);
            }
        }        

        #endregion
    }
}
