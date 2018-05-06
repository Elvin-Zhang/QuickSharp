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

namespace QuickSharp.CodeAssist.Sql
{
    public class Table
    {
        private string _name;
        private string _schema;
        private bool _isView;
        private List<TableColumn> _columns;

        public Table(string name, string schema, bool isView)
        {
            _name = name;
            _schema = schema;
            _isView = isView;
            _columns = new List<TableColumn>();
        }

        #region Properties

        public string Name
        {
            get { return _name; }
        }

        public string Schema
        {
            get { return _schema; }
        }

        public string FullName
        {
            get
            {
                string schema = _schema;
                
                if (String.IsNullOrEmpty(schema))
                    schema = "dbo";

                return String.Format(
                    "{0}.{1}", schema, _name); 
            }
        }

        public bool IsView
        {
            get { return _isView; }
        }

        public List<TableColumn> Columns
        {
            get { return _columns; }
        }

        #endregion
    }
}
