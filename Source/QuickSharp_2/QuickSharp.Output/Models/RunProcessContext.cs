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
using System.Drawing;
using System.Diagnostics;

namespace QuickSharp.Output
{
    /// <summary>
    /// Context information required to run an external program in a new process.
    /// </summary>
    public class RunProcessContext
    {
        /// <summary>
        /// The path of the executable to run in the new process.
        /// </summary>
        public string ExePath { get; set; }

        /// <summary>
        /// Arguments to be passed to the new process.
        /// </summary>
        public string ProcessArgs { get; set; }

        /// <summary>
        /// A text message to be displayed before the output from the process.
        /// </summary>
        public string HeaderText { get; set; }

        /// <summary>
        /// A text message to be displayed after the output from the process.
        /// </summary>
        public string FooterText { get; set; }

        /// <summary>
        /// A line parser used to extract information from each output line of the process.
        /// </summary>
        public IOutputLineParser LineParser { get; set; }

        /// <summary>
        /// The exit code used to represent successful completion of the process.
        /// </summary>
        public int ExitCode { get; set; }

        private OutputForm _output;

        /// <summary>
        /// A reference to the parent output window.
        /// </summary>
        public OutputForm Output
        {
            set { _output = value; }
        }

        /// <summary>
        /// Create a new context object.
        /// </summary>
        public RunProcessContext()
        {
            HeaderText = String.Empty;
            FooterText = String.Empty;
            ExitCode = 0;
        }

        /// <summary>
        /// Callback used to receive output from the running process.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event arguments.</param>
        public void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null) return;
            
            string outputText = e.Data;

            _output.AddLineToTextView(outputText);

            foreach (string line in outputText.Split('\n'))
                _output.AddLineToListView(line, LineParser);
        }

        /// <summary>
        /// Callback used to receive error output from the running process.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Event arguments.</param>
        public void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null) return;

            string errorText = e.Data;

            _output.AddLineToTextView(errorText);

            foreach (string line in errorText.Split('\n'))
                _output.AddLineToListView(line, LineParser, Color.Red);
        }
    }
}
