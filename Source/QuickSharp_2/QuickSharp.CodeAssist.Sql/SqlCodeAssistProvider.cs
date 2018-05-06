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
using System.Linq;
using System.Text;
using QuickSharp.Core;
using QuickSharp.Editor;
using QuickSharp.CodeAssist;
using QuickSharp.SqlManager;

namespace QuickSharp.CodeAssist.Sql
{
    public class SqlCodeAssistProvider : ICodeAssistProvider
    {
        private CodeAssistManager _codeAssistManager;
        private SqlCodeAssistManager _sqlCodeAssistManager;
        private SqlConnectionManager _sqlConnectionManager;
        private ISqlCodeAssistProvider _activeProvider;

        public SqlCodeAssistProvider()
        {
            _codeAssistManager = CodeAssistManager.GetInstance();
            _sqlCodeAssistManager = SqlCodeAssistManager.GetInstance();
            _sqlConnectionManager = SqlConnectionManager.GetInstance();

            _sqlConnectionManager.ConnectionChange +=
                new MessageHandler(ConnectionChanged);
        }

        #region ICodeAssistProvider

        public DocumentType DocumentType
        {
            get
            {
                return new DocumentType(Constants.DOCUMENT_TYPE);
            }
        }

        public bool IsAvailable
        {
            get { return (_activeProvider != null); }
        }

        public void DocumentActivated(ScintillaEditForm document)
        {
            if (_activeProvider != null)
                _activeProvider.ColorizeDocument(document);
        }

        public LookupList GetLookupList(ScintillaEditForm document)
        {
            if (_activeProvider != null)
                return _activeProvider.GetLookupList(document);
            else
                return null;
        }

        #endregion

        #region Connection Change Handler

        private void ConnectionChanged()
        {
            _activeProvider = null;

            if (_sqlConnectionManager.ConnectionIsActive)
            {
                SqlConnection cnn = 
                    _sqlConnectionManager.SelectedConnection;

                if (cnn != null)
                {
                    ISqlCodeAssistProvider provider =
                        _sqlCodeAssistManager.GetProvider(
                            cnn.Provider.InvariantName);

                    if (provider != null)
                    {
                        _activeProvider = provider;
                        provider.LoadDatabaseMetadata();
                    }
                }
            }

            _codeAssistManager.UpdateCodeAssistStatus();
        }

        #endregion
    }
}
