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
using System.IO;
using QuickSharp.Core;
using ScintillaNet;

namespace QuickSharp.Editor
{
    /// <summary>
    /// Manages editor find and replace.
    /// </summary>
    public class SearchManager
    {
        #region Singleton

        private static SearchManager _singleton;

        /// <summary>
        /// Get a reference to the SearchManager singleton.
        /// </summary>
        /// <returns>A reference to the SearchManager.</returns>
        public static SearchManager GetInstance()
        {
            if (_singleton == null)
                _singleton = new SearchManager();

            return _singleton;
        }

        #endregion

        private SearchForm _searchForm;

        private SearchManager()
        {
            _searchForm = new SearchForm();
        }

        /// <summary>
        /// Show the find/replace form.
        /// </summary>
        /// <param name="isReplace">Show replace if true.</param>
        public void ShowSearchForm(bool isReplace)
        {
            if (isReplace)
                _searchForm.ShowReplace();
            else
                _searchForm.ShowFind();
        }

        /// <summary>
        /// Hide the find/replace form.
        /// </summary>
        public void HideSearchForm()
        {
            _searchForm.Visible = false;
        }
    }
}

