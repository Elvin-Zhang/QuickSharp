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
using System.Text.RegularExpressions;
using System.Collections.Generic;
using QuickSharp.Core;
using QuickSharp.Editor;
using ScintillaNet;

namespace QuickSharp.CodeAssist.Html
{
    public abstract class XhtmlCodeAssistProvider
    {
        protected Dictionary<String, TagItem> xhtmlTagDictionary;
        protected Dictionary<String, String> xhtmlAttrDictionary;
        protected Dictionary<String, TagItem> html5TagDictionary;
        protected Dictionary<String, String> html5AttrDictionary;
        protected string scriptTag;

        private string _fullTag;
        private string _closingTag;
        private string _emptyTag;

        private const string CORE = "class id title style";
        private const string I18N = "dir lang xml:lang";
        private const string EVENT = "onclick ondblclick onmousedown onmouseup onmouseover onmousemove onmouseout onkeypress onkeydown onkeyup";
        private const string GLOBAL = " accesskey class contenteditable contextmenu dir draggable dropzone hidden id lang spellcheck style tabindex title " +
            "onabort onblur oncanplay oncanplaythrough onchange onclick oncontextmenu oncuechange ondblclick ondrag ondragend ondragenter " +
            "ondragleave ondragover ondragstart ondrop ondurationchange onemptied onended onerror onfocus oninput oninvalid onkeydown onkeypress " +
            "onkeyup onload onloadeddata onloadedmetadata onloadstart onmousedown onmousemove onmouseout onmouseover onmouseup onmousewheel onpause " +
            "onplay onplaying onprogress onratechange onreadystatechange onreset onscroll onseeked onseeking onselect onshow onstalled onsubmit " +
            "onsuspend ontimeupdate onvolumechange onwaiting";

        public XhtmlCodeAssistProvider()
        {
            /*
             * Load the tag data.
             */

            LoadDictionaries();

            /*
             * Define the insertion templates for the tags. Because we can have
             * empty tags (such as "<img />") we need two different insertion templates.
             */

            string textPlaceHolder = QuickSharp.CodeAssist.
                Constants.INSERTION_TEMPLATE_TEXT_PLACEHOLDER;
            string cposPlaceHolder = QuickSharp.CodeAssist.
                Constants.INSERTION_TEMPLATE_CPOS_PLACEHOLDER;

            /*
             * Only the XML variant of HTML5 is support - tags must always be
             * closed and attributes quoted.
             */

            _fullTag = String.Format("{0}{1}></{0}>", textPlaceHolder, cposPlaceHolder);
            _closingTag = String.Format("{0}>", textPlaceHolder);
            _emptyTag = String.Format("{0} {1}/>", textPlaceHolder, cposPlaceHolder);
        }

        #region Code Assist Driver

