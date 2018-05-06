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

namespace QuickSharp.Editor
{
    /// <summary>
    /// Access the constants used in the plugin.
    /// </summary>
    public class Constants
    {
        public const string PLUGIN_NAME = "QuickSharp.Editor";
        public const string CONFIG_DIR_NAME = "Scintilla";
        public const string DEFAULT_SCINTILLA_LEXER = "default";
        public const string DEFAULT_FONT_NAME = "Courier New";
        public const string UNTITLED_FILENAME = "untitled";
        public const string SNIPPET_FILE_PATTERN = "*.txt";
        public const string SNIPPETS_FOLDER = "Snippets";
        public const int BOOKMARK_MARKER = 1;
        public const int BOOKMARK_MASK = 2; // 0x01

        // Registry Keys
        public const string KEY_OVERRIDE_CONFIG_FILES = "OverrideConfigFiles";
        public const string KEY_USE_TABS = "UseTabs";
        public const string KEY_TAB_SIZE = "TabSize";
        public const string KEY_BACKSPACE_UNINDENTS = "BackspaceUnindents";
        public const string KEY_SHOW_INDENTATION_GUIDES = "ShowIndentationGuides";
        public const string KEY_LINE_NUMBER_MARGIN_SIZE = "LineNumberMarginSize";
        public const string KEY_BOOKMARK_MARGIN_SIZE = "BookmarkMarginSize";
        public const string KEY_SHOW_FOLDING = "ShowFolding";
        public const string KEY_FOLD_STYLE = "FoldStyle";
        public const string KEY_FOLD_MARKER_STYLE = "FoldMarkerStyle";
        public const string KEY_FONT_NAME = "FontName";
        public const string KEY_FONT_SIZE = "FontSize";
        public const string KEY_MATCH_BRACES = "MatchBraces";
        public const string KEY_WORD_WRAP = "WordWrap";
        public const string KEY_PRINTING_COLOR_MODE = "PrintColorMode";

