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
    /// Represents a simple method with no parameters or return type.
    /// </summary>
    public delegate void MessageHandler();

    /// <summary>
    /// Represents an action that can be performed by a document.
    /// </summary>
    /// <returns>True if the action completed successfully.</returns>
    public delegate bool ActionHandler();

    /// <summary>
    /// Represents a method returning the state of an action that can be performed
    /// by a document. Used to enable or disable UI elements such as menu items when a document
    /// becomes active.
    /// </summary>
    /// <returns>True if the action is enabled.</returns>
    public delegate bool ActionStateHandler();

    /// <summary>
    /// Represents a method used to return a new document.
    /// </summary>
    /// <returns>A document.</returns>
    public delegate IDockContent NewDocumentHandler();

    /// <summary>
    /// Represents a method used to open a document.
    /// </summary>
    /// <param name="path">The path of the document to be opened.</param>
    /// <param name="readOnly">Open the document read-only.</param>
    /// <returns>The document or null if the load fails.</returns>
    public delegate IDockContent OpenDocumentHandler(string path, bool readOnly);

    /// <summary>
    /// Represents a method called during the loading of a document to modify the load
    /// process.
    /// </summary>
    /// <param name="documentType">The doument type of the document to be loaded.</param>
    /// <returns>True if loading is to proceed. False if it is to be cancelled.</returns>
    public delegate bool DocumentPreLoadHandler(DocumentType documentType);

    /// <summary>
    /// Represents a method that returns a filename filter for the file Open and Save dialogs.
    /// </summary>
    /// <returns>A filter string.</returns>
    public delegate string DocumentFilterHandler();

    /// <summary>
    /// Represents a method that returns an option page for the main options editor.
    /// </summary>
    /// <returns>An options page.</returns>
    public delegate OptionsPage OptionsPageFactoryHandler();

    /// <summary>
    /// Represents a method that returns a form.
    /// </summary>
    /// <returns>A form.</returns>
    public delegate Form FormFactoryHandler();
 
    /// <summary>
    /// Represents a method that returns a persistence manager.
    /// </summary>
    /// <param name="key">The persistence sub key.</param>
    /// <returns>An persistence manager object.</returns>
    public delegate IPersistenceManager PersistenceManagerFactoryHandler(string key);
    
    /// <summary>
    /// Represents a method that can update the control collection of a form.
    /// </summary>
    /// <param name="controls">The form's control collection.</param>
    public delegate void FormControlUpdateHandler(
        Control.ControlCollection controls);

    /// <summary>
    /// Represents a method that can update the pages of the options editor.
    /// </summary>
    /// <param name="pages">The option editor's page collection.</param>
    public delegate void OptionsFormUpdateHandler(Dictionary<String, OptionsPage> pages);
}
