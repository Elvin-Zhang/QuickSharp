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
using QuickSharp.CodeAssist.DotNet;
using QuickSharp.Core;

namespace QuickSharp.CodeAssist.CSharp
{
    public abstract class CSharpCodeAssistProviderBase
        : CodeAssistProviderBase
    {
        #region C# Code Assist Driver

        protected LookupList GetCSharpLookupList(LookupContext context)
        {
            List<LookupListItem> lookupItemList = null;

            /******************************************************************
             * CodeAssist lookups start here. The type of lookup performed
             * depends on the context of the search target and the result
             * of each lookup so order is important. At each stage we ask
             * "what are we looking for?" and "what are we expecting to find?"
             ******************************************************************/

            /*
             * Are we looking for namespaces as part of a 'using' declaration?
             * We have typed 'using' and now expect to see a list of available
             * namespaces (as determined by the assemblies available in the
             * assembly search list). If nothing else has been typed, the root
             * namespaces will be shown; if a partial namespace has been entered
             * (i.e. the name includes one or more '.') the appropriate child
             * namespaces are shown.
             */

            lookupItemList = GetChildNamespaces(
                context.Target, context.Line);

            if (lookupItemList != null)
                return new LookupList(
                    context.Target.LookAhead, lookupItemList);

            /*
             * If before the class definition give up looking.
             */

            if (!context.BeforeClass) return null;

            /*
             * Are we looking for members of a namespace?
             * We have typed a name including at least one '.' and expect to
             * see the relevant members of this entity. This could be a
             * namespace, class or variable. First we check to see if we get
             * anything back by assuming it's a namespace.
             */

            if (context.Target.Entity != String.Empty)
            {
                List<LookupListItem> types = 
                    FindNamespaceTypeLookupItems(context.Target.Entity);

                List<LookupListItem> namespaces = 
                    FindNamespaceChildLookupItems(context.Target.Entity);

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
                return new LookupList(
                    context.Target.LookAhead, lookupItemList);

            /*
             * Get the local variables declared within the scope visible from
             * the lookup location (the caret) and any variables inherited
             * from the base class.
             */

            /*
             * Are we in a static or instance method?
             * Set the context accordingly.
             */

            GetDeclarationContext(context);

            context.DeclaredVariables = GetDeclaredVariables(
                context.PreSource, true,
                context.DeclarationContext);

            context.InheritedVariables = GetInheritedVariables(
                context.PreSource,
                context.DeclarationContext,
                workspaceAssemblyList,
                fullNamespaceList,
                rootNamespaceList);

            /*
             * Are we looking for anything in scope?
             * We have typed a name without a '.' so it could be a local
             * variable, a class visible within any of the imported namespaces
             * or any root namespace in the referenced assemblies.
             */

            if (String.IsNullOrEmpty(context.Target.Entity))
            {
                lookupItemList = GetVisibleTypes(context,
                    true, true, true);

                if (lookupItemList != null)
                {
                    // Add the root namespaces
                    foreach (string ns in rootNamespaceList)
                    {
                        LookupListItem item = new LookupListItem();
                        item.DisplayText = ns;
                        item.InsertText = ns;
                        item.Category = 
                            QuickSharp.CodeAssist.Constants.NAMESPACE;
                        item.ToolTipText = String.Format("namespace {0}", ns);
                        lookupItemList.Add(item);
                    }

                    return new LookupList(
                        context.Target.LookAhead, lookupItemList);
                }
            }

            /*
             * The target cannot be blank after this point; if we haven't
             * found anything using a blank target then there's nothing
             * to find.
             */

            if (String.IsNullOrEmpty(context.Target.Entity)) return null;

            /******************************************************************
             * From this point on we must have at least one '.' in the target
             * so we're looking for members of various entities not the
             * entities themselves. These could be the current class or its
             * base; variables (instance member lookup) or classes (static
             * member lookup).
             ******************************************************************/

            /*
             * Convert C# type names to .NET type names.
             */

            context.Target.Entity =
                CSharpFormattingTools.ToCTSType(context.Target.Entity);

            /*
             * Are we looking for members of the current class?
             * This is inaccurate in that it doesn't include the members of
             * the current type (as we're not parsing the code) only the
             * inherited members and local variables. It's basically the
             * same as 'base' with variables added.
             */

            if (context.Target.Entity == "this")
            {
                lookupItemList = GetVisibleTypes(
                    context, true, false, true);
                
                if (lookupItemList != null)
                    return new LookupList(
                        context.Target.LookAhead, lookupItemList);
            }

            /*
             * Are we looking for members of the base class?
             */

            if (context.Target.Entity == "base")
            {
                lookupItemList = GetVisibleTypes(
                    context, false, false, false);
                
                if (lookupItemList != null)
                    return new LookupList(
                        context.Target.LookAhead, lookupItemList);
            }

            /*
             * If the target is a sub-member of 'this' or 'base' we can
             * treat this as though the 'this' or 'base' aren't there.
             */

            if (context.Target.Entity.StartsWith("this.") ||
                context.Target.Entity.StartsWith("base."))
                context.Target.Entity = context.Target.Entity.Substring(5);

            /*
             * Are we looking for members of a variable?
             * We will have the variable name followed by one or more
             * sub-members (e.g. sr.ReadLine().ToString().GetType()...).
             * We need to convert the variable to it's type
             * (e.g. StreamReader) and perform instance lookups
             * down the member chain getting the return type of
             * each so we can get its members, find the type of the next
             * and so on. The last type found is the type we need to
             * show in the lookup list.
             */

            Variable variable = context.DeclaredVariables.
                GetVariable(context.Target.Entity);

            if (variable == null)
                variable = context.Properties.
                    GetProperty(context.Target.Entity);

            if (variable == null)
                variable = context.InheritedVariables.
                    GetVariable(context.Target.Entity);
            
            if (variable != null)
            {
                context.Target.Name = variable.Name;

                String[] split = context.Target.Entity.Split('.');

                /*
                 * An added complication is if the variable is a collection
                 * type such as an array or List<T>. If we have item operators
                 * (e.g. list[]) we need to convert the variable type to
                 * the type being collected not the collection itself. So if
                 * we have a List<String>, List. presents members of the
                 * List class but List[]. presents members of the String class.
                 */

                if (context.Target.IsIndexed)
                {
                    context.Target.Entity =
                        variable.GetVariableCollectionType(
                            context.Target.Entity);
                }
                else
                {
                    split[0] = variable.Type;
                    context.Target.Entity = String.Join(".", split);
                }

                lookupItemList = GetTypeMembers(
                    context, context.Target, DeclarationContext.Instance,
                    null, false, false);

                if (lookupItemList != null)
                    return new LookupList(
                        context.Target.LookAhead, lookupItemList);
            }

            /*
             * Are we looking for members of a local
             * method's return type?
             */

            string[] methodSplit = context.Target.Name.Split('.');

            LocalMethods.MethodDefinition method = 
                context.Methods.GetMethod(methodSplit[0]);

            if (method != null)
            {
                int i = context.Target.Entity.IndexOf('.');

                if (i == -1)
                    context.Target.Entity = method.ReturnType;
                else
                    context.Target.Entity =
                        method.ReturnType +
                        context.Target.Entity.Substring(i);
                        
                lookupItemList = GetTypeMembers(
                    context, context.Target, DeclarationContext.Instance,
                    null, false, false);

                if (lookupItemList != null)
                    return new LookupList(
                        context.Target.LookAhead, lookupItemList);
            }

            /*
             * Are we looking for members of a class?
             * We expect to see a list of static members of the class. This is
             * almost the same as the previous instance lookup except that we
             * already know the type of the entity and our first level member
             * lookup will be for static members.
             */

            lookupItemList = GetTypeMembers(
                context, context.Target, DeclarationContext.Static,
                null, false, false);

            return new LookupList(
                context.Target.LookAhead, lookupItemList);

            /*
             * If the final lookup finds nothing we will return null.
             */
        }

        #endregion

        #region Overrides

        protected abstract List<LookupListItem> GetChildNamespaces(
            LookupTarget target, string text);

        protected abstract DeclaredVariables GetDeclaredVariables(
            string source, bool visibleScopesOnly,
            DeclarationContext declarationContext);

        protected abstract InheritedVariablesBase GetInheritedVariables(
            string source,
            DeclarationContext declarationContext,
            List<string> workspaceAssemblies,
            List<string> fullNamespaces,
            List<string> rootNamespaces);

        #endregion

        #region Cleanup and Declaration Context

        protected string GetFullSource(string text, ref int pos)
        {
            /*
             * This is pretty crude - it just attempts to remove
             * class definitions outside the current one from
             * the text so that we don't get out of scope methods
             * and properties showing up. It will also get confused
             * by nested classes - the scope of the current class will start
             * at the beginning of the last nested class before the caret.
             * 
             * We also need to preserve the current cursor location within
             * the source so that the method/property and declaration
             * context detection will work correctly.
             */

            text = text.Insert(pos, "¬¬");
            text = CSharpFormattingTools.RemoveUnwantedText(text);

            pos = text.IndexOf("¬¬");
            if (pos == -1) return String.Empty;

            string src1 = text.Substring(0, pos);
            string src2 = text.Substring(pos);

            int start = src1.LastIndexOf("class ");
            int end = src2.IndexOf("class ");

            if (start != -1)
                src1 = src1.Substring(start);

            if (end != -1)
                src2 = src2.Substring(0, end);

            text = src1 + src2;

            pos = text.IndexOf("¬¬");

            text = text.Replace("¬¬", String.Empty);

            /*
             * Find the location of the closing '}'.
             */

            int i = 0;
            int braceLevel = 0;
            while (i < text.Length && text[i] != '{') i++;

            while (i < text.Length)
            {
                if (text[i] == '{') braceLevel++;
                if (text[i] == '}') braceLevel--;

                if (braceLevel == 0) break;

                i++;
            }

            return text.Substring(0, i);
        }

        private void GetDeclarationContext(LookupContext context)
        {
            foreach (LocalMethods.MethodDefinition method in 
                context.Methods.Items)
            {
                if (method.StartPos < context.CurrentPos &&
                    method.EndPos >= context.CurrentPos)
                {
                    if (method.IsStatic)
                        context.DeclarationContext =
                            DeclarationContext.Static;
                    else
                        context.DeclarationContext =
                            DeclarationContext.All;

                    return;
                }
            }

            foreach (LocalProperties.PropertyDefinition property in
                context.Properties.Items)
            {
                if (property.StartPos < context.CurrentPos &&
                    property.EndPos >= context.CurrentPos)
                {
                    if (property.IsStatic)
                        context.DeclarationContext =
                            DeclarationContext.Static;
                    else
                        context.DeclarationContext =
                            DeclarationContext.All;

                    return;
                }
            }

            context.DeclarationContext = DeclarationContext.Static;
        }

        #endregion
    }
}
