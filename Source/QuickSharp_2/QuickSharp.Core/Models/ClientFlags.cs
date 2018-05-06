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

namespace QuickSharp.Core
{
    /// <summary>
    /// Provides access to the ClientFlag strings available within the QuickSharp core plugins.
    /// </summary>
    public class ClientFlags
    {
        // Plugin specific
        public const string BuildToolsDisableBuildSettingsPage = "BuildToolsDisableBuildSettingsPage";
        public const string BuildToolsDisableCompilerUI = "BuildToolsDisableCompilerUI";
        public const string CodeAssistDotNetAutoPopulateDatabase = "CodeAssistDotNetAutoPopulateDatabase";
        public const string CodeAssistDotNetDisableReloadDatabase = "CodeAssistDotNetDisableReloadDatabase";
        public const string CodeAssistDotNetDisableColorization = "CodeAssistDotNetDisableColorization";
        public const string CodeAssistDotNetDisableDefaultWebNamespaces = "CodeAssistDotNetDisableDefaultWebNamespaces";
        public const string CodeAssistObjectBrowserIncludeWorkspace = "CodeAssistObjectBrowserIncludeWorkspace";
        public const string CodeAssistObjectBrowserUseMainToolbar = "CodeAssistObjectBrowserUseMainToolbar";
        public const string EditorChangeDirectoryOnSave = "EditorChangeDirectoryOnSave";
        public const string EditorDisableCodeFeatures = "EditorDisableCodeFeatures";
        public const string EditorDisableDragAndDropFileOpen = "EditorDisableDragAndDropFileOpen";
        public const string EditorDisableFileTimestampCheck = "EditorDisableFileTimestampCheck";
        public const string ExplorerCurrentDirectoryFollowsRoot = "ExplorerCurrentDirectoryFollowsRoot";
        public const string ExplorerDisableWindowsShell = "ExplorerDisableWindowsShell";
        public const string ExplorerDockRightByDefault = "ExplorerDockRightByDefault";
        public const string ExplorerEnableTitleBarUpdate = "ExplorerEnableTitleBarUpdate";
        public const string ExplorerHideByDefault = "ExplorerHideByDefault";
        public const string ExplorerStartFromCurrentDirectory = "ExplorerStartsFromCurrentDirectory";
        public const string FindInFilesShowOutputOnFind = "FindInFilesShowOutputOnFind";
        public const string TextEditorClaimUnknownDocument = "TextEditorClaimUnknownDocument";
        public const string TextEditorClaimEmptyDocument = "TextEditorClaimEmptyDocument";
        public const string TextEditorDisableDocumentTypeConfigFile = "TextEditorDisableDocumentTypeConfigFile";
        public const string OutputHideByDefault = "OutputHideByDefault";
        public const string SqlEditorEnableXsdExport = "SqlEditorEnableXsdExport";
        public const string SqlManagerEnableDbmlExtract = "SqlManagerEnableDbmlExtract";
        public const string WorkspaceDisableWindowsShell = "WorkspaceDisableWindowsShell";
        public const string WorkspaceEnableShowSource = "WorkspaceEnableShowSource";
        public const string WorkspaceEnableTitleBarUpdate = "WorkspaceEnableTitleBarUpdate";
        public const string WorkspaceHideByDefault = "WorkspaceHideByDefault";
        public const string WorkspaceDockLeftByDefault = "WorkspaceDockLeftByDefault";

        // Core
        public const string CoreDisableDragAndDropFileOpen = "CoreDisableDragAndDropFileOpen";
        public const string CoreDisableVisualStudio2008Theme = "CoreDisableVisualStudio2008Theme";
        public const string CoreDisableVisualStudio2010Theme = "CoreDisableVisualStudio2010Theme";
    }
}
