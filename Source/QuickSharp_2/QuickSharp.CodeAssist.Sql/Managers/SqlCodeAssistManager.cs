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
using QuickSharp.Core;
using QuickSharp.Editor;

namespace QuickSharp.CodeAssist.Sql
{
    public class SqlCodeAssistManager
    {
        #region Singleton

        private static SqlCodeAssistManager _singleton;

        public static SqlCodeAssistManager GetInstance()
        {
            if (_singleton == null)
                _singleton = new SqlCodeAssistManager();

            return _singleton;
        }

        #endregion

        private Dictionary<String,
                ISqlCodeAssistProviderFactoryDelegate>
            _codeAssistProviderFactoryHandlers;

        private SqlCodeAssistManager()
        {
            _codeAssistProviderFactoryHandlers = 
                new Dictionary<String,
                    ISqlCodeAssistProviderFactoryDelegate>();
        }

        public void RegisterProviderFactoryHandlers(
            string providerInvariantName, 
            ISqlCodeAssistProviderFactoryDelegate handler)
        {
            string name = providerInvariantName.ToLower();

            _codeAssistProviderFactoryHandlers[name] = handler;
        }

        public ISqlCodeAssistProvider GetProvider(
            string providerInvariantName)
        {
            string name = providerInvariantName.ToLower();

            if (!_codeAssistProviderFactoryHandlers.ContainsKey(name))
                return null;

            ISqlCodeAssistProviderFactoryDelegate handler =
                _codeAssistProviderFactoryHandlers[name];

            return handler();
        }
    }
}
