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
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using QuickSharp.Core;
using QuickSharp.Editor;
using QuickSharp.CodeAssist.DotNet;
using QuickSharp.CodeAssist.CSharp;

using ScintillaNet;

namespace QuickSharp.CodeAssist.AspNet
{
    public class CSharpWebCodeAssistProvider : CSharpCodeAssistProviderBase
    {
        private CSharpXhtmlCodeAssistProvider xhtmlProvider;
        private List<TagNamespace> _webConfigTagNamespaces;
        private List<String> _webConfigConnectionStrings;
        private List<String> _webConfigAppSettings;
        private Dictionary<String, String> _pageDirectives;

        public CSharpWebCodeAssistProvider()
        {
            xhtmlProvider = new CSharpXhtmlCodeAssistProvider();

            _webConfigTagNamespaces = new List<TagNamespace>();
            _webConfigConnectionStrings = new List<String>();
            _webConfigAppSettings = new List<String>();

            #region Directive data

            _pageDirectives = new Dictionary<String, String>();
            _pageDirectives["Page"] = "Async AsyncTimeOut AspCompat AutoEventWireup Buffer ClassName ClientTarget CodeBehind CodeFile CodeFileBaseClass CodePage CompilationMode CompilerOptions ContentType Culture Debug Description EnableEventValidation EnableSessionState EnableTheming EnableViewState EnableViewStateMac ErrorPage Explicit Inherits Language LCID LinePragmas MaintainScrollPositionOnPostback MasterPageFile ResponseEncoding SmartNavigation Src Strict StyleSheetTheme TargetSchema Theme Title Trace TraceMode Transaction UICulture ValidateRequest ViewStateEncryptionMode WarningLevel";
            _pageDirectives["Control"] = "AutoEventWireup ClassName CodeBehind CodeFile CodeFileBaseClass CompilationMode CompilerOptions Debug Description EnableTheming EnableViewState Explicit Inherits Language LinePragmas Src Strict TargetSchema WarningLevel";
            _pageDirectives["Import"] = "Namespace";
            _pageDirectives["Implements"] = "Interface";
            _pageDirectives["Register"] = "Assembly Namespace Src Tagname Tagprefix";
            _pageDirectives["Assembly"] = "Name Src";
            _pageDirectives["Master"] = "AutoEventWireup ClassName CodeFile CompilationMode CompilerOptions Debug Description EnableTheming EnableViewState Explicit Inherits Language LinePragmas MasterPageFile Src Strict WarningLevel";
            _pageDirectives["PreviousPageType"] = "TypeName VirtualPath";
            _pageDirectives["MasterType"] = "TypeName VirtualPath";
            _pageDirectives["OutputCache"] = "Duration Location Shared VaryByControl VaryByCustom VaryByHeader VaryByParam CacheProfile NoStore SqlDependency";
            _pageDirectives["Reference"] = "Page Control VirtualPath";
            _pageDirectives["WebHandler"] = "CompilerOptions Debug Description Language WarningLevel";
            _pageDirectives["WebService"] = "Class CodeBehind Debug Language";

            #endregion
        }

        /*
         * Note: this was moved to the ASP.NET provider to remove the
         * Xhtml module dependency in the C# provider. However, some
         * of the web-specific supporting code remains in the C# module.
         */

        #region Provider interface

        public virtual void DocumentActivated(ScintillaEditForm document)
        {
            Colorize(document);
        }

