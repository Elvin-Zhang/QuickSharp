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
using QuickSharp.Core;
using QuickSharp.CodeAssist.DotNet;

namespace QuickSharp.CodeAssist.CSharp
{
    public abstract class CodeAssistBase
    {
        protected ApplicationManager applicationManager;
        protected SettingsManager settingsManager;
        protected ReferenceManager referenceManager;
        protected CacheManager cacheManager;
        protected List<string> workspaceAssemblyList;
        protected List<string> fullNamespaceList;
        protected List<string> rootNamespaceList;
        protected List<string> configNamespaceList;

        public CodeAssistBase()
        {
            applicationManager = ApplicationManager.GetInstance();
            settingsManager = SettingsManager.GetInstance();
            referenceManager = ReferenceManager.GetInstance();
            cacheManager = CacheManager.GetInstance();
            workspaceAssemblyList = new List<string>();
            fullNamespaceList = new List<string>();
            rootNamespaceList = new List<string>();
            configNamespaceList = new List<string>();
        }

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
         * allow them to be populated from alreading existing versions.
         */

        protected void CopyLists(
            List<string> workspaceAssemblies,
            List<string> fullNamespaces,
            List<string> rootNamespaces,
            List<string> webConfigNamespaces)
        {
            workspaceAssemblyList.Clear();
            workspaceAssemblyList.AddRange(workspaceAssemblies);
            fullNamespaceList.Clear();
            fullNamespaceList.AddRange(fullNamespaces);
            rootNamespaceList.Clear();
            rootNamespaceList.AddRange(rootNamespaces);
            configNamespaceList.Clear();
            configNamespaceList.AddRange(webConfigNamespaces);
        }

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

        #endregion

        #region Namespace Searches

        /*
         * Use the namespace list to create each possible fully
         * qualified name for the target and test each one by
         * seeing if the type really exists. We have to do this
         * because C# allows unqualified type names which are
         * inferred from the 'using' declarations in the source.
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
         * Get the immediate child namespaces for a 'using' declaration
         */

        protected List<LookupListItem> GetChildNamespaceList(
            LookupTarget target, string text)
        {
            string[] split = text.Split(';');
            text = split[split.Length - 1];

            // Is it a using declaration...
            Regex re1 = new Regex(@"using\s+");
            MatchCollection mc = re1.Matches(text);
            if (mc.Count == 0) return null;

            // ...but not a using statement.
            Regex re2 = new Regex(@"using\s*\(");
            Match m = re2.Match(text);
            if (m.Success) return null;

            // Get the namespaces
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

        /*
         * Get namespaces either for web or code pages.
         */

        protected abstract List<String> GetNamespaceList(string source);

        /*
         * Get a list of all the namespaces declared in the
         * source including the namespace the current class
         * belongs to.
         */

        protected List<String> GetCodeDeclaredNamespaceList(string source)
        {
            List<String> namespaces = new List<String>();

            Regex re = new Regex(@"using\s+(\w+\s*=)?\s*([\w\.]*);");
            MatchCollection mc = re.Matches(source);

            foreach (Match m in mc)
                namespaces.Add(m.Groups[2].Value);

            string localNamespace = GetLocalNamespace(source);

            if (!String.IsNullOrEmpty(localNamespace) &&
                !namespaces.Contains(localNamespace))
                namespaces.Add(localNamespace);

            return namespaces;
        }

        /*
         * Get namespaces declared in a web page.
         */

        protected List<String> GetWebDeclaredNamespaceList(string source)
        {
            /*
             * Add the standard namespaces. This is arbitrary
             * as the default list can easily be changed in
             * the root web.config.
             */

            List<String> namespaces = new List<String>();

            if (!applicationManager.ClientProfile.HaveFlag(
                ClientFlags.CodeAssistDotNetDisableDefaultWebNamespaces))
            {
                namespaces.Add("System");
                namespaces.Add("System.Collections");
                namespaces.Add("System.Collections.Specialized");
                namespaces.Add("System.Configuration");
                namespaces.Add("System.Text");
                namespaces.Add("System.Text.RegularExpressions");
                namespaces.Add("System.Web");
                namespaces.Add("System.Web.Caching");
                namespaces.Add("System.Web.SessionState");
                namespaces.Add("System.Web.Security");
                namespaces.Add("System.Web.Profile");
                namespaces.Add("System.Web.UI");
                namespaces.Add("System.Web.UI.WebControls");
                namespaces.Add("System.Web.UI.WebControls.WebParts");
                namespaces.Add("System.Web.UI.HtmlControls");
            }

            /*
             * Now add any declared in the page.
             */

            Regex re = new Regex("(?i)<%@\\s+import\\s+namespace\\s*=\\s*\\\"([\\w\\.]+)\\\"\\s*%>");
            foreach (Match m in re.Matches(source))
                namespaces.Add(m.Groups[1].Value);

            return namespaces;
        }

        /*
         * Get the current enclosing namespace.
         */

        private string GetLocalNamespace(string source)
        {
            Regex re = new Regex(@"namespace\s+([\w\.]+)\s*\{");
            Match m = re.Match(source);
            if (m.Success)
                return m.Groups[1].Value;
            else
                return null;
        }

        #endregion

        #region Base Types

        /*
         * Get base types for a web or code page.
         */

        protected abstract List<String> GetBaseTypes(string source);

        /*
         * Get a list of all the types from which the current
         * class is derived. This includes the base class and
         * any interfacess. If the class is not explicitly
         * derived from a class return System.Object as the
         * base class.
         */

        protected List<String> GetCodeBaseTypeList(string source)
        {
            List<String> list = new List<String>();

            source = CSharpFormattingTools.RemoveClassDefinitions(source);

            Regex re = new Regex(@"(?s)class\s+[\w]+\s*:?\s*[global::]*([\w\s\.,]*)\s*\{");
            Match m = re.Match(source);

            if (m.Success)
            {
                string types = m.Groups[1].Value;

                if (!String.IsNullOrEmpty(types))
                {
                    string[] split = types.Split(',');
                    foreach (string s in split)
                        list.Add(s.Trim());
                }
            }

            return list;
        }

        /*
         * Get the base types of the current web page.
         */

        protected List<String> GetWebBaseTypeList(string source)
        {
            List<String> list = new List<String>();

            /*
             * The 'inherits' attribute must be on the same line
             * as the opening tag.
             */

            Regex re = new Regex("(?i)<%@.+inherits\\s*=\\s*\\\"([\\w\\.]+)\\\"");
            Match m = re.Match(source);

            if (m.Success)
                list.Add(m.Groups[1].Value);
            else
                list.Add("System.Web.UI.Page");

            return list;
        }

        #endregion
        
        #region Member Type Name

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
    }
}
