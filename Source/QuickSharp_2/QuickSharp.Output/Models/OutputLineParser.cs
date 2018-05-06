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
using System.Text;
using System.Text.RegularExpressions;

namespace QuickSharp.Output
{
    /// <summary>
    /// A generic output line parser based on a regular expression. Allows basic error checking
    /// parsers to be provided without having to create a custom class.
    /// </summary>
    public class OutputLineParser : IOutputLineParser
    {
        private Regex _regex;

        /// <summary>
        /// Create an OutputLineParser object.
        /// </summary>
        /// <param name="pattern">The regular expression used to parse the output line.
        /// Must include capturing groups named 'path' and 'line'.</param>
        public OutputLineParser(string pattern)
        {
            _regex = new Regex(pattern);

            string[] groupNames = _regex.GetGroupNames();

            if (Array.IndexOf<string>(groupNames, Constants.REGEX_GROUP_PATH) == -1)
                throw new Exception(String.Format("{0}: {1}",
                    Resources.RegexGroupMissing, Constants.REGEX_GROUP_PATH));

            if (Array.IndexOf<string>(groupNames, Constants.REGEX_GROUP_LINE) == -1)
                throw new Exception(String.Format("{0}: {1}",
                    Resources.RegexGroupMissing, Constants.REGEX_GROUP_LINE));
        }

        /// <summary>
        /// Parse the output text using the regular expression provided. All
        /// lines matching the expression will be reported as errors.
        /// </summary>
        /// <param name="text">The text to be parsed.</param>
        /// <returns>A ParsedLine object containing the error information.</returns>
        public ParsedLine Parse(string text)
        {
            Match m = _regex.Match(text);
            if (!m.Success) return null;

            try
            {
                string path = m.Groups[Constants.REGEX_GROUP_PATH].Value;
                string line = m.Groups[Constants.REGEX_GROUP_LINE].Value;

                return new ParsedLine(path, Int32.Parse(line), 0, String.Empty, true);
            }
            catch
            {
                return null;
            }
        }
    }
}
