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
using Microsoft.Win32;

namespace QuickSharp.Core
{
    /// <summary>
    /// Implements IPersistenceManager providing a Windows registry-based
    /// persistence manager. Application settings are stored in the registry
    /// under an application-specific key located at "HKEY\CURRENT_USER\Software".
    /// Individual instances of the persistence manager use a sub-key to store
    /// settings within the main application hive providing a simple namespacing
    /// system for the application plugins.
    /// </summary>
    public class RegistryPersistenceManager : IPersistenceManager
    {
        private ApplicationManager _applicationManager;
        private string _registryKey;

        /// <summary>
        /// Create an instance of a persistence manager for registry access.
        /// </summary>
        /// <param name="key">The registry sub-key.</param>
        public RegistryPersistenceManager(string key)
        {
            if (String.IsNullOrEmpty(key))
                throw new Exception("Registry key cannot be null or empty");

            _applicationManager = ApplicationManager.GetInstance();

            string parent = _applicationManager.
                ClientProfile.PersistenceKey.Trim();

            if (parent == String.Empty)
                throw new Exception("Persistence key cannot be empty");

            _registryKey = String.Format(
                "{0}\\software\\{1}\\{2}",
                Registry.CurrentUser, parent, key);
        }

        #region Factory

        /// <summary>
        /// Get an instance of the registry persistence manager.
        /// </summary>
        /// <param name="key">The registry sub-key.</param>
        /// <returns>The persistence manager instance.</returns>
        public static IPersistenceManager GetInstance(string key)
        {
            return new RegistryPersistenceManager(key);
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Read an Int32 value from the registry with the specified name.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="defaultValue">The item's default value.</param>
        /// <returns>The item's value or the default value if not found.</returns>
        public int ReadInt(string itemName, int defaultValue)
        {
            return Int32.Parse(
                ReadString(itemName, defaultValue.ToString()));
        }

        /// <summary>
        /// Write an Int32 value to the registry with the specified name.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="itemValue">The item value.</param>
        public void WriteInt(string itemName, int itemValue)
        {
            WriteString(itemName, itemValue.ToString());
        }

        /// <summary>
        /// Read a Double value from the registry with the specified name.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="defaultValue">The item's default value.</param>
        /// <returns>The item's value or the default value if not found.</returns>
        public double ReadDouble(string itemName, double defaultValue)
        {
            return Double.Parse(
                ReadString(itemName, defaultValue.ToString()));
        }

        /// <summary>
        /// Write a Double value to the registry with the specified name.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="itemValue">The item value.</param>
        public void WriteDouble(string itemName, double itemValue)
        {
            WriteString(itemName, itemValue.ToString());
        }

        /// <summary>
        /// Read a String value from the registry with the specified name.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="defaultValue">The item's default value.</param>
        /// <returns>The item's value or the default value if not found.</returns>
        public string ReadString(string itemName, string defaultValue)
        {
            string itemValue = Registry.GetValue(
                _registryKey, itemName, defaultValue) as string;

            if (itemValue == null) itemValue = defaultValue;

            return itemValue;
        }

        /// <summary>
        /// Write a String value to the registry with the specified name.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="itemValue">The item value.</param>
        public void WriteString(string itemName, string itemValue)
        {
            Registry.SetValue(_registryKey, itemName, itemValue);
        }

        /// <summary>
        /// Read a Boolean value from the registry with the specified name.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="defaultValue">The item's default value.</param>
        /// <returns>The item's value or the default value if not found.</returns>
        public bool ReadBoolean(string itemName, bool defaultValue)
        {
            return ReadString(
                itemName, defaultValue ? "1" : "0") == "1";
        }

        /// <summary>
        /// Write a Boolean value to the registry with the specified name.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="itemValue">The item value.</param>
        public void WriteBoolean(string itemName, bool itemValue)
        {
            WriteString(itemName, itemValue ? "1" : "0");
        }

        /// <summary>
        /// Read a DateTime value from the registry with the specified name.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="defaultValue">The item's default value.</param>
        /// <returns>The item's value or the default value if not found.</returns>
        public DateTime ReadDateTime(string itemName, DateTime defaultValue)
        {
            return DateTime.Parse(
                ReadString(itemName, defaultValue.ToString()));
        }

        /// <summary>
        /// Write a DateTime value to the registry with the specified name.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="itemValue">The item value.</param>
        public void WriteDateTime(string itemName, DateTime itemValue)
        {
            WriteString(itemName, itemValue.ToString());
        }

        /// <summary>
        /// Read a list of Strings from the registry with the specified name.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <returns>The list of strings or an empty list if not found.</returns>
        public List<String> ReadStrings(string itemName)
        {
            object arr = Registry.GetValue(_registryKey, itemName, null);
            if (arr == null)
                return new List<String>();
            else
                return new List<String>( (string[]) arr);
        }

        /// <summary>
        /// Write a List of String values to the registry with the specified name.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="itemValue">The List of Strings.</param>
        public void WriteStrings(string itemName, List<String> itemValue)
        {
            Registry.SetValue(
                _registryKey, itemName, itemValue.ToArray());
        }

        #endregion
    }
}
