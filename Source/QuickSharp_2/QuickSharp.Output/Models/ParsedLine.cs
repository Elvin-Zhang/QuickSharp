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

namespace QuickSharp.Output
{
    /// <summary>
    /// Provides information extracted from the output of an external
    /// tool such as a compiler of interpreter.
    /// </summary>
    public class ParsedLine
    {
        private string _path;
        private int _line;
        private int _column;
        private string _message;
        private bool _error;

        /// <summary>
        /// Create a new ParsedLine object.
        /// </summary>
        /// <param name="path">The path of the file containing the parsed message.</param>
        /// <param name="line">The line number of the parsed message.</param>
        /// <param name="column">The column reported for the parsed message.</param>
        /// <param name="message">The parsed message.</param>
        /// <param name="error">True if the message is to be reported as an error.</param>
        public ParsedLine(
            string path, 
            int line,
            int column, 
            string message,
            bool error)
        {
            _path = path;
            _line = line;
            _column = column;
            _message = message;
            _error = error;
        }

        /// <summary>
        /// The path of the file containing the parsed message.
        /// </summary>
        public string FilePath
        {
            get { return _path; }
        }

        /// <summary>
        /// The line number of the parsed message.
        /// </summary>
        public int Line
        {
            get { return _line; }
        }

        /// <summary>
        /// The column reported for the parsed message.
        /// </summary>
        public int Column
        {
            get { return _column; }
        }

        /// <summary>
        /// The parsed message.
        /// </summary>
        public string Message
        {
            get { return _message; }
        }

        /// <summary>
        /// True if the message is to be reported as an error.
        /// </summary>
        public bool IsError
        {
            get { return _error; }
        }
    }
}