        public LookupList GetXhtmlLookupList(ScintillaEditForm document)
        {
            string text = document.GetContent() as string;
            if (String.IsNullOrEmpty(text)) return null;

            text = text.Substring(0, document.Editor.CurrentPos);

            // XHTML of HTML5?
            bool isHTML5 = IsHTML5(text);

            // Split the current line into opening-tag delimited segments
            string[] split = text.Split('<');

            // If no split we have no opening tag
            if (split.Length == 1) return null;

            // Current tag is start of last split segment
            string target = split[split.Length - 1];

            // If we have a closed tag we are in content, not a tag
            if (target.IndexOf('>') != -1) return null;

            // If no spaces in target text must be tag name (or part of)
            if (target.IndexOf(' ') == -1)
            {
                /*
                 * Tag lookup.
                 */

                List<LookupListItem> tagList = GetTagList(isHTML5);
                List<String> insertionTemplates = new List<String>();

                // Full tag is either complete or just the closing tag (index = 0)
                if (target.StartsWith("/"))
                {
                    target = target.TrimStart('/');
                    insertionTemplates.Add(_closingTag);
                }
                else
                {
                    insertionTemplates.Add(_fullTag);
                }

                // Add the template for closed tags (index = 1)
                insertionTemplates.Add(_emptyTag);

                // Add the server-side script template (index = 2)
                insertionTemplates.Add(scriptTag);

                return new LookupList(target, tagList, insertionTemplates);
            }
            else
            {
                /*
                 * Target consists of tag name followed by one or more attributes.
                 * Attributes are quote delimited and may have spaces between the
                 * attribute name and the value string. The values may be singular
                 * or multiple separated by semicolons or spaces.
                 * 
                 * <tag a="b" c="d;e" foo = " bar baz|
                 * 
                 * For the lookup the attribute and current values will be foo
                 * and baz (or blank if there is a space or ';' after baz).
                 */

                // Split into tag and trailing content
                string[] split2 = target.Split();
                string tagName = split2[0];
                string tagContent = target.Substring(tagName.Length).TrimStart();

                if (InAttributeValue(target))
                {
                    /*
                     * Attribute value lookup.
                     */

                    // Get the attribute name
                    string[] split3 = tagContent.Split('=');
                    string attrName = String.Empty;

                    if (split.Length > 1)
                    {
                        string[] split4 = split3[split3.Length - 2].Trim().Split();
                        attrName = split4[split4.Length - 1].Trim();
                    }

                    // Get the lookup lead text
                    string valText = split3[split3.Length - 1];
                    int i = valText.Length - 1;

                    // Allow for multiple values
                    if (tagContent[i] != ';' && tagContent[i] != ' ')
                    {
                        string[] split4 = tagContent.Split(new Char[] { ' ', ';', '"' });
                        valText = split4[split4.Length - 1];
                    }

                    List<LookupListItem> valList =
                        GetAttributeValueList(tagName, attrName, isHTML5);

                    if (valList == null)
                        return null;
                    else
                        return new LookupList(valText, valList);
                }
                else
                {
                    /*
                     * Attribute lookup.
                     */

                    // Get the attribute name or partial name
                    string[] split3 = tagContent.Split();
                    string attrName = split3[split3.Length - 1];

                    List<LookupListItem> attrList = GetAttributeList(tagName, isHTML5);
                    if (attrList == null) return null;

                    string template = String.Format("{0}=\"{1}\"",
                        QuickSharp.CodeAssist.Constants.
                            INSERTION_TEMPLATE_TEXT_PLACEHOLDER,
                        QuickSharp.CodeAssist.Constants.
                            INSERTION_TEMPLATE_CPOS_PLACEHOLDER);

                    return new LookupList(attrName, attrList, template);
                }
            }
        }

        #endregion

        #region Helpers

        private bool IsHTML5(string text)
        {
            Regex re = new Regex(@"\s*<!DOCTYPE\s+html\s*>");
            return (re.Match(text).Success);
        }

        private bool InAttributeValue(string target)
        {
            /*
             * If we are within the quotes of an attribute value
             * the number of preceding quotes will be odd.
             */

            int count = 0;
            int i = 0;

            while (i < target.Length)
            {
                if (target[i] == '\"') count++;
                i++;
            }

            return ((count % 2) == 1);
        }

        #endregion

        #region Tag Lookup

        private List<LookupListItem> GetTagList(bool isHTML5)
        {
            // Set the dictionary
            Dictionary<String, TagItem> tagDictionary =
                (isHTML5 ? html5TagDictionary : xhtmlTagDictionary);

            // Get the tags
            List<LookupListItem> list = new List<LookupListItem>();
    
            foreach (string tag in tagDictionary.Keys)
            {
                LookupListItem item = new LookupListItem();
                item.DisplayText = tag;
                item.InsertText = tag;
                item.TemplateIndex = tagDictionary[tag].TemplateIndex;
                item.Category = QuickSharp.CodeAssist.Constants.WEBCONTROL;
                list.Add(item);
            }

            return list;
        }

        #endregion

        #region Attribute Lookup

