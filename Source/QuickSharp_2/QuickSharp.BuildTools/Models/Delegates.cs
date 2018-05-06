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
    /// Retrieves a build command from a provider supplied by a language support plugin.
    /// The plugin contains the actual build functionality and provides a method to create
    /// a build command to represent this functionality. A delegate to this method is registered
    /// with the build system. When the delegate is invoked the plugin provider takes a build tool
    /// definition and source file information to creates the build command to perform the actual build task.
    /// </summary>
    /// <param name="buildTool">The build tool to be passed to the build command provider.</param>
    /// <param name="srcInfo">The source file to be passed to the build command provider.</param>
    /// <returns>A BuildCommand instance created by the build command provider.</returns>
    public delegate BuildCommand BuildCommandDelegate(
        BuildTool buildTool, FileInfo srcInfo);
}
