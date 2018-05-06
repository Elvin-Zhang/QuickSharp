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
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using QuickSharp.CodeAssist.DotNet;
using QuickSharp.Core;
using QuickSharp.Editor;
using ScintillaNet;

namespace QuickSharp.CodeAssist.CSharp
{
    public class CSharpCodeAssistProvider
        : CSharpCodeAssistProviderBase, ICodeAssistProvider
    {
        #region ICodeAssistProvider

        public virtual DocumentType DocumentType
        {
            get
            {
                return new DocumentType(Constants.DOCUMENT_TYPE);
            }
        }

        public virtual bool IsAvailable
        {
            get { return true; }
        }

        public virtual void DocumentActivated(ScintillaEditForm document)
        {
            if ((settingsManager.ColorizeTypes ||
                settingsManager.ColorizeVariables) &&
                settingsManager.ColorizeOnActivate)
            {
                UpdateLists();

                Control parent = document.Parent;

                /*
                 * Use the wait cursor to make document
                 * switching look a bit snappier.
                 */

                try
                {
                    if (parent != null)
                        parent.Cursor = Cursors.WaitCursor;
                    
                    ColourizeVariablesAndTypes(document);
                }
                finally
                {
                    if (parent != null)
                        parent.Cursor = Cursors.Default;
                }
            }
        }

        public virtual LookupList GetLookupList(ScintillaEditForm document)
        {
            UpdateLists();

            if ((settingsManager.ColorizeTypes ||
                settingsManager.ColorizeVariables) &&
                settingsManager.ColorizeOnLookup)
                ColourizeVariablesAndTypes(document);

            /*
             * Prepare the search content and target for
             * the code assist lookup.
             */

            Line line = document.Editor.Lines.Current;

            string text = line.Text.Substring(0,
                line.SelectionStartPosition -
                line.StartPosition);

            int lineStartPos = GetLineStartPosition(text);

            text = text.TrimStart();

            /*
             * Check for embedded option or empty comment.
             */

            if (text.StartsWith("//"))
            {
                if (text.Length > 2 && "$?&".IndexOf(text[2]) != -1)
                    return EmbeddedOptionHelper.GetEmbbededOptionFileList(text);
                else
                    return null;
            }

            /*
             * Get the full source and the source up to the
             * caret position.
             */

            string fullSource = document.GetContent() as string;

            int currentPos = document.Editor.CurrentPos;
            string preSource = fullSource.Substring(0, currentPos);

            /*
             * Restrict the fullSource to the current class definition.
             */

            fullSource = GetFullSource(fullSource, ref currentPos);
            if (fullSource == String.Empty) return null;
            if (currentPos > fullSource.Length) return null;

            /*
             * Detect if we are before the first class declaration.
             * If not we can stop the lookup as soon as we've done
             * the 'using' search as this is the only lookup type
             * valid before a class definition. This means 'using'
             * declarations have to appear before the class defs
             * but I think that's good practice anyway.
             */
            
            Regex re = new Regex(@"(?s)class\s+.+\{");
            bool beforeClass = re.Match(preSource).Success;

            LookupContext context = new LookupContext(
                fullSource, preSource, text,
                lineStartPos, currentPos, beforeClass);

            return GetCSharpLookupList(context);
        }

        #endregion

        #region Colorization

        private void ColourizeVariablesAndTypes(ScintillaEditForm document)
        {
            string source = CSharpFormattingTools.
                RemoveUnwantedText(document.GetContent() as string);

            source = CSharpFormattingTools.
                RemoveUnwantedBracketText(source);

            List<string> namespaces = GetNamespaceList(source);

            if (settingsManager.ColorizeTypes)
            {
                /*
                 * Lexer WORD2 - types
                 */

                List<string> assemblies = new List<string>();

                foreach (string ns in namespaces)
                {
                    List<string> names = 
                        referenceManager.GetNamespaceAssemblies(ns);
                 
                    foreach (string name in names)
                        if (!assemblies.Contains(name))
                            assemblies.Add(name);
                }

                assemblies.AddRange(workspaceAssemblyList);

                document.Editor.Lexing.Keywords[1] =
                    CodeAssistTools.GetNamespaceTypesAsString(
                        namespaces, assemblies);
            }

            if (settingsManager.ColorizeVariables)
            {
                /*
                 * Lexer GLOBALCLASS - variables
                 */

                DeclaredVariables declaredVariables =
                    new DeclaredVariables(
                        source, namespaces, false,
                        DeclarationContext.All);

                InheritedVariablesBase inheritedVariables
                    = new InheritedVariablesCode(
                        source,
                        DeclarationContext.All,
                        workspaceAssemblyList,
                        fullNamespaceList,
                        rootNamespaceList,
                        configNamespaceList);

                StringBuilder sb = new StringBuilder();

                foreach (Variable v in declaredVariables.Items)
                {
                    sb.Append(v.Name);
                    sb.Append(" ");
                }

                foreach (Variable v in inheritedVariables.Items)
                {
                    sb.Append(v.Name);
                    sb.Append(" ");
                }

                document.Editor.Lexing.Keywords[3] = sb.ToString();
            }

            document.Editor.Lexing.Colorize();
        }

        #endregion

        #region Overrides

        protected override List<LookupListItem> GetChildNamespaces(
            LookupTarget target, string text)
        {
            return GetChildNamespaceList(target, text);
        }

        protected override DeclaredVariables GetDeclaredVariables(
            string source, bool visibleScopesOnly,
            DeclarationContext declarationContext)
        {
            return new DeclaredVariables(source,
                fullNamespaceList, visibleScopesOnly,
                declarationContext);
        }

        protected override InheritedVariablesBase GetInheritedVariables(
            string source,
            DeclarationContext declarationContext,
            List<string> workspaceAssemblies,
            List<string> fullNamespaces,
            List<string> rootNamespaces)
        {
            return new InheritedVariablesCode(
                source,
                declarationContext,
                workspaceAssemblies,
                fullNamespaces,
                rootNamespaces,
                configNamespaceList);
        }

        protected override List<string> GetBaseTypes(string source)
        {
            return GetCodeBaseTypeList(source);
        }

        protected override List<String> GetNamespaceList(string source)
        {
            return GetCodeDeclaredNamespaceList(source);
        }

        #endregion
    }
}
