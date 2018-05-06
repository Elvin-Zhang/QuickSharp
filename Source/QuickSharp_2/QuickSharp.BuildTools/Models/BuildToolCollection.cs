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
using System.Linq;
using QuickSharp.Core;

namespace QuickSharp.BuildTools
{
    /// <summary>
    /// A collection of build tools registered with the build tool system.
    /// </summary>
    public class BuildToolCollection
    {
        /**********************************************************************
         * BUILD TOOL COLLECTION
         * 
         * Build tools are mapped to a document type and an action. Each
         * document type can have none, one or many tools associated with a
         * particular action (e.g. compile). For a particular document type 
         * and action only one tool can be in use at any time. This is the
         * selected tool. BuildToolCollection maintains two lists: the full tool
         * list from which the selected tool can be chosen and a list of
         * selected tools. For each type/action the selected tool is the one
         * called when the user initiates the action. Actions are referred to
         * using upper case text strings (e.g. "COMPILE") to simplify extension
         * by additional plugins.
         **********************************************************************/

        private Dictionary<String, BuildTool> _tools;
        private Dictionary<String, DocumentTypeAction> _selectedTools;

        /// <summary>
        /// Create a BuildToolCollection.
        /// </summary>
        public BuildToolCollection()
        {
            _tools = new Dictionary<String, BuildTool>();
            _selectedTools = new Dictionary<String, DocumentTypeAction>();
        }

        /// <summary>
        /// Duplicate the build tool collection.
        /// </summary>
        /// <returns></returns>
        public BuildToolCollection Clone()
        {
            BuildToolCollection collection = new BuildToolCollection();

            Dictionary<String, BuildTool> tools =
                new Dictionary<String, BuildTool>();

            foreach (string key in _tools.Keys)
                tools[key] = _tools[key].Clone();

            Dictionary<String, DocumentTypeAction> selectedTools = 
                new Dictionary<String, DocumentTypeAction>();

            foreach (string key in _selectedTools.Keys)
                selectedTools[key] = _selectedTools[key].Clone();

            collection.Tools = tools;
            collection.SelectedTools = selectedTools;

            return collection;
        }

        #region Properties

        /// <summary>
        /// A dictionary of the tools in the collection indexed by tool ID.
        /// </summary>
        public Dictionary<String, BuildTool> Tools
        {
            get { return _tools; }
            set { _tools = value; }
        }

        /// <summary>
        /// A dictionary of the tools currently selected for each
        /// document type/build action combination, indexed by tool ID.
        /// </summary>
        public Dictionary<String, DocumentTypeAction> SelectedTools
        {
            get { return _selectedTools; }
            set { _selectedTools = value; }
        }

        #endregion

        #region Build Tool Management

        /// <summary>
        /// Add a tool to the collection.
        /// </summary>
        /// <param name="tool">A BuildTool instance.</param>
        public void AddTool(BuildTool tool)
        {
            _tools[tool.Id] = tool;
        }

        /// <summary>
        /// Get a tool by ID.
        /// </summary>
        /// <param name="id">The tool's ID.</param>
        /// <returns>A BuildTool instance.</returns>
        public BuildTool GetTool(string id)
        {
            if (_tools.ContainsKey(id))
                return _tools[id];
            else
                return null;
        }

        /// <summary>
        /// Delete a tool by ID.
        /// </summary>
        /// <param name="id">The tool's ID.</param>
        public void DeleteTool(string id)
        {
            _tools.Remove(id);
            _selectedTools.Remove(id);
        }

        /// <summary>
        /// Remove all the tools from the collection.
        /// </summary>
        public void ClearAll()
        {
            _tools.Clear();
            _selectedTools.Clear();
        }

        /// <summary>
        /// Get all the tools registered for a document type.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <returns>A dictionary of the tools indexed by ID.</returns>
        public Dictionary<String, BuildTool> GetTools(
            DocumentType documentType)
        {
            return GetTools(documentType, null);
        }

        /// <summary>
        /// Get all the tools registered for a document
        /// type/build action combination.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <param name="action">The build action.</param>
        /// <returns>A dictionary of the tools indexed by ID.</returns>
        public Dictionary<String, BuildTool> GetTools(
            DocumentType documentType, string action)
        {
            Dictionary<String, BuildTool> tools =
                new Dictionary<String, BuildTool>();

            foreach (string id in _tools.Keys)
                if (documentType.Matches(_tools[id].DocumentType) &&
                    (action == null || action == _tools[id].Action))
                    tools[id] = _tools[id];

            return tools;
        }

        /// <summary>
        /// Get the number of tools registered for a document type.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <returns>The number of tools.</returns>
        public int GetToolCount(
            DocumentType documentType)
        {
            return GetToolCount(documentType, null);
        }

        /// <summary>
        /// Get the number of tools registered for a document
        /// type/build action combination.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <param name="action">The build action.</param>
        /// <returns>The number of tools.</returns>
        public int GetToolCount(
            DocumentType documentType, string action)
        {
            Dictionary<String, BuildTool> tools =
                GetTools(documentType, action);
            
            return tools.Keys.Count;
        }

