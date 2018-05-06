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

namespace QuickSharp.CodeAssist.CSharp
{
    public class Variable
    {
        private string _name;
        private int _index;
        private string _type;
        private string _declaredType;
        private bool _isLocal;
        private bool _isStatic;
        private bool _isArray;
        private string _arrayType;
        private bool _isGeneric;
        private int _genericOrder;
        private string _genericType1;
        private string _genericType2;
        private string _genericType3;

        #region Properties

        public string Name
        {
            get { return _name; }
        }

        public int Index
        {
            get { return _index; }
        }

        public string Type
        {
            get { return CSharpFormattingTools.ToCTSType(_type); }
            set { _type = value; }
        }

        public string DeclaredType
        {
            get { return CSharpFormattingTools.ToCTSType(_declaredType); }
        }

        public bool IsLocal
        {
            get { return _isLocal; }
        }

        public bool IsStatic
        {
            get { return _isStatic; }
        }

        public bool IsArray
        {
            get { return _isArray; }
        }

        public string ArrayType
        {
            get { return CSharpFormattingTools.ToCTSType(_arrayType); }
        }

        public bool IsGeneric
        {
            get { return _isGeneric; }
        }

        public int GenericOrder
        {
            get { return _genericOrder; }
        }

        public string GenericType1
        {
            get { return CSharpFormattingTools.ToCTSType(_genericType1); }
        }

        public string GenericType2
        {
            get { return CSharpFormattingTools.ToCTSType(_genericType2); }
        }

        public string GenericType3
        {
            get { return CSharpFormattingTools.ToCTSType(_genericType3); }
        }

        #endregion

        #region Constructors

        public Variable(string name)
        {
            _name = name;
        }

        public Variable(string name, int index, bool isLocal, bool isStatic)
        {
            _name = name;
            _index = index;
            _isLocal = isLocal;
            _isStatic = isStatic;
        }

        public Variable(string name, string declaredType)
        {
            _name = name;
            _declaredType = declaredType;
            _type = _declaredType;
        }

        public Variable(string name, string declaredType, bool isStatic)
        {
            _name = name;
            _declaredType = declaredType;
            _type = _declaredType;
            _isStatic = isStatic;
        }

        public Variable(string name, int index, string declaredType)
        {
            _name = name;
            _index = index;
            _declaredType = declaredType;
            _type = _declaredType;
        }

        #endregion

        #region Initialization

        public void Setup(bool isArray, string type)
        {
            _isArray = isArray;
            _isGeneric = false;
            _genericOrder = 0;

            if (isArray)
            {
                _type = "System.Array";
                _declaredType = _type;
                _arrayType = type;
            }
            else
            {
                _type = type;
                _declaredType = _type;
            }
        }

        public void SetupGeneric1(string type1,
            string type2)
        {
            _isArray = false;
            _isGeneric = true;
            _genericOrder = 1;
            _genericType1 = type2;
            _type = type1 + "`1";
            _declaredType = _type;
        }

        public void SetupGeneric2(string type1,
            string type2, string type3)
        {
            _isArray = false;
            _isGeneric = true;
            _genericOrder = 2;
            _genericType1 = type2;
            _genericType2 = type3;
            _type = type1 + "`2";
            _declaredType = _type;
        }

        public void SetupGeneric3(string type1,
            string type2, string type3, string type4)
        {
            _isArray = false;
            _isGeneric = true;
            _genericOrder = 3;
            _genericType1 = type2;
            _genericType2 = type3;
            _genericType3 = type4;
            _type = type1 + "`3";
            _declaredType = _type;
        }

        #endregion

        #region Type Formatting

        public string GetVariableType()
        {
            if (_isArray)
            {
                return String.Format("{0}[]", ArrayType);
            }
            else if (_isGeneric)
            {
                string t = Type.Substring(0, Type.Length - 2);

                switch (_genericOrder)
                {
                    case 1:
                        return String.Format("{0}<{1}>", t,
                            GenericType1);
                    case 2:
                        return String.Format("{0}<{1}, {2}>", t,
                            GenericType1, GenericType2);
                    case 3:
                        return String.Format("{0}<{1}, {2}, {3}>", t,
                            GenericType1, GenericType2, GenericType3);
                    default:
                        return String.Format("{0}<>", t);
                }
            }
            else
            {
                return Type;
            }
        }

        public string GetVariableCollectionType(string entity)
        {
            /*
             * For arrays and generics we return the item that
             * the object is a collection of.
             */

            if (_isArray)
                return ArrayType;
            else if (_isGeneric)
            {
                /*
                 * This is very fuzzy and is really designed to
                 * cope with Lists and Dictionaries only. Our index
                 * enumeration system is pretty crap anyway so
                 * this is hardly likely to make it any worse.
                 */

                if (GenericOrder == 1)
                    return GenericType1;    // As in a List
                else if (GenericOrder == 2)
                    return GenericType2;    // As in a Dictionary

                return GenericType3;        // Er....
            }
            else
            {
                /*
                 * For anything other than an array or generic
                 * we need to swap in the base type and append
                 * 'Item'. This will be for 'old-style' collections
                 * such as StringCollection.
                 */

                string[] split = entity.Split('.');
                split[0] = _type;

                return String.Join(".", split) + ".Item";
            }
        }

        #endregion
    }
}
