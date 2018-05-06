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
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace QuickSharp.Core
{
    /// <summary>
    /// Provides utility methods for working with files and folders.
    /// </summary>
    public static class FileTools
    {
        /// <summary>
        /// Get the full path of an executable file from the system path.
        /// </summary>
        /// <param name="name">The file to find.</param>
        /// <returns>The full path or null if not found.</returns>
        public static string FindOnSystemPath(string name)
        {
            string systemPath = Environment.GetEnvironmentVariable("PATH");
            if (systemPath == null) return null;

            foreach (string dir in systemPath.Split(';'))
            {
                string path = Path.Combine(dir, name);
                if (File.Exists(path)) return path;
            }
                
            return null;
        }

        /// <summary>
        /// Launch an application.
        /// </summary>
        /// <param name="useWindow">Run the application in it's own window.</param>
        /// <param name="exe">The path of the application executable.</param>
        /// <param name="args">The arguments to be passed to the executable.</param>
        public static void LaunchApplication(
            bool useWindow, string exe, string args)
        {
            LaunchApplication(useWindow, exe, args, null);
        }

        /// <summary>
        /// Launch an application with a working directory.
        /// </summary>
        /// <param name="useWindow">Run the application in it's own window.</param>
        /// <param name="exe">The path of the application executable.</param>
        /// <param name="args">The arguments to be passed to the executable.</param>
        /// <param name="folder">The working directory for the application.</param>
        public static void LaunchApplication(
            bool useWindow, string exe, string args, string folder)
        {
            if (!File.Exists(exe)) throw new Exception("File not found: " + exe);

            ProcessStartInfo pi = new ProcessStartInfo();
            pi.FileName = String.Format("\"{0}\"", exe);
            pi.Arguments = args;
            pi.UseShellExecute = false;
            pi.CreateNoWindow = !useWindow;

            if (!String.IsNullOrEmpty(folder))
                pi.WorkingDirectory = folder;

            Process p = Process.Start(pi);
        }

        /// <summary>
        /// Open a file using the Windows shell. Throws Exception if the file
        /// cannot be found.
        /// </summary>
        /// <param name="path">The path of the file to be opened.</param>
        public static void ShellOpenFile(string path)
        {
            ShellOpenFile(path, true);
        }

        /// <summary>
        /// Open a file using the Windows shell. Optionally throws Exception if
        /// the file cannot be found.
        /// </summary>
        /// <param name="path">The path of the file to be opened.</param>
        /// <param name="checkFileExists">Check to see if the file exists.
        /// If true, check and throw Exception if the file cannot be found.
        /// Don't check if false.</param>
        public static void ShellOpenFile(string path, bool checkFileExists)
        {
            if (checkFileExists && !File.Exists(path))
                throw new Exception("File not found: " + path);

            ProcessStartInfo pi = new ProcessStartInfo();
            pi.UseShellExecute = true;
            pi.FileName = path;

            Process p = Process.Start(pi);
        }

        /// <summary>
        /// Determine if a folder has subfolders.
        /// </summary>
        /// <param name="path">The path of the folder.</param>
        /// <returns>True if the folder has subfolders.</returns>
        public static bool FolderHasChildren(string path)
        {
            string [] files = Directory.GetFiles(path);
            if (files.Length > 0) return true;

            string [] dirs = Directory.GetDirectories(path);
            if (dirs.Length > 0) return true;

            return false;
        }

        /// <summary>
        /// Rename a file without changing its location.
        /// </summary>
        /// <param name="filePath">The path of the file to be renamed.</param>
        /// <param name="newName">The new name for the file.</param>
        /// <returns>The new path for the renamed file.</returns>
        public static string ChangeFileName(string filePath, string newName)
        {
            string newFilePath = Path.Combine(Path.GetDirectoryName(filePath), newName);
            File.Move(filePath, newFilePath);
            return newFilePath;
        }

        /// <summary>
        /// Rename a folder without changing its location.
        /// </summary>
        /// <param name="directoryPath">The path of the folder to be renamed.</param>
        /// <param name="newName">The new name for the folder.</param>
        /// <returns>The new path for the renamed folder.</returns>
        public static string ChangeDirectoryName(string directoryPath, string newName)
        {
            string newDirectoryPath = Path.Combine(Path.GetDirectoryName(directoryPath), newName);
            Directory.Move(directoryPath, newDirectoryPath);
            return newDirectoryPath;
        }

        /// <summary>
        /// Read a text file and return its content as a string.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>The content of the file.</returns>
        public static string ReadFile(string path)
        {
            /*
             * We don't really care what the encoding is
             * as long as the file is read correctly.
             */

            Encoding encoding = null;

            return ReadFile(path, out encoding);
        }

        /// <summary>
        /// Create an empty file if it doesn't already exist.
        /// </summary>
        /// <param name="path">The path of the new file.</param>
        /// <returns>True if the file was created successfully or the
        /// file already exists.</returns>
        public static bool CreateFile(string path)
        {
            if (File.Exists(path)) return true;

            try
            {
                StreamWriter sr = new StreamWriter(path);
                sr.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /*
         * This would seem to be the 'correct' way to do this but the BOM
         * detection doesn't seem to work for Unicode (BE and LE) encodings.
         * This has been reverted to the manual BOM detection used earlier.
         */

        //public static string ReadFile(string path, out Encoding encoding)
        //{
        //    // Default to ASCII in case the file isn't found.
        //    encoding = Encoding.ASCII;

        //    string text = null;

        //    if (!File.Exists(path)) return null;

        //    using (StreamReader sr = new StreamReader(path, encoding, true))
        //    {
        //        text = sr.ReadToEnd();
        //        encoding = sr.CurrentEncoding;
        //    }

        //    return text;
        //}

        /// <summary>
        /// Read a text file and returns its content and encoding. The
        /// encoding is determined using byte order marks (BOM) and defaults
        /// to ASCII if a BOM is not found or a problem occurs reading the file.
        /// Supported encodings are ASCII, UTF-8, UTF-16 BE and UTF-16 LE.
        /// </summary>
        /// <param name="path">The path of the file to read.</param>
        /// <param name="encoding">The file's encoding.</param>
        /// <returns>The content of the file.</returns>
        public static string ReadFile(string path, out Encoding encoding)
        {
            /*
             * Default to ASCII in case the file isn't found.
             */

            encoding = Encoding.ASCII;

            if (!File.Exists(path)) return null;

            encoding = GetEncoding(path);

            using (StreamReader sr = new StreamReader(new FileStream(path,
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite), encoding))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Open a text file and return its encoding. The
        /// encoding is determined using byte order marks (BOM) and defaults
        /// to ASCII if a BOM is not found or a problem occurs reading the file.
        /// Supported encodings are ASCII, UTF-8, UTF-16 BE and UTF-16 LE.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>The file's encoding.</returns>
        private static Encoding GetEncoding(string path)
        {
            /*
             * Return ASCII unless a BOM indicates unicode encoding.
             */

            FileStream fs = null;
            byte[] data = null;

            try
            {
                fs = File.Open(path, FileMode.Open,
                    FileAccess.Read, FileShare.ReadWrite);

                if (fs.Length < 3) return Encoding.ASCII;

                data = new byte[3];
                fs.Read(data, 0, 3);

                if (data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF)
                    return Encoding.UTF8;               // UTF-8
                if (data[0] == 0xFE && data[1] == 0xFF)
                    return Encoding.BigEndianUnicode;   // UTF-16 (BE)
                if (data[0] == 0xFF && data[1] == 0xFE)
                    return Encoding.Unicode;            // UTF-16 (LE)
                else
                    return Encoding.ASCII;
            }
            catch
            {
                return Encoding.ASCII;
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }

        /// <summary>
        /// Write text to a text file. A new file is always created and the text
        /// is written using the specified encoding.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="text">The content to write to the file.</param>
        /// <param name="encoding">The file's encoding.</param>
        public static void WriteFile(string path, string text, Encoding encoding)
        {
            using (StreamWriter sw = new StreamWriter(path, false, encoding))
            {
                sw.Write(text);
            }
        }

        /// <summary>
        /// Determine if a filename is invalid. A filename is invalid
        /// if it is null, empty or contains illegal characters.
        /// </summary>
        /// <param name="name">The path of the file.</param>
        /// <returns>True if the file is invalid.</returns>
        public static bool FilenameIsInvalid(string name)
        {
            if (String.IsNullOrEmpty(name)) return true;

            char[] invalidChars = Path.GetInvalidFileNameChars();

            foreach (char c in invalidChars)
                if (name.IndexOf(c) != -1) return true;

            return false;
        }

        /// <summary>
        /// Determine if a file was last written to after a specified time.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="timeStamp">The time to which the file is to be compared.</param>
        /// <returns>True if the specified time is older than the last
        /// write time of the file.</returns>
        public static bool FileIsNewerOnDisk(string path, DateTime timeStamp)
        {
            if (String.IsNullOrEmpty(path) || !File.Exists(path))
                return false;
            
            FileInfo fi = new FileInfo(path);

            return (timeStamp < fi.LastWriteTime);
        }

        /// <summary>
        /// Compare two file paths.
        /// </summary>
        /// <param name="p1">A file path.</param>
        /// <param name="p2">A file path.</param>
        /// <returns>True if the paths match.</returns>
        public static bool MatchPaths(string p1, string p2)
        {
            FileInfo f1 = new FileInfo(p1.ToLower());
            FileInfo f2 = new FileInfo(p2.ToLower());

            return (f1.FullName == f2.FullName);
        }

        #region SHFileOperation

        private const int FO_DELETE = 0x3;
        private const int FOF_ALLOWUNDO = 0x0040;
        private const int FOF_NOCONFIRMATION = 0x0010;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        internal struct SHFILEOPSTRUCT
        {
            internal IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            internal int wFunc;
            internal string pFrom;
            internal string pTo;
            internal short fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            internal bool fAnyOperationsAborted;
            internal IntPtr hNameMappings;
            internal string lpszProgressTitle;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

        /// <summary>
        /// Delete a file to the Windows recycle bin.
        /// </summary>
        /// <param name="filePath">The path of the file to be deleted.</param>
        public static void DeleteWithUndo(string filePath)
        {
            // filePath must be fully qualified for undelete to work
            SHFILEOPSTRUCT fileop = new SHFILEOPSTRUCT();
            fileop.wFunc = FO_DELETE;
            fileop.pFrom = filePath + '\0' + '\0';
            fileop.fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION;

            int res = SHFileOperation(ref fileop);

            if (res != 0) throw new Exception("System error");
        }

        #endregion
    }
}