        /// <summary>
        /// Determine if tools have been registered for a document
        /// type/build action combination.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <param name="action">The build action.</param>
        /// <returns>True if tools are available.</returns>
        public bool ToolsAreAvailable(
            DocumentType documentType, string action)
        {
            Dictionary<String, BuildTool> tools =
                GetTools(documentType, action);

            foreach (string id in tools.Keys)
                if (tools[id].BuildCommand != null)
                    return true;

            return false;
        }

        /// <summary>
        /// Get a list of the document types for which tools have been registered.
        /// </summary>
        /// <returns>A list of document types.</returns>
        public List<String> GetDocumentTypes()
        {
            List<String> list = new List<String>();

            foreach (string id in _tools.Keys)
            {
                string documentType =
                    _tools[id].DocumentType.ToString();

                if (!list.Contains(documentType))
                    list.Add(documentType);
            }

            return list;
        }

        #endregion

        #region Tool Selection

        /// <summary>
        /// Select a tool by ID. Selecting a tool makes it the currently
        /// selected tool for a document type/build action combination. Any
        /// previously selected tool is deselected.
        /// </summary>
        /// <param name="id">The tool's ID.</param>
        public void SelectTool(string id)
        {
            if (!_tools.ContainsKey(id)) return;
            SelectTool(_tools[id]);
        }

        /// <summary>
        /// Select a build tool. Selecting a tool makes it the currently
        /// selected tool for a document type/build action combination. Any
        /// previously selected tool is deselected.
        /// </summary>
        /// <param name="tool">A BuildTool instance.</param>
        public void SelectTool(BuildTool tool)
        {
            DocumentType documentType = tool.DocumentType;
            string action = tool.Action;

            foreach (string id in _selectedTools.Keys)
            {
                if (_selectedTools[id].DocumentType.Matches(documentType) &&
                    _selectedTools[id].Action == action)
                {
                    _selectedTools.Remove(id);
                    break;
                }
            }

            _selectedTools[tool.Id] = new DocumentTypeAction()
                { DocumentType = tool.DocumentType, Action = tool.Action };
        }

        /// <summary>
        /// Get the currently selected tool for a document
        /// type/build action combination.
        /// </summary>
        /// <param name="type">The document type.</param>
        /// <param name="action">The build action.</param>
        /// <returns>A BuildTool instance.</returns>
        public BuildTool GetSelectedTool(string type, string action)
        {
            return GetSelectedTool(new DocumentType(type), action);
        }

        /// <summary>
        /// Get the currently selected tool for a document
        /// type/buid action combination.
        /// </summary>
        /// <param name="type">The document type.</param>
        /// <param name="action">The build action.</param>
        /// <returns>A BuildTool instance.</returns>
        public BuildTool GetSelectedTool(DocumentType type, string action)
        {
            foreach (string id in _selectedTools.Keys)
                if (_selectedTools[id].DocumentType.Matches(type) &&
                    _selectedTools[id].Action == action)
                    return _tools[id];

            return null;
        }

        /// <summary>
        /// Determine if a build tool is selected.
        /// </summary>
        /// <param name="tool">A BuildTool instance.</param>
        /// <returns>True if the tool is selected.</returns>
        public bool ToolIsSelected(BuildTool tool)
        {
            return _selectedTools.ContainsKey(tool.Id);
        }

        /// <summary>
        /// Determine if a selected tool is available for a document
        /// type/build tool action combination.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <param name="action">The build action.</param>
        /// <returns>True if a selected tool is available.</returns>
        public bool SelectedToolIsAvailable(DocumentType documentType, string action)
        {
            BuildTool selectedTool =
                GetSelectedTool(documentType, action);
            
            if (selectedTool == null) return false;
            return (selectedTool.BuildCommand != null);
        }

        #endregion

        #region Sorting

        /// <summary>
        /// Sort the tool collection into document type/build action order.
        /// </summary>
        public void SortTools()
        {
            List<String> keys = _tools.Keys.ToList<String>();

            keys.Sort(CompareBuildTools);

            Dictionary<String, BuildTool> newTools = 
                new Dictionary<String, BuildTool>();

            foreach (string id in keys)
                newTools.Add(id, _tools[id]);

            _tools = newTools;
        }

        private int CompareBuildTools(string id1, string id2)
        {
            BuildTool t1 = _tools[id1];
            BuildTool t2 = _tools[id2];

            if (t1.DocumentType.Matches(t2.DocumentType))
            {
                if (t1.Action == t2.Action)
                {
                    return t1.DisplayName.CompareTo(t2.DisplayName);
                }
                else
                {
                    return t1.Action.CompareTo(t2.Action);
                }
            }
            else
            {
                return t1.DocumentType.CompareTo(t2.DocumentType);
            }
        }

        #endregion
    }
}
