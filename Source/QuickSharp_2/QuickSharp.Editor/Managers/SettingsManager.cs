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

using QuickSharp.Core;
using ScintillaNet;

namespace QuickSharp.Editor
{
    /// <summary>
    /// Manage user-defined editor settings.
    /// </summary>
    public class SettingsManager
    {
        #region Singleton

        private static SettingsManager _singleton;

        /// <summary>
        /// Get a reference to the SettingsManager singleton.
        /// </summary>
        /// <returns>A reference to the SettingsManager.</returns>
        public static SettingsManager GetInstance()
        {
            if (_singleton == null)
                _singleton = new SettingsManager();

            return _singleton;
        }

        #endregion

        private ApplicationManager _applicationManager;
        private IPersistenceManager _persistenceManager;

        /// <summary>
        /// If true allow settings to override editor configuration files.
        /// </summary>
        public bool OverrideConfigFiles { get; set; }

        /// <summary>
        /// The tab size (in spaces).
        /// </summary>
        public int TabSize { get; set; }

        /// <summary>
        /// If true use tabs instead of spaces.
        /// </summary>
        public bool UseTabs { get; set; }

        /// <summary>
        /// If true the backspace key can unindent.
        /// </summary>
        public bool BackspaceUnindents { get; set; }

        /// <summary>
        /// The size of the line number margin.
        /// </summary>
        public int LineNumberMarginSize { get; set; }

        /// <summary>
        /// If true show indentation guides.
        /// </summary>
        public bool ShowIndentationGuides { get; set; }

        /// <summary>
        /// If true show folding.
        /// </summary>
        public bool ShowFolding { get; set; }

        /// <summary>
        /// The style used to indicate folded sections.
        /// </summary>
        public int FoldStyle { get; set; }

        /// <summary>
        /// The style of fold margin indicators.
        /// </summary>
        public int FoldMarkerStyle { get; set; }

        /// <summary>
        /// The name of the base font.
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// The size of the base font.
        /// </summary>
        public float FontSize { get; set; }

        /// <summary>
        /// If true brace matching should be displayed.
        /// </summary>
        public bool MatchBraces { get; set; }

        /// <summary>
        /// If true wrap long lines.
        /// </summary>
        public bool WordWrap { get; set; }

        /// <summary>
        /// Color mode used from printing
        /// </summary>
        public PrintColorMode PrintingColorMode { get; set; }

        private SettingsManager()
        {
            _applicationManager =
                ApplicationManager.GetInstance();

            _persistenceManager =
                _applicationManager.GetPersistenceManager(
                    Constants.PLUGIN_NAME);

            Update();
        }

        /// <summary>
        /// Retrieve the settings from the session persistence store (e.g. the registry).
        /// </summary>
        public void Update()
        {
            OverrideConfigFiles = _persistenceManager.ReadBoolean(
                Constants.KEY_OVERRIDE_CONFIG_FILES, true);
            TabSize = _persistenceManager.ReadInt(
                Constants.KEY_TAB_SIZE, 4);
            UseTabs = _persistenceManager.ReadBoolean(
                Constants.KEY_USE_TABS, false);
            BackspaceUnindents = _persistenceManager.ReadBoolean(
                Constants.KEY_BACKSPACE_UNINDENTS, true);
            ShowIndentationGuides = _persistenceManager.ReadBoolean(
                Constants.KEY_SHOW_INDENTATION_GUIDES, false);
            LineNumberMarginSize = _persistenceManager.ReadInt(
                Constants.KEY_LINE_NUMBER_MARGIN_SIZE, 36);
            ShowFolding = _persistenceManager.ReadBoolean(
                Constants.KEY_SHOW_FOLDING, true);
            FoldStyle = _persistenceManager.ReadInt(
                Constants.KEY_FOLD_STYLE, 2);
            FoldMarkerStyle = _persistenceManager.ReadInt(
                Constants.KEY_FOLD_MARKER_STYLE, 0);
            FontName = _persistenceManager.ReadString(
                Constants.KEY_FONT_NAME, Constants.DEFAULT_FONT_NAME);
            FontSize = (float)_persistenceManager.ReadDouble(
                Constants.KEY_FONT_SIZE, 10.0F);
            MatchBraces = _persistenceManager.ReadBoolean(
                Constants.KEY_MATCH_BRACES, true);
            WordWrap = _persistenceManager.ReadBoolean(
                Constants.KEY_WORD_WRAP, false);

            int colorMode = _persistenceManager.ReadInt(
                Constants.KEY_PRINTING_COLOR_MODE, 0);

            PrintingColorMode = GetPrintingColorMode(colorMode);
        }