        // UI Elements
        public const string UI_FILE_MENU_PRINT = "UI_FILE_MENU_PRINT";
        public const string UI_FILE_MENU_PRINT_PREVIEW = "UI_FILE_MENU_PRINT_PREVIEW";
        public const string UI_FILE_MENU_PAGE_SETUP = "UI_FILE_MENU_PAGE_SETUP";
        public const string UI_EDIT_MENU = "UI_EDIT_MENU";
        public const string UI_EDIT_MENU_UNDO = "UI_EDIT_MENU_UNDO";
        public const string UI_EDIT_MENU_REDO = "UI_EDIT_MENU_REDO";
        public const string UI_EDIT_MENU_CUT = "UI_EDIT_MENU_CUT";
        public const string UI_EDIT_MENU_COPY = "UI_EDIT_MENU_COPY";
        public const string UI_EDIT_MENU_PASTE = "UI_EDIT_MENU_PASTE";
        public const string UI_EDIT_MENU_SELECT_ALL = "UI_EDIT_MENU_SELECT_ALL";
        public const string UI_EDIT_MENU_FIND = "UI_EDIT_MENU_FIND";
        public const string UI_EDIT_MENU_REPLACE = "UI_EDIT_MENU_REPLACE";
        public const string UI_EDIT_MENU_GOTO = "UI_EDIT_MENU_GOTO";
        public const string UI_EDIT_MENU_ADVANCED = "UI_EDIT_MENU_ADVANCED";
        public const string UI_EDIT_MENU_MAKE_UPPERCASE = "UI_EDIT_MENU_MAKE_UPPERCASE";
        public const string UI_EDIT_MENU_MAKE_LOWERCASE = "UI_EDIT_MENU_MAKE_LOWERCASE";
        public const string UI_EDIT_MENU_LINE_COMMENT = "UI_EDIT_MENU_LINE_COMMENT";
        public const string UI_EDIT_MENU_BLOCK_COMMENT = "UI_EDIT_MENU_BLOCK_COMMENT";
        public const string UI_EDIT_MENU_VIEW_WHITESPACE = "UI_EDIT_MENU_VIEW_WHITESPACE";
        public const string UI_EDIT_MENU_TRIM_WHITESPACE = "UI_EDIT_MENU_TRIM_WHITESPACE";
        public const string UI_EDIT_MENU_WORD_WRAP = "UI_EDIT_MENU_WORD_WRAP";
        public const string UI_EDIT_MENU_SET_READ_ONLY = "UI_EDIT_MENU_SET_READ_ONLY";
        public const string UI_EDIT_MENU_BOOKMARKS = "UI_EDIT_MENU_BOOKMARKS";
        public const string UI_EDIT_MENU_TOGGLE_BOOKMARK = "UI_EDIT_MENU_TOGGLE_BOOKMARK";
        public const string UI_EDIT_MENU_CLEAR_BOOKMARKS = "UI_EDIT_MENU_CLEAR_BOOKMARKS";
        public const string UI_EDIT_MENU_NEXT_BOOKMARK = "UI_EDIT_MENU_NEXT_BOOKMARK";
        public const string UI_EDIT_MENU_PREV_BOOKMARK = "UI_EDIT_MENU_PREV_BOOKMARK";
        public const string UI_EDIT_MENU_FOLDING = "UI_EDIT_MENU_FOLDING";
        public const string UI_EDIT_MENU_COLLAPSE_ALL = "UI_EDIT_MENU_COLLAPSE_ALL";
        public const string UI_EDIT_MENU_EXPAND_ALL = "UI_EDIT_MENU_EXPAND_ALL";
        public const string UI_EDIT_MENU_TOGGLE_FOLD = "UI_EDIT_MENU_TOGGLE_FOLD";
        public const string UI_EDIT_MENU_MACROS = "UI_EDIT_MENU_MACROS";
        public const string UI_EDIT_MENU_MACRO_RECORD = "UI_EDIT_MENU_MACRO_RECORD";
        public const string UI_EDIT_MENU_MACRO_PLAY = "UI_EDIT_MENU_MACRO_PLAY";
        public const string UI_EDIT_MENU_MACRO_LOAD = "UI_EDIT_MENU_MACRO_LOAD";
        public const string UI_EDIT_MENU_MACRO_SAVE = "UI_EDIT_MENU_MACRO_SAVE";
        public const string UI_EDIT_MENU_SNIPPETS = "UI_EDIT_MENU_SNIPPETS";
        public const string UI_EDIT_MENU_MANAGE_SNIPPETS = "UI_EDIT_MENU_MANAGE_SNIPPETS";
        public const string UI_EDIT_MENU_MANAGE_SNIPPETS_SEP = "UI_EDIT_MENU_MANAGE_SNIPPETS_SEP";
        public const string UI_EDIT_MENU_ENCODING = "UI_EDIT_MENU_ENCODING";
        public const string UI_EDIT_MENU_ENCODING_ANSI = "UI_EDIT_MENU_ENCODING_ANSI";
        public const string UI_EDIT_MENU_ENCODING_UTF8 = "UI_EDIT_MENU_ENCODING_UTF8";
        public const string UI_EDIT_MENU_ENCODING_UTF16BE = "UI_EDIT_MENU_ENCODING_UTF16BE";
        public const string UI_EDIT_MENU_ENCODING_UTF16LE = "UI_EDIT_MENU_ENCODING_UF16LE";
        public const string UI_EDIT_MENU_LINE_ENDING = "UI_EDIT_MENU_LINE_ENDING";
        public const string UI_EDIT_MENU_LINE_ENDING_CRLF = "UI_EDIT_MENU_LINE_ENDING_CRLF";
        public const string UI_EDIT_MENU_LINE_ENDING_LF = "UI_EDIT_MENU_LINE_ENDING_LF";
        public const string UI_EDIT_MENU_LINE_ENDING_CR = "UI_EDIT_MENU_LINE_ENDING_CR";
        public const string UI_OPTIONS_PAGE_EDITOR = "UI_OPTIONS_PAGE_EDITOR";
        public const string UI_OPTIONS_PAGE_GLOBAL = "UI_OPTIONS_PAGE_GLOBAL";
        public const string UI_STATUSBAR_CURSOR_POSITION_INDICATOR = "UI_STATUSBAR_CURSOR_POSITION_INDICATOR";
    }
}
 