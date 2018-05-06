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

namespace QuickSharp.CodeAssist.DotNet
{
    public class Constants
    {
        public const string PLUGIN_NAME = "QuickSharp.CodeAssist.DotNet";
        public const string ASSEMBLY_CACHE_FOLDER = "CodeAssist";
        public const string DEFAULT_DATABASE_FILENAME = "QuickSharp.CodeAssist.DotNet.config";
        public const string LOAD_FROM_LIST_FILENAME = "_loadfrom.txt";

        // Registry keys
        public const string KEY_COLORIZE_TYPES = "ColorizeTypes";
        public const string KEY_COLORIZE_VARIABLES = "ColorizeVariables";
        public const string KEY_COLORIZE_ON_ACTIVATE = "ColorizeOnActivate";
        public const string KEY_COLORIZE_ON_LOOKUP = "ColorizeOnLookup";
        public const string KEY_DATABASE_FILENAME = "DatabaseFilename";
        
        // UI Elements
        public const string UI_OPTIONS_CODE_ASSIST_PAGE = "UI_OPTIONS_CODE_ASSIST_PAGE";
    }
}
