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
using System.Data.SQLite;
using System.IO;
using QuickSharp.Core;

namespace QuickSharp.Persistence.SQLite
{
    public class SQLitePersistenceManager : IPersistenceManager
    {
        private string _sectionKey;
        private string _databasePath;
        private string _connectionString;

        public SQLitePersistenceManager(string key)
        {
            ApplicationManager _applicationManager =
                ApplicationManager.GetInstance();

            string databaseName = _applicationManager.
                ClientProfile.PersistenceKey.Trim();

            if (databaseName == String.Empty)
                throw new Exception("Persistence key cannot be empty");

            _sectionKey = key;

            _databasePath = Path.Combine(
                _applicationManager.QuickSharpUserHome, databaseName);

            _connectionString = "Data Source=" + _databasePath;

            if (!File.Exists(_databasePath)) CreateDatabase();
        }

        public static IPersistenceManager GetInstance(string key)
        {
            return new SQLitePersistenceManager(key);
        }

        #region Create Database

        private void CreateDatabase()
        {
            ExecuteNonQuery("CREATE TABLE scalars (key, name, value)");
        }

        private void ExecuteNonQuery(string sql)
        {
            SQLiteConnection cnn = null;
            SQLiteCommand cmd = null;

            try
            {
                cnn = new SQLiteConnection(_connectionString);
                cnn.Open();

                cmd = new SQLiteCommand();
                cmd.Connection = cnn;
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (cnn != null) cnn.Dispose();
            }
        }

        #endregion

        #region Interface

        public int ReadInt(string itemName, int defaultValue)
        {
            return (int)(long)ReadItem(itemName, defaultValue);
        }

        public void WriteInt(string itemName, int itemValue)
        {
            WriteItem(itemName, itemValue);
        }

        public double ReadDouble(string itemName, double defaultValue)
        {
            return (double) ReadItem(itemName, defaultValue);
        }

        public void WriteDouble(string itemName, double itemValue)
        {
            WriteItem(itemName, itemValue);
        }

        public string ReadString(string itemName, string defaultValue)
        {
            if (String.IsNullOrEmpty(defaultValue))
                defaultValue = String.Empty;

            return (string) ReadItem(itemName, defaultValue);
        }

        public void WriteString(string itemName, string itemValue)
        {
            if (String.IsNullOrEmpty(itemValue))
                itemValue = String.Empty;

            WriteItem(itemName, itemValue);
        }

        public bool ReadBoolean(string itemName, bool defaultValue)
        {
            long val = (long) ReadItem(itemName, defaultValue);
            return (val == 1);
        }

        public void WriteBoolean(string itemName, bool itemValue)
        {
            WriteItem(itemName, itemValue);
        }

        public DateTime ReadDateTime(string itemName, DateTime defaultValue)
        {
            string val = (string)ReadItem(itemName, defaultValue.ToString());
            return DateTime.Parse(val);
        }

        public void WriteDateTime(string itemName, DateTime itemValue)
        {
            WriteItem(itemName, itemValue.ToString());
        }

        public List<String> ReadStrings(string itemName)
        {
            List<Object> objects = ReadItems(itemName);
            List<String> strings = new List<String>();

            foreach (object o in objects)
                strings.Add(o as string);

            return strings;
        }

        public void WriteStrings(string itemName, List<String> itemValue)
        {
            List<Object> items = new List<Object>();
            foreach (string s in itemValue)
                items.Add(s);

            WriteItems(itemName, items);
        }

        #endregion

        #region Implementation

        private void WriteItem(string itemName, object itemValue)
        {
            SQLiteConnection cnn = null;
            SQLiteTransaction trn = null;
            SQLiteCommand cmd = null;

            try
            {
                cnn = new SQLiteConnection(_connectionString);
                cnn.Open();

                trn = cnn.BeginTransaction();

                cmd = new SQLiteCommand();
                cmd.Connection = cnn;
                cmd.Transaction = trn;

                cmd.CommandText = "delete from scalars where key = ? and name = ?";
                cmd.Parameters.AddWithValue("key", _sectionKey);
                cmd.Parameters.AddWithValue("name", itemName);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "insert into scalars (key, name, value) values (?, ?, ?)";
                cmd.Parameters.AddWithValue("value", itemValue);
                cmd.ExecuteNonQuery();

                trn.Commit();
            }
            catch
            {
                trn.Rollback();
                throw;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (trn != null) trn.Dispose();
                if (cnn != null) cnn.Dispose();
            }
        }

        private object ReadItem(string itemName, object defaultValue)
        {
            SQLiteConnection cnn = null;
            SQLiteTransaction trn = null;
            SQLiteCommand cmd = null;

            try
            {
                cnn = new SQLiteConnection(_connectionString);
                cnn.Open();

                trn = cnn.BeginTransaction();

                cmd = new SQLiteCommand();
                cmd.Connection = cnn;
                cmd.Transaction = trn;

                cmd.CommandText = "select count(name) from scalars where key = ? and name = ?";
                cmd.Parameters.AddWithValue("key", _sectionKey);
                cmd.Parameters.AddWithValue("name", itemName);

                long count = (long) cmd.ExecuteScalar();

                if (count < 1)
                {
                    cmd.CommandText = "insert into scalars (key, name, value) values (?, ?, ?)";
                    cmd.Parameters.AddWithValue("value", defaultValue);
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = "select value from scalars where key = ? and name = ?";
                object val = cmd.ExecuteScalar();

                trn.Commit();

                return val;
            }
            catch
            {
                trn.Rollback();
                throw;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (trn != null) trn.Dispose();
                if (cnn != null) cnn.Dispose();
            }
        }

        private void WriteItems(string itemName, List<Object> itemValue)
        {
            SQLiteConnection cnn = null;
            SQLiteTransaction trn = null;
            SQLiteCommand cmd = null;

            try
            {
                cnn = new SQLiteConnection(_connectionString);
                cnn.Open();

                trn = cnn.BeginTransaction();

                cmd = new SQLiteCommand();
                cmd.Connection = cnn;
                cmd.Transaction = trn;

                cmd.CommandText = "delete from scalars where key = ? and name = ?";
                cmd.Parameters.AddWithValue("key", _sectionKey);
                cmd.Parameters.AddWithValue("name", itemName);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "insert into scalars (key, name, value) values (?, ?, ?)";

                SQLiteParameter p1 = new SQLiteParameter("value");
                cmd.Parameters.Add(p1);

                foreach (object val in itemValue)
                {
                    p1.Value = val;
                    cmd.ExecuteNonQuery();
                }

                trn.Commit();
            }
            catch
            {
                trn.Rollback();
                throw;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (trn != null) trn.Dispose();
                if (cnn != null) cnn.Dispose();
            }
        }

        private List<Object> ReadItems(string itemName)
        {
            SQLiteConnection cnn = null;
            SQLiteCommand cmd = null;
            SQLiteDataReader rdr = null;

            List<Object> list = new List<Object>();

            try
            {
                cnn = new SQLiteConnection(_connectionString);
                cnn.Open();

                cmd = new SQLiteCommand("select value from scalars where key = ? and name = ?", cnn);
                cmd.Parameters.AddWithValue("key", _sectionKey);
                cmd.Parameters.AddWithValue("name", itemName);

                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                    list.Add(rdr.GetValue(0));

                return list;
            }
            finally
            {
                if (rdr != null) rdr.Dispose();
                if (cmd != null) cmd.Dispose();
                if (cnn != null) cnn.Dispose();
            }
        }

        #endregion
    }
}