        private List<LookupListItem> GetAttributeList(string tag, bool isHTML5)
        {
            string attributes = String.Empty;

            if (isHTML5)
            {
                if (!html5TagDictionary.ContainsKey(tag)) return null;
                attributes = html5TagDictionary[tag].Attributes;
                attributes += GLOBAL;
            }
            else
            {
                if (!xhtmlTagDictionary.ContainsKey(tag)) return null;
                attributes = xhtmlTagDictionary[tag].Attributes;
                attributes = attributes.Replace("CORE", CORE);
                attributes = attributes.Replace("I18N", I18N);
                attributes = attributes.Replace("EVENT", EVENT);
            }

            attributes = attributes.Trim();
            if (attributes == String.Empty) return null;

            List<LookupListItem> list = new List<LookupListItem>();

            string[] split = attributes.Split();

            foreach (string attr in split)
            {
                LookupListItem item = new LookupListItem();
                item.DisplayText = attr;
                item.InsertText = attr;
                item.Category = QuickSharp.CodeAssist.Constants.PROPERTIES;
                list.Add(item);
            }

            return list;
        }

        #endregion

        #region Attribute Value Lookup

        private List<LookupListItem> GetAttributeValueList(string tag, string attr, bool isHTML5)
        {
            // Set the dictionary
            Dictionary<String, String> attrDictionary =
                (isHTML5 ? html5AttrDictionary : xhtmlAttrDictionary);

            // Try tag specific value first...
            string key = String.Format("{0}|{1}", tag, attr);

            // If not found, try generic...
            if (!attrDictionary.ContainsKey(key))
            {
                key = attr;

                // Fail if not found
                if (!attrDictionary.ContainsKey(key))
                    return null;
            }

            string values = attrDictionary[key].Trim();
            if (values == String.Empty) return null;

            List<LookupListItem> list = new List<LookupListItem>();

            string[] split = values.Split();
            foreach (string val in split)
            {
                LookupListItem item = new LookupListItem();
                item.DisplayText = val;
                item.InsertText = val;
                item.Category = QuickSharp.CodeAssist.Constants.CONSTANT;
                list.Add(item);
            }

            return list;
        }

        #endregion

        #region Tag Data

        protected struct TagItem
        {
            public string Attributes;
            public int TemplateIndex;

            public TagItem(string attributes, int templateIndex)
            {
                Attributes = attributes;
                TemplateIndex = templateIndex;
            }
        }