        public virtual LookupList GetLookupList(ScintillaEditForm document)
        {
            /*
             * Read controls, assemblies and namespaces from the web.config.
             */

            ReadWebConfig();

            /*
             * Refresh the master namespace lists.
             */

            UpdateLists();

            /*
             * Colorize the ASP.NET tags. This just looks for
             * strings that look like tags, there is no attempt
             * to recognize them as valid types so no Code Assist
             * functionality is required.
             */

            Colorize(document);

            /*
             * Prepare the search content and target for
             * the code assist lookup. Get the full source
             * and the source up to the caret position.
             */

            string text = document.GetContent() as string;

            /*
             * Save all the content. Unlike the C# code
             * provider we are also interested in content after
             * the caret. (This is because of a shortcut in the
             * code provider where we treat the current scope as
             * only existing before the caret. We can mostly get
             * away with it in normal code but not here.)
             */

            _fullSource = text;

            /*
             * Now we have the source see if there are any extra
             * assemblies declared in the page.
             */

            AddDeclaredAssemblies(text);

            /*
             * Call the appropriate lookup provider.
             */

            int currentPos = document.Editor.CurrentPos;

            /*
             * Because scriptlets and directives have similar syntax,
             * directive check must come before the scriptlet test.
             */

            if (IsCodeSection(text, currentPos))            // <script runat="server">
            {
                return GetCodeSectionLookupList(document, text);
            }
            else if (IsDirectiveSection(text, currentPos))  // <%@
            {
                return GetDirectiveSectionLookupList(text, currentPos);
            }
            else if (IsScriptletSection(text, currentPos))  // <% or <%=
            {
                return GetCodeSectionLookupList(document, text);
            }
            else // In a tag
            {
                text = text.Substring(0, currentPos);

                /*
                 * Look for a dollar expression (e.g. "<%$ AppSettings:MyVal " )
                 */

                Regex re = new Regex(@"<%\$\s*(\w*)(:)?(\w*)(\s*)$");
                Match match = re.Match(text);

                if (match.Success)
                    return GetDollarExpressionLookupList(match);

                /*
                 * Look for tags or tag attributes.
                 */

                AspNetTag tag = GetAspNetTag(text);

                if (tag != null)
                {
                    if (tag.WantAttributes)
                        return GetTagAttributesLookupList(tag);
                    else
                        return GetTagLookupList(tag);
                }
                else
                {
                    return xhtmlProvider.GetXhtmlLookupList(document);
                }
            }
        }

        #endregion

        #region Overrides

        protected override DeclaredVariables GetDeclaredVariables(
            string source, bool visibleScopesOnly,
            DeclarationContext declarationContext)
        {
            DeclaredVariables variables = new DeclaredVariables(
                source, fullNamespaceList, visibleScopesOnly,
                declarationContext);

            List<Variable> controls = GetServerControls();

            variables.Items.AddRange(controls);

            return variables;
        }

        protected override List<LookupListItem> GetChildNamespaces(
            LookupTarget target, string text)
        {
            return null;
        }

        protected override InheritedVariablesBase GetInheritedVariables(
            string source,
            DeclarationContext declarationContext,
            List<string> workspaceAssemblies,
            List<string> fullNamespaces,
            List<string> rootNamespaces)
        {
            return new InheritedVariablesWeb(
                _fullSource,
                declarationContext,
                workspaceAssemblies,
                fullNamespaces,
                rootNamespaces,
                configNamespaceList);
        }

        protected override List<string> GetBaseTypes(string source)
        {
            return GetWebBaseTypeList(_fullSource);
        }

        protected override List<String> GetNamespaceList(string source)
        {
            /*
             * Get namespaces declared in the master web.config
             * and added in the page using the 'Import' directive.
             */

            List<String> namespaces = 
                GetWebDeclaredNamespaceList(_fullSource);

            /*
             * Add extra namespaces declared in the application
             * web.config.
             */

            foreach (string ns in configNamespaceList)
                if (!namespaces.Contains(ns))
                    namespaces.Add(ns);
            
            return namespaces;
        }

        #endregion

        #region Directive section

        private bool IsDirectiveSection(string text, int pos)
        {
            text = text.Substring(0, pos);

            // Can't be in section if no start tag
            int s1 = text.LastIndexOf("<%@");
            if (s1 == -1) return false;

            // If have start but no end must be in section.
            int s2 = text.LastIndexOf("%>");
            if (s2 == -1) return true;

            // If have both, depends on order
            return (s1 > s2);
        }

