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
using QuickSharp.Editor;
using QuickSharp.Core;

namespace QuickSharp.CodeAssist.Html
{
    public class PhpCodeAssistProvider :
            XhtmlCodeAssistProvider, ICodeAssistProvider
    {
        public PhpCodeAssistProvider()
        {
            // Add the server-side script tag.
            xhtmlTagDictionary.Add("?php", new TagItem("", 2));
            html5TagDictionary.Add("?php", new TagItem("", 2));

            // Define the insertion template for the tag.
            scriptTag = String.Format("{0} {1} ?>",
                QuickSharp.CodeAssist.Constants.INSERTION_TEMPLATE_TEXT_PLACEHOLDER,
                QuickSharp.CodeAssist.Constants.INSERTION_TEMPLATE_CPOS_PLACEHOLDER);
        }

        public DocumentType DocumentType
        {
            get { return new DocumentType(Constants.DOCUMENT_TYPE_PHP); }
        }

        public bool IsAvailable
        {
            get { return true; }
        }

        public void DocumentActivated(ScintillaEditForm document)
        {
        }

        public LookupList GetLookupList(ScintillaEditForm document)
        {
            string content = document.GetContent() as string;
            if (content == null) return null;

            if (!IsPhpCodeSection(content, document.Editor.CurrentPos))
                return GetXhtmlLookupList(document);
            else
                return null;
        }

        private bool IsPhpCodeSection(string content, int pos)
        {
            string preText = content.Substring(0, pos).ToLower();

            int s1 = preText.LastIndexOf("<?php");
            int s2 = preText.LastIndexOf("?>");

            if (s1 == -1) return false;
            if (s1 != -1 && s2 == -1) return true;
            return (s1 > s2);
        }
    }
}