        private void LoadDictionaries()
        {
            /*
             * Tag support is: XHTML 1.0 Strict/HTML4 Strict and HTML5.
             * 
             * HTML5 based on W3C Working Draft 24 May 2011.
             */

            #region XHTML: Tag->Attribute

            xhtmlTagDictionary = new Dictionary<String, TagItem>();
            xhtmlTagDictionary.Add("html", new TagItem("I18N xmlns", 0));
            xhtmlTagDictionary.Add("a", new TagItem("CORE I18N EVENT charset type name href hreflang rel rev accesskey shape coords tabindex onfocus onblur", 0));
            xhtmlTagDictionary.Add("link", new TagItem("CORE I18N EVENT charset href hreflang type rel rev media", 1));
            xhtmlTagDictionary.Add("img", new TagItem("CORE I18N EVENT src alt longdesc name height width usemap ismap", 1));
            xhtmlTagDictionary.Add("object", new TagItem("CORE I18N EVENT declare classid codebase data type codetype archive standby height width name usemap tabindex", 0));
            xhtmlTagDictionary.Add("hr", new TagItem("CORE I18N EVENT", 0));            
            xhtmlTagDictionary.Add("p", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("h1", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("h2", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("h3", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("h4", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("h5", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("h6", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("pre", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("q", new TagItem("CORE I18N EVENT cite", 0));
            xhtmlTagDictionary.Add("blockquote", new TagItem("CORE I18N EVENT cite", 0));
            xhtmlTagDictionary.Add("ins", new TagItem("CORE I18N EVENT cite datetime", 0));
            xhtmlTagDictionary.Add("del", new TagItem("CORE I18N EVENT cite datetime", 0));
            xhtmlTagDictionary.Add("dl", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("dt", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("dd", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("ol", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("ul", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("li", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("form", new TagItem("CORE I18N EVENT action method enctype accept name onsubmit onreset accept-charset", 0));
            xhtmlTagDictionary.Add("label", new TagItem("CORE I18N EVENT for accesskey onfocus onblur", 0));
            xhtmlTagDictionary.Add("input", new TagItem("CORE I18N EVENT type name value checked disabled readonly size maxlength src alt usemap ismap tabindex accesskey onfocus onblur onselect onchange accept", 1));
            xhtmlTagDictionary.Add("select", new TagItem("CORE I18N EVENT name size multiple disabled tabindex onfocus onblur onchange", 0));
            xhtmlTagDictionary.Add("optgroup", new TagItem("CORE I18N EVENT label disabled", 0));
            xhtmlTagDictionary.Add("option", new TagItem("CORE I18N EVENT selected disabled label value", 0));
            xhtmlTagDictionary.Add("textarea", new TagItem("CORE I18N EVENT name rows cols disabled readonly tabindex accesskey onfocus onblur onselect onchange", 0));
            xhtmlTagDictionary.Add("fieldset", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("legend", new TagItem("CORE I18N EVENT accesskey", 0));
            xhtmlTagDictionary.Add("button", new TagItem("CORE I18N EVENT name value type disabled tabindex accesskey onfocus onblur", 0));
            xhtmlTagDictionary.Add("table", new TagItem("CORE I18N EVENT summary width border frame rules cellspacing cellpadding", 0));
            xhtmlTagDictionary.Add("caption", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("colgroup", new TagItem("CORE I18N EVENT span align valign char charoff", 0));
            xhtmlTagDictionary.Add("col", new TagItem("CORE I18N EVENT span width align valign char charoff", 1));
            xhtmlTagDictionary.Add("thead", new TagItem("CORE I18N EVENT align valign char charoff", 0));
            xhtmlTagDictionary.Add("tbody", new TagItem("CORE I18N EVENT align valign char charoff", 0));
            xhtmlTagDictionary.Add("tfoot", new TagItem("CORE I18N EVENT align valign char charoff", 0));
            xhtmlTagDictionary.Add("tr", new TagItem("CORE I18N EVENT align valign char charoff", 0));
            xhtmlTagDictionary.Add("td", new TagItem("CORE I18N EVENT abbr axis headers scope rowspan colspan align valign char charoff", 0));
            xhtmlTagDictionary.Add("th", new TagItem("CORE I18N EVENT abbr axis headers scope rowspan colspan align valign char charoff", 0));
            xhtmlTagDictionary.Add("head", new TagItem("I18N profile", 0));
            xhtmlTagDictionary.Add("title", new TagItem("I18N", 0));
            xhtmlTagDictionary.Add("base", new TagItem("href", 1));
            xhtmlTagDictionary.Add("meta", new TagItem("I18N content name http-equiv scheme", 1));
            xhtmlTagDictionary.Add("style", new TagItem("I18N type media title", 0));
            xhtmlTagDictionary.Add("script", new TagItem("type src charset defer", 0));
            xhtmlTagDictionary.Add("noscript", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("span", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("bdo", new TagItem("CORE dir xml:lang lang", 0)); 
            xhtmlTagDictionary.Add("br", new TagItem("CORE", 1));
            xhtmlTagDictionary.Add("body", new TagItem("CORE I18N EVENT onload onunload", 0));
            xhtmlTagDictionary.Add("address", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("div", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("map", new TagItem("CORE I18N EVENT name", 0));
            xhtmlTagDictionary.Add("area", new TagItem("CORE I18N EVENT shape coords href nohref alt tabindex accesskey onfocus onblur", 1));
            xhtmlTagDictionary.Add("em", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("strong", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("dfn", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("code", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("samp", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("kbd", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("var", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("cite", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("abbr", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("acronym", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("param", new TagItem("id name value type valuetype", 1));
            
            // Additional HTML4 tags
            xhtmlTagDictionary.Add("sub", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("sup", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("tt", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("i", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("b", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("big", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("small", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("strike", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("s", new TagItem("CORE I18N EVENT", 0));
            xhtmlTagDictionary.Add("u", new TagItem("CORE I18N EVENT", 0));

            #endregion

            #region XHTML: Attribute->Values

            xhtmlAttrDictionary = new Dictionary<String, String>();

            // Generic
            xhtmlAttrDictionary.Add("dir", "ltr rtl");
            xhtmlAttrDictionary.Add("align", "left center right justify char");
            xhtmlAttrDictionary.Add("valign", "top middle bottom baseline");
            xhtmlAttrDictionary.Add("scope", "row col rowgroup colgroup");
            xhtmlAttrDictionary.Add("media", "screen tty tv projection handheld print braille aural all");
            xhtmlAttrDictionary.Add("shape", "rect circle poly default");
            xhtmlAttrDictionary.Add("rel", "alternate stylesheet start next prev contents index glossary copyright chapter section subsection appendix help bookmark");
            xhtmlAttrDictionary.Add("rev", "alternate stylesheet start next prev contents index glossary copyright chapter section subsection appendix help bookmark");
            xhtmlAttrDictionary.Add("disabled", "disabled");
            xhtmlAttrDictionary.Add("readonly", "readonly");

            // Tag Specific
            xhtmlAttrDictionary.Add("img|ismap", "ismap");
            xhtmlAttrDictionary.Add("object|declare", "declare");
            xhtmlAttrDictionary.Add("form|method", "get post");
            xhtmlAttrDictionary.Add("input|checked", "checked");
            xhtmlAttrDictionary.Add("input|ismap", "ismap");
            xhtmlAttrDictionary.Add("input|type", "text password checkbox radio submit reset file hidden image button");
            xhtmlAttrDictionary.Add("select|multiple", "multiple");
            xhtmlAttrDictionary.Add("option|selected", "selected");
            xhtmlAttrDictionary.Add("button|type", "button submit reset");
            xhtmlAttrDictionary.Add("table|frame", "void above below hsides lhs rhs vsides box border");
            xhtmlAttrDictionary.Add("table|rules", "none groups rows cols all");
            xhtmlAttrDictionary.Add("script|type", "application/ecmascript application/javascript application/x-ecmascript application/x-javascript text/ecmascript text/javascript text/javascript1.0 text/javascript1.1 text/javascript1.2 text/javascript1.3 text/javascript1.4 text/javascript1.5 text/jscript text/livescript text/x-ecmascript text/x-javascript");
            xhtmlAttrDictionary.Add("script|defer", "defer");
            xhtmlAttrDictionary.Add("param|valuetype", "data ref object");

            #endregion

            #region HTML5: Tag->Attributes

            html5TagDictionary = new Dictionary<String, TagItem>();
            html5TagDictionary.Add("html", new TagItem("manifest", 0));
            html5TagDictionary.Add("head", new TagItem("", 0));
            html5TagDictionary.Add("title", new TagItem("", 0));
            html5TagDictionary.Add("base", new TagItem("href target", 1));
            html5TagDictionary.Add("link", new TagItem("href rel media hreflang type sizes", 1));
            html5TagDictionary.Add("meta", new TagItem("name http-equiv content charset", 1));
            html5TagDictionary.Add("style", new TagItem("media type scoped", 0));
            html5TagDictionary.Add("script", new TagItem("src async defer type charset", 0));
            html5TagDictionary.Add("noscript", new TagItem("", 0));
            html5TagDictionary.Add("body", new TagItem("onafterprint onbeforeprint onbeforeunload onhashchange onmessage onoffline ononline onpagehide onpageshow onpopstate onredo onresize onstorage onundo onunload", 0));
            html5TagDictionary.Add("section", new TagItem("", 0));
            html5TagDictionary.Add("nav", new TagItem("", 0));
            html5TagDictionary.Add("article", new TagItem("", 0));
            html5TagDictionary.Add("aside", new TagItem("", 0));
            html5TagDictionary.Add("h1", new TagItem("", 0));
            html5TagDictionary.Add("h2", new TagItem("", 0));
            html5TagDictionary.Add("h3", new TagItem("", 0));
            html5TagDictionary.Add("h4", new TagItem("", 0));
            html5TagDictionary.Add("h5", new TagItem("", 0));
            html5TagDictionary.Add("h6", new TagItem("", 0));
            html5TagDictionary.Add("hgroup", new TagItem("", 0));
            html5TagDictionary.Add("header", new TagItem("", 0));
            html5TagDictionary.Add("footer", new TagItem("", 0));
            html5TagDictionary.Add("address", new TagItem("", 0));
            html5TagDictionary.Add("p", new TagItem("", 0));
            html5TagDictionary.Add("hr", new TagItem("", 1));
            html5TagDictionary.Add("pre", new TagItem("", 0));
            html5TagDictionary.Add("blockquote", new TagItem("cite", 0));
            html5TagDictionary.Add("ol", new TagItem("reversed start type", 0));
            html5TagDictionary.Add("ul", new TagItem("", 0));
            html5TagDictionary.Add("li", new TagItem("value", 0));
            html5TagDictionary.Add("dl", new TagItem("", 0));
            html5TagDictionary.Add("dt", new TagItem("", 0));
            html5TagDictionary.Add("dd", new TagItem("", 0));
            html5TagDictionary.Add("figure", new TagItem("", 0));
            html5TagDictionary.Add("figcaption", new TagItem("", 0));
            html5TagDictionary.Add("div", new TagItem("", 0));
            html5TagDictionary.Add("a", new TagItem("href target rel media hreflang type", 0));
            html5TagDictionary.Add("em", new TagItem("", 0));
            html5TagDictionary.Add("strong", new TagItem("", 0));
            html5TagDictionary.Add("small", new TagItem("", 0));
            html5TagDictionary.Add("s", new TagItem("", 0));
            html5TagDictionary.Add("site", new TagItem("", 0));
            html5TagDictionary.Add("q", new TagItem("cite", 0));
            html5TagDictionary.Add("dfn", new TagItem("", 0));
            html5TagDictionary.Add("abbr", new TagItem("", 0));
            html5TagDictionary.Add("time", new TagItem("datetime pubdate", 0));
            html5TagDictionary.Add("code", new TagItem("", 0));
            html5TagDictionary.Add("var", new TagItem("", 0));
            html5TagDictionary.Add("samp", new TagItem("", 0));
            html5TagDictionary.Add("kbd", new TagItem("", 0));
            html5TagDictionary.Add("sub", new TagItem("", 0));
            html5TagDictionary.Add("sup", new TagItem("", 0));
            html5TagDictionary.Add("i", new TagItem("", 0));
            html5TagDictionary.Add("b", new TagItem("", 0));
            html5TagDictionary.Add("u", new TagItem("", 0));
            html5TagDictionary.Add("mark", new TagItem("", 0));
            html5TagDictionary.Add("ruby", new TagItem("", 0));
            html5TagDictionary.Add("rt", new TagItem("", 0));
            html5TagDictionary.Add("rp", new TagItem("", 0));
            html5TagDictionary.Add("bdi", new TagItem("dir", 0));
            html5TagDictionary.Add("bdo", new TagItem("dir", 0));
            html5TagDictionary.Add("span", new TagItem("", 0));
            html5TagDictionary.Add("br", new TagItem("", 1));
            html5TagDictionary.Add("wbr", new TagItem("", 1));
            html5TagDictionary.Add("ins", new TagItem("cite datetime", 0));
            html5TagDictionary.Add("del", new TagItem("cite datetime", 0));
            html5TagDictionary.Add("img", new TagItem("alt src crossorigin usemap ismap width height", 1));
            html5TagDictionary.Add("iframe", new TagItem("src srcdoc name sandbox seamless width height", 0));
            html5TagDictionary.Add("embed", new TagItem("src type width height", 1));
            html5TagDictionary.Add("object", new TagItem("data type name usemap form width height", 0));
            html5TagDictionary.Add("param", new TagItem("name value", 1));
            html5TagDictionary.Add("video", new TagItem("src crossorigin poster preload autoplay mediagroup loop muted controls width height", 0));
            html5TagDictionary.Add("audio", new TagItem("src crossorigin preload autoplay mediagroup loop muted controls", 0));
            html5TagDictionary.Add("source", new TagItem("src type media", 1));
            html5TagDictionary.Add("track", new TagItem("kind src srclang label default", 1));
            html5TagDictionary.Add("canvas", new TagItem("width height", 0));
            html5TagDictionary.Add("map", new TagItem("name", 0));
            html5TagDictionary.Add("area", new TagItem("alt coords shape href target rel media hreflang type", 1));
            html5TagDictionary.Add("table", new TagItem("border", 0));
            html5TagDictionary.Add("caption", new TagItem("", 0));
            html5TagDictionary.Add("colgroup", new TagItem("span", 0));
            html5TagDictionary.Add("col", new TagItem("span", 1));
            html5TagDictionary.Add("tbody", new TagItem("", 0));
            html5TagDictionary.Add("thead", new TagItem("", 0));
            html5TagDictionary.Add("tfoot", new TagItem("", 0));
            html5TagDictionary.Add("tr", new TagItem("", 0));
            html5TagDictionary.Add("td", new TagItem("colspan rowspan headers", 0));
            html5TagDictionary.Add("th", new TagItem("colspan rowspan headers scope", 0));
            html5TagDictionary.Add("form", new TagItem("accept-charset action autocomplete enctype method name novalidate target", 0));
            html5TagDictionary.Add("fieldset", new TagItem("disabled form name", 0));
            html5TagDictionary.Add("legend", new TagItem("", 0));
            html5TagDictionary.Add("label", new TagItem("form for", 0));
            html5TagDictionary.Add("input", new TagItem("accept alt autocomplete autofocus checked dirname disabled form formaction formenctype formmethod formnovalidate formtarget height list max maxlength min multiple name pattern placeholder readonly required size src step type value width", 1));
            html5TagDictionary.Add("button", new TagItem("autofocus disabled form formaction formenctype formmethod formnovalidate formtarget name type value", 0));
            html5TagDictionary.Add("select", new TagItem("autofocus disabled form multiple name required size", 0));
            html5TagDictionary.Add("datalist", new TagItem("", 0));
            html5TagDictionary.Add("optgroup", new TagItem("disabled label", 0));
            html5TagDictionary.Add("option", new TagItem("disabled label selected value", 0));
            html5TagDictionary.Add("textarea", new TagItem("autofocus cols dirname disabled form maxlength name placeholder readonly required rows wrap", 0));
            html5TagDictionary.Add("keygen", new TagItem("autofocus challenge disabled form keytype name", 1));
            html5TagDictionary.Add("output", new TagItem("for form name", 0));
            html5TagDictionary.Add("progress", new TagItem("value max form", 0));
            html5TagDictionary.Add("meter", new TagItem("value min max low high optimum form", 0));
            html5TagDictionary.Add("details", new TagItem("open", 0));
            html5TagDictionary.Add("summary", new TagItem("", 0));
            html5TagDictionary.Add("command", new TagItem("type label icon disabled checked radiogroup", 1));
            html5TagDictionary.Add("menu", new TagItem("type label", 0));

            #endregion

            #region HTML5: Attribute->Values

            html5AttrDictionary = new Dictionary<String, String>();

            // Generic
            html5AttrDictionary.Add("target", "_blank _self _parent _top");
            html5AttrDictionary.Add("rel", "alternate author bookmark external help icon license next nofollow noreferrer pingback prefetch prev search sidebar stylesheet tag"); // link a area
            html5AttrDictionary.Add("media", "all screen print handheld projection tty tv projection braille aural"); // link style a source area
            html5AttrDictionary.Add("autoplay", "autoplay");
            html5AttrDictionary.Add("loop", "loop");
            html5AttrDictionary.Add("muted", "muted");
            html5AttrDictionary.Add("controls", "controls");
            html5AttrDictionary.Add("preload", "none metadata auto");
            html5AttrDictionary.Add("crossorigin", "anonymous use-credentials");
            html5AttrDictionary.Add("disabled", "disabled");
            html5AttrDictionary.Add("autofocus", "autofocus");
            html5AttrDictionary.Add("multiple", "multiple");
            html5AttrDictionary.Add("readonly", "readonly");
            html5AttrDictionary.Add("required", "required");
            html5AttrDictionary.Add("hidden", "hidden");
            html5AttrDictionary.Add("contenteditable", "true false");
            html5AttrDictionary.Add("dir", "auto ltr rtl");
            html5AttrDictionary.Add("draggable", "true false");
            html5AttrDictionary.Add("dropzone", "copy move link");
            html5AttrDictionary.Add("spellcheck", "true false");

            // Tag Specific
            html5AttrDictionary.Add("meta|name", "application-name author description generator keywords");
            html5AttrDictionary.Add("meta|http-equiv", "content-type default-style refresh");
            html5AttrDictionary.Add("style|scoped", "scoped");
            html5AttrDictionary.Add("style|type", "text/css");
            html5AttrDictionary.Add("script|async", "async");
            html5AttrDictionary.Add("script|defer", "defer");
            html5AttrDictionary.Add("script|type", "application/ecmascript application/javascript application/x-ecmascript application/x-javascript text/ecmascript text/javascript text/javascript1.0 text/javascript1.1 text/javascript1.2 text/javascript1.3 text/javascript1.4 text/javascript1.5 text/jscript text/livescript text/x-ecmascript text/x-javascript");
            html5AttrDictionary.Add("ol|reversed", "reversed");
            html5AttrDictionary.Add("ol|type", "1 A a I i");
            html5AttrDictionary.Add("time|pubdate", "pubdate");
            html5AttrDictionary.Add("img|crossorigin", "anonymous use-credentials");
            html5AttrDictionary.Add("img|ismap", "ismap");
            html5AttrDictionary.Add("iframe|sandbox", "allow-same-origin allow-top-navigation allow-forms allow-scripts");
            html5AttrDictionary.Add("iframe|seamless", "seamless");
            html5AttrDictionary.Add("track|kind", "subtitles captions descriptions chapters metadata");
            html5AttrDictionary.Add("track|default", "default");
            html5AttrDictionary.Add("area|shape", "circle default poly rect");
            html5AttrDictionary.Add("table|border", "1");
            html5AttrDictionary.Add("form|autocomplete", "on off");
            html5AttrDictionary.Add("form|enctype", "application/x-www-form-urlencoded multipart/form-data text/plain");
            html5AttrDictionary.Add("form|method", "get post");
            html5AttrDictionary.Add("form|novalidate", "novalidate");
            html5AttrDictionary.Add("input|accept", "audio/* video/* image/*");
            html5AttrDictionary.Add("input|autocomplete", "on off");
            html5AttrDictionary.Add("input|checked", "checked");
            html5AttrDictionary.Add("input|dirname", "ltr rtl");
            html5AttrDictionary.Add("input|formenctype", "application/x-www-form-urlencoded multipart/form-data text/plain");
            html5AttrDictionary.Add("input|formmethod", "get post");
            html5AttrDictionary.Add("input|formnovalidate", "formnovalidate");
            html5AttrDictionary.Add("input|type", "hidden text search tel url email password datetime date month week time datetime-local number range color checkbox radio file submit image reset button");
            html5AttrDictionary.Add("button|formenctype", "application/x-www-form-urlencoded multipart/form-data text/plain");
            html5AttrDictionary.Add("button|formmethod", "get post");
            html5AttrDictionary.Add("button|formnovalidate", "formnovalidate");
            html5AttrDictionary.Add("button|type", "submit reset button");
            html5AttrDictionary.Add("option|selected", "selected");
            html5AttrDictionary.Add("textarea|dirname", "ltr rtl");
            html5AttrDictionary.Add("textarea|wrap", "soft hard");
            html5AttrDictionary.Add("keygen|keytype", "rsa");
            html5AttrDictionary.Add("details|open", "open");
            html5AttrDictionary.Add("command|type", "command checkbox radio");
            html5AttrDictionary.Add("command|checked", "checked");
            html5AttrDictionary.Add("menu|type", "context toolbar list");

            #endregion
        }

        #endregion
    }
}
