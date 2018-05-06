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
using System.Text;
using System.Collections.Generic;

namespace QuickSharp.Core
{
    /// <summary>
    /// Provides a general-purpose, application-wide object store.
    /// Used to provide a means for plugins to share data without
    /// requiring explicit dependencies between them.
    /// Objects are stored using text keys. Care must be taken with 
    /// key management: keys need to be defined consistently wherever
    /// the data is accessed but because of the possibility that the
    /// provider and consumer might not have a direct dependency it
    /// will not allways be possible to use shared string constants to
    /// define the keys.
    /// </summary>
    public class ApplicationStorage
    {
        #region Singleton

        private static ApplicationStorage _singleton;

        /// <summary>
        /// Retrieves a reference to the ApplicationStorage singleton.
        /// </summary>
        /// <returns>A reference to the ApplicationStorage singleton.</returns>
        public static ApplicationStorage GetInstance()
        {
            if (_singleton == null)
                _singleton = new ApplicationStorage();

            return _singleton;
        }

        #endregion

        private Dictionary<String, Object> _appStore;

        private ApplicationStorage()
        {
            _appStore = new Dictionary<String, Object>();
        }

        /// <summary>
        /// Add an item to the store. Duplicate items are overwitten.
        /// Does nothing if the key is empty of null.
        /// </summary>
        /// <param name="key">The item's storage key.</param>
        /// <param name="value">The item.</param>
        public void Add(string key, object value)
        {
            Add(key, value, false);
        }

        /// <summary>
        /// Add an item to the store. Optionally throw 
        /// System.ArgumentException if the key already exists
        /// or System.ArgumentNullException if the key is null.
        /// </summary>
        /// <param name="key">The item's storage key.</param>
        /// <param name="value">The item.</param>
        /// <param name="throwOnError">Throw exception if key is nul
        /// or already exists.</param>
        public void Add(string key, object value, bool throwOnError)
        {
            if (throwOnError)
            {
                _appStore.Add(key, value);
            }
            else
            {
                if (!String.IsNullOrEmpty(key))
                    _appStore[key] = value;
            }
        }

        /// <summary>
        /// Get an item from the store using the specified key.
        /// </summary>
        /// <param name="key">The item's storage key.</param>
        /// <returns>The item or null if the key is null,
        /// empty or not found.</returns>
        public object Get(string key)
        {
            if (String.IsNullOrEmpty(key))
                return null;

            if (_appStore.ContainsKey(key))
                return _appStore[key];
            else
                return null;
        }

        /// <summary>
        /// Get an item from the store using the specified key.
        /// The item is created if it doesn't exist.
        /// </summary>
        /// <param name="key">The item's storage key.</param>
        /// <param name="type">The type of the item.</param>
        /// <returns>The item or null if the key is empty or null.</returns>
        public object Get(string key, Type type)
        {
            if (String.IsNullOrEmpty(key))
                return null;

            object item = Get(key);

            if (item == null)
            {
                // Create if not found.
                item = Activator.CreateInstance(type);
                _appStore[key] = item;
            }

            return item;
        }

        /// <summary>
        /// Gets or sets an item using the specified key.
        /// Duplicate items are overwitten.
        /// </summary>
        /// <param name="key">The item's storage key.</param>
        /// <returns>The value or null if the key is null, empty
        /// or not found.</returns>
        public object this[string key]
        {
            get
            {
                if (String.IsNullOrEmpty(key))
                    return null;

                if (_appStore.ContainsKey(key))
                    return _appStore[key];
                else
                    return null;
            }
            set
            {
                if (!String.IsNullOrEmpty(key))
                    _appStore[key] = value;
            }
        }
    }
}
