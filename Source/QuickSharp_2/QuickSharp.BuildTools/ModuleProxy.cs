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

namespace QuickSharp.BuildTools
{
    /// <summary>
    /// Provides access to the public members of the plugin module.
    /// </summary>
    public class ModuleProxy
    {
        #region Singleton

        private static ModuleProxy _singleton;

        /// <summary>
        /// Get a reference to the module proxy singleton.
        /// </summary>
        /// <returns></returns>
        public static ModuleProxy GetInstance()
        {
            if (_singleton == null)
                _singleton = new ModuleProxy();

            return _singleton;
        }

        #endregion

        private ModuleProxy()
        {
        }

        /// <summary>
        /// A reference to the module.
        /// </summary>
        public Module Module { get; set; }
    }
}