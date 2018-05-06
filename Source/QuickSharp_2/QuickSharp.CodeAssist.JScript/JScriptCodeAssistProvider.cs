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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using QuickSharp.CodeAssist;
using QuickSharp.CodeAssist.DotNet;
using QuickSharp.Core;
using ScintillaNet;

namespace QuickSharp.CodeAssist.JScript
{
    public class JScriptCodeAssistProvider : 
        JScriptCodeAssistProviderBase,
        ICodeAssistProvider
    {
        #region ICodeAssistProvider

        public DocumentType DocumentType
        {
            get
            {
                return new DocumentType(Constants.DOCUMENT_TYPE);
            }
        }

        public bool IsAvailable
        {
            get { return true; }
        }

        public void DocumentActivated(
            QuickSharp.Editor.ScintillaEditForm document)
        {
            if ((settingsManager.ColorizeTypes ||
                settingsManager.ColorizeVariables) &&
                settingsManager.ColorizeOnActivate)
            {
                UpdateLists();

                ColourizeVariablesAndTypes(document);
            }
        }

        public LookupList GetLookupList(
            QuickSharp.Editor.ScintillaEditForm document)
        {
            return GetJScriptLookupList(document);
        }

        #endregion

        #region JScript Code Assist Driver

        private LookupList GetJScriptLookupList(
            QuickSharp.Editor.ScintillaEditForm document)
        {
            UpdateLists();

            if ((settingsManager.ColorizeTypes ||
                settingsManager.ColorizeVariables) &&
                settingsManager.ColorizeOnLookup)
                ColourizeVariablesAndTypes(document);

            /*
             * Get the search target from the current line.
             */

            Line line = document.Editor.Lines.Current;

            string text = line.Text.Substring(0,
                line.SelectionStartPosition -
                line.StartPosition).TrimStart();

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
             * Cleanup the source.
             */

            text = JScriptFormattingTools.RemoveUnwantedText(text);
            text = JScriptFormattingTools.RemoveUnwantedBracketText(text);
            text = JScriptFormattingTools.RemoveUnwantedParentheses(text);

            LookupTarget target = new LookupTarget(text);
            List<LookupListItem> lookupItemList;            

            string source = document.GetContent() as string;
            source = source.Substring(0, document.Editor.CurrentPos);
            source = JScriptFormattingTools.RemoveUnwantedText(source);
            source = JScriptFormattingTools.RemoveUnwantedBracketText(source);

            /*
             * Looking for namespaces as part of an 'import'
             * declaration...
             */

            lookupItemList = GetChildNamespaces(target, text);

            if (lookupItemList != null)
                return new LookupList(target.LookAhead, lookupItemList);

            /*
             * Looking for members of a namespace...
             */

            if (target.Entity != String.Empty)
            {
                List<LookupListItem> types =
                    FindNamespaceTypeLookupItems(target.Entity);

                List<LookupListItem> namespaces =
                    FindNamespaceChildLookupItems(target.Entity);

                if (types != null || namespaces != null)
                {
                    lookupItemList = new List<LookupListItem>();
                    if (types != null)
                        lookupItemList.AddRange(types);
                    if (namespaces != null)
                        lookupItemList.AddRange(namespaces);
                }
            }

            if (lookupItemList != null)
                return new LookupList(target.LookAhead, lookupItemList);

            /*
             * Get the local variables declared within the scope visible
             * from the lookup location (the caret)...
             */

            DeclaredVariables declaredVariables =
                new DeclaredVariables(source, fullNamespaceList, true);

            /*
             * Looking for anything in scope...
             */

            if (String.IsNullOrEmpty(target.Entity))
            {
                lookupItemList = GetVisibleTypes(
                    source, declaredVariables, true, true);

                if (lookupItemList != null)
                {
                    // Add the root namespaces
                    foreach (string ns in rootNamespaceList)
                    {
                        LookupListItem item = new LookupListItem();
                        item.DisplayText = ns;
                        item.InsertText = ns;
                        item.Category = QuickSharp.CodeAssist.Constants.NAMESPACE;
                        item.ToolTipText = String.Format("namespace {0}", ns);
                        lookupItemList.Add(item);
                    }

                    return new LookupList(target.LookAhead, lookupItemList);
                }
            }

            /*
             * The target cannot be blank after this point; if we haven't found
             * anything using a blank target then there's nothing to find.
             */

            if (String.IsNullOrEmpty(target.Entity)) return null;

            /*
             * Looking for members of a variable...
             */

            Variable variable = declaredVariables.GetVariable(target.Entity);

            if (variable != null)
            {
                String[] split = target.Entity.Split('.');

                if (target.IsIndexed)
                {
                    target.Entity = variable.GetVariableCollectionType(target.Entity);
                }
                else
                {
                    split[0] = variable.Type;
                    target.Entity = String.Join(".", split);
                }

                lookupItemList = GetTypeMembers(
                    source, target, false, false);

                if (lookupItemList != null)
                    return new LookupList(target.LookAhead, lookupItemList);
            }

            /*
             * Looking for members of a class...
             */

            lookupItemList = GetTypeMembers(source, target, true, false);

            return new LookupList(target.LookAhead, lookupItemList);
        }

        #endregion

        #region Colorization

        private void ColourizeVariablesAndTypes(
            QuickSharp.Editor.ScintillaEditForm document)
        {
            string source = document.GetContent() as string;
            source = CSharpFormattingTools.RemoveUnwantedText(source);
            source = CSharpFormattingTools.RemoveUnwantedBracketText(source);

            List<string> namespaces = GetNamespaceList(source);

            if (settingsManager.ColorizeTypes)
            {
                // Lexer WORD2 - types

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
                    "Array Boolean Date Enumerator Error Function Number Object RegExp String VBArray " +
                    CodeAssistTools.GetNamespaceTypesAsString(
                        namespaces, assemblies);
            }

            if (settingsManager.ColorizeVariables)
            {
                // Lexer GLOBALCLASS - variables
                DeclaredVariables declaredVariables =
                    new DeclaredVariables(source, fullNamespaceList, false);

                StringBuilder sb = new StringBuilder();

                foreach (Variable v in declaredVariables.Items)
                {
                    sb.Append(v.Name);
                    sb.Append(" ");
                }

                document.Editor.Lexing.Keywords[3] = sb.ToString();
            }

            document.Editor.Lexing.Colorize();
        }

        #endregion
    }
}
