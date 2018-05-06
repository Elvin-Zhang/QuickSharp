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

namespace QuickSharp.Core
{
    /// <summary>
    /// Defines the interface to the user settings storage provider.
    /// See the classes which implement the interface for more details.
    /// </summary>
    public interface IPersistenceManager
    {
        int ReadInt(string itemName, int defaultValue);
        void WriteInt(string itemName, int itemValue);
        double ReadDouble(string itemName, double defaultValue);
        void WriteDouble(string itemName, double itemValue);
        string ReadString(string itemName, string defaultValue);
        void WriteString(string itemName, string itemValue);
        bool ReadBoolean(string itemName, bool defaultValue);
        void WriteBoolean(string itemName, bool itemValue);
        DateTime ReadDateTime(string itemName, DateTime defaultValue);
        void WriteDateTime(string itemName, DateTime itemValue);
        List<String> ReadStrings(string itemName);
        void WriteStrings(string itemName, List<String> itemValue);
    }
}