        private LookupList GetDirectiveSectionLookupList(
            string text, int pos)
        {
            text = text.Substring(0, pos);

            // Save for later
            string lastToken = GetLastToken(text);

            int i = text.LastIndexOf("<%@");
            if (i == -1) return null;

            text = text.Substring(i);

            Regex re = new Regex(@"<%@\s+(\w*)(\s*)");
            Match match = re.Match(text);

            if (!match.Success) return null;

            // Get directive
            string name = match.Groups[1].Value;
            
            List<LookupListItem> items = new List<LookupListItem>();

            if (match.Groups[2].Value == String.Empty)
            {
                // Get Directive list
                foreach (string key in _pageDirectives.Keys)
                {
                    LookupListItem item = new LookupListItem();
                    item.DisplayText = item.InsertText = key;
                    item.Category = QuickSharp.CodeAssist.Constants.VALUETYPE;
                    items.Add(item);
                }

                string insertionTemplate = String.Format("{0} ",
                    QuickSharp.CodeAssist.Constants.
                        INSERTION_TEMPLATE_TEXT_PLACEHOLDER);
                                
                return new LookupList(name, items, insertionTemplate);
            }
            else
            {
                // Get attribute list
                if (!_pageDirectives.ContainsKey(name)) return null;

                string[] split = _pageDirectives[name].Split();

                foreach (string attribute in split)
                {
                    LookupListItem item = new LookupListItem();
                    item.DisplayText = item.InsertText = attribute;
                    item.Category = QuickSharp.CodeAssist.Constants.PROPERTIES;
                    items.Add(item);
                }

                string insertionTemplate = String.Format("{0}=\"{1}\"",
                    QuickSharp.CodeAssist.Constants.
                        INSERTION_TEMPLATE_TEXT_PLACEHOLDER,
                    QuickSharp.CodeAssist.Constants.
                        INSERTION_TEMPLATE_CPOS_PLACEHOLDER);

                return new LookupList(lastToken, items, insertionTemplate);
            }
        }

        #endregion
          
        #region Inline code sections

        private bool IsCodeSection(string text, int pos)
        {
            bool isScript = IsScriptSection(text, pos);
            if (!isScript) return false;

            /*
             * get the current script section and see if
             * it is server-side code?
             */

            string content = text;

            int i = content.IndexOf("</script>", pos);
            if (i != -1) content = content.Substring(0, i);

            i = content.LastIndexOf("<script ");
            if (i != -1) content = content.Substring(i);

            Regex re = new Regex("<script\\s+runat\\s*=\\s*\\\"server\\\"");
            return re.Match(content).Success;
        }

        private bool IsScriptletSection(string text, int pos)
        {
            // Must have a closing tag.
            int i = text.IndexOf("%>", pos);
            if (i == -1) return false;

            // Ignore eveything after closing tag.
            text = text.Substring(0, i);

            // Find nearest opening tag.
            int j = text.LastIndexOf("<%");

            // If after the cursor position, not in a scriptlet section.
            if (j > pos - 2) return false;

            // In a scriptlet section but could be a dollar expression.
            if ((text.Length > j + 2) && (text[j + 2] == '$'))
                return false;
 
            return true;
        }

        private bool IsScriptSection(string text, int pos)
        {
            string content = text.Substring(pos).ToLower();

            // Must have a closing tag.
            int s1 = content.IndexOf("</script>");
            if (s1 == -1) return false;

            // See if we have an opening tag.
            int s2 = content.IndexOf("<script ");

            // We are already before a closing tag; if no
            // opening tag visible we are in a code section.
            if (s2 == -1) return true;

            // Opening tag must be after closing tag if we're
            // in a code section.
            return s2 > s1;
        }

        private string GetInlineCode(string text)
        {
            /*
             * Find the start and end tags.
             */

            Regex re1 = new Regex(@"(?i)<script\s+");
            Regex re2 = new Regex(@"(?i)</script>");
            Regex re3 = new Regex("runat\\s*=\\s*\\\"server\\\"");

            MatchCollection startTags = re1.Matches(text);
            MatchCollection endTags = re2.Matches(text);

            /*
             * To make life easier we'll assume the start and end
             * tags match - i.e. no code assist until the tags are
             * all in place.
             */

            if (startTags.Count != endTags.Count)
                return String.Empty;

            StringBuilder sb = new StringBuilder();

            /*
             * Walk through the matches to pick out the content
             * between the tags.
             */

            for (int i = 0; i < startTags.Count; i++)
            {
                /*
                 * The script section may not be a server block so we
                 * need to check this.
                 */

                int start = startTags[i].Index;
                int end = endTags[i].Index;

                // Skip if tags not balanced properly.
                if (end - start < 0) continue;

                string content = text.Substring(start, end - start);

                int j = content.IndexOf(">");

                // Skip if opening tag doesn't end!
                if (j == -1) continue;  

                string tag = content.Substring(0, j);

                // Skip if not a server-side block.
                if (!re3.Match(tag).Success) continue;

                // Get the script content
                content = content.Substring(j);     // Still have '>'
                content = content.TrimStart('>');   // Safe removal

                sb.Append(content);
            }

            return sb.ToString();
        }

