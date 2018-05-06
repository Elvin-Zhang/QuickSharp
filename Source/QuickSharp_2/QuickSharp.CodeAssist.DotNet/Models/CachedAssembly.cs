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
using System.IO;
using System.Reflection;

namespace QuickSharp.CodeAssist.DotNet
{
    public class CachedAssembly
    {
        public string AssemblyPath { get; set; }
        public string CachePath { get; set; }
        public DateTime TimeStamp { get; set; }

        public CachedAssembly(string assemblyPath, string cacheRoot)
        {
            AssemblyPath = assemblyPath;
            CachePath = Path.Combine(
                cacheRoot,
                String.Format("{0}_{1}",
                    Guid.NewGuid().ToString(),
                    Path.GetFileName(assemblyPath)));

            TimeStamp = File.GetLastWriteTime(assemblyPath);
        }

        public bool CopyToCache()
        {
            try
            {
                File.Copy(AssemblyPath, CachePath);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
