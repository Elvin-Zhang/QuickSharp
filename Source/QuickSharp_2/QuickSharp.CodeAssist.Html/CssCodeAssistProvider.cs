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
using QuickSharp.Core;
using QuickSharp.Editor;
using ScintillaNet;

namespace QuickSharp.CodeAssist.Html
{
    public class CssCodeAssistProvider : ICodeAssistProvider
    {
        #region ICodeAssistProvider

        public DocumentType DocumentType
        {
            get
            {
                return new DocumentType(Constants.DOCUMENT_TYPE_CSS);
            }
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
            return GetCssLookupList(document);
        }

        #endregion

        private Dictionary<string, List<string>> _cssProperties;

        private LookupList GetCssLookupList(ScintillaEditForm document)
        {
            Line line = document.Editor.Lines.Current;

            string text = line.Text.Substring(0,
                line.SelectionStartPosition -
                line.StartPosition).TrimStart();

            if (!IsInsideRule(document)) return null;

            /*
             * Load the property list and get the property/value
             * text required for the lookup.
             */

            _cssProperties = GetCssProperties();

            Char[] separators = { '{', '}', ';' };

            string[] split = text.Split(separators);

            string lookahead = split[split.Length - 1];

            if (lookahead.IndexOf(":") != -1)
            {
                /*
                 * Looking for a CSS property value name.
                 */

                string[] split2 = lookahead.Split(':');

                string property = split2[0].Trim();

                // Get the property values

                if (!_cssProperties.ContainsKey(property))
                    return null;

                List<LookupListItem> list =
                    GetPropertyValueList(_cssProperties[property]);

                // Get the last delimited segment of the lookahead

                lookahead = split2.Length > 1 ? split2[1] : String.Empty;

                string[] split3 = lookahead.Split(new Char[] { ' ', ',' });

                lookahead = split3[split3.Length - 1];

                return new LookupList(lookahead, list);
            }
            else
            {
                /*
                 * Looking for a CSS property name.
                 */

                List<LookupListItem> list = 
                    GetPropertyList(_cssProperties);

                string insertionTemplate = String.Format("{0}:",
                    QuickSharp.CodeAssist.Constants.
                        INSERTION_TEMPLATE_TEXT_PLACEHOLDER);

                return new LookupList(lookahead, list, insertionTemplate);
            }
        }

        #region Helpers

        private List<LookupListItem> GetPropertyList(
            Dictionary<string, List<string>> cssProperties)
        {
            List<LookupListItem> list = new List<LookupListItem>();

            foreach (string s in cssProperties.Keys)
            {
                LookupListItem item = new LookupListItem();
                item.DisplayText = s;
                item.InsertText = s;
                item.Category = QuickSharp.CodeAssist.Constants.PROPERTIES;

                list.Add(item);
            }

            return list;
        }

        private List<LookupListItem> GetPropertyValueList(
            List<string> cssPropertyValues)
        {
            List<LookupListItem> list = new List<LookupListItem>();

            foreach (string s in cssPropertyValues)
            {
                LookupListItem item = new LookupListItem();

                if (s.StartsWith("~"))
                {
                    item.DisplayText = s.Substring(1);
                    item.InsertText = String.Empty;
                    item.Category = QuickSharp.CodeAssist.Constants.FIELD;
                }
                else if (s.StartsWith("#"))
                {
                    item.DisplayText = s.Substring(1);
                    item.InsertText = String.Empty;
                    item.Category = QuickSharp.CodeAssist.Constants.EXCEPTION;
                }
                else
                {
                    item.DisplayText = s;
                    item.InsertText = s;
                    item.Category = QuickSharp.CodeAssist.Constants.CONSTANT;
                }


                list.Add(item);
            }

            return list;
        }

        private bool IsInsideRule(ScintillaEditForm document)
        {
            string content = document.GetContent() as string;
            content = content.Substring(0, document.Editor.CurrentPos);

            int open = content.LastIndexOf("{");
            int close = content.LastIndexOf("}");

            if (open == -1 && close == -1) return false;
            if (open != -1 && close == -1) return true;

            return (close < open);
        }

