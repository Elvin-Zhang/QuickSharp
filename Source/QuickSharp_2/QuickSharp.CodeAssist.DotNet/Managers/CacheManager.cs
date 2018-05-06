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
using QuickSharp.Core;

namespace QuickSharp.CodeAssist.DotNet
{
    public class CacheManager
    {
        #region Singleton

        private static CacheManager _singleton;

        public static CacheManager GetInstance()
        {
            if (_singleton == null)
                _singleton = new CacheManager();

            return _singleton;
        }

        #endregion

        private Dictionary<String, CachedAssembly> _cache;
        private string _cachePath;

        private CacheManager()
        {
            _cache = new Dictionary<String, CachedAssembly>();

            _cachePath = Path.Combine(
                ApplicationManager.GetInstance().QuickSharpUserHome,
                Constants.ASSEMBLY_CACHE_FOLDER);

            /*
             * Delete cached assemblies from previous session.
             */

            try
            {
                if (Directory.Exists(_cachePath))
                    Directory.Delete(_cachePath, true);
            }
            catch
            {
                // Do nothing - try again next time
            }

            /*
             * Set up handler to resolve assemblies from the cache.
             */

            AppDomain.CurrentDomain.AssemblyResolve += 
                new ResolveEventHandler(ResolveAssembly);
        }

        private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            try
            {
                string name = args.Name.Split(',')[0];
                string path = CacheManager.GetInstance().GetCachedAssemblyPath(name + ".dll");
                if (path == null) return null;

                return Assembly.LoadFile(path);
            }
            catch
            {
                return null;
            }
        }

        #region Workspace assembly cache

        public List<string> UpdateAssemblyCache()
        {
            List<string> cachedAssemblies = new List<string>();

            try
            {
                if (!Directory.Exists(_cachePath))
                    Directory.CreateDirectory(_cachePath);
            }
            catch
            {
                return cachedAssemblies;
            }

            string workspaceDirectory =
                Directory.GetCurrentDirectory();

            AddDirectoryToCache(cachedAssemblies,
                workspaceDirectory);

            string workspaceBinDirectory =
                Path.Combine(workspaceDirectory, "bin");

            if (Directory.Exists(workspaceBinDirectory))
                AddDirectoryToCache(cachedAssemblies,
                    workspaceBinDirectory);

            return cachedAssemblies;
        }

        private void AddDirectoryToCache(
            List<string> cachedAssemblies, string assemblyDirectory)
        {
            try
            {
                string [] dllFiles = 
                    Directory.GetFiles(assemblyDirectory, "*.dll");

                GetAssemblyFiles(cachedAssemblies, dllFiles);

                string[] exeFiles =
                    Directory.GetFiles(assemblyDirectory, "*.exe");

                GetAssemblyFiles(cachedAssemblies, exeFiles);
            }
            catch
            {
                // Not much we can do here, just suppress the
                // error and return the list we have so far.
            }
        }

        private void GetAssemblyFiles(List<string> cachedAssemblies, string[] files)
        {
            foreach (string f in files)
            {
                CachedAssembly item = null;

                if (_cache.ContainsKey(f))
                {
                    item = _cache[f];

                    if (File.GetLastWriteTime(f) > item.TimeStamp)
                    {
                        item = new CachedAssembly(f, _cachePath);
                        _cache[f] = item; // Update existing item

                        if (!item.CopyToCache()) item = null;
                    }
                }
                else
                {
                    item = new CachedAssembly(f, _cachePath);
                    _cache.Add(f, item); // Create new item

                    if (!item.CopyToCache()) item = null;
                }

                if (item != null)
                    cachedAssemblies.Add("?" + item.CachePath);
            }
        }

        private string GetCachedAssemblyPath(string filename)
        {
            foreach (string key in _cache.Keys)
                if (Path.GetFileName(key) == filename)
                    return _cache[key].CachePath;

            return null;
        }

        #endregion
    }
}
