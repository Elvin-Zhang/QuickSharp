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
    public class ErbCodeAssistProvider : RubyCodeAssistProvider, ICodeAssistProvider
    {
        public DocumentType DocumentType
        {
            get { return new DocumentType(Constants.DOCUMENT_TYPE_ERB); }
        }
    }

    public class RhtmlCodeAssistProvider : RubyCodeAssistProvider, ICodeAssistProvider
    {
        public DocumentType DocumentType
        {
            get { return new DocumentType(Constants.DOCUMENT_TYPE_RHTML); }
        }
    }

    public abstract class RubyCodeAssistProvider : XhtmlCodeAssistProvider
    {
        public RubyCodeAssistProvider()
        {
            // Add the server-side script tags.
            xhtmlTagDictionary.Add("%", new TagItem("", 2));
            xhtmlTagDictionary.Add("%=", new TagItem("", 2));
            html5TagDictionary.Add("%", new TagItem("", 2));
            html5TagDictionary.Add("%=", new TagItem("", 2));

            // Define the insertion template for the tags.
            scriptTag = String.Format("{0} {1} %>",
                QuickSharp.CodeAssist.Constants.INSERTION_TEMPLATE_TEXT_PLACEHOLDER,
                QuickSharp.CodeAssist.Constants.INSERTION_TEMPLATE_CPOS_PLACEHOLDER);
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

            if (!IsRubyCodeSection(content, document.Editor.CurrentPos))
                return GetXhtmlLookupList(document);
            else
                return null;
        }

        private bool IsRubyCodeSection(string content, int pos)
        {
            content = content.Substring(pos).ToLower();

            int s1 = content.IndexOf("%>");
            if (s1 == -1) return false;
            int s2 = content.IndexOf("<%");
            if (s2 == -1) return true;
            return s2 > s1;
        }
    }
}