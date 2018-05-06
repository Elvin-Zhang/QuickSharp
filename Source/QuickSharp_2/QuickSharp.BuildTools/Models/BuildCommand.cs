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

using System.IO;

namespace QuickSharp.BuildTools
{
    /// <summary>
    /// A build command represents a concrete invocation of a build tool
    /// on a source file.
    /// </summary>
    public class BuildCommand
    {
        /// <summary>
        /// The build tool used in the command.
        /// </summary>
        public BuildTool BuildTool { get; set; }

        /// <summary>
        /// Information about the input file.
        /// </summary>
        public FileInfo SourceInfo { get; set; }

        /// <summary>
        /// Source code supplied to the build tool.
        /// </summary>
        public string SourceText { get; set; }

        /// <summary>
        /// Information about the output file.
        /// </summary>
        public FileInfo TargetInfo { get; set; }

        /// <summary>
        /// The document type of the output file.
        /// </summary>
        public string TargetType { get; set; }

        /// <summary>
        /// The expanded file path of the build tool.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The expanded arguments passed to the build tool.
        /// </summary>
        public string Args { get; set; }

        /// <summary>
        /// Text displayed before the tool is run.
        /// </summary>
        public string StartText { get; set; }

        /// <summary>
        /// Text displayed after the tool has run.
        /// </summary>
        public string FinishText { get; set; }

        /// <summary>
        /// Flag used to allow the command to be cancelled.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Result to be returned by the command in the event of a cancellation.
        /// </summary>
        public bool CancelResult { get; set; }

        /// <summary>
        /// Return code to determine successful completion of a build command.
        /// </summary>
        public int SuccessCode { get; set; }

        /// <summary>
        /// Flag to determine if a target requires rebuilding before the command
        /// is executed (used by run commands).
        /// </summary>
        /// <returns></returns>
        public bool TargetBuildRequired()
        {
            if (!File.Exists(TargetInfo.FullName)) return true;
            return (SourceInfo.LastWriteTime > TargetInfo.LastWriteTime);
        }
    }
}