        #endregion

        #region CSS Properties

        private Dictionary<string, List<string>> GetCssProperties()
        {
            Dictionary<string, List<string>> dict =
                new Dictionary<string, List<string>>();

            foreach (string p in css3properties)
            {
                string[] split = p.Split('|');

                /*
                 * Property name is dictionary key.
                 */

                dict[split[0]] = null;

                /*
                 * Property values are the keys value.
                 */

                if (split.Length > 1)
                {
                    List<string> list = new List<string>();

                    for (int i = 1; i < split.Length; i++)
                        list.Add(split[i]);

                    dict[split[0]] = list;
                }
            }

            return dict;
        }

        #endregion

        #region CSS 2.1 Properties
        /*
        private string[] css2properties =
        {
            // CSS 2.1
            "azimuth|inherit|left-side|far-left|left|center-left|center|center-right|right|far-right|right-side|behind|leftwards|rightwards|~angle",
            "background-attachment|inherit|scroll|fixed",
            "background-color|inherit|transparent|~color",
            "background-image|inherit|none|~url",
            "background-position|inherit|top|right|bottom|left|center|~percentage|~length",
            "background-repeat|inherit|repeat|repeat-x|repeat-y|no-repeat",
            "background|#color image repeat attachment position|inherit",
            "border-collapse|inherit|collapse|separate",
            "border-color|inherit|transparent|~color",            
            "border-spacing|inherit|~length",
            "border-style|inherit|none|dotted|dashed|solid|double|groove|ridge|inset|outset|hidden",
            "border-top|#width style color|inherit",
            "border-left|#width style color|inherit",
            "border-right|#width style color|inherit",
            "border-bottom|#width style color|inherit",
            "border-top-color|inherit|transparent|~color",
            "border-right-color|inherit|transparent|~color",
            "border-left-color|inherit|transparent|~color",
            "border-bottom-color|inherit|transparent|~color",
            "border-top-style|inherit|none|dotted|dashed|solid|double|groove|ridge|inset|outset|hidden",
            "border-right-style|inherit|none|dotted|dashed|solid|double|groove|ridge|inset|outset|hidden",
            "border-left-style|inherit|none|dotted|dashed|solid|double|groove|ridge|inset|outset|hidden",
            "border-bottom-style|inherit|none|dotted|dashed|solid|double|groove|ridge|inset|outset|hidden",   
            "border-top-width|inherit|thin|medium|thick|~length",
            "border-right-width|inherit|thin|medium|thick|~length",
            "border-left-width|inherit|thin|medium|thick|~length",
            "border-bottom-width|inherit|thin|medium|thick|~length",
            "border-width|inherit|thin|medium|thick|~length",
            "border|#width style color|inherit",
            "bottom|inherit|auto|~percentage|~length",
            "caption-side|inherit|top|bottom",
            "clear|inherit|none|left|right|both",
            "clip|inherit|~shape|auto",
            "color|inherit|~color",
            "content|inherit|normal|open-quote|close-quote|no-open-quote|no-close-quote|attr()|counter()|counters()|~url|~string",
            "counter-increment|inherit|none|~name|~name number",
            "counter-reset|inherit|none|~name|~name number",
            "cue-after|inherit|none|~url",
            "cue-before|inherit|none|~url",
            "cursor|inherit|~url|auto|default|pointer|text|help|wait|progress|crosshair|move|e-resize|ne-resize|n-resize|nw-resize|w-resize|sw-resize|s-resize|se-resize",
            "direction|inherit|ltr|rtl",
            "display|inherit|none|inline|block|list-item|run-in|inline-block|inline-table|table-row-group|table-header-group|table-footer-group|table-row|table-column-group|table-column|table-cell|table-caption",
            "elevation|inherit|below|above|higher|lower|level|~angle",
            "empty-cells|inherit|show|hide",
            "float|inherit|left|right|none",
            "font|#style variant weight size/line-height family|inherit",
            "font-family|inherit|~font",
            "font-size|inherit|xx-small|x-small|small|medium|large|x-large|xx-large|smaller|larger|~percentage|~length",
            "font-style|inherit|normal|italic|oblique",
            "font-variant|inherit|normal|small-caps",
            "font-weight|inherit|normal|bold|bolder|lighter|100|200|300|400|500|600|800|800|900",
            "height|inherit|auto|~percentage|~length",
            "left|inherit|auto|~percentage|~length",
            "letter-spacing|inherit|normal|~length",
            "line-height|inherit|normal|~number|~percentage|~length",
            "list-style|#type position image|inherit",
            "list-style-image|inherit|~url|none",
            "list-style-position|inherit|inside|outside",
            "list-style-type|inherit|none|disc|circle|square|decimal|decimal-leading-zero|lower-roman|upper-roman|lower-alpha|upper-alpha|lower-latin|upper-latin|lower-greek|Armenian|georgian",
            "margin|inherit|~percentage|~length",
            "margin-left|inherit|~percentage|~length",
            "margin-right|inherit|~percentage|~length",
            "margin-top|inherit|~percentage|~length",
            "margin-bottom|inherit|~percentage|~length",
            "max-height|inherit|none|~percentage|~length",
            "max-width|inherit|none|~percentage|~length",
            "min-height|inherit|none|~percentage|~length",
            "min-width|inherit|none|~percentage|~length",
            "orphans|inherit|~number",
            "outline|#color style width|inherit",
            "outline-color|invert|~color",
            "outline-stlye|none|dotted|dashed|solid|double|groove|ridge|inset|outset|hidden",
            "outline-width|thin|medium|thick|~length",
            "overflow|inherit|visible|hidden|scroll|auto",
            "padding|inherit|~percentage|~length",
            "padding-top|inherit|~percentage|~length",
            "padding-left|inherit|~percentage|~length",
            "padding-right|inherit|~percentage|~length",
            "padding-bottom|inherit|~percentage|~length",
            "page-break-after|inherit|auto|always|avoid|left|right",
            "page-break-before|inherit|auto|always|avoid|left|right",
            "page-break-inside|inherit|auto|avoid",
            "pause-after|inherit|~time|~percentage",
            "pause-before|inherit|~time|~percentage",
            "pitch|inherit|x-low|low|medium|high|x-high|~frequency",
            "pitch-range|inherit|~number",
            "play-during|inherit|auto|mix|repeat|none|~url",
            "position|inherit|static|relative|absolute|fixed",
            "quotes|inherit|none",
            "richness|inherit|~number",
            "right|inherit|auto|~percentage|~length",
            "speak|inherit|normal|none|spell-out",
            "speak-header|inherit|once|allways",
            "speak-numeral|inherit|digits|continuous",
            "speak-punctuation|inherit|code|none",
            "speech-rate|inherit|x-slow|slow|medium|fast|x-fast|~number",
            "stress|inherit|~number",
            "table-layout|inherit|auto|fixed",
            "text-align|inherit|left|right|center|justify",
            "text-decoration|inherit|none|underline|overline|line-through|blink",
            "text-indent|inherit|~percentage|~length",
            "text-transform|inherit|none|capitalize|uppercase|lowercase",
            "top|inherit|auto|~percentage|~length",
            "unicode-bidi|inherit|normal|embed|bidi-override",
            "vertical-align|inherit|baseline|sub|super|top|text-top|middle|bottom|text-bottom|~percentage|~length",
            "visibility|inherit|visible|hidden|collapse",
            "white-space|inherit|normal|pre|nowrap|pre-wrap|pre-line",
            "widows|inherit|~number",
            "width|inherit|auto|~percentage|~length",
            "word-spacing|inherit|normal|~length",
            "z-index|inherit|auto|~number"
        };
         */
        #endregion

