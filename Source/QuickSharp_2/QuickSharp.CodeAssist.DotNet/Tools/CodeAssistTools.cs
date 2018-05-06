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
using System.IO;
using System.Reflection;
using System.Text;
using QuickSharp.Core;

namespace QuickSharp.CodeAssist.DotNet
{
    public static class CodeAssistTools
    {
        #region Assembly Loader

        public static Assembly LoadAssembly(string name)
        {
            try
            {
                if (name[0] == '?')
                    return Assembly.LoadFile(name.Substring(1));
                else if (name[0] == '@')
                    return Assembly.LoadFrom(name.Substring(1));
                else
                    return Assembly.Load(name);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Colorization Helpers

        /*
         * Get a space delimited list of all the types in
         * the currently included namespaces. This is 
         * used by the editor type colorization to populate
         * the Scintilla keyword list.
         */

        public static string GetNamespaceTypesAsString(
            List<string> namespaceList,
            List<string> assemblyList)
        {
            List<string> typeNames = new List<string>();

            foreach (string assemblyName in assemblyList)
            {
                Assembly assembly = LoadAssembly(assemblyName);
                if (assembly == null) continue;

                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if ((type.IsClass ||
                            type.IsValueType ||
                            type.IsInterface) &&
                            namespaceList.Contains(type.Namespace))
                        {
                            string typeName = type.Name;
                            int i = typeName.IndexOf('`');

                            if (i != -1)
                                typeName = typeName.Substring(0, i);

                            if (!typeNames.Contains(typeName))
                                typeNames.Add(typeName);
                        }
                    }
                }
                catch
                {
                    // Ignore problematic assemblies
                }
            }

            StringBuilder sb = new StringBuilder();

            foreach (string name in typeNames)
            {
                sb.Append(name);
                sb.Append(" ");
            }

            return sb.ToString();
        }

        #endregion

        #region Assembly Dependency Sort

        /*
         * We receive and return assembly filenames
         * but need to work with assembly names for the
         * sort. This class associates the filenames with
         * the assemblies.
         */

        private class SortableAssembly
        {
            public string Name;
            public Assembly Assembly;
            public AssemblyName[] Refs;
        }

        public static List<string> SortAssemblies(List<string> assemblyNames)
        {
            List<SortableAssembly> unsorted = new List<SortableAssembly>();

            foreach (string name in assemblyNames)
            {
                Assembly assembly = CodeAssistTools.LoadAssembly(name);
                if (assembly != null)
                {
                    /*
                     * We don't know how expensive getting the
                     * referenced assemblies actually is so we get
                     * them once and keep a local copy.
                     */

                    SortableAssembly sa = new SortableAssembly();
                    sa.Name = name;
                    sa.Assembly = assembly;
                    sa.Refs = assembly.GetReferencedAssemblies();
                    unsorted.Add(sa);
                }
            }

            /*
             * Build the sorted list in reverse. Makes the logic easier
             * to follow and we have to iterate through the final list
             * to extract the filenames so we might as well do it backward.
             */

            List<SortableAssembly> sorted = new List<SortableAssembly>();

            foreach (SortableAssembly assembly in unsorted)
            {
                for (int i = 0; i < sorted.Count; i++)
                {
                    if (Array.IndexOf(
                        sorted[i].Refs, assembly.Assembly.FullName) != -1)
                    {
                        sorted.Insert(i, assembly);
                        break;
                    }
                }

                sorted.Add(assembly);
            }

            /*
             * Return the original filenames sorted in
             * dependency order.
             */

            List<string> sortedNames = new List<string>();

            foreach (SortableAssembly assembly in sorted)
                sortedNames.Insert(0, assembly.Name);

            return sortedNames;
        }

        #endregion
    }
}
