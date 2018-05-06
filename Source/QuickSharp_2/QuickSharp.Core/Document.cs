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
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace QuickSharp.Core
{
    /// <summary>
    /// The base class for documents managed by the QuickSharp document
    /// management system. A document is a form used to present and manage
    /// data within the application. Document presentation is managed by the docking
    /// window system. Documents often represent files (but don't have to) and are
    /// created using new and open document handlers. Document operations are
    /// controlled by action and actionstate handlers.
    /// </summary>
    public class Document : DockContent
    {
        private Dictionary<String, ActionHandler> _actionHandlers;
        private Dictionary<String, ActionStateHandler> _actionStateHandlers;
        
        /// <summary>
        /// The document's file name.
        /// </summary>
        protected string documentFileName;

        /// <summary>
        /// The document's file path.
        /// </summary>
        protected string documentFilePath;

        /// <summary>
        /// The document's file timestamp.
        /// </summary>
        protected DateTime documentTimeStamp = DateTime.Now;

        /// <summary>
        /// Create a new document.
        /// </summary>
        public Document()
        {
            _actionHandlers =
                new Dictionary<String, ActionHandler>();

            _actionStateHandlers =
                new Dictionary<String, ActionStateHandler>();

            /*
             * Default to document docking only.
             */

            DockAreas = DockAreas.Document;

            /*
             * The docking window manager needs to know the name
             * of the document to restore if session reloading is
             * enabled. We create a callback to return the file path
             * of the document contents so that it can be reloaded if
             * we opt to have documents reloaded from the previous
             * session.
             */

            DockHandler.GetPersistStringCallback =
                new GetPersistStringCallback(GetPersistString);
        }

        /// <summary>
        /// The document filename.
        /// </summary>
        public virtual string FileName
        {
            get { return documentFileName; }
            set { documentFileName = value; }
        }

        /// <summary>
        /// The document file path. This is null for an unsaved document.
        /// </summary>
        public virtual string FilePath
        {
            get { return documentFilePath; }
            set { documentFilePath = value; }
        }

        /// <summary>
        /// The latest saved time of the document.
        /// </summary>
        public virtual DateTime FileTimeStamp
        {
            get { return documentTimeStamp; }
            set { documentTimeStamp = value; }
        }

        /// <summary>
        /// Get the document content. Override in derived classes to return the
        /// actual data content of the document.
        /// </summary>
        /// <returns></returns>
        public virtual object GetContent()
        {
            return null;
        }

        /// <summary>
        /// Set the document content. Override in derived classes to set the
        /// actual data content of the document.
        /// </summary>
        /// <param name="content"></param>
        public virtual void SetContent(object content)
        {
        }

        /// <summary>
        /// Set a location within the document. Override in derived classes to
        /// set an actual location within the document.
        /// </summary>
        /// <param name="x">The X coordinate of the location.</param>
        /// <param name="y">The Y coordinate of the location.</param>
        public virtual void SetLocation(int x, int y)
        {
        }

        /// <summary>
        /// Determine if the document type allows multiple loading
        /// of the same document. This is false by default.
        /// </summary>
        /// <returns>True if multiple loading is allowed.</returns>
        public virtual bool AllowDuplicates()
        {
            return false;
        }

        /// <summary>
        /// Get a string representing the document for storing in the
        /// docking window manager configuration file. Override in derived
        /// classes as required. This defaults to the file path of the document.
        /// Return an empty string to prevent the document from being reloaded
        /// at the start of a new session.
        /// </summary>
        /// <returns></returns>
        protected override string GetPersistString()
        {
            if (FilePath == null)
                return String.Empty;
            else
                return FilePath;
        }

        #region Action Delegate Handlers

        /*
         * ActionState Handlers
         * 
         * ActionState handlers allow a document to register callback
         * methods with the MainForm. Each callback responds to a
         * particular action available from a MainForm UI element
         * (menu and toolbar). When a document becomes active the
         * mainform calls each method registered for that document to
         * determine the state of each available UI element.
         * 
         * If the callback returns false or no callback has been
         * registered the corresponding UI element will be disabled.
         */

        /// <summary>
        /// Register an ActionState handler for the document. ActionState
        /// handlers allow a document to register callback methods with document
        /// management system. Each callback responds to a particular action
        /// available from a MainForm UI element (menu and toolbar). When a
        /// document becomes active the document manager calls each method
        /// registered for that document to determine the state of each available
        /// UI element. If the callback returns false or no callback has been
        /// registered the corresponding UI element will be disabled.
        /// </summary>
        /// <param name="action">The name of the action.</param>
        /// <param name="handler">The ActionState event handler.</param>
        public void RegisterActionStateHandler(
            string action, ActionStateHandler handler)
        {
            _actionStateHandlers[action] = handler;
        }

        /*
         * Action Handlers
         * 
         * As for ActionState handlers, Action handlers allow callback methods
         * to be associated with the UI elements in the MainForm. If a
         * callback is available for a particular action when the document
         * is active, invoking the action will invoke the callback.
         */

        /// <summary>
        /// Register an Action handler for the document. Action handlers allow
        /// callback methods to be associated with the UI elements in the MainForm.
        /// If a callback is available for a particular action when the document
        /// is active, invoking the action will invoke the callback.
        /// </summary>
        /// <param name="action">The name of the action.</param>
        /// <param name="handler">The Action event handler.</param>
        public void RegisterActionHandler(
            string action, ActionHandler handler)
        {
            _actionHandlers[action] = handler;
        }

        #endregion

        #region Menu Access

        /*
         * The MainForm will call these methods to determine the
         * availability of the actions associated with its UI
         * elements (menu and toolbar) via the document's
         * actionstate handlers.
         */

        /// <summary>
        /// Determine if a menu item is enabled. This uses the registered
        /// ActionState handlers to determine if the menu item should be
        /// enabled when the document is active.
        /// </summary>
        /// <param name="menuItem">The menu item.</param>
        /// <returns>True if the menu is enabled.</returns>
        public bool IsMenuItemEnabled(ToolStripMenuItem menuItem)
        {
            return IsActionEnabled(menuItem.Name);
        }

        /// <summary>
        /// Determine if a toolbar button is enabled. This uses the registered
        /// ActionState handlers to determine if the button should be
        /// enabled when the document is active.
        /// </summary>
        /// <param name="button">The toolbar button.</param>
        /// <returns>True if the button is enabled.</returns>
        /// 
        public bool IsToolbarButtonEnabled(ToolStripButton button)
        {
            return IsActionEnabled(button.Name);
        }

        /// <summary>
        /// Determine if an action should be enabled.
        /// </summary>
        /// <param name="actionName">The action name.</param>
        /// <returns>True if the action should be enabled. False if not or
        /// the action doesn't exist.</returns>
        public bool IsActionEnabled(string actionName)
        {
            /*
             * Return false if the action doesn't exist.
             * Cannot enable an action it can't do.
             */

            if (!_actionStateHandlers.ContainsKey(actionName))
                return false;

            ActionStateHandler d = _actionStateHandlers[actionName];
            if (d != null)
                return d();
            else
                return false;
        }

        /*
         * The MainForm will call these methods to perform the
         * actions associated with its UI elements (menu and
         * toolbar) via the document's action handlers.
         */

        /// <summary>
        /// Perform the action associated with the menu item for the
        /// current document.
        /// </summary>
        /// <param name="menuItem">The menu item.</param>
        /// <returns>The result of the action or true if it doesn't exist
        /// (cannot fail to perfom something it can't do).</returns>
        public bool PerformMenuAction(ToolStripMenuItem menuItem)
        {
            return PerformAction(menuItem.Name);
        }

        /// <summary>
        /// Perform the action associated with the toolbar button
        /// for the current document.
        /// </summary>
        /// <param name="button">The toolbar button.</param>
        /// <returns>The result of the action or true if it doesn't exist
        /// (cannot fail to perfom something it can't do).</returns>
        public bool PerformToolbarAction(ToolStripButton button)
        {
            return PerformAction(button.Name);
        }

        /// <summary>
        /// Perform the specified action.
        /// </summary>
        /// <param name="actionName">The name of the action.</param>
        /// <returns>The result of the action or true if it doesn't exist
        /// (cannot fail to perfom something it can't do).</returns>
        public bool PerformAction(string actionName)
        {
            /*
             * Return true if the action doesn't exist.
             * Cannot fail to perfom something it can't do.
             */

            if (!_actionHandlers.ContainsKey(actionName))
                return true;

            /* 
             * Return false if action is recognized but delegate not found.
             */

            ActionHandler d = _actionHandlers[actionName];
            if (d != null)
                return d();
            else
                return false;
        }

        #endregion
    }
}