        private string GetScriptletCode(string text)
        {
            /*
             * Split the code into "<% .. %>" sections.
             */

            string[] split = text.Split(
                new String[] {"<%"}, StringSplitOptions.None);

            StringBuilder sb = new StringBuilder();

            foreach (string s in split)
            {
                if (s.Length < 1) continue;

                /*
                 * Don't allow lookup immediately after tag '%', need
                 * '=' or some white space character.
                 */

                if (s[0] == '=' || Char.IsWhiteSpace(s[0]))
                {
                    int i = s.IndexOf("%>");
                    if (i != -1)
                        sb.Append(s.Substring(0, i));
                    else
                        sb.Append(s);
                }
            }

            /*
             * Create a dummy method to contain the code. Use an illegal name
             * to allow the code assist to weed it out of the list of
             * genuine methods.
             */

            string method = "private void 000() {\r\n" + sb.ToString() + "\r\n}\r\n";

            return method;
        }

        private LookupList GetCodeSectionLookupList(
            ScintillaEditForm document, string fullSource)
        {
            /*
             * Get the token at the cursor position for the lookup target.
             */

            Line line = document.Editor.Lines.Current;

            string text = line.Text.Substring(0,
                line.SelectionStartPosition -
                line.StartPosition);

            int lineStartPos = GetLineStartPosition(text);

            text = text.TrimStart();

            /*
             * Get the code content from the page. This will be all the
             * code between script tags and all the code in scriptlet
             * tags (amalgamated into a single dummy method).
             */

            // Mark the current location.
            fullSource = fullSource.Insert(
                document.Editor.CurrentPos, "!##!");

            // Get the code content from the page.
            string scriptSource = GetInlineCode(fullSource);
            string scriptletSource = GetScriptletCode(fullSource);

            fullSource = scriptSource + "\r\n" + scriptletSource;

            string preSource = fullSource;

            int currentPos = preSource.IndexOf("!##!");
            if (currentPos == -1) return null;

            preSource = preSource.Substring(0, currentPos);
            fullSource = fullSource.Replace("!##!", String.Empty);

            LookupContext context = new LookupContext(
                fullSource, preSource, text,
                lineStartPos, currentPos, true);

            return GetCSharpLookupList(context);
        }

        #endregion

        #region Server controls

        /*
         * Not sure where this note should go so it goes here.
         * The detection of user controls in the bin directory is
         * based on the workspace assembly lookup used for all C# code
         * and will detect custom controls in any assembly in bin
         * regardless of whether they are made visible in the page
         * using a register directive. They will only show up in
         * lookups if the tag used is associated with the namespace
         * in which the found controls are located. This isn't much of
         * an issue but it may be confusing if a tag is used to
         * declare a namespace/assembly and other controls exist in
         * the same namespace but different assemblies. In such cases
         * the other controls will appear in the lookups even if they
         * haven't been explicity registered.
         */

        private string _fullSource = null;

        private List<Variable> GetServerControls()
        {
            /*
             * Rules:
             * 1. The opening tag and ID attribute must be on the same line.
             * 2. Controls must have the correct case for the class to
             *    be recognized (e.g. 'asp:Label' not 'asp:label' ).
             */

            /*
             * Get all the available control types and tag/namespaces
             */

            List<Type> controlTypes = GetAllControlTypes();
            List<TagNamespace> tagNamespaces = GetTagNamespaces();

            /*
             * Find all the declared controls.
             */

            Regex re = new Regex("(?i)<(\\w+):(\\w+)\\s+.*id\\s*=\\s*\\\"([\\w]+)\\\"\\s+");

            List<Variable> controls = new List<Variable>();

            foreach (Match m in re.Matches(_fullSource))
            {
                string tag = m.Groups[1].Value;
                string typeName = m.Groups[2].Value;
                string name = m.Groups[3].Value;

                /*
                 * Need to get the full typename for the control.
                 */

                foreach (Type ct in controlTypes)
                {
                    if (typeName == ct.Name &&
                        MatchTagNamespace(tag, ct.Namespace, tagNamespaces))
                    {
                        Variable v = new Variable(
                            name, m.Index, ct.Namespace + "." + typeName);

                        controls.Add(v);
                        break;
                    }
                }
            }

            return controls;
        }