        #region CSS 3 Properties

        // Info from OReilly's CSS Pocket Reference 4th Edition.

        private string[] css3properties =
        {
            "animation|#name duration timing-function delay iteration-count direction",
            "animation-delay|~time",
            "animation-direction|normal|alternate",
            "animation-iteration-count|infinite|~number", 
            "animation-name|none",
            "animation-play-state|running|paused",
            "animation-timing-function|ease|linear|ease-in|ease-out|ease-in-out|~cubic-bezier",
            "backface-visibility|visible|hidden",
            "background|#color image position repeat attachment",
            "background-attachment|scroll|fixed|local",
            "background-clip|border-box|padding-box|content-box",
            "background-color|transparent|~color",
            "background-image|none|~url",
            "background-origin|border-box|padding-box|content-box",
            "background-position|top|right|bottom|left|center|~percentage|~length",
            "background-repeat|repeat|repeat-x|repeat-y|no-repeat|space|round",
            "background-size|~percentage|~length|auto|cover|contain",
            "border|#width style color",
            "border-bottom|#width style color",
            "border-bottom-color|transparent|~color",
            "border-bottom-left-radius|~percentage|~length",
            "border-bottom-right-radius|~percentage|~length",
            "border-bottom-style|none|dotted|dashed|solid|double|groove|ridge|inset|outset|hidden",   
            "border-bottom-width|thin|medium|thick|~length",
            "border-collapse|collapse|separate",
            "border-color|transparent|~color", 
            "border-image|#source slice / width / outset repeat",
            "border-image-outset|~length|~number",
            "border-image-repeat|stretch|repeat|round",
            "border-image-slice|~number|~percentage",
            "border-image-source|none|~url",
            "border-image-width|~length|~percentage|~number|auto",
            "border-left|#width style color",
            "border-left-color|transparent|~color",
            "border-left-style|none|dotted|dashed|solid|double|groove|ridge|inset|outset|hidden",
            "border-left-width|thin|medium|thick|~length",
            "border-radius|~length|~percentage",
            "border-right|#width style color",
            "border-right-color|transparent|~color",
            "border-right-style|none|dotted|dashed|solid|double|groove|ridge|inset|outset|hidden",
            "border-right-width|thin|medium|thick|~length",
            "border-spacing|~length",
            "border-style|none|dotted|dashed|solid|double|groove|ridge|inset|outset|hidden",
            "border-top|#width style color",
            "border-top-color|transparent|~color", 
            "border-top-left-radius|~percentage|~length",
            "border-top-right-radius|~percentage|~length",
            "border-top-style|none|dotted|dashed|solid|double|groove|ridge|inset|outset|hidden",
            "border-top-width|thin|medium|thick|~length",
            "border-width|thin|medium|thick|~length",
            "bottom|auto|~percentage|~length",
            "box-align|stretch|start|end|center|baseline",
            "box-decoration-break|slice|clone",
            "box-direction|normal|reverse",
            "box-flex|~number",
            "box-lines|single|multiple",
            "box-ordinal-group|~number",
            "box-orient|horizontal|vertical|inline-axis|block-axis",
            "box-pack|start|end|center|justify",
            "box-shadow|none|inset|~length",
            "box-sizing|content-box|border-box",
            "caption-side|top|bottom",
            "clear|none|left|right|both",
            "clip|~shape|auto",
            "color|~color",
            "column-count|auto|~number",
            "column-fill|auto|balance",
            "column-gap|normal|~length",
            "column-rule|#width style color",
            "column-rule-color:~color",
            "column-rule-style:none|dotted|dashed|solid|double|groove|ridge|inset|outset|hidden",
            "column-rule-width|thin|medium|thick|~length",
            "column-span|none|all",
            "column-width|auto|~length",
            "columns|#width count",
            "content|normal|open-quote|close-quote|no-open-quote|no-close-quote|~attr|~counter|~url|~string",
            "counter-increment|none|~name|~name number",
            "counter-reset|none|~name|~name number",
            "cursor|~url|auto|default|pointer|text|help|wait|progress|crosshair|move|e-resize|ne-resize|n-resize|nw-resize|w-resize|sw-resize|s-resize|se-resize",
            "direction|ltr|rtl",
            "display|none|inline|block|inline-block|list-item|run-in|compact|table|inline-table|table-row-group|table-header-group|table-footer-group|" +
                "table-row|table-column-group|table-column|table-cell|table-caption|ruby|ruby-base|ruby-text|ruby-base-container|ruby-text-container",
            "elevation|below|above|higher|lower|level|~angle",
            "empty-cells|show|hide",
            "float|left|right|none",
            "font|#style variant weight size / line-height font-family|caption|icon|menu|message-box|small-caption|status-bar",
            "font-family|~font|serif|sans-serif|monospace|cursive|fantasy",
            "font-size|xx-small|x-small|small|medium|large|x-large|xx-large|smaller|larger|~percentage|~length",
            "font-size-adjust|~number|none",
            "font-style|normal|italic|oblique",
            "font-variant|normal|small-caps",
            "font-weight|normal|bold|bolder|lighter|100|200|300|400|500|600|800|800|900",
            "height|auto|~percentage|~length",
            "left|auto|~percentage|~length",
            "letter-spacing|normal|~length",
            "line-height|normal|~number|~percentage|~length",
            "list-style|#type position image",
            "list-style-image|~url|none",
            "list-style-position|inside|outside",
            "list-style-type|normal|none|box|check|circle|diamond|disc|hyphen|square|armenian|cjk-ideographic|ethiopic-numeric|georgian|hebrew|" +
                "japanese-formal|japanese-informal|lower-armenian|lower-roman|simp-chinese-formal|simp-chinese-informal|syriac|tamil|trad-chinese-formal|" +
                "trad-chinese-informal|upper-armenian|upper-roman|arabic-indic|binary|bengali|cambodian|decimal|decimal-leading-zero|devanagari|gujarati|" +
                "gurmukhi|kannada|khmer|lao|lower-hexadecimal|malayalam|mongolian|myanmar|octal|oriya|persian|telugu|tibetan|thai|upper-hexadecimal|urdu|" +
                "afar|amharic|amharic-abegede|cjk-earthly-branch|cjk-heavenly-stem|ethiopic|ethiopic-abegede|ethiopic-abegede-am-et|ethiopic-abegede-gez|" +
                "ethiopic-abegede-ti-er|ethiopic-abegede-ti-et|ethiopic-halehame-aa-er|ethiopic-halehame-aa-et|ethiopic-halehame-am-et|ethiopic-halehame-gez|" +
                "ethiopic-halehame-om-et|ethiopic-halehame-sid-et|ethiopic-halehame-so-et|ethiopic-halehame-ti-er|ethiopic-halehame-ti-et|" +
                "ethiopic-halehame-tig|hangul|hangul-consonant|hiragana|hiragana-iroha|katakana|katakana-iroha|lower-alpha|lower-greek|lower-norwegian|" +
                "lower-latin|oromo|sidama|somali|tigre|tigrinya-er|tigrinya-er-abegede|tigrinya-et|tigrinya-et-abegede|upper-alpha|upper-greek|" +
                "upper-norwegian|upper-latin|asterisks|footnotes|circled-decimal|circled-lower-latin|circled-upper-latin|dotted-decimal|" +
                "double-circled-decimal|filled-circled-decimal|parenthesised-decimal|parenthesised-lower-latin",
            "margin|auto|~percentage|~length",
            "margin-bottom|auto|~percentage|~length",
            "margin-left|auto|~percentage|~length",
            "margin-right|auto|~percentage|~length",
            "margin-top|auto|~percentage|~length",
            "max-height|none|~percentage|~length",
            "max-width|none|~percentage|~length",
            "min-height|none|~percentage|~length",
            "min-width|none|~percentage|~length",
            "opacity|~number",
            "outline|#color style width",
            "outline-color|invert|~color",
            "outline-stlye|none|dotted|dashed|solid|double|groove|ridge|inset|outset|hidden",
            "outline-width|thin|medium|thick|~length",
            "overflow|visible|hidden|scroll|auto|no-display|no-content",
            "overflow-x|visible|hidden|scroll|auto|no-display|no-content",
            "overflow-y|visible|hidden|scroll|auto|no-display|no-content",
            "padding|~percentage|~length",
            "padding-bottom|~percentage|~length",
            "padding-left|~percentage|~length",
            "padding-right|~percentage|~length",
            "padding-top|~percentage|~length",
            "perspective|none|~number",
            "perspective-origin|center|left|right|top|bottom|~length|~percentage",
            "position|static|relative|absolute|fixed",
            "quotes|none",
            "resize|none|both|horizontal|vertical",
            "right|~length|~percentage|auto",
            "ruby-align|auto|start|left|center|end|right|distribute-letter|distribute-space|line-edge",
            "ruby-overhang|auto|start|end|none",
            "ruby-position|before|after|right",
            "ruby-span|~attr|none",
            "table-layout|auto|fixed",
            "text-align|start|end|left|center|right|justify|match-parent",
            "text-decoration|none|underline|overline|line-through|blink",
            "text-indent|~percentage|~length|hanging|each-line",
            "text-overflow|clip|ellipsis",
            "text-shadow|none|~length|~color",
            "text-transform|none|capitalize|uppercase|lowercase",
            "top|auto|~percentage|~length",
            "transform|none|~transform-function",
            "transform-origin|center|left|right|top|bottom|~length|~percentage",
            "transform-style|flat|preserve-3d",
            "transition|#property duration timing-function delay",
            "transition-delay|~time",
            "transition-duration|~time",
            "transition-property|none|all",
            "transition-timing-function|ease|linear|ease-in|ease-out|ease-in-out|~cubic-bezier",
            "unicode-bidi|normal|embed|bidi-override",
            "vertical-align|baseline|sub|super|top|text-top|middle|bottom|text-bottom|~percentage|~length",
            "visibility|visible|hidden|collapse",
            "white-space|normal|pre|nowrap|pre-wrap|pre-line",
            "width|auto|~percentage|~length",
            "word-spacing|normal|~length|~percentage",
            "word-wrap|normal|break-word",
            "z-index|auto|~number",
            "break-after|auto|always|avoid|left|right|page|column|avoid-page|avoid-column",
            "break-before|auto|always|avoid|left|right|page|column|avoid-page|avoid-column",
            "break-inside|auto|avoid|avoid-page|avoid-column",
            "image-orientation|auto|~angle",
            "marks|crop|cross|none",
            "orphans|~number",
            "page|auto",
            "page-break-after|auto|always|avoid|left|right",
            "page-break-before|auto|always|avoid|left|right",
            "page-break-inside|auto|avoid",
            "page-policy|start|first|last",
            "size|auto|~length|A5|A4|A3|B5|B4|letter|legal|ledger|portrait|landscape",
            "widows|~number",
            "cue|#before after",
            "cue-after|none|~uri|~number|~percentage|silent|x-soft|soft|medium|loud|x-loud",
            "cue-before|none|~uri|~number|~percentage|silent|x-soft|soft|medium|loud|x-loud",
            "pause|#before after",
            "pause-after|~time|none|x-weak|weak|medium|strong|x-strong",
            "pause-before|~time|none|x-weak|weak|medium|strong|x-strong",
            "phonemes|~string",
            "rest|#before after",
            "rest-after|none|x-weak|weak|medium|strong|x-strong|~time",
            "rest-before|none|x-weak|weak|medium|strong|x-strong|~time",
            "speak|normal|spell-out|digits|literal-punctuation|no-punctuation",
            "speakability|auto|none|normal",
            "voice-balance|~number|left|center|right|leftwards|rightwards",
            "voice-duration|~time",
            "voice-family|child|young|old|male|female|neutral|~number",
            "voice-pitch|~number|~percentage|x-low|low|medium|high|x-high",
            "voice-pitch-range|~number|~percentage|x-low|low|medium|high|x-high",
            "voice-rate|~percentage|x-slow|slow|medium|fast|x-fast",
            "voice-stress|strong|moderate|reduced|none",
            "voice-volume|~number|~percentage|silent|x-soft|soft|medium|loud|x-loud"
        };

        #endregion
    }
}