        /// <summary>
        /// Save the editor settings to the session persistence store.
        /// </summary>
        public void SaveEditorSettings()
        {
            _persistenceManager.WriteBoolean(
                Constants.KEY_OVERRIDE_CONFIG_FILES, OverrideConfigFiles);
            _persistenceManager.WriteInt(
                Constants.KEY_TAB_SIZE, TabSize);
            _persistenceManager.WriteBoolean(
                Constants.KEY_USE_TABS, UseTabs);
            _persistenceManager.WriteBoolean(
                Constants.KEY_BACKSPACE_UNINDENTS, BackspaceUnindents);
            _persistenceManager.WriteBoolean(
                Constants.KEY_SHOW_INDENTATION_GUIDES, ShowIndentationGuides);
            _persistenceManager.WriteInt(
                Constants.KEY_LINE_NUMBER_MARGIN_SIZE, LineNumberMarginSize);
            _persistenceManager.WriteBoolean(
                Constants.KEY_SHOW_FOLDING, ShowFolding);
            _persistenceManager.WriteInt(
                Constants.KEY_FOLD_STYLE, FoldStyle);
            _persistenceManager.WriteInt(
                Constants.KEY_FOLD_MARKER_STYLE, FoldMarkerStyle);
            _persistenceManager.WriteString(
                Constants.KEY_FONT_NAME, FontName);
            _persistenceManager.WriteDouble(
                Constants.KEY_FONT_SIZE, FontSize);
        }

        /// <summary>
        /// Save the global settings to the session persistence store.
        /// </summary>
        public void SaveGlobalSettings()
        {
            _persistenceManager.WriteBoolean(
                Constants.KEY_MATCH_BRACES, MatchBraces);
            _persistenceManager.WriteBoolean(
                Constants.KEY_WORD_WRAP, WordWrap);

            int colorMode = GetPrintingColorModeCode(PrintingColorMode);

            _persistenceManager.WriteInt(
                Constants.KEY_PRINTING_COLOR_MODE, colorMode);
        }

        #region Helpers

        /// <summary>
        /// Folded section marker styles.
        /// </summary>
        public FoldFlag Flags
        {
            get
            {
                if (FoldStyle == 1) return FoldFlag.LineBeforeContracted;
                if (FoldStyle == 2) return FoldFlag.LineAfterContracted;
                return 0;
            }
        }

        /// <summary>
        /// Fold margin marker styles.
        /// </summary>
        public FoldMarkerScheme MarkerScheme
        {
            get
            {
                if (FoldMarkerStyle == 1)
                    return FoldMarkerScheme.CirclePlusMinus;
                if (FoldMarkerStyle == 2)
                    return FoldMarkerScheme.PlusMinus;
                if (FoldMarkerStyle == 3)
                    return FoldMarkerScheme.Arrow;
                return FoldMarkerScheme.BoxPlusMinus;
            }
        }

        /// <summary>
        /// Translate PrintColorMode to int.
        /// </summary>
        /// <param name="mode">A PrintColorMode value.</param>
        /// <returns>The value as an int (0, 1 or 2).</returns>
        public int GetPrintingColorModeCode(PrintColorMode mode)
        {
            switch (mode)
            {
                case PrintColorMode.BlackOnWhite:
                    return 1;
                case PrintColorMode.ColorOnWhite:
                    return 2;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Translate an int to the corresponding PrintColorMode value.
        /// </summary>
        /// <param name="code">An int (0, 1 or 2).</param>
        /// <returns>A PrintColorMode value.</returns>
        public PrintColorMode GetPrintingColorMode(int code)
        {
            switch (code)
            {
                case 1:
                    return PrintColorMode.BlackOnWhite;
                case 2:
                    return PrintColorMode.ColorOnWhite;
                default:
                    return PrintColorMode.Normal;
            }
        }

        #endregion
    }
}