        private bool MatchTagNamespace(
            string tag, string ns, List<TagNamespace> tagNamespaces)
        {
            foreach (TagNamespace tn in tagNamespaces)
                if (tn.TagPrefix == tag && tn.Namespace == ns)
                    return true;

            return false;
        }

        #endregion

        #region ASP.NET tags

        private class AspNetTag
        {
            public string Context { get; set; }
            public string TagPrefix { get; set; }
            public string LookAhead { get; set; }
            public bool ClosingTag { get; set; }
            public bool WantAttributes { get; set; }

            public AspNetTag(string text)
            {
                Context = text;
            }
        }

        private AspNetTag GetAspNetTag(string text)
        {
            text = CSharpFormattingTools.RemoveUnwantedText(text);

            string[] split1 = text.Split('<');
            string[] split2 = split1[split1.Length - 1].Split('>');
            string tag = split2[0];

            AspNetTag aspNetTag = new AspNetTag(text);

            if (split2.Length > 1) return null;

            /*
             * Check tag is ASP.NET and get the type.
             */

            Regex re = new Regex(@"^/?(\w+):(\w*)(\s*)");
            Match m = re.Match(tag);

            if (m.Success)
            {
                aspNetTag.TagPrefix = m.Groups[1].Value;
                aspNetTag.LookAhead = m.Groups[2].Value;
                aspNetTag.ClosingTag = tag.StartsWith("/");
                aspNetTag.WantAttributes = 
                    m.Groups[3].Value != String.Empty;

                return aspNetTag;
            }

            return null;
        }

        private LookupList GetTagLookupList(AspNetTag tag)
        {
            List<Type> controlTypes = GetTagControlTypes(tag);

            List<LookupListItem> lookupItems =
                new List<LookupListItem>();

            foreach (Type type in controlTypes)
            {
                LookupListItem item = GetItem(type);
                item.Category = QuickSharp.CodeAssist.Constants.WEBCONTROL;
                item.MenuItems.Clear();
                lookupItems.Add(item);
            }

            string insertionTemplate = 
                String.Format("{0}>",
                    QuickSharp.CodeAssist.Constants.
                        INSERTION_TEMPLATE_TEXT_PLACEHOLDER);

            if (!tag.ClosingTag)
            {
                insertionTemplate = String.Format(
                    "{0} ID=\"{1}\" runat=\"server\"></{2}:{0}>",
                    QuickSharp.CodeAssist.Constants.
                        INSERTION_TEMPLATE_TEXT_PLACEHOLDER,
                    QuickSharp.CodeAssist.Constants.
                        INSERTION_TEMPLATE_CPOS_PLACEHOLDER,
                    tag.TagPrefix);
            }

            return new LookupList(
                tag.LookAhead, lookupItems, insertionTemplate);
        }

