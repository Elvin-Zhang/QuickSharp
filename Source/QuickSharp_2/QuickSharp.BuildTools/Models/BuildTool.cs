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

using System.Text;
using QuickSharp.Core;
using QuickSharp.Output;

namespace QuickSharp.BuildTools
{
    /// <summary>
    /// Defines a specific configuration of a build tool registered with the build system.
    /// A build command is retrieved from a language support plugin and provides the functionality
    /// required to actually perform a particular build task such as running a compiler.
    /// The build tool defines the specific parameters to be passed to the plugin to allow
    /// a concrete invocation of the build tool to be created. For example, the plugin might define
    /// the invocation of a C# compiler but the build tool will define its actual path (allowing
    /// separate configurations for different compiler versions), or the switches to be passed
    /// (for release and debug builds). The build command defines an abstract tool definition
    /// and a build tool configuration allows it to be converted into an actual invocation for
    /// execution in the output window.
    /// </summary>
    public class BuildTool
    {
        private string _id;
        private DocumentType _documentType;
        private string _toolAction;
        private string _toolPath;
        private string _toolArgs;
        private string _userArgs;
        private string _displayName;
        private string _lineParserName;
        private IOutputLineParser _lineParser;
        private BuildCommandDelegate _buildCommand;

        /// <summary>
        /// Create a build tool configuration.
        /// </summary>
        /// <param name="id">A unique ID representing the tool.</param>
        /// <param name="documentType">The document type the tool is associated with.</param>
        /// <param name="displayName">The tool's display name.</param>
        public BuildTool(string id, DocumentType documentType, string displayName)
        {
            _id = id;
            _documentType = documentType;
            _displayName = displayName;
        }

        /// <summary>
        /// A unique ID representing the tool.
        /// </summary>
        public string Id
        {
            get { return _id; }
        }

        /// <summary>
        /// The document type the tool is associated with.
        /// </summary>
        public DocumentType DocumentType
        {
            get { return _documentType; }
        }

        /// <summary>
        /// The tool's display name.
        /// </summary>
        public string DisplayName
        {
            get { return _displayName; }
        }

        /// <summary>
        /// The build action the tool performs.
        /// </summary>
        public string Action
        {
            get { return _toolAction; }
            set { _toolAction = value; }
        }

        /// <summary>
        /// The path of the build tool. This
        /// can be a string template containing macros for expansion.
        /// </summary>
        public string Path
        {
            get { return _toolPath; }
            set { _toolPath = value; }
        }

        /// <summary>
        /// Arguments to be passed to the build tool. This
        /// can be a string template containing macros for expansion.
        /// </summary>
        public string Args
        {
            get { return _toolArgs; }
            set { _toolArgs = value; }
        }

        /// <summary>
        /// Common arguments to be passed to the build tool.
        /// </summary>
        public string UserArgs
        {
            get { return _userArgs; }
            set { _userArgs = value; }
        }

        /// <summary>
        /// The name of the line parser associated with the tool.
        /// </summary>
        public string LineParserName
        {
            get { return _lineParserName; }
            set { _lineParserName = value; }
        }

        /// <summary>
        /// A reference to an instance of the line parser associated with the tool.
        /// </summary>
        public IOutputLineParser LineParser
        {
            get { return _lineParser; }
            set { _lineParser = value; }
        }

        /// <summary>
        /// A delegate to the build command provider associated with the tool.
        /// </summary>
        public BuildCommandDelegate BuildCommand
        {
            get { return _buildCommand; }
            set { _buildCommand = value; }
        }

        /// <summary>
        /// Provide a 'serialized' version of the tool for
        /// saving in the persistence store.
        /// </summary>
        /// <returns>The configuration as a string.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(_id);
            sb.Append(Constants.SERIALIZATION_DELIMITER);
            sb.Append(_documentType.ToString());
            sb.Append(Constants.SERIALIZATION_DELIMITER);
            sb.Append(_displayName);
            sb.Append(Constants.SERIALIZATION_DELIMITER);
            sb.Append(_toolAction);
            sb.Append(Constants.SERIALIZATION_DELIMITER);
            sb.Append(_toolPath);
            sb.Append(Constants.SERIALIZATION_DELIMITER);
            sb.Append(_toolArgs);
            sb.Append(Constants.SERIALIZATION_DELIMITER);
            sb.Append(_userArgs);
            sb.Append(Constants.SERIALIZATION_DELIMITER);
            sb.Append(_lineParserName);

            return sb.ToString();            
        }

        /// <summary>
        /// Convert a saved tool configuration into a build tool instance.
        /// </summary>
        /// <param name="s">The saved configuration.</param>
        /// <returns>A BuildTool instance.</returns>
        public static BuildTool Parse(string s)
        {
            string [] items =
                s.Split(Constants.SERIALIZATION_DELIMITER[0]);
            
            if (items.Length != 8) return null;

            BuildTool tool = new BuildTool(
                items[0], new DocumentType(items[1]), items[2]);
            
            tool.Action = items[3];
            tool.Path = items[4];
            tool.Args = items[5];
            tool.UserArgs = items[6];
            tool.LineParserName = items[7];

            return tool;
        }

        /// <summary>
        /// Duplicate an existing build tool.
        /// </summary>
        /// <returns>A BuildTool instance.</returns>
        public BuildTool Clone()
        {
            BuildTool tool = new BuildTool(_id, _documentType, _displayName);
            tool.Action = _toolAction;
            tool.Path = _toolPath;
            tool.Args = _toolArgs;
            tool.UserArgs = _userArgs;
            tool.LineParserName = _lineParserName;
            tool.LineParser = _lineParser;
            tool.BuildCommand = _buildCommand;

            return tool;
        }
    }
}
