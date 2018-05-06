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

using System.Collections.Generic;

namespace QuickSharp.CodeAssist.DotNet
{
    public class ReferenceNamespace
    {
        private string _name;
        private List<string> _assemblyList;

        public string Name
        {
            get { return _name; }
        }

        public List<string> AssemblyList
        {
            get { return _assemblyList; }
        }

        public ReferenceNamespace(string name)
        {
            _name = name;
            _assemblyList = new List<string>();
        }

        public void AddAssembly(string assemblyName)
        {
            if (!_assemblyList.Contains(assemblyName))
                _assemblyList.Add(assemblyName);
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