        private LookupList GetTagAttributesLookupList(AspNetTag tag)
        {
            string controlName = tag.LookAhead.ToLower();
            Type controlType = null;

            foreach (Type t in GetTagControlTypes(tag))
            {
                if (controlName == t.Name.ToLower())
                {
                    controlType = t;
                    break;
                }
            }

            if (controlType == null) return null;

            /*
             * Get the public properties.
             */

            List<LookupListItem> items = new List<LookupListItem>();

            BindingFlags flags =
                BindingFlags.Public |
                BindingFlags.FlattenHierarchy |
                BindingFlags.Instance;

            List<String> foundNames = new List<String>();

            foreach (MethodInfo mi in controlType.GetMethods(flags))
            {
                if (!mi.Name.StartsWith("set_") &&
                    !mi.Name.StartsWith("add_")) continue;

                string name = mi.Name.Substring(4);

                if (mi.Name.StartsWith("add_"))
                    name = "On" + name;

                if (foundNames.Contains(name)) continue;
                foundNames.Add(name);

                LookupListItem li = new LookupListItem();
                li.DisplayText = name;
                li.InsertText = name;
                li.Category = QuickSharp.CodeAssist.Constants.PROPERTIES;

                items.Add(li);
            }

            /*
             * Get the lookahead.
             */

            string lookAhead = String.Empty;

            string[] split = tag.Context.Split(' ');
            if (split[split.Length - 1] != String.Empty)
                lookAhead = split[split.Length - 1];

            lookAhead = lookAhead.Trim();

            string insertionTemplate = String.Format("{0}=\"{1}\"",
                QuickSharp.CodeAssist.Constants.
                    INSERTION_TEMPLATE_TEXT_PLACEHOLDER,
                QuickSharp.CodeAssist.Constants.
                    INSERTION_TEMPLATE_CPOS_PLACEHOLDER);

            return new LookupList(lookAhead, items, insertionTemplate);
        }

        private LookupList GetTagItemLookupList(AspNetTag tag)
        {
            return null;
        }

        private LookupList GetDollarExpressionLookupList(Match match)
        {
            /*
             * Only create the list if there's one to create!
             */

            if (_webConfigAppSettings.Count == 0 &&
                _webConfigConnectionStrings.Count == 0)
                return null;

            /*
             * Get the appropriate list.
             */

            // No lookup before end tag
            if (match.Groups[4].Value != String.Empty) return null;

            List<LookupListItem> items = new List<LookupListItem>();

            if (match.Groups[2].Value == String.Empty)
            {
                LookupListItem item1 = new LookupListItem();
                item1.DisplayText = Constants.APP_SETTINGS;
                item1.InsertText = Constants.APP_SETTINGS + ":";
                item1.Category = QuickSharp.CodeAssist.Constants.PROPERTIES;
                items.Add(item1);

                LookupListItem item2 = new LookupListItem();
                item2.DisplayText = Constants.CONNECTION_STRINGS;
                item2.InsertText = Constants.CONNECTION_STRINGS + ":";
                item2.Category = QuickSharp.CodeAssist.Constants.PROPERTIES;
                items.Add(item2);

                return new LookupList(match.Groups[1].Value, items);
            }

            if (match.Groups[1].Value == Constants.APP_SETTINGS)
            {
                foreach (String appSetting in _webConfigAppSettings)
                {
                    LookupListItem item = new LookupListItem();
                    item.DisplayText = appSetting;
                    item.InsertText = appSetting;
                    item.Category = QuickSharp.CodeAssist.Constants.CONSTANT;
                    items.Add(item);
                }
            }

            if (match.Groups[1].Value == Constants.CONNECTION_STRINGS)
            {
                foreach (String connectionString in _webConfigConnectionStrings)
                {
                    LookupListItem item = new LookupListItem();
                    item.DisplayText = connectionString;
                    item.InsertText = connectionString;
                    item.Category = QuickSharp.CodeAssist.Constants.CONSTANT;
                    items.Add(item);
                }
            }

            return new LookupList(match.Groups[3].Value, items);
        }

        #endregion

        #region Helpers

        private List<Type> GetAllControlTypes()
        {
            List<String> namespaceList = new List<String>();

            foreach (TagNamespace tn in GetTagNamespaces())
                if (!namespaceList.Contains(tn.Namespace))
                    namespaceList.Add(tn.Namespace);

            return GetNamespaceControlTypes(namespaceList);
        }

        private List<Type> GetTagControlTypes(AspNetTag tag)
        {
            List<String> namespaceList = new List<String>();

            foreach (TagNamespace tn in GetTagNamespaces())
                if (tn.TagPrefix == tag.TagPrefix &&
                    !namespaceList.Contains(tn.Namespace))
                    namespaceList.Add(tn.Namespace);

            return GetNamespaceControlTypes(namespaceList);
        }

