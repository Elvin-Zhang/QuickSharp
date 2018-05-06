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
using System.Text.RegularExpressions;
using System.IO;
using QuickSharp.Core;
using QuickSharp.Output;
using QuickSharp.Editor;
using ScintillaNet;

namespace QuickSharp.FindInFiles
{
    public class Finder
    {
        private MainForm _mainForm;
        private OutputForm _outputForm;
        private IOutputLineParser _outputLineParser;

        public Finder(OutputForm output)
        {
            _mainForm = ApplicationManager.GetInstance().MainForm;
            _outputForm = output;
            _outputLineParser = new FinderOutputLineParser();
        }

        public void Find(string findText, string replaceText,
            string filePattern, bool searchSubFolders,
            bool useRegex, bool matchCase, bool findOnly)
        {
            /*
             * Prepare the output view.
             */

            _outputForm.ClearOutputViews();
            _outputForm.Text = String.Format(findOnly ?
                Resources.OutputTitleFind : Resources.OutputTitleReplace,
                filePattern);

            /*
             * Find all the matching files.
             */

            string[] files = Directory.GetFiles(
                ".", filePattern, searchSubFolders ? 
                    SearchOption.AllDirectories :
                    SearchOption.TopDirectoryOnly);

            if (files.Length == 0)
            {
                _outputForm.AddLineToOutputView(
                    Resources.FindMessageNoMatchingFiles);

                _outputForm.AdjustOutputWidth();
                
                return;
            }

            /*
             * Prepare the regular expression if required.
             */

            Regex regex = null;

            if (useRegex)
            {
                try
                {
                    regex = new Regex(findText);
                }
                catch (ArgumentException ex)
                {
                    _outputForm.AddLineToOutputView(
                        String.Format(Resources.FindError, ex.Message),
                        System.Drawing.Color.Red);

                    _outputForm.AdjustOutputWidth();

                    return;
                }
            }

            /*
             * Search. File is ".\filename.ext" so we use Substring(2) for the name.
             */

            if (findOnly)
            {
                int lineCount = 0;

                foreach (string file in files)
                {
                    if (useRegex)
                        lineCount += FindInFileRE(file.Substring(2), regex);
                    else
                        lineCount += FindInFile(file.Substring(2), findText, matchCase);
                }

                _outputForm.AddLineToOutputView(
                    String.Format(lineCount == 1 ?
                        Resources.FindMessageMatchCountSingle :
                        Resources.FindMessageMatchCountMulti, lineCount));
            }
            else
            {
                int fileCount = 0;

                foreach (string file in files)
                {
                    bool updated = false;
                    
                    if (useRegex)
                        updated = ReplaceInFileRE(file.Substring(2), replaceText, regex);
                    else
                        updated = ReplaceInFile(file.Substring(2), findText, replaceText);
                    
                    if (updated)
                        fileCount++;
                }

                _outputForm.AddLineToOutputView(
                    String.Format(fileCount == 1 ?
                        Resources.FindMessageReplaceCountSingle :
                        Resources.FindMessageReplaceCountMulti, fileCount));
            }

            _outputForm.AdjustOutputWidth();
        }

        #region Plain Matches

        private int FindInFile(string filePath, string findText, bool matchCase)
        {
            if (!matchCase) findText = findText.ToLower();

            /*
             * Read the file line by line to find the matches.
             */

            StreamReader sr = null;

            int linesMatchedCount = 0;
               
            try
            {
                sr = new StreamReader(filePath, Encoding.ASCII, true);
                string line = null;
                int lineNumber = 0;

                while ((line = sr.ReadLine()) != null)
                {
                    lineNumber++;

                    string s = matchCase ? line : line.ToLower();

                    if (s.IndexOf(findText) != -1)
                    {
                        _outputForm.AddLineToOutputView(String.Format(
                                "{0}:{1}:{2}", filePath, lineNumber, line),
                            _outputLineParser);

                        linesMatchedCount++;
                    }
                }
            }
            catch
            {
                // Just ignore any dodgy files
            }
            finally
            {
                if (sr != null) sr.Close();
            }

            return linesMatchedCount;
        }

        private bool ReplaceInFile(string filePath, string findText, string replaceText)
        {
            /*
             * Check if the file contains the search term.
             */

            string fullPath = Path.GetFullPath(filePath);
            
            string fileText = GetFileText(fullPath);

            if (fileText.IndexOf(findText) == -1)
                return false;

            /*
             * Load the file into an editor and replace all matches
             * with the new text (don't update the MRU list).
             */

            _mainForm.LoadDocumentIntoWindow(fullPath, false);

            ScintillaEditForm document =
                _mainForm.ActiveDocument as ScintillaEditForm;

            if (document == null) return false;

            document.Editor.Markers.DeleteAll(
                QuickSharp.Editor.Constants.BOOKMARK_MARKER);

            document.Editor.UndoRedo.BeginUndoAction();

            foreach (Line line in document.Lines)
            {
                if (line.Text.IndexOf(findText) != -1)
                {
                    string s = line.Text.Replace(findText, replaceText);
                    line.AddMarker(QuickSharp.Editor.Constants.BOOKMARK_MARKER);

                    /*
                     * Line needs to be trimmed of any EOL characters to prevent 
                     * extra blank lines being inserted.
                     */

                    line.Text = s.TrimEnd(new Char[] { '\n', '\r' });
                }
            }

            document.Editor.UndoRedo.EndUndoAction();

            return true;
        }

