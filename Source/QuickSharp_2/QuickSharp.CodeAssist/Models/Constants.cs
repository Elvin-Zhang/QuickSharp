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

namespace QuickSharp.CodeAssist
{
    /// <summary>
    /// Provide access to the constants used within the plugin.
    /// </summary>
    public class Constants
    {
        public const string PLUGIN_NAME = "QuickSharp.CodeAssist";
        public const int LOOKUP_WINDOW_HEIGHT = 180;

        // Replacement characters for insertion text and cursor position
        public const string INSERTION_TEMPLATE_TEXT_PLACEHOLDER = "^`^";
        public const string INSERTION_TEMPLATE_CPOS_PLACEHOLDER = "`¬`";

        // UI Elements
        public const string UI_EDIT_MENU_CODE_ASSIST = "UI_EDIT_MENU_CODE_ASSIST";
        public const string ASSEMBLY = "ASSEMBLY";
        public const string CLASS = "CLASS";
        public const string CLASS_FRIEND = "CLASS_FRIEND";
        public const string CLASS_PRIVATE = "CLASS_PRIVATE";
        public const string CLASS_PROTECTED = "CLASS_PROTECTED";
        public const string CLASS_SEALED = "CLASS_SEALED";
        public const string CONSTANT = "CONSTANT";
        public const string CONSTANT_FRIEND = "CONSTANT_FRIEND";
        public const string CONSTANT_PRIVATE = "CONSTANT_PRIVATE";
        public const string CONSTANT_PROTECTED = "CONSTANT_PROTECTED";
        public const string CONSTANT_SEALED = "CONSTANT_SEALED";
        public const string DELEGATE = "DELEGATE";
        public const string DELEGATE_FRIEND = "DELEGATE_FRIEND";
        public const string DELEGATE_PRIVATE = "DELEGATE_PRIVATE";
        public const string DELEGATE_PROTECTED = "DELEGATE_PROTECTED";
        public const string DELEGATE_SEALED = "DELEGATE_SEALED";
        public const string ENUM = "ENUM";
        public const string ENUM_FRIEND = "ENUM_FRIEND";
        public const string ENUM_PROTECTED = "ENUM_PROTECTED";
        public const string ENUM_SEALED = "ENUM_SEALED";
        public const string EVENT = "EVENT";
        public const string EVENT_FRIEND = "EVENT_FRIEND";
        public const string EVENT_PRIVATE = "EVENT_PRIVATE";
        public const string EVENT_PROTECTED = "EVENT_PROTECTED";
        public const string EVENT_SEALED = "EVENT_SEALED";
        public const string EXCEPTION = "EXCEPTION";
        public const string EXCEPTION_PRIVATE = "EXCEPTION_PRIVATE";
        public const string EXCEPTION_FRIEND = "EXCEPTION_FRIEND";
        public const string EXCEPTION_PROTECTED = "EXCEPTION_PROTECTED";
        public const string EXCEPTION_SEALED = "EXCEPTION_SEALED";
        public const string FIELD = "FIELD";
        public const string FIELD_FRIEND = "FIELD_FRIEND";
        public const string FIELD_PRIVATE = "FIELD_PRIVATE";
        public const string FIELD_PROTECTED = "FIELD_PROTECTED";
        public const string FIELD_SEALED = "FIELD_SEALED";
        public const string FILE_REF = "FILE_REF";
        public const string INTERFACE = "INTERFACE";
        public const string INTERFACE_FRIEND = "INTERFACE_FRIEND";
        public const string INTERFACE_PRIVATE = "INTERFACE_PRIVATE";
        public const string INTERFACE_PROTECTED = "INTERFACE_PROTECTED";
        public const string INTERFACE_SEALED = "INTERFACE_SEALED";
        public const string METHOD = "METHOD";
        public const string METHODOVERLOAD = "METHODOVERLOAD";
        public const string METHODOVERLOAD_FRIEND = "METHODOVERLOAD_FRIEND";
        public const string METHODOVERLOAD_PRIVATE = "METHODOVERLOAD_PRIVATE";
        public const string METHODOVERLOAD_PROTECTED = "METHODOVERLOAD_PROTECTED";
        public const string METHODOVERLOAD_SEALED = "METHODOVERLOAD_SEALED";
        public const string METHOD_FRIEND = "METHOD_FRIEND";
        public const string METHOD_PRIVATE = "METHOD_PRIVATE";
        public const string METHOD_PROTECTED = "METHOD_PROTECTED";
        public const string METHOD_SEALED = "METHOD_SEALED";
        public const string METHOD_EXTENSION = "METHOD_EXTENSION";
        public const string NAMESPACE = "NAMESPACE";
        public const string NAMESPACE_PRIVATE = "NAMESPACE_PRIVATE";
        public const string NAMESPACE_PROTECTED = "NAMESPACE_PROTECTED";
        public const string NAMESPACE_FRIEND = "NAMESPACE_FRIEND";
        public const string NAMESPACE_SEALED = "NAMESPACE_SEALED";
        public const string OPERATOR = "OPERATOR";
        public const string OPERATOR_FRIEND = "OPERATOR_FRIEND";
        public const string OPERATOR_PRIVATE = "OPERATOR_PRIVATE";
        public const string OPERATOR_PROTECTED = "OPERATOR_PROTECTED";
        public const string OPERATOR_SEALED = "OPERATOR_SEALED";
        public const string PROPERTIES = "PROPERTIES";
        public const string PROPERTIES_FRIEND = "PROPERTIES_FRIEND";
        public const string PROPERTIES_PRIVATE = "PROPERTIES_PRIVATE";
        public const string PROPERTIES_PROTECTED = "PROPERTIES_PROTECTED";
        public const string PROPERTIES_SEALED = "PROPERTIES_SEALED";
        public const string STRUCTURE = "STRUCTURE";
        public const string STRUCTURE_FRIEND = "STRUCTURE_FRIEND";
        public const string STRUCTURE_PRIVATE = "STRUCTURE_PRIVATE";
        public const string STRUCTURE_PROTECTED = "STRUCTURE_PROTECTED";
        public const string STRUCTURE_SEALED = "STRUCTURE_SEALED";
        public const string TABLE = "TABLE";
        public const string TEMPLATE = "TEMPLATE";
        public const string TEMPLATE_FRIEND = "TEMPLATE_FRIEND";
        public const string TEMPLATE_PRIVATE = "TEMPLATE_PRIVATE";
        public const string TEMPLATE_PROTECTED = "TEMPLATE_PROTECTED";
        public const string TEMPLATE_SEALED = "TEMPLATE_SEALED";
        public const string TYPE = "TYPE";
        public const string TYPEDEF = "TYPEDEF";
        public const string TYPEDEF_FRIEND = "TYPEDEF_FRIEND";
        public const string TYPEDEF_PRIVATE = "TYPEDEF_PRIVATE";
        public const string TYPEDEF_PROTECTED = "TYPEDEF_PROTECTED";
        public const string TYPEDEF_SEALED = "TYPEDEF_SEALED";
        public const string TYPE_FRIEND = "TYPE_FRIEND";
        public const string TYPE_PRIVATE = "TYPE_PRIVATE";
        public const string TYPE_PROTECTED = "TYPE_PROTECTED";
        public const string TYPE_SEALED = "TYPE_SEALED";
        public const string UNION = "UNION";
        public const string UNION_PRIVATE = "UNION_PRIVATE";
        public const string UNION_FRIEND = "UNION_FRIEND";
        public const string UNION_PROTECTED = "UNION_PROTECTED";
        public const string UNION_SEALED = "UNION_SEALED";
        public const string VALUETYPE = "VALUETYPE";
        public const string VALUETYPE_PRIVATE = "VALUETYPE_PRIVATE";
        public const string VALUETYPE_FRIEND = "VALUETYPE_FRIEND";
        public const string VALUETYPE_PROTECTED = "VALUETYPE_PROTECTED";
        public const string VALUETYPE_SEALED = "VALUETYPE_SEALED";
        public const string VIEW = "VIEW";
        public const string WEBCONTROL = "WEBCONTROL";
        public const string FIELD_ALIAS = "FIELD_ALIAS";
        public const string TABLE_ALIAS = "TABLE_ALIAS";
        public const string VIEW_ALIAS = "VIEW_ALIAS";
    }
}
