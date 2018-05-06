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

namespace QuickSharp.CodeAssist
{
    /// <summary>
    /// Encapsulates the information returned by a code assist provider.
    /// </summary>
    public class LookupList
    {
        private string _lookAheadText;
        private List<LookupListItem> _items;
        private List<String> _insertionTemplates;

        /// <summary>
        /// Create a LookupList with a default insertion template. This will insert the selected
        /// lookup item directly into the text at the point of invocation.
        /// </summary>
        /// <param name="lookAheadText">The lookahead text for the code assist invocation.</param>
        /// <param name="items">The items returned by the code assist lookup.</param>
        public LookupList(
            string lookAheadText,
            List<LookupListItem> items)
        {
            _lookAheadText = lookAheadText;
            _items = items;
            _insertionTemplates = new List<String>();
        }

        /// <summary>
        /// Create a LookupList with a custom insertion template. A custom template allows
        /// the selected lookup item to be inserted within a text template at the point
        /// of invocation.
        /// </summary>
        /// <param name="lookAheadText">The lookahead text for the code assist invocation.</param>
        /// <param name="items">The items returned by the code assist lookup.</param>
        /// <param name="insertionTemplate">The insertion template.</param>
        public LookupList(
            string lookAheadText,
            List<LookupListItem> items,
            string insertionTemplate)
        {
            _lookAheadText = lookAheadText;
            _items = items;
            _insertionTemplates = new List<String>();
            _insertionTemplates.Add(insertionTemplate);
        }

        /// <summary>
        /// Create a LookupList with custom insertion templates. A custom template allows
        /// the selected lookup item to be inserted within a text template at the point
        /// of invocation.
        /// </summary>
        /// <param name="lookAheadText">The lookahead text for the code assist invocation.</param>
        /// <param name="items">The items returned by the code assist lookup.</param>
        /// <param name="insertionTemplates">A list of insertion templates.</param>
        public LookupList(
            string lookAheadText,
            List<LookupListItem> items,
            List<String> insertionTemplates)
        {
            _lookAheadText = lookAheadText;
            _items = items;
            _insertionTemplates = new List<String>();
            _insertionTemplates.AddRange(insertionTemplates);
        }

        /// <summary>
        /// The lookahead text; the partial fragment of text used to preselect
        /// a partial match in the lookup item list.
        /// </summary>
        public string LookAheadText
        {
            get { return _lookAheadText; }
        }

        /// <summary>
        /// The lookup items selected by the code assist lookup.
        /// </summary>
        public List<LookupListItem> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// The list of insertion templates available for use by the code assist lookup.
        /// </summary>
        public List<String> InsertionTemplates
        {
            get { return _insertionTemplates; }
        }
    }
}
