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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace QuickSharp.CodeAssist.DotNet
{
    public static class JScriptFormattingTools
    {
        #region Unwanted text

        public static string RemoveUnwantedText(string text)
        {
            text = text.Replace("\r\n", "\n");

            StringBuilder sb = new StringBuilder();
            int max = text.Length - 1;
            int i = 0;

            while (i < text.Length)
            {
                if (text[i] == '/' && i < max && text[i + 1] == '/')
                {
                    // Goto line end
                    while (i < max && text[i] != '\n') i++;
                }
                else if (text[i] == '/' && i < max && text[i + 1] == '*')
                {
                    // Goto next '*/'
                    while (i < max)
                    {
                        if (text[i] == '*' && text[i + 1] == '/') break;
                        i++;
                    }
                    i += 2;
                }
                else if (text[i] == '"')
                {
                    sb.Append(text[i]);
                    if (i < max) i++;

                    // Goto next unescaped '"'
                    while (i < max)
                    {
                        if (text[i] == '"' && i > 0 && text[i - 1] != '\\') break;
                        i++;
                    }
                }
                else if (text[i] == '\'')
                {
                    sb.Append(text[i]);
                    if (i < max) i++;

                    // Goto next unescaped '''
                    while (i < max)
                    {
                        if (text[i] == '\'' && i > 0 && text[i - 1] != '\\') break;
                        i++;
                    }
                }
                else if (text[i] == '#' && i < max)
                {
                    // Goto line end
                    while (i < max && text[i] != '\n') i++;
                }

                if (i <= max) sb.Append(text[i]);
                i++;
            }

            return sb.ToString();
        }

        public static string RemoveUnwantedBracketText(string text)
        {
            StringBuilder sb = new StringBuilder();

            int level = 0;

            int i = text.Length - 1;

            while (i >= 0)
            {
                if (text[i] == ']')
                {
                    if (level == 0)
                        sb.Insert(0, text[i]);
                    level++;
                }
                if (text[i] == '[')
                {
                    level--;
                    if (level < 0)
                        level = 0;
                }

                if (level == 0)
                    sb.Insert(0, text[i]);

                i--;
            }

            return sb.ToString();
        }

        public static string RemoveUnwantedParentheses(string text)
        {
            StringBuilder sb = new StringBuilder();

            int level = 0;

            int i = text.Length - 1;

            while (i >= 0)
            {
                if (text[i] == ')')
                {
                    level++;
                }
                if (text[i] == '(')
                {
                    level--;
                    if (level == 0) i--;
                    if (level < 0) level = 0;
                }

                if (level == 0 && i >= 0)
                    sb.Insert(0, text[i]);

                i--;
            }

            return sb.ToString();
        }

        #endregion

        #region Scope

        public static string RemoveInaccessibleScopes(string text)
        {
            int i = 0;
            int j = 0;
            int braceLevel = 0;

            /*
             * Mark the beginning and end of function
             * and class blocks.
             */

            text = text.Replace("class", "¬");
            text = text.Replace("function", "¬");
                
            char[] c = text.ToCharArray();

            while (j < c.Length)
            {
                // Look for the start of each function block
                if (c[j] == '¬')
                {
                    // Mark the corresponding block end
                    i = j;
                    braceLevel = 0;
                    while (i < c.Length)
                    {
                        if (c[i] == '{') braceLevel++;
                        if (c[i] == '}')
                        {
                            braceLevel--;
                            if (braceLevel == 0)
                            {
                                c[i] = '`';
                                break;
                            }
                        }

                        i++;
                    }
                }

                j++;
            }

            /*
             * Remove any out of scope text.
             */

            StringBuilder sb = new StringBuilder();

            i = c.Length - 1;
            braceLevel = 0;

            // Backtrack to current block start or previous block end
            while (i >= 0)
            {
                if (c[i] == '¬' || c[i] == '`')
                    break;

                sb.Insert(0, c[i]);
                i--;
            }

            // Backtrack to start, skipping out of scope blocks
            while (i >= 0)
            {
                if (c[i] == '`')
                {
                    while (i >= 0)
                    {
                        if (c[i] == '`') braceLevel++;
                        if (c[i] == '¬') braceLevel--;
                        i--;

                        if (braceLevel == 0) break;
                    }
                }

                if (i >= 0) sb.Insert(0, c[i]);
                i--;
            }

            return sb.ToString();
        }

        #endregion

        #region Namespace declarations

        public static string RemoveNamespaceDeclarations(string text)
        {
            Regex re = new Regex(@"import\s+([\w\.]*);");
            return re.Replace(text, String.Empty);
        }

        #endregion

        #region Formatting

        /* 
         * Although JScript doesn't support Generics, the
         * framework class library does; some types listed
         * in the lookups will be generics and we need to make
         * them look nice even if we can't use them!
         */

        public static string RemoveGenericSignature(string name)
        {
            int i = name.IndexOf('`');
            if (i != -1)
                return name.Substring(0, i);
            else
                return name;
        }

        public static string GetTypeSignature(Type type)
        {
            return GetTypeSignature(type, false);
        }

        public static string GetTypeSignature(Type type, bool fullName)
        {
            StringBuilder sb = new StringBuilder();

            if (type.IsNested)
            {
                string name = type.FullName;
                if (name == null) name = type.Name;

                string[] split1 = name.Split('+');

                if (fullName)
                    sb.Append(RemoveGenericSignature(split1[0]));
                else
                {
                    string[] split2 = split1[0].Split('.');
                    sb.Append(RemoveGenericSignature(split2[split2.Length - 1]));

                }

                if (type.IsGenericType)
                    sb.Append(GetGenericTypeArgs(type));

                if (split1.Length > 1)
                {
                    for (int i = 1; i < split1.Length; i++)
                    {
                        sb.Append(".");
                        sb.Append(split1[i]);
                    }
                }
            }
            else
            {
                if (fullName)
                    sb.Append(RemoveGenericSignature(type.FullName));
                else
                    sb.Append(RemoveGenericSignature(type.Name));
                
                if (type.IsGenericType)
                    sb.Append(GetGenericTypeArgs(type));
            }

            return sb.ToString();
        }

        public static string GetMethodSignature(MethodInfo methodInfo)
        {
            return GetMethodSignature(methodInfo, false);
        }

        public static string GetMethodSignature(MethodInfo methodInfo,
            bool isExtensionMethod)
        {
            return GetMethodSignature(methodInfo, isExtensionMethod, false);
        }
        
        public static string GetMethodSignature(MethodInfo methodInfo, 
            bool isExtensionMethod, bool hideExtensionTarget)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(RemoveGenericSignature(methodInfo.Name));

            if (methodInfo.IsGenericMethod)
                sb.Append(GetGenericMethodArgs(methodInfo));

            sb.Append(GetMethodParameters(
                methodInfo.GetParameters(), 
                isExtensionMethod, 
                hideExtensionTarget));

            return sb.ToString();
        }

        public static string GetMinimalMethodSignature(
            MethodInfo methodInfo)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(RemoveGenericSignature(methodInfo.Name));

            if (methodInfo.IsGenericMethod)
                sb.Append(GetGenericMethodArgs(methodInfo));

            sb.Append(GetMethodParameterNames(
                methodInfo.GetParameters()));

            return sb.ToString();
        }

        public static string GetConstructorSignature(
            ConstructorInfo constructorInfo, Type type)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(RemoveGenericSignature(
                constructorInfo.DeclaringType.Name));

            if (constructorInfo.IsGenericMethod)
                sb.Append(GetGenericConstructorArgs(type));

            sb.Append(GetMethodParameters(
                constructorInfo.GetParameters(), false));

            return sb.ToString();
        }

        public static string GetMinimalConstructorSignature(
            ConstructorInfo constructorInfo, Type type)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(RemoveGenericSignature(
                constructorInfo.DeclaringType.Name));

            sb.Append(GetGenericConstructorArgs(type));

            sb.Append(GetMethodParameterNames(
                constructorInfo.GetParameters()));

            return sb.ToString();
        }

        public static string GetGenericTypeArgs(Type type)
        {
            StringBuilder sb = new StringBuilder();

            Type[] args = 
                type.GetGenericTypeDefinition().GetGenericArguments();

            if (args.Length > 0) sb.Append("<");

            for (int i = 0; i < args.Length; i++)
            {
                sb.Append(args[i].Name);
                if (i < args.Length - 1) sb.Append(",");
            }

            if (args.Length > 0) sb.Append(">");

            return sb.ToString();
        }

        public static string GetGenericMethodArgs(MethodInfo methodInfo)
        {
            StringBuilder sb = new StringBuilder();

            Type[] args =
                methodInfo.GetGenericMethodDefinition().GetGenericArguments();

            if (args.Length > 0) sb.Append("<");

            for (int i = 0; i < args.Length; i++)
            {
                sb.Append(args[i].Name);
                if (i < args.Length - 1) sb.Append(",");
            }

            if (args.Length > 0) sb.Append(">");

            return sb.ToString();
        }

        public static string GetGenericConstructorArgs(Type type)
        {
            string s = CSharpFormattingTools.GetTypeSignature(type);

            int i = s.IndexOf('<');
            if (i == -1)
                return String.Empty;
            else
                return s.Substring(i);
        }

        public static string GetMethodParameters(
            ParameterInfo[] parameters,
            bool isExtensionMethod)
        {
            return GetMethodParameters(parameters, isExtensionMethod, false);
        }

        public static string GetMethodParameters(
            ParameterInfo[] parameters, 
            bool isExtensionMethod, bool hideExtensionTarget)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("(");

            if (isExtensionMethod && !hideExtensionTarget) sb.Append("this ");

            for (int i = 0; i < parameters.Length; i++)
            {
                if (i == 0 && hideExtensionTarget) continue;

                if (parameters[i].ParameterType.IsGenericType)
                {
                    sb.Append(RemoveGenericSignature(parameters[i].ParameterType.Name));
                    sb.Append(GetGenericTypeArgs(parameters[i].ParameterType));
                }
                else
                {
                    string typeName = parameters[i].ParameterType.Name;
                    if (typeName.EndsWith("&"))
                    {
                        typeName = typeName.Substring(0, typeName.Length - 1);
                        typeName = (parameters[i].IsOut ? "out " : "ref ") + typeName;
                    }

                    sb.Append(typeName);
                }

                sb.Append(" ");
                sb.Append(parameters[i].Name);

                if (i < parameters.Length - 1) sb.Append(", ");
            }

            sb.Append(")");

            return sb.ToString();
        }

        public static string GetMethodParameterNames(
            ParameterInfo[] parameters)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("(");

            for (int i = 0; i < parameters.Length; i++)
            {
                sb.Append(parameters[i].Name);
                if (i < parameters.Length - 1) sb.Append(", ");
            }

            sb.Append(")");

            return sb.ToString();
        }

        #endregion

        #region Type conversion

        public static string ToCTSType(string type)
        {
            if (type == "boolean") return "System.Boolean";
            if (type == "char") return "System.Char";
            if (type == "float") return "System.Single";
            if (type == "double") return "System.Double";
            if (type == "decimal") return "System.Decimal";
            if (type == "byte") return "System.Byte";
            if (type == "ushort") return "System.UInt16";
            if (type == "uint") return "System.UInt32";
            if (type == "ulong") return "System.UInt64";
            if (type == "sbyte") return "System.SByte";
            if (type == "short") return "System.Int16";
            if (type == "int") return "System.Int32";
            if (type == "long") return "System.Int64";

            return type;
        }

        #endregion
    }
}