        private List<TagNamespace> GetTagNamespaces()
        {
            List<TagNamespace> list = new List<TagNamespace>();
            list.Add(new TagNamespace("asp", "System.Web.UI.WebControls"));
            list.Add(new TagNamespace("asp", "System.Web.UI.WebControls.WebParts"));

            Regex re = new Regex("(?i)<%@\\s*register\\s+tagprefix\\s*=\\s*\\\"(\\w+)\\\"\\s+namespace\\s*=\\s*\\\"([\\w\\.]+)\\\"");

            foreach (Match m in re.Matches(_fullSource))
                list.Add(new TagNamespace(
                    m.Groups[1].Value,
                    m.Groups[2].Value));

            if (_webConfigTagNamespaces != null)
                foreach (TagNamespace tn in _webConfigTagNamespaces)
                    list.Add(tn);

            return list;
        }

        private List<Type> GetNamespaceControlTypes(
            List<String> namespaceList)
        {
            List<Type> types = FindNamespaceTypes(namespaceList);

            List<Type> controls = new List<Type>();

            /*
             * We can get the main controls like this.
             */

            controls.AddRange(GetControlTypes(types, typeof(System.Web.UI.Control)));

            /*  
             * But we also need to get the 'sub elements' used in composite controls such as the
             * data grid and data sources. This is a bit patchy as all the elements aren't here
             * but it covers the most common ones.
             */

            controls.AddRange(GetControlTypes(types, typeof(System.Web.UI.WebControls.DataGridColumn)));
            controls.AddRange(GetControlTypes(types, typeof(System.Web.UI.WebControls.Parameter)));
            controls.AddRange(GetControlTypes(types, typeof(System.Web.UI.WebControls.HotSpot)));

            return controls;
        }

        private List<Type> GetControlTypes(List<Type> types, Type baseType)
        {
            List<Type> controls = new List<Type>();

            foreach (Type type in types)
            {
                if (type.IsAbstract) continue;
                if (type.IsNotPublic) continue;
                
                if (type.Equals(baseType) || type.IsSubclassOf(baseType))
                    controls.Add(type);
            }

            return controls;
        }

        private void AddDeclaredAssemblies(string text)
        {
            Regex re = new Regex("(?i)<%@\\s*assembly\\s+name\\s*=\\s*\\\"(.+)\\\"\\s*%>");

            foreach (Match m in re.Matches(text))
            {
                string a = m.Groups[1].Value.ToLower();
                if (a != String.Empty && !workspaceAssemblyList.Contains(a))
                    workspaceAssemblyList.Add(a);
            }
        }

        private string GetLastToken(string text)
        {
            if (text.EndsWith(" ")) return String.Empty;

            string[] split = text.Split();

            return split[split.Length - 1];
        }

        #endregion

        #region Colorization

        private void Colorize(ScintillaEditForm document)
        {
            string content = document.GetContent() as String;
            string tags = GetAspNetTagsAsString(content);

            document.Editor.Lexing.Keywords[0] = defaultTags + tags;
            document.Editor.Lexing.Colorize();
        }

        private string GetAspNetTagsAsString(string source)
        {
            Regex re = new Regex("(?i)<(\\w+:\\w+)\\s+");

            StringBuilder sb = new StringBuilder();

            foreach (Match m in re.Matches(source))
            {
                sb.Append(" ");
                sb.Append(m.Groups[1].Value.ToLower());
            }

            return sb.ToString();
        }

        private string defaultTags = 
            "!doctype a abbr accept-charset accept accesskey acronym action address align alink alt applet " +
            "archive area axis b background base basefont bdo bgcolor big blockquote body border br button " +
            "caption cellpadding cellspacing center char charoff charset checkbox checked cite class classid " +
            "clear code codebase codetype col colgroup color cols colspan compact content coords data datafld " +
            "dataformatas datapagesize datasrc datetime dd declare defer del dfn dir disabled div dl dt em " +
            "enctype event face fieldset file font for form frame frameborder frameset h1 h2 h3 h4 h5 h6 head " +
            "headers height hidden hr href hreflang hspace html http-equiv i id iframe image img input ins " +
            "isindex ismap kbd label lang language leftmargin legend li link longdesc map marginwidth " +
            "marginheight maxlength media menu meta method multiple name noframes nohref noresize noscript " +
            "noshade nowrap object ol onblur onchange onclick ondblclick onfocus onkeydown onkeypress onkeyup " +
            "onload onmousedown onmousemove onmouseover onmouseout onmouseup optgroup option onreset onselect " +
            "onsubmit onunload p param password profile pre prompt public q radio readonly rel reset rev rows " +
            "rowspan rules s samp scheme scope script select selected shape size small span src standby start " +
            "strike strong style sub submit summary sup tabindex table target tbody td text textarea tfoot th " +
            "thead title topmargin tr tt type u ul usemap valign value valuetype var version vlink vspace width " +
            "xml xmlns runat ";

