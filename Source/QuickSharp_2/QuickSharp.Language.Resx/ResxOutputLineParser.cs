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
using System.IO;
using System.Text.RegularExpressions;
using QuickSharp.Output;

namespace QuickSharp.Language.Resx
{
    public class ResxOutputLineParser : IOutputLineParser
    {
        public ParsedLine Parse(string line)
        {
            Regex re = new Regex(@"(.+)\((\d+),(\d+)\):(.+):(.+)");
            Match m = re.Match(line);
            if (!m.Success) return null;

            try
            {
                return new ParsedLine(
                    Path.Combine(Directory.GetCurrentDirectory(), m.Groups[1].Value),
                    Int32.Parse(m.Groups[2].Value),
                    Int32.Parse(m.Groups[3].Value),
                    m.Groups[5].Value,
                    (m.Groups[4].Value.ToLower().IndexOf("error") != -1));
            }
            catch
            {
                return null;
            }
        }
    }
}
