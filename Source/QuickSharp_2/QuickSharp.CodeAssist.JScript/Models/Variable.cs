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
using QuickSharp.CodeAssist.DotNet;

namespace QuickSharp.CodeAssist.JScript
{
    public class Variable
    {
        public string Name { get; set; }
        public string Type { get; set; } 
        public int Index { get; set; }
        public bool IsArray { get; set; }
        public string ArrayType { get; set; }

        public string GetVariableType()
        {
            if (IsArray)
            {
                return String.Format("{0}[]", ArrayType);
            }
            else
            {
                return Type;
            }
        }

        public string GetVariableCollectionType(string entity)
        {
            /*
             * For arrays we return the item that the object is a
             * collection of. For anything else we need to swap in
             * the base type and append 'Item'.
             */

            if (IsArray)
                 return JScriptFormattingTools.ToCTSType(ArrayType);
            else
            {
                string[] split = entity.Split('.');
                split[0] = Type;

                return String.Join(".", split) + ".Item";
            }
        }
    }
}