        #endregion

        #region Regular Expression Matches

        private int FindInFileRE(string filePath, Regex regex)
        {
            /*
             * Must match against whole document as regex matches can
             * be multi-line; each match might span several lines so
             * we can't use a line by line approach.
             */

            string fullPath = Path.GetFullPath(filePath);
            string fileText = null;

            try
            {
                fileText = GetFileText(filePath);
            }
            catch
            {
                // just ignore dodgy files
                return 0;
            }

            MatchCollection matches = regex.Matches(fileText);

            if (matches.Count == 0) return 0;

            /*
             * We have the index of each match - translate them to line numbers.
             * Scan the text counting the end of line markers and recording
             * the line on which each match occurred. First we need to normailze
             * the line endings without changing the line lengths.
             */

            fileText = fileText.Replace("\r\n", "\n ");
            fileText = fileText.Replace("\r", "\n");

            // Get the individual lines for reporting
            string [] lines = fileText.Split('\n');

            int textPos = 0;
            int linePos = 1;
            int lastLine = -1;
            int lineCount = 0;

            while (textPos < fileText.Length)
            {
                if (fileText[textPos] == '\n')
                {
                    textPos++;
                    linePos++;
                    continue;
                }

                foreach (Match m in matches)
                {
                    if (textPos == m.Index && linePos != lastLine)
                    {
                        _outputForm.AddLineToOutputView(String.Format(
                                "{0}:{1}:{2}", filePath, linePos, lines[linePos - 1]),
                            _outputLineParser);

                        lastLine = linePos;
                        lineCount++;
                    }
                }

                textPos++;
            }

            return lineCount;
        }
 
        private bool ReplaceInFileRE(string filePath, string replaceText, Regex regex)
        {
            /*
             * Must match against whole document as regex matches can
             * be multi-line; each match might span several lines so
             * we can't use a line by line approach.
             */

            string fullPath = Path.GetFullPath(filePath);
            string fileText = null;

            try
            {
                fileText = GetFileText(filePath);
            }
            catch
            {
                // just ignore dodgy files
                return false;
            }

            MatchCollection matches = regex.Matches(fileText);

            if (matches.Count == 0) return false;

            /*
             * Load the file into an editor and replace all matches
             * with the new text (don't update the MRU list).
             */

            _mainForm.LoadDocumentIntoWindow(fullPath, false);
            
            ScintillaEditForm document =
                _mainForm.ActiveDocument as ScintillaEditForm;

            if (document == null) return false;

            document.Editor.Markers.DeleteAll(
                QuickSharp.Editor.Constants.BOOKMARK_MARKER);

            /*
             * Can't track the actual change locations using regex replace so we add a
             * sentinel string to mark the lines on which the changes occur. Then we
             * bookmark each line with a sentinel and remove it.
             * This is a bit hacky and would break if the sentinel actually appears
             * in the text but it's the only way to do multi-line replaces and still
             * preserve the bookmarks in the right places.
             */

            string sentinel = "{^¬£`^}";

            document.Editor.UndoRedo.BeginUndoAction();

            document.Editor.Text = regex.Replace(
                document.Editor.Text, sentinel + replaceText);

            foreach (Line line in document.Editor.Lines)
            {
                if (line.Text.IndexOf(sentinel) != -1)
                {
                    string s = line.Text.Replace(sentinel, String.Empty);
                    line.Text = s.TrimEnd(new Char[] { '\n', '\r' });

                    line.AddMarker(QuickSharp.Editor.Constants.BOOKMARK_MARKER);
                }
            }

            document.Editor.UndoRedo.EndUndoAction();

            return true;
        }

        #endregion

        #region Helpers

        private string GetFileText(string path)
        {
            /*
             * Check the file isn't already open.
             */

            foreach (QuickSharp.Core.Document document in
                _mainForm.ClientWindow.Documents)
            {
                ScintillaEditForm editor =
                    document as ScintillaEditForm;

                if (editor != null &&  FileTools.MatchPaths(path, editor.FilePath))
                    return editor.Editor.Text;
            }

            /*
             * Read the content from the disk.
             */

            return FileTools.ReadFile(path);
        }

        #endregion
    }
}