        #endregion

        #region Web Config

        private void ReadWebConfig()
        {
            /*
             * web.config is located in the current directory unless an
             * ApplicationStorage value has been set by a plugin that
             * determines the root of the current web app (e.g. Cassini).
             */

            ApplicationStorage appStore = ApplicationStorage.GetInstance(); 

            string webConfigDir = 
                appStore[Constants.APP_STORE_KEY_WEB_APP_ROOT] as string;

            if (webConfigDir == null)
                webConfigDir = Directory.GetCurrentDirectory();

            string webConfig = Path.Combine(
                webConfigDir, Constants.WEB_CONFIG_FILENAME);

            /*
             * Reset the config namespace list before we check for the file.
             * If there is no file the list should be empty.
             */

            configNamespaceList.Clear();

            if (!File.Exists(webConfig)) return;

            _webConfigTagNamespaces.Clear();
            _webConfigConnectionStrings.Clear();
            _webConfigAppSettings.Clear();

            /*
             * This looks for the parent elements it needs and assumes
             * that the 'assemblies', 'controls' and 'namespaces' nodes
             * only appear as children in the sections we are interested
             * in.
             */

            XmlTextReader rdr = null;

            try
            {
                rdr = new XmlTextReader(webConfig);
                bool readAssemblies = false;
                bool readControls = false;
                bool readNamespaces = false;
                bool readConnectionStrings = false;
                bool readAppSettings = false;

                while (rdr.Read())
                {
                    if (rdr.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        switch (rdr.Name)
                        {
                            case "assemblies":
                                readAssemblies = true;
                                break;

                            case "controls":
                                readControls = true;
                                break;

                            case "namespaces":
                                readNamespaces = true;
                                break;

                            case "connectionStrings":
                                readConnectionStrings = true;
                                break;

                            case "appSettings":
                                readAppSettings = true;
                                break;

                            case "add":

                                if (readAssemblies)
                                {
                                    /*
                                     * Update the assembly list.
                                     */

                                    string a = rdr.GetAttribute("assembly");

                                    if (a != null)
                                    {
                                        a = a.ToLower();
                                        if (!workspaceAssemblyList.Contains(a))
                                            workspaceAssemblyList.Add(a);
                                    }
                                }
                                else if (readControls)
                                {
                                    /*
                                     * Update the controls list.
                                     */

                                    string tp = rdr.GetAttribute("tagPrefix");
                                    string ns = rdr.GetAttribute("namespace");

                                    if (tp != null && ns != null)
                                        _webConfigTagNamespaces.Add(
                                            new TagNamespace(tp, ns));
                                }
                                else if (readNamespaces)
                                {
                                    configNamespaceList.Add(rdr.GetAttribute("namespace"));
                                }
                                else if (readConnectionStrings)
                                {
                                    _webConfigConnectionStrings.Add(rdr.GetAttribute("name"));
                                }
                                else if (readAppSettings)
                                {
                                    _webConfigAppSettings.Add(rdr.GetAttribute("key"));
                                }

                                break;

                            /*
                             * As we're only scanning the application level config
                             * we can ignore the clear and remove tags.
                             */
                        }
                    }
                    else if (rdr.NodeType == System.Xml.XmlNodeType.EndElement)
                    {
                        switch (rdr.Name)
                        {
                            case "assemblies":
                                readAssemblies = false;
                                break;

                            case "controls":
                                readControls = false;
                                break;

                            case "namespaces":
                                readNamespaces = false;
                                break;

                            case "connectionStrings":
                                readConnectionStrings = false;
                                break;

                            case "appSettings":
                                readAppSettings = false;
                                break;
                        }
                    }
                }
            }
            catch
            {
                // Give up!
            }
            finally
            {
                if (rdr != null) rdr.Close();
            }
        }

        #endregion
    }
}
